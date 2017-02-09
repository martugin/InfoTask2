using BaseLibrary;
using BaseLibraryTest;
using ComClients;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneratorTest
{
    [TestClass]
    public class LargeGenLest
    {
        [TestMethod]
        public void GenModuleLarge()
        {
            var client = new ItClient();
            client.Initialize("Test", "GenerationLarge");
            Generate(client, "ModuleLarge");
            Generate(client, "ApdControl");
        }

        private void Generate(ItClient client, string dirName)
        {
            TestLib.CopyDir("Generator", dirName);
            var dir = TestLib.TestRunDir + @"Generator\" + dirName + @"\";
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