using BaseLibrary;
using BaseLibraryTest;
using ComLaunchers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneratorTest
{
    [TestClass]
    public class LargeGenTest
    {
        [TestMethod]
        public void GenModuleLarge()
        {
            var launcher = new ItLauncher();
            launcher.Initialize("Test", "GenerationLarge");
            Generate(launcher, "ModuleLarge");
            Generate(launcher, "ApdControl");
        }

        private void Generate(ItLauncher launcher, string dirName)
        {
            TestLib.CopyDir("Generator", dirName);
            var dir = TestLib.TestRunDir + @"Generator\" + dirName + @"\";
            launcher.GenerateParams(dir);
            using (var db1 = new DaoDb(dir + "Compiled.accdb"))
                using (var db2 = new DaoDb(dir + "CorrectCompiled.accdb"))
                {
                    TestLib.CompareTables(db1, db2, "GeneratedParams", "ParamId");
                    TestLib.CompareTables(db1, db2, "GeneratedSubParams", "SubParamId");
                }
        }
    }
}