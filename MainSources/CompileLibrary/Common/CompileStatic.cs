using BaseLibrary;
using CommonTypes;

namespace CompileLibrary
{
    //Статические функции и конвертеры
    public static class CompileStatic
    {
        //Перевод строки в ArrayType
        public static ArrayType ToArrayType(this string s)
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
        public static string ToEnglish(this ArrayType t)
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
        public static string ToRussian(this ArrayType t)
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