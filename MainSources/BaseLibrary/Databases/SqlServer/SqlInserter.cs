using System;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace BaseLibrary
{
    //Класс для добавления записей в таблицу SQL Server через INSERT
    public class SqlInserter : IRecordAdd
    {
        public SqlInserter(SqlProps props, string table)
        {
            _sqlProps = props;
            _connection = SqlDb.Connect(props);
            _table = table;
        }

        //Свойства
        private readonly SqlProps _sqlProps;
        //Соединение с SQL
        private readonly SqlConnection _connection;
        //Таблица, для добавления строк
        private readonly string _table;

        //Словарь добавляемых значений, ключи - имена полей, значения строки значений для запроса
        private readonly DicS<string> _values = new DicS<string>(); 

        public void Dispose()
        {
            try {_connection.Close();} catch {}
        }
        public void AddNew()
        {
            _values.Clear();
        }

        public void Update()
        {
            var sb = new StringBuilder("INSERT INTO ");
            sb.Append(_table).Append("(");
            bool e = false;
            foreach (var field in _values.Keys)
            {
                if (e) sb.Append(", ");
                sb.Append(field);
                e = true;
            }
            sb.Append(") VALUES (");
            e = false;
            foreach (var v in _values.Values)
            {
                if (e) sb.Append(", ");
                sb.Append(v);
                e = true;
            }
            sb.Append(")");
            SqlDb.Execute(_sqlProps, sb.ToString());
            _values.Clear();
        }

        public void Put(string field, string val, bool cut = false)
        {
            _values.Add(field, "'" + val + "'");
        }

        public void Put(string field, double val)
        {
            _values.Add(field, val.ToString());
        }

        public void Put(string field, double? val)
        {
            _values.Add(field, val == null ? "Null" : val.ToString());
        }

        public void Put(string field, int val)
        {
            _values.Add(field, val.ToString());
        }

        public void Put(string field, int? val)
        {
            _values.Add(field, val == null ? "Null" : val.ToString());
        }

        public void Put(string field, DateTime val)
        {
            _values.Add(field, val.ToSqlString());
        }

        public void Put(string field, DateTime? val)
        {
            _values.Add(field, val == null ? "Null" : ((DateTime)val).ToSqlString());
        }

        public void Put(string field, bool val)
        {
            _values.Add(field, val ? "True" : "False");
        }

        public void Put(string field, bool? val)
        {
            _values.Add(field, val == null ? "Null" : ((bool)val ? "True" : "False"));
        }

        public void PutFromDataGrid(string field, DataGridViewCellCollection cells, string gridField = null)
        {
            var v = cells[gridField ?? field].Value;
            string s = DBNull.Value.Equals(v) ? null : v.ToString();
            _values.Add(field, s);
        }

        public void PutFromDataGrid(string field, DataGridViewRow row, string gridField = null)
        {
            PutFromDataGrid(field, row.Cells, gridField);
        }
    }
}