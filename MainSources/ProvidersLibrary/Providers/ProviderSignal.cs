using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для всех сигналов источников и приемников
    public abstract class ProviderSignal 
    {
        protected ProviderSignal(string code, string codeOut)
        {
            Code = code;
            CodeOuts = codeOut;
        }

        protected ProviderSignal(string code, string codeOut, DataType dataType, string signalInf)
        {
            Code = code;
            CodeOuts = codeOut;
            DataType = dataType;
            Inf = signalInf.ToPropertyDicS();
        }

        //Полный код сигнала
        public string Code { get; private set; }
        //Код объекта + InfOut
        public string CodeOuts { get; private set; }

        //Тип данных 
        public DataType DataType { get; protected set; }

        //Словарь свойств
        public DicS<string> Inf { get; private set; }
    }
}