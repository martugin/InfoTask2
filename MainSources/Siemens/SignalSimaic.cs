using BaseLibrary;
using CommonTypes;

namespace Provider
{
    //Сигнал
    internal class SignalSimatic : SourceSignal
    {
        public SignalSimatic(string signalInf, string code, DataType dataType, ISource provider, bool skipRepeats, int idInClone = 0) 
            : base(signalInf, code, dataType, provider, skipRepeats, idInClone)
        {
            Id = Inf.GetInt("Id"); 
        }

        //Id в таблице архива
        public int Id { get; private set; }
    }
}