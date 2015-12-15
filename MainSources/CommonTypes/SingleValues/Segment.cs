using System;
using BaseLibrary;

namespace CommonTypes
{
    //Один интервал события
    public class Segment : TimeInterval
    {
        public Segment(DateTime begin, DateTime end, double pos, int nd = 0, ErrMom err = null)
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
            Nd = nd;
            Error = err;
        }

        //Положение значения
        public double Pos { get; private set; }
        //Время значения
        public DateTime ResultTime { get; set; }
        //True, если время значения указано
        public bool IsResultTime { get; set; }
        //Ошибка
        public ErrMom ErrMom { get; private set; }

        //Todo убрать
        //Недостоверность
        public int Nd { get; set; }
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
        
        //Todo Убрать, страя версия сортировки
        //Сравнение сегментов для сортировки
        public static int Compare(Segment s1, Segment s2)
        {
            if (s1.Begin < s2.Begin) return -1;
            if (s1.Begin > s2.Begin) return 1;
            if (s1.End < s2.End) return -1;
            if (s1.End > s2.End) return 1;
            if (!s1.IsResultTime && s2.IsResultTime) return -1;
            if (s1.IsResultTime && !s2.IsResultTime) return 1;
            if (!s1.IsResultTime && !s2.IsResultTime) return 0;
            if (s1.ResultTime < s2.ResultTime) return -1;
            if (s1.ResultTime > s2.ResultTime) return 1;
            return 0;
        }
        
        //Глубокое копирование, если erradr != null, то ошибке приписывается в цепочку новый расчетный параметр
        public Segment Clone(string erradr = null)
        {
            var seg = new Segment(Begin, End, Pos, Nd, Error);
            //if (seg.Error != null && erradr != null) 
            //    seg.Error = new ErrorCalc(erradr, seg.Error);
            return seg;
        }
    }
}