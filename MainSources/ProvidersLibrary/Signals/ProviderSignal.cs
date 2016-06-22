using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для всех сигналов источников и приемников
    public class ProviderSignal : IContextable
    {
        public ProviderSignal(string code, DataType dataType)
        {
            Code = code;
            DataType = dataType;
        }
        public ProviderSignal(string code, DataType dataType, string signalInf) : this(code, dataType)
        {
            Inf = signalInf.ToPropertyDicS();
        }

        //Полный код сигнала
        public string Code { get; private set; }
        //Адрес для формирования ошибок мгновенных значений
        public string Context { get { return "{" + Code + "}" ; } }
        //Тип данных 
        public DataType DataType { get; private set; }

        //Словарь свойств
        public DicS<string> Inf { get; private set; }
    }
}