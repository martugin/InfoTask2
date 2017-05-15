using System;
using BaseLibrary;
using BaseLibraryTest;
using CommonTypes;
using InfoTaskLauncherTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mir;
using ProvidersLibrary;

namespace ProvidersTest
{
    [TestClass]
    public class MirTest
    {
        private SourceConnect MakeProviders()
        {
            var factory = new ProvidersFactory();
            var logger = new Logger(new TestHistory(), new AppIndicator());
            var con = (SourceConnect)factory.CreateConnect(logger, ProviderType.Source,  "SourceCon", "Mir");
            var prov = factory.CreateProvider(logger, "MirSource", TestLib.TestSqlInf("EnergyRes"));
            con.JoinProvider(prov);
            return con;
        }

        [TestMethod]
        public void Signals()
        {
            var con = MakeProviders();
            var prov = (MirSource)con.Provider;
            Assert.AreEqual("SourceCon", con.Code);
            Assert.AreEqual("Mir", con.Complect);
            Assert.AreEqual(ProviderType.Source, con.Type);
            Assert.IsNotNull(con.Logger);
            Assert.AreEqual("SourceCon", con.Context);
            Assert.IsNotNull(con.Provider);
            Assert.IsTrue(con.Provider is MirSource);
            Assert.AreEqual("MirSource", prov.Code);
            Assert.AreEqual("SourceCon", prov.Context);
            Assert.AreEqual(TestLib.TestSqlInf("EnergyRes"), prov.Inf);
            Assert.AreSame(con, prov.ProviderConnect);
            Assert.IsNotNull(prov.Logger);
            Assert.IsFalse(prov.IsConnected);
            Assert.IsFalse(prov.IsPrepared);

            Assert.AreEqual(0, con.ReadingSignals.Count);
            con.ClearSignals();
            Assert.AreEqual(0, con.ReadingSignals.Count);

            con.AddSignal("ГТЭС 4х6 Игольская.В-6 2Г.Активная прямая.Unit", DataType.Real, SignalType.List, 
                "NAME_OBJECT=ГТЭС 4х6 Игольская;NAME_DEVICE=В-6 2Г", "NAME_TYPE=Активная прямая", "ValueType=Unit");
            con.AddSignal("ГТЭС 4х6 Игольская.В-6 2Г.Активная прямая.Indication", DataType.Real, SignalType.List, 
                "NAME_OBJECT=ГТЭС 4х6 Игольская;NAME_DEVICE=В-6 2Г", "NAME_TYPE=Активная прямая", "ValueType=Indication");
            con.AddSignal("ГТЭС 4х6 Игольская.В-6 2Г.Активная обратная.Unit", DataType.Real, SignalType.List, 
                "NAME_OBJECT=ГТЭС 4х6 Игольская;NAME_DEVICE=В-6 2Г", "NAME_TYPE=Активная обратная", "ValueType=Unit");
            con.AddSignal("ГТЭС 4х6 Игольская.В-6 2Г.Активная обратная.Indication", DataType.Real, SignalType.List, 
                "NAME_OBJECT=ГТЭС 4х6 Игольская;NAME_DEVICE=В-6 2Г", "NAME_TYPE=Активная обратная", "ValueType=Indication");
            con.AddSignal("ГТЭС 4х6 Игольская.В-6 2Г.Реактивная прямая.Unit", DataType.Real, SignalType.List, 
                "NAME_OBJECT=ГТЭС 4х6 Игольская;NAME_DEVICE=В-6 2Г", "NAME_TYPE=Реактивная прямая", "ValueType=Unit");
            con.AddSignal("ГТЭС 4х6 Игольская.В-6 2Г.Реактивная прямая.Indication", DataType.Real, SignalType.List, 
                "NAME_OBJECT=ГТЭС 4х6 Игольская;NAME_DEVICE=В-6 2Г", "NAME_TYPE=Реактивная прямая", "ValueType=Indication");
            con.AddSignal("ГТЭС 4х6 Игольская.В-6 2Г.Реактивная обратная.Unit", DataType.Real, SignalType.List, 
                "NAME_OBJECT=ГТЭС 4х6 Игольская;NAME_DEVICE=В-6 2Г", "NAME_TYPE=Реактивная обратная", "ValueType=Unit");
            con.AddSignal("ГТЭС 4х6 Игольская.В-6 2Г.Реактивная обратная.Indication", DataType.Real, SignalType.List, 
                "NAME_OBJECT=ГТЭС 4х6 Игольская;NAME_DEVICE=В-6 2Г", "NAME_TYPE=Реактивная обратная", "ValueType=Indication");

            Assert.AreEqual(8, con.ReadingSignals.Count);
            Assert.AreEqual(8, con.InitialSignals.Count);
            Assert.IsTrue(con.ReadingSignals.ContainsKey("ГТЭС 4х6 Игольская.В-6 2Г.Активная прямая.Unit"));
            Assert.AreEqual(DataType.Real, con.ReadingSignals["ГТЭС 4х6 Игольская.В-6 2Г.Активная прямая.Unit"].DataType);
            Assert.IsTrue(con.ReadingSignals.ContainsKey("ГТЭС 4х6 Игольская.В-6 2Г.Реактивная прямая.Indication"));
            Assert.AreEqual(DataType.Real, con.ReadingSignals["ГТЭС 4х6 Игольская.В-6 2Г.Реактивная прямая.Indication"].DataType);

            con.AddSignal("ГТЭС 2х6МВт Игольско-Талового нмр..Яч.14 Ввод 6Г.Активная прямая.Unit", DataType.Real, SignalType.List, 
                "NAME_OBJECT=ГТЭС 2х6МВт Игольско-Талового нмр.;NAME_DEVICE=Яч.14 Ввод 6Г", "NAME_TYPE=Активная прямая", "ValueType=Unit");
            con.AddSignal("ГТЭС 2х6МВт Игольско-Талового нмр..Яч.14 Ввод 6Г.Активная обратная.Unit", DataType.Real, SignalType.List, 
                "NAME_OBJECT=ГТЭС 2х6МВт Игольско-Талового нмр.;NAME_DEVICE=Яч.14 Ввод 6Г", "NAME_TYPE=Активная обратная", "ValueType=Unit");

            Assert.AreEqual(10, con.ReadingSignals.Count);
            Assert.AreEqual(10, con.InitialSignals.Count);
            Assert.IsTrue(con.ReadingSignals.ContainsKey("ГТЭС 2х6МВт Игольско-Талового нмр..Яч.14 Ввод 6Г.Активная прямая.Unit"));
            Assert.AreEqual(DataType.Real, con.ReadingSignals["ГТЭС 2х6МВт Игольско-Талового нмр..Яч.14 Ввод 6Г.Активная прямая.Unit"].DataType);

            Assert.IsFalse(prov.IsPrepared);
            Assert.IsFalse(prov.IsConnected);
            prov.Prepare();
            Assert.IsTrue(prov.IsConnected);
            Assert.IsTrue(prov.IsPrepared);
            Assert.AreEqual(6, prov.Outs.Count);
            Assert.AreEqual(8, prov.OutsId.Count);
            Assert.IsTrue(prov.Outs.ContainsKey("ГТЭС 4х6 Игольская.В-6 2Г.Активная прямая"));
            Assert.IsNotNull(prov.Outs["ГТЭС 4х6 Игольская.В-6 2Г.Активная прямая"].UnitSignal);
            Assert.IsNotNull(prov.Outs["ГТЭС 4х6 Игольская.В-6 2Г.Активная прямая"].IndicationSignal);
            Assert.IsTrue(prov.Outs.ContainsKey("ГТЭС 4х6 Игольская.В-6 2Г.Активная обратная"));
            Assert.IsNotNull(prov.Outs["ГТЭС 4х6 Игольская.В-6 2Г.Активная обратная"].UnitSignal);
            Assert.IsNotNull(prov.Outs["ГТЭС 4х6 Игольская.В-6 2Г.Активная обратная"].IndicationSignal);
            Assert.IsTrue(prov.Outs.ContainsKey("ГТЭС 4х6 Игольская.В-6 2Г.Реактивная прямая"));
            Assert.IsNotNull(prov.Outs["ГТЭС 4х6 Игольская.В-6 2Г.Реактивная прямая"].UnitSignal);
            Assert.IsNotNull(prov.Outs["ГТЭС 4х6 Игольская.В-6 2Г.Реактивная прямая"].IndicationSignal);
            Assert.IsTrue(prov.Outs.ContainsKey("ГТЭС 4х6 Игольская.В-6 2Г.Реактивная обратная"));
            Assert.IsNotNull(prov.Outs["ГТЭС 4х6 Игольская.В-6 2Г.Реактивная обратная"].UnitSignal);
            Assert.IsNotNull(prov.Outs["ГТЭС 4х6 Игольская.В-6 2Г.Реактивная обратная"].IndicationSignal);

            Assert.IsTrue(prov.Outs.ContainsKey("ГТЭС 2х6МВт Игольско-Талового нмр..Яч.14 Ввод 6Г.Активная прямая"));
            Assert.IsNotNull(prov.Outs["ГТЭС 2х6МВт Игольско-Талового нмр..Яч.14 Ввод 6Г.Активная прямая"].UnitSignal);
            Assert.IsNull(prov.Outs["ГТЭС 2х6МВт Игольско-Талового нмр..Яч.14 Ввод 6Г.Активная прямая"].IndicationSignal);
            Assert.IsTrue(prov.Outs.ContainsKey("ГТЭС 2х6МВт Игольско-Талового нмр..Яч.14 Ввод 6Г.Активная обратная"));
            Assert.IsNotNull(prov.Outs["ГТЭС 2х6МВт Игольско-Талового нмр..Яч.14 Ввод 6Г.Активная обратная"].UnitSignal);
            Assert.IsNull(prov.Outs["ГТЭС 2х6МВт Игольско-Талового нмр..Яч.14 Ввод 6Г.Активная обратная"].IndicationSignal);

            Assert.IsTrue(prov.OutsId.ContainsKey(676));
            Assert.IsTrue(prov.OutsId.ContainsKey(677));
            Assert.IsTrue(prov.OutsId.ContainsKey(678));
            Assert.IsTrue(prov.OutsId.ContainsKey(679));
            Assert.IsTrue(prov.OutsId.ContainsKey(779));
            Assert.IsTrue(prov.OutsId.ContainsKey(780));
            Assert.IsTrue(prov.OutsId.ContainsKey(1085));
            Assert.IsTrue(prov.OutsId.ContainsKey(1086));

            con.ClearSignals();
            Assert.IsFalse(prov.IsPrepared);
            Assert.AreEqual(0, con.ReadingSignals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);
            Assert.AreEqual(0, con.InitialSignals.Count);
            Assert.AreEqual(0, prov.Outs.Count);
            Assert.AreEqual(0, prov.OutsId.Count);
        }

