using System;
using BaseLibrary;
using BaseLibraryTest;
using CommonTypes;
using Fictive;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProvidersLibrary;

namespace ProvidersLibraryTest
{
    [TestClass]
    public class FictiveTest
    {
        private SourceConnect MakeFictiveConnect(string prefix, bool makeReserve = false)
        {
            var factory = new ProvidersFactory();
            var connect = (SourceConnect)factory.CreateConnect(ProviderType.Source, "TestSource", "Fictive", new Logger());
            TestLib.CopyFile("ProvidersLibrary", "Fictive.accdb", "Fictive" + prefix + ".accdb");
            var source = (FictiveSource)factory.CreateProvider("FictiveSource", @"DbFile=" + TestLib.TestRunDir + @"ProvidersLibrary\Fictive" + prefix + ".accdb");
            FictiveSource source2 = null;
            if (makeReserve)
                source2 = (FictiveSource)factory.CreateProvider("FictiveSource", @"DbFile=" + TestLib.TestRunDir + @"ProvidersLibrary\Fictive" + prefix + ".accdb");
            connect.JoinProviders(source, source2);
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
            var connect = MakeFictiveConnect("Signals");
            DateTime beg = RTime(1), en = RTime(2);

            var source = (FictiveSource)connect.Provider;
            Assert.AreEqual("TestSource", connect.Name);
            Assert.AreEqual("Fictive", connect.Complect);
            Assert.AreEqual("Источник: TestSource", connect.Context);
            Assert.IsNotNull(source);
            Assert.AreEqual("FictiveSource", source.Code);
            Assert.AreEqual("Источник: TestSource, FictiveSource", source.Context);

            connect.AddInitialSignal("Ob1.StateSignal", "Ob1", DataType.Integer, "Table=MomValues;NumObject=1;Signal=State", true);
            connect.AddInitialSignal("Ob1.ValueSignal", "Ob1", DataType.Real, "Table=MomValues;NumObject=1;Signal=Value", true);
            connect.AddInitialSignal("Ob1.BoolSignal", "Ob1", DataType.Boolean, "Table=MomValues;NumObject=1;Signal=Bool", true);
            connect.AddInitialSignal("Ob1.IntSignal", "Ob1", DataType.Integer, "Table=MomValues;NumObject=1;Signal=Int", true);
            connect.AddInitialSignal("Ob1.RealSignal", "Ob1", DataType.Real, "Table=MomValues;NumObject=1;Signal=Real", true);
            connect.AddInitialSignal("Ob1.TimeSignal", "Ob1", DataType.Time, "Table=MomValues;NumObject=1;Signal=Time", true);
            connect.AddInitialSignal("Ob1.StringSignal", "Ob1", DataType.String, "Table=MomValues;NumObject=1;Signal=String", true);
            Assert.AreEqual(7, connect.Signals.Count);
            Assert.AreEqual(7, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);
            Assert.IsTrue(connect.Signals["Ob1.StateSignal"] is UniformSignal);
            Assert.AreEqual("Ob1.StateSignal", connect.Signals["Ob1.StateSignal"].Code);
            Assert.AreEqual(DataType.Integer, connect.Signals["Ob1.StateSignal"].DataType);
            Assert.AreEqual("MomValues", connect.Signals["Ob1.StateSignal"].Inf["Table"]);
            Assert.AreEqual("1", connect.Signals["Ob1.StateSignal"].Inf["NumObject"]);
            Assert.AreEqual("State", connect.Signals["Ob1.StateSignal"].Inf["Signal"]);
            Assert.AreEqual(0, connect.Signals["Ob1.StateSignal"].MomList.Count);

            connect.AddInitialSignal("Ob2.StateSignal", "Ob2", DataType.Integer, "Table=MomValues;NumObject=2;Signal=State", true);
            connect.AddInitialSignal("Ob2.ValueSignal", "Ob2", DataType.Real, "Table=MomValues;NumObject=2;Signal=Value", true);
            connect.AddInitialSignal("Ob2.BoolSignal", "Ob2", DataType.Boolean, "Table=MomValues;NumObject=2;Signal=Bool", true);
            connect.AddInitialSignal("Ob2.IntSignal", "Ob2", DataType.Integer, "Table=MomValues;NumObject=2;Signal=Int", true);
            connect.AddInitialSignal("Ob2.RealSignal", "Ob2", DataType.Real, "Table=MomValues;NumObject=2;Signal=Real", true);
            connect.AddInitialSignal("Ob2.TimeSignal", "Ob2", DataType.Time, "Table=MomValues;NumObject=2;Signal=Time", true);
            connect.AddInitialSignal("Ob2.StringSignal", "Ob2", DataType.String, "Table=MomValues;NumObject=2;Signal=String", true);
            Assert.AreEqual(14, connect.Signals.Count);
            Assert.AreEqual(14, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);

            connect.AddInitialSignal("Ob3.StateSignal", "Ob3", DataType.Integer, "Table=MomValues;NumObject=3;Signal=State", true);
            connect.AddInitialSignal("Ob3.ValueSignal", "Ob3", DataType.Real, "Table=MomValues;NumObject=3;Signal=Value", true);
            connect.AddInitialSignal("Ob3.BoolSignal", "Ob3", DataType.Boolean, "Table=MomValues;NumObject=3;Signal=Bool", true);
            connect.AddInitialSignal("Ob3.IntSignal", "Ob3", DataType.Integer, "Table=MomValues;NumObject=3;Signal=Int", true);
            connect.AddInitialSignal("Ob3.RealSignal", "Ob3", DataType.Real, "Table=MomValues;NumObject=3;Signal=Real", true);
            connect.AddInitialSignal("Ob3.TimeSignal", "Ob3", DataType.Time, "Table=MomValues;NumObject=3;Signal=Time", true);
            connect.AddInitialSignal("Ob3.StringSignal", "Ob3", DataType.String, "Table=MomValues;NumObject=3;Signal=String", true);
            Assert.AreEqual(21, connect.Signals.Count);
            Assert.AreEqual(21, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);

            connect.AddInitialSignal("ObX.ValueSignal", "ObX", DataType.Real, "Table=MomValues2;NumObject=5;Signal=Value", true);
            connect.AddInitialSignal("ObX.Value2Signal", "ObX", DataType.Real, "Table=MomValues2;NumObject=5;Signal=Value2", true);
            Assert.AreEqual(23, connect.Signals.Count);
            Assert.AreEqual(23, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);

            connect.AddInitialSignal("ObY.ValueSignal", "ObY", DataType.Real, "Table=MomValues2;NumObject=6;Signal=Value", true);
            connect.AddInitialSignal("ObY.Value2Signal", "ObY", DataType.Real, "Table=MomValues2;NumObject=6;Signal=Value2", true);
            Assert.AreEqual(25, connect.Signals.Count);
            Assert.AreEqual(25, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);

            connect.AddInitialSignal("ObZ.ValueSignal", "ObZ", DataType.Real, "Table=MomValues2;NumObject=7;Signal=Value", true);
            connect.AddInitialSignal("ObZ.Value2Signal", "ObZ", DataType.Real, "Table=MomValues2;NumObject=7;Signal=Value2", true);
            Assert.AreEqual(27, connect.Signals.Count);
            Assert.AreEqual(27, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);

            connect.AddInitialSignal("Operator.CommandText", "Operator", DataType.String, "Table=MomOperator;NumObject=8;Signal=CommandText", false);
            connect.AddInitialSignal("Operator.CommandNumber", "Operator", DataType.Integer, "Table=MomOperator;NumObject=8;Signal=CommandNumber", false);
            Assert.AreEqual(29, connect.Signals.Count);
            Assert.AreEqual(29, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);

            GetValues(connect, beg, en);
            Assert.AreEqual(3, source.Outs.Count);
            Assert.AreEqual(3, source.OutsId.Count);
            Assert.AreEqual(3, source.Outs2.Count);
            Assert.AreEqual(3, source.OutsId2.Count);
            Assert.IsNotNull(source.OperatorOut);

            Assert.IsTrue(source.OutsId.ContainsKey(11));
            Assert.IsTrue(source.OutsId.ContainsKey(12));
            Assert.IsTrue(source.OutsId.ContainsKey(13));
            Assert.IsTrue(source.Outs.ContainsKey("Ob1"));
            Assert.IsTrue(source.Outs.ContainsKey("Ob2"));
            Assert.IsTrue(source.Outs.ContainsKey("Ob3"));
            Assert.IsTrue(source.OutsId2.ContainsKey(15));
            Assert.IsTrue(source.OutsId2.ContainsKey(16));
            Assert.IsTrue(source.OutsId2.ContainsKey(17));
            Assert.IsTrue(source.Outs2.ContainsKey("ObX"));
            Assert.IsTrue(source.Outs2.ContainsKey("ObY"));
            Assert.IsTrue(source.Outs2.ContainsKey("ObZ"));
            Assert.IsNotNull(source.OperatorOut);
            Assert.IsNotNull(source.ErrPool);

            var ti = connect.GetTime();
            Assert.AreEqual(RTime(0), ti.Begin);
            Assert.AreEqual(RTime(30), ti.End);
            ti = connect.GetTime();
            Assert.AreEqual(RTime(0), ti.Begin);
            Assert.AreEqual(RTime(30), ti.End);

            Assert.IsTrue(source.Connect());
            Assert.IsTrue(source.Reconnect());
            source.Disconnect();
            Assert.IsTrue(source.Reconnect());
            source.Disconnect();
            Assert.IsTrue(source.Connect());
            source.Dispose();
        }

