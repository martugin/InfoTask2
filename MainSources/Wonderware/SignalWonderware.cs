using BaseLibrary;
using CommonTypes;

namespace Provider
{
    //Сигнал
    internal class SignalWonderware : SourceSignal
    {
        internal SignalWonderware(string signalInf, string code, DataType dataType, ISource provider, bool skipRepeats, int idInClone)
            : base(signalInf, code, dataType, provider, skipRepeats, idInClone)
        {
            TagName = Inf["TagName"];
        }

        //Имя тэга 
        public string TagName { get; private set; }
    }
}