        private DateTime D(int h, int m = 0)
        {
            return new DateTime(2015, 10, 8).AddHours(h).AddMinutes(m);
        }

        private void CheckTimeAndErr(ListSignal sig)
        {
            Assert.AreEqual(49, sig.OutValue.Count);
            Assert.AreEqual(D(0), sig.OutValue.TimeI(0));
            Assert.AreEqual(D(0, 30), sig.OutValue.TimeI(1));
            Assert.AreEqual(D(6), sig.OutValue.TimeI(12));
            Assert.AreEqual(D(10, 30), sig.OutValue.TimeI(21));
            Assert.AreEqual(D(20), sig.OutValue.TimeI(40));
            Assert.AreEqual(D(23, 30), sig.OutValue.TimeI(47));
            Assert.AreEqual(D(24), sig.OutValue.TimeI(48));

            Assert.AreEqual(null, sig.OutValue.ErrorI(0));
            Assert.AreEqual(null, sig.OutValue.ErrorI(1));
            Assert.AreEqual(null, sig.OutValue.ErrorI(7));
            Assert.AreEqual(null, sig.OutValue.ErrorI(21));
            Assert.AreEqual(null, sig.OutValue.ErrorI(31));
            Assert.AreEqual(null, sig.OutValue.ErrorI(43));
            Assert.AreEqual(null, sig.OutValue.ErrorI(44));
        }

