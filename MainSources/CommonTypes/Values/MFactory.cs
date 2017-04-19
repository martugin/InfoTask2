using System;
using BaseLibrary;

namespace CommonTypes
{
    //Фабрика мгновенных значений: Mean, Mom, MomList
    public static class MFactory
    {
        #region Mean
        public static Mean NewMean(DataType dtype)
        {
            return CreateMean(dtype);
        }

        public static Mean NewMean(bool b)
        {
            return new BoolMean(b);
        }
        public static Mean NewMean(DataType dtype, bool b)
        {
            var mean = CreateMean(dtype);
            mean.Boolean = b;
            return mean;
        }

        public static Mean NewMean(int i)
        {
            return new IntMean(i);
        }
        public static Mean NewMean(DataType dtype, int i)
        {
            var mean = CreateMean(dtype);
            mean.Integer = i;
            return mean;
        }

        public static Mean NewMean(double d)
        {
            return new RealMean(d);
        }
        public static Mean NewMean(DataType dtype, double r)
        {
            var mean = CreateMean(dtype);
            mean.Real = r;
            return mean;
        }

        public static Mean NewMean(string s)
        {
            return new StringMean(s);
        }
        public static Mean NewMean(DataType dtype, string s)
        {
            var mean = CreateMean(dtype);
            mean.String = s;
            return mean;
        }

        public static Mean NewMean(DateTime d)
        {
            return new TimeMean(d);
        }
        public static Mean NewMean(DataType dtype, DateTime d)
        {
            var mean = CreateMean(dtype);
            mean.Date = d;
            return mean;
        }
        
        private static Mean CreateMean(DataType dtype)
        {
            switch (dtype)
            {
                case DataType.Real:
                    return new RealMean();
                case DataType.String:
                    return new StringMean();
                case DataType.Integer:
                    return new IntMean();
                case DataType.Boolean:
                    return new BoolMean();
                case DataType.Time:
                    return new TimeMean();
            }
            return null;
        }

        //Записать значение в рекордсет
        public static void PutMean(this IRecordAdd rec, string field, IReadMean mean)
        {
            mean.ValueToRec(rec, field);
        }

        //Получение значения из рекордсета
        public static Mean GetMean(this IRecordRead rec, DataType dtype, string field)
        {
            switch (dtype)
            {
                case DataType.Real:
                    return new RealMean(rec.GetDouble(field));
                case DataType.String:
                    return new StringMean(rec.GetString(field));
                case DataType.Integer:
                    return new IntMean(rec.GetInt(field));
                case DataType.Boolean:
                    return new BoolMean(rec.GetBool(field));
                case DataType.Time:
                    return new TimeMean(rec.GetTime(field));
            }
            return null;
        }
        #endregion

        #region Mom

        public static IMean NewMom(DataType dtype)
        {
            return CreateMom(dtype, DateTime.MinValue, null);
        }
        public static IMean NewMom(DateTime time, bool b, MomErr err = null)
        {
            return new BoolMom(time, b, err);
        }
        public static IMean NewMom(DataType dtype, DateTime time, bool b, MomErr err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.Boolean = b;
            return mom;
        }

        public static IMean NewMom(DateTime time, int i, MomErr err = null)
        {
            return new IntMom(time, i, err);
        }
        public static IMean NewMom(DataType dtype, DateTime time, int i, MomErr err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.Integer = i;
            return mom;
        }

        public static IMean NewMom(DateTime time, double r, MomErr err = null)
        {
            return new RealMom(time, r, err);
        }
        public static IMean NewMom(DataType dtype, DateTime time, double r, MomErr err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.Real = r;
            return mom;
        }

        public static IMean NewMom(DateTime time, string s, MomErr err = null)
        {
            return new StringMom(time, s, err);
        }
        public static IMean NewMom(DataType dtype, DateTime time, string s, MomErr err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.String = s;
            return mom;
        }

        public static IMean NewMom(DateTime time, DateTime d, MomErr err = null)
        {
            return new TimeMom(time, d, err);
        }
        public static IMean NewMom(DataType dtype, DateTime time, DateTime d, MomErr err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.Date = d;
            return mom;
        }

        public static IMean NewMom(DataType dtype, DateTime time, object ob, MomErr err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.Object = ob;
            return mom;
        }

        internal static IMean NewMom(DataType dtype, DateTime time, MomErr err = null)
        {
            return CreateMom(dtype, time, err);
        }

        private static IMean CreateMom(DataType dtype, DateTime time, MomErr err)
        {
            switch (dtype)
            {
                case DataType.Real:
                    return new RealMom { Time = time, Error = err};
                case DataType.String:
                    return new StringMom { Time = time, Error = err };
                case DataType.Integer:
                    return new IntMom { Time = time, Error = err };
                case DataType.Boolean:
                    return new BoolMom { Time = time, Error = err };
                case DataType.Time:
                    return new TimeMom { Time = time, Error = err };
                case DataType.Weighted:
                    return new WeightedMom { Time = time, Error = err};
                case DataType.Value:
                    return new ValueMom { Time = time, Error = err};
            }
            return null;
        }
        #endregion

        #region MomList
        public static MomList NewList(DataType dtype)
        {
            switch (dtype)
            {
                case DataType.Boolean:
                    return new BoolMomList();
                case DataType.Integer:
                    return new IntMomList();
                case DataType.Real:
                    return new RealMomList();
                case DataType.String:
                    return new StringMomList();
                case DataType.Time:
                    return new TimeMomList();
                case DataType.Weighted:
                    return new WeightedMomList();
            }
            return null;
        }
        #endregion
    }
}