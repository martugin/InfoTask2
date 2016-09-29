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

        public static IMean NewMean(DataType dtype, ErrMom err)
        {
            return CreateMean(dtype, err);
        }

        public static IMean NewMean(bool b, ErrMom err = null)
        {
            return err == null ? new MeanBool(b) : new MeanErrBool(b, err);
        }
        public static IMean NewMean(DataType dtype, bool b, ErrMom err = null)
        {
            var mean = CreateMean(dtype, err);
            mean.Boolean = b;
            return mean;
        }

        public static IMean NewMean(int i, ErrMom err = null)
        {
            return err == null ? new MeanInt(i) : new MeanErrInt(i, err);
        }
        public static Mean NewMean(DataType dtype, int i, ErrMom err = null)
        {
            var mean = CreateMean(dtype, err);
            mean.Integer = i;
            return mean;
        }

        public static IMean NewMean(double d, ErrMom err = null)
        {
            return err == null ? new MeanReal(d) : new MeanErrReal(d, err);
        }
        public static Mean NewMean(DataType dtype, double r, ErrMom err = null)
        {
            var mean = CreateMean(dtype, err);
            mean.Real = r;
            return mean;
        }

        public static IMean NewMean(string s, ErrMom err = null)
        {
            return err == null ? new MeanString(s) : new MeanErrString(s, err);
        }
        public static Mean NewMean(DataType dtype, string s, ErrMom err = null)
        {
            var mean = CreateMean(dtype, err);
            mean.String = s;
            return mean;
        }

        public static IMean NewMean(DateTime d, ErrMom err = null)
        {
            return err == null ? new MeanTime(d) : new MeanErrTime(d, err);
        }
        public static Mean NewMean(DataType dtype, DateTime d, ErrMom err = null)
        {
            var mean = CreateMean(dtype, err);
            mean.Date = d;
            return mean;
        }
        
        private static Mean CreateMean(DataType dtype, ErrMom err)
        {
            if (err == null)
                switch (dtype)
                {
                    case DataType.Real:
                        return new MeanReal();
                    case DataType.String:
                        return new MeanString();
                    case DataType.Integer:
                        return new MeanInt();
                    case DataType.Boolean:
                        return new MeanBool();
                    case DataType.Time:
                        return new MeanTime();
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
                    return new MeanReal(rec.GetDouble(field));
                case DataType.String:
                    return new MeanString(rec.GetString(field));
                case DataType.Integer:
                    return new MeanInt(rec.GetInt(field));
                case DataType.Boolean:
                    return new MeanBool(rec.GetBool(field));
                case DataType.Time:
                    return new MeanTime(rec.GetTime(field));
            }
            return null;
        }
        #endregion

        //NewMom
        #region
        public static IMean NewMom(DateTime time, bool b, ErrMom err = null)
        {
            return err == null ? new MomBool(time, b) : new MomErrBool(time, b, err);
        }
        public static IMean NewMom(DataType dtype, DateTime time, bool b, ErrMom err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.Boolean = b;
            return (IMean) mom;
        }

        public static IMean NewMom(DateTime time, int i, ErrMom err = null)
        {
            return err == null ? new MomInt(time, i) : new MomErrInt(time, i, err);
        }
        public static IMean NewMom(DataType dtype, DateTime time, int i, ErrMom err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.Integer = i;
            return (IMean)mom;
        }

        public static IMean NewMom(DateTime time, double r, ErrMom err = null)
        {
            return err == null ? new MomReal(time, r) : new MomErrReal(time, r, err);
        }
        public static IMean NewMom(DataType dtype, DateTime time, double r, ErrMom err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.Real = r;
            return (IMean)mom;
        }

        public static IMean NewMom(DateTime time, string s, ErrMom err = null)
        {
            return err == null ? new MomString(time, s) : new MomErrString(time, s, err);
        }
        public static IMean NewMom(DataType dtype, DateTime time, string s, ErrMom err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.String = s;
            return (IMean)mom;
        }

        public static IMean NewMom(DateTime time, DateTime d, ErrMom err = null)
        {
            return err == null ? new MomTime(time, d) : new MomErrTime(time, d, err);
        }
        public static IMean NewMom(DataType dtype, DateTime time, DateTime d, ErrMom err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.Date = d;
            return (IMean)mom;
        }

        public static IMean NewMom(DataType dtype, DateTime time, object ob, ErrMom err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.Object = ob;
            return (IMean)mom;
        }

        internal static IMean NewMom(DataType dtype, DateTime time, ErrMom err = null)
        {
            return (IMean)CreateMom(dtype, time, err);
        }

        private static Mean CreateMom(DataType dtype, DateTime time, ErrMom err)
        {
            if (err == null)
            {
                switch (dtype)
                {
                    case DataType.Real:
                        return new MomReal {Time = time};
                    case DataType.String:
                        return new MomString {Time = time};
                    case DataType.Integer:
                        return new MomInt {Time = time};
                    case DataType.Boolean:
                        return new MomBool {Time = time};
                    case DataType.Time:
                        return new MomTime {Time = time};
                    case DataType.Weighted:
                        return new MomWeighted {Time = time};
                    case DataType.Value:
                        return new MomValue {Time = time};
                }
            }
            switch (dtype)
            {
                case DataType.Real:
                    return new MomErrReal { Time = time, Error = err};
                case DataType.String:
                    return new MomErrString { Time = time, Error = err };
                case DataType.Integer:
                    return new MomErrInt { Time = time, Error = err };
                case DataType.Boolean:
                    return new MomErrBool { Time = time, Error = err };
                case DataType.Time:
                    return new MomErrTime { Time = time, Error = err };
                case DataType.Weighted:
                    return new MomWeighted { Time = time, Error = err};
                case DataType.Value:
                    return new MomValue { Time = time, Error = err};
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
                    return new MomListBool();
                case DataType.Integer:
                    return new MomListInt();
                case DataType.Real:
                    return new MomListReal();
                case DataType.String:
                    return new MomListString();
                case DataType.Time:
                    return new MomListTime();
                case DataType.Weighted:
                    return new MomListWeighted();
            }
            return null;
        }
        #endregion
    }
}