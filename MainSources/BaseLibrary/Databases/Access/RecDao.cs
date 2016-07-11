using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Office.Interop.Access.Dao;

namespace BaseLibrary
{
    //Обертка для рекордсета DAO
    //Ускоряет работу с полями, используюя свой словарь
    //Автоматизирует Edit и Update
    //Обрабатывает запись и чтение значений разных типов, переводит DbNull во что надо
    public class RecDao : IRecordSet
    {
        //True - конструктор вызван с указанием соединения, False - с указанием файла Access или списка свойств SQL Server
        private readonly bool _useDb;
        //Ссылка на рекордсет
        public Recordset Recordset { get; private set; }
        //Ссылки на базу данных
        public DaoDb DaoDb { get; private set; }
        //Словари полей рекордсета, ключи имена или номера полей
        private readonly Dictionary<string, Field> _fields = new Dictionary<string, Field>();
        private readonly Dictionary<int, Field> _fieldsNum = new Dictionary<int, Field>();

        //Существует ли поле в рекордсете
        public bool ContainsField(string fname)
        {
            return _fields.ContainsKey(fname);
        }
        //Количество полей рекордсета
        public int FieldsCount { get { return _fields.Count; } }


        //Открывает рекордсет, при закрытии база данных не будет закрыта
        public RecDao(DaoDb daodb, string stSql, object type = null, object options = null, object lockEdit = null)
        {
            _useDb = true;
            DaoDb = daodb.ConnectDao();
            Open(daodb.Database, stSql, type, options, lockEdit);
        }

        //Открывает рекордсет, при закрытии база данных будет закрыта
        public RecDao(string file, string stSql, object type = null, object options = null, object lockEdit = null)
        {
            _useDb = false;
            DaoDb = new DaoDb(file).ConnectDao();
            Open(DaoDb.Database, stSql, type, options, lockEdit);
        }

        private void Open(Database db, string stSql, object type = null, object options = null, object lockEdit = null)
        {
            if (db == null || stSql.IsEmpty())
                throw new NullReferenceException("База данных и строка запроса не могут быть равными null");
            if (type == null) Recordset = db.OpenRecordset(stSql, RecordsetTypeEnum.dbOpenDynaset);
            else if (options == null) Recordset = db.OpenRecordset(stSql, type);
            else if (lockEdit == null) Recordset = db.OpenRecordset(stSql, type, options);
            else Recordset = db.OpenRecordset(stSql, type, options, lockEdit);
            for (int i = 0; i < Recordset.Fields.Count; i++)
            {
                _fields.Add(Recordset.Fields[i].Name, Recordset.Fields[i]);
                _fieldsNum.Add(i, Recordset.Fields[i]);
            }
            if (Recordset.RecordCount != 0)
                Recordset.MoveFirst();
            _isEdit = false;
            _isMove = false;  
        }

        //Закрытие
        public void Dispose()
        {
            try { if (_isEdit) Recordset.Update(); }
            catch { }
            try
            {
                Recordset.Close();
                Recordset = null;
            }
            catch { }
             if (!_useDb) DaoDb.Dispose();
        }
        
        //Количество записей
        public int RecordCount
        {
            get
            {
                if (Recordset.RecordCount == 0) return 0;
                Recordset.MoveLast();
                return Recordset.RecordCount;
            }
        }
        //True, если содержит записи
        public bool HasRows
        {
            get { return Recordset.RecordCount != 0; }
        }

        //Текущая запись после конца таблицы
        public bool EOF { get { return !HasRows || Recordset.EOF; } }

        //Текущая запись до начала таблицы
        public bool BOF { get { return !HasRows || Recordset.BOF; } }

        //True, если началось редактирование
        private bool _isEdit;
        //True, если курсор передвигался
        private bool _isMove;

        //Переводит значение поля field таблицы rec в строку, DbNull переводит в nullValue
        public string GetString(string field, string nullValue = null)
        {
            var v = _fields[field].Value;
            return DBNull.Value.Equals(v) ? nullValue : (string)Convert.ToString(v);    
        }
        public string GetString(int num, string nullValue = null)
        {
            var v = _fieldsNum[num].Value;
            return DBNull.Value.Equals(v) ? nullValue : (string)Convert.ToString(v);
        }

