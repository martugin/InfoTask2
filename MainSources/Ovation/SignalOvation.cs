using BaseLibrary;
using CommonTypes;

namespace Provider
{
    //Один сигнал (с учетом бита)
    internal class SignalOvation : SourceSignal
    {
        internal SignalOvation(string signalInf, string code, DataType dataType, IProvider provider, int idInClone)
            : base(signalInf, code, dataType, provider, idInClone)
        {
            Id = Inf.GetInt("Id");
            IsState = Inf["Prop"] == "STAT";
        }

        //Является сигналом состояния
        internal bool IsState { get; private set; }
        //Id в Historian
        internal int Id { get; private set; }
    }
}