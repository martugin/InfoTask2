using BaseLibrary;
using BaseLibraryTest;
using Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneratorTest
{
    [TestClass]
    public class LargeGenTest
    {
        [TestMethod]
        public void GenModuleLarge()
        {
            var gen = new TablGenerator(new Logger(new TestHistory(), new TestIndicator()));
            Generate(gen, "ModuleLarge");
            Generate(gen, "ApdControl");
        }

        private void Generate(TablGenerator gen, string dirName)
        {
            TestLib.CopyDir("Generator", dirName);
            var dir = TestLib.TestRunDir + @"Generator\" + dirName + @"\";
            gen.GenerateParams(dir);
            TestLib.CompareGeneratedParams(dir + "Compiled.accdb", dir + "CorrectCompiled.accdb");
        }
    }
}