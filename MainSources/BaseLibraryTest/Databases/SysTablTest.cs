using System;
using BaseLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseLibraryTest
{
    //Тесты для ЫныЕфид
    [TestClass]
    public class SysTablTest
    {
        //Открытие тестовых баз с копированием
        private string CopyFile() 
        {
            return TestLib.CopyFile(@"BaseLibrary\DbDao.accdb");
        }

        [TestMethod]
        public void InstanceFuns()
        {
            using (var sys = new SysTabl(CopyFile(), false))
            {
                Assert.IsNotNull(sys);
                Assert.AreEqual("OptionsValue", sys.Value("FileOptions"));
                Assert.AreEqual("ParamValue", sys.Value("Param"));
                Assert.AreEqual("OptionsTag", sys.Tag("FileOptions"));
                Assert.AreEqual("ParamTag", sys.Tag("Param"));
                sys.PutValue("Param", "s1", "t1");
                Assert.AreEqual("s1", sys.Value("Param"));
                Assert.AreEqual("t1", sys.Tag("Param"));
                sys.PutValue("Param", 22, "t2");
                Assert.AreEqual("22", sys.Value("Param"));
                Assert.AreEqual("t2", sys.Tag("Param"));
                sys.PutValue("Param", 33.3, "t3");
                Assert.AreEqual("33,3", sys.Value("Param"));
                Assert.AreEqual("t3", sys.Tag("Param"));
                sys.PutValue("Param", new DateTime(2000, 1, 1));
                Assert.AreEqual("01.01.2000 0:00:00", sys.Value("Param"));
                Assert.AreEqual("t3", sys.Tag("Param"));
                sys.PutValue("Param", true, "");
                Assert.AreEqual("True", sys.Value("Param"));
                Assert.AreEqual("", sys.Tag("Param"));
                sys.PutTag("Param", "Tag", "Value");
                Assert.AreEqual("Value", sys.Value("Param"));
                Assert.AreEqual("Tag", sys.Tag("Param"));
            }

            var db = new DaoDb(CopyFile());
            using (var sys = new SysTabl(db))
            {
                Assert.IsNotNull(sys);
                Assert.AreEqual("OptionsValue", sys.Value("FileOptions"));
                Assert.AreEqual("ParamValue", sys.Value("Param"));
                Assert.AreEqual("OptionsTag", sys.Tag("FileOptions"));
                Assert.AreEqual("ParamTag", sys.Tag("Param"));

                Assert.AreEqual("Тестовый файл", sys.SubValue("FileOptions", "FileDescription"));
                Assert.AreEqual("DaoTest", sys.SubValue("FileOptions", "FileType"));
                Assert.AreEqual("2.0.0", sys.SubValue("FileOptions", "FileVersion"));
                Assert.AreEqual("11.07.2016", sys.SubValue("FileOptions", "FileVersionDate"));
                Assert.AreEqual("SubValue", sys.SubValue("Param", "SubParam"));
                Assert.AreEqual(null, sys.SubTag("FileOptions", "FileDescription"));
                Assert.AreEqual("SubTag", sys.SubTag("Param", "SubParam"));
                sys.PutSubValue("Param", "SubParam", "s1", "t1");
                Assert.AreEqual("s1", sys.SubValue("Param", "SubParam"));
                Assert.AreEqual("t1", sys.SubTag("Param", "SubParam"));
                sys.PutSubValue("Param", "SubParam", 22, "t2");
                Assert.AreEqual("22", sys.SubValue("Param", "SubParam"));
                Assert.AreEqual("t2", sys.SubTag("Param", "SubParam"));
                sys.PutSubValue("Param", "SubParam", 33.3, "t3");
                Assert.AreEqual("33,3", sys.SubValue("Param", "SubParam"));
                Assert.AreEqual("t3", sys.SubTag("Param", "SubParam"));
                sys.PutSubValue("Param", "SubParam", new DateTime(2000, 1, 1));
                Assert.AreEqual("01.01.2000 0:00:00", sys.SubValue("Param", "SubParam"));
                Assert.AreEqual("t3", sys.SubTag("Param", "SubParam"));
                sys.PutSubValue("Param", "SubParam", true, "");
                Assert.AreEqual("True", sys.SubValue("Param", "SubParam"));
                Assert.AreEqual("", sys.SubTag("Param", "SubParam"));
                sys.PutSubTag("Param", "SubParam", "Tag", "Value");
                Assert.AreEqual("Value", sys.SubValue("Param", "SubParam"));
                Assert.AreEqual("Tag", sys.SubTag("Param", "SubParam"));
            }
            db.Dispose();
        }

        [TestMethod]
        public void StaticFuns()
        {
            var file = CopyFile();
            Assert.AreEqual("OptionsValue", SysTabl.ValueS(file, "FileOptions"));
            Assert.AreEqual("ParamValue", SysTabl.ValueS(file, "Param"));
            Assert.AreEqual("OptionsTag", SysTabl.TagS(file, "FileOptions"));
            Assert.AreEqual("ParamTag", SysTabl.TagS(file, "Param"));
            SysTabl.PutValueS(file, "Param", "s1", "t1");
            Assert.AreEqual("s1", SysTabl.ValueS(file, "Param"));
            Assert.AreEqual("t1", SysTabl.TagS(file, "Param"));
            SysTabl.PutValueS(file, "Param", 22, "t2");
            Assert.AreEqual("22", SysTabl.ValueS(file, "Param"));
            Assert.AreEqual("t2", SysTabl.TagS(file, "Param"));
            SysTabl.PutValueS(file, "Param", 33.3, "t3");
            Assert.AreEqual("33,3", SysTabl.ValueS(file, "Param"));
            Assert.AreEqual("t3", SysTabl.TagS(file, "Param"));
            SysTabl.PutValueS(file, "Param", new DateTime(2000, 1, 1));
            Assert.AreEqual("01.01.2000 0:00:00", SysTabl.ValueS(file, "Param"));
            Assert.AreEqual("t3", SysTabl.TagS(file, "Param"));
            SysTabl.PutValueS(file, "Param", true, "");
            Assert.AreEqual("True", SysTabl.ValueS(file, "Param"));
            Assert.AreEqual("", SysTabl.TagS(file, "Param"));
            SysTabl.PutTagS(file, "Param", "Tag", "Value");
            Assert.AreEqual("Value", SysTabl.ValueS(file, "Param"));
            Assert.AreEqual("Tag", SysTabl.TagS(file, "Param"));

            Assert.AreEqual("Тестовый файл", SysTabl.SubValueS(file, "FileOptions", "FileDescription"));
            Assert.AreEqual("DaoTest", SysTabl.SubValueS(file, "FileOptions", "FileType"));
            Assert.AreEqual("2.0.0", SysTabl.SubValueS(file, "FileOptions", "FileVersion"));
            Assert.AreEqual("11.07.2016", SysTabl.SubValueS(file, "FileOptions", "FileVersionDate"));
            Assert.AreEqual("SubValue", SysTabl.SubValueS(file, "Param", "SubParam"));
            Assert.AreEqual(null, SysTabl.SubTagS(file, "FileOptions", "FileDescription"));
            Assert.AreEqual("SubTag", SysTabl.SubTagS(file, "Param", "SubParam"));
            SysTabl.PutSubValueS(file, "Param", "SubParam", "s1", "t1");
            Assert.AreEqual("s1", SysTabl.SubValueS(file, "Param", "SubParam"));
            Assert.AreEqual("t1", SysTabl.SubTagS(file, "Param", "SubParam"));
            SysTabl.PutSubValueS(file, "Param", "SubParam", 22, "t2");
            Assert.AreEqual("22", SysTabl.SubValueS(file, "Param", "SubParam"));
            Assert.AreEqual("t2", SysTabl.SubTagS(file, "Param", "SubParam"));
            SysTabl.PutSubValueS(file, "Param", "SubParam", 33.3, "t3");
            Assert.AreEqual("33,3", SysTabl.SubValueS(file, "Param", "SubParam"));
            Assert.AreEqual("t3", SysTabl.SubTagS(file, "Param", "SubParam"));
            SysTabl.PutSubValueS(file, "Param", "SubParam", new DateTime(2000, 1, 1));
            Assert.AreEqual("01.01.2000 0:00:00", SysTabl.SubValueS(file, "Param", "SubParam"));
            Assert.AreEqual("t3", SysTabl.SubTagS(file, "Param", "SubParam"));
            SysTabl.PutSubValueS(file, "Param", "SubParam", true, "");
            Assert.AreEqual("True", SysTabl.SubValueS(file, "Param", "SubParam"));
            Assert.AreEqual("", SysTabl.SubTagS(file, "Param", "SubParam"));
            SysTabl.PutSubTagS(file, "Param", "SubParam", "Tag", "Value");
            Assert.AreEqual("Value", SysTabl.SubValueS(file, "Param", "SubParam"));
            Assert.AreEqual("Tag", SysTabl.SubTagS(file, "Param", "SubParam"));
        }
    }
}