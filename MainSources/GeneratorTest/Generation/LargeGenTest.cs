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
            launcher.Initialize("Test");
            launcher.LoadProjectByCode("GenerationLarge");
            Generate(launcher, "ModuleLarge");
            Generate(launcher, "ApdControl");
        }

        private void Generate(ItLauncher launcher, string dirName)
        {
            TestLib.CopyDir("Generator", dirName);
            var dir = TestLib.TestRunDir + @"Generator\" + dirName + @"\";
            launcher.GenerateParams(dir);
            TestLib.CompareGeneratedParams(dir + "Compiled.accdb", dir + "CorrectCompiled.accdb");
        }
    }
}