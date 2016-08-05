﻿using System;
using System.Reflection;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Расчетный сигнал
    public class CalcSignal : SourceSignal
    {
        public CalcSignal(string code, string codeObject, InitialSignal initialSignal, string formula)
            : base(initialSignal.Connect, code, codeObject)
        {
            _initialSignal = initialSignal;
            ParseFormula(formula);
        }

        //Вычисление функции, возвращает количество сформированных значений
        internal Action Calculate { get; private set; }
        //Сигнал, на основе которого вычисляется значение
        private readonly InitialSignal _initialSignal;

        //Дополнительные параметры
        private IMean[] _pars;

        //Разбор формулы заданной прямой польской записью
        //Формула имеет вид ИмяФункции;КоличествоПараметров;Параметр;Параметр;...
        private void ParseFormula(string formula)
        {
            var lexemes = formula.Split(';');
            var funName = lexemes[0];
            DefineDataType(funName);
            MethodInfo met = typeof(CalcSignal).GetMethod(funName);
            if (met != null) Calculate = (Action)Delegate.CreateDelegate(typeof(Action), this, met);
            _pars = new IMean[lexemes[1].ToInt()];
            for (int i = 0; i < _pars.Length; i++)
                _pars[i] = MFactory.NewMean(DataType.Real, lexemes[i + 2]);
        }

        //Определение типа данных возвращаемых значений
        private void DefineDataType(string funName)
        {
            if (funName.StartsWith("Bit")) DataType = DataType.Boolean;
            else if (funName == "Average") DataType = DataType.Real;
            else DataType = _initialSignal.DataType;
            MList = MFactory.NewList(DataType);
            MomList = new MomListReadOnly(MList);
        }

        //Взятие бита, pars[0] - номер бита
        private void Bit()
        {
            int bit = _pars[0].Integer;
            var moms = _initialSignal.MomList;
            for (int i = 0; i < moms.Count; i++)
                MList.AddMom(moms.Time(i), moms.Integer(i).GetBit(bit), moms.Error(i));
        }

        //Взятие битов и сложение по Or, pars - номера битов
        private void BitOr()
        {
            var moms = _initialSignal.MomList;
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
            var moms = _initialSignal.MomList;
            for (int i = 0; i < moms.Count; i++)
            {
                var v = moms.Integer(i);
                bool res = true;
                foreach (var par in _pars)
                    res &= v.GetBit(par.Integer);
                MList.AddMom(moms.Time(i), res, moms.Error(i));
            }
        }

        //Агрегирующие функции по равномерным сегментам
        //pars[0] - длина сегмента в секундах, pars[1] - сдвиг итогового значения, по умолчанию 0
        
        //Среднее по равномерным сегментам, 
        private void Average() { Agregate(AverageScalar); }
        private void AverageScalar(MomEdit res, MomList mlist, int i, DateTime t)
        {
            DateTime time = (i >= mlist.Count - 1 || mlist.Time(i + 1) > t) ? t : mlist.Time(i + 1);
            res.Real += mlist.Real(i) * (time.Subtract(mlist.Time(i)).TotalSeconds) /_pars[0].Real;
        }

        //Первое по равномерным сегментам, pars[0] - длина сегмента в секундах
        private void First() { Agregate(FirstScalar); }
        private void FirstScalar(MomEdit res, MomList mlist, int i, DateTime t) { }
        
        //Последнее по равномерным сегментам, pars[0] - длина сегмента в секундах
        private void Last() { Agregate( LastScalar); }
        private void LastScalar(MomEdit res, MomList mlist, int i, DateTime t)
        {
            res.CopyValueFrom(mlist, i);
            res.Error = mlist.Error(i);
        }

        //Минимум по равномерным сегментам, pars[0] - длина сегмента в секундах
        private void Min() { Agregate(MinScalar); }
        private void MinScalar(MomEdit res, MomList mlist, int i, DateTime t)
        {
            if (mlist.Mean(i).ValueLess(res))
            {
                res.CopyValueFrom(mlist, i);
                res.Error = mlist.Error(i);
            }
        }

        //Максимум по равномерным сегментам, pars[0] - длина сегмента в секундах
        private void Max() { Agregate(MaxScalar); }
        private void MaxScalar(MomEdit res, MomList mlist, int i, DateTime t)
        {
            if (res.ValueLess(mlist.Mean(i)))
            {
                res.CopyValueFrom(mlist, i);
                res.Error = mlist.Error(i);
            }
        }

        //Агрегация по равномерным сегментам
        //На входе функция с параметрами: текущий результат, список и обрабатываемый номер
        private void Agregate(Action<MomEdit, MomList, int, DateTime> fun)
        {
            double seglen = _pars[0].Real, segshift = _pars.Length < 2 ? 0 : _pars[1].Real * seglen;
            var moms = _initialSignal.MomList;
            if (moms.Count == 0) return;
            var mlist = MFactory.NewList(DataType);
            //Добавляем в список границы сегментов
            var t = Connect.PeriodBegin;
            int i = 0;
            while (t < Connect.PeriodEnd.AddMilliseconds(1))
            {
                while (i < moms.Count && moms.Time(i) < t)
                {
                    if (moms.Time(i) >= Connect.PeriodBegin && moms.Time(i) <= Connect.PeriodEnd)
                        mlist.AddMom(moms.Clone(i));
                    i++;
                }
                AddUniformMom(moms, mlist, i, t);
                t = t.AddSeconds(seglen);
            }
            if (i < moms.Count && moms.Time(i) == Connect.PeriodEnd)
                mlist.AddMom(moms.Clone(i));

            t = Connect.PeriodBegin;
            i = 0;
            while (t < Connect.PeriodEnd.AddMilliseconds(-1))
            {
                var me = new MomEdit(DataType, t.AddSeconds(segshift));
                if (fun.Method.Name == "AverageScalar")
                    me.Real = 0;
                else me.CopyValueFrom(mlist, i);
                t = t.AddSeconds(seglen);
                while (i < mlist.Count && mlist.Time(i) <= t)
                    fun(me, mlist, i++, t);
                if (i > 0 && mlist.Time(i-1) == t) i--;
                MList.AddMom(me);
            }
        }

        private void AddUniformMom(IMomListReadOnly fromList, MomList toList, int i, DateTime t)
        {
            if (i < fromList.Count && fromList.Time(i) == t) return;
            toList.AddMom(fromList.Clone(i == 0 ? 0 : i - 1, t));
        }
    }
}