using BaseLibrary;
using CommonTypes;

namespace Tablik
{
    //Общие статические функции для Tablik
    internal static class TablikStatic
    {
        //Перевод строки в ArrayType
        internal static ArrayType ToArrayType(this string s)
        {
            if (s.IsEmpty()) return ArrayType.Single;
            switch (s.ToLower())
            {
                case "list":
                case "список":
                    return ArrayType.List;
                case "dicnumbers":
                case "словарьчисла":
                    return ArrayType.DicNumbers;
                case "dicstrings":
                case "словарьстроки":
                    return ArrayType.DicStrings;
            }
            return ArrayType.Error;
        }

        //Перевод ArrayType в английскую строку
        internal static string ToEnglish(this ArrayType t)
        {
            switch (t)
            {
                case ArrayType.Single:
                    return "";
                case ArrayType.List:
                    return "List";
                case ArrayType.DicNumbers:
                    return "DicNumbers";
                case ArrayType.DicStrings:
                    return "DicStrings";
            }
            return "";
        }


        //Перевод ArrayType в русскую строку
        internal static string ToRussian(this ArrayType t)
        {
            switch (t)
            {
                case ArrayType.Single:
                    return "";
                case ArrayType.List:
                    return "Список";
                case ArrayType.DicNumbers:
                    return "СловарьЧисла";
                case ArrayType.DicStrings:
                    return "СловарьСтроки";
            }
            return "";
        }

        //Сложение типов данных
        internal static ITablikType Add(this ITablikType t1, ITablikType t2)
        {
            if (t1 == null) return t2;
            if (t2 == null) return t1;
            if (t1.IsOfType(t2)) return t2;
            if (t2.IsOfType(t1)) return t1;
            return t1.Simple.ArrayType == t2.Simple.ArrayType 
                ? new SimpleType(t1.DataType.Add(t2.DataType), t1.Simple.ArrayType) 
                : new SimpleType();
        }
    }
}