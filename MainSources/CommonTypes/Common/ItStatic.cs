using System;
using System.IO;
using System.Reflection;
using BaseLibrary;

namespace CommonTypes
{
    //Общие функции для InfoTask и конвертеры 
    public static class ItStatic
    {
        //Чтение из реестра пути к каталогу InfoTask, в возвращаемом пути \ на конце
        public static string InfoTaskDir()
        {
            var fi = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var dir = fi.Directory.Parent;
            if (dir.Name == "Providers" || dir.Name == "TestsRun") dir = dir.Parent;
            return dir.FullName.EndDir();

            //var dir = Static.GetRegistry(@"software\InfoTask", "InfoTask2Path");
            //if (dir == "") dir = Static.GetRegistry(@"software\Wow6432Node\InfoTask", "InfoTask2Path");
            //return dir.EndDir();
        }

        //Имя организации-пользователя
        public static string UserOrg { get { return "УТЭ"; } }
        //Версия InfoTask
        public static string InfoTaskVersion { get { return "2.0.0"; } }
        //Дата версии InfoTask
        public static DateTime InfoTaskVersionDate { get { return new DateTime(2017, 6, 1); } }
        
        //Каталог шаблонов
        public static string TemplatesDir
        {
            get { return InfoTaskDir() + @"Templates\"; }
        }

        //Инициализация истории
        public static AccessHistory CreateHistory(Logger logger, //Логгер
                                                                        string historyFilePrefix) //Путь к файлу истории относительно каталога истории
        {
            return new AccessHistory(logger,
                    InfoTaskDir() + @"LocalData\History\" + historyFilePrefix + "History.accdb",
                    TemplatesDir + @"LocalData\History.accdb");
        }

        //Выбирает одну ошибку из двух
        public static MomErr Add(this MomErr err1, MomErr err2)
        {
            if (err1 == null) return err2;
            if (err2 == null) return err1;
            if (err2.Quality > err1.Quality) return err2;
            return err1;
        }

        //Перевод из строки в ProviderType
        public static ProviderType ToProviderType(this string t)
        {
            if (t == null) return ProviderType.Error;
            switch (t.ToLower())
            {
                case "источник":
                case "source":
                    return ProviderType.Source;
                case "ручнойввод":
                case "handinput":
                    return ProviderType.HandInput;
                case "архив":
                case "archive":
                    return ProviderType.Archive;
                case "приемник":
                case "receiver":
                    return ProviderType.Receiver;
            }
            return ProviderType.Error;
        }

        //Перевод из ProviderType в русское имя

        public static string ToRussian(this ProviderType t)
        {
            switch (t)
            {
                case ProviderType.Source:
                    return "Источник";
                case ProviderType.HandInput:
                    return "РучнойВвод";
                case ProviderType.Archive:
                    return "Архив";
                case ProviderType.Receiver:
                    return "Приемник";
            }
            return "Ошибка";
        }

        //Перевод из ProviderType в английское имя

        public static string ToEnglish(this ProviderType t)
        {
            switch (t)
            {
                case ProviderType.Source:
                    return "Source";
                case ProviderType.HandInput:
                    return "HandInput";
                case ProviderType.Archive:
                    return "Archive";
                case ProviderType.Receiver:
                    return "Receiver";
            }
            return "Error";
        }

        //Возвращает тип ошибки как строку
        public static string ToRussian(this ErrQuality quality)
        {
            switch (quality)
            {
                case ErrQuality.Error:
                    return "Ошибка";
                case ErrQuality.Warning:
                    return "Предупреждение";
            }
            return "";
        }
    }
}