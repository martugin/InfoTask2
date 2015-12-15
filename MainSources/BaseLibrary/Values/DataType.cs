using System;
using Microsoft.Office.Interop.Access.Dao;

namespace BaseLibrary
{
    //ToDo перенести в CommonTypes
    //Типы данных
    public enum DataType : byte
    {
        Value = 0, //Время, недостоверность и ошибка без начения
        Boolean = 1, //Логическое значение
        Integer = 2, //Целое значение
        Weighted = 3, //Действительное значение, с указанием длины периода
        Real = 4, //Действительное значение
        Time = 5, //Значение тип Время
        String = 6, //Строковое значение
        Variant = 7, //Значение неопределенного типа
        Segments = 8, //Набор сегментов
        Error = 9 //Задан неверный тип
    }

    //---------------------------------------------------------------------

    public static class DataTypeConv
    {
        //Преобразование в DataType
        public static DataType ToDataType(this string s)
        {
            if (s == null) return DataType.Error;
            switch (s.ToLower())
            {
                case "real":
                case "действ":
                    return DataType.Real;
                case "int":
                case "целое":
                    return DataType.Integer;
                case "bool":
                case "логич":
                    return DataType.Boolean;
                case "string":
                case "строка":
                    return DataType.String;
                case "variant":
                case "вариант":
                    return DataType.Variant;
                case "время":
                case "time":
                    return DataType.Time;
                case "weighted":
                case "взвешенное":
                    return DataType.Weighted;
                case "segments":
                case "сегменты":
                    return DataType.Segments;
                case "value":
                case "величина":
                    return DataType.Value;
                case "error":
                case "ошибка":
                    return DataType.Error;
            }
            return DataType.Error;
        }

        //Получение буквы типа 
        public static char ToLetter(this DataType d)
        {
            switch (d)
            {
                case DataType.Real:
                    return 'r';
                case DataType.Integer:
                    return 'i';
                case DataType.Boolean:
                    return 'b';
                case DataType.Error:
                    return 'e';
                case DataType.Variant:
                    return 'u';
                case DataType.String:
                    return 's';
                case DataType.Time:
                    return 'd';
                case DataType.Weighted:
                    return 'w';
                case DataType.Segments:
                    return 'g';
                case DataType.Value:
                    return 'v';
            }
            return 'e';
        }

        //Получение русского имени типа 
        public static string ToRussian(this DataType d)
        {
            switch (d)
            {
                case DataType.Real:
                    return "Действ";
                case DataType.Integer:
                    return "Целое";
                case DataType.Boolean:
                    return "Логич";
                case DataType.String:
                    return "Строка";
                case DataType.Time:
                    return "Время";
                case DataType.Weighted:
                    return "Взвешенное";
                case DataType.Segments:
                    return "Сегменты";
                case DataType.Variant:
                    return "Вариант";
                case DataType.Value:
                    return "Величина";
            }
            return "Ошибка";
        }

        //Получение имени типа 
        public static string ToEnglish(this DataType d)
        {
            switch (d)
            {
                case DataType.Real:
                    return "Real";
                case DataType.Integer:
                    return "Int";
                case DataType.Boolean:
                    return "Bool";
                case DataType.String:
                    return "String";
                case DataType.Time:
                    return "Time";
                case DataType.Weighted:
                    return "Weighted";
                case DataType.Segments:
                    return "Segments";
                case DataType.Variant:
                    return "Variant";
                case DataType.Value:
                    return "Value";
            }
            return "Error";
        }

        //Получение типа данных из типа данных таблицы Access
        public static DataType ToDataType(this short d)
        {
            switch ((DataTypeEnum)d)
            {
                case DataTypeEnum.dbText:
                case DataTypeEnum.dbMemo:
                    return DataType.String;
                case DataTypeEnum.dbInteger:
                case DataTypeEnum.dbLong:
                case DataTypeEnum.dbBigInt:
                case DataTypeEnum.dbByte:
                    return DataType.Integer;
                case DataTypeEnum.dbDouble:
                case DataTypeEnum.dbSingle:
                case DataTypeEnum.dbDecimal:
                case DataTypeEnum.dbFloat:
                    return DataType.Real;
                case DataTypeEnum.dbBoolean:
                    return DataType.Boolean;
                case DataTypeEnum.dbDate:
                case DataTypeEnum.dbTime:
                case DataTypeEnum.dbTimeStamp:
                    return DataType.Time;
            }
            return DataType.Error;
        }

        //Получение типа данных таблицы Access из типа данных 
        public static DataTypeEnum ToColumnType(this DataType d)
        {
            switch (d)
            {
                case DataType.String:
                    return DataTypeEnum.dbText;
                case DataType.Real:
                case DataType.Weighted:
                    return DataTypeEnum.dbDouble;
                case DataType.Boolean:
                    return DataTypeEnum.dbBoolean;
                case DataType.Integer:
                    return DataTypeEnum.dbLong;
                case DataType.Time:
                    return DataTypeEnum.dbDate;
            }
            return DataTypeEnum.dbText;
        }

        //Сравнение двух типов данных, True - если первый приводим ко второму
        public static bool LessOrEquals(this DataType t1, DataType t2)
        {
            if (t1 > t2) return false;
            if (t2 <= DataType.Integer || t2 == DataType.Real || t2 == DataType.String || t2 == DataType.Variant)
                return true;
            if (t1 == t2) return true;
            if (t2 == DataType.Time || t2 == DataType.Weighted) 
                return t1 == DataType.Value;
            return false;
        }

        //Возвращает общий минимум для двух типов
        public static DataType Inf(this DataType t1, DataType t2)
        {
            if (t2.LessOrEquals(t1)) return t2;
            if (t1.LessOrEquals(t2)) return t1;
            return DataType.Value;
        }

        //Возвращает общий максимум для двух типов
        public static DataType Add(this DataType t1, DataType t2)
        {
            if (t2.LessOrEquals(t1)) return t1;
            if (t1.LessOrEquals(t2)) return t2;
            if (t1 == DataType.Segments || t2 == DataType.Segments)
                return DataType.Error;
            return DataType.String;
        }

        //Проверяет, является ли заданная строка константой указанного типа данных (без кавычек и диезов)
        public static bool IsOfType(this string s, DataType type)
        {
            switch (type)
            {
                case DataType.Boolean:
                    return s == "0" || s == "1";
                case DataType.Integer:
                    int resi;
                    return int.TryParse(s, out resi);
                case DataType.Real:
                    return !double.IsNaN(s.ToDouble());
                case DataType.Time:
                    DateTime rest;
                    return DateTime.TryParse(s, out rest);
                case DataType.String:
                    return s.Length <= 255;
            }
            return true;
        }
    }
}