        private void CheckValue(double v, ListSignal sig, int num, int round = 4)
        {
            Assert.AreEqual(v, Math.Round(sig.OutValue.RealI(num), round));
        }

        [TestMethod]
        public void Values()
        {
            var con = MakeProviders();
            var prov = (MirSource)con.Provider;
            con.ClearSignals();
            con.AddSignal("ГТЭС 4х6 Игольская.В-6 1Г.Активная прямая.Unit", DataType.Real, SignalType.List, 
                "NAME_OBJECT=ГТЭС 4х6 Игольская;NAME_DEVICE=В-6 1Г", "NAME_TYPE=Активная прямая", "ValueType=Unit");
            con.AddSignal("ГТЭС 4х6 Игольская.В-6 1Г.Активная прямая.Indication", DataType.Real, SignalType.List, 
                "NAME_OBJECT=ГТЭС 4х6 Игольская;NAME_DEVICE=В-6 1Г", "NAME_TYPE=Активная прямая", "ValueType=Indication");
            con.AddSignal("ГТЭС 4х6 Игольская.В-6 1Г.Активная обратная.Unit", DataType.Real, SignalType.List, 
                "NAME_OBJECT=ГТЭС 4х6 Игольская;NAME_DEVICE=В-6 1Г", "NAME_TYPE=Активная обратная", "ValueType=Unit");
            con.AddSignal("ГТЭС 4х6 Игольская.В-6 1Г.Активная обратная.Indication", DataType.Real, SignalType.List, 
                "NAME_OBJECT=ГТЭС 4х6 Игольская;NAME_DEVICE=В-6 1Г", "NAME_TYPE=Активная обратная", "ValueType=Indication");
            con.AddSignal("ГТЭС 4х6 Игольская.В-6 1Г.Реактивная прямая.Unit", DataType.Real, SignalType.List, 
                "NAME_OBJECT=ГТЭС 4х6 Игольская;NAME_DEVICE=В-6 1Г", "NAME_TYPE=Реактивная прямая", "ValueType=Unit");
            con.AddSignal("ГТЭС 4х6 Игольская.В-6 1Г.Реактивная прямая.Indication", DataType.Real, SignalType.List, 
                "NAME_OBJECT=ГТЭС 4х6 Игольская;NAME_DEVICE=В-6 1Г", "NAME_TYPE=Реактивная прямая", "ValueType=Indication");
            con.AddSignal("ГТЭС 4х6 Игольская.В-6 1Г.Реактивная обратная.Unit", DataType.Real, SignalType.List, 
                "NAME_OBJECT=ГТЭС 4х6 Игольская;NAME_DEVICE=В-6 1Г", "NAME_TYPE=Реактивная обратная", "ValueType=Unit");
            con.AddSignal("ГТЭС 4х6 Игольская.В-6 1Г.Реактивная обратная.Indication", DataType.Real, SignalType.List, 
                "NAME_OBJECT=ГТЭС 4х6 Игольская;NAME_DEVICE=В-6 1Г", "NAME_TYPE=Реактивная обратная", "ValueType=Indication");
            con.AddSignal("ГТЭС 2х6МВт Игольско-Талового нмр..Яч.14 Ввод 6Г.Активная прямая.Unit", DataType.Real, SignalType.List, 
                "NAME_OBJECT=ГТЭС 2х6МВт Игольско-Талового нмр.;NAME_DEVICE=Яч.14 Ввод 6Г", "NAME_TYPE=Активная прямая", "ValueType=Unit");
            con.AddSignal("ГТЭС 2х6МВт Игольско-Талового нмр..Яч.14 Ввод 6Г.Активная обратная.Unit", DataType.Real, SignalType.List, 
                "NAME_OBJECT=ГТЭС 2х6МВт Игольско-Талового нмр.;NAME_DEVICE=Яч.14 Ввод 6Г", "NAME_TYPE=Активная обратная", "ValueType=Unit");
            
            Assert.AreEqual(10, con.ReadingSignals.Count);

            using (con.StartPeriod(D(0), D(24), "Single"))
            {
                con.GetValues();
                var sig = (ListSignal)con.ReadingSignals["ГТЭС 4х6 Игольская.В-6 1Г.Активная прямая.Unit"];
                CheckTimeAndErr(sig);
                CheckValue(2289.6, sig, 0);
                CheckValue(2292.48, sig, 1);
                CheckValue(2164.80, sig, 29);
                CheckValue(2170.56, sig, 30);
                CheckValue(2172.48, sig, 31);
                CheckValue(2168.64, sig, 32);
                CheckValue(2164.80, sig, 33);
                CheckValue(2287.68, sig, 47);
                CheckValue(2290.56, sig, 48);

                sig = (ListSignal)con.ReadingSignals["ГТЭС 4х6 Игольская.В-6 1Г.Активная прямая.Indication"];
                CheckTimeAndErr(sig);
                CheckValue(41606.4227, sig, 0);
                CheckValue(41606.6615, sig, 1);
                CheckValue(41613.2129, sig, 29);
                CheckValue(41613.4390, sig, 30);
                CheckValue(41613.6653, sig, 31);
                CheckValue(41613.8912, sig, 32);
                CheckValue(41614.1167, sig, 33);
                CheckValue(41617.4368, sig, 47);
                CheckValue(41617.6754, sig, 48);

                sig = (ListSignal)con.ReadingSignals["ГТЭС 4х6 Игольская.В-6 1Г.Активная обратная.Unit"];
                CheckTimeAndErr(sig);
                CheckValue(0, sig, 0);
                CheckValue(0, sig, 1);
                CheckValue(0, sig, 29);
                CheckValue(0, sig, 30);
                CheckValue(0, sig, 31);
                CheckValue(0, sig, 32);
                CheckValue(0, sig, 33);
                CheckValue(0, sig, 47);
                CheckValue(0, sig, 48);

                sig = (ListSignal)con.ReadingSignals["ГТЭС 4х6 Игольская.В-6 1Г.Активная обратная.Indication"];
                CheckTimeAndErr(sig);
                CheckValue(0.0988, sig, 0);
                CheckValue(0.0988, sig, 1);
                CheckValue(0.0988, sig, 29);
                CheckValue(0.0988, sig, 30);
                CheckValue(0.0988, sig, 31);
                CheckValue(0.0988, sig, 32);
                CheckValue(0.0988, sig, 33);
                CheckValue(0.0988, sig, 47);
                CheckValue(0.0988, sig, 48);

                sig = (ListSignal)con.ReadingSignals["ГТЭС 4х6 Игольская.В-6 1Г.Реактивная прямая.Unit"];
                CheckTimeAndErr(sig);
                CheckValue(214.08, sig, 0);
                CheckValue(213.12, sig, 1);
                CheckValue(183.36, sig, 29);
                CheckValue(183.36, sig, 30);
                CheckValue(192.96, sig, 31);
                CheckValue(198.72, sig, 32);
                CheckValue(206.4, sig, 33);
                CheckValue(216.96, sig, 34);
                CheckValue(158.4, sig, 47);
                CheckValue(163.2, sig, 48);

                sig = (ListSignal)con.ReadingSignals["ГТЭС 4х6 Игольская.В-6 1Г.Реактивная прямая.Indication"];
                CheckTimeAndErr(sig);
                CheckValue(5795.1083, sig, 0);
                CheckValue(5795.1305, sig, 1);
                CheckValue(5795.7073, sig, 29);
                CheckValue(5795.7264, sig, 30);
                CheckValue(5795.7465, sig, 31);
                CheckValue(5795.7672, sig, 32);
                CheckValue(5795.7887, sig, 33);
                CheckValue(5796.0499, sig, 47);
                CheckValue(5796.0669, sig, 48);

                sig = (ListSignal)con.ReadingSignals["ГТЭС 4х6 Игольская.В-6 1Г.Реактивная обратная.Unit"];
                CheckTimeAndErr(sig);
                CheckValue(0, sig, 0);
                CheckValue(0, sig, 1);
                CheckValue(0, sig, 29);
                CheckValue(0, sig, 30);
                CheckValue(0, sig, 31);
                CheckValue(0, sig, 32);
                CheckValue(0, sig, 33);
                CheckValue(0, sig, 47);
                CheckValue(0, sig, 48);

                sig = (ListSignal)con.ReadingSignals["ГТЭС 4х6 Игольская.В-6 1Г.Реактивная обратная.Indication"];
                CheckTimeAndErr(sig);
                CheckValue(0.8527, sig, 0);
                CheckValue(0.8527, sig, 1);
                CheckValue(0.8527, sig, 29);
                CheckValue(0.8527, sig, 30);
                CheckValue(0.8527, sig, 31);
                CheckValue(0.8527, sig, 32);
                CheckValue(0.8527, sig, 33);
                CheckValue(0.8527, sig, 47);
                CheckValue(0.8527, sig, 48);

                sig = (ListSignal)con.ReadingSignals["ГТЭС 2х6МВт Игольско-Талового нмр..Яч.14 Ввод 6Г.Активная прямая.Unit"];
                CheckTimeAndErr(sig);
                CheckValue(2491.20, sig, 0);
                CheckValue(2484, sig, 1);
                CheckValue(2479.2, sig, 29);
                CheckValue(2481.6, sig, 30);
                CheckValue(2493.6, sig, 31);
                CheckValue(2498.4, sig, 32);
                CheckValue(2498.4, sig, 33);
                CheckValue(2481.6, sig, 47);
                CheckValue(2488.8, sig, 48);

                sig = (ListSignal)con.ReadingSignals["ГТЭС 2х6МВт Игольско-Талового нмр..Яч.14 Ввод 6Г.Активная обратная.Unit"];
                CheckTimeAndErr(sig);
                CheckValue(0, sig, 0);
                CheckValue(0, sig, 1);
                CheckValue(0, sig, 29);
                CheckValue(0, sig, 30);
                CheckValue(0, sig, 31);
                CheckValue(0, sig, 32);
                CheckValue(0, sig, 33);
                CheckValue(0, sig, 47);
                CheckValue(0, sig, 48);
            }

            con.ClearSignals();
            Assert.IsFalse(prov.IsPrepared);
            Assert.AreEqual(0, con.ReadingSignals.Count);
        }

        [TestMethod]
        public void Clone()
        {
            TestLib.CopyDir(@"Providers\Mir", "Clone");

            var app = new TestApp("Test");
            var con = new ClonerConnect(app);
            con.JoinProvider(app.ProvidersFactory.CreateProvider(app, "MirSource", TestLib.TestSqlInf("EnergyRes")));
            var cloneDir = TestLib.TestRunDir + @"Providers\Mir\Clone\";
            SysTabl.PutValueS(cloneDir + "Clone.accdb", "SourceInf", TestLib.TestSqlInf("EnergyRes"));
            using (con.StartPeriod(D(48), D(96), "Single"))
                con.MakeClone(cloneDir);
            TestLib.CompareClones(cloneDir + "Clone.accdb", cloneDir + "CorrectClone.accdb");
        }
    }
}