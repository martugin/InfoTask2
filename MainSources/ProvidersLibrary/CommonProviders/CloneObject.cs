using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Объект для клона, содержит один сигнал
    internal class CloneObject : SourceObject
    {
        internal CloneObject(SourceBase source) : base(source) { }

        //Id в клоне
        internal int CloneId { get; set; }

        internal protected override int ReadMoments(IRecordRead rec)
        {
            var time = rec.GetTime("Time");
            var errNum = rec.GetIntNull("ErrNum");
            var err = errNum == null ? null : MakeError((int)errNum);
            if (ValueSignal.DataType.LessOrEquals(DataType.Real))
                return AddMom(ValueSignal, time, rec.GetDouble("RealValue"), err);
            if (ValueSignal.DataType == DataType.String)
                return AddMom(ValueSignal, time, rec.GetString("StrValue"), err);
            if (ValueSignal.DataType == DataType.Time)
                return AddMom(ValueSignal, time, rec.GetTime("TimeValue"), err);
            return 0;
        }
    }
}