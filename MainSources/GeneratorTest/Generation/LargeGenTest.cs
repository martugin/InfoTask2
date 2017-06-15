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
            var logger = new Logger(new TestIndicator());
            logger.History = new TestHistory(logger);
            var gen = new TablGenerator(logger);
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