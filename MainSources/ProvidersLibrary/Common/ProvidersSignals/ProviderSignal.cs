using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для всех сигналов источников и приемников
    public class ProviderSignal : ISourceSignal, IReceiverSignal
    {
        protected internal ProviderSignal(ProviderConnect connect, string code)
        {
            Connect = connect;
            Code = code;
        }

        protected internal ProviderSignal(ProviderConnect connect, string code, DataType dataType, string infObject, string infOut, string infProp)
        {
            Connect = connect;
            Code = code;
            DataType = dataType;
            CodeOut = infObject + (infOut.IsEmpty() ? "" : ";" + infOut);
            Inf = infObject.ToPropertyDicS().AddDic(infOut.ToPropertyDicS()).AddDic(infProp.ToPropertyDicS());
            Inf.DefVal = "";
        }

        //Соединение
        protected internal ProviderConnect Connect { get; private set; }

        //Полный код сигнала
        public string Code { get; private set; }
        //Тип данных 
        public DataType DataType { get; protected set; }
        //InfObject + InfOut
        public string CodeOut { get; private set; }
        //Словарь свойств
        public DicS<string> Inf { get; private set; }

        //Значение или список значений
        public virtual IMean Value { get; set; }
    }
}