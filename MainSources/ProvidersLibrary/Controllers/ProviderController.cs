using System;
using System.Threading;
using System.Threading.Tasks;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для SourceController и ReceiverController
    public abstract class ProviderController : ExternalLogger
    {
        protected ProviderController(Project project, ProviderConnect connect)
        {
            Connect = connect;
            Logger = new Logger(project.CreateHistory(@"Threads\" + connect.Name + "History.accdb"), null);
        }

        //Соединение 
        public ProviderConnect Connect { get; private set; }

        //Интервал времени между запуском циклов в мс
        public int IntervalLength { get; set; }

        //Остановить процесс
        private readonly object _finishLocker = new object();
        private bool _isFinishing;

        //Запуск процесса
        public void StartProcess()
        {
            new Task(Run).Start();
        }

        //Завершение процесса 
        public void FinishProcess()
        {
            lock (_finishLocker)
                _isFinishing = true;
        }

        //Команда, выполняемая в потоке
        private void Run()
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
}