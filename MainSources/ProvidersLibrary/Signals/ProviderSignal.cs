using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для всех сигналов источников и приемников
    public abstract class ProviderSignal 
    {
        protected ProviderSignal(string code, DataType dataType)
        {
            Code = code;
            DataType = dataType;
            IsReal = dataType.LessOrEquals(DataType.Real);
        }
        protected ProviderSignal(string code, DataType dataType, string signalInf) 
            : this(code, dataType)
        {
            Inf = signalInf.ToPropertyDicS();
        }

        //Полный код сигнала
        public string Code { get; private set; }
        //Тип данных 
        public DataType DataType { get; private set; }
        //Сводится к действительному числу
        public bool IsReal { get; private set; }

        //Словарь свойств
        public DicS<string> Inf { get; private set; }
    }
}