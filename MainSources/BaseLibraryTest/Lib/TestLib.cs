using System;
using System.IO;
using BaseLibrary;
using CommonTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                return new DirectoryInfo(ItStatic.InfoTaskDir()).Parent.FullName.EndDir();
                //var dir = Static.GetRegistry(@"software\InfoTask", "InfoTask2Path");
                //if (dir == "") dir = Static.GetRegistry(@"software\Wow6432Node\InfoTask", "InfoTask2Path").EndDir();
                //var n = dir.LastIndexOf(@"\", dir.Length - 2, StringComparison.Ordinal);
                //return dir.Substring(0, n + 1);    
            }
        }
        //Путь к каталогу TestRun
        public static string TestRunDir
        {
            get { return InfoTaskDevelopDir + @"Debug\TestsRun\"; }
        }

        //Создание тестового логгера
        public static Logger CreateTestLogger(LoggerStability stability = LoggerStability.Single)
        {
            return new Logger(new TestHistory(), new TestIndicator(), stability);
        }

        //Строка настроек провайдера для соединения с тестовым SQL Server
        public static string TestSqlInf(string dbName)
        {
            using (var sys = new SysTabl(TestRunDir + "TestsSettings.accdb"))
                return SqlParam(sys, "SqlServer") + ";" + SqlParam(sys, "IdentType") + ";" + SqlParam(sys, "Login") + ";" + SqlParam(sys, "Password") + ";DataBase=" + dbName;  
         
        }
        private static string SqlParam(SysTabl sys, string name)
        {
            return name + "=" + sys.SubValue("SQLServerSettings", name);
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
            Static.CopyDir(f.FullName, s);
            return s;
        }

        //Сравнение двух таблиц на полное совпадение, 
        public static bool CompareTables(DaoDb db1, DaoDb db2, //Базы данных для сравнения
                                                          string tableName, //Имяы таблицы
                                                          string idField, string idField2 = null, string idField3 = null, //Ключевые поля для сравнения
                                                          string tableName2 = null,  //Имя таблицы во второй базе, если отличается
                                                          params string[] exeptionFields) //Поля, исключаемые из сравнения
        {
            var exFields = new SetS();
            foreach (var f in exeptionFields)
                exFields.Add(f);
            using (var rec1 = new DaoRec(db1, "SELECT * FROM " + tableName + " ORDER BY " + idField + (idField2 == null ? "" : ", " + idField2) + (idField3 == null ? "" : ", " + idField3)))
                using (var rec2 = new DaoRec(db2, "SELECT * FROM " + (tableName2 ?? tableName) + " ORDER BY " + idField + (idField2 == null ? "" : ", " + idField2) + (idField3 == null ? "" : ", " + idField3)))
                {
                    rec1.Read();
                    while (rec2.Read())
                    {
                        Assert.AreEqual(rec1.EOF, rec2.EOF);
                        foreach (var k in rec1.Fileds.Keys)
                            if (!exFields.Contains(k))
                                Assert.AreEqual(rec1.Recordset.Fields[k].Value, rec2.Recordset.Fields[k].Value);
                        rec1.Read();
                    }
                    Assert.AreEqual(rec1.EOF, rec2.EOF);
                }
            return true;
        }
        public static bool CompareTables(string file1, string file2, string tableName, string idField, string idField2 = null, string idField3 = null, string tableName2 = null, params string[] exeptionFields)
        {
            return CompareTables(new DaoDb(file1), new DaoDb(file2), tableName, idField, idField2, idField3, tableName2, exeptionFields);
        }

        //Сравнение файлов конкретных типов 
        #region CompareFiles
        //Сравнение файлов истории
        public static void CompareHistories(string file1, string file2)
        {
            using (var db1 = new DaoDb(file1))
                using (var db2 = new DaoDb(file2))
                {
                    CompareTables(db1, db2, "SuperHistory", "SuperHistoryId", null, null, null, "Time", "ProcessLength");
                    CompareTables(db1, db2, "History", "HistoryId", null, null, null, "Time", "ProcessLength");
                    CompareTables(db1, db2, "SubHistory", "Id", null, null, null, "Time", "FromStart");
                    CompareTables(db1, db2, "ErrorsList", "Id", null, null, null, "Time");
                }
        }

        //Сравнение клонов
        public static void CompareClones(string file1, string file2)
        {
            using (var db1 = new DaoDb(file1))
                using (var db2 = new DaoDb(file2))
                {
                    CompareTables(db1, db2, "Signals", "SignalId");
                    CompareTables(db1, db2, "ErrorsObjects", "OutContext");
                    CompareTables(db1, db2, "MomentErrors", "ErrNum");
                    CompareTables(db1, db2, "MomentValues", "SignalId", "Time");
                    CompareTables(db1, db2, "MomentStrValues", "SignalId", "Time");
                    CompareTables(db1, db2, "MomentValuesCut", "SignalId", "Time");
                    CompareTables(db1, db2, "MomentStrValuesCut", "SignalId", "Time");
                }
        }

        //Сравнение фалов со сгенерированными параметрами
        public static void CompareGeneratedParams(string file1, string file2)
        {
            using (var db1 = new DaoDb(file1))
                using (var db2 = new DaoDb(file2))
                {
                    CompareTables(db1, db2, "GeneratedParams", "ParamId");
                    CompareTables(db1, db2, "GeneratedSubParams", "SubParamId");
                }
        }
        #endregion
    }
}