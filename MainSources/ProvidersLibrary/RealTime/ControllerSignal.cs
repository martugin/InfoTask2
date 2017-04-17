using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Один сигнал для чтения или записи в провайдер в режиме реального времени
    public class ControllerSignal : IReadSignal, IWriteSignal
    {
        public ControllerSignal(ProviderSignal signal)
        {
            Signal = signal;
        }
        
        //Ссылка на сигнал провайдера
        internal ProviderSignal Signal { get; private set; }

        //Буферное значение для взаимодействия с провайдером
        internal IReadMean BufferValue { get; set; }
        //Значение для взаимодействия с потоком
        public IReadMean Value { get; set; }

        //Переопределение свойств из ProviderSignal
        public string Code { get { return Signal.Code; } }
        public DataType DataType { get { return Signal.DataType; } }
        public DicS<string> Inf { get { return Signal.Inf; } }
        public string ContextOut { get { return Signal.ContextOut; } }
        public ProviderConnect Connect { get { return Signal.Connect; } }
        public bool IsInitial { get { return Signal.IsInitial; } }
    }
}