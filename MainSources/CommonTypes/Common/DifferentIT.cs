﻿using System.Collections.Generic;
using BaseLibrary;

namespace CommonTypes
{
    //Общие функции для InfoTask и конвертеры 
    public static class DifferentIT
    {
        //Чтение из реестра пути к каталогу InfoTask, в возвращаемом пути \ на конце
        public static string GetInfoTaskDir()
        {
            var dir = Different.GetRegistry(@"software\InfoTask", "InfoTaskPath");
            if (dir == "") dir = Different.GetRegistry(@"software\Wow6432Node\InfoTask", "InfoTaskPath");
            if (!dir.EndsWith(@"\")) dir += @"\";
            return dir;
        }

        //Путь к каталогу разработки InfoTask
        public static string GetInfoTaskDevelopDir()
        {
            var itd = GetInfoTaskDir();
            var n = itd.LastIndexOf(@"\", itd.Length - 2);
            return itd.Substring(0, n + 1);
        }

        //Возвращает тип ошибки как строку
        public static string ToRussian(this ErrorQuality quality)
        {
            switch (quality)
            {
                case ErrorQuality.Error:
                    return "Ошибка";
                case ErrorQuality.Warning:
                    return "Предупреждение";
            }
            return "";
        }

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

        //Определяет, является ли провайдер источником данных
        public static bool IsProviderSource(this ProviderType t)
        {
            return t == ProviderType.Source || t == ProviderType.Imitator;
        }

        //Перевод из строки в ProviderType
        public static ProviderType ToProviderType(this string t)
        {
            if (t == null) return ProviderType.Error;
            switch (t.ToLower())
            {
                case "коммуникатор":
                case "communicator":
                    return ProviderType.Communicator;
                case "коммуникаторприемника":
                case "communicatorreceiver":
                    return ProviderType.CommReceiver;
                case "источник":
                case "source":
                    return ProviderType.Source;
                case "архив":
                case "archive":
                    return ProviderType.Archive;
                case "приемник":
                case "receiver":
                    return ProviderType.Receiver;
                case "имитатор":
                case "imitator":
                    return ProviderType.Imitator;
                case "поток":
                case "thread":
                    return ProviderType.Thread;
                case "проект":
                case "project":
                    return ProviderType.Project;
            }
            return ProviderType.Error;
        }

        //Перевод из ProviderType в русское имя
        public static string ToRussian(this ProviderType t)
        {
            switch (t)
            {
                case ProviderType.Communicator:
                    return "Коммуникатор";
                case ProviderType.CommReceiver:
                    return "КоммуникаторПриемника";
                case ProviderType.Source:
                    return "Источник";
                case ProviderType.Archive:
                    return "Архив";
                case ProviderType.Receiver:
                    return "Приемник";
                case ProviderType.Imitator:
                    return "Имитатор";
                case ProviderType.Thread:
                    return "Поток";
                case ProviderType.Project:
                    return "Проект";
            }
            return "Ошибка";
        }

        //Перевод из ProviderType в английское имя
        public static string ToEnglish(this ProviderType t)
        {
            switch (t)
            {
                case ProviderType.Communicator:
                    return "Communicator";
                case ProviderType.CommReceiver:
                    return "CommunicatorReceiver";
                case ProviderType.Source:
                    return "Source";
                case ProviderType.Archive:
                    return "Archive";
                case ProviderType.Receiver:
                    return "Receiver";
                case ProviderType.Imitator:
                    return "Imitator";
                case ProviderType.Thread:
                    return "Thread";
                case ProviderType.Project:
                    return "Project";
            }
            return "Error";
        }

        //ToDo Переделать
        //Список совместимых кодов провайдеров для выбранного кода провайдера
        public static List<string> JointProviderCodes(this string code)
        {
            var list = new List<string> { code };
            using (var rec = new RecDao(GetInfoTaskDir() + @"General\Config.accdb", "SysTabl"))
            {
                if (code == "CloneSource")
                {
                    while (rec.Read())
                        if (rec.GetString("ParamValue") == "Источник" && rec.GetString("ParamName") != "CloneSource")
                            list.Add(rec.GetString("ParamName"));
                }
                else
                {
                    rec.FindFirst("ParamName", code);
                    if (!rec.NoMatch())
                    {
                        var set = rec.GetString("ParamTag").ToPropertyDicS()["JointProviders"].ToPropertyHashSet();
                        foreach (var c in set)
                        {
                            rec.FindFirst("ParamName", c);
                            if (!rec.NoMatch()) list.Add(c);
                        }
                    }
                }
            }
            return list;
        }

        //Перевод из строки в ProviderSetupType
        public static ProviderSetupType ToProviderSetupType(this string s)
        {
            if (s.IsEmpty()) return ProviderSetupType.Error;
            switch (s.ToLower())
            {
                case "reporter":
                    return ProviderSetupType.Reporter;
                case "controller":
                    return ProviderSetupType.Controller;
                case "reporterarchive":
                    return ProviderSetupType.ReporterArchive;
                case "analyzer":
                    return ProviderSetupType.Analyzer;
                case "constructor":
                    return ProviderSetupType.Constructor;
                case "default":
                    return ProviderSetupType.Default;
            }
            return ProviderSetupType.Error;
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

        //Перевод из строки в InterpolationType
        public static InterpolationType ToInterpolation(this string t)
        {
            if (t == null) return InterpolationType.None;
            switch (t.ToLower())
            {
                case "ступенчатая":
                case "constant":
                    return InterpolationType.Constant;
                case "линейная":
                case "linear":
                    return InterpolationType.Linear;
                case "":
                    return InterpolationType.None;
            }
            return InterpolationType.Error;
        }

        //Перевод из InterpolationType в строку
        public static string ToRussian(this InterpolationType t)
        {
            switch (t)
            {
                case InterpolationType.Constant:
                    return "Ступенчатая";
                case InterpolationType.Linear:
                    return "Линейная";
            }
            return null;
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
            if (sp == SuperProcess.Average ) return DataType.Real;
            if (sp == SuperProcess.Summ && dt == DataType.Boolean) return DataType.Integer;
            return dt;
        }

        //Перевод из строки в тип приложения
        public static ApplicationType ToApplicationType(this string m)
        {
            if (m == null) return ApplicationType.Error;
            switch (m.ToLower())
            {
                case "конструктор":
                case "constructor":
                    return ApplicationType.Constructor;
                case "рас":
                case "ras":
                    return ApplicationType.Ras;
                case "контроллер":
                case "controller":
                    return ApplicationType.Controller;
                case "анализатор":
                case "analyzer":
                    return ApplicationType.Analyzer;
                case "excel":
                    return ApplicationType.Excel;
                case "powerppoint":
                    return ApplicationType.PowerPoint;
                case "ошибка":
                case "error":
                    return ApplicationType.Error;
            }
            return ApplicationType.Error;
        }

        //Перевод из режима расчета в строку
        public static string ToRussian(this ApplicationType m)
        {
            switch (m)
            {
                case ApplicationType.Constructor:
                    return "Конструктор";
                case ApplicationType.Ras:
                    return "РАС";
                case ApplicationType.Controller:
                    return "Контроллер";
                case ApplicationType.Analyzer:
                    return "Анализатор";
                case ApplicationType.Excel:
                    return "Excel";
                case ApplicationType.PowerPoint:
                    return "PowerPoint";
            }
            return "Ошибка";
        }

        //Перевод из режима расчета в строку
        public static string ToEnglish(this ApplicationType m)
        {
            switch (m)
            {
                case ApplicationType.Constructor:
                    return "Constructor";
                case ApplicationType.Ras:
                    return "Ras";
                case ApplicationType.Controller:
                    return "Controller";
                case ApplicationType.Analyzer:
                    return "Analyzer";
                case ApplicationType.Excel:
                    return "Excel";
                case ApplicationType.PowerPoint:
                    return "PowerPoint";
            }
            return "Error";
        }

        //Перевод из режима расчета в режим натсроек
        public static ProviderSetupType ToProviderSetupType(this ApplicationType m)
        {
            switch (m)
            {
                case ApplicationType.Constructor:
                    return ProviderSetupType.Constructor;
                case ApplicationType.Controller:
                    return ProviderSetupType.Controller;
                case ApplicationType.Analyzer:
                    return ProviderSetupType.Analyzer;
            }
            return ProviderSetupType.Reporter;
        }

        //Приложение является посторителем отчетов
        public static bool IsReport(this ApplicationType t)
        {
            return t == ApplicationType.Excel || t == ApplicationType.PowerPoint;
        }

        //Перевод строки в тип отчета 
        public static ReportType ToReportType(this string s)
        {
            if (s == null) return ReportType.Error;
            switch (s.ToLower())
            {
                case "calc":
                case "расчет":
                    return ReportType.Calc;
                case "template":
                case "шаблон":
                    return ReportType.Template;
                case "источник":
                case "source":
                    return ReportType.Source;
                case "excel":
                    return ReportType.Excel;
                case "powerpoint":
                    return ReportType.PowerPoint;
            }
            return ReportType.Error;
        }

        //Перевод типа шаблона в русское имя
        public static string ToEnglish(this ReportType t)
        {
            switch (t)
            {
                case ReportType.Calc:
                    return "Calc";
                case ReportType.Template:
                    return "Template";
                case ReportType.Source:
                    return "Source";
                case ReportType.Excel:
                    return "Excel";
                case ReportType.PowerPoint:
                    return "PowerPoint";
            }
            return "Ошибка";
        }

        //Перевод типа отчета в английское имя
        public static string ToRussian(this ReportType t)
        {
            switch (t)
            {
                case ReportType.Calc:
                    return "Расчет";
                case ReportType.Template:
                    return "Шаблон";
                case ReportType.Source:
                    return "Источник";
                case ReportType.Excel:
                    return "Excel";
                case ReportType.PowerPoint:
                    return "PowerPoint";
            }
            return "Error";
        }

        //Перевод строки в тип интервала 
        public static IntervalType ToIntervalType(this string s)
        {
            if (s.IsEmpty()) return IntervalType.Empty;
            switch (s.ToLower())
            {
                case "single":
                case "разовый":
                    return IntervalType.Single;
                case "named":
                case "именованный":
                    return IntervalType.Named;
                case "namedadd":
                    return IntervalType.NamedAdd;
                case "namedaddparams":
                    return IntervalType.NamedAddParams;
                case "periodic":
                    return IntervalType.Periodic;
                case "moments":
                case "мгновенные":
                    return IntervalType.Moments;
                case "base":
                case "базовый":
                    return IntervalType.Base;
                case "hour":
                case "часовой":
                    return IntervalType.Hour;
                case "day":
                case "суточный":
                    return IntervalType.Day;
                case "absolute":
                case "абсолютный":
                    return IntervalType.Absolute;
                case "absoluteday":
                case "абсолютныйсутки":
                    return IntervalType.AbsoluteDay;
                case "combined":
                case "комбинированный":
                    return IntervalType.Combined;
            }
            return IntervalType.Empty;
        }

        //Перевод типа интервала в английское имя
        public static string ToEnglish(this IntervalType t)
        {
            switch (t)
            {
                case IntervalType.Single:
                    return "Single";
                case IntervalType.Named:
                case IntervalType.NamedAdd:
                case IntervalType.NamedAddParams:
                    return "Named";
                case IntervalType.Periodic:
                    return "Periodic";
                case IntervalType.Moments:
                    return "Moments";
                case IntervalType.Base:
                    return "Base";
                case IntervalType.Hour:
                    return "Hour";
                case IntervalType.Day:
                    return "Day";
                case IntervalType.Absolute:
                    return "Absolute";
                case IntervalType.AbsoluteDay:
                    return "AbsoluteDay";
                case IntervalType.Combined:
                    return "Combined";
                case IntervalType.Empty:
                    return null;
            }
            return null;
        }

        //Перевод типа интервала в русское имя
        public static string ToRussian(this IntervalType t)
        {
            switch (t)
            {
                case IntervalType.Single:
                    return "Разовый";
                case IntervalType.Named:
                case IntervalType.NamedAdd:
                case IntervalType.NamedAddParams:
                    return "Именованный";
                case IntervalType.Periodic:
                    return "Периодический";
                case IntervalType.Moments:
                    return "Мгновенные";
                case IntervalType.Base:
                    return "Базовый";
                case IntervalType.Hour:
                    return "Часовой";
                case IntervalType.Day:
                    return "Суточный";
                case IntervalType.Absolute:
                    return "Абсолютный";
                case IntervalType.AbsoluteDay:
                    return "АбсолютныйСутки";
                case IntervalType.Combined:
                    return "Комбинированный";
                case IntervalType.Empty:
                    return null;
            }
            return null;
        }

        //True, если тип интервал является именованным
        public static bool IsNamed(this IntervalType t)
        {
            return t == IntervalType.Named || t == IntervalType.NamedAdd || t == IntervalType.NamedAddParams;
        }

        //True, если тип интервала используется при разовом расчете
        public static bool IsSingle(this IntervalType t)
        {
            return t == IntervalType.Single || t.IsNamed();
        }

        //True, если тип интервала используется при периодическом накоплении
        public static bool IsPeriodic(this IntervalType t)
        {
            return t == IntervalType.Base || t == IntervalType.Hour || t == IntervalType.Day || t == IntervalType.Combined;
        }

        //True, если тип интервала используется при абсолютном накоплении
        public static bool IsAbsolute(this IntervalType t)
        {
            return t == IntervalType.Absolute || t == IntervalType.AbsoluteDay;
        }

        //Название таблицы числовых значений архива по типу интервала
        public static string ToValuesTable(this IntervalType t)
        {
            if (t.IsNamed()) return "NamedValues";
            return t.ToEnglish() + "Values";
        }

        //Название таблицы строковых значений архива по типу интервала
        public static string ToStrValuesTable(this IntervalType t)
        {
            if (t.IsNamed()) return "NamedStrValues";
            return t.ToEnglish() + "StrValues";
        }

        //Название таблицы интервалов архива по типу интервала
        public static string ToIntervalsTable(this IntervalType t)
        {
            if (t.IsNamed()) return "NamedIntervals";
            return t.ToEnglish() + "Intervals";
        }

        //Номер часа, в начале которого удаляются старые интервалы указанного типа
        public static int HourNumber(this IntervalType t)
        {
            switch (t)
            {
                case IntervalType.Base:
                    return 2;
                case IntervalType.Hour:
                    return 4;
                case IntervalType.Day:
                    return 6;
                case IntervalType.AbsoluteDay:
                    return 8;
                case IntervalType.Moments:
                    return 22;
            }
            return 0;
        }

        //Перевод строки в ImitMode, isImit - включена имитация
        public static ImitMode ToImitMode(this string t, bool isImit)
        {
            if (!isImit || t.IsEmpty()) return ImitMode.NoImit;
            switch (t.ToLower())
            {
                case "отсчитывать от начала периода":
                case "frombegin":
                    return ImitMode.FromBegin;
                case "отсчитывать от начала часа":
                case "fromhour":
                    return ImitMode.FromHour;
                case "отсчитывать от начала суток":
                case "fromday":
                    return ImitMode.FromDay;
            }
            return ImitMode.NoImit;
        }

        //Перевод ImitMode в русскую строку
        public static string ToRussian(this ImitMode t)
        {
            switch (t)
            {
                case ImitMode.FromBegin:
                    return "Отсчитывать от начала периода";
                case ImitMode.FromHour:
                    return "Отсчитывать от начала часа";
                case ImitMode.FromDay:
                    return "Отсчитывать от начала суток";
            }
            return "";
        }

        //Выбирает одну ошибку из двух
        public static ErrMom Add(this ErrMom err1, ErrMom err2)
        {
            if (err1 == null) return err2;
            if (err2 == null) return err1;
            if (err2.Quality > err1.Quality) return err2;
            return err1;
        }
    }
}