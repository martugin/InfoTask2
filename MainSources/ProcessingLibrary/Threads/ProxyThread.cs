using System;
using System.Threading;
using BaseLibrary;

namespace ProcessingLibrary
{
    //Поток, получающий значения из QueuedProxy
    public class ProxyThread : RealTimeBaseThread
    {
        public ProxyThread(ProcessProject project, int id, string name, IIndicator indicator, QueuedProxyConnect proxy)
            : base(project, id, name, indicator)
        {
            _proxy = proxy;
        }

        //Прокси с очередью
        private readonly QueuedProxyConnect _proxy;

        //Ожидание, между проверками очереди
        protected override void Waiting()
        {
            Thread.Sleep(100);
        }

        //Цикл
        protected override void Cycle()
        {
            while (true)
            {
                if (_proxy.ValuesCount == 0) return;
                _proxy.ReadValues();
                RunCycle();
            }
        }
    }
}