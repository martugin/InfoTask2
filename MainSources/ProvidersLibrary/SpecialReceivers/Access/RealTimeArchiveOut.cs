using CommonTypes;

namespace ProvidersLibrary
{
    public class RealTimeArchiveOut : ReceiverOut
    {
        public RealTimeArchiveOut(RealTimeAccessReceiver receiver) 
            : base(receiver) { }

        //Id в таблице Signals архива
        public int Id { get; set; }

        //Запись в файл архива (клона)
        public void ValueToRec()
        {
            var r = (RealTimeArchive)Receiver;
            var rec = ValueSignal.DataType.IsReal() ? r.CloneRec : r.CloneStrRec;
            var m = ValueSignal.InValue;
            for (int i = 0; i < m.Count; i++)
            {
                rec.AddNew();
                rec.Put("SignalId", Id);
                rec.Put("Time", m.TimeI(i));
                if (m.ErrorI(i) != null)
                    rec.Put("ErrNum", m.ErrorI(i).Number);
                if (m.DataType.IsReal())
                    m.ValueToRecI(i, rec, "RealValue");
                else if (m.DataType == DataType.String)
                    m.ValueToRecI(i, rec, "StrValue");
                else m.ValueToRecI(i, rec, "TimeValue");
                rec.Update();
            }
                
        }
    }
}