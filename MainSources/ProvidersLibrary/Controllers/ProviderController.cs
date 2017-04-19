using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProvidersLibrary
{
    //Базовый класс для SourceController и ReceiverController
    public abstract class ProviderController
    {
        protected ProviderController(ProviderConnect connect)
        {
            Connect = connect;
        }

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
}