using System;
using BaseLibrary;

namespace CommonTypes
{
    public class MomWeighted : MomReal
    {
        public MomWeighted(DateTime time, double r, double w, ErrMom err = null) : base(err)
        {
            Time = time;
            Real = r;
            Weight = w;
        }

        //Тип данных
        public override DataType DataType
        {
            get { return DataType.Weighted; }
        }

        //Длина интервала
        public double Weight { get; private set; }

        //Копия значения
        public override Mom Clone(DateTime time)
        {
            return new MomWeighted(time, Real, Weight, Error);
        }
    }
}