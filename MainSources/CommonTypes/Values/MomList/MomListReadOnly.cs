using System;
using BaseLibrary;

namespace CommonTypes
{
    //Список значений без возможности редактирования
    public class MomListReadOnly : CalcVal, IMomListReadOnly
    {
        public MomListReadOnly(MomList momList)
        {
            _momList = momList;
        }

        //Обертываемый список значений
        private readonly MomList _momList;

        public override DataType DataType { get { return _momList.DataType; } }
        public override ErrMom TotalError { get { return _momList.TotalError; } }
        public bool Boolean { get { return _momList.Boolean; } }
        public int Integer { get { return _momList.Integer; } }
        public double Real { get { return _momList.Real; } }
        public DateTime Date { get { return _momList.Date; } }
        public string String { get { return _momList.String; } }
        public object Object { get { return _momList.Object; } }
        public ErrMom Error { get { return _momList.Error; } }
        public bool ValueEquals(IMean mean) { return _momList.ValueEquals(mean); }
        public bool ValueLess(IMean mean) { return _momList.ValueLess(mean); }
        public bool ValueAndErrorEquals(IMean mean) { return _momList.ValueAndErrorEquals(mean); }
        public void ValueToRec(IRecordAdd rec, string field) { _momList.ValueToRec(rec, field); }
        public IMom Clone() { return _momList.Clone(); }
        public IMom Clone(DateTime time) { return _momList.Clone(time); }
        public IMom Clone(DateTime time, ErrMom err) { return _momList.Clone(time, err); }
        public int Count { get { return _momList.Count; } }
        public IMean LastMean { get { return _momList.LastMean; } }
        public DateTime Time { get { return _momList.Time; } }
        public int CurNum { get { return _momList.CurNum; } set { _momList.CurNum = value; } }
        public DateTime GetTime(int i) { return _momList.GetTime(i); }
        public ErrMom GetError(int i) { return _momList.GetError(i); }
        public bool GetBoolean(int i) { return _momList.GetBoolean(i); }
        public int GetInteger(int i) { return _momList.GetInteger(i); }
        public double GetReal(int i) { return _momList.GetReal(i); }
        public DateTime GetDate(int i) { return _momList.GetDate(i); }
        public string GetString(int i) { return _momList.GetString(i); }
        public IMom Clone(int i) { return _momList.Clone(i); }
        public IMom Clone(int i, DateTime time) { return _momList.Clone(i, time); }
        public IMom Clone(int i, DateTime time, ErrMom err) { return _momList.Clone(i, time, err); }
    }
}