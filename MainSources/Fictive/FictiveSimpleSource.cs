using System;
using System.ComponentModel.Composition;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Fictive
{
    //Фиктивный тестовый источник, реализация без чтения по блокам и OleDb
    [Export(typeof(BaseProvider))]
    [ExportMetadata("Code", "FictiveSimpleSource")]
    public class FictiveSimpleSource : BaseSource
    {
        //Код
        public override string Code { get { return "FictiveSimpleSource"; } }
        //Свойства
        protected override void ReadInf(DicS<string> dic)
        {
            Label = dic["Label"];
        }
        protected override string Hash { get { return "Fictive: " + Label; } }
        //Метка правйдера, чтобы различать экземпляры
        internal string Label { get; private set; }
        
        //Диапазон источника
        protected override TimeInterval GetTimeSource()
        {
            return new TimeInterval(new DateTime(2000, 1, 1), new DateTime(2100, 1, 1));
        }

        //Словарь объектов, ключи - номера
        private readonly DicI<FictiveOut> _objects = new DicI<FictiveOut>();
        internal DicI<FictiveOut> Objects { get { return _objects; } }

        //Добавить объект в провайдер
        protected override SourceOut AddOut(InitialSignal sig)
        {
            var num = sig.Inf.GetInt("NumObject");
            if (_objects.ContainsKey(num)) return _objects[num];
            return _objects.Add(num, new FictiveOut(this, sig.Inf.GetInt("ValuesInterval")));
        }

        //Очистка списков объектов
        protected override void ClearOuts()
        {
            _objects.Clear();
        }

        //Подготока источника
        protected override void PrepareProvider()
        {
            foreach (var ob in _objects.Values)
                ob.IsInitialized = true;
        }
        
        //Создание фабрики ошибок
        protected override IMomErrFactory MakeErrFactory()
        {
            var factory = new MomErrFactory(ProviderConnect.Name, MomErrType.Source);
            factory.AddGoodDescr(0);
            factory.AddDescr(1, "Предупреждение", ErrQuality.Warning);
            factory.AddDescr(2, "Ошибка");
            return factory;
        }

        //Чтение списков равномерных значений
        protected override ValuesCount ReadCut()
        {
            var vc = new ValuesCount();
            foreach (var ob in _objects.Values)
                vc.WriteCount += ob.MakeUniformValues(PeriodBegin, PeriodBegin, true);
            return vc;
        }
        protected override ValuesCount ReadChanges()
        {
            if (_makeNextError)
            {
                _makeNextError = false;
                return new ValuesCount(VcStatus.Fail);
            }

            var vc = new ValuesCount();
            foreach (var ob in _objects.Values)
                vc.WriteCount += ob.MakeUniformValues(PeriodBegin, PeriodEnd, false);
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
