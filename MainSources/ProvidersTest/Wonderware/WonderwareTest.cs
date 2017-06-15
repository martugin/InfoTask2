using BaseLibrary;
using BaseLibraryTest;
using CommonTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProvidersLibrary;
using Wonderware;

namespace ProvidersTest
{
    [TestClass]
    public class WonderwareTest
    {
        private SourceConnect MakeProviders()
        {
            var factory = new ProvidersFactory();
            var logger = new Logger(new AppIndicator());
            logger.History = new TestHistory(logger);
            var con = (SourceConnect)factory.CreateConnect(logger, ProviderType.Source, "SourceCon", "Wonderware");
            var prov = factory.CreateProvider(logger, "WonderwareSource", TestLib.TestSqlInf("RunTime"));
            con.JoinProvider(prov);
            return con;
        }

        [TestMethod]
        public void Signals()
        {
            var con = MakeProviders();
            var prov = (WonderwareSource)con.Provider;
            Assert.AreEqual("SourceCon", con.Code);
            Assert.AreEqual("Wonderware", con.Complect);
            Assert.AreEqual(ProviderType.Source, con.Type);
            Assert.IsNotNull(con.Logger);
            Assert.AreEqual("SourceCon", con.Context);
            Assert.IsNotNull(con.Provider);
            Assert.IsTrue(con.Provider is WonderwareSource);
            Assert.AreEqual("WonderwareSource", prov.Code);
            Assert.AreEqual("SourceCon", prov.Context);
            Assert.AreEqual(TestLib.TestSqlInf("RunTime"), prov.Inf);
            Assert.AreSame(con, prov.ProviderConnect);
            Assert.IsNotNull(prov.Logger);
            Assert.IsFalse(prov.IsConnected);
            Assert.IsFalse(prov.IsPrepared);

            Assert.AreEqual(0, con.ReadingSignals.Count);
            con.ClearSignals();
            Assert.AreEqual(0, con.ReadingSignals.Count);
            con.AddSignal("A00RL31H02KNB0.Пар", DataType.Real, SignalType.Uniform, "TagName=A00RL31H02KNB0");
            con.AddSignal("A00RL31S01ZSST.Пар", DataType.Real, SignalType.Uniform, "TagName=A00RL31S01ZSST");
            con.AddSignal("D1_NKK03B01.Пар", DataType.Boolean, SignalType.Uniform, "TagName=D1_NKK03B01");
            con.AddSignal("D2CAUP02ON.Пар", DataType.Boolean, SignalType.Uniform, "TagName=D2CAUP02ON");

            Assert.AreEqual(4, con.ReadingSignals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);
            Assert.AreEqual(4, con.InitialSignals.Count);
            Assert.IsTrue(con.ReadingSignals.ContainsKey("A00RL31H02KNB0.Пар"));
            Assert.AreEqual(DataType.Real, con.ReadingSignals["A00RL31H02KNB0.Пар"].DataType);
            Assert.IsTrue(con.ReadingSignals.ContainsKey("A00RL31S01ZSST.Пар"));
            Assert.AreEqual(DataType.Real, con.ReadingSignals["A00RL31S01ZSST.Пар"].DataType);
            Assert.IsTrue(con.ReadingSignals.ContainsKey("D1_NKK03B01.Пар"));
            Assert.AreEqual(DataType.Boolean, con.ReadingSignals["D1_NKK03B01.Пар"].DataType);
            Assert.IsTrue(con.ReadingSignals.ContainsKey("D2CAUP02ON.Пар"));
            Assert.AreEqual(DataType.Boolean, con.ReadingSignals["D2CAUP02ON.Пар"].DataType);

            Assert.AreEqual(0, prov.Outs.Count);
            Assert.IsFalse(prov.IsPrepared);
            prov.Prepare(false);
            Assert.IsTrue(prov.IsPrepared);
            Assert.AreEqual(4, prov.Outs.Count);
            Assert.IsTrue(prov.Outs.ContainsKey("A00RL31H02KNB0"));
            Assert.IsNotNull(prov.Outs["A00RL31H02KNB0"].ValueSignal);
            Assert.IsTrue(prov.Outs.ContainsKey("A00RL31S01ZSST"));
            Assert.IsNotNull(prov.Outs["A00RL31S01ZSST"].ValueSignal);
            Assert.IsTrue(prov.Outs.ContainsKey("D1_NKK03B01"));
            Assert.IsNotNull(prov.Outs["D1_NKK03B01"].ValueSignal);
            Assert.IsTrue(prov.Outs.ContainsKey("D2CAUP02ON"));
            Assert.IsNotNull(prov.Outs["D2CAUP02ON"].ValueSignal);

            con.ClearSignals();
            Assert.IsFalse(prov.IsPrepared);
            Assert.AreEqual(0, con.ReadingSignals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);
            Assert.AreEqual(0, con.InitialSignals.Count);
            Assert.AreEqual(0, prov.Outs.Count);
        }
    }
}