using System;
using System.Collections.Generic;
using System.Data;

namespace BaseLibrary
{
    internal class BulkStarter : IDataReader
    {
        public BulkStarter(int fieldCount, List<object[]> table)
        {
            _fieldCount = fieldCount;
            _table = table;
        }

        //Список строк значений, каджая строка - массив значений колонок
        private readonly List<object[]> _table;
        //Текущая строка данных для добавления и чтения
        private object[] _row;
        //Номер текущей строки для чтения
        private int _rowNum = -1;

        //Возвращает количество столбцов в источнике данных
        private readonly int _fieldCount;
        public int FieldCount { get { return _fieldCount; } }

        //Читает очередную строку. Возвращает true — если конец файла/источника не достигнут, иначе false
        public bool Read()
        {
            if (_rowNum + 1 >= _table.Count) return false;
            _row = _table[++_rowNum];
            return true;
        }

        //Возвращает значение с указанным индексом для текущей строки
        public object GetValue(int i)
        {
            return _row[i] ?? DBNull.Value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        //Не нужные методы IDataReader
        public void Close() { throw new NotImplementedException(); }
        public void Dispose() { throw new NotImplementedException(); }
        public string GetName(int i) { throw new NotImplementedException(); }
        public string GetDataTypeName(int i) { throw new NotImplementedException(); }
        public Type GetFieldType(int i) { throw new NotImplementedException(); }
        public int GetValues(object[] values) { throw new NotImplementedException(); }
        public int GetOrdinal(string name) { throw new NotImplementedException(); }
        public bool GetBoolean(int i) { throw new NotImplementedException(); }
        public byte GetByte(int i) { throw new NotImplementedException(); }
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) { throw new NotImplementedException(); }
        public char GetChar(int i) { throw new NotImplementedException(); }
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) { throw new NotImplementedException(); }
        public Guid GetGuid(int i) { throw new NotImplementedException(); }
        public short GetInt16(int i) { throw new NotImplementedException(); }
        public int GetInt32(int i) { throw new NotImplementedException(); }
        public long GetInt64(int i) { throw new NotImplementedException(); }
        public float GetFloat(int i) { throw new NotImplementedException(); }
        public double GetDouble(int i) { throw new NotImplementedException(); }
        public string GetString(int i) { throw new NotImplementedException(); }
        public decimal GetDecimal(int i) { throw new NotImplementedException(); }
        public DateTime GetDateTime(int i) { throw new NotImplementedException(); }
        public IDataReader GetData(int i) { throw new NotImplementedException(); }
        public bool IsDBNull(int i) { throw new NotImplementedException(); }
        object IDataRecord.this[int i] { get { throw new NotImplementedException(); } }
        object IDataRecord.this[string name] { get { throw new NotImplementedException(); } }
        public DataTable GetSchemaTable() { throw new NotImplementedException(); }
        public bool NextResult() { throw new NotImplementedException(); }
        public int Depth { get { throw new NotImplementedException(); } }
        public bool IsClosed { get { throw new NotImplementedException(); } }
        public int RecordsAffected { get { throw new NotImplementedException(); } }
    }
}