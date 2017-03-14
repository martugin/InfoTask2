using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace BaseLibrary
{
    //Датасет SQL Server
    public class DataSetSql : IRecordSet
    {
        //stSql - строка запроса, props - свойства соединения
        public DataSetSql(SqlProps props, string stSql)
        {
            _con = SqlDb.Connect(props);
            _adapter = new SqlDataAdapter(stSql, _con) 
                { SelectCommand = {CommandTimeout = 100000} };
            new SqlCommandBuilder(_adapter);
            Reload();
        }

        //Соединение 
        private readonly SqlConnection _con;
        public SqlConnection Connection { get { return _con; } }
        //Адаптер
        private readonly SqlDataAdapter _adapter;
        //Датасет
        private DataSet _dataSet;
        public DataSet DataSet { get { return _dataSet; } }
        //Таблица
        private DataTable _table;
        //Коллекция рядов таблицы
        private DataRowCollection _rows;
        //Текущий ряд и его номер
        private DataRow _row;
        private int _rowNum;
        //True, если курсор передвигался
        private bool _isMove;
        //False, если был изменен, но не обновлен
        private bool _isChanged;

        //Повторно загружает данные из таблицы
        public void Reload()
        {
            _dataSet = new DataSet();
            _adapter.Fill(_dataSet);
            _table = _dataSet.Tables[0];
            _rows = _table.Rows;
            _isMove = false;
            _rowNum = 0;
            _row = HasRows ? _rows[0] : null;
        }

        public bool Read()
        {
            if (_isMove)
            {
                _rowNum++;
                _row = !EOF ? _rows[_rowNum] : null;
            }
            _isMove = true;
            return !EOF;
        }

        public bool EOF { get { return _rowNum >= _rows.Count; }}

        public bool BOF { get { return _rowNum < 0; } }
        
        public void Dispose()
        {
            try { if (_isChanged) Update(); }
            catch { }
            try { _rows.Clear(); } catch {}
            try { _dataSet.Dispose();} catch {}
            try { _adapter.Dispose(); } catch {}
            try { _con.Close(); } catch { }
        }
        
        public bool HasRows
        {
            get { return _rows.Count > 0; }
        }

        public string GetString(string field, string nullValue = null)
        {
            return DBNull.Value.Equals(_row[field]) ? nullValue : (string)_row[field];
        }
        public string GetString(int num, string nullValue = null)
        {
            return DBNull.Value.Equals(_row[num]) ? nullValue : (string)_row[num];
        }

        public double GetDouble(string field, double nullValue = 0)
        {
            return DBNull.Value.Equals(_row[field]) ? nullValue : (double)_row[field];
        }
        public double GetDouble(int num, double nullValue = 0)
        {
            return DBNull.Value.Equals(_row[num]) ? nullValue : (double)_row[num];
        }

        public double? GetDoubleNull(string field)
        {
            if (DBNull.Value.Equals(_row[field])) return null;
            return  (double)_row[field];
        }
        public double? GetDoubleNull(int num)
        {
            if (DBNull.Value.Equals(_row[num])) return null;
            return (double)_row[num];
        }

        public int GetInt(string field, int nullValue = 0)
        {
            return DBNull.Value.Equals(_row[field]) ? nullValue : (int)_row[field];
        }
        public int GetInt(int num, int nullValue = 0)
        {
            return DBNull.Value.Equals(_row[num]) ? nullValue : (int)_row[num];
        }

        public int? GetIntNull(string field)
        {
            if (DBNull.Value.Equals(_row[field])) return null;
            return (int)_row[field];
        }
        public int? GetIntNull(int num)
        {
            if (DBNull.Value.Equals(_row[num])) return null;
            return (int)_row[num];
        }

        public DateTime GetTime(string field)
        {
            return DBNull.Value.Equals(_row[field]) ? Static.MinDate : (DateTime)_row[field];
        }
        public DateTime GetTime(int num)
        {
            return DBNull.Value.Equals(_row[num]) ? Static.MinDate : (DateTime)_row[num];
        }

        public DateTime? GetTimeNull(string field)
        {
            if (DBNull.Value.Equals(_row[field])) return null;
            return (DateTime)_row[field];
        }
        public DateTime? GetTimeNull(int num)
        {
            if (DBNull.Value.Equals(_row[num])) return null;
            return (DateTime)_row[num];
        }

        public bool GetBool(string field, bool nullValue = false)
        {
            return DBNull.Value.Equals(_row[field]) ? nullValue : (bool)_row[field];
        }
        public bool GetBool(int num, bool nullValue = false)
        {
            return DBNull.Value.Equals(_row[num]) ? nullValue : (bool)_row[num];
        }

        public bool? GetBoolNull(string field)
        {
            if (DBNull.Value.Equals(_row[field])) return null;
            return (bool)_row[field];
        }
        public bool? GetBoolNull(int num)
        {
            if (DBNull.Value.Equals(_row[num])) return null;
            return (bool)_row[num];
        }

        public bool IsNull(string field)
        {
            return DBNull.Value.Equals(_row[field]);
        }
        public bool IsNull(int num)
        {
            return DBNull.Value.Equals(_row[num]);
        }

        public void GetToDataGrid(string field, DataGridViewCellCollection cells, string gridField = null)
        {
            cells[gridField ?? field].Value = _row[field];
        }
        public void GetToDataGrid(string field, DataGridViewRow row, string gridField = null)
        {
            GetToDataGrid(field, row.Cells, gridField);
        }

        public void Update()
        {
            _adapter.Update(_dataSet);
            _isChanged = false;
        }

        public void AddNew()
        {
            _isChanged = true;
            _row = _table.NewRow();
            _rows.Add(_row);
            _rowNum = _rows.Count - 1;
        }
        
        public void Put(string field, string val, bool cut = false)
        {
            _isChanged = true;
            if (val == null) _row[field] = DBNull.Value;
            else
            {
                if (val.Length <= _table.Columns[field].MaxLength || !cut) _row[field] = val;
                else _row[field] = val.Substring(0, _table.Columns[field].MaxLength);
            } 
        }
        public void Put(int num, string val, bool cut = false)
        {
            _isChanged = true;
            if (val == null) _row[num] = DBNull.Value;
            else
            {
                if (val.Length <= _table.Columns[num].MaxLength || !cut) _row[num] = val;
                else _row[num] = val.Substring(0, _table.Columns[num].MaxLength);
            }
        }

        public void Put(string field, double val)
        {
            _isChanged = true;
            _row[field] = val;
        }
        public void Put(int num, double val)
        {
            _isChanged = true;
            _row[num] = val;
        }

        public void Put(string field, double? val)
        {
            _isChanged = true;
            if (val == null) _row[field] = DBNull.Value;
            else _row[field] = (double)val;
        }
        public void Put(int num, double? val)
        {
            _isChanged = true;
            if (val == null) _row[num] = DBNull.Value;
            else _row[num] = (double)val;
        }

        public void Put(string field, int val)
        {
            _isChanged = true;
            _row[field] = val;
        }
        public void Put(int num, int val)
        {
            _isChanged = true;
            _row[num] = val;
        }

        public void Put(string field, int? val)
        {
            _isChanged = true;
            if (val == null) _row[field] = DBNull.Value;
            else _row[field] = (int)val;
        }
        public void Put(int num, int? val)
        {
            _isChanged = true;
            if (val == null) _row[num] = DBNull.Value;
            else _row[num] = (int)val;
        }

        public void Put(string field, DateTime val)
        {
            _isChanged = true;
            if (val < Static.MinDate) _row[field] = Static.MinDate;
            else if (val > Static.MaxDate) _row[field] = Static.MaxDate;
            else _row[field] = val;
        }
        public void Put(int num, DateTime val)
        {
            _isChanged = true;
            if (val < Static.MinDate) _row[num] = Static.MinDate;
            else if (val > Static.MaxDate) _row[num] = Static.MaxDate;
            else _row[num] = val;
        }

        public void Put(string field, DateTime? val)
        {
            _isChanged = true;
            if (val == null) _row[field] = DBNull.Value;
            else if ((DateTime)val < Static.MinDate) _row[field]  = Static.MinDate;
            else if ((DateTime)val > Static.MaxDate) _row[field] = Static.MaxDate;
            else Put(field, (DateTime)val);
        }
        public void Put(int num, DateTime? val)
        {
            _isChanged = true;
            if (val == null) _row[num] = DBNull.Value;
            else if ((DateTime)val < Static.MinDate) _row[num] = Static.MinDate;
            else if ((DateTime)val > Static.MaxDate) _row[num] = Static.MaxDate;
            else Put(num, (DateTime)val);
        }

        public void Put(string field, bool val)
        {
            _isChanged = true;
            _row[field] = val;
        }
        public void Put(int num, bool val)
        {
            _isChanged = true;
            _row[num] = val;
        }

        public void Put(string field, bool? val)
        {
            _isChanged = true;
            if (val == null) _row[field] = DBNull.Value;
            else _row[field] = (bool)val;
        }
        public void Put(int num, bool? val)
        {
            _isChanged = true;
            if (val == null) _row[num] = DBNull.Value;
            else _row[num] = (bool)val;
        }

        public void PutFromDataGrid(string field, DataGridViewCellCollection cells, string gridField = null)
        {
            _isChanged = true;
            _row[field] = cells[gridField ?? field].Value;
        }
        public void PutFromDataGrid(string field, DataGridViewRow row, string gridField = null)
        {
            PutFromDataGrid(field, row.Cells, gridField);
        }

        public int RecordCount { get { return _rows.Count; } }

        public bool MoveFirst()
        {
            _rowNum = 0;
            _row = HasRows ? _rows[0] : null;
            _isMove = true;
            return _row != null;
        }

        public bool MoveLast()
        {
            _rowNum = _rows.Count - 1;
            _row = HasRows ? _rows[_rows.Count - 1] : null;
            _isMove = true;
            return _row != null;
        }

        public bool MoveNext()
        {
            if (EOF) throw new Exception("Текущая запись отсутствует");
            _rowNum++;
            _row = EOF ? null : _rows[_rowNum];
            _isMove = true;
            return _row != null;
        }

        public bool MovePrevious()
        {
            if (BOF) throw new Exception("Текущая запись отсутствует");
            _rowNum--;
            _row = BOF ? null : _rows[_rowNum];
            _isMove = true;
            return _row != null;
        }

        public bool Find(int key)
        {
            try
            {
                _isMove = true;
                _row = _rows.Find(key);
                _rowNum = _rows.IndexOf(_row);
                return true;
            }
            catch { return false;}
        }

        public bool Find(string key)
        {
            try
            {
                _isMove = true;
                _row = _rows.Find(key);
                _rowNum = _rows.IndexOf(_row);
                return true;
            }
            catch { return false; }
        }

        private bool StrEqual(int num, string val, string field)
        {
            bool isnull = DBNull.Value.Equals(_rows[num][field]);
            if (isnull && val == null) return true;
            if (!isnull && val != null)
                return ((string) _rows[num][field]).ToUpper() == val.ToUpper();
            return false;
        }

        private bool IntEqual(int num, int val, string field)
        {
            return !DBNull.Value.Equals(_rows[num][field]) && (int)_rows[num][field] == val;
        }

        public bool FindFirst(string field, string val)
        {
            _rowNum = -1;
            return FindNext(field, val);
        }

        public bool FindFirst(string field, int val)
        {
            _rowNum = -1;
            return FindNext(field, val);
        }

        public bool FindLast(string field, string val)
        {
            _rowNum = _rows.Count;
            return FindPrevious(field, val);
        }

        public bool FindLast(string field, int val)
        {
            _rowNum = _rows.Count;
            return FindPrevious(field, val);
        }

        public bool FindNext(string field, string val)
        {
            _isMove = true;
            if (!HasRows) return false;
            while (Read() && !StrEqual(_rowNum, val, field)) { }
            _row = EOF ? null : _rows[_rowNum];
            return !EOF;
        }

        public bool FindNext(string field, int val)
        {
            _isMove = true;
            if (!HasRows) return false;
            while (Read() && !IntEqual(_rowNum, val, field)) { }
            _row = EOF ? null : _rows[_rowNum];
            return !EOF;
        }

        public bool FindPrevious(string field, string val)
        {
            _isMove = true;
            if (!HasRows) return false;
            while (MovePrevious() && !StrEqual(_rowNum, val, field)) { }
            _row = BOF ? null : _rows[_rowNum];
            return !BOF;
        }

        public bool FindPrevious(string field, int val)
        {
            _isMove = true;
            if (!HasRows) return false;
            while (MovePrevious() && !IntEqual(_rowNum, val, field)) { }
            _row = BOF ? null : _rows[_rowNum];
            return !BOF;
        }
    }
}