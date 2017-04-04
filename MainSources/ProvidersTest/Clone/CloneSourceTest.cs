using System;
using BaseLibrary;
using BaseLibraryTest;
using CommonTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProvidersLibrary;

namespace ProvidersTest
{
    [TestClass]
    public class CloneSourceTest
    {
        private SourceConnect MakeCloneConnect(string prefix)
        {
            TestLib.CopyDir(@"Providers\Fictive", "TestClone", "Clone" + prefix);
            var factory = new ProvidersFactory();
            var connect = factory.CreateConnect(ProviderType.Source, "TestSource", "Clones", new Logger());
            connect.JoinProvider(factory.CreateProvider("CloneSource", "CloneDir=" + TestLib.TestRunDir + @"Providers\Fictive\Clone" + prefix));
            return (SourceConnect)connect;
        }

        private static void GetValues(SourceConnect connect, DateTime beg, DateTime en)
        {
            connect.Logger.StartPeriod(beg, en);
            connect.GetValues();
            connect.Logger.FinishPeriod();
        }

        [TestMethod]
        public void CloneProps()
        {
            var connect = MakeCloneConnect("Props");
            var source = (CloneSource)connect.Provider;
            Assert.IsNotNull(connect);
            Assert.AreEqual(ProviderType.Source, connect.Type);
            Assert.AreEqual("TestSource", connect.Name);
            Assert.AreEqual("CloneSource", connect.Provider.Code);
            Assert.AreEqual("Clones", connect.Complect);
            Assert.AreEqual("Источник: TestSource", connect.Context);
            Assert.IsNotNull(connect.Logger);
            Assert.AreEqual(0, connect.CalcSignals.Count);
            Assert.AreEqual(0, connect.Signals.Count);

            Assert.AreEqual(TestLib.TestRunDir + @"Providers\Fictive\CloneProps\Clone.accdb", source.CloneFile);
            Assert.AreEqual("Источник: TestSource", connect.Context);
            Assert.AreEqual(source, connect.Provider);
            Assert.IsTrue(source.Connect());
         
            Assert.AreEqual(new DateTime(2016, 7, 8), connect.GetTime().Begin);
            Assert.AreEqual(new DateTime(2016, 7, 8, 0, 30, 0), connect.GetTime().End);
        }

        [TestMethod]
        public void FullRead()
        {
            var connect = MakeCloneConnect("Read");
            var sig = connect.AddInitialSignal("Ob.SigB", DataType.Boolean, "Ob.SigB");
            Assert.AreEqual(DataType.Boolean, sig.DataType);
            Assert.AreEqual(connect, sig.Connect);
            Assert.AreEqual(1, connect.Signals.Count);
            Assert.IsTrue(connect.Signals.ContainsKey("Ob.SigB"));
            sig = connect.AddInitialSignal("Ob.SigI", DataType.Integer, "Ob.SigI");
            Assert.AreEqual(DataType.Integer, sig.DataType);
            Assert.AreEqual(connect, sig.Connect);
            Assert.AreEqual(2, connect.Signals.Count);
            Assert.IsTrue(connect.Signals.ContainsKey("Ob.SigI"));
            sig = connect.AddInitialSignal("Ob.SigR", DataType.Real, "Ob.SigR");
            Assert.AreEqual(DataType.Real, sig.DataType);
            Assert.AreEqual(connect, sig.Connect);
            Assert.AreEqual(3, connect.Signals.Count);
            Assert.IsTrue(connect.Signals.ContainsKey("Ob.SigR"));
            sig = connect.AddInitialSignal("Ob.SigS", DataType.String, "Ob.SigS");
            Assert.AreEqual(DataType.String, sig.DataType);
            Assert.AreEqual(connect, sig.Connect);
            Assert.AreEqual(4, connect.Signals.Count);
            Assert.IsTrue(connect.Signals.ContainsKey("Ob.SigS"));
            sig = connect.AddInitialSignal("Ob.SigT", DataType.Time, "Ob.SigT");
            Assert.AreEqual(DataType.Time, sig.DataType);
            Assert.AreEqual(connect, sig.Connect);
            Assert.AreEqual(5, connect.Signals.Count);
            Assert.IsTrue(connect.Signals.ContainsKey("Ob.SigT"));
            connect.AddInitialSignal("ObConst.SigB", DataType.Boolean, "ObConst.SigB");
            connect.AddInitialSignal("ObConst.SigI", DataType.Integer, "ObConst.SigI");
            connect.AddInitialSignal("ObConst.SigR", DataType.Real, "ObConst.SigR");
            connect.AddInitialSignal("ObConst.SigS", DataType.String, "ObConst.SigS");
            Assert.AreEqual(9, connect.Signals.Count);
            Assert.IsTrue(connect.Signals.ContainsKey("ObConst.SigB"));
            Assert.IsTrue(connect.Signals.ContainsKey("ObConst.SigI"));
            Assert.IsTrue(connect.Signals.ContainsKey("ObConst.SigR"));
            Assert.IsTrue(connect.Signals.ContainsKey("ObConst.SigS"));

            GetValues(connect, new DateTime(2016, 7, 8), new DateTime(2016, 7, 8, 1, 0, 0));
        }
    }
}