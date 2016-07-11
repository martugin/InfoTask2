using System;
using System.IO;
using BaseLibrary;

namespace BaseLibraryTest
{
    //Общие функции для тестов
    public static class TestLib
    {
        //Чтение из реестра пути к каталогу InfoTask разработчика (без Debug), в возвращаемом пути \ на конце
        public static string InfoTaskDevelopDir
        {
            get
            {
                var dir = Different.GetRegistry(@"software\InfoTask", "InfoTask2Path");
                if (dir == "") dir = Different.GetRegistry(@"software\Wow6432Node\InfoTask", "InfoTask2Path");
                if (!dir.EndsWith(@"\")) dir += @"\";
                var n = dir.LastIndexOf(@"\", dir.Length - 2, StringComparison.Ordinal);
                return dir.Substring(0, n + 1);    
            }
        }

        //Открывает DaoDb предварительно копируя файл из Tests в TestsRun
        //file - относительный путь к файлу
        public static DaoDb RunCopyDb(string file)
        {
            var itd = InfoTaskDevelopDir;
            var f = new FileInfo(itd + @"Tests\" + file);
            string s = itd + @"TestsRun\" + file;
            f.CopyTo(s, true);
            return new DaoDb(s);
        }

        //Открывает DaoDb из TestsRun
        //file - относительный путь к файлу
        public static DaoDb RunDb(string file)
        {
            return new DaoDb(InfoTaskDevelopDir + @"TestsRun\" + file);
        }
    }
}