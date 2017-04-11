﻿using System;
using BaseLibrary;
using BaseLibraryTest;
using CommonTypes;
using Logika;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProvidersLibrary;

namespace ProvidersTest
{
    [TestClass]
    public class LogikaTest
    {
        private SourceConnect MakeProviders(string prefix)
        {
            TestLib.CopyFile(@"Providers\Logika", "prolog.mdb", prefix + "Prolog.mdb");
            var factory = new ProvidersFactory();
            var logger = new Logger(new TestHistory(), new AppIndicator());
            var con = (SourceConnect)factory.CreateConnect(ProviderType.Source, "SourceCon", "Logika", logger);
            var prov = factory.CreateProvider("LogikaSource", "DbFile=" + TestLib.TestRunDir + @"Providers\Logika\" + prefix + "Prolog.mdb");
            con.JoinProvider(prov);
            return con;
        }

        [TestMethod]
        public void Signals()
        {
            var con = MakeProviders("Signals");
            var prov = (LogikaSource)con.Provider;

            Assert.AreEqual("SourceCon", con.Name);
            Assert.AreEqual("Logika", con.Complect);
            Assert.AreEqual(ProviderType.Source, con.Type);
            Assert.IsNotNull(con.Logger);
            Assert.AreEqual(con.Context, "Источник: SourceCon");
            Assert.IsNotNull(con.Provider);
            Assert.IsTrue(con.Provider is LogikaSource);
            Assert.AreEqual("LogikaSource", prov.Code);
            Assert.AreEqual("Источник: SourceCon, LogikaSource", prov.Context);
            Assert.AreEqual("DbFile=" + TestLib.TestRunDir + @"Providers\Logika\SignalsProlog.mdb", prov.Inf);
            Assert.AreSame(con, prov.ProviderConnect);
            Assert.IsNotNull(prov.Logger);
            Assert.IsFalse(prov.IsConnected);
            Assert.IsFalse(prov.IsPrepared);

            var ti = con.GetTime();
            Assert.IsTrue(prov.IsConnected);
            Assert.IsFalse(prov.IsPrepared);
            Assert.AreEqual(Static.MinDate, ti.Begin);

            con.AddInitialSignal("T941M_412.t1", DataType.Real, "TableName=T941M;NodeId=412", "", "SignalCode=t1", false);
            con.AddInitialSignal("T941M_412.t2", DataType.Real, "TableName=T941M;NodeId=412", "", "SignalCode=t2", false);
            con.AddInitialSignal("T941M_412.V1", DataType.Real, "TableName=T941M;NodeId=412", "", "SignalCode=V1", false);
            con.AddInitialSignal("T941M_412.V2", DataType.Real, "TableName=T941M;NodeId=412", "", "SignalCode=V2", false);
            con.AddInitialSignal("T941M_412.V3", DataType.Real, "TableName=T941M;NodeId=412", "", "SignalCode=V3", false);
            con.AddInitialSignal("T941M_412.M1", DataType.Real, "TableName=T941M;NodeId=412", "", "SignalCode=M1", false);
            con.AddInitialSignal("T941M_412.M2", DataType.Real, "TableName=T941M;NodeId=412", "", "SignalCode=M2", false);
            con.AddInitialSignal("T941M_412.M3", DataType.Real, "TableName=T941M;NodeId=412", "", "SignalCode=M3", false);
            con.AddInitialSignal("T941M_412.Q", DataType.Real, "TableName=T941M;NodeId=412", "", "SignalCode=Q", false);
            con.AddInitialSignal("T941M_412.Tи", DataType.Real, "TableName=T941M;NodeId=412", "", "SignalCode=Tи", false);

            Assert.AreEqual(10, con.Signals.Count);
            Assert.AreEqual(10, con.InitialSignals.Count);
            Assert.IsTrue(con.Signals.ContainsKey("T941M_412.t1"));
            Assert.AreEqual(DataType.Real, con.Signals["T941M_412.t1"].DataType);
            Assert.IsTrue(con.Signals.ContainsKey("T941M_412.t2"));
            Assert.IsTrue(con.Signals.ContainsKey("T941M_412.V1"));
            Assert.IsTrue(con.Signals.ContainsKey("T941M_412.V2"));
            Assert.IsTrue(con.Signals.ContainsKey("T941M_412.V3"));
            Assert.IsTrue(con.Signals.ContainsKey("T941M_412.M1"));
            Assert.IsTrue(con.Signals.ContainsKey("T941M_412.M2"));
            Assert.IsTrue(con.Signals.ContainsKey("T941M_412.M3"));
            Assert.IsTrue(con.Signals.ContainsKey("T941M_412.Q"));
            Assert.IsTrue(con.Signals.ContainsKey("T941M_412.Tи"));

            Assert.IsFalse(prov.IsPrepared);
            prov.Prepare(false);
            Assert.IsTrue(prov.IsPrepared);
            Assert.IsTrue(prov.IsConnected);
            Assert.IsTrue(prov.IsPrepared);
            Assert.AreEqual(1, prov.Outs.Count);
            Assert.AreEqual(1, prov.OutsId.Count);
            Assert.IsTrue(prov.Outs.ContainsKey("T941M"));
            Assert.IsTrue(prov.Outs["T941M"].ContainsKey(412));
            Assert.IsTrue(prov.OutsId.ContainsKey(412));

            con.ClearSignals();
            Assert.IsFalse(prov.IsPrepared);
            Assert.AreEqual(0, con.Signals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);
            Assert.AreEqual(0, con.InitialSignals.Count);
            Assert.AreEqual(0, prov.Outs.Count);
            Assert.AreEqual(0, prov.OutsId.Count);

            prov = (LogikaSource)new ProvidersFactory().CreateProvider("LogikaSource", "DbFile=" + TestLib.TestRunDir + @"Providers\Logika\НеТотProlog.mdb");
            con.JoinProvider(prov);
            Assert.IsFalse(prov.IsConnected);
            prov.Connect();
            Assert.IsFalse(prov.IsConnected);
        }

        private DateTime D(int h)
        {
            return new DateTime(2016, 10, 11).AddHours(h);
        }

        private void CheckTimeAndErr(SourceSignal sig)
        {
            Assert.AreEqual(25, sig.Value.Count);
            Assert.AreEqual(D(0), sig.Value.TimeI(0));
            Assert.AreEqual(D(1), sig.Value.TimeI(1));
            Assert.AreEqual(D(6), sig.Value.TimeI(6));
            Assert.AreEqual(D(10), sig.Value.TimeI(10));
            Assert.AreEqual(D(20), sig.Value.TimeI(20));
            Assert.AreEqual(D(23), sig.Value.TimeI(23));
            Assert.AreEqual(D(24), sig.Value.TimeI(24));

            Assert.AreEqual(null, sig.Value.ErrorI(0));
            Assert.AreEqual(null, sig.Value.ErrorI(1));
            Assert.AreEqual(null, sig.Value.ErrorI(7));
            Assert.AreEqual(null, sig.Value.ErrorI(11));
            Assert.AreEqual(null, sig.Value.ErrorI(21));
            Assert.AreEqual(null, sig.Value.ErrorI(23));
            Assert.AreEqual(null, sig.Value.ErrorI(24));
        }

        private void CheckValue(double v, SourceSignal sig, int num, int round = 5)
        {
            Assert.AreEqual(v, Math.Round(sig.Value.RealI(num), round));
        }

        [TestMethod]
        public void Values()
        {
            var con = MakeProviders("Values");
            con.AddInitialSignal("T941M_412.t1", DataType.Real, "TableName=T941M;NodeId=412", "", "SignalCode=t1", false);
            con.AddInitialSignal("T941M_412.t2", DataType.Real, "TableName=T941M;NodeId=412", "", "SignalCode=t2", false);
            con.AddInitialSignal("T941M_412.V1", DataType.Real, "TableName=T941M;NodeId=412", "", "SignalCode=V1", false);
            con.AddInitialSignal("T941M_412.V2", DataType.Real, "TableName=T941M;NodeId=412", "", "SignalCode=V2", false);
            con.AddInitialSignal("T941M_412.V3", DataType.Real, "TableName=T941M;NodeId=412", "", "SignalCode=V3", false);
            con.AddInitialSignal("T941M_412.M1", DataType.Real, "TableName=T941M;NodeId=412", "", "SignalCode=M1", false);
            con.AddInitialSignal("T941M_412.M2", DataType.Real, "TableName=T941M;NodeId=412", "", "SignalCode=M2", false);
            con.AddInitialSignal("T941M_412.M3", DataType.Real, "TableName=T941M;NodeId=412", "", "SignalCode=M3", false);
            con.AddInitialSignal("T941M_412.Q", DataType.Real, "TableName=T941M;NodeId=412", "", "SignalCode=Q", false);
            con.AddInitialSignal("T941M_412.Tи", DataType.Real, "TableName=T941M;NodeId=412", "", "SignalCode=Tи", false);

            using (con.StartPeriod(D(0), D(24), "Single"))
            {
                con.GetValues();
                Assert.IsTrue(con.Source.IsConnected);
                Assert.IsTrue(con.Source.IsPrepared);
                var sig = con.Signals["T941M_412.t1"];
                CheckTimeAndErr(sig);
                CheckValue(71.65745, sig, 0);
                CheckValue(71.7553, sig, 1);
                CheckValue(72.64032, sig, 7);
                CheckValue(71.74596, sig, 14);
                CheckValue(71.55679, sig, 23);

                sig = con.Signals["T941M_412.t2"];
                CheckTimeAndErr(sig);
                CheckValue(58.88191, sig, 0);
                CheckValue(58.84616, sig, 1);
                CheckValue(59.69515, sig, 7);
                CheckValue(59.96256, sig, 14);
                CheckValue(58.90205, sig, 23);

                sig = con.Signals["T941M_412.V1"];
                CheckTimeAndErr(sig);
                CheckValue(289.9, sig, 0, 1);
                CheckValue(290.1, sig, 1, 1);
                CheckValue(290.5, sig, 7, 1);
                CheckValue(290.3, sig, 14, 1);
                CheckValue(290.5, sig, 23, 1);

                sig = con.Signals["T941M_412.V2"];
                CheckTimeAndErr(sig);
                CheckValue(285.4, sig, 0, 1);
                CheckValue(285.5, sig, 1, 1);
                CheckValue(285.3, sig, 7, 1);
                CheckValue(285.6, sig, 14, 1);
                CheckValue(285.5, sig, 23, 1);

                sig = con.Signals["T941M_412.V3"];
                CheckTimeAndErr(sig);
                CheckValue(0, sig, 0);
                CheckValue(0, sig, 1);
                CheckValue(0, sig, 7);
                CheckValue(0, sig, 14);
                CheckValue(0, sig, 23);

                sig = con.Signals["T941M_412.M1"];
                CheckTimeAndErr(sig);
                CheckValue(283.2027, sig, 0);
                CheckValue(283.38159, sig, 1);
                CheckValue(283.62253, sig, 7);
                CheckValue(283.57852, sig, 14);
                CheckValue(283.80579, sig, 23);

                sig = con.Signals["T941M_412.M2"];
                CheckTimeAndErr(sig);
                CheckValue(280.79285, sig, 0);
                CheckValue(280.89642, sig, 1);
                CheckValue(280.57599, sig, 7);
                CheckValue(280.83179, sig, 14);
                CheckValue(280.88831, sig, 23);

                sig = con.Signals["T941M_412.M3"];
                CheckTimeAndErr(sig);
                CheckValue(0, sig, 0);
                CheckValue(0, sig, 1);
                CheckValue(0, sig, 7);
                CheckValue(0, sig, 14);
                CheckValue(0, sig, 23);

                sig = con.Signals["T941M_412.Q"];
                CheckTimeAndErr(sig);
                CheckValue(3.590067, sig, 0, 6);
                CheckValue(3.629146, sig, 1, 6);
                CheckValue(3.635262, sig, 7, 6);
                CheckValue(3.311962, sig, 14, 6);
                CheckValue(3.557389, sig, 23, 6);

                sig = con.Signals["T941M_412.Tи"];
                CheckTimeAndErr(sig);
                CheckValue(1, sig, 0);
                CheckValue(1, sig, 1);
                CheckValue(1, sig, 7);
                CheckValue(1, sig, 14);
                CheckValue(1, sig, 23);
            }
        }

        [TestMethod]
        public void Clone()
        {
            TestLib.CopyDir(@"Providers\Logika", "Clone");
            var con = MakeProviders("Clone");
            var cloneDir = TestLib.TestRunDir + @"Providers\Logika\Clone\";
            using (con.StartPeriod(D(0), D(24), "Single"))
                con.MakeClone(cloneDir);
            TestLib.CompareClones(cloneDir + "Clone.accdb", cloneDir + "CorrectClone.accdb");
        }
    }
}