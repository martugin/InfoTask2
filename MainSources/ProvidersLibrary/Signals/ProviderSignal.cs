using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для всех сигналов источников и приемников
    public abstract class ProviderSignal 
    {
        protected ProviderSignal(string code, string codeObject)
        {
            Code = code;
            CodeObject = codeObject;
        }

        protected ProviderSignal(string code, string codeObject, DataType dataType, string signalInf)
        {
            Code = code;
            CodeObject = codeObject;
            DataType = dataType;
            Inf = signalInf.ToPropertyDicS();
        }

        //Полный код сигнала
        public string Code { get; private set; }
        //Код объекта
        public string CodeObject { get; private set; }


        //Тип данных 
        public DataType DataType { get; protected set; }

        //Словарь свойств
        public DicS<string> Inf { get; private set; }
    }
}