using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Threading;
using System.Windows.Forms;

namespace BaseLibrary
{
    //Обертка для reader из ADO.NET
    public class ReaderAdo : IRecordRead
    {
        //Тип базы данных
        public DatabaseType DatabaseType { get; private set; }
        //True - конструктор вызван с указанием соединения, False - с указанием файла Access или списка свойств SQL Server
        private readonly bool _useDb;

        //Ссылка на OleDbDataReader или SqlDataReader
        public IDataReader Reader { get; private set; }
        //Ссылка на базу данных
        public DaoDb DaoDb { get; private set; }
        //Ссылка на Command
        private DbCommand _command;
        //Свойства соединения с SQL
        private readonly SqlProps _props;
        private readonly SqlConnection _connection;
        //Конец
        public bool EOF { get; private set; }
        //True, если курсор передвигался
        private bool _isMove;

        public ReaderAdo(DaoDb db, //база Access
                                    string stSql) //запрос
        {
            _useDb = true;
            DatabaseType = DatabaseType.Access;
            DaoDb = db.ConnectAdo();
            OpenReader(stSql, db.Connection);
        }

        public ReaderAdo(string db, //файл accdb
                                    string stSql) //запрос
        {
            _useDb = false;
            DatabaseType = DatabaseType.Access;
            if (stSql.IsEmpty() || db.IsEmpty())
                throw new NullReferenceException("Путь к файлу базы данных не может быть пустой строкой или null");
            DaoDb = new DaoDb(db).ConnectAdo();
            OpenReader(stSql, DaoDb.Connection);
        }

        //Ридер для SQL Server
        public ReaderAdo(SqlProps props, //Настройки соединения с SQL
                                   string stSql, //запрос
                                   int timeout = 300) //ограничение времени на команду, сек
        {
            _useDb = false;
            _props = props;
            DatabaseType = DatabaseType.SqlServer;
            _connection = SqlDb.Connect(props);
            OpenReader(stSql, _connection, timeout);
        }

        //Ридер для произвольного OleDbConnection, не SQL и не Access
        public ReaderAdo(OleDbConnection con, //Открытое соединени с источником OleDb
                                   string stSql, //строка команды
                                   params OleDbParameter[] pars) //параметры команды
        {
            _useDb = true;
            DatabaseType = DatabaseType.OleDb;
            OpenReader(stSql, con,  300, pars);
        }

        private void OpenReader(string stSql, IDbConnection con, int timeout = 300, params OleDbParameter[] pars)
        {
            if (stSql.IsEmpty() || con == null)
                throw new NullReferenceException("Строка запроса и соединение не могут быть пустыми строками или null");
            switch (DatabaseType)
            {
                case DatabaseType.Access:
                case DatabaseType.OleDb:
                    var cmdo = new OleDbCommand(stSql, (OleDbConnection)con) { CommandType = CommandType.Text };
                    foreach (var par in pars)
                        cmdo.Parameters.Add(par);
                    var ro = cmdo.ExecuteReader();
                    _command = cmdo;
                    Reader = ro;
                    EOF = ro == null || !ro.HasRows;
                    break;
                case DatabaseType.SqlServer:
                    var cmds = new SqlCommand(stSql, (SqlConnection)con) {CommandTimeout = timeout};
                    var rs = cmds.ExecuteReader();
                    _command = cmds;
                    Reader = rs;
                    EOF = !rs.HasRows;
                    break;
            }
            if (!EOF && Reader != null) Reader.Read();
            _isMove = false;
        }

        //True - если есть записи
        public bool HasRows
        {
            get
            {
                if (Reader is OleDbDataReader)
                    return ((OleDbDataReader)Reader).HasRows;
                if (Reader is SqlDataReader)
                    return ((SqlDataReader)Reader).HasRows;
                return false;    
            }
        }

        //Количество записей, на входе строка запроса-комманды Count
        public int RecordCount(string stSql)
        {
            if (stSql.IsEmpty())
                throw new NullReferenceException("Строка запроса не может быть пустой строкой или null");
            switch (DatabaseType)
            {
                case DatabaseType.Access:
                    var cmdo = new OleDbCommand(stSql, DaoDb.Connection);
                    return (int)cmdo.ExecuteScalar();
                case DatabaseType.SqlServer:
                    var cmds = new SqlCommand(stSql, SqlDb.Connect(_props)) {CommandTimeout = 120 };
                    return (int)cmds.ExecuteScalar();
            }
            return 0;
        }

        //Закрытие рекордсета, полная очистка ресурсов
        public void Dispose()
        {
            try { _command.Dispose(); } catch { }
            try
            {
                Reader.Close();
                Reader.Dispose();
            } catch { }
            try
            {
                if (DatabaseType == DatabaseType.SqlServer)
                    _connection.Close();
            }
            catch { }
            if (!_useDb && DatabaseType == DatabaseType.Access) 
                DaoDb.Dispose();
        }
        
        //Переход на следующую запись
        public bool Read()
        {
            if (_isMove)
            {
                try { EOF = !Reader.Read(); }
                catch (SqlException ex)
                {
                    Thread.Sleep(1);
                    EOF = !Reader.Read();
                }
            }
            else _isMove = true;
            return !EOF;
        }

