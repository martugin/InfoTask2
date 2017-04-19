using System;
using BaseLibrary;
using BaseLibraryTest;
using CommonTypes;
using Fictive;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProvidersLibrary;

namespace ProvidersTest
{
    [TestClass]
    public class FictiveTest
    {
        private SourceConnect MakeFictiveConnect(string prefix, bool makeReserve = false)
        {
            var factory = new ProvidersFactory();
            var logger = new Logger(new TestHistory(), new AppIndicator());
            var connect = (SourceConnect)factory.CreateConnect(ProviderType.Source, "TestSource", "Fictive", logger);
            TestLib.CopyFile(@"Providers\Fictive", "Fictive.accdb", "Fictive" + prefix + ".accdb");
            var source = (FictiveSource)factory.CreateProvider("FictiveSource", @"DbFile=" + TestLib.TestRunDir + @"Providers\Fictive\Fictive" + prefix + ".accdb");
            FictiveSource source2 = null;
            if (makeReserve)
                source2 = (FictiveSource)factory.CreateProvider("FictiveSource", @"DbFile=" + TestLib.TestRunDir + @"Providers\Fictive\Fictive" + prefix + ".accdb");
            connect.JoinProvider(source, source2);
            return connect;
        }

        private static ValuesCount GetValues(SourceConnect connect, DateTime beg, DateTime en)
        {
            connect.Logger.StartPeriod(beg, en);
            var vc = connect.GetValues();
            connect.Logger.FinishPeriod();
            return vc;
        }

        //Время по заданным относительным минутам и секундам
        private DateTime RTime(int minutes, int seconds = 0)
        {
            return new DateTime(2016, 07, 08, 0, 0, 0).AddMinutes(minutes).AddSeconds(seconds);
        }

