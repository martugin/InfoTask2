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
        //Загрузить в буферное значение
        public Mean Mean(int i) { return _momList.Mean(i);}

        public override DataType DataType { get { return _momList.DataType; } }
        public override ErrMom TotalError { get { return _momList.TotalError; } }
        public int Count { get { return _momList.Count; } }
        public IMean LastMean { get { return _momList.LastMean; } }
        public DateTime Time(int i) { return _momList.Time(i); }
        public ErrMom Error(int i) { return _momList.Error(i); }
        public bool Boolean(int i) { return _momList.Boolean(i); }
        public int Integer(int i) { return _momList.Integer(i); }
        public double Real(int i) { return _momList.Real(i); }
        public DateTime Date(int i) { return _momList.Date(i); }
        public string String(int i) { return _momList.String(i); }
        public void ValueToRec(IRecordAdd rec, string field, int i) { _momList.ValueToRec(rec, field, i); }
        
        public IMom Clone(int i) { return _momList.Clone(i); }
        public IMom Clone(int i, DateTime time) { return _momList.Clone(i, time); }
        public IMom Clone(int i, DateTime time, ErrMom err) { return _momList.Clone(i, time, err); }
    }
}