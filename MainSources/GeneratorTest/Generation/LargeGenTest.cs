using BaseLibraryTest;
using ComLaunchers;
using InfoTaskLauncherTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneratorTest
{
    [TestClass]
    public class LargeGenTest
    {
        [TestMethod]
        public void GenModuleLarge()
        {
            var launcher = new TestItLauncher();
            launcher.TestInitialize("Test");
            var pr = launcher.LoadProjectByCode("GenerationLarge");
            Generate(pr, "ModuleLarge");
            Generate(pr, "ApdControl");
        }

        private void Generate(LauncherProject pr, string dirName)
        {
            TestLib.CopyDir("Generator", dirName);
            var dir = TestLib.TestRunDir + @"Generator\" + dirName + @"\";
            pr.GenerateParams(dir);
            TestLib.CompareGeneratedParams(dir + "Compiled.accdb", dir + "CorrectCompiled.accdb");
        }
    }
}