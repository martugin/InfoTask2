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
        //Файлы используемых баз данных
        private readonly string _file = TestLib.TestRunDir + @"BaseLibrary\DbDao.accdb";
        private readonly string _fileWrong = TestLib.TestRunDir + @"BaseLibrary\DbDao2.accdb";
        private readonly string _fileCopy = TestLib.TestRunDir + @"BaseLibrary\DbDaoCopy.accdb";
        private readonly string _fileTmp = TestLib.TestRunDir + @"BaseLibrary\TmpDbDao.accdb";
        //Открытие тестовых баз с копированием 
        private DaoDb CopyDb()
        {
            return new DaoDb(TestLib.CopyFile(@"BaseLibrary\DbDao.accdb"));
        }

        [TestMethod]
        public void DaoDbTest()
        {
            var db = CopyDb();
            db.ConnectDao();
            Assert.AreEqual(_file, db.File);
            Assert.IsNull(db.Connection);
            Assert.IsNotNull(db.Database);
            Assert.AreEqual(_file, db.Database.Name);
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
            db.SetColumnLong("SubTabl", "IntSubField");
            Assert.IsTrue(db.ColumnExists("SubTabl", "IntSubField"));
            db.SetColumnDouble("SubTabl", "RealField", IndexModes.WithoutChange, 0);
            Assert.IsTrue(db.ColumnExists("SubTabl", "RealField"));
            db.SetColumnString("SubTabl", "StringSubField", 30, IndexModes.EmptyIndex, "aaa", false, false);
            Assert.IsTrue(db.ColumnExists("SubTabl", "StringSubField"));
            db.SetColumnMemo("SubTabl", "MemoField", "bbb");
            Assert.IsTrue(db.ColumnExists("SubTabl", "MemoField"));
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
            var db = CopyDb();
            db.Dispose();
            Assert.IsNull(db.Database);
            Assert.IsNull(db.Connection);
            Assert.IsTrue(DaoDb.Check(_file, "DaoTest"));
            Assert.IsTrue(DaoDb.Check(_file, "DaoTest", new[] { "Tabl", "SubTabl", "EmptyTabl", "SysTabl", "SysSubTabl" }));
            Assert.IsTrue(DaoDb.Check(_file, new[] { "Tabl", "SubTabl", "EmptyTabl" }));
            Assert.IsFalse(DaoDb.Check(_fileWrong, "Fignia"));
            Assert.IsFalse(DaoDb.Check(null, "Fignia"));
            Assert.IsFalse(DaoDb.Check(_fileWrong, "Fignia"));
            Assert.IsFalse(DaoDb.Check(_fileWrong, new[] { "Tabl" }));
            Assert.IsFalse(DaoDb.Check(_file, new[] { "Tabl", "SubTabl", "EmptyTabl1" }));

            DaoDb.Compress(_file, 10000000);
            DaoDb.Compress(_file, 10000);
            Assert.IsTrue(new FileInfo(_fileTmp).Exists);

            Assert.IsTrue(DaoDb.FromTemplate(_file,_fileCopy, ReplaceByTemplate.Always));
            Assert.IsFalse(DaoDb.FromTemplate(_file, _fileCopy, ReplaceByTemplate.IfNotExists));
            Assert.IsFalse(DaoDb.FromTemplate(_file, _fileCopy, ReplaceByTemplate.IfNewVersion));
            new FileInfo(_fileCopy).Delete();
            Assert.IsTrue(DaoDb.FromTemplate(_file, _fileCopy, ReplaceByTemplate.IfNotExists));
            new FileInfo(_fileCopy).Delete();
            Assert.IsTrue(DaoDb.FromTemplate(_file, _fileCopy, ReplaceByTemplate.IfNewVersion));
            Assert.IsTrue(new FileInfo(_fileCopy).Exists);

            DaoDb.Execute(_file, "DELETE * FROM Tabl");
            DaoDb.ExecuteAdo(_file, "DELETE * FROM SysTabl");
            using (var rec = new ReaderAdo(_file, "SELECT * FROM Tabl"))
                Assert.IsFalse(rec.HasRows);
            using (var rec = new ReaderAdo(_file, "SELECT * FROM SubTabl"))
                Assert.IsFalse(rec.HasRows);
            using (var rec = new RecDao(_file, "SysTabl"))
                Assert.IsFalse(rec.HasRows);
            using (var rec = new RecDao(_file, "SysSubTabl"))
                Assert.IsFalse(rec.HasRows);
        }
    }
}