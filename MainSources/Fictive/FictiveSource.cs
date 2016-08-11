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
        //private int _numConnect;
        //protected override bool ConnectProvider()
        //{
        //    return _numConnect++ % 2 == 1;
        //}

        //Диапазон источника
        protected override TimeInterval GetTimeSource()
        {
            if (!Connect()) return TimeInterval.CreateDefault();
            using (var sys = new SysTabl(DbFile))
                return new TimeInterval(sys.Value("BeginInterval").ToDateTime(), sys.Value("EndInterval").ToDateTime());
        }
        
        //Словари объектов, ключи - номера и коды
        internal readonly DicI<ObjectFictive> ObjectsId = new DicI<ObjectFictive>();
        internal readonly DicS<ObjectFictive> Objects = new DicS<ObjectFictive>();
        //Словари объектов второй таблицы
        internal readonly DicI<ObjectFictiveSmall> ObjectsId2 = new DicI<ObjectFictiveSmall>();
        internal readonly DicS<ObjectFictiveSmall> Objects2 = new DicS<ObjectFictiveSmall>();
        //Объект действий оператора
        internal ObjectFictiveOperator OperatorObject;

        //Добавление объекта
        protected override SourceObject AddObject(InitialSignal sig)
        {
            var table = sig.Inf.Get("Table");
            var code = sig.CodeObject;
            if (table == "Values")
            {
                if (!Objects.ContainsKey(code))
                    return Objects.Add(code, new ObjectFictive(this));
                return Objects[code];    
            }
            if (table == "Values2")
            {
                if (!Objects2.ContainsKey(code))
                    return Objects2.Add(code, new ObjectFictiveSmall(this));
                return Objects2[code];
            }
            if (table == "Operator")
            {
                if (OperatorObject == null)
                    OperatorObject = new ObjectFictiveOperator(this);
                return OperatorObject;
            }
            return null;
        }

        //Очистка списка объектов
        protected override void ClearObjects()
        {
            Objects.Clear();
            ObjectsId.Clear();
            Objects2.Clear();
            ObjectsId2.Clear();
            OperatorObject = null;
        }

        //Подготова источника
        protected override void PrepareSource()
        {
            ObjectsId.Clear();
            ObjectsId2.Clear();
            using (var rec = new RecDao(DbFile, "Objects"))
                while (rec.Read())
                {
                    var code = rec.GetString("Code");
                    var table = rec.GetString("TableName");
                    var id = rec.GetInt("ObjectId");
                    if (table == "Values" && Objects.ContainsKey(code))
                    {
                        var ob = Objects[code];
                        ob.IsInitialized = true;
                        ObjectsId.Add(id, ob).Id = id;
                    }
                    if (table == "Values2" && Objects2.ContainsKey(code))
                    {
                        var ob = Objects2[code];
                        ObjectsId2.Add(id, ob).Id = id;
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
            return ReadByParts(ObjectsId.Values, 2, PeriodBegin.AddMinutes(-5), PeriodBegin, true) +
                      ReadByParts(ObjectsId2.Values, 2, PeriodBegin.AddDays(-1), PeriodBegin, true, QueryValues2, DefineObject2);
        }
        //Чтение изменений
        protected override ValuesCount ReadChanges()
        {
            return ReadByParts(ObjectsId.Values, 2) +
                      ReadByParts(Objects2.Values, 2) +
                      ReadOneObject(OperatorObject, QueryOperator);
        }

        //Запрос значений по одному блоку из таблицы Values
        protected override IRecordRead QueryValues(IList<SourceObject> part, DateTime beg, DateTime en, bool isCut)
        {
            return RunQuery("Values", part, beg, en);
        }
        //Определение текущего считываемого объекта
        protected override SourceObject DefineObject(IRecordRead rec)
        {
            return ObjectsId[rec.GetInt("ObjectId")];
        }

        //Запрос значений по одному блоку из таблицы Values2
        protected IRecordRead QueryValues2(IList<SourceObject> part, DateTime beg, DateTime en, bool isCut)
        {
            return RunQuery("Values2", part, beg, en);
        }
        protected SourceObject DefineObject2(IRecordRead rec)
        {
            return ObjectsId2[rec.GetInt("ObjectId")];
        }

        //Запрос
        private IRecordRead RunQuery(string table, IList<SourceObject> part, DateTime beg, DateTime en)
        {
            var sb = new StringBuilder("SELECT * FROM " + table + " WHERE (ObjectId = " + ((ObjectFictive)part[0]).Id + ")");
            for (int i = 1; i < part.Count; i++)
            {
                sb.Append(" Or ");
                sb.Append("(ObjectId = " + ((ObjectFictive)part[i]).Id + ")");
            }
            sb.Append(") And (Time >= " + beg.ToAccessString() + ") And (Time <= " + en.ToAccessString() + ")");
            sb.Append(" ORDER BY Time");
            return new RecDao(DbFile, sb.ToString());
        }

        //Запрос действий оператора
        protected IRecordRead QueryOperator(IList<SourceObject> part, DateTime beg, DateTime en, bool isCut)
        {
            return new RecDao(DbFile, "SELECT * FROM Operator WHERE (Time >= " + beg.ToAccessString() + ") And (Time <= " + en.ToAccessString() + ")");
        }
    }
}