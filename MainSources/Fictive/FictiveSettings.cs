using System;
using BaseLibrary;
using ProvidersLibrary;

namespace Fictive
{
    //Подключение к тестовому источнику
    public class FictiveSettings : SourceSettings
    {
        public override string Hash { get { return "Fictive"; } }

        protected override void ReadInf(DicS<string> dic)
        {
            Frequency = dic.GetInt("Frequency");
        }
        //С какой частотой в секундах добавлять значения в результат
        internal int Frequency { get; private set; }

        //Диапазон источника
        protected override TimeInterval GetSourceTime()
        {
            return new TimeInterval(new DateTime(2000, 1, 1), new DateTime(2100, 1, 1));
        }
    }
}