        //Получение значения из ридера
        private object GetValue(string field)
        {
            try { return Reader[field]; }
            catch (SqlException ex)
            {
                Thread.Sleep(1);
                return Reader[field];
            }
        }
        private object GetValue(int num)
        {
            try { return Reader[num]; }
            catch (SqlException ex)
            {
                Thread.Sleep(1);
                return Reader[num];
            }
        }

        //Переводит значение поля field таблицы rec в строку, DbNull переводит в nullValue
        public string GetString(string field, string nullValue = null)
        {
            var v = GetValue(field);
            return DBNull.Value.Equals(v) ? nullValue : Convert.ToString(v);
        }
        public string GetString(int num, string nullValue = null)
        {
            var v = GetValue(num);
            return DBNull.Value.Equals(v) ? nullValue : Convert.ToString(v);
        }

        //Переводит значение поля field таблицы rec в число, DbNull переводит в nullValue
        public double GetDouble(string field, double nullValue = 0)
        {
            var v = GetValue(field);
            return DBNull.Value.Equals(v) ? nullValue : Convert.ToDouble(v);
        }
        public double GetDouble(int num, double nullValue = 0)
        {
            var v = GetValue(num);
            return DBNull.Value.Equals(v) ? nullValue : Convert.ToDouble(v);
        }
        //Переводит значение поля field таблицы rec в число, DbNull переводит в null
        public double? GetDoubleNull(string field)
        {
            var v = GetValue(field);
            if (DBNull.Value.Equals(v)) return null;
            return Convert.ToDouble(v);
        }
        public double? GetDoubleNull(int num)
        {
            var v = GetValue(num);
            if (DBNull.Value.Equals(v)) return null;
            return Convert.ToDouble(v);
        }

        //Переводит значение поля field таблицы rec в число, DbNull переводит в nullValue
        public int GetInt(string field, int nullValue = 0)
        {
            var v = GetValue(field);
            return DBNull.Value.Equals(v) ? nullValue : Convert.ToInt32(v);
        }
        public int GetInt(int num, int nullValue = 0)
        {
            var v = GetValue(num);
            return DBNull.Value.Equals(v) ? nullValue : Convert.ToInt32(v);
        }
        //Переводит значение поля field таблицы rec в число, DbNull переводит в null
        public int? GetIntNull(string field)
        {
            var v = GetValue(field);
            if (DBNull.Value.Equals(v)) return null;
            return Convert.ToInt32(v);
        }
        public int? GetIntNull(int num)
        {
            var v = GetValue(num);
            if (DBNull.Value.Equals(v)) return null;
            return Convert.ToInt32(v);
        }

        //Переводит значение поля field таблицы rec в дату, DbNull переводит в DateTime.MinValue
        public DateTime GetTime(string field)
        {
            var v = GetValue(field);
            return DBNull.Value.Equals(v) ? Different.MinDate : Convert.ToDateTime(v);
        }
        public DateTime GetTime(int num)
        {
            var v = GetValue(num);
            return DBNull.Value.Equals(v) ? Different.MinDate : Convert.ToDateTime(v);
        }
        //Переводит значение поля field таблицы rec в дату, DbNull переводит в null
        public DateTime? GetTimeNull(string field)
        {
            var v = GetValue(field);
            if (DBNull.Value.Equals(v)) return null;
            return Convert.ToDateTime(v);
        }
        public DateTime? GetTimeNull(int num)
        {
            var v = GetValue(num);
            if (DBNull.Value.Equals(v)) return null;
            return Convert.ToDateTime(v);
        }

        //Переводит значение поля field таблицы rec в bool, DbNull переводит в nullValue
        public bool GetBool(string field, bool nullValue = false)
        {
            var v = GetValue(field);
            return DBNull.Value.Equals(v) ? nullValue : Convert.ToBoolean(v);
        }
        public bool GetBool(int num, bool nullValue = false)
        {
            var v = GetValue(num);
            return DBNull.Value.Equals(v) ? nullValue : Convert.ToBoolean(v);
        }
        //Переводит значение поля field таблицы rec в bool, DbNull переводит в null
        public bool? GetBoolNull(string field)
        {
            var v = GetValue(field);
            if (DBNull.Value.Equals(v)) return null;
            return Convert.ToBoolean(v);
        }
        public bool? GetBoolNull(int num)
        {
            var v = GetValue(num);
            if (DBNull.Value.Equals(v)) return null;
            return Convert.ToBoolean(v);
        }

        //Проверяет, равно ли значение заданного поля null
        public bool IsNull(string field)
        {
            return DBNull.Value.Equals(GetValue(field));
        }
        public bool IsNull(int num)
        {
            return DBNull.Value.Equals(GetValue(num));
        }

        //Копирует значение поля field в ячейку gridField, ряда cells или row, датагрида WinForms
        public void GetToDataGrid(string field, DataGridViewCellCollection cells, string gridField = null)
        {
            cells[gridField ?? field].Value = GetValue(field);
        }
        public void GetToDataGrid(string field, DataGridViewRow row, string gridField = null)
        {
            GetToDataGrid(field, row.Cells, gridField);
        }
    }
}
