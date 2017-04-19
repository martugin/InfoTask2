using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Постоянная передача данных в приемник в реальном времени
    public class ReceiverController : ProviderController
    {
        public ReceiverController(ReceiverConnect connect)
            : base(connect) { }

        //Соединение 
        public ReceiverConnect ReceiverConnect { get { return (ReceiverConnect)Connect; } }

        //Список сигналов, содержащих возвращаемые значения
        protected readonly DicS<ControllerReceiverSignal> ControllerSignals = new DicS<ControllerReceiverSignal>();
        public IDicSForRead<ControllerReceiverSignal> Signals { get { return ControllerSignals; } }

        //Запись данных в приемник
        protected override void RunCycle()
        {
            lock (BufferLooker)
                foreach (var csig in ControllerSignals.Values)
                    csig.ReceiverSignal.Value = (IReadMean)csig.BufferValue.Clone();
            ReceiverConnect.WriteValues();
        }

        //Получение значений из буферных
        public void SetValues()
        {
            lock (BufferLooker)
                foreach (var csig in ControllerSignals.Values)
                    csig.BufferValue = (IReadMean)csig.Value.Clone();
        }
    }
}