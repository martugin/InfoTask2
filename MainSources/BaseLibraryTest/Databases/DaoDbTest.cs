using System.IO;
using BaseLibrary;

namespace BaseLibraryTest
{
    //Класс запускающий тестовые файлы баз данных
    //Перед использованием файлы копируются из Tests в TestsRun
    public static class DaoDbTest
    {
        //Чтение из реестра пути к каталогу InfoTask, в возвращаемом пути \ на конце
        private static string InfoTaskDevelopDir()
        {
            var dir = Different.GetRegistry(@"software\InfoTask", "InfoTaskPath");
            if (dir == "") dir = Different.GetRegistry(@"software\Wow6432Node\InfoTask", "InfoTaskPath");
            if (!dir.EndsWith(@"\")) dir += @"\";
            var n = dir.LastIndexOf(@"\", dir.Length - 2);
            return dir.Substring(0, n + 1);
        }

        //Открывает DaoDb предварительно копируя файл из Tests в TestsRun
        //file - относительный путь к файлу
        public static DaoDb RunCopyDb(string file)
        {
            var itd = InfoTaskDevelopDir();
            var f = new FileInfo(itd + @"Test\" + file);
            string s = itd + @"TestRun\" + file;
            f.CopyTo(s, true);
            return new DaoDb(s);
        }

        //Открывает DaoDb из TestsRun
        //file - относительный путь к файлу
        public static DaoDb RunDb(string file)
        {
            return new DaoDb(InfoTaskDevelopDir() + @"TestRun\" + file);
        }
    }
}