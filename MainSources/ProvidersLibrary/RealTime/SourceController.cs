using System;
using System.Threading;
using System.Threading.Tasks;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для SourceController и ReceiverController
    public abstract class ProviderController
    {
        protected ProviderController(ProviderConnect connect)
        {
            Connect = connect;
        }

        //Список сигналов, содержащих возвращаемые значения
        protected readonly DicS<ControllerSignal> ControllerSignals = new DicS<ControllerSignal>();
        
        //Соединение 
        public ProviderConnect Connect { get; private set; }

        //Интервал времени между запуском циклов в мс
        public int IntervalLength { get; set; }

        //Остановить процесс
        private readonly object _finishLocker = new object();
        private bool _isFinishing;

        //Запуск процесса
        public void Start()
        {
            new Task(Run).Start();
        }

        //Завершение процесса 
        public void Finish()
        {
            lock (_finishLocker)
                _isFinishing = true;
        }

        //Команда, выполняемая в потоке
        public void Run()
        {
            while (true)
            {
                var lastTime = DateTime.Now;
                lock (_finishLocker)
                    if (_isFinishing)
                    {
                        _isFinishing = false;
                        break;
                    }
                RunCycle();
                int t = Convert.ToInt32(lastTime.AddMilliseconds(IntervalLength).Subtract(DateTime.Now).TotalMilliseconds);
                if (t > 0) Thread.Sleep(t);
            }
        }

        //Выполение одного цикла 
        protected abstract void RunCycle();

        //Блокировка буферных значений
        protected object BufferLooker = new object();
    }

    //----------------------------------------------------------------------------------------------

    //Постоянное получение данных с источника в реальном времени
    public class SourceController : ProviderController
    {
        public SourceController(SourceConnect connect) 
            :base(connect) { }

        //Соединение 
        public SourceConnect SourceConnect { get { return (SourceConnect) Connect; } }

        //Список сигналов, содержащих возвращаемые значения
        public IDicSForRead<ISourceSignal> Signals { get { return ControllerSignals; } }

        //Получение данных из источника
        protected override void RunCycle()
        {
            SourceConnect.GetValues();
            lock (BufferLooker)
                foreach (var csig in ControllerSignals.Values)
                    csig.BufferValue = (IMean)csig.Signal.Value.Clone();
        }

        //Получение значений из буферных
        public void GetValues()
        {
            lock (BufferLooker)
                foreach (var csig in ControllerSignals.Values)
                    csig.Value = (IMean)csig.BufferValue.Clone();
        }
    }

    //----------------------------------------------------------------------------------------------

    //Постоянная передача данных в приемник в реальном времени
    public class ReceiverController : ProviderController
    {
        public ReceiverController(ReceiverConnect connect)
            : base(connect) { }

        //Соединение 
        public ReceiverConnect ReceiverConnect { get { return (ReceiverConnect)Connect; } }

        //Список сигналов, содержащих возвращаемые значения
        public IDicSForRead<IReceiverSignal> Signals { get { return ControllerSignals; } }

        //Запись данных в приемник
        protected override void RunCycle()
        {
            lock (BufferLooker)
                foreach (var csig in ControllerSignals.Values)
                    csig.Signal.Value = (IMean)csig.BufferValue.Clone();
            ReceiverConnect.WriteValues();
        }

        //Получение значений из буферных
        public void SetValues()
        {
            lock (BufferLooker)
                foreach (var csig in ControllerSignals.Values)
                    csig.BufferValue = (IMean)csig.Value.Clone();
        }
    }
}