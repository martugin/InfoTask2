using System;
using BaseLibrary;

namespace CommonTypes
{
    //Один интервал события
    public class Segment : TimeInterval
    {
        public Segment(DateTime begin, DateTime end, double pos, ErrMom err = null)
            : base(begin, end)
        {
            Pos = pos;
            if (pos == -1)
            {
                IsResultTime = false;
                ResultTime = Begin;
            }
            else
            {
                IsResultTime = true;
                ResultTime = Begin.AddSeconds(end.Subtract(Begin).TotalSeconds*pos);
            }
            Error = err;
        }

        //Положение значения
        public double Pos { get; private set; }
        //Время значения
        public DateTime ResultTime { get; set; }
        //True, если время значения указано
        public bool IsResultTime { get; set; }
        //Ошибка
        public ErrMom Error { get; set; }

        //Сравнение сегментов
        public bool EqualsTo(Segment s)
        {
            return Begin == s.Begin && End == s.End && ResultTime == s.ResultTime;
        }

        //Сравнение времен сегментов для сортировки
        public bool Less(Segment s)
        {
            if (!Begin.EqulasToMilliSeconds(s.Begin))
                return Begin.Minus(s.Begin) < 0;
            if (!End.EqulasToMilliSeconds(s.End))
                return End.Minus(s.End) < 0;
            if (IsResultTime && s.IsResultTime)
                return ResultTime.Minus(s.ResultTime) < 0;
            return false;
        }
        
        public Segment Clone()
        {
            return new Segment(Begin, End, Pos, Error);
        }
    }
}