using System;
using System.Data;
using System.IO;
using System.Threading;
using BaseLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Office.Interop.Access.Dao;

namespace BaseLibraryTest
{
    //Класс запускающий тестовые файлы баз данных
    //Перед использованием файлы копируются из Tests в TestsRun
    [TestClass]
    public class DaoTest
    {
        //Каталог запуска тестовых баз
        private readonly string _dir = TestLib.InfoTaskDevelopDir + @"TestsRun\BaseLibrary\Databases\";
        //Открытие тестовых баз с копированием и без
        private DaoDb CopyDb(string fileName) //Имя файла
        {
            return TestLib.RunCopyDb(@"BaseLibrary\Databases\" + fileName);
        }
        private DaoDb RunDb(string fileName) //Имя файла
        {
            return TestLib.RunDb(@"BaseLibrary\Databases\" + fileName);
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

        [TestMethod]
        public void Rec()
        {
            var db = CopyDb("DbDao.accdb");
            var rec = new RecDao(db, "Tabl");
            Assert.IsNotNull(rec.DaoDb);
            Assert.AreEqual(_dir + "DbDao.accdb", rec.DaoDb.File);
            Assert.AreEqual(7, rec.FieldsCount);
            Assert.IsTrue(rec.ContainsField("IntField"));
            Assert.IsTrue(rec.ContainsField("RealField"));
            Assert.IsTrue(rec.ContainsField("StringField1"));
            Assert.IsTrue(rec.ContainsField("Id"));

            Assert.AreEqual(5, rec.RecordCount);
            Assert.IsTrue(rec.HasRows);
            Assert.IsFalse(rec.EOF);
            Assert.IsFalse(rec.BOF);
            Assert.IsTrue(rec.Read());
            Assert.IsTrue(rec.MoveFirst());
            Assert.IsTrue(rec.MoveNext());
            Assert.IsTrue(rec.MoveFirst());
            Assert.IsFalse(rec.MovePrevious());
            Assert.IsTrue(rec.BOF);
            Assert.IsFalse(rec.EOF);
            Assert.IsTrue(rec.MoveLast());
            Assert.IsTrue(rec.MovePrevious());
            Assert.IsTrue(rec.MoveLast());
            Assert.IsFalse(rec.MoveNext());
            Assert.IsTrue(rec.EOF);
            Assert.IsFalse(rec.BOF);
            Assert.IsFalse(rec.NoMatch);
            rec.Dispose();

            rec = new RecDao(db, "EmptyTabl");
            Assert.IsNotNull(rec.DaoDb);
            Assert.AreEqual(_dir + "DbDao.accdb", rec.DaoDb.File);
            Assert.AreEqual(1, rec.FieldsCount);
            Assert.IsTrue(rec.ContainsField("StringField"));

            Assert.AreEqual(0, rec.RecordCount);
            Assert.IsFalse(rec.HasRows);
            Assert.IsTrue(rec.EOF);
            Assert.IsTrue(rec.BOF);
            Assert.IsFalse(rec.Read());
            Assert.IsFalse(rec.MoveFirst());
            Assert.IsFalse(rec.MoveNext());
            Assert.IsFalse(rec.MoveLast());
            Assert.IsFalse(rec.MovePrevious());
            Assert.IsTrue(rec.EOF);
            Assert.IsTrue(rec.EOF);
            Assert.IsFalse(rec.NoMatch);
            rec.Dispose();

            rec = new RecDao(db, "SELECT Tabl.IntField, Tabl.RealField, SubTabl.StringSubField FROM Tabl INNER JOIN SubTabl ON Tabl.Id = SubTabl.ParentId");
            Assert.IsNotNull(rec.DaoDb);
            Assert.AreEqual(_dir + "DbDao.accdb", rec.DaoDb.File);
            Assert.AreEqual(3, rec.FieldsCount);
            Assert.IsTrue(rec.ContainsField("IntField"));
            Assert.IsTrue(rec.ContainsField("RealField"));
            Assert.IsTrue(rec.ContainsField("StringSubField"));
            
            Assert.AreEqual(7, rec.RecordCount);
            Assert.IsTrue(rec.HasRows);
            Assert.IsFalse(rec.EOF);
            Assert.IsFalse(rec.BOF);
            Assert.IsTrue(rec.Read());
            Assert.IsTrue(rec.MoveFirst());
            Assert.IsTrue(rec.MoveNext());
            Assert.IsTrue(rec.MoveFirst());
            Assert.IsFalse(rec.MovePrevious());
            Assert.IsTrue(rec.BOF);
            Assert.IsFalse(rec.EOF);
            Assert.IsTrue(rec.MoveLast());
            Assert.IsTrue(rec.MovePrevious());
            Assert.IsTrue(rec.MoveLast());
            Assert.IsFalse(rec.MoveNext());
            Assert.IsTrue(rec.EOF);
            Assert.IsFalse(rec.BOF);
            Assert.IsFalse(rec.NoMatch);
            rec.Dispose();

            db.Dispose();
        }

        [TestMethod]
        public void RecRead()
        {
            using (var rec = new RecDao(CopyDb("DbDao.accdb"), "Tabl"))
            {
                Assert.IsTrue(rec.GetBool("BoolField"));
                Assert.AreEqual(true, rec.GetBoolNull("BoolField"));    
                Assert.AreEqual(1, rec.GetInt("Id"));
                Assert.AreEqual(1, rec.GetIntNull("Id"));
                Assert.AreEqual(10, rec.GetInt("IntField"));
                Assert.AreEqual(10, rec.GetIntNull("IntField"));
                Assert.AreEqual(1.5, rec.GetDouble("RealField"));
                Assert.AreEqual(1.5, rec.GetDoubleNull("RealField"));
                Assert.AreEqual("Большой текст 1", rec.GetString("StringField1"));
                Assert.AreEqual("Text", rec.GetString("StringField2"));
                Assert.AreEqual(new DateTime(2016, 7, 1), rec.GetTime("TimeField"));
                Assert.AreEqual(new DateTime(2016, 7, 1), rec.GetTimeNull("TimeField"));

                Assert.IsTrue(rec.Read());
                Assert.IsTrue(rec.Read());
                Assert.IsFalse(rec.GetBool("BoolField"));
                Assert.AreEqual(false, rec.GetBoolNull("BoolField"));
                Assert.AreEqual(2, rec.GetInt("Id"));
                Assert.AreEqual(2, rec.GetIntNull("Id"));
                Assert.AreEqual(20, rec.GetInt("IntField"));
                Assert.AreEqual(20, rec.GetIntNull("IntField"));
                Assert.AreEqual(0, rec.GetDouble("RealField"));
                Assert.AreEqual(null, rec.GetDoubleNull("RealField"));
                Assert.AreEqual("Большой текст 2", rec.GetString("StringField1"));
                Assert.AreEqual("null", rec.GetString("StringField2", "null"));
                Assert.AreEqual(new DateTime(2016, 7, 2, 2, 3, 4), rec.GetTime("TimeField"));
                Assert.AreEqual(new DateTime(2016, 7, 2, 2, 3, 4), rec.GetTimeNull("TimeField"));
            }
        }
    }
}