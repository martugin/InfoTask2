using System;

namespace BaseLibrary
{
    //Начало и конец временного интервала
    public class TimeInterval
    {
        public TimeInterval()
        { }

        public TimeInterval(DateTime begin, DateTime end)
        {
            Begin = begin;
            End = end;
        }

        public TimeInterval(IRecordRead rec)
        {
            try { Begin = rec.GetTime("TimeBegin"); }
            catch { Begin = rec.GetTime("Begin"); }
            try { End = rec.GetTime("TimeEnd"); }
            catch { End = rec.GetTime("End"); }
        }

        public DateTime Begin { get; set; }
        public DateTime End { get; set; }

        //Длина интервала в секундах
        public double Length()
        {
            return End.Subtract(Begin).TotalSeconds;
        }

        //Является интервалом по умолчанию
        public bool IsDefault
        {
            get { return Begin == Static.MinDate && End == Static.MaxDate; }
        }

        //Возвращает временной интервал по умолчанию
        public static TimeInterval CreateDefault()
        {
            return new TimeInterval(Static.MinDate, Static.MaxDate);
        }
    }
}
