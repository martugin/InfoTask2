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
            var logger = new Logger(new TestHistory(), new AppIndicator());
            var connect = factory.CreateConnect(ProviderType.Source, "TestSource", "Clones", logger);
            connect.JoinProvider(factory.CreateProvider("CloneSource", "CloneDir=" + TestLib.TestRunDir + @"Providers\Fictive\Clone" + prefix));
            return (SourceConnect)connect;
        }

        private static void GetValues(SourceConnect connect, DateTime beg, DateTime en)
        {
            connect.Logger.StartPeriod(beg, en);
            connect.GetValues();
            connect.Logger.FinishPeriod();
        }

        private DateTime D(int m, int s = 0)
        {
            return new DateTime(2016, 7, 8).AddMinutes(m).AddSeconds(s);
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
         
            Assert.AreEqual(D(0), connect.GetTime().Begin);
            Assert.AreEqual(D(30), connect.GetTime().End);
        }

        [TestMethod]
        public void Signals()
        {
            var con = MakeCloneConnect("Signals");
            var prov = (CloneSource)con.Source;
            var sig = con.AddSignal("Ob.SigB", DataType.Boolean, SignalType.Uniform, "Ob.SigB");
            Assert.AreEqual(DataType.Boolean, sig.DataType);
            Assert.AreEqual(con, sig.Connect);
            Assert.AreEqual(1, con.Signals.Count);
            Assert.IsTrue(con.Signals.ContainsKey("Ob.SigB"));
            sig = con.AddSignal("Ob.SigI", DataType.Integer, SignalType.Uniform, "Ob.SigI");
            Assert.AreEqual(DataType.Integer, sig.DataType);
            Assert.AreEqual(con, sig.Connect);
            Assert.AreEqual(2, con.Signals.Count);
            Assert.IsTrue(con.Signals.ContainsKey("Ob.SigI"));
            sig = con.AddSignal("Ob.SigR", DataType.Real, SignalType.Uniform, "Ob.SigR");
            Assert.AreEqual(DataType.Real, sig.DataType);
            Assert.AreEqual(con, sig.Connect);
            Assert.AreEqual(3, con.Signals.Count);
            Assert.IsTrue(con.Signals.ContainsKey("Ob.SigR"));
            sig = con.AddSignal("Ob.SigS", DataType.String, SignalType.Uniform, "Ob.SigS");
            Assert.AreEqual(DataType.String, sig.DataType);
            Assert.AreEqual(con, sig.Connect);
            Assert.AreEqual(4, con.Signals.Count);
            Assert.IsTrue(con.Signals.ContainsKey("Ob.SigS"));
            sig = con.AddSignal("Ob.SigT", DataType.Time, SignalType.Uniform, "Ob.SigT");
            Assert.AreEqual(DataType.Time, sig.DataType);
            Assert.AreEqual(con, sig.Connect);
            Assert.AreEqual(5, con.Signals.Count);
            Assert.IsTrue(con.Signals.ContainsKey("Ob.SigT"));
            con.AddSignal("ObConst.SigB", DataType.Boolean, SignalType.Uniform, "ObConst.SigB");
            con.AddSignal("ObConst.SigI", DataType.Integer, SignalType.Uniform, "ObConst.SigI");
            con.AddSignal("ObConst.SigR", DataType.Real, SignalType.Uniform, "ObConst.SigR");
            con.AddSignal("ObConst.SigS", DataType.String, SignalType.Uniform, "ObConst.SigS");
            Assert.AreEqual(9, con.Signals.Count);
            Assert.IsTrue(con.Signals.ContainsKey("ObConst.SigB"));
            Assert.IsTrue(con.Signals.ContainsKey("ObConst.SigI"));
            Assert.IsTrue(con.Signals.ContainsKey("ObConst.SigR"));
            Assert.IsTrue(con.Signals.ContainsKey("ObConst.SigS"));

            Assert.IsFalse(prov.IsPrepared);
            prov.Prepare(false);
            Assert.IsTrue(prov.IsPrepared);
            Assert.AreEqual(9, prov.Objects.Count);
            Assert.AreEqual(9, prov.ObjectsId.Count);
            Assert.AreEqual(9, prov.ObjectsList.Count);

            Assert.IsTrue(prov.Objects.ContainsKey("Ob.SigB"));
            Assert.AreEqual(DataType.Boolean, prov.Objects["Ob.SigB"].ValueSignal.DataType);
            Assert.IsTrue(prov.Objects.ContainsKey("Ob.SigI"));
            Assert.AreEqual(DataType.Integer, prov.Objects["Ob.SigI"].ValueSignal.DataType);
            Assert.IsTrue(prov.Objects.ContainsKey("Ob.SigR"));
            Assert.AreEqual(DataType.Real, prov.Objects["Ob.SigR"].ValueSignal.DataType);
            Assert.IsTrue(prov.Objects.ContainsKey("Ob.SigS"));
            Assert.AreEqual(DataType.String, prov.Objects["Ob.SigS"].ValueSignal.DataType);
            Assert.IsTrue(prov.Objects.ContainsKey("Ob.SigT"));
            Assert.AreEqual(DataType.Time, prov.Objects["Ob.SigT"].ValueSignal.DataType);

            Assert.IsTrue(prov.Objects.ContainsKey("ObConst.SigB"));
            Assert.AreEqual(DataType.Boolean, prov.Objects["ObConst.SigB"].ValueSignal.DataType);
            Assert.IsTrue(prov.Objects.ContainsKey("ObConst.SigI"));
            Assert.AreEqual(DataType.Integer, prov.Objects["ObConst.SigI"].ValueSignal.DataType);
            Assert.IsTrue(prov.Objects.ContainsKey("ObConst.SigR"));
            Assert.AreEqual(DataType.Real, prov.Objects["ObConst.SigR"].ValueSignal.DataType);
            Assert.IsTrue(prov.Objects.ContainsKey("ObConst.SigS"));
            Assert.AreEqual(DataType.String, prov.Objects["ObConst.SigS"].ValueSignal.DataType);
        }

        [TestMethod]
        public void ReadValues()
        {
            var con = MakeCloneConnect("ReadValues");
            var prov = (CloneSource)con.Source;
            con.AddSignal("Ob.SigB", DataType.Boolean, SignalType.Uniform, "Ob.SigB");
            con.AddSignal("Ob.SigI", DataType.Integer, SignalType.Uniform, "Ob.SigI");
            con.AddSignal("Ob.SigR", DataType.Real, SignalType.Uniform, "Ob.SigR");
            con.AddSignal("Ob.SigS", DataType.String, SignalType.Uniform, "Ob.SigS");
            con.AddSignal("Ob.SigT", DataType.Time, SignalType.Uniform, "Ob.SigT");
            con.AddSignal("ObConst.SigB", DataType.Boolean, SignalType.Uniform, "ObConst.SigB");
            con.AddSignal("ObConst.SigI", DataType.Integer, SignalType.Uniform, "ObConst.SigI");
            con.AddSignal("ObConst.SigR", DataType.Real, SignalType.Uniform, "ObConst.SigR");
            con.AddSignal("ObConst.SigS", DataType.String,SignalType.Uniform, "ObConst.SigS");

            Assert.IsFalse(prov.IsConnected);
            Assert.IsFalse(prov.IsPrepared);
            GetValues(con, D(0), D(60));
            Assert.IsTrue(prov.IsPrepared);
            Assert.IsTrue(prov.IsConnected);

            var mlist = con.Signals["Ob.SigB"].Value;
            Assert.AreEqual(13, mlist.Count);
            Assert.AreEqual(D(0), mlist.TimeI(0));
            Assert.AreEqual(false, mlist.BooleanI(0));
            Assert.IsNull(mlist.ErrorI(0));
            Assert.AreEqual(D(2), mlist.TimeI(1));
            Assert.AreEqual(true, mlist.BooleanI(1));
            Assert.IsNull(mlist.ErrorI(1));
            Assert.AreEqual(D(18), mlist.TimeI(7));
            Assert.AreEqual(false, mlist.BooleanI(7));
            Assert.IsNotNull(mlist.ErrorI(7));
            Assert.AreEqual(2, mlist.ErrorI(7).Number);
            Assert.AreEqual(ErrQuality.Error, mlist.ErrorI(7).Quality);
            Assert.AreEqual("Ошибка клона", mlist.ErrorI(7).Text);

            mlist = con.Signals["Ob.SigI"].Value;
            Assert.AreEqual(16, mlist.Count);
            Assert.AreEqual(D(0), mlist.TimeI(0));
            Assert.AreEqual(2, mlist.IntegerI(0));
            Assert.IsNull(mlist.ErrorI(0));
            Assert.AreEqual(16, mlist.Count);
            Assert.AreEqual(D(7), mlist.TimeI(4));
            Assert.AreEqual(-1, mlist.IntegerI(4));
            Assert.IsNull(mlist.ErrorI(4));
            Assert.AreEqual(D(27), mlist.TimeI(15));
            Assert.AreEqual(4, mlist.IntegerI(15));
            Assert.IsNull(mlist.ErrorI(15));

            mlist = con.Signals["Ob.SigR"].Value;
            Assert.AreEqual(18, mlist.Count);
            Assert.AreEqual(D(0), mlist.TimeI(0));
            Assert.AreEqual(2, mlist.RealI(0));
            Assert.IsNull(mlist.ErrorI(0));
            Assert.AreEqual(D(3, 30), mlist.TimeI(2));
            Assert.AreEqual(-2, mlist.RealI(2));
            Assert.IsNotNull(mlist.ErrorI(2));
            Assert.AreEqual(2, mlist.ErrorI(2).Number);
            Assert.AreEqual(ErrQuality.Error, mlist.ErrorI(2).Quality);
            Assert.AreEqual("Ошибка клона", mlist.ErrorI(2).Text);
            Assert.AreEqual(D(6, 30), mlist.TimeI(4));
            Assert.AreEqual(1.5, mlist.RealI(4));
            Assert.IsNotNull(mlist.ErrorI(4));
            Assert.AreEqual(1, mlist.ErrorI(4).Number);
            Assert.AreEqual(ErrQuality.Warning, mlist.ErrorI(4).Quality);
            Assert.AreEqual("Предупреждение клона", mlist.ErrorI(4).Text);

            mlist = con.Signals["Ob.SigS"].Value;
            Assert.AreEqual(5, mlist.Count);
            Assert.AreEqual(D(0), mlist.TimeI(0));
            Assert.AreEqual("a", mlist.StringI(0));
            Assert.IsNull(mlist.ErrorI(0));
            Assert.AreEqual(D(14), mlist.TimeI(2));
            Assert.AreEqual("c", mlist.StringI(2));
            Assert.IsNotNull(mlist.ErrorI(2));
            Assert.AreEqual(ErrQuality.Error, mlist.ErrorI(2).Quality);
            Assert.AreEqual("Ошибка клона", mlist.ErrorI(2).Text);
            Assert.AreEqual(D(21), mlist.TimeI(3));
            Assert.AreEqual("d", mlist.StringI(3));
            Assert.IsNull(mlist.ErrorI(3));

            mlist = con.Signals["Ob.SigT"].Value;
            Assert.AreEqual(4, mlist.Count);
            Assert.AreEqual(D(0), mlist.TimeI(0));
            Assert.AreEqual(D(0), mlist.DateI(0));
            Assert.IsNull(mlist.ErrorI(0));
            Assert.AreEqual(D(14, 16), mlist.TimeI(2));
            Assert.AreEqual(D(15), mlist.DateI(2));
            Assert.IsNotNull(mlist.ErrorI(2));
            Assert.AreEqual(ErrQuality.Warning, mlist.ErrorI(2).Quality);
            Assert.AreEqual("Предупреждение клона", mlist.ErrorI(2).Text);
            Assert.AreEqual(D(24, 56), mlist.TimeI(3));
            Assert.AreEqual(D(25), mlist.DateI(3));
            Assert.IsNotNull(mlist.ErrorI(3));
            Assert.AreEqual(ErrQuality.Error, mlist.ErrorI(3).Quality);
            Assert.AreEqual("Ошибка клона", mlist.ErrorI(3).Text);

            mlist = con.Signals["ObConst.SigB"].Value;
            Assert.AreEqual(1, mlist.Count);
            Assert.AreEqual(D(0), mlist.TimeI(0));
            Assert.AreEqual(true, mlist.BooleanI(0));
            Assert.IsNotNull(mlist.ErrorI(0));
            Assert.AreEqual(ErrQuality.Error, mlist.ErrorI(0).Quality);
            Assert.AreEqual("Ошибка клона", mlist.ErrorI(0).Text);

            mlist = con.Signals["ObConst.SigR"].Value;
            Assert.AreEqual(1, mlist.Count);
            Assert.AreEqual(D(0), mlist.TimeI(0));
            Assert.AreEqual(1.5, mlist.RealI(0));
            Assert.IsNull(mlist.ErrorI(0));

            mlist = con.Signals["ObConst.SigS"].Value;
            Assert.AreEqual(1, mlist.Count);
            Assert.AreEqual(D(0), mlist.TimeI(0));
            Assert.AreEqual("sss", mlist.StringI(0));
            Assert.IsNull(mlist.ErrorI(0));
        }

        [TestMethod]
        public void ReadCut()
        {
            var con = MakeCloneConnect("ReadCut");
            con.AddSignal("Ob.SigB", DataType.Boolean, SignalType.Uniform, "Ob.SigB");
            con.AddSignal("Ob.SigI", DataType.Integer, SignalType.Uniform, "Ob.SigI");
            con.AddSignal("Ob.SigR", DataType.Real, SignalType.Uniform, "Ob.SigR");
            con.AddSignal("Ob.SigS", DataType.String, SignalType.Uniform, "Ob.SigS");
            con.AddSignal("Ob.SigT", DataType.Time, SignalType.Uniform, "Ob.SigT");
            con.AddSignal("ObConst.SigB", DataType.Boolean, SignalType.Uniform, "ObConst.SigB");
            con.AddSignal("ObConst.SigI", DataType.Integer, SignalType.Uniform, "ObConst.SigI");
            con.AddSignal("ObConst.SigR", DataType.Real, SignalType.Uniform, "ObConst.SigR");
            con.AddSignal("ObConst.SigS", DataType.String, SignalType.Uniform, "ObConst.SigS");

            GetValues(con, D(12), D(20));
            var mlist = con.Signals["Ob.SigB"].Value;
            Assert.AreEqual(5, mlist.Count);
            Assert.AreEqual(D(11), mlist.TimeI(0));
            Assert.AreEqual(true, mlist.BooleanI(0));
            Assert.IsNull(mlist.ErrorI(0));
            Assert.AreEqual(D(18), mlist.TimeI(3));
            Assert.AreEqual(false, mlist.BooleanI(3));
            Assert.IsNotNull(mlist.ErrorI(3));
            Assert.AreEqual(ErrQuality.Error, mlist.ErrorI(3).Quality);
            Assert.AreEqual("Ошибка клона", mlist.ErrorI(3).Text);
            Assert.AreEqual(D(19), mlist.TimeI(4));
            Assert.AreEqual(false, mlist.BooleanI(4));
            Assert.IsNull(mlist.ErrorI(4));

            mlist = con.Signals["Ob.SigI"].Value;
            Assert.AreEqual(5, mlist.Count);
            Assert.AreEqual(D(11), mlist.TimeI(0));
            Assert.AreEqual(1, mlist.IntegerI(0));
            Assert.IsNull(mlist.ErrorI(0));
            Assert.AreEqual(D(16), mlist.TimeI(2));
            Assert.AreEqual(4, mlist.IntegerI(2));
            Assert.IsNull(mlist.ErrorI(2));
            Assert.AreEqual(D(20), mlist.TimeI(4));
            Assert.AreEqual(2, mlist.IntegerI(4));
            Assert.IsNull(mlist.ErrorI(4));

            mlist = con.Signals["Ob.SigS"].Value;
            Assert.AreEqual(2, mlist.Count);
            Assert.AreEqual(D(7), mlist.TimeI(0));
            Assert.AreEqual("b", mlist.StringI(0));
            Assert.IsNull(mlist.ErrorI(0));
            Assert.AreEqual(D(14), mlist.TimeI(1));
            Assert.AreEqual("c", mlist.StringI(1));
            Assert.IsNotNull(mlist.ErrorI(1));
            Assert.AreEqual(ErrQuality.Error, mlist.ErrorI(1).Quality);
            Assert.AreEqual("Ошибка клона", mlist.ErrorI(1).Text);

            mlist = con.Signals["Ob.SigT"].Value;
            Assert.AreEqual(2, mlist.Count);
            Assert.AreEqual(D(4, 28), mlist.TimeI(0));
            Assert.AreEqual(D(10), mlist.DateI(0));
            Assert.IsNull(mlist.ErrorI(0));
            Assert.AreEqual(D(14, 16), mlist.TimeI(1));
            Assert.AreEqual(D(15), mlist.DateI(1));
            Assert.IsNotNull(mlist.ErrorI(1));
            Assert.AreEqual(ErrQuality.Warning, mlist.ErrorI(1).Quality);
            Assert.AreEqual("Предупреждение клона", mlist.ErrorI(1).Text);

            mlist = con.Signals["ObConst.SigI"].Value;
            Assert.AreEqual(1, mlist.Count);
            Assert.AreEqual(D(0), mlist.TimeI(0));
            Assert.AreEqual(10, mlist.IntegerI(0));
            Assert.IsNotNull(mlist.ErrorI(0));
            Assert.AreEqual(ErrQuality.Warning, mlist.ErrorI(0).Quality);
            Assert.AreEqual("Предупреждение клона", mlist.ErrorI(0).Text);

            mlist = con.Signals["ObConst.SigS"].Value;
            Assert.AreEqual(1, mlist.Count);
            Assert.AreEqual(D(0), mlist.TimeI(0));
            Assert.AreEqual("sss", mlist.StringI(0));
            Assert.IsNull(mlist.ErrorI(0));
        }
    }
}