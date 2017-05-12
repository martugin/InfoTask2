using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Сигнал приемника
    public class ReceiverSignal : ProviderSignal, IWriteSignal
    {
        public ReceiverSignal(ProviderConnect connect, string code, DataType dataType, string contextOut, DicS<string> inf) 
            : base(connect, code, dataType, contextOut, inf) { }

        //Тип сигнала
        public override SignalType Type
        {
            get { return SignalType.Receiver;}
        }

        //Соединение с приемником
        public ReceiverConnect ReceiverConnect
        {
            get { return (ReceiverConnect)Connect; }
        }

        //Значение
        public IReadMean InValue { get; set; }
        //Записать значение
        public void PutValue(IReadMean value)
        {
            throw new NotImplementedException();
        }
    }
}