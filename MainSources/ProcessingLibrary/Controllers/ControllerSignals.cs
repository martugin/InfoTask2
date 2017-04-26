using CommonTypes;
using ProvidersLibrary;

namespace ProcessingLibrary
{
    //Один сигнал контроллера провайдера
    public class ControllerSignal
    {
        public ControllerSignal(ProviderSignal signal)
        {
            Signal = signal;
        }
        
        //Ссылка на сигнал провайдера
        internal ProviderSignal Signal { get; private set; }

        //Буферное значение для взаимодействия с провайдером
        internal IReadMean BufferValue { get; set; }

        //Переопределение свойств из ProviderSignal
        public string Code { get { return Signal.Code; } }
        public DataType DataType { get { return Signal.DataType; } }
    }

    //------------------------------------------------------------------------------------------------
    //Один сигнал контроллера источника
    public class ControllerSourceSignal : ControllerSignal
    {
        public ControllerSourceSignal(SourceSignal signal) 
            : base(signal) { }

        //Ссылка на сигнал провайдера
        internal SourceSignal SourceSignal
        {
            get { return (SourceSignal)Signal; }
        }

        //Значение для взаимодействия с потоком
        public IReadMean Value { get; internal set; }
    }

    //------------------------------------------------------------------------------------------------
    //Один сигнал контроллера приемника
    public class ControllerReceiverSignal : ControllerSignal
    {
        public ControllerReceiverSignal(ReceiverSignal signal) 
            : base(signal) { }

        //Ссылка на сигнал провайдера
        internal ReceiverSignal ReceiverSignal
        {
            get { return (ReceiverSignal)Signal; }
        }

        //Значение для взаимодействия с потоком
        public IReadMean Value { internal get; set; }
    }
}