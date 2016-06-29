using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    public abstract class KosmotronikaBaseConnect : OleDbSourceConnect
    {
        //Проверка соединения
        public override bool CheckConnection()
        {
            if (Check() && GetTime() != null)
            {
                var ti = GetTime();
                if (ti != null)
                {
                    CheckConnectionMessage = "Успешное соединение. Диапазон источника: " + ti.Begin + " - " + ti.End;
                    return true;
                }
            }
            AddError(CheckConnectionMessage = "Ошибка соединения с Ретро-сервером");
            return false;
        }

        //Получение времени архива ПТК
        protected override TimeInterval GetSourceTime()
        {
            using (var rec = new ReaderAdo(Connection, "Exec RT_ARCHDATE"))
            {
                var beg = rec.GetTime(0); 
                var en = rec.GetTime(1);
                if (beg.ToString() != "0:00:00")
                    return new TimeInterval(beg, en);
                return TimeInterval.CreateDefault();
            }
        }
    }
}