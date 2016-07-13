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
        //Каталог запуска тестовых баз
        private readonly string _dir = TestLib.TestRunDir + @"BaseLibrary\";
        //Открытие тестовых баз с копированием и без
        private DaoDb CopyDb(string fileName) //Имя файла
        {
            return new DaoDb(TestLib.CopyFile(@"BaseLibrary\" + fileName));
        }

        [TestMethod]
        public void DaoDbTest()
        {
            var db = CopyDb("DbDao.accdb");
            db.ConnectDao();
            Assert.AreEqual(_dir + "DbDao.accdb", db.File);
            Assert.IsNull(db.Connection);
            Assert.IsNotNull(db.Database);
            Assert.AreEqual(_dir + "DbDao.accdb", db.Database.Name);
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
            var db = CopyDb("DbDao.accdb");
            db.Dispose();
            Assert.IsNull(db.Database);
            Assert.IsNull(db.Connection);
            Assert.IsTrue(DaoDb.Check(_dir + "DbDao.accdb", "DaoTest"));
            Assert.IsTrue(DaoDb.Check(_dir + "DbDao.accdb", "DaoTest", new[] { "Tabl", "SubTabl", "EmptyTabl", "SysTabl", "SysSubTabl" }));
            Assert.IsTrue(DaoDb.Check(_dir + "DbDao.accdb", new[] { "Tabl", "SubTabl", "EmptyTabl" }));
            Assert.IsFalse(DaoDb.Check(_dir + "DbDao.accdb", "Fignia"));
            Assert.IsFalse(DaoDb.Check(null, "Fignia"));
            Assert.IsFalse(DaoDb.Check(_dir + "Dao.accdb", "Fignia"));
            Assert.IsFalse(DaoDb.Check(_dir + "Dao.accdb", new[] { "Tabl" }));
            Assert.IsFalse(DaoDb.Check(_dir + "DbDao.accdb", new[] { "Tabl", "SubTabl", "EmptyTabl1" }));

            DaoDb.Compress(_dir + "DbDao.accdb", 10000000);
            DaoDb.Compress(_dir + "DbDao.accdb", 10000);
            Assert.IsTrue(new FileInfo(_dir + "TmpDbDao.accdb").Exists);

            Assert.IsTrue(DaoDb.FromTemplate(_dir + "DbDao.accdb", _dir + "DbDaoCopy.accdb", ReplaceByTemplate.Always));
            Assert.IsFalse(DaoDb.FromTemplate(_dir + "DbDao.accdb", _dir + "DbDaoCopy.accdb", ReplaceByTemplate.IfNotExists));
            Assert.IsFalse(DaoDb.FromTemplate(_dir + "DbDao.accdb", _dir + "DbDaoCopy.accdb", ReplaceByTemplate.IfNewVersion));
            new FileInfo(_dir + "DbDaoCopy.accdb").Delete();
            Assert.IsTrue(DaoDb.FromTemplate(_dir + "DbDao.accdb", _dir + "DbDaoCopy.accdb", ReplaceByTemplate.IfNotExists));
            new FileInfo(_dir + "DbDaoCopy.accdb").Delete();
            Assert.IsTrue(DaoDb.FromTemplate(_dir + "DbDao.accdb", _dir + "DbDaoCopy.accdb", ReplaceByTemplate.IfNewVersion));
            Assert.IsTrue(new FileInfo(_dir + "DbDaoCopy.accdb").Exists);

            DaoDb.Execute(_dir + "DbDao.accdb", "DELETE * FROM Tabl");
            DaoDb.ExecuteAdo(_dir + "DbDao.accdb", "DELETE * FROM SysTabl");
            using (var rec = new ReaderAdo(_dir + "DbDao.accdb", "SELECT * FROM Tabl"))
                Assert.IsFalse(rec.HasRows);
            using (var rec = new ReaderAdo(_dir + "DbDao.accdb", "SELECT * FROM SubTabl"))
                Assert.IsFalse(rec.HasRows);
            using (var rec = new RecDao(_dir + "DbDao.accdb", "SysTabl"))
                Assert.IsFalse(rec.HasRows);
            using (var rec = new RecDao(_dir + "DbDao.accdb", "SysSubTabl"))
                Assert.IsFalse(rec.HasRows);
        }
    }
}