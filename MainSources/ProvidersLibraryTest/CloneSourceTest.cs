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
        
        private SourceConnect MakeCloneConnect()
        {
            CopyFile("Clone.accdb");
            var factory = new ProvidersFactory();
            var connect = factory.CreateConnect(ProviderType.Source, "TestSource", "Clones", new Logger());
            connect.JionProviders(factory.CreateProvider("CloneSource", "CloneDir=" + _dir));
            return (SourceConnect)connect;
        }

        [TestMethod]
        public void CloneProps()
        {
            var connect = MakeCloneConnect();
            var source = (CloneSource)connect.Provider;
            Assert.IsNotNull(connect);
            Assert.AreEqual(ProviderType.Source, connect.Type);
            Assert.AreEqual("TestSource", connect.Name);
            Assert.AreEqual("CloneSource", connect.Provider.Code);
            Assert.AreEqual("Clones", connect.Complect);
            Assert.AreEqual("Источник: TestSource, CloneSource", connect.Context);
            Assert.IsNotNull(connect.Logger);
            Assert.AreEqual(0, connect.CalcSignals.Count);
            Assert.AreEqual(0, connect.Signals.Count);
            
            Assert.AreEqual(_dir + "Clone.accdb", source.CloneFile);
            Assert.AreEqual("Источник: TestSource, CloneSource", connect.Context);
            Assert.AreEqual(source, connect.Provider);
            Assert.IsTrue(source.Connect());
            Assert.IsTrue(source.CheckConnection());
            Assert.AreEqual("Успешное соединение", source.CheckConnectionMessage);
         
            Assert.AreEqual(new DateTime(2016, 7, 8), connect.GetTime().Begin);
            Assert.AreEqual(new DateTime(2016, 7, 8, 0, 30, 0), connect.GetTime().End);
        }

        [TestMethod]
        public void FullRead()
        {
            var connect = MakeCloneConnect();
            var sig = connect.AddInitialSignal("Ob.SigB", "Ob", DataType.Boolean, "", true);
            Assert.AreEqual(DataType.Boolean, sig.DataType);
            Assert.AreEqual(connect, sig.Connect);
            Assert.AreEqual(1, connect.Signals.Count);
            Assert.IsTrue(connect.Signals.ContainsKey("Ob.SigB"));
            sig = connect.AddInitialSignal("Ob.SigI", "Ob", DataType.Integer, "", true);
            Assert.AreEqual(DataType.Integer, sig.DataType);
            Assert.AreEqual(connect, sig.Connect);
            Assert.AreEqual(2, connect.Signals.Count);
            Assert.IsTrue(connect.Signals.ContainsKey("Ob.SigI"));
            sig = connect.AddInitialSignal("Ob.SigR", "Ob", DataType.Real, "", true);
            Assert.AreEqual(DataType.Real, sig.DataType);
            Assert.AreEqual(connect, sig.Connect);
            Assert.AreEqual(3, connect.Signals.Count);
            Assert.IsTrue(connect.Signals.ContainsKey("Ob.SigR"));
            sig = connect.AddInitialSignal("Ob.SigS", "Ob", DataType.String, "", true);
            Assert.AreEqual(DataType.String, sig.DataType);
            Assert.AreEqual(connect, sig.Connect);
            Assert.AreEqual(4, connect.Signals.Count);
            Assert.IsTrue(connect.Signals.ContainsKey("Ob.SigS"));
            sig = connect.AddInitialSignal("Ob.SigT", "Ob", DataType.Time, "", true);
            Assert.AreEqual(DataType.Time, sig.DataType);
            Assert.AreEqual(connect, sig.Connect);
            Assert.AreEqual(5, connect.Signals.Count);
            Assert.IsTrue(connect.Signals.ContainsKey("Ob.SigT"));
            connect.AddInitialSignal("ObConst.SigB", "ObConst", DataType.Boolean, "", true);
            connect.AddInitialSignal("ObConst.SigI", "ObConst", DataType.Integer, "", true);
            connect.AddInitialSignal("ObConst.SigR", "ObConst", DataType.Real, "", true);
            connect.AddInitialSignal("ObConst.SigS", "ObConst", DataType.String, "", true);
            Assert.AreEqual(9, connect.Signals.Count);
            Assert.IsTrue(connect.Signals.ContainsKey("ObConst.SigB"));
            Assert.IsTrue(connect.Signals.ContainsKey("ObConst.SigI"));
            Assert.IsTrue(connect.Signals.ContainsKey("ObConst.SigR"));
            Assert.IsTrue(connect.Signals.ContainsKey("ObConst.SigS"));

            connect.Prepare();
            connect.GetValues(new DateTime(2016, 7, 8), new DateTime(2016, 7, 8, 1, 0, 0));
        }
    }
}