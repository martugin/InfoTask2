using BaseLibrary;
using CommonTypes;

namespace Provider
{
    //Один сигнал источника Овации
    internal class SignalOvation : SourceSignal
    {
        internal SignalOvation(string signalInf, string code, DataType dataType, ISource provider, bool skipRepeats, int idInClone)
            : base(signalInf, code, dataType, provider, skipRepeats, idInClone)
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