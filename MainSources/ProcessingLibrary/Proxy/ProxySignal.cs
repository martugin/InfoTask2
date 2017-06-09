using CommonTypes;

namespace ProcessingLibrary
{
    //Сигнал для прокси
    public class ProxySignal : IWriteSignal, IReadSignal
    {
        public ProxySignal(string connectCode, IReadSignal signal)
        {
            InSignal = signal;
            ConnectCode = connectCode;
            OutValue = MFactory.NewList(DataType);
        }

        //Код и тип данных
        public string Code { get { return InSignal.Code; } }
        public DataType DataType { get { return InSignal.DataType; } }
        //Код соединения
        public string ConnectCode { get; private set; }

        //Cигнал с исходным значением
        public IReadSignal InSignal { get; private set; }
        
        //Возвращаемое значение
        public IReadMean OutValue { get; protected set; }

        //Взять значение из исходного сигнала в прокси
        public virtual void WriteValue()
        {
            OutValue = (IReadMean)InSignal.OutValue.Clone();
        }

        //Прочитать значение из прокси в OutValue
        public virtual IReadMean ReadValue()
        {
            return OutValue;
        }
    }
}