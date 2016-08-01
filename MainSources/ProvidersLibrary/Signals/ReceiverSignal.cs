﻿using CommonTypes;

namespace ProvidersLibrary
{
    //Сигнал приемника
    public class ReceiverSignal : ProviderSignal
    {
        public ReceiverSignal(Receiver receiver, string code, string codeObject, DataType dataType, string signalInf)
            : base(code, codeObject, dataType, signalInf)
        {
            Receiver = receiver;
        }

        //Передаваемое значение
        public IMean Value { get; set; }
        //Соединение - приемник
        protected Receiver Receiver { get; private set; }
    }
}