using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для всех сигналов
    public abstract class ProviderSignal : ISignal
    {
        protected internal ProviderSignal(ProviderConnect connect, string code)
        {
            Connect = connect;
            Code = code;
        }

        protected internal ProviderSignal(ProviderConnect connect, string code, DataType dataType, string contextOut, DicS<string> inf)
        {
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

        //Тип значений сигнала
        public abstract SignalValueType ValueType { get; }

        //Значение или список значений
        public virtual IMean Value { get; set; }

        //Мгновенное значение сигнала или буферное значение
        internal abstract IMean BufMom { get; }

        //Добавка мгновенного значения (BufMom) в список или клон
        //Возвращает количество реально добавленных значений 
        internal abstract int AddMom(DateTime time, MomErr err);
    }
}