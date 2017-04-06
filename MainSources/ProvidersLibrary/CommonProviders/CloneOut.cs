using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Объект для клона, содержит один сигнал
    internal class CloneOut : SourceOut
    {
        internal CloneOut(BaseSource source) : base(source) { }

        //Чтение одного значения из рекордсета клона
        protected internal override int ReadMoments(IRecordRead rec)
        {
            var time = rec.GetTime("Time");
            var errNum = rec.GetIntNull("ErrNum");
            var err = errNum == null ? null : MakeError((int)errNum);
            if (ValueSignal.DataType.LessOrEquals(DataType.Real))
                return AddMomReal(ValueSignal, time, rec, "RealValue", err);
            return ValueSignal.DataType == DataType.Time 
                ? AddMomTime(ValueSignal, time, rec, "TimeValue", err) 
                : AddMomString(ValueSignal, time, rec, "StrValue", err);
        }
    }
}