        [TestMethod]
        public void Signals()
        {
            var con = MakeFictiveConnect("Signals");
            DateTime beg = RTime(1), en = RTime(2);

            var prov = (FictiveSource)con.Provider;
            Assert.AreEqual("TestSource", con.Name);
            Assert.AreEqual("Fictive", con.Complect);
            Assert.AreEqual("Источник: TestSource", con.Context);
            Assert.IsNotNull(prov);
            Assert.AreEqual("FictiveSource", prov.Code);
            Assert.AreEqual("Источник: TestSource, FictiveSource", prov.Context);

            con.AddSignal("Ob1.StateSignal", DataType.Integer, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob1;NumObject=1", "", "Signal=State");
            con.AddSignal("Ob1.StateSignal", DataType.Integer, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob1;NumObject=1", "", "Signal=State");
            con.AddSignal("Ob1.ValueSignal", DataType.Real, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob1;NumObject=1", "", "Signal=Value");
            con.AddSignal("Ob1.BoolSignal", DataType.Boolean, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob1;NumObject=1", "", "Signal=Bool");
            con.AddSignal("Ob1.IntSignal", DataType.Integer, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob1;NumObject=1", "", "Signal=Int");
            con.AddSignal("Ob1.RealSignal", DataType.Real, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob1;NumObject=1", "", "Signal=Real");
            con.AddSignal("Ob1.TimeSignal", DataType.Time, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob1;NumObject=1", "", "Signal=Time");
            con.AddSignal("Ob1.StringSignal", DataType.String, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob1;NumObject=1", "", "Signal=String");
            Assert.AreEqual(7, con.Signals.Count);
            Assert.AreEqual(7, con.InitialSignals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);
            Assert.IsTrue(con.Signals["Ob1.StateSignal"] is UniformSignal);
            Assert.AreEqual("Ob1.StateSignal", con.Signals["Ob1.StateSignal"].Code);
            Assert.AreEqual(DataType.Integer, con.Signals["Ob1.StateSignal"].DataType);
            Assert.AreEqual("MomValues", con.Signals["Ob1.StateSignal"].Inf["Table"]);
            Assert.AreEqual("1", con.Signals["Ob1.StateSignal"].Inf["NumObject"]);
            Assert.AreEqual("State", con.Signals["Ob1.StateSignal"].Inf["Signal"]);
            Assert.AreEqual(0, con.Signals["Ob1.StateSignal"].Value.Count);

            con.AddSignal("Ob2.StateSignal", DataType.Integer, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob2;NumObject=2", "", "Signal=State");
            con.AddSignal("Ob2.ValueSignal", DataType.Real, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob2;NumObject=2", "", "Signal=Value");
            con.AddSignal("Ob2.BoolSignal", DataType.Boolean, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob2;NumObject=2", "", "Signal=Bool");
            con.AddSignal("Ob2.IntSignal", DataType.Integer, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob2;NumObject=2", "", "Signal=Int");
            con.AddSignal("Ob2.RealSignal", DataType.Real, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob2;NumObject=2", "", "Signal=Real");
            con.AddSignal("Ob2.TimeSignal", DataType.Time, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob2;NumObject=2", "", "Signal=Time");
            con.AddSignal("Ob2.StringSignal", DataType.String, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob2;NumObject=2", "", "Signal=String");
            Assert.AreEqual(14, con.Signals.Count);
            Assert.AreEqual(14, con.InitialSignals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);

            con.AddSignal("Ob3.StateSignal", DataType.Integer, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob3;NumObject=3", "", "Signal=State");
            con.AddSignal("Ob3.ValueSignal", DataType.Real, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob3;NumObject=3", "", "Signal=Value");
            con.AddSignal("Ob3.BoolSignal", DataType.Boolean, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob3;NumObject=3", "", "Signal=Bool");
            con.AddSignal("Ob3.IntSignal", DataType.Integer, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob3;NumObject=3", "", "Signal=Int");
            con.AddSignal("Ob3.RealSignal", DataType.Real, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob3;NumObject=3", "", "Signal=Real");
            con.AddSignal("Ob3.TimeSignal", DataType.Time, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob3;NumObject=3", "", "Signal=Time");
            con.AddSignal("Ob3.StringSignal", DataType.String, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob3;NumObject=3", "", "Signal=String");
            Assert.AreEqual(21, con.Signals.Count);
            Assert.AreEqual(21, con.InitialSignals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);

            con.AddSignal("ObX.ValueSignal", DataType.Real, SignalType.Uniform, "Table=MomValues2;ObjectCode=ObX;NumObject=5", "", "Signal=Value");
            con.AddSignal("ObX.Value2Signal", DataType.Real, SignalType.Uniform, "Table=MomValues2;ObjectCode=ObX;NumObject=5", "", "Signal=Value2");
            Assert.AreEqual(23, con.Signals.Count);
            Assert.AreEqual(23, con.InitialSignals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);

            con.AddSignal("ObY.ValueSignal", DataType.Real, SignalType.Uniform, "Table=MomValues2;ObjectCode=ObY;NumObject=6", "", "Signal=Value");
            con.AddSignal("ObY.Value2Signal", DataType.Real, SignalType.Uniform, "Table=MomValues2;ObjectCode=ObY;NumObject=6", "", "Signal=Value2");
            Assert.AreEqual(25, con.Signals.Count);
            Assert.AreEqual(25, con.InitialSignals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);

            con.AddSignal("ObZ.ValueSignal", DataType.Real, SignalType.Uniform, "Table=MomValues2;ObjectCode=ObZ;NumObject=7", "", "Signal=Value");
            con.AddSignal("ObZ.Value2Signal", DataType.Real, SignalType.Uniform, "Table=MomValues2;ObjectCode=ObZ;NumObject=7", "", "Signal=Value2");
            Assert.AreEqual(27, con.Signals.Count);
            Assert.AreEqual(27, con.InitialSignals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);

            con.AddSignal("Operator.CommandText", DataType.String, SignalType.List, "Table=MomOperator;ObjectCode=Operator;NumObject=8", "", "Signal=CommandText");
            con.AddSignal("Operator.CommandNumber", DataType.Integer, SignalType.List,  "Table=MomOperator;ObjectCode=Operator;NumObject=8", "", "Signal=CommandNumber");
            Assert.AreEqual(29, con.Signals.Count);
            Assert.AreEqual(29, con.InitialSignals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);

            GetValues(con, beg, en);
            Assert.AreEqual(3, prov.Outs.Count);
            Assert.AreEqual(3, prov.OutsId.Count);
            Assert.AreEqual(3, prov.Outs2.Count);
            Assert.AreEqual(3, prov.OutsId2.Count);
            Assert.IsNotNull(prov.OperatorOut);

            Assert.IsTrue(prov.OutsId.ContainsKey(11));
            Assert.IsTrue(prov.OutsId.ContainsKey(12));
            Assert.IsTrue(prov.OutsId.ContainsKey(13));
            Assert.IsTrue(prov.Outs.ContainsKey("Ob1"));
            Assert.IsTrue(prov.Outs.ContainsKey("Ob2"));
            Assert.IsTrue(prov.Outs.ContainsKey("Ob3"));
            Assert.IsTrue(prov.OutsId2.ContainsKey(15));
            Assert.IsTrue(prov.OutsId2.ContainsKey(16));
            Assert.IsTrue(prov.OutsId2.ContainsKey(17));
            Assert.IsTrue(prov.Outs2.ContainsKey("ObX"));
            Assert.IsTrue(prov.Outs2.ContainsKey("ObY"));
            Assert.IsTrue(prov.Outs2.ContainsKey("ObZ"));
            Assert.IsNotNull(prov.OperatorOut);
            Assert.IsNotNull(prov.ErrPool);

            var ti = con.GetTime();
            Assert.AreEqual(RTime(0), ti.Begin);
            Assert.AreEqual(RTime(30), ti.End);
            ti = con.GetTime();
            Assert.AreEqual(RTime(0), ti.Begin);
            Assert.AreEqual(RTime(30), ti.End);

            Assert.IsTrue(prov.Connect());
            Assert.IsTrue(prov.Reconnect());
            prov.Disconnect();
            Assert.IsTrue(prov.Reconnect());
            prov.Disconnect();
            Assert.IsTrue(prov.Connect());

            con.ClearSignals();
            con.AddSignal("ObX.ValueSignal", DataType.Real, SignalType.Uniform, "Table=MomValuesNo;ObjectCode=ObNo;NumObject=5No", "", "Signal=ValueNo");
            Assert.AreEqual(1, con.Signals.Count);
            prov.Prepare(false);
            Assert.AreEqual(0, prov.Outs.Count);
            prov.Dispose();
        }

        [TestMethod]
        public void ReadByParts()
        {
            var con = MakeFictiveConnect("ByParts");
            var sig1 = con.AddSignal("Ob1.RealSignal", DataType.Real, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob1;NumObject=1", "", "Signal=Real");
            var sig2 = con.AddSignal("Ob2.BoolSignal", DataType.Boolean, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob2;NumObject=2", "", "Signal=Bool");
            var sig3 = con.AddSignal("Ob3.IntSignal", DataType.Integer, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob3;NumObject=3", "", "Signal=Int");
            var sig4 = con.AddSignal("Ob4.TimeSignal", DataType.Time, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob4;NumObject=4", "", "Signal=Time");
            var sig5 = con.AddSignal("ObX.ValueSignal", DataType.Real, SignalType.Uniform, "Table=MomValues2;ObjectCode=ObX;NumObject=5", "", "Signal=Value");
            var sig6 = con.AddSignal("ObY.ValueSignal", DataType.Real, SignalType.Uniform, "Table=MomValues2;ObjectCode=ObY;NumObject=6", "", "Signal=Value");
            var sig7 = con.AddSignal("ObZ.ValueSignal2", DataType.Real, SignalType.Uniform, "Table=MomValues2;ObjectCode=ObZ;NumObject=7", "", "Signal=Value2");
            var sig8 = con.AddSignal("Operator.CommandText", DataType.String, SignalType.List, "Table=MomOperator;ObjectCode=Operator;NumObject=8", "", "Signal=CommandText");
            var sig9 = con.AddSignal("Operator.CommandNumber", DataType.Integer, SignalType.List, "Table=MomOperator;ObjectCode=Operator;NumObject=8", "", "Signal=CommandNumber");
            
            Assert.AreEqual(9, con.Signals.Count);
            Assert.AreEqual(9, con.InitialSignals.Count);
            Assert.AreEqual("Ob1.RealSignal", sig1.Code);
            Assert.AreEqual("Table=MomValues;ObjectCode=Ob1;NumObject=1", sig1.ContextOut);
            Assert.AreEqual("Ob2.BoolSignal", sig2.Code);
            Assert.AreEqual("Table=MomValues;ObjectCode=Ob2;NumObject=2", sig2.ContextOut);
            Assert.AreEqual("Ob3.IntSignal", sig3.Code);
            Assert.AreEqual("Table=MomValues;ObjectCode=Ob3;NumObject=3", sig3.ContextOut);
            Assert.AreEqual("Ob4.TimeSignal", sig4.Code);
            Assert.AreEqual("Table=MomValues;ObjectCode=Ob4;NumObject=4", sig4.ContextOut);
            Assert.AreEqual("ObX.ValueSignal", sig5.Code);
            Assert.AreEqual("Table=MomValues2;ObjectCode=ObX;NumObject=5", sig5.ContextOut);
            Assert.AreEqual("ObY.ValueSignal", sig6.Code);
            Assert.AreEqual("Table=MomValues2;ObjectCode=ObY;NumObject=6", sig6.ContextOut);
            Assert.AreEqual("ObZ.ValueSignal2", sig7.Code);
            Assert.AreEqual("Table=MomValues2;ObjectCode=ObZ;NumObject=7", sig7.ContextOut);
            Assert.AreEqual("Operator.CommandText", sig8.Code);
            Assert.AreEqual("Table=MomOperator;ObjectCode=Operator;NumObject=8", sig8.ContextOut);
            Assert.AreEqual("Operator.CommandNumber", sig9.Code);
            Assert.AreEqual("Table=MomOperator;ObjectCode=Operator;NumObject=8", sig9.ContextOut);

            GetValues(con, RTime(0), RTime(10));
            Assert.IsNotNull(sig1.Value);
            Assert.AreEqual(12, sig1.Value.Count);
            Assert.IsNotNull(sig2.Value);
            Assert.AreEqual(12, sig2.Value.Count);
            Assert.IsNotNull(sig3.Value);
            Assert.AreEqual(12, sig3.Value.Count);
            Assert.IsNotNull(sig4.Value);
            Assert.AreEqual(12, sig4.Value.Count);
            Assert.IsNotNull(sig5.Value);
            Assert.AreEqual(1, sig5.Value.Count);
            Assert.IsNotNull(sig6.Value);
            Assert.AreEqual(1, sig6.Value.Count);
            Assert.IsNotNull(sig7.Value);
            Assert.AreEqual(1, sig7.Value.Count);
            Assert.IsNotNull(sig8.Value);
            Assert.AreEqual(3, sig8.Value.Count);
            Assert.IsNotNull(sig9.Value);
            Assert.AreEqual(3, sig9.Value.Count);
            
            Assert.AreEqual(RTime(0), sig1.Value.TimeI(0));
            Assert.IsNull(sig1.Value.ErrorI(0));
            Assert.AreEqual(3, sig1.Value.RealI(0));
            Assert.AreEqual(RTime(1, 30), sig1.Value.TimeI(1));
            Assert.IsNull(sig1.Value.ErrorI(1));
            Assert.AreEqual(-2, sig1.Value.RealI(1));
            Assert.AreEqual(RTime(2), sig1.Value.TimeI(2));
            Assert.IsNull(sig1.Value.ErrorI(2));
            Assert.AreEqual(-2, sig1.Value.RealI(2));
            Assert.AreEqual(RTime(3, 30), sig1.Value.TimeI(3));
            Assert.IsNotNull(sig1.Value.ErrorI(3));
            Assert.AreEqual(ErrQuality.Error, sig1.Value.ErrorI(3).Quality);
            Assert.AreEqual(2, sig1.Value.ErrorI(3).Number);
            Assert.AreEqual("Ошибка", sig1.Value.ErrorI(3).Text);
            Assert.AreEqual(-2, sig1.Value.RealI(3));
            Assert.AreEqual(RTime(5), sig1.Value.TimeI(4));
            Assert.AreEqual(ErrQuality.Error, sig1.Value.ErrorI(4).Quality);
            Assert.AreEqual(2, sig1.Value.ErrorI(4).Number);
            Assert.AreEqual("Ошибка", sig1.Value.ErrorI(4).Text);
            Assert.AreEqual(1.5, sig1.Value.RealI(4));
            Assert.AreEqual(RTime(6), sig1.Value.TimeI(5));
            Assert.AreEqual(ErrQuality.Error, sig1.Value.ErrorI(5).Quality);
            Assert.AreEqual(2, sig1.Value.ErrorI(5).Number);
            Assert.AreEqual("Ошибка", sig1.Value.ErrorI(5).Text);
            Assert.AreEqual(1.5, sig1.Value.RealI(5));
            Assert.AreEqual(RTime(6, 30), sig1.Value.TimeI(6));
            Assert.AreEqual(ErrQuality.Warning, sig1.Value.ErrorI(6).Quality);
            Assert.AreEqual(1, sig1.Value.ErrorI(6).Number);
            Assert.AreEqual("Предупреждение", sig1.Value.ErrorI(6).Text);
            Assert.AreEqual(1.5, sig1.Value.RealI(6));
            Assert.AreEqual(RTime(7), sig1.Value.TimeI(7));
            Assert.AreEqual(ErrQuality.Warning, sig1.Value.ErrorI(7).Quality);
            Assert.AreEqual(1, sig1.Value.ErrorI(7).Number);
            Assert.AreEqual("Предупреждение", sig1.Value.ErrorI(7).Text);
            Assert.AreEqual(1.5, sig1.Value.RealI(7));
            Assert.AreEqual(RTime(7, 30), sig1.Value.TimeI(8));
            Assert.IsNull(sig1.Value.ErrorI(8));
            Assert.AreEqual(2.5, sig1.Value.RealI(8));
            Assert.AreEqual(RTime(8), sig1.Value.TimeI(9));
            Assert.IsNull(sig1.Value.ErrorI(9));
            Assert.AreEqual(2.5, sig1.Value.RealI(9));
            Assert.AreEqual(RTime(8, 30), sig1.Value.TimeI(10));
            Assert.IsNull(sig1.Value.ErrorI(10));
            Assert.AreEqual(0, sig1.Value.RealI(10));
            Assert.AreEqual(RTime(10), sig1.Value.TimeI(11));
            Assert.IsNull(sig1.Value.ErrorI(11));
            Assert.AreEqual(0, sig1.Value.RealI(11));

            Assert.AreEqual(RTime(0), sig2.Value.TimeI(0));
            Assert.IsNull(sig2.Value.ErrorI(0));
            Assert.AreEqual(false, sig2.Value.BooleanI(0));
            Assert.AreEqual(RTime(1, 30), sig2.Value.TimeI(1));
            Assert.IsNull(sig2.Value.ErrorI(1));
            Assert.AreEqual(false, sig2.Value.BooleanI(1));
            Assert.AreEqual(RTime(2), sig2.Value.TimeI(2));
            Assert.IsNull(sig2.Value.ErrorI(2));
            Assert.AreEqual(true, sig2.Value.BooleanI(2));
            Assert.AreEqual(RTime(3, 30), sig2.Value.TimeI(3));
            Assert.IsNull(sig2.Value.ErrorI(3));
            Assert.AreEqual(true, sig2.Value.BooleanI(3));
            Assert.IsNull(sig2.Value.ErrorI(4));
            Assert.AreEqual(false, sig2.Value.BooleanI(4));
            Assert.AreEqual(RTime(6), sig2.Value.TimeI(5));
            Assert.IsNull(sig2.Value.ErrorI(5));
            Assert.AreEqual(false, sig2.Value.BooleanI(5));
            Assert.AreEqual(RTime(6, 30), sig2.Value.TimeI(6));
            Assert.IsNull(sig2.Value.ErrorI(6));
            Assert.AreEqual(false, sig2.Value.BooleanI(6));
            Assert.AreEqual(RTime(7), sig2.Value.TimeI(7));
            Assert.IsNull(sig2.Value.ErrorI(7));
            Assert.AreEqual(false, sig2.Value.BooleanI(7));
            Assert.AreEqual(RTime(7, 30), sig2.Value.TimeI(8));
            Assert.IsNull(sig2.Value.ErrorI(8));
            Assert.AreEqual(false, sig2.Value.BooleanI(8));
            Assert.AreEqual(RTime(8), sig2.Value.TimeI(9));
            Assert.IsNull(sig2.Value.ErrorI(9));
            Assert.AreEqual(true, sig2.Value.BooleanI(9));
            Assert.AreEqual(RTime(8, 30), sig2.Value.TimeI(10));
            Assert.IsNull(sig2.Value.ErrorI(10));
            Assert.AreEqual(true, sig2.Value.BooleanI(10));
            Assert.AreEqual(RTime(10), sig2.Value.TimeI(11));
            Assert.IsNull(sig2.Value.ErrorI(11));
            Assert.AreEqual(true, sig2.Value.BooleanI(11));

            Assert.AreEqual(RTime(0), sig3.Value.TimeI(0));
            Assert.IsNull(sig3.Value.ErrorI(0));
            Assert.AreEqual(2, sig3.Value.RealI(0));
            Assert.AreEqual(RTime(1, 30), sig3.Value.TimeI(1));
            Assert.IsNull(sig3.Value.ErrorI(1));
            Assert.AreEqual(2, sig3.Value.RealI(1));
            Assert.AreEqual(RTime(2), sig3.Value.TimeI(2));
            Assert.IsNull(sig3.Value.ErrorI(2));
            Assert.AreEqual(4, sig3.Value.RealI(2));
            Assert.AreEqual(RTime(3, 30), sig3.Value.TimeI(3));
            Assert.IsNull(sig3.Value.ErrorI(3));
            Assert.AreEqual(4, sig3.Value.RealI(3));
            Assert.IsNull(sig3.Value.ErrorI(4));
            Assert.AreEqual(3, sig3.Value.RealI(4));
            Assert.AreEqual(RTime(6), sig3.Value.TimeI(5));
            Assert.IsNull(sig3.Value.ErrorI(5));
            Assert.AreEqual(1, sig3.Value.RealI(5));
            Assert.AreEqual(RTime(6, 30), sig3.Value.TimeI(6));
            Assert.IsNull(sig3.Value.ErrorI(6));
            Assert.AreEqual(1, sig3.Value.RealI(6));
            Assert.AreEqual(RTime(7), sig3.Value.TimeI(7));
            Assert.IsNull(sig3.Value.ErrorI(7));
            Assert.AreEqual(-1, sig3.Value.RealI(7));
            Assert.AreEqual(RTime(7, 30), sig3.Value.TimeI(8));
            Assert.IsNull(sig3.Value.ErrorI(8));
            Assert.AreEqual(-1, sig3.Value.RealI(8));
            Assert.AreEqual(RTime(8), sig3.Value.TimeI(9));
            Assert.IsNull(sig3.Value.ErrorI(9));
            Assert.AreEqual(-1, sig3.Value.RealI(9));
            Assert.AreEqual(RTime(8, 30), sig3.Value.TimeI(10));
            Assert.IsNull(sig3.Value.ErrorI(10));
            Assert.AreEqual(-1, sig3.Value.RealI(10));
            Assert.AreEqual(RTime(10), sig3.Value.TimeI(11));
            Assert.IsNull(sig3.Value.ErrorI(11));
            Assert.AreEqual(-2, sig3.Value.RealI(11));

            Assert.AreEqual(RTime(0), sig4.Value.TimeI(0));
            Assert.IsNull(sig4.Value.ErrorI(0));
            Assert.AreEqual(RTime(60), sig4.Value.DateI(0));
            Assert.AreEqual(RTime(1, 30), sig4.Value.TimeI(1));
            Assert.IsNull(sig4.Value.ErrorI(1));
            Assert.AreEqual(RTime(61, 30), sig4.Value.DateI(1));
            Assert.AreEqual(RTime(2), sig4.Value.TimeI(2));
            Assert.IsNull(sig4.Value.ErrorI(2));
            Assert.AreEqual(RTime(62), sig4.Value.DateI(2));
            Assert.AreEqual(RTime(3, 30), sig4.Value.TimeI(3));
            Assert.IsNull(sig4.Value.ErrorI(3));
            Assert.AreEqual(RTime(63, 30), sig4.Value.DateI(3));
            Assert.IsNull(sig4.Value.ErrorI(4));
            Assert.AreEqual(RTime(65), sig4.Value.DateI(4));
            Assert.AreEqual(RTime(6), sig4.Value.TimeI(5));
            Assert.IsNull(sig4.Value.ErrorI(5));
            Assert.AreEqual(RTime(66), sig4.Value.DateI(5));
            Assert.AreEqual(RTime(6, 30), sig4.Value.TimeI(6));
            Assert.IsNull(sig4.Value.ErrorI(6));
            Assert.AreEqual(RTime(66, 30), sig4.Value.DateI(6));
            Assert.AreEqual(RTime(7), sig4.Value.TimeI(7));
            Assert.IsNull(sig4.Value.ErrorI(7));
            Assert.AreEqual(RTime(67), sig4.Value.DateI(7));
            Assert.AreEqual(RTime(7, 30), sig4.Value.TimeI(8));
            Assert.IsNull(sig4.Value.ErrorI(8));
            Assert.AreEqual(RTime(67, 30), sig4.Value.DateI(8));
            Assert.AreEqual(RTime(8), sig4.Value.TimeI(9));
            Assert.IsNull(sig4.Value.ErrorI(9));
            Assert.AreEqual(RTime(68), sig4.Value.DateI(9));
            Assert.AreEqual(RTime(8, 30), sig4.Value.TimeI(10));
            Assert.IsNull(sig4.Value.ErrorI(10));
            Assert.AreEqual(RTime(68, 30), sig4.Value.DateI(10));
            Assert.AreEqual(RTime(10), sig4.Value.TimeI(11));
            Assert.IsNull(sig4.Value.ErrorI(11));
            Assert.AreEqual(RTime(70), sig4.Value.DateI(11));

            Assert.AreEqual(RTime(0), sig5.Value.TimeI(0));
            Assert.IsNull(sig5.Value.ErrorI(0));
            Assert.AreEqual(1, sig5.Value.RealI(0));
            Assert.AreEqual(RTime(0), sig6.Value.TimeI(0));
            Assert.IsNull(sig6.Value.ErrorI(0));
            Assert.AreEqual(1.5, sig6.Value.RealI(0));
            Assert.AreEqual(RTime(0), sig7.Value.TimeI(0));
            Assert.IsNull(sig7.Value.ErrorI(0));
            Assert.AreEqual(-2, sig7.Value.RealI(0));

            Assert.AreEqual(RTime(1, 23), sig8.Value.TimeI(0));
            Assert.AreEqual("Нажал", sig8.Value.StringI(0));
            Assert.AreEqual(RTime(1, 28), sig8.Value.TimeI(1));
            Assert.AreEqual("Отпустил", sig8.Value.StringI(1));
            Assert.AreEqual(RTime(6, 55), sig8.Value.TimeI(2));
            Assert.AreEqual("Вставило", sig8.Value.StringI(2));
            Assert.AreEqual(RTime(1, 23), sig9.Value.TimeI(0));
            Assert.AreEqual(1, sig9.Value.IntegerI(0));
            Assert.AreEqual(RTime(1, 28), sig9.Value.TimeI(1));
            Assert.AreEqual(2, sig9.Value.IntegerI(1));
            Assert.AreEqual(RTime(6, 55), sig9.Value.TimeI(2));
            Assert.AreEqual(3, sig9.Value.IntegerI(2));

            var source = (FictiveSource)con.Provider;
            Assert.AreEqual(4, source.Outs.Count);
            Assert.AreEqual(4, source.OutsId.Count);
            Assert.AreEqual(3, source.Outs2.Count);
            Assert.AreEqual(3, source.OutsId2.Count);
            Assert.IsNotNull(source.OperatorOut);
            con.GetValues();
            Assert.AreEqual(4, source.Outs.Count);
            Assert.AreEqual(4, source.OutsId.Count);
            Assert.AreEqual(3, source.Outs2.Count);
            Assert.AreEqual(3, source.OutsId2.Count);
            Assert.IsNotNull(source.OperatorOut);
        }

        [TestMethod]
        public void ReadCut()
        {
            var connect = MakeFictiveConnect("Cut");
            var source = (FictiveSource)connect.Provider;
            var sigR = (UniformSignal)connect.AddSignal("Ob1.RealSignal", DataType.Real, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob1;NumObject=1", "", "Signal=Real");
            var sigS = (UniformSignal)connect.AddSignal("Ob2.StringSignal", DataType.String, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob2;NumObject=2", "", "Signal=String");
            var sigI = (UniformSignal)connect.AddSignal("Ob2.IntSignal", DataType.Integer, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob2;NumObject=2", "", "Signal=Int");
            var sigX = (UniformSignal)connect.AddSignal("ObX.ValueSignal", DataType.Real, SignalType.Uniform, "Table=MomValues2;ObjectCode=ObX;NumObject=5", "", "Signal=Value");
            var sigT = connect.AddSignal("Operator.CommandText", DataType.String, SignalType.List, "Table=MomOperator;ObjectCode=Operator;NumObject=8", "", "Signal=CommandText");
            var sigN = connect.AddSignal("Operator.CommandNumber", DataType.Integer, SignalType.List, "Table=MomOperator;ObjectCode=Operator;NumObject=8", "", "Signal=CommandNumber");
            Assert.AreEqual(6, connect.Signals.Count);
            Assert.AreEqual(6, connect.InitialSignals.Count);

            var vc = GetValues(connect, RTime(-6), RTime(-1));
            Assert.IsNotNull(sigR.Value);
            Assert.AreEqual(0, sigR.Value.Count);
            Assert.AreEqual(0, sigS.Value.Count);
            Assert.AreEqual(0, sigI.Value.Count);
            Assert.AreEqual(0, sigX.Value.Count);
            Assert.AreEqual(0, sigT.Value.Count);
            Assert.AreEqual(0, sigN.Value.Count);
            Assert.IsFalse(source.Outs["Ob1"].HasBegin);
            Assert.IsFalse(source.Outs["Ob2"].HasBegin);
            Assert.IsFalse(source.Outs2["ObX"].HasBegin);
            Assert.AreEqual(0, vc.ReadCount);
            Assert.AreEqual(0, vc.WriteCount);
            Assert.AreEqual(VcStatus.Undefined, vc.Status);

            vc = GetValues(connect, RTime(-1), RTime(4));
            Assert.IsNotNull(sigR.Value);
            Assert.AreEqual(4, sigR.Value.Count);
            Assert.AreEqual(4, sigS.Value.Count);
            Assert.AreEqual(4, sigI.Value.Count);
            Assert.AreEqual(1, sigX.Value.Count);
            Assert.AreEqual(2, sigT.Value.Count);
            Assert.AreEqual(2, sigN.Value.Count);
            Assert.IsTrue(source.Outs["Ob1"].HasBegin);
            Assert.IsTrue(source.Outs["Ob2"].HasBegin);
            Assert.IsTrue(source.Outs2["ObX"].HasBegin);
            Assert.AreEqual(11, vc.ReadCount);
            Assert.AreEqual(17, vc.WriteCount);
            Assert.AreEqual(VcStatus.Success, vc.Status);
            
            Assert.AreEqual(RTime(0), sigR.Value.TimeI(0));
            Assert.IsNull(sigR.Value.ErrorI(0));
            Assert.AreEqual(3, sigR.Value.RealI(0));
            Assert.AreEqual(RTime(1, 30), sigR.Value.TimeI(1));
            Assert.IsNull(sigR.Value.ErrorI(1));
            Assert.AreEqual(-2, sigR.Value.RealI(1));
            Assert.AreEqual(RTime(2), sigR.Value.TimeI(2));
            Assert.IsNull(sigR.Value.ErrorI(2));
            Assert.AreEqual(-2, sigR.Value.RealI(2));
            Assert.AreEqual(RTime(3, 30), sigR.Value.TimeI(3));
            Assert.IsNotNull(sigR.Value.ErrorI(3));
            Assert.AreEqual(ErrQuality.Error, sigR.Value.ErrorI(3).Quality);
            Assert.AreEqual(2, sigR.Value.ErrorI(3).Number);
            Assert.AreEqual("Ошибка", sigR.Value.ErrorI(3).Text);
            Assert.AreEqual(-2, sigR.Value.RealI(3));

            Assert.AreEqual(RTime(0), sigI.Value.TimeI(0));
            Assert.IsNull(sigI.Value.ErrorI(0));
            Assert.AreEqual(2, sigI.Value.IntegerI(0));
            Assert.AreEqual(RTime(1, 30), sigI.Value.TimeI(1));
            Assert.IsNull(sigI.Value.ErrorI(1));
            Assert.AreEqual(2, sigI.Value.IntegerI(1));
            Assert.AreEqual(RTime(2), sigI.Value.TimeI(2));
            Assert.IsNull(sigI.Value.ErrorI(2));
            Assert.AreEqual(4, sigI.Value.IntegerI(2));
            Assert.AreEqual(RTime(3, 30), sigI.Value.TimeI(3));
            Assert.IsNull(sigI.Value.ErrorI(3));
            Assert.AreEqual(4, sigI.Value.IntegerI(3));

            Assert.AreEqual(RTime(0), sigS.Value.TimeI(0));
            Assert.IsNull(sigS.Value.ErrorI(0));
            Assert.AreEqual("3s", sigS.Value.StringI(0));
            Assert.AreEqual(RTime(1, 30), sigS.Value.TimeI(1));
            Assert.IsNull(sigS.Value.ErrorI(1));
            Assert.AreEqual("-2s", sigS.Value.StringI(1));
            Assert.AreEqual(RTime(2), sigS.Value.TimeI(2));
            Assert.IsNull(sigS.Value.ErrorI(2));
            Assert.AreEqual("-2s", sigS.Value.StringI(2));
            Assert.AreEqual(RTime(3, 30), sigS.Value.TimeI(3));
            Assert.IsNotNull(sigR.Value.ErrorI(3));
            Assert.AreEqual(ErrQuality.Error, sigR.Value.ErrorI(3).Quality);
            Assert.AreEqual(2, sigR.Value.ErrorI(3).Number);
            Assert.AreEqual("Ошибка", sigR.Value.ErrorI(3).Text);
            Assert.AreEqual("-2s", sigS.Value.StringI(3));

            Assert.AreEqual(RTime(0), sigX.Value.TimeI(0));
            Assert.IsNull(sigX.Value.ErrorI(0));
            Assert.AreEqual(1, sigX.Value.RealI(0));
            Assert.AreEqual(RTime(1, 23), sigT.Value.TimeI(0));
            Assert.AreEqual("Нажал", sigT.Value.StringI(0));
            Assert.AreEqual(RTime(1, 28), sigT.Value.TimeI(1));
            Assert.AreEqual("Отпустил", sigT.Value.StringI(1));
            Assert.AreEqual(RTime(1, 23), sigN.Value.TimeI(0));
            Assert.AreEqual(1, sigN.Value.IntegerI(0));
            Assert.AreEqual(RTime(1, 28), sigN.Value.TimeI(1));
            Assert.AreEqual(2, sigN.Value.IntegerI(1));
            
            vc = GetValues(connect, RTime(4), RTime(7));
            Assert.AreEqual(5, sigR.Value.Count);
            Assert.AreEqual(5, sigS.Value.Count);
            Assert.AreEqual(5, sigI.Value.Count);
            Assert.AreEqual(1, sigX.Value.Count);
            Assert.AreEqual(1, sigT.Value.Count);
            Assert.AreEqual(1, sigN.Value.Count);
            Assert.IsTrue(source.Outs["Ob1"].HasBegin);
            Assert.IsTrue(source.Outs["Ob2"].HasBegin);
            Assert.IsTrue(source.Outs2["ObX"].HasBegin);
            Assert.AreEqual(9, vc.ReadCount);
            Assert.AreEqual(18, vc.WriteCount);
            Assert.AreEqual(VcStatus.Success, vc.Status);

            Assert.AreEqual(RTime(3,30), sigI.Value.TimeI(0));
            Assert.AreEqual(4, sigI.Value.IntegerI(0));
            Assert.AreEqual(RTime(5), sigI.Value.TimeI(1));
            Assert.AreEqual(3, sigI.Value.IntegerI(1));
            Assert.AreEqual(RTime(6), sigI.Value.TimeI(2));
            Assert.AreEqual(1, sigI.Value.IntegerI(2));
            Assert.AreEqual(RTime(6, 30), sigI.Value.TimeI(3));
            Assert.AreEqual(1, sigI.Value.IntegerI(3));
            Assert.AreEqual(RTime(7), sigI.Value.TimeI(4));
            Assert.AreEqual(-1, sigI.Value.IntegerI(4));

            Assert.AreEqual(RTime(3, 30), sigR.Value.TimeI(0));
            Assert.AreEqual(-2, sigR.Value.RealI(0));
            Assert.IsNotNull(sigR.Value.ErrorI(0));
            Assert.AreEqual(ErrQuality.Error, sigR.Value.ErrorI(0).Quality);
            Assert.AreEqual(RTime(5), sigR.Value.TimeI(1));
            Assert.AreEqual(1.5, sigR.Value.RealI(1));
            Assert.IsNotNull(sigR.Value.ErrorI(1));
            Assert.AreEqual(ErrQuality.Error, sigR.Value.ErrorI(1).Quality);
            Assert.AreEqual(RTime(6), sigR.Value.TimeI(2));
            Assert.AreEqual(1.5, sigR.Value.RealI(2));
            Assert.IsNotNull(sigR.Value.ErrorI(2));
            Assert.AreEqual(ErrQuality.Error, sigR.Value.ErrorI(2).Quality);
            Assert.AreEqual(RTime(6, 30), sigR.Value.TimeI(3));
            Assert.AreEqual(1.5, sigR.Value.RealI(3));
            Assert.IsNotNull(sigR.Value.ErrorI(3));
            Assert.AreEqual(ErrQuality.Warning, sigR.Value.ErrorI(3).Quality);
            Assert.AreEqual(RTime(7), sigR.Value.TimeI(4));
            Assert.AreEqual(1.5, sigR.Value.RealI(4));
            Assert.IsNotNull(sigR.Value.ErrorI(4));
            Assert.AreEqual(ErrQuality.Warning, sigR.Value.ErrorI(4).Quality);

            Assert.AreEqual(RTime(3, 30), sigS.Value.TimeI(0));
            Assert.AreEqual("-2s", sigS.Value.StringI(0));
            Assert.IsNotNull(sigS.Value.ErrorI(0));
            Assert.AreEqual(ErrQuality.Error, sigS.Value.ErrorI(0).Quality);
            Assert.AreEqual(RTime(5), sigS.Value.TimeI(1));
            Assert.AreEqual("1,5s", sigS.Value.StringI(1));
            Assert.IsNotNull(sigS.Value.ErrorI(1));
            Assert.AreEqual(ErrQuality.Error, sigS.Value.ErrorI(1).Quality);
            Assert.AreEqual(RTime(6), sigS.Value.TimeI(2));
            Assert.AreEqual("1,5s", sigS.Value.StringI(2));
            Assert.IsNotNull(sigS.Value.ErrorI(2));
            Assert.AreEqual(ErrQuality.Error, sigS.Value.ErrorI(2).Quality);
            Assert.AreEqual(RTime(6, 30), sigS.Value.TimeI(3));
            Assert.AreEqual("1,5s", sigS.Value.StringI(3));
            Assert.IsNotNull(sigS.Value.ErrorI(3));
            Assert.AreEqual(ErrQuality.Warning, sigS.Value.ErrorI(3).Quality);
            Assert.AreEqual(RTime(7), sigS.Value.TimeI(4));
            Assert.AreEqual("1,5s", sigS.Value.StringI(4));
            Assert.IsNotNull(sigS.Value.ErrorI(4));
            Assert.AreEqual(ErrQuality.Warning, sigS.Value.ErrorI(4).Quality);

            Assert.AreEqual(RTime(0), sigX.Value.TimeI(0));
            Assert.AreEqual(1, sigX.Value.RealI(0));
            Assert.IsNull(sigX.Value.ErrorI(0));
            Assert.AreEqual(RTime(6, 55), sigT.Value.TimeI(0));
            Assert.AreEqual("Вставило", sigT.Value.StringI(0));
            Assert.AreEqual(RTime(6, 55), sigN.Value.TimeI(0));
            Assert.AreEqual(3, sigN.Value.IntegerI(0));

            vc = GetValues(connect, RTime(7), RTime(12));
            Assert.AreEqual(6, sigR.Value.Count);
            Assert.AreEqual(6, sigS.Value.Count);
            Assert.AreEqual(6, sigI.Value.Count);
            Assert.AreEqual(1, sigX.Value.Count);
            Assert.AreEqual(0, sigT.Value.Count);
            Assert.AreEqual(0, sigN.Value.Count);
            Assert.IsTrue(source.Outs["Ob1"].HasBegin);
            Assert.IsTrue(source.Outs["Ob2"].HasBegin);
            Assert.IsTrue(source.Outs2["ObX"].HasBegin);
            Assert.AreEqual(10, vc.ReadCount);
            Assert.AreEqual(19, vc.WriteCount);
            Assert.AreEqual(VcStatus.Success, vc.Status);

            Assert.AreEqual(RTime(7), sigI.Value.TimeI(0));
            Assert.AreEqual(-1, sigI.Value.IntegerI(0));
            Assert.AreEqual(RTime(7, 30), sigI.Value.TimeI(1));
            Assert.AreEqual(-1, sigI.Value.IntegerI(1));
            Assert.AreEqual(RTime(8), sigI.Value.TimeI(2));
            Assert.AreEqual(-1, sigI.Value.IntegerI(2));
            Assert.AreEqual(RTime(8, 30), sigI.Value.TimeI(3));
            Assert.AreEqual(-1, sigI.Value.IntegerI(3));
            Assert.AreEqual(RTime(10), sigI.Value.TimeI(4));
            Assert.AreEqual(-2, sigI.Value.IntegerI(4));
            Assert.AreEqual(RTime(11), sigI.Value.TimeI(5));
            Assert.AreEqual(1, sigI.Value.IntegerI(5));

            Assert.AreEqual(RTime(7), sigR.Value.TimeI(0));
            Assert.AreEqual(1.5, sigR.Value.RealI(0));
            Assert.IsNotNull(sigR.Value.ErrorI(0));
            Assert.AreEqual(ErrQuality.Warning, sigR.Value.ErrorI(0).Quality);
            Assert.AreEqual(RTime(7, 30), sigR.Value.TimeI(1));
            Assert.AreEqual(2.5, sigR.Value.RealI(1));
            Assert.IsNull(sigR.Value.ErrorI(1));
            Assert.AreEqual(RTime(8), sigR.Value.TimeI(2));
            Assert.AreEqual(2.5, sigR.Value.RealI(1));
            Assert.IsNull(sigR.Value.ErrorI(2));
            Assert.AreEqual(RTime(8, 30), sigR.Value.TimeI(3));
            Assert.AreEqual(0, sigR.Value.RealI(3));
            Assert.IsNull(sigR.Value.ErrorI(3));
            Assert.AreEqual(RTime(10), sigR.Value.TimeI(4));
            Assert.AreEqual(0, sigR.Value.RealI(4));
            Assert.IsNull(sigR.Value.ErrorI(4));
            Assert.AreEqual(RTime(11), sigR.Value.TimeI(5));
            Assert.AreEqual(-1.5, sigR.Value.RealI(5));
            Assert.IsNull(sigR.Value.ErrorI(5));

            Assert.AreEqual(RTime(7), sigS.Value.TimeI(0));
            Assert.AreEqual("1,5s", sigS.Value.StringI(0));
            Assert.IsNotNull(sigS.Value.ErrorI(0));
            Assert.AreEqual(ErrQuality.Warning, sigS.Value.ErrorI(0).Quality);
            Assert.AreEqual(RTime(7, 30), sigS.Value.TimeI(1));
            Assert.AreEqual("2,5s", sigS.Value.StringI(1));
            Assert.IsNull(sigS.Value.ErrorI(1));
            Assert.AreEqual(RTime(8), sigS.Value.TimeI(2));
            Assert.AreEqual("2,5s", sigS.Value.StringI(1));
            Assert.IsNull(sigS.Value.ErrorI(2));
            Assert.AreEqual(RTime(8, 30), sigS.Value.TimeI(3));
            Assert.AreEqual("0s", sigS.Value.StringI(3));
            Assert.IsNull(sigS.Value.ErrorI(3));
            Assert.AreEqual(RTime(10), sigS.Value.TimeI(4));
            Assert.AreEqual("0s", sigS.Value.StringI(4));
            Assert.IsNull(sigS.Value.ErrorI(4));
            Assert.AreEqual(RTime(11), sigS.Value.TimeI(5));
            Assert.AreEqual("-1,5s", sigS.Value.StringI(5));
            Assert.IsNull(sigS.Value.ErrorI(5));

            Assert.AreEqual(RTime(0), sigX.Value.TimeI(0));
            Assert.AreEqual(1, sigX.Value.RealI(0));
            Assert.IsNull(sigX.Value.ErrorI(0));

            vc = GetValues(connect, RTime(12), RTime(15, 30));
            Assert.AreEqual(4, sigR.Value.Count);
            Assert.AreEqual(4, sigS.Value.Count);
            Assert.AreEqual(4, sigI.Value.Count);
            Assert.AreEqual(2, sigX.Value.Count);
            Assert.AreEqual(0, sigT.Value.Count);
            Assert.AreEqual(0, sigN.Value.Count);
            Assert.IsTrue(source.Outs["Ob1"].HasBegin);
            Assert.IsTrue(source.Outs["Ob2"].HasBegin);
            Assert.IsTrue(source.Outs2["ObX"].HasBegin);
            Assert.AreEqual(7, vc.ReadCount);
            Assert.AreEqual(14, vc.WriteCount);
            Assert.AreEqual(VcStatus.Success, vc.Status);

            Assert.AreEqual(RTime(11), sigI.Value.TimeI(0));
            Assert.AreEqual(1, sigI.Value.IntegerI(0));
            Assert.AreEqual(RTime(13), sigI.Value.TimeI(1));
            Assert.AreEqual(0, sigI.Value.IntegerI(1));
            Assert.AreEqual(RTime(14), sigI.Value.TimeI(2));
            Assert.AreEqual(0, sigI.Value.IntegerI(2));
            Assert.AreEqual(RTime(15, 30), sigI.Value.TimeI(3));
            Assert.AreEqual(0, sigI.Value.IntegerI(3));

            Assert.AreEqual(RTime(11), sigR.Value.TimeI(0));
            Assert.AreEqual(-1.5, sigR.Value.RealI(0));
            Assert.IsNull(sigR.Value.ErrorI(0));
            Assert.AreEqual(RTime(13), sigR.Value.TimeI(1));
            Assert.AreEqual(4, sigR.Value.RealI(1));
            Assert.IsNotNull(sigR.Value.ErrorI(1));
            Assert.AreEqual(ErrQuality.Error, sigR.Value.ErrorI(1).Quality);
            Assert.AreEqual(RTime(14), sigR.Value.TimeI(2));
            Assert.AreEqual(4, sigR.Value.RealI(2));
            Assert.IsNull(sigR.Value.ErrorI(2));
            Assert.AreEqual(RTime(15, 30), sigR.Value.TimeI(3));
            Assert.AreEqual(4.5, sigR.Value.RealI(3));
            Assert.IsNull(sigR.Value.ErrorI(3));

            Assert.AreEqual(RTime(11), sigS.Value.TimeI(0));
            Assert.AreEqual("-1,5s", sigS.Value.StringI(0));
            Assert.IsNull(sigS.Value.ErrorI(0));
            Assert.AreEqual(RTime(13), sigS.Value.TimeI(1));
            Assert.AreEqual("4s", sigS.Value.StringI(1));
            Assert.IsNotNull(sigS.Value.ErrorI(1));
            Assert.AreEqual(ErrQuality.Error, sigS.Value.ErrorI(1).Quality);
            Assert.AreEqual(RTime(14), sigS.Value.TimeI(2));
            Assert.AreEqual("4s", sigS.Value.StringI(2));
            Assert.IsNull(sigS.Value.ErrorI(2));
            Assert.AreEqual(RTime(15, 30), sigS.Value.TimeI(3));
            Assert.AreEqual("4,5s", sigS.Value.StringI(3));
            Assert.IsNull(sigS.Value.ErrorI(3));

            Assert.AreEqual(RTime(0), sigX.Value.TimeI(0));
            Assert.AreEqual(1, sigX.Value.RealI(0));
            Assert.IsNull(sigX.Value.ErrorI(0));
            Assert.AreEqual(RTime(15), sigX.Value.TimeI(1));
            Assert.AreEqual(2, sigX.Value.RealI(1));
            Assert.IsNull(sigX.Value.ErrorI(1));
        }

        [TestMethod]
        public void Recursive()
        {
            var con = MakeFictiveConnect("Recursive");
            var sigR = (UniformSignal)con.AddSignal("Ob1.RealSignal", DataType.Real, SignalType.Uniform,  "Table=MomValues;ObjectCode=Ob1;NumObject=1", "", "Signal=Real");
            var sigI = (UniformSignal)con.AddSignal("Ob2.IntSignal", DataType.Integer, SignalType.Uniform,  "Table=MomValues;ObjectCode=Ob2;NumObject=2;IsErrorObject=True", "", "Signal=Int");
            Assert.AreEqual(2, con.Signals.Count);
            Assert.AreEqual(2, con.InitialSignals.Count);

            var vc = GetValues(con, RTime(0), RTime(2));
            Assert.AreEqual(VcStatus.Partial, vc.Status);
            Assert.AreEqual(3, vc.ReadCount);
            Assert.AreEqual(3, vc.WriteCount);

            Assert.AreEqual(3, sigR.Value.Count);
            Assert.AreEqual(0, sigI.Value.Count);
            Assert.AreEqual(RTime(0), sigR.Value.TimeI(0));
            Assert.AreEqual(3, sigR.Value.RealI(0));
            Assert.AreEqual(RTime(1, 30), sigR.Value.TimeI(1));
            Assert.AreEqual(-2, sigR.Value.RealI(1));
            Assert.AreEqual(RTime(2), sigR.Value.TimeI(2));
            Assert.AreEqual(-2, sigR.Value.RealI(2));

            con.ClearSignals();
            Assert.AreEqual(0, con.Signals.Count);
            Assert.AreEqual(0, con.InitialSignals.Count);
            con.AddSignal("Ob2.IntSignal", DataType.Integer, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob2;NumObject=2;IsErrorObject=True", "", "Signal=Int");
            con.AddSignal("Ob3.RealSignal", DataType.Boolean, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob3;NumObject=3;IsErrorObject=True", "", "Signal=Bool");
            con.AddSignal("Ob4.IntSignal", DataType.String, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob4;NumObject=4;IsErrorObject=True", "", "Signal=String");

            vc = GetValues(con, RTime(0), RTime(2));
            Assert.AreEqual(VcStatus.NoSuccess, vc.Status);
            Assert.AreEqual(0, vc.ReadCount);
            Assert.AreEqual(0, vc.WriteCount);

            con.ClearSignals();
            sigR = (UniformSignal)con.AddSignal("Ob1.RealSignal", DataType.Real, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob1;NumObject=1:", "", "Signal=Real");
            sigI = (UniformSignal)con.AddSignal("Ob2.IntSignal", DataType.Integer, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob2;NumObject=2;IsErrorObject=True", "", "Signal=Int");
            var sigB = (UniformSignal)con.AddSignal("Ob3.RealSignal", DataType.Boolean, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob3;NumObject=3", "", "Signal=Bool");
            var sigS = (UniformSignal)con.AddSignal("Ob4.IntSignal", DataType.String, SignalType.Uniform, "Table=MomValues;ObjectCode=Ob4;NumObject=4;IsErrorObject=True", "", "Signal=String");

            vc = GetValues(con, RTime(0), RTime(2));
            Assert.AreEqual(VcStatus.Partial, vc.Status);
            Assert.AreEqual(6, vc.ReadCount);
            Assert.AreEqual(6, vc.WriteCount);

            Assert.AreEqual(3, sigR.Value.Count);
            Assert.AreEqual(0, sigI.Value.Count);
            Assert.AreEqual(3, sigB.Value.Count);
            Assert.AreEqual(0, sigS.Value.Count);
            Assert.AreEqual(RTime(0), sigR.Value.TimeI(0));
            Assert.AreEqual(3, sigR.Value.RealI(0));
            Assert.AreEqual(RTime(1, 30), sigR.Value.TimeI(1));
            Assert.AreEqual(-2, sigR.Value.RealI(1));
            Assert.AreEqual(RTime(2), sigR.Value.TimeI(2));
            Assert.AreEqual(-2, sigR.Value.RealI(2));
            Assert.AreEqual(RTime(0), sigB.Value.TimeI(0));
            Assert.AreEqual(false, sigB.Value.BooleanI(0));
            Assert.AreEqual(RTime(1, 30), sigB.Value.TimeI(1));
            Assert.AreEqual(false, sigB.Value.BooleanI(1));
            Assert.AreEqual(RTime(2), sigB.Value.TimeI(2));
            Assert.AreEqual(true, sigB.Value.BooleanI(2));
        }
    }
}