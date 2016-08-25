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
        public static string CopyFile(string file)//относительный путь к файлу
        {
            var f = new FileInfo(InfoTaskDevelopDir + @"Tests\" + file);
            string s = TestRunDir + file;
            var d = new FileInfo(s).Directory;
            if (!d.Exists) d.Create();
            f.CopyTo(s, true);
            return s;
        }
    }
}