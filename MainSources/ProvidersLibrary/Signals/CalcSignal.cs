using System;
using System.Reflection;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Расчетный сигнал
    public class CalcSignal : Signal
    {
        public CalcSignal(SourceBase source, SourceSignal signal, string code, DataType dataType, string formula) 
            : base(source, code, dataType)
        {
            _signal = signal;
            ParseFormula(formula);
        }

        //Вычисление вынкции
        public Action Calculate { get; private set; }
        //Сигнал, на основе которого вычисляется значение
        private readonly SourceSignal _signal;
        //Дополнительные параметры
        private IMean[] _pars;

        //Разбор формулы заданной прямой польской записью
        //Формула имеет вид ИмяФункции;КоличествоПараметров;Параметр;Параметр;...
        private void ParseFormula(string formula)
        {
            var lexemes = formula.Split(';');
            MethodInfo met = typeof(CalcSignal).GetMethod(lexemes[0]);
            if (met != null) Calculate = (Action)Delegate.CreateDelegate(typeof(Action), met);
            _pars = new IMean[lexemes[1].ToInt()];
            for (int i = 0; i < _pars.Length; i++)
                _pars[i] = MFactory.NewMean(DataType.Integer, lexemes[i + 2]);
        }

        //Взятие бита, pars[0] - номер бита
        private void Bit()
        {
            int bit = _pars[0].Integer;
            var moms = _signal.MomList;
            for (int i = 0; i < moms.Count; i++)
                MList.AddMom(moms.Time(i), moms.Integer(i).GetBit(bit), moms.Error(i));
        }

        //Взятие битов и сложение по Or, pars - номера битов
        private void BitOr()
        {
            var moms = _signal.MomList;
            for (int i = 0; i < moms.Count; i++)
            {
                var v = moms.Integer(i);
                bool res = false;
                foreach (var par in _pars)
                    res |= v.GetBit(par.Integer);
                MList.AddMom(moms.Time(i), res, moms.Error(i));
            }
        }

        //Взятие битов и сложение по And, pars - номера битов
        private void BitAnd()
        {
            var moms = _signal.MomList;
            for (int i = 0; i < moms.Count; i++)
            {
                var v = moms.Integer(i);
                bool res = true;
                foreach (var par in _pars)
                    res &= v.GetBit(par.Integer);
                MList.AddMom(moms.Time(i), res, moms.Error(i));
            }
        }

        //Агрегация по равномерным сегментам
        //На входе функция с параметрами: текущий результат, список и обрабатываемый номер
        private void Agregate(Action<MomEdit, MomList, int> fun)
        {
            double seglen = _pars[0].Real, segshift = _pars[1].Real * seglen;
            var moms = _signal.MomList;
            var mlist = MFactory.NewList(DataType);
            //Добавляем в список границы сегментов
            var t = Source.PeriodBegin;
            int i = 0;
            while (t < Source.PeriodEnd)
            {
                while (i < moms.Count && moms.Time(i) <= t)
                {
                    if (moms.Time(i) > Source.PeriodBegin && moms.Time(i) <= Source.PeriodEnd)
                        mlist.AddMom(moms.Clone(i));
                    i++;
                }
                if (i > 0) i--;
                mlist.AddMom(moms.Clone(i, t));
                t = t.AddSeconds(seglen);
            }

            t = Source.PeriodBegin;
            i = 0;
            while (t < Source.PeriodEnd)
            {
                var me = new MomEdit(DataType, t.AddSeconds(segshift));
                if (fun.Method.Name == "AverageScalar")
                    me.Real = 0;
                else me.CopyValueFrom(mlist, i);
                t = t.AddSeconds(seglen);
                while (mlist.Time(i) < t)
                    fun(me, mlist, i++);
                MList.AddMom(me);
            }
        }

        //Агрегирующие функции по равномерным сегментам
        //pars[0] - длина сегмента в секундах, pars[1] - сдвиг итогового значения, по умолчанию 0
        
        //Среднее по равномерным сегментам, 
        private void Average() { Agregate(AverageScalar); }
        private void AverageScalar(MomEdit res, MomList mlist, int i)
        {
            res.Real += mlist.Real(i) * (mlist.Time(i+1).Subtract(mlist.Time(i)).TotalSeconds) /_pars[0].Real;
        }

        //Первое по равномерным сегментам, pars[0] - длина сегмента в секундах
        private void First() { Agregate(FirstScalar); }
        private void FirstScalar(MomEdit res, MomList mlist, int i) { }
        
        //Последнее по равномерным сегментам, pars[0] - длина сегмента в секундах
        private void Last() { Agregate( LastScalar); }
        private void LastScalar(MomEdit res, MomList mlist, int i)
        {
            res.CopyValueFrom(mlist, i);
            res.Error = mlist.Error(i);
        }

        //Минимум по равномерным сегментам, pars[0] - длина сегмента в секундах
        private void Min() { Agregate(MinScalar); }
        private void MinScalar(MomEdit res, MomList mlist, int i)
        {
            if (mlist.Mom(i).ValueLess(res))
            {
                res.CopyValueFrom(mlist, i);
                res.Error = mlist.Error(i);
            }
        }

        //Максимум по равномерным сегментам, pars[0] - длина сегмента в секундах
        private void Max() { Agregate(MaxScalar); }
        private void MaxScalar(MomEdit res, MomList mlist, int i)
        {
            if (res.ValueLess(mlist.Mom(i)))
            {
                res.CopyValueFrom(mlist, i);
                res.Error = mlist.Error(i);
            }
        }
    }
}