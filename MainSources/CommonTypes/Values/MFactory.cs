using System;
using BaseLibrary;

namespace CommonTypes
{
    //Фабрика мгновенных значений: Meam, EMean, Mom
    public static class MFactory
    {
        //NewMean
        #region
        public static IMean NewMean(DataType dtype)
        {
            return CreateMean(dtype, null);
        }

        public static IMean NewMean(DataType dtype, MomErr err)
        {
            return CreateMean(dtype, err);
        }

        public static IMean NewMean(bool b, MomErr err = null)
        {
            return err == null ? new BoolMean(b) : new MeanErrBool(b, err);
        }
        public static IMean NewMean(DataType dtype, bool b, MomErr err = null)
        {
            var mean = CreateMean(dtype, err);
            mean.Boolean = b;
            return mean;
        }

        public static IMean NewMean(int i, MomErr err = null)
        {
            return err == null ? new IntMean(i) : new MeanErrInt(i, err);
        }
        public static Mean NewMean(DataType dtype, int i, MomErr err = null)
        {
            var mean = CreateMean(dtype, err);
            mean.Integer = i;
            return mean;
        }

        public static IMean NewMean(double d, MomErr err = null)
        {
            return err == null ? new RealMean(d) : new MeanErrReal(d, err);
        }
        public static Mean NewMean(DataType dtype, double r, MomErr err = null)
        {
            var mean = CreateMean(dtype, err);
            mean.Real = r;
            return mean;
        }

        public static IMean NewMean(string s, MomErr err = null)
        {
            return err == null ? new StringMean(s) : new MeanErrString(s, err);
        }
        public static Mean NewMean(DataType dtype, string s, MomErr err = null)
        {
            var mean = CreateMean(dtype, err);
            mean.String = s;
            return mean;
        }

        public static IMean NewMean(DateTime d, MomErr err = null)
        {
            return err == null ? new TimeMean(d) : new MeanErrTime(d, err);
        }
        public static Mean NewMean(DataType dtype, DateTime d, MomErr err = null)
        {
            var mean = CreateMean(dtype, err);
            mean.Date = d;
            return mean;
        }
        
        private static Mean CreateMean(DataType dtype, MomErr err)
        {
            if (err == null)
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
            else
            {
                switch (dtype)
                {
                    case DataType.Real:
                        return new MeanErrReal { Error = err };
                    case DataType.String:
                        return new MeanErrString { Error = err };
                    case DataType.Integer:
                        return new MeanErrInt { Error = err };
                    case DataType.Boolean:
                        return new MeanErrBool { Error = err };
                    case DataType.Time:
                        return new MeanErrTime { Error = err };
                }
            }
            return null;
        }

        //Записать значение в рекордсет
        public static void PutMean(this IRecordAdd rec, string field, IMean mean)
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

        //NewMom
        #region
        public static IMean NewMom(DateTime time, bool b, MomErr err = null)
        {
            return err == null ? new BoolMom(time, b) : new BoolErrMom(time, b, err);
        }
        public static IMean NewMom(DataType dtype, DateTime time, bool b, MomErr err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.Boolean = b;
            return mom;
        }

        public static IMean NewMom(DateTime time, int i, MomErr err = null)
        {
            return err == null ? new IntMom(time, i) : new IntErrMom(time, i, err);
        }
        public static IMean NewMom(DataType dtype, DateTime time, int i, MomErr err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.Integer = i;
            return mom;
        }

        public static IMean NewMom(DateTime time, double r, MomErr err = null)
        {
            return err == null ? new RealMom(time, r) : new RealErrMom(time, r, err);
        }
        public static IMean NewMom(DataType dtype, DateTime time, double r, MomErr err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.Real = r;
            return mom;
        }

        public static IMean NewMom(DateTime time, string s, MomErr err = null)
        {
            return err == null ? new StringMom(time, s) : new StringErrMom(time, s, err);
        }
        public static IMean NewMom(DataType dtype, DateTime time, string s, MomErr err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.String = s;
            return mom;
        }

        public static IMean NewMom(DateTime time, DateTime d, MomErr err = null)
        {
            return err == null ? new TimeMom(time, d) : new TimeErrMom(time, d, err);
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

        private static Mean CreateMom(DataType dtype, DateTime time, MomErr err)
        {
            if (err == null)
            {
                switch (dtype)
                {
                    case DataType.Real:
                        return new RealMom {Time = time};
                    case DataType.String:
                        return new StringMom {Time = time};
                    case DataType.Integer:
                        return new IntMom {Time = time};
                    case DataType.Boolean:
                        return new BoolMom {Time = time};
                    case DataType.Time:
                        return new TimeMom {Time = time};
                    case DataType.Weighted:
                        return new WeightedMom {Time = time};
                    case DataType.Value:
                        return new ValueMom {Time = time};
                }
            }
            switch (dtype)
            {
                case DataType.Real:
                    return new RealErrMom { Time = time, Error = err};
                case DataType.String:
                    return new StringErrMom { Time = time, Error = err };
                case DataType.Integer:
                    return new IntErrMom { Time = time, Error = err };
                case DataType.Boolean:
                    return new BoolErrMom { Time = time, Error = err };
                case DataType.Time:
                    return new TimeErrMom { Time = time, Error = err };
                case DataType.Weighted:
                    return new WeightedMom { Time = time, Error = err};
                case DataType.Value:
                    return new ValueMom { Time = time, Error = err};
            }
            return null;
        }
        #endregion

        //NewList
        #region
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