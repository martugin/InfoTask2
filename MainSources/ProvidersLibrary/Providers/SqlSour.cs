using System;
using System.Collections.Generic;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Источник, использующий SQL-сервер
    public abstract class SqlSour : AdoSour
    {
        //Загрузка свойств из словаря
        protected override void ReadInf(DicS<string> dic)
        {
            bool e = dic["IndentType"].ToUpper() != "WINDOWS";
            string server = dic["SQLServer"], db = dic["Database"];
            SqlProps = new SqlProps(server, db, e, dic["Login"], dic["Password"]);
        }

        public override string Hash
        {
            get { return "SQLServer=" + SqlProps.ServerName + ";Database=" + SqlProps.DatabaseName; }
        }

        //Возвращает выпадающий список для поля настройки, props - словарь значение свойств, propname - имя свойства для ячейки со списком
        internal override List<string> ComboBoxList(Dictionary<string, string> props, string propname)
        {
            try
            {
                bool hasServer = props.ContainsKey("SQLServer") && !props["SQLServer"].IsEmpty();
                var hasLogin = (props["IndentType"].ToUpper() == "WINDOWS" || (props.ContainsKey("Login") && !props["Login"].IsEmpty()));
                if (propname == "Database" && hasServer && hasLogin)
                    return SqlDb.SqlDatabasesList(props["SQLServer"], props["IndentType"].ToUpper() != "WINDOWS", props["Login"], props["Password"]);
            }
            catch { }
            return new List<string>();
        }

        //Настройки SQL Server
        protected SqlProps SqlProps { get; private set; }

        //Проверка соединения
        public override bool Check()
        {
            return Danger(TryCheck, 2, 500, "Не удалось соединиться с SQL-сервером");
        }
        private bool TryCheck()
        {
            try
            {
                using (SqlDb.Connect(SqlProps))
                    return true;
            }
            catch (Exception ex)
            {
                AddError("Не удалось соединиться с SQL-сервером", ex);
                return false;
            }
        }

        //Проверка настроек
        public override string CheckSettings(DicS<string> inf)
        {
            string err = "";
            if (inf["SQLServer"].IsEmpty()) err += "Не указано имя SQL-сервера" + Environment.NewLine;
            if (inf["IndentType"].IsEmpty()) err += "Не задан тип идентификации" + Environment.NewLine;
            if (inf["IndentType"] == "SqlServer" && inf["Login"].IsEmpty()) err += "Не задан логин" + Environment.NewLine;
            if (inf["Database"].IsEmpty()) err += "Не задано имя базы данных" + Environment.NewLine;
            return err;
        }

        //Проверка соединения
        public override bool CheckConnection()
        {
            if (Check())
            {
                CheckConnectionMessage = "Успешное соединение";
                return true;
            }
            AddError(CheckConnectionMessage = "Не удалось соединиться с SQL-сервером");
            return false;
        }
    }
}