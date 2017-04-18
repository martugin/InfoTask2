using System;
using System.Collections.Generic;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Источник данных, получаемых из базы данных SQL Server
    public abstract class SqlServerSource : AdoSource
    {
        //Загрузка свойств из словаря
        protected override void ReadInf(DicS<string> dic)
        {
            bool e = dic["IdentType"].ToUpper() != "WINDOWS";
            string server = dic["SQLServer"], db = dic["Database"];
            SqlProps = new SqlProps(server, db, e, dic["Login"], dic["Password"]);
        }

        //Настройки SQL Server
        protected SqlProps SqlProps { get; private set; }

        //Проверка соединения
        protected override void ConnectProvider()
        {
            SqlDb.Connect(SqlProps);
        }

        //Возвращает выпадающий список для поля настройки 
        internal override List<string> ComboBoxList(Dictionary<string, string> props, //словарь значение свойств
                                                                         string propname) //имя свойства для ячейки со списком
        {
            try
            {
                bool hasServer = props.ContainsKey("SQLServer") && !props["SQLServer"].IsEmpty();
                var hasLogin = props["IndentType"].ToUpper() == "WINDOWS" || (props.ContainsKey("Login") && !props["Login"].IsEmpty());
                if (propname == "Database" && hasServer && hasLogin)
                    return SqlDb.SqlDatabasesList(props["SQLServer"], props["IndentType"].ToUpper() != "WINDOWS", props["Login"], props["Password"]);
            }
            catch { }
            return new List<string>();
        }

        //Проверка настроек
        protected internal override string CheckSettings(DicS<string> inf)
        {
            string err = "";
            if (inf["SQLServer"].IsEmpty()) err += "Не указано имя SQL-сервера" + Environment.NewLine;
            if (inf["IndentType"].IsEmpty()) err += "Не задан тип идентификации" + Environment.NewLine;
            if (inf["IndentType"] == "SqlServer" && inf["Login"].IsEmpty()) err += "Не задан логин" + Environment.NewLine;
            if (inf["Database"].IsEmpty()) err += "Не задано имя базы данных" + Environment.NewLine;
            return err;
        }
    }
}