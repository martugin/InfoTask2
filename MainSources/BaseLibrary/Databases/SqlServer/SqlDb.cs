using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Threading;

namespace BaseLibrary
{
    //База данных SQL
    public static class SqlDb
    {
        //Открывает соединение без соединения с базой данных
        public static SqlConnection ConnectServer(string server, bool sqlIdent = false, string user = "", string password = "")
        {
            var st = "Data Source=" + (server ?? "") + ";Integrated Security=";
            if (!sqlIdent) st += "True";
            else st += "False;User=" + (user ?? "") + "; Password=" + (password ?? "");
            var con = new SqlConnection(st);
            con.Open();
            return con;
        }

        //Открывает соединение, исключения не обрабатывает
        public static SqlConnection Connect(string server, string database, bool sqlIdent = false, string user = "", string password = "")
        {
            var st = "Data Source=" + (server ?? "") + ";Initial Catalog=" + (database ?? "") + ";Integrated Security=";
            if (!sqlIdent) st += "True";
            else st += "False;User=" + (user ?? "") + "; Password=" + (password ?? "");
            var con = new SqlConnection(st);
            con.Open();
            return con;
        }

        //sqlIdent=true - идентификация SqlServer иначе Windows 
        public static SqlConnection Connect(SqlProps props)
        {
            var st = "Data Source=" + (props.ServerName ?? "") + ";Initial Catalog=" + (props.DatabaseName ?? "") + ";Integrated Security=";
            if (!props.SqlIdent) st += "True";
            else st += "False;User=" + (props.Login ?? "") + "; Password=" + (props.Password ?? "");
            var con = new SqlConnection(st);
            con.Open();
            return con;
        } 

        //Выполняет запрос на изменение
        public static void Execute(SqlProps props, string stSql)
        {
            var con = Connect(props);
            var com = new SqlCommand(stSql, con) { CommandTimeout = 300 };
            bool b = false;
            int i = 0;
            Exception exc = null;
            while (!b && i++<3)
            {
                try
                {
                    com.ExecuteNonQuery(); 
                    b = true;
                }
                catch (Exception ex)
                {
                    exc = ex;
                    Thread.Sleep(500);
                }
            }
            try {con.Close();} catch {}
            if (!b) throw exc;
        }

        //Список всех SQL-серверов в сети
        public static List<string> SqlServersList()
        {
            var list = new List<string>();
            DataTable sqlSources = SqlDataSourceEnumerator.Instance.GetDataSources();
            foreach (DataRow source in sqlSources.Rows)
            {
                string instanceName = source["InstanceName"].ToString();
                if (!instanceName.IsEmpty())
                    list.Add(source["InstanceName"].ToString() + '\\' + source["ServerName"]);
                else
                    list.Add(source["ServerName"].ToString() + source["Version"]);
            }
            return list;
        }
        //Список всех баз данных на SQL-сервере
        public static List<string> SqlDatabasesList(string server, bool sqlIdent = false, string user = "", string password = "")
        {
            var list = new List<string>();
            try
            {
                using (var con = ConnectServer(server, sqlIdent, user, password))
                {
                    DataTable databases = con.GetSchema("Databases");
                    foreach (DataRow database in databases.Rows)
                        list.Add(database.Field<String>("database_name"));
                }
            }
            catch { }
            return list;
        }
    }
}
