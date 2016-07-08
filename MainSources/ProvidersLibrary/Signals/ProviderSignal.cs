using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для всех сигналов источников и приемников
    public abstract class ProviderSignal 
    {
        protected ProviderSignal() { }

        protected ProviderSignal(DataType dataType, string signalInf) 
        {
            DataType = dataType;
            Inf = signalInf.ToPropertyDicS();
        }

        //Тип данных 
        public DataType DataType { get; protected set; }

        //Словарь свойств
        public DicS<string> Inf { get; private set; }
    }
}