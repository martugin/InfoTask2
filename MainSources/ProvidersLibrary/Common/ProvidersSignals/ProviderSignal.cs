using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для всех сигналов
    public class ProviderSignal : ISourceSignal, IReceiverSignal
    {
        protected internal ProviderSignal(ProviderConnect connect, string code)
        {
            IsInitial = true;
            Connect = connect;
            Code = code;
        }

        protected internal ProviderSignal(ProviderConnect connect, string code, DataType dataType, string contextOut, DicS<string> inf)
        {
            IsInitial = true;
            Connect = connect;
            Code = code;
            DataType = dataType;
            ContextOut = contextOut;
            Inf = inf;
            Inf.DefVal = "";
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

        //Является основным сигналом (не расчетным и т.п.)
        public bool IsInitial { get; protected set; }

        //Значение или список значений
        public virtual IMean Value { get; set; }
    }
}