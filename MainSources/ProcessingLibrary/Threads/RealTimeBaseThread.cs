using BaseLibrary;

namespace ProcessingLibrary
{
    //Базовый класс для RealTimeThread и ProxyThread
    public abstract class RealTimeBaseThread : BaseThread
    {
        protected RealTimeBaseThread(ProcessProject project, int id, string name, IIndicator indicator)
            : base(project, id, name, indicator, LoggerStability.RealTimeFast) { }

        //Словарь всех соединений прокси
        private readonly DicS<ProxyConnect> _proxies = new DicS<ProxyConnect>();
        public DicS<ProxyConnect> Proxies { get { return _proxies; } }

        //Подготовка потока
        protected override void Prepare()
        {
            using (StartProgress("Подготовка потока"))
            {
                Start(0, 60).Run(LoadModules);
            }
        }

        //Запуск операций цикла обработки
        protected void RunCycle()
        {
            Start(0, 50).Run(ReadSources);
            Start(50, 60).Run(ClaculateModules);
            Start(60, 80).Run(WriteReceivers);
            Start(80, 100).Run(WriteProxies);
        }

        //Запись в прокси
        protected virtual void WriteProxies()
        {
            foreach (var proxy in Proxies.Values)
                using (StartLog(Procent, Procent + 100.0 / Proxies.Count, "Запись в прокси", "", proxy.Code))
                    proxy.WriteValues();
        }
    }
}