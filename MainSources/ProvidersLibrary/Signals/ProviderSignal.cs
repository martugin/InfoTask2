using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для всех сигналов
    public abstract class ProviderSignal
    {
        protected internal ProviderSignal(ProviderConnect connect, string code, DataType dataType, string contextOut, DicS<string> inf)
        {
            Connect = connect;
            Code = code;
            DataType = dataType;
            ContextOut = contextOut;
            Inf = inf;
            Inf.DefVal = "";
        }

        //Конструктор для расчетных сигналов
        protected internal ProviderSignal(ProviderConnect connect, string code)
        {
            Connect = connect;
            Code = code;
        }

        //Соединение
        public ProviderConnect Connect { get; private set; }

        //Полный код сигнала
        public string Code { get; private set; }
        //Тип данных 
        public DataType DataType { get; protected set; }
        //InfObject + InfOut
        public string ContextOut { get; private set; }
        //Словарь свойств
        public DicS<string> Inf { get; private set; }

        //Тип сигнала
        public abstract SignalType Type { get; }
    }
}