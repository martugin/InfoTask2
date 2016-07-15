using System;
using BaseLibrary;
using BaseLibraryTest;
using CommonTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProvidersLibrary;

namespace ProvidersLibraryTest
{
    [TestClass]
    public class CloneSourceTest
    {
        private void CopyFile(string fileName)
        {
            TestLib.CopyFile(@"ProvidersLibrary\TestClone\" + fileName);
        }
        private readonly string _dir = TestLib.TestRunDir + @"ProvidersLibrary\TestClone\";
        
        private CloneSource MakeCloneSource()
        {
            CopyFile("Clone.accdb");
            return (CloneSource)new ProvidersFactory()
                .CreateProvider(new Logger(), "CloneSource", "TestSource", "CloneDir=" + _dir );
        }

        [TestMethod]
        public void CloneProps()
        {
            var source = MakeCloneSource();
            Assert.IsNotNull(source);
            Assert.AreEqual(ProviderType.Source, source.Type);
            Assert.AreEqual("TestSource", source.Name);
            Assert.AreEqual("CloneSource", source.Code);
            Assert.AreEqual("Clones", source.Complect);
            Assert.AreEqual("Источник: TestSource, CloneSource", source.Context);
            Assert.IsNotNull(source.Logger);
            Assert.AreEqual(0, source.CalcSignals.Count);
            Assert.AreEqual(0, source.Signals.Count);
            
            Assert.IsNotNull(source.Settings);
            Assert.AreEqual(_dir + "Clone.accdb", source.Settings.CloneFile);
            Assert.AreEqual("Db=" + _dir + "Clone.accdb", source.Settings.Hash);
            Assert.AreEqual("Источник: TestSource, CloneSource", source.Settings.Context);
            Assert.AreEqual(source, source.Settings.Provider);
            Assert.IsTrue(source.Settings.Connect());
            Assert.IsTrue(source.Settings.CheckConnection());
            Assert.AreEqual("Успешное соединение", source.Settings.CheckConnectionMessage);
         
            Assert.AreEqual(new DateTime(2016, 7, 8), source.GetTime().Begin);
            Assert.AreEqual(new DateTime(2016, 7, 8, 0, 30, 0), source.GetTime().End);
        }

        [TestMethod]
        public void FullRead()
        {
            var source = MakeCloneSource();
            var sig = source.AddSignal("Ob.SigB", "Ob", DataType.Boolean, "");
            Assert.AreEqual(DataType.Boolean, sig.DataType);
            Assert.AreEqual(source, sig.Source);
            Assert.AreEqual(1, source.Signals.Count);
            Assert.IsTrue(source.Signals.ContainsKey("Ob.SigB"));
            sig = source.AddSignal("Ob.SigI", "Ob", DataType.Integer, "");
            Assert.AreEqual(DataType.Integer, sig.DataType);
            Assert.AreEqual(source, sig.Source);
            Assert.AreEqual(2, source.Signals.Count);
            Assert.IsTrue(source.Signals.ContainsKey("Ob.SigI"));
            sig = source.AddSignal("Ob.SigR", "Ob", DataType.Real, "");
            Assert.AreEqual(DataType.Real, sig.DataType);
            Assert.AreEqual(source, sig.Source);
            Assert.AreEqual(3, source.Signals.Count);
            Assert.IsTrue(source.Signals.ContainsKey("Ob.SigR"));
            sig = source.AddSignal("Ob.SigS", "Ob", DataType.String, "");
            Assert.AreEqual(DataType.String, sig.DataType);
            Assert.AreEqual(source, sig.Source);
            Assert.AreEqual(4, source.Signals.Count);
            Assert.IsTrue(source.Signals.ContainsKey("Ob.SigS"));
            sig = source.AddSignal("Ob.SigT", "Ob", DataType.Time, "");
            Assert.AreEqual(DataType.Time, sig.DataType);
            Assert.AreEqual(source, sig.Source);
            Assert.AreEqual(5, source.Signals.Count);
            Assert.IsTrue(source.Signals.ContainsKey("Ob.SigT"));
            source.AddSignal("ObConst.SigB", "ObConst", DataType.Boolean, "");
            source.AddSignal("ObConst.SigI", "ObConst", DataType.Integer, "");
            source.AddSignal("ObConst.SigR", "ObConst", DataType.Real, "");
            source.AddSignal("ObConst.SigS", "ObConst", DataType.String, "");
            Assert.AreEqual(9, source.Signals.Count);
            Assert.IsTrue(source.Signals.ContainsKey("ObConst.SigB"));
            Assert.IsTrue(source.Signals.ContainsKey("ObConst.SigI"));
            Assert.IsTrue(source.Signals.ContainsKey("ObConst.SigR"));
            Assert.IsTrue(source.Signals.ContainsKey("ObConst.SigS"));

            source.Prepare();


            source.GetValues(new DateTime(2016, 7, 8), new DateTime(2016, 7, 8, 1, 0, 0));
        }
    }
}