        [TestMethod]
        public void ReadByParts()
        {
            var connect = MakeFictiveConnect("ByParts");
            var sig1 = connect.AddInitialSignal("Ob1.RealSignal", "Ob1", DataType.Real, "Table=MomValues;NumObject=1;Signal=Real", true);
            var sig2 = connect.AddInitialSignal("Ob2.BoolSignal", "Ob2", DataType.Boolean, "Table=MomValues;NumObject=2;Signal=Bool", true);
            var sig3 = connect.AddInitialSignal("Ob3.IntSignal", "Ob3", DataType.Integer, "Table=MomValues;NumObject=3;Signal=Int", true);
            var sig4 = connect.AddInitialSignal("Ob4.TimeSignal", "Ob4", DataType.Time, "Table=MomValues;NumObject=4;Signal=Time", true);
            var sig5 = connect.AddInitialSignal("ObX.ValueSignal", "ObX", DataType.Real, "Table=MomValues2;NumObject=5;Signal=Value", true);
            var sig6 = connect.AddInitialSignal("ObY.ValueSignal", "ObY", DataType.Real, "Table=MomValues2;NumObject=6;Signal=Value", true);
            var sig7 = connect.AddInitialSignal("ObZ.ValueSignal2", "ObZ", DataType.Real, "Table=MomValues2;NumObject=7;Signal=Value2", true);
            var sig8 = connect.AddInitialSignal("Operator.CommandText", "Operator", DataType.String, "Table=MomOperator;NumObject=8;Signal=CommandText", false);
            var sig9 = connect.AddInitialSignal("Operator.CommandNumber", "Operator", DataType.Integer, "Table=MomOperator;NumObject=8;Signal=CommandNumber", false);
            
            Assert.AreEqual(9, connect.Signals.Count);
            Assert.AreEqual(9, connect.InitialSignals.Count);
            Assert.AreEqual("Ob1.RealSignal", sig1.Code);
            Assert.AreEqual("Ob1", sig1.CodeOuts);
            Assert.AreEqual("Ob2.BoolSignal", sig2.Code);
            Assert.AreEqual("Ob2", sig2.CodeOuts);
            Assert.AreEqual("Ob3.IntSignal", sig3.Code);
            Assert.AreEqual("Ob3", sig3.CodeOuts);
            Assert.AreEqual("Ob4.TimeSignal", sig4.Code);
            Assert.AreEqual("Ob4", sig4.CodeOuts);
            Assert.AreEqual("ObX.ValueSignal", sig5.Code);
            Assert.AreEqual("ObX", sig5.CodeOuts);
            Assert.AreEqual("ObY.ValueSignal", sig6.Code);
            Assert.AreEqual("ObY", sig6.CodeOuts);
            Assert.AreEqual("ObZ.ValueSignal2", sig7.Code);
            Assert.AreEqual("ObZ", sig7.CodeOuts);
            Assert.AreEqual("Operator.CommandText", sig8.Code);
            Assert.AreEqual("Operator", sig8.CodeOuts);
            Assert.AreEqual("Operator.CommandNumber", sig9.Code);
            Assert.AreEqual("Operator", sig9.CodeOuts);

            GetValues(connect, RTime(0), RTime(10));
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

            var source = (FictiveSource)connect.Provider;
            Assert.AreEqual(4, source.Outs.Count);
            Assert.AreEqual(4, source.OutsId.Count);
            Assert.AreEqual(3, source.Outs2.Count);
            Assert.AreEqual(3, source.OutsId2.Count);
            Assert.IsNotNull(source.OperatorOut);
            connect.GetValues();
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
            var sigR = (UniformSignal)connect.AddInitialSignal("Ob1.RealSignal", "Ob1", DataType.Real, "Table=MomValues;NumObject=1;Signal=Real", true);
            var sigS = (UniformSignal)connect.AddInitialSignal("Ob2.StringSignal", "Ob2", DataType.String, "Table=MomValues;NumObject=2;Signal=String", true);
            var sigI = (UniformSignal)connect.AddInitialSignal("Ob2.IntSignal", "Ob2", DataType.Integer, "Table=MomValues;NumObject=2;Signal=Int", true);
            var sigX = (UniformSignal)connect.AddInitialSignal("ObX.ValueSignal", "ObX", DataType.Real, "Table=MomValues2;NumObject=5;Signal=Value", true);
            var sigT = connect.AddInitialSignal("Operator.CommandText", "Operator", DataType.String, "Table=MomOperator;NumObject=8;Signal=CommandText", false);
            var sigN = connect.AddInitialSignal("Operator.CommandNumber", "Operator", DataType.Integer, "Table=MomOperator;NumObject=8;Signal=CommandNumber", false);
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
            var connect = MakeFictiveConnect("Recursive");
            var sigR = (UniformSignal)connect.AddInitialSignal("Ob1.RealSignal", "Ob1", DataType.Real, "Table=MomValues;NumObject=1;Signal=Real", true);
            var sigI = (UniformSignal)connect.AddInitialSignal("Ob2.IntSignal", "Ob2", DataType.Integer, "Table=MomValues;NumObject=2;Signal=Int;IsErrorObject=True", true);
            Assert.AreEqual(2, connect.Signals.Count);
            Assert.AreEqual(2, connect.InitialSignals.Count);

            var vc = GetValues(connect, RTime(0), RTime(2));
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

            connect.ClearSignals();
            Assert.AreEqual(0, connect.Signals.Count);
            Assert.AreEqual(0, connect.InitialSignals.Count);
            connect.AddInitialSignal("Ob1.RealSignal", "Ob1", DataType.Real, "Table=MomValues;NumObject=1;Signal=Real;IsErrorObject=True", true);
            connect.AddInitialSignal("Ob2.IntSignal", "Ob2", DataType.Integer, "Table=MomValues;NumObject=2;Signal=Int;IsErrorObject=True", true);
            connect.AddInitialSignal("Ob3.RealSignal", "Ob3", DataType.Boolean, "Table=MomValues;NumObject=3;Signal=Bool;IsErrorObject=True", true);
            connect.AddInitialSignal("Ob4.IntSignal", "Ob4", DataType.String, "Table=MomValues;NumObject=4;Signal=String;IsErrorObject=True", true);

            vc = GetValues(connect, RTime(0), RTime(2));
            Assert.AreEqual(VcStatus.NoSuccess, vc.Status);
            Assert.AreEqual(0, vc.ReadCount);
            Assert.AreEqual(0, vc.WriteCount);

            connect.ClearSignals();
            sigR = (UniformSignal)connect.AddInitialSignal("Ob1.RealSignal", "Ob1", DataType.Real, "Table=MomValues;NumObject=1;Signal=Real", true);
            sigI = (UniformSignal)connect.AddInitialSignal("Ob2.IntSignal", "Ob2", DataType.Integer, "Table=MomValues;NumObject=2;Signal=Int;IsErrorObject=True", true);
            var sigB = (UniformSignal)connect.AddInitialSignal("Ob3.RealSignal", "Ob3", DataType.Boolean, "Table=MomValues;NumObject=3;Signal=Bool", true);
            var sigS = (UniformSignal)connect.AddInitialSignal("Ob4.IntSignal", "Ob4", DataType.String, "Table=MomValues;NumObject=4;Signal=String;IsErrorObject=True", true);

            vc = GetValues(connect, RTime(0), RTime(2));
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