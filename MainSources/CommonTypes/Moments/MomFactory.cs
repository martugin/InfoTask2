using System;
using BaseLibrary;

namespace CommonTypes
{
    //Фабрика мгновенных значений: Meam, EMean, Mom
    public static class MomFactory
    {
        //NewMean
        #region
        public static IMean NewMean(DataType dtype, bool b)
        {
            var mean = CreateMean(dtype);
            mean.Boolean = b;
            return mean;
        }

        public static IMean NewEMean(DataType dtype, bool b, ErrMom err = null)
        {
            var mean = CreateEMean(dtype);
            mean.Boolean = b;
            mean.Error = err;
            return mean;
        }

        public static Mean NewMean(DataType dtype, int i, ErrMom err = null)
        {
            var mean = CreateMean(dtype);
            mean.Integer = i;
            return mean;
        }

        public static Mean NewEMean(DataType dtype, int i, ErrMom err = null)
        {
            var mean = CreateEMean(dtype);
            mean.Integer = i;
            mean.Error = err;
            return mean;
        }

        public static Mean NewMean(DataType dtype, double r)
        {
            var mean = CreateMean(dtype);
            mean.Real = r;
            return mean;
        }

        public static Mean NewEMean(DataType dtype, double r, ErrMom err = null)
        {
            var mean = CreateMean(dtype);
            mean.Real = r;
            mean.Error = err;
            return mean;
        }

        public static Mean NewMean(DataType dtype, string s)
        {
            var mean = CreateMean(dtype);
            mean.String = s;
            return mean;
        }

        public static Mean NewEMean(DataType dtype, string s, ErrMom err = null)
        {
            var mean = CreateMean(dtype);
            mean.String = s;
            mean.Error = err;
            return mean;
        }

        public static Mean NewMean(DataType dtype, DateTime d)
        {
            var mean = CreateMean(dtype);
            mean.Date = d;
            return mean;
        }

        public static Mean NewEMean(DataType dtype, DateTime d, ErrMom err = null)
        {
            var mean = CreateMean(dtype);
            mean.Date = d;
            mean.Error = err;
            return mean;
        }

        private static Mean CreateMean(DataType dtype)
        {
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
            return null;
        }

        private static Mean CreateEMean(DataType dtype)
        {
            switch (dtype)
            {
                case DataType.Real:
                    return new EMeanReal();
                case DataType.String:
                    return new EMeanString();
                case DataType.Integer:
                    return new EMeanInt();
                case DataType.Boolean:
                    return new EMeanBool();
                case DataType.Time:
                    return new EMeanTime();
            }
            return null;
        }
        #endregion

        //NewMom
        #region
        public static IMom NewMom(DataType dtype, DateTime time, bool b, ErrMom err = null)
        {
            var mom = CreateMom(dtype, time);
            mom.Boolean = b;
            mom.Error = err;
            return (IMom) mom;
        }
        public static IMom NewMom(DataType dtype, DateTime time, int i, ErrMom err = null)
        {
            var mom = CreateMom(dtype, time);
            mom.Integer = i;
            mom.Error = err;
            return (IMom)mom;
        }
        public static IMom NewMom(DataType dtype, DateTime time, double r, ErrMom err = null)
        {
            var mom = CreateMom(dtype, time);
            mom.Real = r;
            mom.Error = err;
            return (IMom)mom;
        }
        public static IMom NewMom(DataType dtype, DateTime time, string s, ErrMom err = null)
        {
            var mom = CreateMom(dtype, time);
            mom.String = s;
            mom.Error = err;
            return (IMom)mom;
        }
        public static IMom NewMom(DataType dtype, DateTime time, DateTime d, ErrMom err = null)
        {
            var mom = CreateMom(dtype, time);
            mom.Date = d;
            mom.Error = err;
            return (IMom)mom;
        }
        public static IMom NewMom(DataType dtype, DateTime time, object ob, ErrMom err = null)
        {
            var mom = CreateMom(dtype, time);
            mom.Object = ob;
            mom.Error = err;
            return (IMom)mom;
        }

        internal static IMom NewMom(DataType dtype, DateTime time, ErrMom err = null)
        {
            var mom = CreateMom(dtype, time);
            mom.Error = err;
            return (IMom)mom;
        }

        private static Mean CreateMom(DataType dtype, DateTime time)
        {
            switch (dtype)
            {
                case DataType.Real:
                    return new MomReal {Time = time};
                case DataType.String:
                    return new MomString { Time = time };
                case DataType.Integer:
                    return new MomInt { Time = time };
                case DataType.Boolean:
                    return new MomBool { Time = time };
                case DataType.Time:
                    return new MomTime { Time = time };
            }
            return null;
        }
        #endregion

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
    }
}