using System.ComponentModel.Composition;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Provider
{
    [Export(typeof(ProvConn))]
    [ExportMetadata("Complect", "OvationConn")]
    //Соединение с комплектом источников овации
    public class OvationConn : SourConn
    {
        //Комплект
        public override string Complect { get { return "Ovation"; }}

        //Словарь объектов по Id в Historian
        private readonly DicI<ObjectOvation> _objectsId = new DicI<ObjectOvation>();
        internal DicI<ObjectOvation> ObjectsId { get { return _objectsId; } }

        //Добавить объект
        protected override SourceObject AddObject(SourceInitSignal sig)
        {
            int id = sig.Inf.GetInt("Id");
            if (!_objectsId.ContainsKey(id))
                return _objectsId.Add(id, new ObjectOvation(this, id));
            return _objectsId[id];
        }

        //Удалить все сигналы
        public override void ClearSignals()
        {
            base.ClearSignals();
            _objectsId.Clear();
        }

        //Создание фабрики ошибок
        protected override IErrMomFactory MakeErrFactory()
        {
            var factory = new ErrMomFactory(Name, ErrMomType.Source);
            factory.AddGoodDescr(0);
            factory.AddDescr(1, "FAIR", ErrorQuality.Warning);
            factory.AddDescr(2, "POOR", ErrorQuality.Warning);
            factory.AddDescr(3, "BAD");
            factory.AddDescr(4, "Нет данных");
            return factory;
        }
    }
}