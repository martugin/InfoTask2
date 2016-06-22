using System;
using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    //Соединение с источником Wonderware
    public class WonderwareSourceConnect : SqlSourceConnect
    {
        //Комплект
        public override string Complect { get { return "Wonderware"; } }
        //Код провайдера
        public override string Code { get { return "WonderwareSource"; } }

        //Получение диапазона архива по блокам истории
        public override TimeInterval GetTime()
        {
            DateTime mind = Different.MaxDate, maxd = Different.MinDate;
            try
            {
                using (var rec = new ReaderAdo(SqlProps, "SELECT FromDate, ToDate FROM v_HistoryBlock ORDER BY FromDate, ToDate DESC"))
                    while (rec.Read())
                    {
                        var fromd = rec.GetTime("FromDate");
                        var tod = rec.GetTime("ToDate");
                        if (fromd < mind) mind = fromd;
                        if (maxd < tod) maxd = tod;
                    }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при получении диапазона источника", ex);
            }
            if (mind == Different.MaxDate && maxd == Different.MinDate)
                return new TimeInterval(Different.MinDate, Different.MaxDate);
            AddEvent("Диапазон источника определен", mind + " - " + maxd);
            return new TimeInterval(mind, maxd);
        }
    }
}