        //Переводит значение поля field таблицы rec в число, DbNull переводит в nullValue
        public double GetDouble(string field, double nullValue = 0)
        {
            var v = _fields[field].Value;
            return DBNull.Value.Equals(v) ? nullValue : (double)Convert.ToDouble(v);
        }
        public double GetDouble(int num, double nullValue = 0)
        {
            var v = _fieldsNum[num].Value;
            return DBNull.Value.Equals(v) ? nullValue : (double)Convert.ToDouble(v);
        }
        //Переводит значение поля field таблицы rec в число, DbNull переводит в null
        public double? GetDoubleNull(string field)
        {
            var v = _fields[field].Value;
            if (DBNull.Value.Equals(v)) return null;
            return (double)Convert.ToDouble(v);
        }
        public double? GetDoubleNull(int num)
        {
            var v = _fieldsNum[num].Value;
            if (DBNull.Value.Equals(v)) return null;
            return (double)Convert.ToDouble(v);
        }

        //Переводит значение поля field таблицы rec в число, DbNull переводит в nullValue
        public int GetInt(string field, int nullValue = 0)
        {
            var v = _fields[field].Value;
            return DBNull.Value.Equals(v) ? nullValue : (int)Convert.ToInt32(v);
        }
        public int GetInt(int num, int nullValue = 0)
        {
            var v = _fieldsNum[num].Value;
            return DBNull.Value.Equals(v) ? nullValue : (int)Convert.ToInt32(v);
        }
        //Переводит значение поля field таблицы rec в число, DbNull переводит в null
        public int? GetIntNull(string field)
        {
            var v = _fields[field].Value;
            if (DBNull.Value.Equals(v)) return null;
            return (int)Convert.ToInt32(v);
        }
        public int? GetIntNull(int num)
        {
            var v = _fieldsNum[num].Value;
            if (DBNull.Value.Equals(v)) return null;
            return (int)Convert.ToInt32(v);
        }

        //Переводит значение поля field таблицы rec в дату, DbNull переводит в Different.MinDate
        public DateTime GetTime(string field)
        {
            var v = _fields[field].Value;
            return DBNull.Value.Equals(v) ? Different.MinDate : (DateTime)Convert.ToDateTime(v);
        }
        public DateTime GetTime(int num)
        {
            var v = _fieldsNum[num].Value;
            return DBNull.Value.Equals(v) ? Different.MinDate : (DateTime)Convert.ToDateTime(v);
        }
        //Переводит значение поля field таблицы rec в дату, DbNull переводит в null
        public DateTime? GetTimeNull(string field)
        {
            var v = _fields[field].Value;
            if (DBNull.Value.Equals(v)) return null;
            return (DateTime)Convert.ToDateTime(v);
        }
        public DateTime? GetTimeNull(int num)
        {
            var v = _fieldsNum[num].Value;
            if (DBNull.Value.Equals(v)) return null;
            return (DateTime)Convert.ToDateTime(v);
        }

        //Переводит значение поля field таблицы rec в bool, DbNull переводит в nullValue
        public bool GetBool(string field, bool nullValue = false)
        {
            var v = _fields[field].Value;
            return DBNull.Value.Equals(v) ? nullValue : (bool)Convert.ToBoolean(v);
        }
        public bool GetBool(int num, bool nullValue = false)
        {
            var v = _fieldsNum[num].Value;
            return DBNull.Value.Equals(v) ? nullValue : (bool)Convert.ToBoolean(v);
        }
        //Переводит значение поля field таблицы rec в bool, DbNull переводит в null
        public bool? GetBoolNull(string field)
        {
            var v = _fields[field].Value;
            if (DBNull.Value.Equals(v)) return null;
            return (bool)Convert.ToBoolean(v);
        }
        public bool? GetBoolNull(int num)
        {
            var v = _fieldsNum[num].Value;
            if (DBNull.Value.Equals(v)) return null;
            return (bool)Convert.ToBoolean(v);
        }

