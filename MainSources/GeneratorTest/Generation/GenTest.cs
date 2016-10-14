using BaseLibrary;
using BaseLibraryTest;
using CommonTypes;
using Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneratorTest
{
    [TestClass]
    public class GenTest
    {
        private void Generate(string tablName, string subTablName = null)
        {
            var tabls = new TablsList();
            using (var db = new DaoDb(TestLib.CopyFile("Generator", "GenData.accdb", "Gen" + tablName + "Data.accdb")))
            {
                tabls.AddStruct(db, "Tabl", "SubTabl", "SubSubTabl");
                tabls.AddDbStructs(db);
                tabls.LoadValues(db);
            }

            TestLib.CopyFile("Generator", "GenTemplate.accdb", "Gen" + tablName + "Template.accdb");
            TestLib.CopyFile("Generator", "GenRes.accdb", "Gen" + tablName + "Res.accdb");

            var templatesFile = TestLib.TestRunDir + @"Generator\Gen" + tablName + "Template.accdb";
            var generator = subTablName != null
                                ? new TablGenerator(new Logger(), tabls, templatesFile, tablName, "Id", "GenRule", "ErrMess", subTablName, "ParentId", "GenRule", "ErrMess")
                                : new TablGenerator(new Logger(), tabls, templatesFile, tablName, "Id", "GenRule", "ErrMess");
            generator.Generate(TestLib.TestRunDir + @"Generator\Gen" + tablName + "Res.accdb", tablName, subTablName);
        }

        [TestMethod]
        public void GenSimple()
        {
            Generate("OneLevel");

        }
    }
}