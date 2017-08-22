using System;
using System.ComponentModel.Composition;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Fictive
{
    //Способ формирования значений фиктивного простого источника
    internal enum ValueForming
    {
        Uniform, //По порядку от начала периода
        Time //Исходя из времени
    }

    //-----------------------------------------------------------------------------------------------------
    //Фиктивный простой тестовый источник, реализация без чтения по блокам и OleDb
    [Export(typeof(Provider))]
    [ExportMetadata("Code", "FictiveSimpleSource")]
    public class FictiveSimpleSource : ListSource
    {
        //Код
        public override string Code { get { return "FictiveSimpleSource"; } }
        //Свойства
        protected override void ReadInf(DicS<string> dic)
        {
            Label = dic["Label"];
            ValueForming = dic["ValueForming"] == "Time" ? ValueForming.Time : ValueForming.Uniform;
        }
        internal string Label { get; private set; }
        //Способ формирования значений
        internal ValueForming ValueForming { get; private set; }
        
        //Диапазон источника
        protected override TimeInterval GetTimeProvider()
        {
            return new TimeInterval(new DateTime(2000, 1, 1), new DateTime(2100, 1, 1));
        }

        //Словарь объектов, ключи - номера
        private readonly DicI<FictiveOut> _outs = new DicI<FictiveOut>();
        internal DicI<FictiveOut> Outs { get { return _outs; } }

        //Добавить объект в провайдер
        protected override ProviderOut AddOut(ProviderSignal sig)
        {
            var num = sig.Inf.GetInt("NumObject");
            if (_outs.ContainsKey(num)) return _outs[num];
            return _outs.Add(num, new FictiveOut(this, sig.Inf.GetInt("ValuesInterval")));
        }

        //Очистка списков объектов
        protected override void ClearOuts()
        {
            _outs.Clear();
        }

        //Подготока источника
        protected override void PrepareProvider()
        {
            foreach (var ob in _outs.Values)
                ob.IsInitialized = true;
        }
        
        //Создание фабрики ошибок
        protected override IMomErrFactory MakeErrFactory()
        {
            var factory = new MomErrFactory(ProviderConnect.Code, MomErrType.Source);
            factory.AddGoodDescr(0);
            factory.AddDescr(1, "Предупреждение", ErrQuality.Warning);
            factory.AddDescr(2, "Ошибка");
            return factory;
        }

        //Чтение списков равномерных значений
        protected override ValuesCount ReadCut()
        {
            var vc = new ValuesCount();
            foreach (var ob in _outs.Values)
            {
                if (ValueForming == ValueForming.Uniform)
                    vc.WriteCount += ob.MakeUniformValues(PeriodBegin, PeriodBegin, true);
                else vc.WriteCount += ob.MakeTimeValues(PeriodBegin, PeriodBegin, true);
            }
            return vc;
        }
        protected override ValuesCount ReadChanges(DateTime beg, DateTime en)
        {
            if (_makeNextError)
            {
                _makeNextError = false;
                return new ValuesCount(VcStatus.Fail);
            }

            var vc = new ValuesCount();
            foreach (var ob in _outs.Values)
            {
                if (ValueForming == ValueForming.Uniform)
                    vc.WriteCount += ob.MakeUniformValues(beg, en, true);
                else vc.WriteCount += ob.MakeTimeValues(beg, en, true);
            }
            return vc;
        }

        //В следующий запуск произойдет ошибка
        private bool _makeNextError;
        internal void MakeErrorOnTheNextReading()
        {
            _makeNextError = true;
        }
    }
}
