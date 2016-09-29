namespace BaseLibrary
{
    //Временные интервалы
    public enum TimeUnit
    {
        Year = 0,
        Month = 1,
        Day = 2,
        Hour = 3,
        Minute = 4,
        Second = 5,
        MSec = 6,
        Error = 7
    }

    //---------------------------------------------------------------------
    public static class TimeUnitConv
    {
        public static TimeUnit ToTimeUnit(this string s)
        {
            if (s.IsEmpty()) return TimeUnit.Error;
            switch (s.ToLower())
            {
                case "секунда":
                case "сек":
                case "second":
                    return TimeUnit.Second;
                case "мс":
                case "ms":
                    return TimeUnit.MSec;
                case "минута":
                case "мин":
                case "minute":
                    return TimeUnit.Minute;
                case "час":
                case "hour":
                    return TimeUnit.Hour;
                case "сут":
                case "сутки":
                case "day":
                    return TimeUnit.Day;
                case "месяц":
                case "мес":
                case "month":
                    return TimeUnit.Month;
                case "год":
                case "year":
                    return TimeUnit.Year;
            }
            return TimeUnit.Error;
        }

        public static string ToEnglish(this TimeUnit u)
        {
            switch (u)
            {
                case TimeUnit.Second:
                    return "second";
                case TimeUnit.MSec:
                    return "msec";
                case TimeUnit.Minute:
                    return "minute";
                case TimeUnit.Hour:
                    return "hour";
                case TimeUnit.Day:
                    return "day";
                case TimeUnit.Month:
                    return "month";
                case TimeUnit.Year:
                    return "year";
            }
            return "";
        }
    }
}