        //Проверяет, равно ли значение заданного поля null
        public bool IsNull(string field)
        {
            return DBNull.Value.Equals(_fields[field].Value);
        }
        public bool IsNull(int num)
        {
            return DBNull.Value.Equals(_fieldsNum[num].Value);
        }

        //Копирует строковое значение поля field таблицы rec в ячейку gridField, строки cells, датагрида WinForms
        public void GetToDataGrid(string field, DataGridViewCellCollection cells, string gridField = null)
        {
            cells[gridField ?? field].Value = _fields[field].Value;
        }
        public void GetToDataGrid(string field, DataGridViewRow row, string gridField = null)
        {
            GetToDataGrid(field, row.Cells, gridField);
        }

        //Готовит строку для записи значения и выкидывает исключения, если нужно
        private void BeginEdit()
        {
            if (!_isEdit)
            {
                try
                {
                    Recordset.Edit();
                }
                catch (COMException ex)
                {
                    if (ex.Message.StartsWith("Превышено число блокировок") || ex.Message.StartsWith("Обновление невозможно"))
                    {
                        Thread.Sleep(1);
                        Recordset.Edit();    
                    }
                }
                _isEdit = true;
            }        
        }

        //Записывает значение в рекодсет, null переводит в DbNull
        //если cut = true, то обрезает если не влазит в поле
        public void Put(string field, string val, bool cut = false)
        {
            BeginEdit();
            var f = _fields[field];
            if (val == null) f.Value = DBNull.Value;
            else
            {
                if (val.Length <= f.Size || !cut) f.Value = val;
                else f.Value = val.Substring(0, f.Size);
            }
        }
        public void Put(int num, string val, bool cut = false)
        {
            BeginEdit();
            var f = _fieldsNum[num];
            if (val == null) f.Value = DBNull.Value;
            else
            {
                if (val.Length <= f.Size || !cut) f.Value = val;
                else f.Value = val.Substring(0, f.Size);
            }
        }

        public void Put(string field, double val)
        {
            BeginEdit();    
            _fields[field].Value = val;
        }
        public void Put(int num, double val)
        {
            BeginEdit();
            _fieldsNum[num].Value = val;
        }
        public void Put(string field, double? val)
        {
            BeginEdit();    
            if (val == null) _fields[field].Value = DBNull.Value;
            else _fields[field].Value = val;
        }
        public void Put(int num, double? val)
        {
            BeginEdit();
            if (val == null) _fieldsNum[num].Value = DBNull.Value;
            else _fieldsNum[num].Value = val;
        }

        public void Put(string field, int val)
        {
            BeginEdit();   
            _fields[field].Value = val;
        }
        public void Put(int num, int val)
        {
            BeginEdit();
            _fieldsNum[num].Value = val;
        }
        public void Put(string field, int? val)
        {
            BeginEdit();
            if (val == null) _fields[field].Value = DBNull.Value;
            else _fields[field].Value = val;
        }
        public void Put(int num, int? val)
        {
            BeginEdit();
            if (val == null) _fieldsNum[num].Value = DBNull.Value;
            else _fieldsNum[num].Value = val;
        }

        public void Put(string field, DateTime val)
        {
            BeginEdit();  
            _fields[field].Value = val;
        }
        public void Put(int num, DateTime val)
        {
            BeginEdit();
            _fieldsNum[num].Value = val;
        }
        public void Put(string field, DateTime? val)
        {
            BeginEdit();
            if (val == null) _fields[field].Value = DBNull.Value;
            else _fields[field].Value = val;
        }
        public void Put(int num, DateTime? val)
        {
            BeginEdit();
            if (val == null) _fieldsNum[num].Value = DBNull.Value;
            else _fieldsNum[num].Value = val;
        }

