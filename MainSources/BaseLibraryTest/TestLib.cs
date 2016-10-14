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
        //Путь к каталогу TestRun
        public static string TestRunDir
        {
            get { return InfoTaskDevelopDir + @"TestsRun\"; }
        }
        
        //Копирует файл из Tests в TestsRun, возвращает полный путь к итоговому файлу
        //Копируется файл Tests + dir + file в TestsRun + dir + newFile
        public static string CopyFile(string dir, //каталог
                                                   string file, //относительный путь к исходному файлу
                                                   string newFile = null) //относительный путь к итоговому файлу
        {
            var f = new FileInfo(InfoTaskDevelopDir + @"Tests\" + dir + "\\" + file);
            string s = TestRunDir + dir + "\\" + (newFile ?? file);
            var d = new FileInfo(s).Directory;
            if (!d.Exists) d.Create();
            f.CopyTo(s, true);
            return s;
        }
        //Копирует каталог из Tests в TestsRun, возвращает полный путь к итоговому каталогу
        //Копируется файл Tests + parentDir + dir в TestsRun + parentDir + newDir
        public static string CopyDir(string parentDir, //родительский каталог
                                                   string dir, //относительный путь к исходному каталогу
                                                   string newDir = null) //относительный путь к итоговому каталогу
        {
            var f = new DirectoryInfo(InfoTaskDevelopDir + @"Tests\" + parentDir + "\\" + dir);
            string s = TestRunDir + parentDir + "\\" + (newDir ?? dir);
            var d = new DirectoryInfo(s).Parent;
            if (!d.Exists) d.Create();
            Different.CopyDir(f.FullName, s);
            return s;
        }

    }
}