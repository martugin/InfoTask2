using System;
using System.Data;
using System.IO;
using System.Threading;
using BaseLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseLibraryTest
{
    //Класс запускающий тестовые файлы баз данных
    //Перед использованием файлы копируются из Tests в TestsRun
    [TestClass]
    public class DaoTest
    {
        //Открытие тестовых баз с копированием 
        private DaoDb CopyDb(string prefix)
        {
            return new DaoDb(TestLib.CopyFile("BaseLibrary", "DbDao.accdb", "Dao" + prefix + ".accdb"));
        }
        //Путь к файлу
        private string File(string prefix)
        {
            return TestLib.TestRunDir + @"BaseLibrary\Dao" + prefix + ".accdb";
        }

        [TestMethod]
        public void DaoDbTest()
        {
            try { new DaoDb("hhh").ConnectDao(); }
            catch (Exception ex) { Assert.AreEqual("Файл базы данных не найден", ex.Message); }
            try { new DaoDb("hhh").ConnectAdo(); }
            catch (Exception ex) { Assert.AreEqual("Файл базы данных не найден", ex.Message); }

            var db = CopyDb("Common");
            string file = File("Common");
            db.ConnectDao();
            Assert.AreEqual(file, db.File);
            Assert.IsNull(db.Connection);
            Assert.IsNotNull(db.Database);
            Assert.AreEqual(file, db.Database.Name);
            db.ConnectAdo();
            Assert.IsNotNull(db.Connection);
            Assert.IsNotNull(db.Database);
            Assert.AreEqual(ConnectionState.Open, db.Connection.State);

            db.Execute("DELETE * FROM Tabl");
            Thread.Sleep(800);
            db.ExecuteAdo("DELETE * FROM SysTabl");
            using (var rec = new ReaderAdo(db, "SELECT * FROM Tabl"))
                Assert.IsFalse(rec.HasRows);
            using (var rec = new ReaderAdo(db, "SELECT * FROM SubTabl"))
                Assert.IsFalse(rec.HasRows);
            using (var rec = new RecDao(db, "SysTabl"))
                Assert.IsFalse(rec.HasRows);
            using (var rec = new RecDao(db, "SysSubTabl"))
                Assert.IsFalse(rec.HasRows);

            Assert.IsTrue(db.TableExists("SubTabl"));
            Assert.IsFalse(db.TableExists("SubTabl2"));
            Assert.IsTrue(db.ColumnExists("Tabl", "IntField"));
            Assert.IsFalse(db.ColumnExists("SubTabl", "RealField"));
            Assert.IsTrue(db.ColumnExists("SubTabl", "StringSubField"));

            db.SetColumnBool("SubTabl", "BoolField");
            Assert.IsTrue(db.ColumnExists("SubTabl", "BoolField"));
            db.SetColumnBool("SubTabl", "BoolField");
            Assert.IsTrue(db.ColumnExists("SubTabl", "BoolField"));

            db.SetColumnLong("SubTabl", "IntSubField");
            Assert.IsTrue(db.ColumnExists("SubTabl", "IntSubField"));
            db.SetColumnLong("SubTabl", "IntSubField");
            Assert.IsTrue(db.ColumnExists("SubTabl", "IntSubField"));

            db.SetColumnDouble("SubTabl", "RealField", IndexModes.WithoutChange, 0);
            Assert.IsTrue(db.ColumnExists("SubTabl", "RealField"));
            db.SetColumnDouble("SubTabl", "RealField", IndexModes.WithoutChange, 0);
            Assert.IsTrue(db.ColumnExists("SubTabl", "RealField"));

            db.SetColumnString("SubTabl", "StringSubField", 30, IndexModes.EmptyIndex, "aaa", false, false);
            Assert.IsTrue(db.ColumnExists("SubTabl", "StringSubField"));
            db.SetColumnString("SubTabl", "StringSubField", 50, IndexModes.EmptyIndex, "aaa", false, false);
            Assert.IsTrue(db.ColumnExists("SubTabl", "StringSubField"));

            db.SetColumnMemo("SubTabl", "MemoField", "bbb");
            Assert.IsTrue(db.ColumnExists("SubTabl", "MemoField"));
            db.SetColumnMemo("SubTabl", "MemoField", "bbb");
            Assert.IsTrue(db.ColumnExists("SubTabl", "MemoField"));

            db.SetColumnDateTime("SubTabl", "TimeField");
            Assert.IsTrue(db.ColumnExists("SubTabl", "TimeField"));
            db.SetColumnDateTime("SubTabl", "TimeField");
            Assert.IsTrue(db.ColumnExists("SubTabl", "TimeField"));

            db.DeleteColumn("SubTabl", "TimeField");
            db.DeleteColumn("SubTabl", "IntSubField");
            db.Dispose();
            Assert.IsFalse(db.ColumnExists("SubTabl", "TimeField"));
            Assert.IsFalse(db.ColumnExists("SubTabl", "IntSubField"));

            db.RenameTable("EmptyTabl", "SmallTabl");
            Assert.IsTrue(db.TableExists("SmallTabl"));
            db.AddTable("BigTabl", "SmallTabl");
            db.Dispose();
            Assert.IsTrue(db.TableExists("BigTabl"));
            db.DeleteTable("SmallTabl");
            db.DeleteTable("BigTabl");
            Assert.IsFalse(db.TableExists("BigTabl"));
            Assert.IsFalse(db.TableExists("SmallTabl"));

            db.SetColumnIndex("Tabl", "StringField1", IndexModes.UniqueIndex);
            db.SetColumnIndex("Tabl", "StringField2", IndexModes.CommonIndex);
            db.SetColumnIndex("Tabl", "IntField", "RealField", false, IndexModes.UniqueIndex);

            db.Dispose();
        }

        [TestMethod]
        public void DaoDbStatic()
        {
            var db = CopyDb("Static");
            string file = File("Static");
            string fileWrong = File("Static2");
            string fileCopy = File("Copy");
            string fileTmp = TestLib.TestRunDir + @"BaseLibrary\TmpDaoStatic.accdb";
            db.Dispose();
            Assert.IsNull(db.Database);
            Assert.IsNull(db.Connection);
            Assert.IsTrue(DaoDb.Check(file, "DaoTest"));
            Assert.IsTrue(DaoDb.Check(file, "DaoTest", new[] { "Tabl", "SubTabl", "EmptyTabl", "SysTabl", "SysSubTabl" }));
            Assert.IsTrue(DaoDb.Check(file, new[] { "Tabl", "SubTabl", "EmptyTabl" }));
            Assert.IsFalse(DaoDb.Check(fileWrong, "Fignia"));
            Assert.IsFalse(DaoDb.Check(null, "Fignia"));
            Assert.IsFalse(DaoDb.Check(fileWrong, "Fignia"));
            Assert.IsFalse(DaoDb.Check(fileWrong, new[] { "Tabl" }));
            Assert.IsFalse(DaoDb.Check(file, new[] { "Tabl", "SubTabl", "EmptyTabl1" }));

            DaoDb.Compress(file, 10000000);
            DaoDb.Compress(file, 10000);
            Assert.IsTrue(new FileInfo(fileTmp).Exists);

            Assert.IsTrue(DaoDb.FromTemplate(file, fileCopy, ReplaceByTemplate.Always));
            Assert.IsFalse(DaoDb.FromTemplate(file, fileCopy, ReplaceByTemplate.IfNotExists));
            Assert.IsFalse(DaoDb.FromTemplate(file, fileCopy, ReplaceByTemplate.IfNewVersion));
            new FileInfo(fileCopy).Delete();
            Assert.IsTrue(DaoDb.FromTemplate(file, fileCopy, ReplaceByTemplate.IfNotExists));
            new FileInfo(fileCopy).Delete();
            Assert.IsTrue(DaoDb.FromTemplate(file, fileCopy, ReplaceByTemplate.IfNewVersion));
            Assert.IsTrue(new FileInfo(fileCopy).Exists);

            DaoDb.Execute(file, "DELETE * FROM Tabl");
            DaoDb.ExecuteAdo(file, "DELETE * FROM SysTabl");
            using (var rec = new ReaderAdo(file, "SELECT * FROM Tabl"))
                Assert.IsFalse(rec.HasRows);
            using (var rec = new ReaderAdo(file, "SELECT * FROM SubTabl"))
                Assert.IsFalse(rec.HasRows);
            using (var rec = new RecDao(file, "SysTabl"))
                Assert.IsFalse(rec.HasRows);
            using (var rec = new RecDao(file, "SysSubTabl"))
                Assert.IsFalse(rec.HasRows);
        }
    }
}