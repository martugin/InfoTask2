using System.Collections.Generic;
using System.Data;
using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    //Подключение к архиву SIMATIC
    public class SimaticConnect : OleDbSourceConnect
    {
        //Имя сервера
        private string _serverName;

        protected override void ReadInf(DicS<string> dic)
        {
            _serverName = dic["SQLServer"];
        }

        //Хэш
        public override string Hash { get { return "SQLServer=" + _serverName; } }

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
        public override bool CheckConnection()
        {
            try
            {
                if (Check() && Connection.State == ConnectionState.Open)
                {
                    CheckConnectionMessage += "Успешное соединение с архивом";
                    return true;
                }
                CheckConnectionMessage += "Не удалось соединиться с архивом";
            }
            catch { CheckConnectionMessage += "Не удалось соединиться с архивом"; }
            return false;
        }

        //Проверка настроек
        public override string CheckSettings(DicS<string> inf)
        {
            if (!inf.ContainsKey("SQLServer"))
                return "Не указано имя архивного сервера";
            return "";
        }
    }
}