using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Text;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Provider
{
    [Export(typeof(ProviderBase))]
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
        //Хэш
        protected override string Hash { get { return "SQLServer=" + _serverName; } }

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

        //Проверка соединения
        protected override bool CheckConnection()
        {
            try
            {
                if (Reconnect() && Connection.State == ConnectionState.Open)
                {
                    CheckConnectionMessage += "Успешное соединение с архивом WINCC";
                    return true;
                }
                CheckConnectionMessage += "Не удалось соединиться с архивом WINCC";
            }
            catch { CheckConnectionMessage += "Не удалось соединиться с архивом WINCC"; }
            return false;
        }

        //Проверка настроек
        protected override string CheckSettings(DicS<string> inf)
        {
            if (!inf.ContainsKey("SQLServer"))
                return "Не указано имя архивного сервера";
            return "";
        }

        //Словари сигналов, ключи полные коды и Id
        private readonly DicI<OutSimatic> _outsId = new DicI<OutSimatic>();

        //Добавить объект в провайдер
        protected override SourceOut AddOut(InitialSignal sig)
        {
            int id = sig.Inf.GetInt("Id");
            if (!_outsId.ContainsKey(id))
                return _outsId.Add(id, new OutSimatic(this, sig.Inf["Archive"], sig.Inf["Tag"], id));
            return _outsId[id];
        }
        
        //Очистка списка сигналов
        protected override void ClearOuts()
        {
            _outsId.Clear();
        }

        //Создание фабрики ошибок
        protected override IErrMomFactory MakeErrFactory()
        {
            var factory = new ErrMomFactory(ProviderConnect.Name, ErrMomType.Source) {UndefinedErrorText = "Ошибка"};
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
        protected override IRecordRead QueryValues(IList<SourceOut> part, DateTime beg, DateTime en, bool isCut)
        {
            var sb = new StringBuilder("TAG:R, ");
            if (part.Count == 1)
                sb.Append(((OutSimatic)part[0]).Id);
            else
            {
                sb.Append("(").Append(((OutSimatic) part[0]).Id);
                for (int i = 1; i < part.Count; i++)
                    sb.Append(";").Append(((OutSimatic) part[i]).Id);
                sb.Append(";-2)");
            }
            sb.Append(", ").Append(DateToSimatic(beg));
            sb.Append(", ").Append(DateToSimatic(en));
            
            AddEvent("Запрос значений из архива", part.Count + " тегов");
            return new ReaderAdo(Connection, sb.ToString());
        }

        //Определение текущего считываемого выхода
        protected override SourceOut DefineOut(IRecordRead rec)
        {
            return _outsId[rec.GetInt(0)];
        }
        
        //Чтение среза
        protected override ValuesCount ReadCut()
        {
            return ReadByParts(_outsId.Values, 500, PeriodBegin.AddSeconds(-60), PeriodBegin, true);
        }
        //Чтение изменений
        protected override ValuesCount ReadChanges()
        {
            return ReadByParts(_outsId.Values, 500);
        }
    }
}

