using BaseLibrary;
using BaseLibraryTest;
using ComClients;
using CommonTypes;
using Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneratorTest
{
    [TestClass]
    public class GenTest
    {
        private void Generate(string tablName, string subTablName = null, bool copyRes = true)
        {
            var tabls = new TablsList();
            using (var db = new DaoDb(TestLib.CopyFile("Generator", "GenData.accdb", "Gen" + tablName + "Data.accdb")))
            {
                tabls.AddStruct(db, "Tabl", "SubTabl", "SubSubTabl");
                tabls.AddDbStructs(db);
                tabls.LoadValues(db, true);
            }

            TestLib.CopyFile("Generator", "GenTemplate.accdb", "Gen" + tablName + "Template.accdb");
            if (copyRes) TestLib.CopyFile("Generator", "GenRes.accdb", "Gen" + tablName + "Res.accdb");
            TestLib.CopyFile("Generator", "CorrectGen" + tablName + ".accdb", "Gen" + tablName + "Correct.accdb");

            var templatesFile = TestLib.TestRunDir + @"Generator\Gen" + tablName + "Template.accdb";
            var table = new GenTemplateTable(tablName, "GenRule", "ErrMess", "CalcOn", "Id");
            var subTable = subTablName == null ? null : new GenTemplateTable(subTablName, table, "GenRule", "ErrMess", "CalcOn", "Id", "ParentId");
            var generator = new ModuleGenerator(new Logger(), tabls, templatesFile, table, subTable);

            var s = TestLib.TestRunDir + @"Generator\Gen" + tablName;
            if (copyRes) generator.Generate(s + "Res.accdb", tablName, subTablName);

            using (var db1 = new DaoDb(s + (copyRes ? "Res" : "Template") + ".accdb"))
                using (var db2 = new DaoDb(s + "Correct" + ".accdb"))
                {
                    TestLib.CompareTables(db1, db2, tablName);
                    if (subTablName != null)
                        TestLib.CompareTables(db1, db2, subTablName);
                }
        }

        [TestMethod]
        public void GenOneLevel()
        {
            Generate("OneLevel");
        }

        [TestMethod]
        public void GenTwoLevel()
        {
            Generate("TwoLevel", "TwoLevelSub");
        }

        [TestMethod]
        public void GenErrLevel()
        {
            Generate("ErrLevel", "ErrLevelSub", false);
        }

        [TestMethod]
        public void GenCalcParams()
        {
            TestLib.CopyDir("Generator", "Module");
            
            var client = new ItClient();
            string dir = TestLib.TestRunDir + @"Generator\Module\";
            client.GenerateParams(dir);
            
            using (var db1 = new DaoDb(dir + "Compiled.accdb"))
                using (var db2 = new DaoDb(dir + "CorrectCompiled.accdb"))
                {
                    TestLib.CompareTables(db1, db2, "GeneratedParams", "ParamId");
                    TestLib.CompareTables(db1, db2, "GeneratedSubParams", "SubParamId");
                }
        }
    }
}