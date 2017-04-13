using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Simatic
{
    [Export(typeof(Provider))]
    [ExportMetadata("Code", "SimaticSource")]
    public class SimaticSource : OleDbSource
    {
        //Код провайдера
        public override string Code { get { return "SimaticSource"; } }

        //Имя сервера
        private string _serverName;
        //Чтение настроек
        protected override void ReadInf(DicS<string> dic)
        {
            _serverName = dic["SQLServer"];
        }

        //Строка соединения
        protected override string ConnectionString
        {
            get
            {
                var list = SqlDb.SqlDatabasesList(_serverName);
                var dbName = "";
                foreach (var db in list)
                    if (db.StartsWith("CC_") && db.EndsWith("R"))
                        dbName = db;
                if (dbName.IsEmpty()) return null;
                SqlDb.Connect(_serverName, dbName).GetSchema();//Проверка
                var dic = new Dictionary<string, string>
                    {
                        {"Provider", "WinCCOLEDBProvider.1"},
                        {"Catalog", dbName},
                        {"Data Source", _serverName}
                    };
                return dic.ToPropertyString();
            }
        }
        
        //Проверка настроек
        protected override string CheckSettings(DicS<string> inf)
        {
            if (!inf.ContainsKey("SQLServer"))
                return "Не указано имя архивного сервера";
            return "";
        }

        //Словари сигналов, ключи полные коды и Id
        internal readonly DicI<SimaticOut> OutsId = new DicI<SimaticOut>();

        //Добавить объект в провайдер
        protected override ProviderOut AddOut(ProviderSignal sig)
        {
            int id = sig.Inf.GetInt("Id");
            return OutsId.ContainsKey(id) 
                ? OutsId[id] 
                : OutsId.Add(id, new SimaticOut(this, sig.Inf["Archive"], sig.Inf["Tag"], id));
        }
        
        //Очистка списка сигналов
        protected override void ClearOuts()
        {
            OutsId.Clear();
        }

        //Создание фабрики ошибок
        protected override IMomErrFactory MakeErrFactory()
        {
            var factory = new MomErrFactory(ProviderConnect.Name, MomErrType.Source) {UndefinedErrorText = "Ошибка"};
            factory.AddGoodDescr(128);
            return factory;
        }

        //Преводит дату в формат для запросов Simatic
        private static string DateToSimatic(DateTime d)
        {
            DateTime dd = d.ToUniversalTime();
            return "'" + dd.Year + "-" + dd.Month + "-" + dd.Day + " " + dd.Hour + ":" + dd.Minute + ":" + dd.Second + "." + dd.Millisecond + "'";
        }

        //Запрос значений из архива по списку выходов и интервалу
        protected override IRecordRead QueryValues(IList<ListSourceOut> part, DateTime beg, DateTime en, bool isCut)
        {
            var sb = new StringBuilder("TAG:R, ");
            if (part.Count == 1)
                sb.Append(((SimaticOut)part[0]).Id);
            else
            {
                sb.Append("(").Append(((SimaticOut) part[0]).Id);
                for (int i = 1; i < part.Count; i++)
                    sb.Append(";").Append(((SimaticOut) part[i]).Id);
                sb.Append(";-2)");
            }
            sb.Append(", ").Append(DateToSimatic(beg));
            sb.Append(", ").Append(DateToSimatic(en));
            
            AddEvent("Запрос значений из архива", part.Count + " тегов");
            return new AdoReader(Connection, sb.ToString());
        }

        //Определение текущего считываемого выхода
        protected override ListSourceOut DefineOut(IRecordRead rec)
        {
            return OutsId[rec.GetInt(0)];
        }
        
        //Чтение среза
        protected override ValuesCount ReadCut()
        {
            return ReadByParts(OutsId.Values, 500, PeriodBegin.AddSeconds(-60), PeriodBegin, true);
        }
        //Чтение изменений
        protected override ValuesCount ReadChanges()
        {
            return ReadByParts(OutsId.Values, 500);
        }
    }
}

