using CommonTypes;

namespace Tablik
{
    //Общие статические функции для Tablik 
    internal static class TablikStatic
    {
        //Сложение типов данных
        internal static ITablikType Add(this ITablikType t1, ITablikType t2)
        {
            if (t1 == null) return t2;
            if (t2 == null) return t1;
            if (t1.LessOrEquals(t2)) return t2;
            if (t2.LessOrEquals(t1)) return t1;
            var ts1 = t1.TablikSignalType;
            var ts2 = t2.TablikSignalType;
            if (ts1 != null && ts2 != null)
            {
                if (ts1.LessOrEquals(ts2)) return ts2;
                if (ts2.LessOrEquals(ts1)) return ts1;
            }
            return t1.Simple.ArrayType == t2.Simple.ArrayType 
                ? new SimpleType(t1.DataType.Add(t2.DataType), t1.Simple.ArrayType) 
                : new SimpleType();
        }
    }
}