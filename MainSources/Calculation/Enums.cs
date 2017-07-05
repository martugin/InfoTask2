using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Тип функции
    public enum FunType
    {
        Scalar, //Скалярные
        ScalarComplex, //Скалярные с указанием переменных, используемых в данной точке
        ScalarObject, //Скалярные с первым не скалярным параметром 
        Const, //Без параметров 
        Object, //С первым не скалярным параметром (Tabl, Prev), а остальными - Mom
        Moments, //Работа со списками мгновенных значений
        CalcData, //Работа с сегментами и статистические функции
        Calc, //Работа с расчетными значениями
        Val, //Общие функции
        Operator //Операторы
    }

    //--------------------------------------------------------------------
    //Тип графика
    public enum GraficType
    {
        Grafic, //График
        Grafic0, //График - ступенчатая интерполяция
        Diagramm, //Диаграмма
        Error
    }

    //----------------------------------------------------------------------
    //Способ формирования параметра для ведомости или архива результатов
    public enum ArchiveParamType
    {
        Param, //Параметр
        SubParam, //Подпараметр
        NotSave, //Не сохранять
        Error //Ошибка
    }

    //----------------------------------------------------------------------
    //Типы накопления
    public enum SuperProcess
    {
        Moment, //Мгновенные
        First, //Первое
        Last, //Последнее
        Min, //Минимум
        Max, //Максимум
        Average, //Среднее
        Summ, //Сумма
        None, //Нет накопления
        Error //Не правильно заполненное
    }

    //----------------------------------------------------------------------
    //Работа с перечислениями
    public static class CalulationEnumsConv
    {
        //Перевод типа функции в строку
        public static string ToString(this FunType f)
        {
            switch (f)
            {
                case FunType.Scalar:
                    return "Scalar";
                case FunType.CalcData:
                    return "CalcData";
                case FunType.Moments:
                    return "Moments";
                case FunType.Operator:
                    return "Operator";
                case FunType.Const:
                    return "Const";
                case FunType.ScalarObject:
                    return "ScalarObject";
                case FunType.Object:
                    return "Object";
                case FunType.ScalarComplex:
                    return "ScalarComplex";
                case FunType.Val:
                    return "Val";
                case FunType.Calc:
                    return "Calc";
            }
            return "Val";
        }

        //Перевод строки в тип функции
        public static FunType ToFunType(this string s)
        {
            if (s.IsEmpty()) return FunType.Val;
            switch (s.ToLower())
            {
                case "scalar":
                    return FunType.Scalar;
                case "calcdata":
                    return FunType.CalcData;
                case "moments":
                    return FunType.Moments;
                case "operator":
                    return FunType.Operator;
                case "const":
                    return FunType.Const;
                case "scalarobject":
                    return FunType.ScalarObject;
                case "object":
                    return FunType.Object;
                case "scalarcomplex":
                    return FunType.ScalarComplex;
                case "val":
                    return FunType.Val;
                case "calc":
                    return FunType.Calc;
            }
            return FunType.Val;
        }

        //Перевод строки в тип графика
        public static GraficType ToGraficType(this string t)
        {
            if (t == null) return GraficType.Error;
            switch (t.ToLower())
            {
                case "график":
                    return GraficType.Grafic;
                case "график0":
                    return GraficType.Grafic0;
                case "диаграмма":
                    return GraficType.Diagramm;
            }
            return GraficType.Error;
        }

        //Перевод типа графика в строку
        public static string ToRussian(this GraficType t)
        {
            switch (t)
            {
                case GraficType.Grafic:
                    return "График";
                case GraficType.Grafic0:
                    return "График0";
                case GraficType.Diagramm:
                    return "Диаграмма";
            }
            return "Ошибка";
        }

        //Перевод из строки в ArchiveParamType
        public static ArchiveParamType ToArchiveParamType(this string t)
        {
            if (t == null) return ArchiveParamType.NotSave;
            switch (t.ToLower())
            {
                case "param":
                case "параметр":
                    return ArchiveParamType.Param;
                case "subparam":
                case "подпараметр":
                    return ArchiveParamType.SubParam;
                case "notsave":
                case "не сохранять":
                    return ArchiveParamType.NotSave;
            }
            return ArchiveParamType.Error;
        }

        //Перевод из ArchiveParamType в строку
        public static string ToRussian(this ArchiveParamType t)
        {
            switch (t)
            {
                case ArchiveParamType.Param:
                    return "Параметр";
                case ArchiveParamType.SubParam:
                    return "Подпараметр";
                case ArchiveParamType.NotSave:
                    return "Не сохранять";
                case ArchiveParamType.Error:
                    return "Ошибка";
            }
            return "";
        }

        //Перевод из строки в SuperProcessType
        public static SuperProcess ToSuperProcess(this string t)
        {
            if (t == null) return SuperProcess.None;
            switch (t.ToLower())
            {
                case "мгновенные":
                case "moment":
                    return SuperProcess.Moment;
                case "среднее":
                case "average":
                    return SuperProcess.Average;
                case "минимум":
                case "minimum":
                    return SuperProcess.Min;
                case "максимум":
                case "maximum":
                    return SuperProcess.Max;
                case "первое":
                case "first":
                    return SuperProcess.First;
                case "последнее":
                case "last":
                    return SuperProcess.Last;
                case "сумма":
                case "summ":
                    return SuperProcess.Summ;
                case "":
                    return SuperProcess.None;
            }
            return SuperProcess.Error;
        }

        //Перевод из SuperProcessType в строку
        public static string ToRussian(this SuperProcess t)
        {
            switch (t)
            {
                case SuperProcess.Moment:
                    return "Мгновенные";
                case SuperProcess.Average:
                    return "Среднее";
                case SuperProcess.Min:
                    return "Минимум";
                case SuperProcess.Max:
                    return "Максимум";
                case SuperProcess.First:
                    return "Первое";
                case SuperProcess.Last:
                    return "Последнее";
                case SuperProcess.Summ:
                    return "Сумма";
            }
            return null;
        }

        //True, если тип накопления не заполнен, или заполнен с ошибкой
        public static bool IsNone(this SuperProcess t)
        {
            return t == SuperProcess.None || t == SuperProcess.Error;
        }


        //Как преобразуется тип расчетного параметра при преобразовании в архивный параметр с учетом типа накопления
        public static DataType AplySuperProcess(this DataType dt, SuperProcess sp)
        {
            if (sp == SuperProcess.Average) return DataType.Real;
            if (sp == SuperProcess.Summ && dt == DataType.Boolean) return DataType.Integer;
            return dt;
        }
    }
}