﻿using System;
using BaseLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseLibraryTest
{
    //Тесты для RecDao
    [TestClass]
    public class RecDaoTest
    {
        //Открытие тестовых баз с копированием 
        private DaoDb CopyDb(string prefix)
        {
            return new DaoDb(TestLib.CopyFile("Libraries", "DbDao.accdb", "Rec" + prefix + ".accdb"));
        }
        //Путь к файлу
        private string File(string prefix)
        {
            return TestLib.TestRunDir + @"Libraries\Rec" + prefix + ".accdb";
        }

        [TestMethod]
        public void Rec()
        {
            var db = CopyDb("");
            var file = File("");
            var rec = new DaoRec(db, "Tabl");
            Assert.IsNotNull(rec.DaoDb);
            Assert.AreEqual(file, rec.DaoDb.File);
            Assert.AreEqual(7, rec.FieldsCount);
            Assert.IsTrue(rec.ContainsField("IntField"));
            Assert.IsTrue(rec.ContainsField("RealField"));
            Assert.IsTrue(rec.ContainsField("StringField1"));
            Assert.IsTrue(rec.ContainsField("id"));

            Assert.AreEqual(5, rec.RecordCount);
            Assert.IsTrue(rec.HasRows);
            Assert.IsFalse(rec.EOF);
            Assert.IsFalse(rec.BOF);
            Assert.IsFalse(rec.Read());
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

            rec = new DaoRec(db, "EmptyTabl");
            Assert.IsNotNull(rec.DaoDb);
            Assert.AreEqual(file, rec.DaoDb.File);
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

            rec = new DaoRec(db, "SELECT Tabl.IntField, Tabl.RealField, SubTabl.StringSubField FROM Tabl INNER JOIN SubTabl ON Tabl.Id = SubTabl.ParentId");
            Assert.IsNotNull(rec.DaoDb);
            Assert.AreEqual(file, rec.DaoDb.File);
            Assert.AreEqual(3, rec.FieldsCount);
            Assert.IsTrue(rec.ContainsField("IntField"));
            Assert.IsTrue(rec.ContainsField("RealField"));
            Assert.IsTrue(rec.ContainsField("StringSubField"));

            Assert.AreEqual(7, rec.RecordCount);
            Assert.IsTrue(rec.HasRows);
            Assert.IsFalse(rec.EOF);
            Assert.IsFalse(rec.BOF);
            Assert.IsFalse(rec.Read());
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
            using (var rec = new DaoRec(CopyDb("Read"), "Tabl"))
            {
                Assert.IsNotNull(rec.DaoDb);
                Assert.AreEqual(File("Read"), rec.DaoDb.File);
                Assert.AreEqual(7, rec.FiledsNum.Count);
                Assert.AreEqual(7, rec.Fileds.Count);
                Assert.AreEqual(7, rec.FieldsCount);
                
                Assert.IsTrue(rec.GetBool("BoolField"));
                Assert.AreEqual(true, rec.GetBoolNull("BoolField"));
                Assert.AreEqual(1, rec.GetInt("Id"));
                Assert.AreEqual(1, rec.GetIntNull("Id"));
                Assert.AreEqual(10, rec.GetInt("intField"));
                Assert.AreEqual(10, rec.GetIntNull("IntField"));
                Assert.AreEqual(1.5, rec.GetDouble("RealField"));
                Assert.AreEqual(1.5, rec.GetDoubleNull("REALFIELD"));
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

                Assert.IsFalse(rec.Read());
                Exception exception = null;
                try { Assert.IsTrue(rec.GetBool("BoolField")); }
                catch (Exception ex) { exception = ex; }
                Assert.IsNotNull(exception);

                Assert.IsNotNull(rec.Recordset);
                rec.Recordset.MoveFirst();
                Assert.AreEqual(1, (int)rec.Recordset.Fields["Id"].Value);
            }

            using (var rec = new DaoRec(File("Read"),
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

                Assert.IsTrue(rec.FindFirst("StringF LIKE '5555*'"));
                Assert.IsFalse(rec.NoMatch);
                Assert.AreEqual(50, rec.GetInt("IntF"));
                Assert.AreEqual("55555", rec.GetString("StringF"));
                Assert.IsTrue(rec.FindNext("StringF LIKE '5555*'"));
                Assert.IsFalse(rec.NoMatch);
                Assert.AreEqual(50, rec.GetInt("IntF"));
                Assert.AreEqual("555555", rec.GetString("StringF"));
                Assert.IsFalse(rec.FindNext("StringF LIKE '5555*'"));
                Assert.IsTrue(rec.NoMatch);

                Assert.IsTrue(rec.FindFirst("Id", 4));
                Assert.IsFalse(rec.NoMatch);
                Assert.AreEqual(40, rec.GetInt("IntF"));
                Assert.AreEqual(null, rec.GetString("StringF"));
                Assert.IsTrue(rec.MovePrevious());
                Assert.AreEqual(3.5, rec.GetDouble("RealField"));
                Assert.AreEqual("3333", rec.GetString("StringF"));
                Assert.IsTrue(rec.FindFirst("StringF", "22"));
                Assert.IsFalse(rec.NoMatch);
                Assert.AreEqual(20, rec.GetInt("IntF"));
                Assert.IsFalse(rec.FindFirst("StringF", "2222"));
                Assert.IsTrue(rec.NoMatch);
                Assert.IsTrue(rec.FindLast("StringF", "3333"));
                Assert.IsFalse(rec.NoMatch);
                Assert.AreEqual(3333, rec.GetInt("StringF"));
                Assert.IsTrue(rec.FindPrevious("Id", 1));
                Assert.AreEqual(10, rec.GetInt("IntF"));
                Assert.IsTrue(rec.FindLast("Id", 5));
                Assert.AreEqual(50, rec.GetInt("IntF"));
                Assert.IsTrue(rec.FindPrevious("StringF", "222"));
                Assert.AreEqual(20, rec.GetInt("IntF"));
                Assert.IsTrue(rec.FindNext("StringF", "55555"));
                Assert.AreEqual(50, rec.GetInt("IntF"));
                Assert.IsTrue(rec.FindNext("Id", 5));
                Assert.AreEqual(50, rec.GetInt("IntF"));
                Assert.IsFalse(rec.FindNext("StringF", "xxx"));
                Assert.IsFalse(rec.FindNext("IntF", 234));
            }
        }

        [TestMethod]
        public void RecWrite()
        {
            using (var rec = new DaoRec(CopyDb("Write"), "Tabl"))
            {
                rec.Read();
                rec.Put("IntField", 11);
                rec.Put("RealField", 1.6);
                rec.Put("StringField1", "Большой текст");
                rec.Put("StringField2", "TextTextTextTextTextText", true);
                rec.Put("BoolField", false);
                rec.Put("TimeField", new DateTime(2016, 07, 02));
                Assert.AreEqual(11, rec.GetInt("IntField"));
                Assert.AreEqual(1.6, rec.GetDouble("RealField"));
                Assert.AreEqual("Большой текст", rec.GetString("StringField1"));
                Assert.AreEqual("TextTextTe", rec.GetString("StringField2"));
                Assert.AreEqual(new DateTime(2016, 07, 02), rec.GetTime("TimeField"));
                Assert.IsFalse(rec.GetBool("BoolField"));
                Assert.AreEqual(1, rec.GetInt("Id"));
                rec.Update();
                rec.MoveFirst();
                Assert.AreEqual(11, rec.GetInt("IntField"));
                Assert.AreEqual(1.6, rec.GetDouble("RealField"));
                Assert.AreEqual("Большой текст", rec.GetString("StringField1"));
                Assert.AreEqual("TextTextTe", rec.GetString("StringField2"));
                Assert.AreEqual(new DateTime(2016, 07, 02), rec.GetTime("TimeField"));
                Assert.IsFalse(rec.GetBool("BoolField"));
                Assert.AreEqual(1, rec.GetInt("Id"));
                rec.Put(1, 11);
                rec.Put(2, 1.6);
                rec.Put(3, "Большой текст");
                rec.Put(4, "TextTextTextTextTextText", true);
                rec.Put(5, false);
                rec.Put(6, new DateTime(2016, 07, 02));
                Assert.AreEqual(11, rec.GetInt("IntField"));
                Assert.AreEqual(1.6, rec.GetDouble("RealField"));
                Assert.AreEqual("Большой текст", rec.GetString("StringField1"));
                Assert.AreEqual("TextTextTe", rec.GetString("StringField2"));
                Assert.AreEqual(new DateTime(2016, 07, 02), rec.GetTime("TimeField"));
                Assert.IsFalse(rec.GetBool("BoolField"));
                Assert.AreEqual(1, rec.GetInt("Id"));

                rec.Read();
                rec.Put("IntField", (int?)null);
                rec.Put("RealField", (double?)null);
                rec.Put("StringField1", (string)null);
                rec.Put("StringField2", "Text", true);
                rec.Put("BoolField", (bool?)false);
                rec.Put("TimeField", (DateTime?)null);
                Assert.AreEqual(null, rec.GetIntNull("IntField"));
                Assert.AreEqual(0, rec.GetDouble("RealField"));
                Assert.AreEqual("", rec.GetString("StringField1", ""));
                Assert.AreEqual("Text", rec.GetString("StringField2"));
                Assert.AreEqual(false, rec.GetBoolNull("BoolField"));
                Assert.AreEqual(2, rec.GetInt("Id"));
                rec.Read();
                rec.MovePrevious();
                Assert.AreEqual(0, rec.GetInt("IntField"));
                Assert.AreEqual(null, rec.GetDoubleNull("RealField"));
                Assert.AreEqual(null, rec.GetString("StringField1"));
                Assert.AreEqual("Text", rec.GetString("StringField2"));
                Assert.AreEqual(null, rec.GetTimeNull("TimeField"));
                Assert.AreEqual(false, rec.GetBool("BoolField"));
                Assert.AreEqual(2, rec.GetIntNull("Id"));
                rec.Put(1, (int?)null);
                rec.Put(2, (double?)null);
                rec.Put(3, (string)null);
                rec.Put(4, "Text", true);
                rec.Put(5, (bool?)false);
                rec.Put(6, (DateTime?)null);
                Assert.AreEqual(null, rec.GetIntNull("IntField"));
                Assert.AreEqual(0, rec.GetDouble("RealField"));
                Assert.AreEqual("", rec.GetString("StringField1", ""));
                Assert.AreEqual("Text", rec.GetString("StringField2"));
                Assert.AreEqual(false, rec.GetBoolNull("BoolField"));
                Assert.AreEqual(2, rec.GetInt("Id"));

                Assert.AreEqual(5, rec.RecordCount);
                Assert.IsTrue(rec.MoveLast());
                rec.Delete();
                Assert.AreEqual(4, rec.RecordCount);
                Assert.IsTrue(rec.MoveLast());
                Assert.AreEqual(40, rec.GetInt("IntField"));
                Assert.AreEqual(4.5, rec.GetDouble("RealField"));
                Assert.AreEqual("Большой текст 4", rec.GetString("StringField1"));
                Assert.IsTrue(rec.MoveLast());
                Assert.IsFalse(rec.MoveNext());
                Assert.IsTrue(rec.EOF);
                rec.AddNew();
                rec.Put("IntField", 60);
                rec.Put("RealField", 6.6);
                rec.Put("StringField1", "Большой текст 6");
                rec.Put("StringField2", "Text6", true);
                rec.Put("BoolField", true);
                rec.Put("TimeField", new DateTime(2016, 07, 06));
                Assert.AreEqual(60, rec.GetInt("IntField"));
                Assert.AreEqual(6.6, rec.GetDouble("RealField"));
                Assert.AreEqual("Большой текст 6", rec.GetString("StringField1"));
                Assert.AreEqual("Text6", rec.GetString("StringField2"));
                Assert.AreEqual(new DateTime(2016, 07, 06), rec.GetTime("TimeField"));
                Assert.IsTrue(rec.GetBool("BoolField"));
                Assert.AreEqual(6, rec.GetInt("Id"));
                Assert.IsTrue(rec.MoveFirst());
                Assert.IsTrue(rec.MoveLast());
                Assert.AreEqual(60, rec.GetInt("IntField"));
                Assert.AreEqual(6.6, rec.GetDouble("RealField"));
                Assert.AreEqual("Большой текст 6", rec.GetString("StringField1"));
                Assert.AreEqual("Text6", rec.GetString("StringField2"));
                Assert.AreEqual(new DateTime(2016, 07, 06), rec.GetTime("TimeField"));
                Assert.IsTrue(rec.GetBool("BoolField"));
                Assert.AreEqual(6, rec.GetInt("Id"));
                Assert.AreEqual(5, rec.RecordCount);
                Assert.IsTrue(rec.HasRows);
            }
        }
    }
}