        public void Put(string field, bool val)
        {
            BeginEdit();
            _fields[field].Value = val;
        }
        public void Put(int num, bool val)
        {
            BeginEdit();
            _fieldsNum[num].Value = val;
        }
        public void Put(string field, bool? val)
        {
            BeginEdit();
            if (val == null) _fields[field].Value = DBNull.Value;
            else _fields[field].Value = val;
        }
        public void Put(int num, bool? val)
        {
            BeginEdit();
            if (val == null) _fieldsNum[num].Value = DBNull.Value;
            else _fieldsNum[num].Value = val;
        }

        //Копирует строковое значение в поле field таблицы rec из ячейки gridField, строки cells, датагрида WinForms
        public void PutFromDataGrid(string field, DataGridViewCellCollection cells, string gridField = null)
        {
            BeginEdit();
            _fields[field].Value = cells[gridField ?? field].Value;
        }
        public void PutFromDataGrid(string field, DataGridViewRow row, string gridField = null)
        {
            PutFromDataGrid(field, row.Cells, gridField);
        }

        //Поведение как у Read для DataReader
        public bool Read()
        {
            if (_isMove) MoveNext();
            _isMove = true;
            return !EOF;    
        }

        //Переопределение стандартных операций
        public void Update()
        {            
            if (_isEdit)
            {
                _isEdit = false;
                Recordset.Update();
            }
        }

        public bool MoveFirst()
        {
            if (HasRows)
            {
                Update();
                Recordset.MoveFirst();
                _isMove = true;
            }
            return HasRows;
        }

        public bool MoveLast()
        {
            if (HasRows)
            {
                Update();
                Recordset.MoveLast();
                _isMove = true;        
            }
            return HasRows;
        }

        public bool MoveNext()
        {
            if (HasRows)
            {
                Update();
                Recordset.MoveNext();
                _isMove = true;
            }
            return !EOF;
        }

        public bool MovePrevious()
        {
            if (HasRows)
            {
                Update();
                Recordset.MovePrevious();
                _isMove = true;
            }
            return !BOF;
        }

        public void AddNew()
        {
            Update();
            Recordset.AddNew();
            _isEdit = true;
            _isMove = true;
        }

        public void Delete()
        {
            _isEdit = false;
            Recordset.Delete();
            _isMove = true;
        }

        public bool FindFirst(string criteria)
        {
            Update();
            Recordset.FindFirst(criteria);
            _isMove = true;
            return !Recordset.NoMatch;
        }

        public bool FindLast(string criteria)
        {
            Update();
            Recordset.FindLast(criteria);
            _isMove = true;
            return !Recordset.NoMatch;
        }

        public bool FindNext(string criteria)
        {
            Update();
            Recordset.FindNext(criteria);
            _isMove = true;
            return !Recordset.NoMatch;
        }

        public bool FindPrevious(string criteria)
        {
            Update();
            Recordset.FindNext(criteria);
            _isMove = true;
            return !Recordset.NoMatch;
        }

        public bool NoMatch
        {
            get { return Recordset.NoMatch; }
        }

        public bool FindFirst(string field, string val)
        {
            if (val == null) return FindFirst(field + " Is Null");
            return FindFirst(field + "='" + val + "'");
        }

        public bool FindFirst(string field, int val)
        {
            return FindFirst(field + "=" + val);
        }

        public bool FindLast(string field, string val)
        {
            if (val == null) return FindLast(field + " Is Null");
            return FindLast(field + "='" + val + "'");
        }

        public bool FindLast(string field, int val)
        {
            return FindLast(field + "=" + val);
        }

        public bool FindNext(string field, string val)
        {
            if (val == null) return FindNext(field + " Is Null");
            return FindNext(field + "='" + val + "'");
        }

        public bool FindNext(string field, int val)
        {
            return FindNext(field + "=" + val);
        }

        public bool FindPrevious(string field, string val)
        {
            if (val == null) return FindPrevious(field + " Is Null");
            return FindPrevious(field + "='" + val + "'");
        }

        public bool FindPrevious(string field, int val)
        {
            return FindPrevious(field + "=" + val);
        }
    }
}
