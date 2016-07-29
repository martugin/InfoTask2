using BaseLibrary;
using ProvidersLibrary;

namespace Fictive
{
    //Подключение в фиктивному источнику
    public class FictiveTableSettings : AccessSourceSettings 
    {
        //Каждый второй раз соедиение не проходит
        private int _numConnect;
        public override bool Connect()
        {
            return _numConnect++ % 2 == 1;
        }

        //Диапазон источника
        protected override TimeInterval GetSourceTime()
        {
            if (!Connect()) return TimeInterval.CreateDefault();
            using (var sys = new SysTabl(DbFile))
                return new TimeInterval(sys.Value("BeginInterval").ToDateTime(), sys.Value("EndInterval").ToDateTime());
        }
    }
}