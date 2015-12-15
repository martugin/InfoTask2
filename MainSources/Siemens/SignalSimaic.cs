using BaseLibrary;
using CommonTypes;

namespace Provider
{
    //Сигнал
    internal class SignalSimatic : SourceSignal
    {
        public SignalSimatic(string signalInf, string code, DataType dataType, IProvider provider, int idInClone = 0) 
            : base(signalInf, code, dataType, provider, idInClone)
        {
            Id = Inf.GetInt("Id");
        }

        //Id в таблице архива
        public int Id { get; private set; }
    }
}