using System;
using System.ComponentModel.Composition;
using BaseLibrary;
using CommonTypes;

namespace Fictive
{
    //Фиктивный тестовый источник, реализация без чтения по блокам и OleDb
    [Export(typeof(IProvider))]
    [ExportMetadata("Code", "FictiveSource")]
    public class FictiveSource : SourceBase
    {
        public override string Code { get { return "FictiveSource"; } }

        protected override void ReadDicS(DicS<string> dic)
        {
            Frequency = dic.GetInt("Frequency");
        }

        //С какой частотой в секундах добавлять значения в результат
        internal int Frequency { get; private set; }

        public override void Prepare()
        {
            foreach (var ob in _objects.Values)
                ob.IsInitialized = true;
        }

        //Словарь объектов, ключи - номера
        private readonly DicI<ObjectFictive> _objects = new DicI<ObjectFictive>();

        //Добавить объект по заданному сигналу
        protected override SourceObject AddObject(SourceSignal sig)
        {
            var num = sig.Inf.GetInt("NumObject");
            if (!_objects.ContainsKey(num))
                return _objects.Add(num, new ObjectFictive(this, sig.Inf.GetInt("ValuesInterval")));
            return _objects[num];
        }

        public override void ClearSignals()
        {
            ProviderSignals.Clear();
            _objects.Clear();
        }

        public override TimeInterval GetTime()
        {
            return new TimeInterval(new DateTime(2000, 1, 1), new DateTime(2100, 1, 1));
        }

        protected override IErrMomFactory MakeErrFactory()
        {
            var factory = new ErrMomFactory(Code, ErrMomType.Source);
            factory.AddGoodDescr(0);
            factory.AddDescr(1, "Предупреждение", ErrorQuality.Warning);
            factory.AddDescr(2, "Ошибка");
            return factory;
        }

        //Чтение списков равномерных значений
        public override void GetValues(DateTime beginRead, DateTime endRead)
        {
            foreach (var ob in _objects.Values)
                ob.MakeUniformValues(beginRead, endRead);
        }
    }
}
