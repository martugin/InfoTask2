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
            var connect = (SourceConnect)factory.CreateConnect(ProviderType.Source, "TestSource", "Fictive", new Logger());
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

            con.AddInitialSignal("Ob1.StateSignal", DataType.Integer, "Table=MomValues;ObjectCode=Ob1;NumObject=1", "", "Signal=State");
            con.AddInitialSignal("Ob1.StateSignal", DataType.Integer, "Table=MomValues;ObjectCode=Ob1;NumObject=1", "", "Signal=State");
            con.AddInitialSignal("Ob1.ValueSignal", DataType.Real, "Table=MomValues;ObjectCode=Ob1;NumObject=1", "", "Signal=Value");
            con.AddInitialSignal("Ob1.BoolSignal", DataType.Boolean, "Table=MomValues;ObjectCode=Ob1;NumObject=1", "", "Signal=Bool");
            con.AddInitialSignal("Ob1.IntSignal", DataType.Integer, "Table=MomValues;ObjectCode=Ob1;NumObject=1", "", "Signal=Int");
            con.AddInitialSignal("Ob1.RealSignal", DataType.Real, "Table=MomValues;ObjectCode=Ob1;NumObject=1", "", "Signal=Real");
            con.AddInitialSignal("Ob1.TimeSignal", DataType.Time, "Table=MomValues;ObjectCode=Ob1;NumObject=1", "", "Signal=Time");
            con.AddInitialSignal("Ob1.StringSignal", DataType.String, "Table=MomValues;ObjectCode=Ob1;NumObject=1", "", "Signal=String");
            Assert.AreEqual(7, con.Signals.Count);
            Assert.AreEqual(7, con.InitialSignals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);
            Assert.IsTrue(con.Signals["Ob1.StateSignal"] is UniformSignal);
            Assert.AreEqual("Ob1.StateSignal", con.Signals["Ob1.StateSignal"].Code);
            Assert.AreEqual(DataType.Integer, con.Signals["Ob1.StateSignal"].DataType);
            Assert.AreEqual("MomValues", con.Signals["Ob1.StateSignal"].Inf["Table"]);
            Assert.AreEqual("1", con.Signals["Ob1.StateSignal"].Inf["NumObject"]);
            Assert.AreEqual("State", con.Signals["Ob1.StateSignal"].Inf["Signal"]);
            Assert.AreEqual(0, con.Signals["Ob1.StateSignal"].MomList.Count);

            con.AddInitialSignal("Ob2.StateSignal", DataType.Integer, "Table=MomValues;ObjectCode=Ob2;NumObject=2", "", "Signal=State");
            con.AddInitialSignal("Ob2.ValueSignal", DataType.Real, "Table=MomValues;ObjectCode=Ob2;NumObject=2", "", "Signal=Value");
            con.AddInitialSignal("Ob2.BoolSignal", DataType.Boolean, "Table=MomValues;ObjectCode=Ob2;NumObject=2", "", "Signal=Bool");
            con.AddInitialSignal("Ob2.IntSignal", DataType.Integer, "Table=MomValues;ObjectCode=Ob2;NumObject=2", "", "Signal=Int");
            con.AddInitialSignal("Ob2.RealSignal", DataType.Real, "Table=MomValues;ObjectCode=Ob2;NumObject=2", "", "Signal=Real");
            con.AddInitialSignal("Ob2.TimeSignal", DataType.Time, "Table=MomValues;ObjectCode=Ob2;NumObject=2", "", "Signal=Time");
            con.AddInitialSignal("Ob2.StringSignal", DataType.String, "Table=MomValues;ObjectCode=Ob2;NumObject=2", "", "Signal=String");
            Assert.AreEqual(14, con.Signals.Count);
            Assert.AreEqual(14, con.InitialSignals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);

            con.AddInitialSignal("Ob3.StateSignal", DataType.Integer, "Table=MomValues;ObjectCode=Ob3;NumObject=3", "", "Signal=State");
            con.AddInitialSignal("Ob3.ValueSignal", DataType.Real, "Table=MomValues;ObjectCode=Ob3;NumObject=3", "", "Signal=Value");
            con.AddInitialSignal("Ob3.BoolSignal", DataType.Boolean, "Table=MomValues;ObjectCode=Ob3;NumObject=3", "", "Signal=Bool");
            con.AddInitialSignal("Ob3.IntSignal", DataType.Integer, "Table=MomValues;ObjectCode=Ob3;NumObject=3", "", "Signal=Int");
            con.AddInitialSignal("Ob3.RealSignal", DataType.Real, "Table=MomValues;ObjectCode=Ob3;NumObject=3", "", "Signal=Real");
            con.AddInitialSignal("Ob3.TimeSignal", DataType.Time, "Table=MomValues;ObjectCode=Ob3;NumObject=3", "", "Signal=Time");
            con.AddInitialSignal("Ob3.StringSignal", DataType.String, "Table=MomValues;ObjectCode=Ob3;NumObject=3", "", "Signal=String");
            Assert.AreEqual(21, con.Signals.Count);
            Assert.AreEqual(21, con.InitialSignals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);

            con.AddInitialSignal("ObX.ValueSignal", DataType.Real, "Table=MomValues2;ObjectCode=ObX;NumObject=5", "", "Signal=Value");
            con.AddInitialSignal("ObX.Value2Signal", DataType.Real, "Table=MomValues2;ObjectCode=ObX;NumObject=5", "", "Signal=Value2");
            Assert.AreEqual(23, con.Signals.Count);
            Assert.AreEqual(23, con.InitialSignals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);

            con.AddInitialSignal("ObY.ValueSignal", DataType.Real, "Table=MomValues2;ObjectCode=ObY;NumObject=6", "", "Signal=Value");
            con.AddInitialSignal("ObY.Value2Signal", DataType.Real, "Table=MomValues2;ObjectCode=ObY;NumObject=6", "", "Signal=Value2");
            Assert.AreEqual(25, con.Signals.Count);
            Assert.AreEqual(25, con.InitialSignals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);

            con.AddInitialSignal("ObZ.ValueSignal", DataType.Real, "Table=MomValues2;ObjectCode=ObZ;NumObject=7", "", "Signal=Value");
            con.AddInitialSignal("ObZ.Value2Signal", DataType.Real, "Table=MomValues2;ObjectCode=ObZ;NumObject=7", "", "Signal=Value2");
            Assert.AreEqual(27, con.Signals.Count);
            Assert.AreEqual(27, con.InitialSignals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);

            con.AddInitialSignal("Operator.CommandText", DataType.String, "Table=MomOperator;ObjectCode=Operator;NumObject=8", "", "Signal=CommandText", false);
            con.AddInitialSignal("Operator.CommandNumber", DataType.Integer, "Table=MomOperator;ObjectCode=Operator;NumObject=8", "", "Signal=CommandNumber", false);
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
            con.AddInitialSignal("ObX.ValueSignal", DataType.Real, "Table=MomValuesNo;ObjectCode=ObNo;NumObject=5No", "", "Signal=ValueNo");
            Assert.AreEqual(1, con.Signals.Count);
            prov.Prepare(false);
            Assert.AreEqual(0, prov.Outs.Count);
            prov.Dispose();
        }

        [TestMethod]
        public void ReadByParts()
        {
            var con = MakeFictiveConnect("ByParts");
            var sig1 = con.AddInitialSignal("Ob1.RealSignal", DataType.Real, "Table=MomValues;ObjectCode=Ob1;NumObject=1", "", "Signal=Real");
            var sig2 = con.AddInitialSignal("Ob2.BoolSignal", DataType.Boolean, "Table=MomValues;ObjectCode=Ob2;NumObject=2", "", "Signal=Bool");
            var sig3 = con.AddInitialSignal("Ob3.IntSignal", DataType.Integer, "Table=MomValues;ObjectCode=Ob3;NumObject=3", "", "Signal=Int");
            var sig4 = con.AddInitialSignal("Ob4.TimeSignal", DataType.Time, "Table=MomValues;ObjectCode=Ob4;NumObject=4", "", "Signal=Time");
            var sig5 = con.AddInitialSignal("ObX.ValueSignal", DataType.Real, "Table=MomValues2;ObjectCode=ObX;NumObject=5", "", "Signal=Value");
            var sig6 = con.AddInitialSignal("ObY.ValueSignal", DataType.Real, "Table=MomValues2;ObjectCode=ObY;NumObject=6", "", "Signal=Value");
            var sig7 = con.AddInitialSignal("ObZ.ValueSignal2", DataType.Real, "Table=MomValues2;ObjectCode=ObZ;NumObject=7", "", "Signal=Value2");
            var sig8 = con.AddInitialSignal("Operator.CommandText", DataType.String, "Table=MomOperator;ObjectCode=Operator;NumObject=8", "", "Signal=CommandText", false);
            var sig9 = con.AddInitialSignal("Operator.CommandNumber", DataType.Integer, "Table=MomOperator;ObjectCode=Operator;NumObject=8", "", "Signal=CommandNumber", false);
            
            Assert.AreEqual(9, con.Signals.Count);
            Assert.AreEqual(9, con.InitialSignals.Count);
            Assert.AreEqual("Ob1.RealSignal", sig1.Code);
            Assert.AreEqual("Table=MomValues;ObjectCode=Ob1;NumObject=1", sig1.CodeOut);
            Assert.AreEqual("Ob2.BoolSignal", sig2.Code);
            Assert.AreEqual("Table=MomValues;ObjectCode=Ob2;NumObject=2", sig2.CodeOut);
            Assert.AreEqual("Ob3.IntSignal", sig3.Code);
            Assert.AreEqual("Table=MomValues;ObjectCode=Ob3;NumObject=3", sig3.CodeOut);
            Assert.AreEqual("Ob4.TimeSignal", sig4.Code);
            Assert.AreEqual("Table=MomValues;ObjectCode=Ob4;NumObject=4", sig4.CodeOut);
            Assert.AreEqual("ObX.ValueSignal", sig5.Code);
            Assert.AreEqual("Table=MomValues2;ObjectCode=ObX;NumObject=5", sig5.CodeOut);
            Assert.AreEqual("ObY.ValueSignal", sig6.Code);
            Assert.AreEqual("Table=MomValues2;ObjectCode=ObY;NumObject=6", sig6.CodeOut);
            Assert.AreEqual("ObZ.ValueSignal2", sig7.Code);
            Assert.AreEqual("Table=MomValues2;ObjectCode=ObZ;NumObject=7", sig7.CodeOut);
            Assert.AreEqual("Operator.CommandText", sig8.Code);
            Assert.AreEqual("Table=MomOperator;ObjectCode=Operator;NumObject=8", sig8.CodeOut);
            Assert.AreEqual("Operator.CommandNumber", sig9.Code);
            Assert.AreEqual("Table=MomOperator;ObjectCode=Operator;NumObject=8", sig9.CodeOut);

            GetValues(con, RTime(0), RTime(10));
            Assert.IsNotNull(sig1.MomList);
            Assert.AreEqual(12, sig1.MomList.Count);
            Assert.IsNotNull(sig2.MomList);
            Assert.AreEqual(12, sig2.MomList.Count);
            Assert.IsNotNull(sig3.MomList);
            Assert.AreEqual(12, sig3.MomList.Count);
            Assert.IsNotNull(sig4.MomList);
            Assert.AreEqual(12, sig4.MomList.Count);
            Assert.IsNotNull(sig5.MomList);
            Assert.AreEqual(1, sig5.MomList.Count);
            Assert.IsNotNull(sig6.MomList);
            Assert.AreEqual(1, sig6.MomList.Count);
            Assert.IsNotNull(sig7.MomList);
            Assert.AreEqual(1, sig7.MomList.Count);
            Assert.IsNotNull(sig8.MomList);
            Assert.AreEqual(3, sig8.MomList.Count);
            Assert.IsNotNull(sig9.MomList);
            Assert.AreEqual(3, sig9.MomList.Count);
            
            Assert.AreEqual(RTime(0), sig1.MomList.TimeI(0));
            Assert.IsNull(sig1.MomList.ErrorI(0));
            Assert.AreEqual(3, sig1.MomList.RealI(0));
            Assert.AreEqual(RTime(1, 30), sig1.MomList.TimeI(1));
            Assert.IsNull(sig1.MomList.ErrorI(1));
            Assert.AreEqual(-2, sig1.MomList.RealI(1));
            Assert.AreEqual(RTime(2), sig1.MomList.TimeI(2));
            Assert.IsNull(sig1.MomList.ErrorI(2));
            Assert.AreEqual(-2, sig1.MomList.RealI(2));
            Assert.AreEqual(RTime(3, 30), sig1.MomList.TimeI(3));
            Assert.IsNotNull(sig1.MomList.ErrorI(3));
            Assert.AreEqual(ErrQuality.Error, sig1.MomList.ErrorI(3).Quality);
            Assert.AreEqual(2, sig1.MomList.ErrorI(3).Number);
            Assert.AreEqual("Ошибка", sig1.MomList.ErrorI(3).Text);
            Assert.AreEqual(-2, sig1.MomList.RealI(3));
            Assert.AreEqual(RTime(5), sig1.MomList.TimeI(4));
            Assert.AreEqual(ErrQuality.Error, sig1.MomList.ErrorI(4).Quality);
            Assert.AreEqual(2, sig1.MomList.ErrorI(4).Number);
            Assert.AreEqual("Ошибка", sig1.MomList.ErrorI(4).Text);
            Assert.AreEqual(1.5, sig1.MomList.RealI(4));
            Assert.AreEqual(RTime(6), sig1.MomList.TimeI(5));
            Assert.AreEqual(ErrQuality.Error, sig1.MomList.ErrorI(5).Quality);
            Assert.AreEqual(2, sig1.MomList.ErrorI(5).Number);
            Assert.AreEqual("Ошибка", sig1.MomList.ErrorI(5).Text);
            Assert.AreEqual(1.5, sig1.MomList.RealI(5));
            Assert.AreEqual(RTime(6, 30), sig1.MomList.TimeI(6));
            Assert.AreEqual(ErrQuality.Warning, sig1.MomList.ErrorI(6).Quality);
            Assert.AreEqual(1, sig1.MomList.ErrorI(6).Number);
            Assert.AreEqual("Предупреждение", sig1.MomList.ErrorI(6).Text);
            Assert.AreEqual(1.5, sig1.MomList.RealI(6));
            Assert.AreEqual(RTime(7), sig1.MomList.TimeI(7));
            Assert.AreEqual(ErrQuality.Warning, sig1.MomList.ErrorI(7).Quality);
            Assert.AreEqual(1, sig1.MomList.ErrorI(7).Number);
            Assert.AreEqual("Предупреждение", sig1.MomList.ErrorI(7).Text);
            Assert.AreEqual(1.5, sig1.MomList.RealI(7));
            Assert.AreEqual(RTime(7, 30), sig1.MomList.TimeI(8));
            Assert.IsNull(sig1.MomList.ErrorI(8));
            Assert.AreEqual(2.5, sig1.MomList.RealI(8));
            Assert.AreEqual(RTime(8), sig1.MomList.TimeI(9));
            Assert.IsNull(sig1.MomList.ErrorI(9));
            Assert.AreEqual(2.5, sig1.MomList.RealI(9));
            Assert.AreEqual(RTime(8, 30), sig1.MomList.TimeI(10));
            Assert.IsNull(sig1.MomList.ErrorI(10));
            Assert.AreEqual(0, sig1.MomList.RealI(10));
            Assert.AreEqual(RTime(10), sig1.MomList.TimeI(11));
            Assert.IsNull(sig1.MomList.ErrorI(11));
            Assert.AreEqual(0, sig1.MomList.RealI(11));

            Assert.AreEqual(RTime(0), sig2.MomList.TimeI(0));
            Assert.IsNull(sig2.MomList.ErrorI(0));
            Assert.AreEqual(false, sig2.MomList.BooleanI(0));
            Assert.AreEqual(RTime(1, 30), sig2.MomList.TimeI(1));
            Assert.IsNull(sig2.MomList.ErrorI(1));
            Assert.AreEqual(false, sig2.MomList.BooleanI(1));
            Assert.AreEqual(RTime(2), sig2.MomList.TimeI(2));
            Assert.IsNull(sig2.MomList.ErrorI(2));
            Assert.AreEqual(true, sig2.MomList.BooleanI(2));
            Assert.AreEqual(RTime(3, 30), sig2.MomList.TimeI(3));
            Assert.IsNull(sig2.MomList.ErrorI(3));
            Assert.AreEqual(true, sig2.MomList.BooleanI(3));
            Assert.IsNull(sig2.MomList.ErrorI(4));
            Assert.AreEqual(false, sig2.MomList.BooleanI(4));
            Assert.AreEqual(RTime(6), sig2.MomList.TimeI(5));
            Assert.IsNull(sig2.MomList.ErrorI(5));
            Assert.AreEqual(false, sig2.MomList.BooleanI(5));
            Assert.AreEqual(RTime(6, 30), sig2.MomList.TimeI(6));
            Assert.IsNull(sig2.MomList.ErrorI(6));
            Assert.AreEqual(false, sig2.MomList.BooleanI(6));
            Assert.AreEqual(RTime(7), sig2.MomList.TimeI(7));
            Assert.IsNull(sig2.MomList.ErrorI(7));
            Assert.AreEqual(false, sig2.MomList.BooleanI(7));
            Assert.AreEqual(RTime(7, 30), sig2.MomList.TimeI(8));
            Assert.IsNull(sig2.MomList.ErrorI(8));
            Assert.AreEqual(false, sig2.MomList.BooleanI(8));
            Assert.AreEqual(RTime(8), sig2.MomList.TimeI(9));
            Assert.IsNull(sig2.MomList.ErrorI(9));
            Assert.AreEqual(true, sig2.MomList.BooleanI(9));
            Assert.AreEqual(RTime(8, 30), sig2.MomList.TimeI(10));
            Assert.IsNull(sig2.MomList.ErrorI(10));
            Assert.AreEqual(true, sig2.MomList.BooleanI(10));
            Assert.AreEqual(RTime(10), sig2.MomList.TimeI(11));
            Assert.IsNull(sig2.MomList.ErrorI(11));
            Assert.AreEqual(true, sig2.MomList.BooleanI(11));

            Assert.AreEqual(RTime(0), sig3.MomList.TimeI(0));
            Assert.IsNull(sig3.MomList.ErrorI(0));
            Assert.AreEqual(2, sig3.MomList.RealI(0));
            Assert.AreEqual(RTime(1, 30), sig3.MomList.TimeI(1));
            Assert.IsNull(sig3.MomList.ErrorI(1));
            Assert.AreEqual(2, sig3.MomList.RealI(1));
            Assert.AreEqual(RTime(2), sig3.MomList.TimeI(2));
            Assert.IsNull(sig3.MomList.ErrorI(2));
            Assert.AreEqual(4, sig3.MomList.RealI(2));
            Assert.AreEqual(RTime(3, 30), sig3.MomList.TimeI(3));
            Assert.IsNull(sig3.MomList.ErrorI(3));
            Assert.AreEqual(4, sig3.MomList.RealI(3));
            Assert.IsNull(sig3.MomList.ErrorI(4));
            Assert.AreEqual(3, sig3.MomList.RealI(4));
            Assert.AreEqual(RTime(6), sig3.MomList.TimeI(5));
            Assert.IsNull(sig3.MomList.ErrorI(5));
            Assert.AreEqual(1, sig3.MomList.RealI(5));
            Assert.AreEqual(RTime(6, 30), sig3.MomList.TimeI(6));
            Assert.IsNull(sig3.MomList.ErrorI(6));
            Assert.AreEqual(1, sig3.MomList.RealI(6));
            Assert.AreEqual(RTime(7), sig3.MomList.TimeI(7));
            Assert.IsNull(sig3.MomList.ErrorI(7));
            Assert.AreEqual(-1, sig3.MomList.RealI(7));
            Assert.AreEqual(RTime(7, 30), sig3.MomList.TimeI(8));
            Assert.IsNull(sig3.MomList.ErrorI(8));
            Assert.AreEqual(-1, sig3.MomList.RealI(8));
            Assert.AreEqual(RTime(8), sig3.MomList.TimeI(9));
            Assert.IsNull(sig3.MomList.ErrorI(9));
            Assert.AreEqual(-1, sig3.MomList.RealI(9));
            Assert.AreEqual(RTime(8, 30), sig3.MomList.TimeI(10));
            Assert.IsNull(sig3.MomList.ErrorI(10));
            Assert.AreEqual(-1, sig3.MomList.RealI(10));
            Assert.AreEqual(RTime(10), sig3.MomList.TimeI(11));
            Assert.IsNull(sig3.MomList.ErrorI(11));
            Assert.AreEqual(-2, sig3.MomList.RealI(11));

            Assert.AreEqual(RTime(0), sig4.MomList.TimeI(0));
            Assert.IsNull(sig4.MomList.ErrorI(0));
            Assert.AreEqual(RTime(60), sig4.MomList.DateI(0));
            Assert.AreEqual(RTime(1, 30), sig4.MomList.TimeI(1));
            Assert.IsNull(sig4.MomList.ErrorI(1));
            Assert.AreEqual(RTime(61, 30), sig4.MomList.DateI(1));
            Assert.AreEqual(RTime(2), sig4.MomList.TimeI(2));
            Assert.IsNull(sig4.MomList.ErrorI(2));
            Assert.AreEqual(RTime(62), sig4.MomList.DateI(2));
            Assert.AreEqual(RTime(3, 30), sig4.MomList.TimeI(3));
            Assert.IsNull(sig4.MomList.ErrorI(3));
            Assert.AreEqual(RTime(63, 30), sig4.MomList.DateI(3));
            Assert.IsNull(sig4.MomList.ErrorI(4));
            Assert.AreEqual(RTime(65), sig4.MomList.DateI(4));
            Assert.AreEqual(RTime(6), sig4.MomList.TimeI(5));
            Assert.IsNull(sig4.MomList.ErrorI(5));
            Assert.AreEqual(RTime(66), sig4.MomList.DateI(5));
            Assert.AreEqual(RTime(6, 30), sig4.MomList.TimeI(6));
            Assert.IsNull(sig4.MomList.ErrorI(6));
            Assert.AreEqual(RTime(66, 30), sig4.MomList.DateI(6));
            Assert.AreEqual(RTime(7), sig4.MomList.TimeI(7));
            Assert.IsNull(sig4.MomList.ErrorI(7));
            Assert.AreEqual(RTime(67), sig4.MomList.DateI(7));
            Assert.AreEqual(RTime(7, 30), sig4.MomList.TimeI(8));
            Assert.IsNull(sig4.MomList.ErrorI(8));
            Assert.AreEqual(RTime(67, 30), sig4.MomList.DateI(8));
            Assert.AreEqual(RTime(8), sig4.MomList.TimeI(9));
            Assert.IsNull(sig4.MomList.ErrorI(9));
            Assert.AreEqual(RTime(68), sig4.MomList.DateI(9));
            Assert.AreEqual(RTime(8, 30), sig4.MomList.TimeI(10));
            Assert.IsNull(sig4.MomList.ErrorI(10));
            Assert.AreEqual(RTime(68, 30), sig4.MomList.DateI(10));
            Assert.AreEqual(RTime(10), sig4.MomList.TimeI(11));
            Assert.IsNull(sig4.MomList.ErrorI(11));
            Assert.AreEqual(RTime(70), sig4.MomList.DateI(11));

            Assert.AreEqual(RTime(0), sig5.MomList.TimeI(0));
            Assert.IsNull(sig5.MomList.ErrorI(0));
            Assert.AreEqual(1, sig5.MomList.RealI(0));
            Assert.AreEqual(RTime(0), sig6.MomList.TimeI(0));
            Assert.IsNull(sig6.MomList.ErrorI(0));
            Assert.AreEqual(1.5, sig6.MomList.RealI(0));
            Assert.AreEqual(RTime(0), sig7.MomList.TimeI(0));
            Assert.IsNull(sig7.MomList.ErrorI(0));
            Assert.AreEqual(-2, sig7.MomList.RealI(0));

            Assert.AreEqual(RTime(1, 23), sig8.MomList.TimeI(0));
            Assert.AreEqual("Нажал", sig8.MomList.StringI(0));
            Assert.AreEqual(RTime(1, 28), sig8.MomList.TimeI(1));
            Assert.AreEqual("Отпустил", sig8.MomList.StringI(1));
            Assert.AreEqual(RTime(6, 55), sig8.MomList.TimeI(2));
            Assert.AreEqual("Вставило", sig8.MomList.StringI(2));
            Assert.AreEqual(RTime(1, 23), sig9.MomList.TimeI(0));
            Assert.AreEqual(1, sig9.MomList.IntegerI(0));
            Assert.AreEqual(RTime(1, 28), sig9.MomList.TimeI(1));
            Assert.AreEqual(2, sig9.MomList.IntegerI(1));
            Assert.AreEqual(RTime(6, 55), sig9.MomList.TimeI(2));
            Assert.AreEqual(3, sig9.MomList.IntegerI(2));

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
            var sigR = (UniformSignal)connect.AddInitialSignal("Ob1.RealSignal", DataType.Real, "Table=MomValues;ObjectCode=Ob1;NumObject=1", "", "Signal=Real");
            var sigS = (UniformSignal)connect.AddInitialSignal("Ob2.StringSignal", DataType.String, "Table=MomValues;ObjectCode=Ob2;NumObject=2", "", "Signal=String");
            var sigI = (UniformSignal)connect.AddInitialSignal("Ob2.IntSignal", DataType.Integer, "Table=MomValues;ObjectCode=Ob2;NumObject=2", "", "Signal=Int");
            var sigX = (UniformSignal)connect.AddInitialSignal("ObX.ValueSignal", DataType.Real, "Table=MomValues2;ObjectCode=ObX;NumObject=5", "", "Signal=Value");
            var sigT = connect.AddInitialSignal("Operator.CommandText", DataType.String, "Table=MomOperator;ObjectCode=Operator;NumObject=8", "", "Signal=CommandText", false);
            var sigN = connect.AddInitialSignal("Operator.CommandNumber", DataType.Integer, "Table=MomOperator;ObjectCode=Operator;NumObject=8", "", "Signal=CommandNumber", false);
            Assert.AreEqual(6, connect.Signals.Count);
            Assert.AreEqual(6, connect.InitialSignals.Count);

            var vc = GetValues(connect, RTime(-6), RTime(-1));
            Assert.IsNotNull(sigR.MomList);
            Assert.AreEqual(0, sigR.MomList.Count);
            Assert.AreEqual(0, sigS.MomList.Count);
            Assert.AreEqual(0, sigI.MomList.Count);
            Assert.AreEqual(0, sigX.MomList.Count);
            Assert.AreEqual(0, sigT.MomList.Count);
            Assert.AreEqual(0, sigN.MomList.Count);
            Assert.IsFalse(source.Outs["Ob1"].HasBegin);
            Assert.IsFalse(source.Outs["Ob2"].HasBegin);
            Assert.IsFalse(source.Outs2["ObX"].HasBegin);
            Assert.AreEqual(0, vc.ReadCount);
            Assert.AreEqual(0, vc.WriteCount);
            Assert.AreEqual(VcStatus.Undefined, vc.Status);

            vc = GetValues(connect, RTime(-1), RTime(4));
            Assert.IsNotNull(sigR.MomList);
            Assert.AreEqual(4, sigR.MomList.Count);
            Assert.AreEqual(4, sigS.MomList.Count);
            Assert.AreEqual(4, sigI.MomList.Count);
            Assert.AreEqual(1, sigX.MomList.Count);
            Assert.AreEqual(2, sigT.MomList.Count);
            Assert.AreEqual(2, sigN.MomList.Count);
            Assert.IsTrue(source.Outs["Ob1"].HasBegin);
            Assert.IsTrue(source.Outs["Ob2"].HasBegin);
            Assert.IsTrue(source.Outs2["ObX"].HasBegin);
            Assert.AreEqual(11, vc.ReadCount);
            Assert.AreEqual(17, vc.WriteCount);
            Assert.AreEqual(VcStatus.Success, vc.Status);
            
            Assert.AreEqual(RTime(0), sigR.MomList.TimeI(0));
            Assert.IsNull(sigR.MomList.ErrorI(0));
            Assert.AreEqual(3, sigR.MomList.RealI(0));
            Assert.AreEqual(RTime(1, 30), sigR.MomList.TimeI(1));
            Assert.IsNull(sigR.MomList.ErrorI(1));
            Assert.AreEqual(-2, sigR.MomList.RealI(1));
            Assert.AreEqual(RTime(2), sigR.MomList.TimeI(2));
            Assert.IsNull(sigR.MomList.ErrorI(2));
            Assert.AreEqual(-2, sigR.MomList.RealI(2));
            Assert.AreEqual(RTime(3, 30), sigR.MomList.TimeI(3));
            Assert.IsNotNull(sigR.MomList.ErrorI(3));
            Assert.AreEqual(ErrQuality.Error, sigR.MomList.ErrorI(3).Quality);
            Assert.AreEqual(2, sigR.MomList.ErrorI(3).Number);
            Assert.AreEqual("Ошибка", sigR.MomList.ErrorI(3).Text);
            Assert.AreEqual(-2, sigR.MomList.RealI(3));

            Assert.AreEqual(RTime(0), sigI.MomList.TimeI(0));
            Assert.IsNull(sigI.MomList.ErrorI(0));
            Assert.AreEqual(2, sigI.MomList.IntegerI(0));
            Assert.AreEqual(RTime(1, 30), sigI.MomList.TimeI(1));
            Assert.IsNull(sigI.MomList.ErrorI(1));
            Assert.AreEqual(2, sigI.MomList.IntegerI(1));
            Assert.AreEqual(RTime(2), sigI.MomList.TimeI(2));
            Assert.IsNull(sigI.MomList.ErrorI(2));
            Assert.AreEqual(4, sigI.MomList.IntegerI(2));
            Assert.AreEqual(RTime(3, 30), sigI.MomList.TimeI(3));
            Assert.IsNull(sigI.MomList.ErrorI(3));
            Assert.AreEqual(4, sigI.MomList.IntegerI(3));

            Assert.AreEqual(RTime(0), sigS.MomList.TimeI(0));
            Assert.IsNull(sigS.MomList.ErrorI(0));
            Assert.AreEqual("3s", sigS.MomList.StringI(0));
            Assert.AreEqual(RTime(1, 30), sigS.MomList.TimeI(1));
            Assert.IsNull(sigS.MomList.ErrorI(1));
            Assert.AreEqual("-2s", sigS.MomList.StringI(1));
            Assert.AreEqual(RTime(2), sigS.MomList.TimeI(2));
            Assert.IsNull(sigS.MomList.ErrorI(2));
            Assert.AreEqual("-2s", sigS.MomList.StringI(2));
            Assert.AreEqual(RTime(3, 30), sigS.MomList.TimeI(3));
            Assert.IsNotNull(sigR.MomList.ErrorI(3));
            Assert.AreEqual(ErrQuality.Error, sigR.MomList.ErrorI(3).Quality);
            Assert.AreEqual(2, sigR.MomList.ErrorI(3).Number);
            Assert.AreEqual("Ошибка", sigR.MomList.ErrorI(3).Text);
            Assert.AreEqual("-2s", sigS.MomList.StringI(3));

            Assert.AreEqual(RTime(0), sigX.MomList.TimeI(0));
            Assert.IsNull(sigX.MomList.ErrorI(0));
            Assert.AreEqual(1, sigX.MomList.RealI(0));
            Assert.AreEqual(RTime(1, 23), sigT.MomList.TimeI(0));
            Assert.AreEqual("Нажал", sigT.MomList.StringI(0));
            Assert.AreEqual(RTime(1, 28), sigT.MomList.TimeI(1));
            Assert.AreEqual("Отпустил", sigT.MomList.StringI(1));
            Assert.AreEqual(RTime(1, 23), sigN.MomList.TimeI(0));
            Assert.AreEqual(1, sigN.MomList.IntegerI(0));
            Assert.AreEqual(RTime(1, 28), sigN.MomList.TimeI(1));
            Assert.AreEqual(2, sigN.MomList.IntegerI(1));
            
            vc = GetValues(connect, RTime(4), RTime(7));
            Assert.AreEqual(5, sigR.MomList.Count);
            Assert.AreEqual(5, sigS.MomList.Count);
            Assert.AreEqual(5, sigI.MomList.Count);
            Assert.AreEqual(1, sigX.MomList.Count);
            Assert.AreEqual(1, sigT.MomList.Count);
            Assert.AreEqual(1, sigN.MomList.Count);
            Assert.IsTrue(source.Outs["Ob1"].HasBegin);
            Assert.IsTrue(source.Outs["Ob2"].HasBegin);
            Assert.IsTrue(source.Outs2["ObX"].HasBegin);
            Assert.AreEqual(9, vc.ReadCount);
            Assert.AreEqual(18, vc.WriteCount);
            Assert.AreEqual(VcStatus.Success, vc.Status);

            Assert.AreEqual(RTime(3,30), sigI.MomList.TimeI(0));
            Assert.AreEqual(4, sigI.MomList.IntegerI(0));
            Assert.AreEqual(RTime(5), sigI.MomList.TimeI(1));
            Assert.AreEqual(3, sigI.MomList.IntegerI(1));
            Assert.AreEqual(RTime(6), sigI.MomList.TimeI(2));
            Assert.AreEqual(1, sigI.MomList.IntegerI(2));
            Assert.AreEqual(RTime(6, 30), sigI.MomList.TimeI(3));
            Assert.AreEqual(1, sigI.MomList.IntegerI(3));
            Assert.AreEqual(RTime(7), sigI.MomList.TimeI(4));
            Assert.AreEqual(-1, sigI.MomList.IntegerI(4));

            Assert.AreEqual(RTime(3, 30), sigR.MomList.TimeI(0));
            Assert.AreEqual(-2, sigR.MomList.RealI(0));
            Assert.IsNotNull(sigR.MomList.ErrorI(0));
            Assert.AreEqual(ErrQuality.Error, sigR.MomList.ErrorI(0).Quality);
            Assert.AreEqual(RTime(5), sigR.MomList.TimeI(1));
            Assert.AreEqual(1.5, sigR.MomList.RealI(1));
            Assert.IsNotNull(sigR.MomList.ErrorI(1));
            Assert.AreEqual(ErrQuality.Error, sigR.MomList.ErrorI(1).Quality);
            Assert.AreEqual(RTime(6), sigR.MomList.TimeI(2));
            Assert.AreEqual(1.5, sigR.MomList.RealI(2));
            Assert.IsNotNull(sigR.MomList.ErrorI(2));
            Assert.AreEqual(ErrQuality.Error, sigR.MomList.ErrorI(2).Quality);
            Assert.AreEqual(RTime(6, 30), sigR.MomList.TimeI(3));
            Assert.AreEqual(1.5, sigR.MomList.RealI(3));
            Assert.IsNotNull(sigR.MomList.ErrorI(3));
            Assert.AreEqual(ErrQuality.Warning, sigR.MomList.ErrorI(3).Quality);
            Assert.AreEqual(RTime(7), sigR.MomList.TimeI(4));
            Assert.AreEqual(1.5, sigR.MomList.RealI(4));
            Assert.IsNotNull(sigR.MomList.ErrorI(4));
            Assert.AreEqual(ErrQuality.Warning, sigR.MomList.ErrorI(4).Quality);

            Assert.AreEqual(RTime(3, 30), sigS.MomList.TimeI(0));
            Assert.AreEqual("-2s", sigS.MomList.StringI(0));
            Assert.IsNotNull(sigS.MomList.ErrorI(0));
            Assert.AreEqual(ErrQuality.Error, sigS.MomList.ErrorI(0).Quality);
            Assert.AreEqual(RTime(5), sigS.MomList.TimeI(1));
            Assert.AreEqual("1,5s", sigS.MomList.StringI(1));
            Assert.IsNotNull(sigS.MomList.ErrorI(1));
            Assert.AreEqual(ErrQuality.Error, sigS.MomList.ErrorI(1).Quality);
            Assert.AreEqual(RTime(6), sigS.MomList.TimeI(2));
            Assert.AreEqual("1,5s", sigS.MomList.StringI(2));
            Assert.IsNotNull(sigS.MomList.ErrorI(2));
            Assert.AreEqual(ErrQuality.Error, sigS.MomList.ErrorI(2).Quality);
            Assert.AreEqual(RTime(6, 30), sigS.MomList.TimeI(3));
            Assert.AreEqual("1,5s", sigS.MomList.StringI(3));
            Assert.IsNotNull(sigS.MomList.ErrorI(3));
            Assert.AreEqual(ErrQuality.Warning, sigS.MomList.ErrorI(3).Quality);
            Assert.AreEqual(RTime(7), sigS.MomList.TimeI(4));
            Assert.AreEqual("1,5s", sigS.MomList.StringI(4));
            Assert.IsNotNull(sigS.MomList.ErrorI(4));
            Assert.AreEqual(ErrQuality.Warning, sigS.MomList.ErrorI(4).Quality);

            Assert.AreEqual(RTime(0), sigX.MomList.TimeI(0));
            Assert.AreEqual(1, sigX.MomList.RealI(0));
            Assert.IsNull(sigX.MomList.ErrorI(0));
            Assert.AreEqual(RTime(6, 55), sigT.MomList.TimeI(0));
            Assert.AreEqual("Вставило", sigT.MomList.StringI(0));
            Assert.AreEqual(RTime(6, 55), sigN.MomList.TimeI(0));
            Assert.AreEqual(3, sigN.MomList.IntegerI(0));

            vc = GetValues(connect, RTime(7), RTime(12));
            Assert.AreEqual(6, sigR.MomList.Count);
            Assert.AreEqual(6, sigS.MomList.Count);
            Assert.AreEqual(6, sigI.MomList.Count);
            Assert.AreEqual(1, sigX.MomList.Count);
            Assert.AreEqual(0, sigT.MomList.Count);
            Assert.AreEqual(0, sigN.MomList.Count);
            Assert.IsTrue(source.Outs["Ob1"].HasBegin);
            Assert.IsTrue(source.Outs["Ob2"].HasBegin);
            Assert.IsTrue(source.Outs2["ObX"].HasBegin);
            Assert.AreEqual(10, vc.ReadCount);
            Assert.AreEqual(19, vc.WriteCount);
            Assert.AreEqual(VcStatus.Success, vc.Status);

            Assert.AreEqual(RTime(7), sigI.MomList.TimeI(0));
            Assert.AreEqual(-1, sigI.MomList.IntegerI(0));
            Assert.AreEqual(RTime(7, 30), sigI.MomList.TimeI(1));
            Assert.AreEqual(-1, sigI.MomList.IntegerI(1));
            Assert.AreEqual(RTime(8), sigI.MomList.TimeI(2));
            Assert.AreEqual(-1, sigI.MomList.IntegerI(2));
            Assert.AreEqual(RTime(8, 30), sigI.MomList.TimeI(3));
            Assert.AreEqual(-1, sigI.MomList.IntegerI(3));
            Assert.AreEqual(RTime(10), sigI.MomList.TimeI(4));
            Assert.AreEqual(-2, sigI.MomList.IntegerI(4));
            Assert.AreEqual(RTime(11), sigI.MomList.TimeI(5));
            Assert.AreEqual(1, sigI.MomList.IntegerI(5));

            Assert.AreEqual(RTime(7), sigR.MomList.TimeI(0));
            Assert.AreEqual(1.5, sigR.MomList.RealI(0));
            Assert.IsNotNull(sigR.MomList.ErrorI(0));
            Assert.AreEqual(ErrQuality.Warning, sigR.MomList.ErrorI(0).Quality);
            Assert.AreEqual(RTime(7, 30), sigR.MomList.TimeI(1));
            Assert.AreEqual(2.5, sigR.MomList.RealI(1));
            Assert.IsNull(sigR.MomList.ErrorI(1));
            Assert.AreEqual(RTime(8), sigR.MomList.TimeI(2));
            Assert.AreEqual(2.5, sigR.MomList.RealI(1));
            Assert.IsNull(sigR.MomList.ErrorI(2));
            Assert.AreEqual(RTime(8, 30), sigR.MomList.TimeI(3));
            Assert.AreEqual(0, sigR.MomList.RealI(3));
            Assert.IsNull(sigR.MomList.ErrorI(3));
            Assert.AreEqual(RTime(10), sigR.MomList.TimeI(4));
            Assert.AreEqual(0, sigR.MomList.RealI(4));
            Assert.IsNull(sigR.MomList.ErrorI(4));
            Assert.AreEqual(RTime(11), sigR.MomList.TimeI(5));
            Assert.AreEqual(-1.5, sigR.MomList.RealI(5));
            Assert.IsNull(sigR.MomList.ErrorI(5));

            Assert.AreEqual(RTime(7), sigS.MomList.TimeI(0));
            Assert.AreEqual("1,5s", sigS.MomList.StringI(0));
            Assert.IsNotNull(sigS.MomList.ErrorI(0));
            Assert.AreEqual(ErrQuality.Warning, sigS.MomList.ErrorI(0).Quality);
            Assert.AreEqual(RTime(7, 30), sigS.MomList.TimeI(1));
            Assert.AreEqual("2,5s", sigS.MomList.StringI(1));
            Assert.IsNull(sigS.MomList.ErrorI(1));
            Assert.AreEqual(RTime(8), sigS.MomList.TimeI(2));
            Assert.AreEqual("2,5s", sigS.MomList.StringI(1));
            Assert.IsNull(sigS.MomList.ErrorI(2));
            Assert.AreEqual(RTime(8, 30), sigS.MomList.TimeI(3));
            Assert.AreEqual("0s", sigS.MomList.StringI(3));
            Assert.IsNull(sigS.MomList.ErrorI(3));
            Assert.AreEqual(RTime(10), sigS.MomList.TimeI(4));
            Assert.AreEqual("0s", sigS.MomList.StringI(4));
            Assert.IsNull(sigS.MomList.ErrorI(4));
            Assert.AreEqual(RTime(11), sigS.MomList.TimeI(5));
            Assert.AreEqual("-1,5s", sigS.MomList.StringI(5));
            Assert.IsNull(sigS.MomList.ErrorI(5));

            Assert.AreEqual(RTime(0), sigX.MomList.TimeI(0));
            Assert.AreEqual(1, sigX.MomList.RealI(0));
            Assert.IsNull(sigX.MomList.ErrorI(0));

            vc = GetValues(connect, RTime(12), RTime(15, 30));
            Assert.AreEqual(4, sigR.MomList.Count);
            Assert.AreEqual(4, sigS.MomList.Count);
            Assert.AreEqual(4, sigI.MomList.Count);
            Assert.AreEqual(2, sigX.MomList.Count);
            Assert.AreEqual(0, sigT.MomList.Count);
            Assert.AreEqual(0, sigN.MomList.Count);
            Assert.IsTrue(source.Outs["Ob1"].HasBegin);
            Assert.IsTrue(source.Outs["Ob2"].HasBegin);
            Assert.IsTrue(source.Outs2["ObX"].HasBegin);
            Assert.AreEqual(7, vc.ReadCount);
            Assert.AreEqual(14, vc.WriteCount);
            Assert.AreEqual(VcStatus.Success, vc.Status);

            Assert.AreEqual(RTime(11), sigI.MomList.TimeI(0));
            Assert.AreEqual(1, sigI.MomList.IntegerI(0));
            Assert.AreEqual(RTime(13), sigI.MomList.TimeI(1));
            Assert.AreEqual(0, sigI.MomList.IntegerI(1));
            Assert.AreEqual(RTime(14), sigI.MomList.TimeI(2));
            Assert.AreEqual(0, sigI.MomList.IntegerI(2));
            Assert.AreEqual(RTime(15, 30), sigI.MomList.TimeI(3));
            Assert.AreEqual(0, sigI.MomList.IntegerI(3));

            Assert.AreEqual(RTime(11), sigR.MomList.TimeI(0));
            Assert.AreEqual(-1.5, sigR.MomList.RealI(0));
            Assert.IsNull(sigR.MomList.ErrorI(0));
            Assert.AreEqual(RTime(13), sigR.MomList.TimeI(1));
            Assert.AreEqual(4, sigR.MomList.RealI(1));
            Assert.IsNotNull(sigR.MomList.ErrorI(1));
            Assert.AreEqual(ErrQuality.Error, sigR.MomList.ErrorI(1).Quality);
            Assert.AreEqual(RTime(14), sigR.MomList.TimeI(2));
            Assert.AreEqual(4, sigR.MomList.RealI(2));
            Assert.IsNull(sigR.MomList.ErrorI(2));
            Assert.AreEqual(RTime(15, 30), sigR.MomList.TimeI(3));
            Assert.AreEqual(4.5, sigR.MomList.RealI(3));
            Assert.IsNull(sigR.MomList.ErrorI(3));

            Assert.AreEqual(RTime(11), sigS.MomList.TimeI(0));
            Assert.AreEqual("-1,5s", sigS.MomList.StringI(0));
            Assert.IsNull(sigS.MomList.ErrorI(0));
            Assert.AreEqual(RTime(13), sigS.MomList.TimeI(1));
            Assert.AreEqual("4s", sigS.MomList.StringI(1));
            Assert.IsNotNull(sigS.MomList.ErrorI(1));
            Assert.AreEqual(ErrQuality.Error, sigS.MomList.ErrorI(1).Quality);
            Assert.AreEqual(RTime(14), sigS.MomList.TimeI(2));
            Assert.AreEqual("4s", sigS.MomList.StringI(2));
            Assert.IsNull(sigS.MomList.ErrorI(2));
            Assert.AreEqual(RTime(15, 30), sigS.MomList.TimeI(3));
            Assert.AreEqual("4,5s", sigS.MomList.StringI(3));
            Assert.IsNull(sigS.MomList.ErrorI(3));

            Assert.AreEqual(RTime(0), sigX.MomList.TimeI(0));
            Assert.AreEqual(1, sigX.MomList.RealI(0));
            Assert.IsNull(sigX.MomList.ErrorI(0));
            Assert.AreEqual(RTime(15), sigX.MomList.TimeI(1));
            Assert.AreEqual(2, sigX.MomList.RealI(1));
            Assert.IsNull(sigX.MomList.ErrorI(1));
        }

        [TestMethod]
        public void Recursive()
        {
            var con = MakeFictiveConnect("Recursive");
            var sigR = (UniformSignal)con.AddInitialSignal("Ob1.RealSignal", DataType.Real, "Table=MomValues;ObjectCode=Ob1;NumObject=1", "", "Signal=Real");
            var sigI = (UniformSignal)con.AddInitialSignal("Ob2.IntSignal", DataType.Integer, "Table=MomValues;ObjectCode=Ob2;NumObject=2;IsErrorObject=True", "", "Signal=Int");
            Assert.AreEqual(2, con.Signals.Count);
            Assert.AreEqual(2, con.InitialSignals.Count);

            var vc = GetValues(con, RTime(0), RTime(2));
            Assert.AreEqual(VcStatus.Partial, vc.Status);
            Assert.AreEqual(3, vc.ReadCount);
            Assert.AreEqual(3, vc.WriteCount);

            Assert.AreEqual(3, sigR.MomList.Count);
            Assert.AreEqual(0, sigI.MomList.Count);
            Assert.AreEqual(RTime(0), sigR.MomList.TimeI(0));
            Assert.AreEqual(3, sigR.MomList.RealI(0));
            Assert.AreEqual(RTime(1, 30), sigR.MomList.TimeI(1));
            Assert.AreEqual(-2, sigR.MomList.RealI(1));
            Assert.AreEqual(RTime(2), sigR.MomList.TimeI(2));
            Assert.AreEqual(-2, sigR.MomList.RealI(2));

            con.ClearSignals();
            Assert.AreEqual(0, con.Signals.Count);
            Assert.AreEqual(0, con.InitialSignals.Count);
            con.AddInitialSignal("Ob2.IntSignal", DataType.Integer, "Table=MomValues;ObjectCode=Ob2;NumObject=2;IsErrorObject=True", "", "Signal=Int");
            con.AddInitialSignal("Ob3.RealSignal", DataType.Boolean, "Table=MomValues;ObjectCode=Ob3;NumObject=3;IsErrorObject=True", "", "Signal=Bool");
            con.AddInitialSignal("Ob4.IntSignal", DataType.String, "Table=MomValues;ObjectCode=Ob4;NumObject=4;IsErrorObject=True", "", "Signal=String");

            vc = GetValues(con, RTime(0), RTime(2));
            Assert.AreEqual(VcStatus.NoSuccess, vc.Status);
            Assert.AreEqual(0, vc.ReadCount);
            Assert.AreEqual(0, vc.WriteCount);

            con.ClearSignals();
            sigR = (UniformSignal)con.AddInitialSignal("Ob1.RealSignal", DataType.Real, "Table=MomValues;ObjectCode=Ob1;NumObject=1:", "", "Signal=Real");
            sigI = (UniformSignal)con.AddInitialSignal("Ob2.IntSignal", DataType.Integer, "Table=MomValues;ObjectCode=Ob2;NumObject=2;IsErrorObject=True", "", "Signal=Int");
            var sigB = (UniformSignal)con.AddInitialSignal("Ob3.RealSignal", DataType.Boolean, "Table=MomValues;ObjectCode=Ob3;NumObject=3", "", "Signal=Bool");
            var sigS = (UniformSignal)con.AddInitialSignal("Ob4.IntSignal", DataType.String, "Table=MomValues;ObjectCode=Ob4;NumObject=4;IsErrorObject=True", "", "Signal=String");

            vc = GetValues(con, RTime(0), RTime(2));
            Assert.AreEqual(VcStatus.Partial, vc.Status);
            Assert.AreEqual(6, vc.ReadCount);
            Assert.AreEqual(6, vc.WriteCount);

            Assert.AreEqual(3, sigR.MomList.Count);
            Assert.AreEqual(0, sigI.MomList.Count);
            Assert.AreEqual(3, sigB.MomList.Count);
            Assert.AreEqual(0, sigS.MomList.Count);
            Assert.AreEqual(RTime(0), sigR.MomList.TimeI(0));
            Assert.AreEqual(3, sigR.MomList.RealI(0));
            Assert.AreEqual(RTime(1, 30), sigR.MomList.TimeI(1));
            Assert.AreEqual(-2, sigR.MomList.RealI(1));
            Assert.AreEqual(RTime(2), sigR.MomList.TimeI(2));
            Assert.AreEqual(-2, sigR.MomList.RealI(2));
            Assert.AreEqual(RTime(0), sigB.MomList.TimeI(0));
            Assert.AreEqual(false, sigB.MomList.BooleanI(0));
            Assert.AreEqual(RTime(1, 30), sigB.MomList.TimeI(1));
            Assert.AreEqual(false, sigB.MomList.BooleanI(1));
            Assert.AreEqual(RTime(2), sigB.MomList.TimeI(2));
            Assert.AreEqual(true, sigB.MomList.BooleanI(2));
        }
    }
}