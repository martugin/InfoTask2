using System.Threading;
using BaseLibrary;

namespace ProcessingLibrary
{
    //Поток, получающий значения из QueuedProxy
    public class ProxyThread : BaseThread
    {
        public ProxyThread(ProcessProject project, int id, string name, IIndicator indicator) 
            : base(project, id, name, indicator, LoggerStability.RealTimeFast) { }

        //Прокси с очередью
        private QueuedProxyConnect _proxy;

        //Подготовка потока
        protected override void Prepare()
        {
            using (StartProgress("Подготовка потока"))
            {
                Start(0, 60).Run(LoadModules);
            }
        }

        //Ожидание, между проверками очереди
        protected override void Waiting()
        {
            Thread.Sleep(100);
        }

        //Цикл
        protected override void Cycle()
        {
            throw new System.NotImplementedException();
        }
    }
}