using System;
using BaseLibrary;

namespace CommonTypes
{
    //Фабрика мгновенных значений: Meam, EMean, Mom
    public static class MomFactory
    {
        //NewMean
        #region
        public static IMean NewMean(DataType dtype, bool b, ErrMom err = null)
        {
            var mean = CreateMean(dtype, err);
            mean.Boolean = b;
            return mean;
        }

        public static Mean NewMean(DataType dtype, int i, ErrMom err = null)
        {
            var mean = CreateMean(dtype, err);
            mean.Integer = i;
            return mean;
        }

        public static Mean NewMean(DataType dtype, double r, ErrMom err = null)
        {
            var mean = CreateMean(dtype, err);
            mean.Real = r;
            return mean;
        }

        public static Mean NewMean(DataType dtype, string s, ErrMom err = null)
        {
            var mean = CreateMean(dtype, err);
            mean.String = s;
            return mean;
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
                Mean m = null;
                switch (dtype)
                {
                    case DataType.Real:
                        m = new MeanErrReal();
                        break;
                    case DataType.String:
                        m = new MeanErrString();
                        break;
                    case DataType.Integer:
                        m = new MeanErrInt();
                        break;
                    case DataType.Boolean:
                        m = new MeanErrBool();
                        break;
                    case DataType.Time:
                        m = new MeanErrTime();
                        break;
                }
                if (m != null) m.Error = err;
                return m;
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
        public static IMom NewMom(DataType dtype, DateTime time, bool b, ErrMom err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.Boolean = b;
            return (IMom) mom;
        }
        public static IMom NewMom(DataType dtype, DateTime time, int i, ErrMom err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.Integer = i;
            return (IMom)mom;
        }
        public static IMom NewMom(DataType dtype, DateTime time, double r, ErrMom err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.Real = r;
            return (IMom)mom;
        }
        public static IMom NewMom(DataType dtype, DateTime time, string s, ErrMom err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.String = s;
            return (IMom)mom;
        }
        public static IMom NewMom(DataType dtype, DateTime time, DateTime d, ErrMom err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.Date = d;
            return (IMom)mom;
        }
        public static IMom NewMom(DataType dtype, DateTime time, object ob, ErrMom err = null)
        {
            var mom = CreateMom(dtype, time, err);
            mom.Object = ob;
            return (IMom)mom;
        }

        internal static IMom NewMom(DataType dtype, DateTime time, ErrMom err = null)
        {
            return (IMom)CreateMom(dtype, time, err);
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
            }
        }
        #endregion
    }
}