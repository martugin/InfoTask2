﻿using System;
using BaseLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseLibraryTest
{
    //Тесты для AdoReader
    [TestClass]
    public class ReaderAdoTest
    {
        //Открытие тестовых баз с копированием 
        private DaoDb CopyDb(string prefix)
        {
            return new DaoDb(TestLib.CopyFile("Libraries", "DbDao.accdb", "Reader" + prefix + ".accdb"));
        }
        //Путь к файлу
        private string File(string prefix)
        {
            return TestLib.TestRunDir + @"Libraries\Reader" + prefix + ".accdb";
        }

        [TestMethod]
        public void RecAccess()
        {
            var db = CopyDb("");
            var file = File("");
            var rec = new AdoReader(db, "SELECT * FROM Tabl");
            Assert.IsNotNull(rec.DaoDb);
            Assert.IsNotNull(rec.Reader);
            Assert.AreEqual(DatabaseType.Access, rec.DatabaseType);
            Assert.AreEqual(file, rec.DaoDb.File);
            Assert.IsTrue(rec.HasRows);
            Assert.AreEqual(5, rec.RecordCount("SELECT Count(*) FROM Tabl"));
            Assert.IsFalse(rec.EOF);
            Assert.IsTrue(rec.Read());
            Assert.IsFalse(rec.EOF);
            Assert.IsTrue(rec.Read());
            Assert.IsFalse(rec.EOF);
            Assert.IsTrue(rec.Read());
            Assert.IsTrue(rec.Read());
            Assert.IsTrue(rec.Read());
            Assert.IsFalse(rec.Read());
            Assert.IsTrue(rec.EOF);
            rec.Dispose();

            rec = new AdoReader(db, "SELECT * FROM EmptyTabl");
            Assert.AreEqual(DatabaseType.Access, rec.DatabaseType);
            Assert.AreEqual(file, rec.DaoDb.File);
            Assert.IsFalse(rec.HasRows);
            Assert.AreEqual(0, rec.RecordCount("SELECT Count(*) FROM EmptyTabl"));
            Assert.IsTrue(rec.EOF);
            Assert.IsFalse(rec.Read());
            Assert.IsTrue(rec.EOF);
            rec.Dispose();

            rec = new AdoReader(db, "SELECT Tabl.IntField, Tabl.RealField, SubTabl.StringSubField FROM Tabl INNER JOIN SubTabl ON Tabl.Id = SubTabl.ParentId");
            Assert.IsNotNull(rec.DaoDb);
            Assert.IsNotNull(rec.Reader);
            Assert.AreEqual(file, rec.DaoDb.File);
            Assert.AreEqual(DatabaseType.Access, rec.DatabaseType);
            Assert.IsTrue(rec.HasRows);
            Assert.IsFalse(rec.EOF);
            Assert.IsTrue(rec.Read());
            Assert.IsFalse(rec.EOF);
            Assert.IsTrue(rec.Read());
            Assert.IsFalse(rec.EOF);
            Assert.IsTrue(rec.Read());
            Assert.IsTrue(rec.Read());
            Assert.IsTrue(rec.Read());
            Assert.IsTrue(rec.Read());
            Assert.IsTrue(rec.Read());
            Assert.IsFalse(rec.Read());
            Assert.IsTrue(rec.EOF);
            rec.Dispose();
        }

        [TestMethod]
        public void RecAccessRead()
        {
            using (var rec = new AdoReader(CopyDb("Read"), "SELECT * FROM Tabl"))
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

                Assert.AreEqual(1, rec.GetInt(0));
                Assert.AreEqual(1, rec.GetIntNull(0));
                Assert.AreEqual(10, rec.GetInt(1));
                Assert.AreEqual(10, rec.GetIntNull(1));
                Assert.AreEqual(1.5, rec.GetDouble(2));
                Assert.AreEqual(1.5, rec.GetDoubleNull(2));
                Assert.AreEqual("Большой текст 1", rec.GetString(3));
                Assert.AreEqual("Text", rec.GetString(4));
                Assert.AreEqual(true, rec.GetBool(5));
                Assert.AreEqual(new DateTime(2016, 7, 1), rec.GetTime(6));
                Assert.AreEqual(new DateTime(2016, 7, 1), rec.GetTimeNull(6));

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

                Assert.AreEqual(2, rec.GetInt(0));
                Assert.AreEqual(2, rec.GetIntNull(0));
                Assert.AreEqual(20, rec.GetInt(1));
                Assert.AreEqual(20, rec.GetIntNull(1));
                Assert.AreEqual(0, rec.GetDouble(2));
                Assert.AreEqual(null, rec.GetDoubleNull(2));
                Assert.AreEqual("Большой текст 2", rec.GetString(3));
                Assert.AreEqual("null", rec.GetString(4, "null"));
                Assert.IsFalse(rec.GetBool(5));
                Assert.AreEqual(false, rec.GetBoolNull(5));
                Assert.AreEqual(new DateTime(2016, 7, 2, 2, 3, 4), rec.GetTime(6));
                Assert.AreEqual(new DateTime(2016, 7, 2, 2, 3, 4), rec.GetTimeNull(6));

                Assert.IsTrue(rec.Read());
                Assert.IsTrue(rec.GetBool("BoolField"));
                Assert.AreEqual(true, rec.GetBoolNull("BoolField"));
                Assert.AreEqual(3, rec.GetInt("Id"));
                Assert.AreEqual(3, rec.GetIntNull("Id"));
                Assert.AreEqual(0, rec.GetInt("IntField"));
                Assert.AreEqual(null, rec.GetIntNull("IntField"));
                Assert.IsTrue(rec.IsNull("IntField"));
                Assert.AreEqual(3.5, rec.GetDouble("RealField", 300));
                Assert.AreEqual(3.5, rec.GetDoubleNull("RealField"));
                Assert.IsFalse(rec.IsNull("RealField"));
                Assert.AreEqual("Большой текст 3", rec.GetString("StringField1"));
                Assert.AreEqual("Text3", rec.GetString("StringField2", "null"));
                Assert.AreEqual(Static.MinDate, rec.GetTime("TimeField"));
                Assert.AreEqual(null, rec.GetTimeNull("TimeField"));

                Assert.AreEqual(3, rec.GetInt(0));
                Assert.AreEqual(3, rec.GetIntNull(0));
                Assert.AreEqual(0, rec.GetInt(1));
                Assert.AreEqual(null, rec.GetIntNull(1));
                Assert.IsTrue(rec.IsNull(1));
                Assert.AreEqual(3.5, rec.GetDouble(2, 300));
                Assert.AreEqual(3.5, rec.GetDoubleNull(2));
                Assert.IsFalse(rec.IsNull(2));
                Assert.AreEqual("Большой текст 3", rec.GetString(3));
                Assert.AreEqual("Text3", rec.GetString(4, "null"));
                Assert.IsTrue(rec.GetBool(5));
                Assert.AreEqual(true, rec.GetBoolNull(5));
                Assert.AreEqual(Static.MinDate, rec.GetTime(6));
                Assert.AreEqual(null, rec.GetTimeNull(6));

                Assert.IsTrue(rec.Read());
                Assert.IsTrue(rec.Read());
                Assert.IsTrue(rec.GetBool("BoolField"));
                Assert.AreEqual(true, rec.GetBoolNull("BoolField"));
                Assert.AreEqual(5, rec.GetInt("Id"));
                Assert.AreEqual(5, rec.GetIntNull("Id"));
                Assert.AreEqual(50, rec.GetInt("IntField", 400));
                Assert.AreEqual(50, rec.GetIntNull("IntField"));
                Assert.AreEqual(5.5, rec.GetDouble("RealField"));
                Assert.AreEqual(5.5, rec.GetDoubleNull("RealField"));
                Assert.AreEqual("Большой текст 5", rec.GetString("StringField1"));
                Assert.AreEqual(null, rec.GetString("StringField2"));
                Assert.AreEqual(new DateTime(2016, 7, 5), rec.GetTime("TimeField"));
                Assert.AreEqual(new DateTime(2016, 7, 5), rec.GetTimeNull("TimeField"));

                Assert.IsNotNull(rec.Reader);
                Assert.AreEqual(5, (int)rec.Reader["Id"]);
                Assert.AreEqual(5.5, (double)rec.Reader["RealField"]);
                Assert.AreEqual("Большой текст 5", (string)rec.Reader["StringField1"]);

                Assert.IsFalse(rec.Read());
                Exception exception = null;
                try { Assert.IsTrue(rec.GetBool("BoolField")); }
                catch (Exception ex) { exception = ex; }
                Assert.IsNotNull(exception);
            }

            using (var rec = new AdoReader(new DaoDb(File("Read")), 
                "SELECT Tabl.Id, Tabl.IntField AS IntF, Tabl.RealField, SubTabl.StringSubField AS StringF " +
                "FROM Tabl LEFT JOIN SubTabl ON Tabl.Id = SubTabl.ParentId ORDER BY Tabl.Id, SubTabl.StringSubField;"))
            {
                Assert.IsTrue(rec.Read());
                Assert.AreEqual(1, rec.GetInt("Id"));
                Assert.AreEqual(10, rec.GetInt("IntF"));
                Assert.AreEqual(2, rec.GetInt("RealField"));
                Assert.AreEqual(1.5, rec.GetDouble("RealField"));
                Assert.AreEqual("1", rec.GetString("StringF"));
                Assert.AreEqual(1, rec.GetInt(0));
                Assert.AreEqual(10, rec.GetInt(1));
                Assert.AreEqual(1.5, rec.GetDouble(2));
                Assert.AreEqual("1", rec.GetString(3));
                Assert.AreEqual(1, rec.GetIntNull(0));
                Assert.AreEqual(10, rec.GetIntNull(1));
                Assert.AreEqual(1.5, rec.GetDoubleNull(2));

                Assert.IsTrue(rec.Read());
                Assert.AreEqual(2, rec.GetInt("Id"));
                Assert.AreEqual(20, rec.GetInt("IntF"));
                Assert.AreEqual(0, rec.GetInt("RealField"));
                Assert.AreEqual(0, rec.GetDouble("RealField"));
                Assert.AreEqual("22", rec.GetString("StringF"));
                Assert.IsTrue(rec.Read());
                Assert.AreEqual(2, rec.GetInt("Id"));
                Assert.AreEqual("2", rec.GetString("Id"));
                Assert.AreEqual(20, rec.GetInt("IntF"));
                Assert.AreEqual(0, rec.GetInt("RealField"));
                Assert.AreEqual(null, rec.GetIntNull("RealField"));
                Assert.AreEqual(0, rec.GetDouble("RealField"));
                Assert.AreEqual(null, rec.GetDoubleNull("RealField"));
                Assert.AreEqual("222", rec.GetString("StringF"));
                Assert.IsTrue(rec.Read());
                Assert.AreEqual(3, rec.GetInt("Id"));
                Assert.AreEqual(0, rec.GetInt("IntF"));
                Assert.AreEqual(4, rec.GetInt("RealField"));
                Assert.AreEqual(3.5, rec.GetDouble("RealField"));
                Assert.AreEqual("333", rec.GetString("StringF"));
                Assert.IsFalse(rec.EOF);

                Assert.IsTrue(rec.Read());
                Assert.IsTrue(rec.Read());
                Assert.AreEqual(40, rec.GetInt("IntF"));
                Assert.AreEqual(null, rec.GetString("StringF"));
            }
        }
    }
}