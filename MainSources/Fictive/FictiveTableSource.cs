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
    [ExportMetadata("Code", "FictiveTableSource")]
    public class FictiveTableSource : AdoSource
    {
        //Комплект
        public override string Complect { get { return "Fictive"; }}
        //Код
        public override string Code { get { return "FictiveTableSource"; } }
        
        //Создание подключения
        protected override ProviderSettings CreateConnect()
        {
            return new FictiveTableSettings();
        }
        //Ссылка на соединение
        internal FictiveTableSettings Settings { get { return (FictiveTableSettings) CurSettings; } }
        
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
            using (var rec = new RecDao(Settings.TableFile, "Objects"))
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
        protected override void ReadCut()
        {
            ReadValuesByParts(_objectsId.Values, 2, PeriodBegin.AddMinutes(-5), PeriodBegin, true);
        }
        //Чтение изменений
        protected override void ReadChanges()
        {
            ReadValuesByParts(_objectsId.Values, 2, PeriodBegin, PeriodEnd, false);
        }

        //Запрос значений по одному блоку
        protected override IRecordRead QueryPartValues(List<SourceObject> part, DateTime beg, DateTime en, bool isCut)
        {
            var sb = new StringBuilder("SELECT * FROM Values WHERE (ObjectId = " + ((ObjectFictive)part[0]).Id + ")");
            for (int i = 1; i < part.Count; i++)
            {
                sb.Append(" Or ");
                sb.Append("(ObjectId = " + ((ObjectFictive) part[0]).Id + ")");
            }
            return new RecDao(Settings.TableFile, sb.ToString());
        }

        //Определение текущего считываемого объекта
        protected override SourceObject DefineObject(IRecordRead rec)
        {
            return _objectsId[rec.GetInt("ObjectId")];
        }
    }
}