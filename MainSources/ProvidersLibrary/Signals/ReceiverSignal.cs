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

        //Сигнла или параметр, содержащий исходное значение
        public IReadSignal InSignal { get; set; }

        //Значение
        public IReadMean InValue { get; set; }

        //Записать значение
        public void PutValue(IReadMean value)
        {
            InValue = InSignal.OutValue;
        }
    }
}