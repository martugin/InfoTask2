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
            if (t1 is TablikParam && t2 is TablikParam)
            {
                var p1 = (TablikParam) t1; 
                var p2 = (TablikParam) t2;
                if (p1.BaseParams.Contains(p2)) return p2;
                if (p2.BaseParams.Contains(p1)) return p1;
            }
            var st1 = t1.TablikSignalType;
            var st2 = t2.TablikSignalType;
            if (st1 == null || st2 == null)
                return new SimpleType(t1.DataType.Add(t2.DataType));

            if (st1 is TablikObject && st2 == st1)
                return st1;
            if ((st1 is TablikObject || st1 is ObjectType) && (st2 is TablikObject || st2 is ObjectType))
            {
                var o1 = st1 is ObjectType ? (ObjectType) st1 : ((TablikObject) st1).ObjectType;
                var o2 = st2 is ObjectType ? (ObjectType) st2 : ((TablikObject) st2).ObjectType;
                if (o1 == o2 || o1.BaseTypes.Contains(o2)) return o2;
                if (o2.BaseTypes.Contains(o1)) return o1;
                return new SimpleType(t1.DataType.Add(t2.DataType));
            }

            if (st1 is TablikSignal && st2.Signal == st1) return st1;
            if (st2 is TablikSignal && st1.Signal == st2) return st2;
            if (st1 is ObjectType && st1.Signal == st2.Signal) return st1.Signal;
            if (st2 is ObjectType && st1.Signal == st2.Signal) return st2.Signal;
            if (st1 is UsedSignal && st2 == st1) return st1;
            if (st1 is TablikObject && st1.Signal == st2.Signal) return t2;
            if (st2 is TablikObject && st1.Signal == st2.Signal) return t1;
            return new SimpleType(t1.DataType.Add(t2.DataType));
        }
    }
}
