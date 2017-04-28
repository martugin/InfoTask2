using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace ProcessingLibrary
{
    //Постоянное получение данных с источника в реальном времени
    public class SourceController : ProviderController
    {
        public SourceController(SourceConnect connect) 
            :base(connect) { }

        //Соединение 
        public SourceConnect SourceConnect { get { return (SourceConnect) Connect; } }

        //Список сигналов, содержащих возвращаемые значения
        protected readonly DicS<ControllerSourceSignal> ControllerSignals = new DicS<ControllerSourceSignal>();
        public IDicSForRead<ControllerSourceSignal> Signals { get { return ControllerSignals; } }

        //Получение данных из источника
        protected override void RunCycle()
        {
            SourceConnect.GetValues();
            lock (BufferLooker)
                foreach (var csig in ControllerSignals.Values)
                    csig.BufferValue = (IReadMean)csig.SourceSignal.OutValue.Clone();
        }

        //Получение значений из буферных
        public void GetValues()
        {
            lock (BufferLooker)
                foreach (var csig in ControllerSignals.Values)
                    csig.Value = (IReadMean)csig.BufferValue.Clone();
        }
    }
}