using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Fictive
{
    //Фиктивный тестовый источник с чтением по блокам, OleDb, резервным подключением
    [Export(typeof(ProviderBase))]
    [ExportMetadata("Code", "FictiveSource")]
    public class FictiveSource : AccessSource
    {
        //Код
        public override string Code { get { return "FictiveSource"; } }

        //Каждый второй раз соедиение не проходит
        private int _numConnect;
        protected override bool Connect()
        {
            return _numConnect++ % 2 == 1;
        }

        //Диапазон источника
        protected override TimeInterval GetSourceTime()
        {
            if (!Connect()) return TimeInterval.CreateDefault();
            using (var sys = new SysTabl(DbFile))
                return new TimeInterval(sys.Value("BeginInterval").ToDateTime(), sys.Value("EndInterval").ToDateTime());
        }
        
        //Словари объектов, ключи - номера и коды
        private readonly DicI<ObjectFictive> _objectsId = new DicI<ObjectFictive>();
        public DicI<ObjectFictive> ObjectsId { get { return _objectsId; } }
        private readonly DicS<ObjectFictive> _objects = new DicS<ObjectFictive>();
        public DicS<ObjectFictive> Objects { get { return _objects; } }

        //Добавление объекта
        protected override SourceObject AddObject(InitialSignal sig)
        {
            var code = sig.Inf.Get("CodeObject");
            if (!Objects.ContainsKey(code))
                return _objects.Add(code, new ObjectFictive(this));
            return _objects[code];
        }

        //Очистка списка объектов
        protected override void ClearObjects()
        {
            _objects.Clear();
            _objectsId.Clear();
        }

        //Подготова источника
        public override void Prepare()
        {
            _objectsId.Clear();
            using (var rec = new RecDao(DbFile, "Objects"))
                while (rec.Read())
                {
                    var code = rec.GetString("Code");
                    var id = rec.GetInt("ObjectId");
                    if (_objects.ContainsKey(code))
                    {
                        var ob = _objects[code];
                        ob.IsInitialized = true;
                        _objectsId.Add(id, ob).Id = id;
                    }
                }
        }

        //Создание фабрики ошибок
        protected override IErrMomFactory MakeErrFactory()
        {
            var factory = new ErrMomFactory(Code, ErrMomType.Source);
            factory.AddGoodDescr(0);
            factory.AddDescr(1, "Предупреждение", ErrorQuality.Warning);
            factory.AddDescr(2, "Ошибка");
            return factory;
        }

        //Чтение среза
        protected override ValuesCount ReadCut()
        {
            return ReadValuesByParts(_objectsId.Values, 2, PeriodBegin.AddMinutes(-5), PeriodBegin, true);
        }
        //Чтение изменений
        protected override ValuesCount ReadChanges()
        {
            return ReadValuesByParts(_objectsId.Values, 2, PeriodBegin, PeriodEnd, false);
        }

        //Запрос значений по одному блоку
        protected override IRecordRead QueryValues(IList<SourceObject> part, DateTime beg, DateTime en, bool isCut)
        {
            var sb = new StringBuilder("SELECT * FROM Values WHERE (ObjectId = " + ((ObjectFictive)part[0]).Id + ")");
            for (int i = 1; i < part.Count; i++)
            {
                sb.Append(" Or ");
                sb.Append("(ObjectId = " + ((ObjectFictive) part[0]).Id + ")");
            }
            return new RecDao(DbFile, sb.ToString());
        }

        //Определение текущего считываемого объекта
        protected override SourceObject DefineObject(IRecordRead rec)
        {
            return _objectsId[rec.GetInt("ObjectId")];
        }
    }
}