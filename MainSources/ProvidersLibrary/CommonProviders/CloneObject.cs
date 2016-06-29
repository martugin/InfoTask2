using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Объект для клона, содержит один сигнал
    public class CloneObject : SourceObject
    {
        public CloneObject(SourceBase source) : base(source) { }

        public override int ReadMoments(IRecordRead rec)
        {
            var time = rec.GetTime("Time");
            var errNum = rec.GetIntNull("ErrNum");
            var err = errNum == null ? null : MakeError((int)errNum);
            if (ValueSignal.IsReal)
                return AddMom(ValueSignal, time, rec.GetDouble("RealValue"), err);
            if (ValueSignal.DataType == DataType.String)
                return AddMom(ValueSignal, time, rec.GetDouble("StrValue"), err);
            if (ValueSignal.DataType == DataType.Time)
                return AddMom(ValueSignal, time, rec.GetDouble("TimeValue"), err);
            return 0;
        }
    }
}