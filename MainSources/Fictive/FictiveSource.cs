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
    [Export(typeof(Provider))]
    [ExportMetadata("Code", "FictiveSource")]
    public class FictiveSource : AccessSource
    {
        //Код
        public override string Code { get { return "FictiveSource"; } }

        //Диапазон источника
        protected override TimeInterval GetTimeSource()
        {
            if (!Connect()) return TimeInterval.CreateDefault();
            using (var sys = new SysTabl(DbFile))
                return new TimeInterval(sys.Value("BeginInterval").ToDateTime(), sys.Value("EndInterval").ToDateTime());
        }
        
        //Словари выходов, ключи - номера и коды
        internal readonly DicI<FictiveOut> OutsId = new DicI<FictiveOut>();
        internal readonly DicS<FictiveOut> Outs = new DicS<FictiveOut>();
        //Словари объектов второй таблицы
        internal readonly DicI<FictiveSmallOut> OutsId2 = new DicI<FictiveSmallOut>();
        internal readonly DicS<FictiveSmallOut> Outs2 = new DicS<FictiveSmallOut>();
        //Объект действий оператора
        internal FictiveOperatorOut OperatorOut;

        //Добавление объекта
        protected override ListSourceOut AddOut(InitialSignal sig)
        {
            var table = sig.Inf.Get("Table");
            bool isErr = sig.Inf.Get("IsErrorObject") == "True";
            var code = sig.Inf.Get("ObjectCode");
            switch (table)
            {
                case "MomValues":
                    return Outs.ContainsKey(code) 
                        ? Outs[code] 
                        : Outs.Add(code, new FictiveOut(this, isErr));
                case "MomValues2":
                    return Outs2.ContainsKey(code) 
                        ? Outs2[code] 
                        : Outs2.Add(code, new FictiveSmallOut(this));
                case "MomOperator":
                    return OperatorOut ?? (OperatorOut = new FictiveOperatorOut(this));
            }
            return null;
        }

        //Очистка списка объектов
        protected override void ClearOuts()
        {
            Outs.Clear();
            OutsId.Clear();
            Outs2.Clear();
            OutsId2.Clear();
            OperatorOut = null;
        }

        //Подготова источника
        protected override void PrepareProvider()
        {
            OutsId.Clear();
            OutsId2.Clear();
            using (var rec = new DaoRec(DbFile, "Objects"))
                while (rec.Read())
                {
                    var code = rec.GetString("Code");
                    var table = rec.GetString("TableName");
                    var id = rec.GetInt("ObjectId");
                    if (table == "MomValues" && Outs.ContainsKey(code))
                    {
                        var ob = Outs[code];
                        ob.IsInitialized = true;
                        OutsId.Add(id, ob).Id = id;
                    }
                    if (table == "MomValues2" && Outs2.ContainsKey(code))
                    {
                        var ob = Outs2[code];
                        OutsId2.Add(id, ob).Id = id;
                    }
                }
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

        //Запрос значений по одному блоку из таблицы Values
        protected override IRecordRead QueryValues(IList<ListSourceOut> part, DateTime beg, DateTime en, bool isCut)
        {
            var sb = new StringBuilder("SELECT " + "MomValues.* FROM " + "MomValues" + " WHERE (");
            for (int i = 0; i < part.Count; i++)
            {
                var ob = (FictiveOut)part[i];
                if (ob.IsErrorObject) 
                    throw new Exception("Ошибочный объект");
                if (i > 0) sb.Append(" Or ");
                sb.Append("(ObjectId = " + ob.Id + ")");
            }
            sb.Append(") And (Time > " + beg.ToAccessString() + ") And (Time <= " + en.ToAccessString() + ")");
            sb.Append(" ORDER BY Time");
            return new DaoRec(DbFile, sb.ToString());
        }
        //Определение текущего считываемого объекта
        protected override ListSourceOut DefineOut(IRecordRead rec)
        {
            return OutsId[rec.GetInt("ObjectId")];
        }

        //Запрос значений по одному блоку из таблицы Values2
        protected IRecordRead QueryValues2(IList<ListSourceOut> part, DateTime beg, DateTime en, bool isCut)
        {
            var sb = new StringBuilder("SELECT MomValues2.* FROM MomValues2 WHERE ((ObjectId = " + ((FictiveSmallOut)part[0]).Id + ")");
            for (int i = 1; i < part.Count; i++)
            {
                sb.Append(" Or ");
                sb.Append("(ObjectId = " + ((FictiveSmallOut)part[i]).Id + ")");
            }
            sb.Append(") And (Time > " + beg.ToAccessString() + ") And (Time <= " + en.ToAccessString() + ")");
            sb.Append(" ORDER BY Time");
            return new DaoRec(DbFile, sb.ToString());
        }
        protected ListSourceOut DefineOut2(IRecordRead rec)
        {
            return OutsId2[rec.GetInt("ObjectId")];
        }

        //Запрос действий оператора
        protected IRecordRead QueryOperator(IList<ListSourceOut> part, DateTime beg, DateTime en, bool isCut)
        {
            return new DaoRec(DbFile, "SELECT * FROM MomOperator WHERE (Time >= " + beg.ToAccessString() + ") And (Time <= " + en.ToAccessString() + ")");
        }

        //Чтение среза
        protected override ValuesCount ReadCut()
        {
            return ReadByParts(OutsId.Values, 2, PeriodBegin.AddMinutes(-5), PeriodBegin, true) +
                      ReadByParts(OutsId2.Values, 2, PeriodBegin.AddDays(-1), PeriodBegin, true, QueryValues2, DefineOut2);
        }
        //Чтение изменений
        protected override ValuesCount ReadChanges()
        {
            return ReadByParts(OutsId.Values, 2) +
                      ReadByParts(Outs2.Values, 2, QueryValues2, DefineOut2) +
                      ReadOneOut(OperatorOut, QueryOperator);
        }
    }
}