using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace BaseLibrary
{
    //Класс, который содержит все данные для записи в базу SQL и реализует интерфейс для применения SqlBulkCopy
    public class SqlBulk : IRecordAdd
    {
        public SqlBulk(SqlProps props, string tabl)
        {
            _tabl = tabl;
            _props = props;
            //Получение списка колонок таблицы
            using ( var rec = new AdoReader(props, "SELECT fld.name, fld.length FROM sysobjects tab, syscolumns fld WHERE tab.id=fld.id And tab.name='" + tabl + "'"))
            {
                int i = 0;
                while (rec.Read())
                {
                    _cols.Add(rec.GetString("name"), i++);
                    _colsSize.Add(rec.GetString("name"), rec.GetInt("length"));
                }
                _starter = new BulkStarter(_cols.Count, _table);    
            }
        }

        //Свойства соединения с базой данных
        private readonly SqlProps _props;
        //Таблица 
        private readonly string _tabl;
        //Объект и интерфейсом IDataReader для SqlBulkCopy
        private readonly BulkStarter _starter;

        //Словарь, ключи - названия колонок, значения их номера
        private readonly Dictionary<string, int> _cols = new Dictionary<string, int>();
        //Размеры для текстовых колонок
        private readonly Dictionary<string, int> _colsSize = new Dictionary<string, int>();
        //Список строк значений, каджая строка - массив значений колонок
        private readonly List<object[]> _table = new List<object[]>();
        //Текущая строка данных для добавления и чтения
        private object[] _row;
        
        public void AddNew()
        {
            _row = new object[_cols.Count];
            _table.Add(_row);
        }

        public void Put(string field, string val, bool cut = false)
        {
            if (val == null || !cut) _row[_cols[field]] = val;
            else _row[_cols[field]] = val.Length <= _colsSize[field] ? val : val.Substring(0, _colsSize[field]);
        }

        public void Put(string field, double val)
        {
            _row[_cols[field]] = val;
        }

        public void Put(string field, double? val)
        {
            _row[_cols[field]] = val;
        }

        public void Put(string field, int val)
        {
            _row[_cols[field]] = val;
        }

        public void Put(string field, int? val)
        {
            _row[_cols[field]] = val;
        }

        public void Put(string field, DateTime val)
        {
            if (val < Static.MinDate) _row[_cols[field]] = Static.MinDate;
            else if (val > Static.MaxDate) _row[_cols[field]] = Static.MaxDate;
            else _row[_cols[field]] = val;
        }

        public void Put(string field, DateTime? val)
        {
            if (val == null) _row[_cols[field]] = null;
            else Put(field, (DateTime)val);
        }

        public void Put(string field, bool val)
        {
            _row[_cols[field]] = val;
        }

        public void Put(string field, bool? val)
        {
            _row[_cols[field]] = val;
        }

        public void PutFromDataGrid(string field, DataGridViewCellCollection cells, string gridField = null)
        {
            _row[_cols[field]] = cells[gridField ?? field];
        }
        public void PutFromDataGrid(string field, DataGridViewRow row, string gridField = null)
        {
            PutFromDataGrid(field, row.Cells, gridField);
        }

        //Запись из себя через SqlBulkCopy
        public void Update()
        {
            using (var con = SqlDb.Connect(_props))
                using (var loader = new SqlBulkCopy(con) {DestinationTableName = _tabl, BulkCopyTimeout = 200000})
                    loader.WriteToServer(_starter);
        }

        public void Dispose()
        {
            try { _starter.Dispose(); } catch { }
        }
    }
}