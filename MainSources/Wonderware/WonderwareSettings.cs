using System;
using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    //Поключение к Wonderware Historian
    public class WonderwareSettings : SqlSourceSettings
    {
        //Получение диапазона архива по блокам истории
        protected override TimeInterval GetSourceTime()
        {
            DateTime mind = Different.MaxDate, maxd = Different.MinDate;
            using (var rec = new ReaderAdo(SqlProps, "SELECT FromDate, ToDate FROM v_HistoryBlock ORDER BY FromDate, ToDate DESC"))
                while (rec.Read())
                {
                    var fromd = rec.GetTime("FromDate");
                    var tod = rec.GetTime("ToDate");
                    if (fromd < mind) mind = fromd;
                    if (maxd < tod) maxd = tod;
                }
            if (mind == Different.MaxDate && maxd == Different.MinDate)
                return TimeInterval.CreateDefault();
            return new TimeInterval(mind, maxd);
        }
    }
}