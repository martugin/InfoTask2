using System;
using BaseLibrary;

namespace CommonTypes
{
    //Список значений без возможности редактирования
    public class MomListRead : CalcVal, IMomListRead
    {
        public MomListRead(MomList momList)
        {
            _momList = momList;
        }

        //Обертываемый список значений
        private readonly MomList _momList;
        //Загрузить в буферное значение
        public IMean MeanI(int i) { return _momList.MeanI(i);}

        public override DataType DataType { get { return _momList.DataType; } }
        public override ErrMom TotalError { get { return _momList.TotalError; } }
        public int Count { get { return _momList.Count; } }
        public IMean LastMean { get { return _momList.LastMean; } }
        public DateTime TimeI(int i) { return _momList.TimeI(i); }
        public ErrMom ErrorI(int i) { return _momList.ErrorI(i); }
        public bool BooleanI(int i) { return _momList.BooleanI(i); }
        public int IntegerI(int i) { return _momList.IntegerI(i); }
        public double RealI(int i) { return _momList.RealI(i); }
        public DateTime DateI(int i) { return _momList.DateI(i); }
        public string StringI(int i) { return _momList.StringI(i); }
        public void ValueToRecI(IRecordAdd rec, string field, int i) { _momList.ValueToRecI(rec, field, i); }
        public IMean CloneMeanI(int i) { return _momList.CloneMeanI(i); }
        public IMean CloneMeanI(int i, ErrMom err) { return _momList.CloneMeanI(i, err); }
        public IMom CloneMomI(int i) { return _momList.CloneMomI(i); }
        public IMom CloneMomI(int i, ErrMom err) { return _momList.CloneMomI(i, err);}
        public IMom CloneMomI(int i, DateTime time) { return _momList.CloneMomI(i, time); }
        public IMom CloneMomI(int i, DateTime time, ErrMom err) { return _momList.CloneMomI(i, time, err); }
        
    }
}