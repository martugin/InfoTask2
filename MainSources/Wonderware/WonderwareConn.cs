using System.Collections.Generic;
using System.ComponentModel.Composition;
using CommonTypes;
using ProvidersLibrary;

namespace Provider
{
    //Соединение источника Wonderware
    [Export(typeof(ProvConn))]
    [ExportMetadata("Complect", "WonderwareConn")]
    public class WonderwareConn : SourConn
    {
        //Комплект
        public override string Complect { get { return "Wonderware"; }}

        //Словарь объектов по TagName
        private readonly Dictionary<string, ObjectWonderware> _objects = new Dictionary<string, ObjectWonderware>();
        internal Dictionary<string, ObjectWonderware> Objects {  get { return _objects; } }

        //Добавить сигнал в провайдер
        protected override SourObject AddObject(SourInitSignal sig)
        {
            string tag = sig.Inf["TagName"];
            if (!_objects.ContainsKey(tag))
                _objects.Add(tag, new ObjectWonderware(this, tag));
            return _objects[tag];
        }
        
        //Очистка списка сигналов
        public override void ClearSignals()
        {
            base.ClearSignals();
            Objects.Clear();
        }
        
        //Создание фабрики ошибок
        protected override IErrMomFactory MakeErrFactory()
        {
            var factory = new ErrMomFactory(Name, ErrMomType.Source);
            factory.AddGoodDescr(192);
            factory.AddDescr(0, "Bad Quality of undetermined state");
            factory.AddDescr(1, "No data available, tag did not exist at the time");
            factory.AddDescr(2, "Running insert");
            factory.AddDescr(10, "Communication loss");
            factory.AddDescr(16, "Good value, received out of time sync (cyclic tag)");
            factory.AddDescr(17, "Duplicate time stamp; infinite slope");
            factory.AddDescr(20, "IDAS overflow recovery");
            factory.AddDescr(24, "IOServer communication failed");
            factory.AddDescr(33, "Violation of History Duration license feature");
            factory.AddDescr(44, "Pipe reconnect");
            factory.AddDescr(64, "Cannot convert");
            factory.AddDescr(150, "Storage startup");
            factory.AddDescr(151, "Store forward storage startup");
            factory.AddDescr(152, "Incomplete calculated value");
            factory.AddDescr(202, "Good point of version Latest", ErrorQuality.Warning);
            factory.AddDescr(212, "Counter rollover has occurred");
            factory.AddDescr(248, "First value received in store forward mode", ErrorQuality.Warning);
            factory.AddDescr(249, "Not a number");
            factory.AddDescr(252, "First value received from IOServer", ErrorQuality.Warning);
            factory.AddDescr(65536, "No data stored in history, NULL");
            return factory;
        }
    }
}
