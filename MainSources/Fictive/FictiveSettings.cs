using System;
using BaseLibrary;
using ProvidersLibrary;

namespace Fictive
{
    //Подключение к тестовому источнику
    public class FictiveSettings : SourceSettings
    {
        public override string Hash { get { return "Fictive"; } }

        protected override void ReadInf(DicS<string> dic) { }
        
        //Диапазон источника
        protected override TimeInterval GetSourceTime()
        {
            return new TimeInterval(new DateTime(2000, 1, 1), new DateTime(2100, 1, 1));
        }
    }
}