using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Объект для клона, содержит один сигнал
    public class CloneObject : SourObject
    {
        public CloneObject(SourConn conn, string codeObject) 
            : base(conn, codeObject) { }

        protected override int AddObjectMoments(IRecordRead rec)
        {
            var time = rec.GetTime("Time");
            var err = MakeError(rec.GetInt("ErrNum"));
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