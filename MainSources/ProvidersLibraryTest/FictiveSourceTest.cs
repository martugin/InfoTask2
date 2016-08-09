using System;
using BaseLibrary;
using CommonTypes;
using Fictive;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProvidersLibrary;

namespace ProvidersLibraryTest
{
    [TestClass]
    public class FictiveSourceTest
    {
        private SourceConnect MakeFictiveConnect()
        {
            var factory = new ProvidersFactory();
            var connect = (SourceConnect)factory.CreateConnect(ProviderType.Source, "TestSource", "Fictive", new Logger());
            var source = (FictiveSimpleSource)factory.CreateProvider("FictiveSimpleSource", "");
            connect.JoinProviders(source);
            return connect;
        }

        [TestMethod]
        public void Signals()
        {
            SourceConnect connect = MakeFictiveConnect();
            var source = (FictiveSimpleSource)connect.Provider;
            Assert.AreEqual(0, connect.Signals.Count);
            var sig1 = connect.AddInitialSignal("Ob.Int", "Ob", DataType.Integer, "NumObject=1;Signal=Int;ValuesInterval=2000", true);
            Assert.AreEqual(1, connect.Signals.Count);
            Assert.AreEqual(1, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);
            
            Assert.AreEqual(DataType.Integer, sig1.DataType);
            Assert.AreEqual(3, sig1.Inf.Count);
            Assert.AreEqual("1", sig1.Inf["NumObject"]);
            Assert.AreEqual("Int", sig1.Inf["Signal"]);
            Assert.AreEqual("2000", sig1.Inf["ValuesInterval"]);
            Assert.AreEqual("TestSource", sig1.Connect.Name);

            connect.Prepare();
            Assert.AreEqual(1, source.Objects.Count);
            Assert.IsTrue(source.Objects.ContainsKey(1));
            ObjectFictive ob1 = source.Objects[1];
            Assert.AreEqual("Ob", ob1.Context);
            Assert.IsNotNull(ob1.IntSignal);
            Assert.IsNull(ob1.RealSignal);
            Assert.IsNull(ob1.ValueSignal);
            Assert.IsTrue(ob1.IsInitialized);
            Assert.AreEqual(2000, ob1.ValuesInterval);
            
            var sig2 = connect.AddInitialSignal("Ob.Value", "Ob", DataType.Real, "NumObject=1;Signal=Value;ValuesInterval=2000", true);
            Assert.AreEqual(2, connect.Signals.Count);
            Assert.AreEqual(2, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);

            Assert.AreEqual(DataType.Real, sig2.DataType);
            Assert.AreEqual(3, sig2.Inf.Count);
            Assert.AreEqual("1", sig2.Inf["NumObject"]);
            Assert.AreEqual("Value", sig2.Inf["Signal"]);
            Assert.AreEqual("2000", sig2.Inf["ValuesInterval"]);

            var sig3 = connect.AddCalcSignal("Ob.Bit3", "Ob", "Ob.Int", "Bit;1;3");
            Assert.AreEqual(3, connect.Signals.Count);
            Assert.AreEqual(2, connect.InitialSignals.Count);
            Assert.AreEqual(1, connect.CalcSignals.Count);

            Assert.AreEqual(DataType.Boolean, sig3.DataType);
            Assert.IsNull(sig3.Inf);
            Assert.AreEqual("Bit", sig3.Calculate.Method.Name);
            Assert.AreEqual("TestSource", sig3.Connect.Name);

            connect.AddInitialSignal("Ob2.Int", "Ob2", DataType.Integer, "NumObject=2;Signal=Int;ValuesInterval=1000", true);
            var sig4 = connect.AddCalcSignal("Ob2.Bit4or5", "Ob2", "Ob2.Int", "BitOr;2;4;5");
            Assert.AreEqual(5, connect.Signals.Count);
            Assert.AreEqual(3, connect.InitialSignals.Count);
            Assert.AreEqual(2, connect.CalcSignals.Count);

            Assert.AreEqual(DataType.Boolean, sig4.DataType);
            Assert.IsNull(sig4.Inf);
            Assert.AreEqual("BitOr", sig4.Calculate.Method.Name);
            Assert.AreEqual("Ob2.Bit4or5", sig4.Code);
            Assert.AreEqual("TestSource", sig4.Connect.Name);

            connect.Prepare();
            Assert.AreEqual(2, source.Objects.Count);
            Assert.IsTrue(source.Objects.ContainsKey(2));
            ObjectFictive ob2 = source.Objects[2];
            Assert.AreEqual("Ob2", ob2.Context);
            Assert.IsNotNull(ob2.IntSignal);
            Assert.IsNull(ob2.ValueSignal);
            Assert.IsTrue(ob2.IsInitialized);
            Assert.AreEqual(1000, ob2.ValuesInterval);

            Assert.AreEqual(new DateTime(2000, 1, 1), source.GetTime().Begin);
            Assert.AreEqual(new DateTime(2100, 1, 1), source.GetTime().End);
            Assert.AreEqual(new DateTime(2000, 1, 1), connect.GetTime().Begin);
            Assert.AreEqual(new DateTime(2100, 1, 1), connect.GetTime().End);
            Assert.IsTrue(ob1.IsInitialized);
            Assert.IsTrue(ob2.IsInitialized);

            connect.ClearSignals();
            Assert.AreEqual(0, connect.Signals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);
            Assert.AreEqual(0, connect.InitialSignals.Count);
        }

        [TestMethod]
        public void InitialValues()
        {
            SourceConnect connect = MakeFictiveConnect();
            var source = (FictiveSimpleSource)connect.Provider;
            source.Prepare();
            var beg = new DateTime(2007, 11, 20, 10, 30, 0);
            var en = new DateTime(2007, 11, 20, 10, 40, 0);
            connect.GetValues(beg, en);
            Assert.AreEqual(0, connect.Signals.Count);

            var sigi = connect.AddInitialSignal("Ob.Int", "Ob", DataType.Integer, "NumObject=1;Signal=Int;ValuesInterval=1000", true);
            var sigr = connect.AddInitialSignal("Ob.Real", "Ob", DataType.Real, "NumObject=1;Signal=Real;ValuesInterval=1000", true);
            Assert.AreEqual(2, connect.Signals.Count);
            connect.Prepare();
            connect.GetValues(beg, en);
            Assert.AreEqual(601, sigi.MomList.Count);
            Assert.AreEqual(beg, sigi.MomList.Time(0));
            Assert.AreEqual(0, sigi.MomList.Integer(0));
            Assert.AreEqual(0.0, sigi.MomList.Real(0));
            Assert.IsNull(sigi.MomList.Error(0));
            Assert.AreEqual(beg.AddSeconds(1), sigi.MomList.Time(1));
            Assert.AreEqual(1, sigi.MomList.Integer(1));
            Assert.AreEqual(true, sigi.MomList.Boolean(1));
            Assert.IsNull(sigi.MomList.Error(1));
            Assert.AreEqual(beg.AddSeconds(10), sigi.MomList.Time(10));
            Assert.AreEqual(10, sigi.MomList.Integer(10));
            Assert.AreEqual(10.0, sigi.MomList.Real(10));
            Assert.IsNull(sigi.MomList.Error(10));
            Assert.AreEqual(beg.AddSeconds(599), sigi.MomList.Time(599));
            Assert.AreEqual(599, sigi.MomList.Integer(599));
            Assert.AreEqual("599", sigi.MomList.String(599));
            Assert.IsNull(sigi.MomList.Error(599));

            Assert.AreEqual(601, sigr.MomList.Count);
            Assert.AreEqual(beg, sigr.MomList.Time(0));
            Assert.AreEqual(0.0, sigr.MomList.Real(0));
            Assert.IsNull(sigr.MomList.Error(0));
            Assert.AreEqual(beg.AddSeconds(1), sigr.MomList.Time(1));
            Assert.AreEqual(0.5, sigr.MomList.Real(1));
            Assert.IsNull(sigr.MomList.Error(1));
            Assert.AreEqual(beg.AddSeconds(10), sigr.MomList.Time(10));
            Assert.AreEqual(5.0, sigr.MomList.Real(10));
            Assert.IsNull(sigr.MomList.Error(10));
            Assert.AreEqual(beg.AddSeconds(599), sigr.MomList.Time(599));
            Assert.AreEqual(299.5, sigr.MomList.Real(599));
            Assert.AreEqual("299,5", sigr.MomList.String(599));
            Assert.IsNull(sigr.MomList.Error(599));
        }

        [TestMethod]
        public void CalcValuesFull()
        {
            SourceConnect connect = MakeFictiveConnect();
            var source = (FictiveSimpleSource)connect.Provider;
            connect.AddInitialSignal("Ob.Int", "Ob", DataType.Integer, "NumObject=1;Signal=Int;ValuesInterval=1000", true);
            connect.AddInitialSignal("Ob.Real", "Ob", DataType.Real, "NumObject=1;Signal=Real;ValuesInterval=1000", true);
            Assert.AreEqual(2, connect.Signals.Count);
            Assert.AreEqual(2, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);
            
            var sigBit = connect.AddCalcSignal("Ob.Bit2", "Ob", "Ob.Int", "Bit;1;2");
            var sigBitOr = connect.AddCalcSignal("Ob.Bit1Or2", "Ob", "Ob.Int", "BitOr;2;1;2");
            var sigBitAnd = connect.AddCalcSignal("Ob.Bit1And2", "Ob", "Ob.Int", "BitAnd;2;1;2");
            var sigFirst = connect.AddCalcSignal("Ob.First", "Ob", "Ob.Real", "First;1;5");
            var sigLast = connect.AddCalcSignal("Ob.Last", "Ob", "Ob.Real", "Last;2;5;1");
            var sigMin = connect.AddCalcSignal("Ob.Min", "Ob", "Ob.Real", "Min;1;5");
            var sigMax = connect.AddCalcSignal("Ob.Max", "Ob", "Ob.Real", "Max;2;5;1");
            var sigAverage = connect.AddCalcSignal("Ob.Average", "Ob", "Ob.Real", "Average;2;5;0.5");

            connect.Prepare();
            Assert.AreEqual(1, source.Objects.Count);
            var beg = new DateTime(2007, 11, 20, 10, 30, 0);
            var en = new DateTime(2007, 11, 20, 10, 40, 0);
            connect.GetValues(beg, en);

            //Bit
            Assert.AreEqual(601, sigBit.MomList.Count);
            Assert.AreEqual(beg, sigBit.MomList.Time(0));
            Assert.AreEqual(0, sigBit.MomList.Integer(0));
            Assert.AreEqual(0.0, sigBit.MomList.Real(0));
            Assert.IsNull(sigBit.MomList.Error(0));
            Assert.AreEqual(beg.AddSeconds(5), sigBit.MomList.Time(5));
            Assert.AreEqual(1, sigBit.MomList.Integer(5));
            Assert.AreEqual(true, sigBit.MomList.Boolean(5));
            Assert.IsNull(sigBit.MomList.Error(5));
            Assert.AreEqual(beg.AddSeconds(10), sigBit.MomList.Time(10));
            Assert.AreEqual(false, sigBit.MomList.Boolean(10));
            Assert.IsNull(sigBit.MomList.Error(10));
            Assert.AreEqual(beg.AddSeconds(599), sigBit.MomList.Time(599));
            Assert.AreEqual(true, sigBit.MomList.Boolean(599));
            Assert.AreEqual("1", sigBit.MomList.String(599));
            Assert.IsNull(sigBit.MomList.Error(599));

            //BitOr
            Assert.AreEqual(601, sigBitOr.MomList.Count);
            Assert.AreEqual(beg, sigBitOr.MomList.Time(0));
            Assert.AreEqual(false, sigBitOr.MomList.Boolean(0));
            Assert.AreEqual(0.0, sigBitOr.MomList.Real(0));
            Assert.IsNull(sigBitOr.MomList.Error(0));
            Assert.AreEqual(beg.AddSeconds(5), sigBitOr.MomList.Time(5));
            Assert.AreEqual(1, sigBitOr.MomList.Integer(5));
            Assert.AreEqual(true, sigBitOr.MomList.Boolean(5));
            Assert.IsNull(sigBitOr.MomList.Error(5));
            Assert.AreEqual(beg.AddSeconds(10), sigBitOr.MomList.Time(10));
            Assert.AreEqual(true, sigBitOr.MomList.Boolean(10));
            Assert.IsNull(sigBitOr.MomList.Error(10));
            Assert.AreEqual(beg.AddSeconds(592), sigBitOr.MomList.Time(592));
            Assert.AreEqual(false, sigBitOr.MomList.Boolean(592));
            Assert.AreEqual("0", sigBitOr.MomList.String(592));
            Assert.IsNull(sigBitOr.MomList.Error(592));

            //BitAnd
            Assert.AreEqual(601, sigBitAnd.MomList.Count);
            Assert.AreEqual(beg, sigBitAnd.MomList.Time(0));
            Assert.AreEqual(false, sigBitAnd.MomList.Boolean(0));
            Assert.AreEqual(0.0, sigBitAnd.MomList.Real(0));
            Assert.IsNull(sigBitAnd.MomList.Error(0));
            Assert.AreEqual(beg.AddSeconds(7), sigBitAnd.MomList.Time(7));
            Assert.AreEqual(1, sigBitAnd.MomList.Integer(7));
            Assert.AreEqual(true, sigBitAnd.MomList.Boolean(7));
            Assert.IsNull(sigBitAnd.MomList.Error(7));
            Assert.AreEqual(beg.AddSeconds(10), sigBitAnd.MomList.Time(10));
            Assert.AreEqual(false, sigBitAnd.MomList.Boolean(10));
            Assert.AreEqual("0", sigBitAnd.MomList.String(10));
            Assert.IsNull(sigBitAnd.MomList.Error(10));
            Assert.AreEqual(beg.AddSeconds(599), sigBitAnd.MomList.Time(599));
            Assert.AreEqual(true, sigBitAnd.MomList.Boolean(599));
            Assert.IsNull(sigBitAnd.MomList.Error(599));

            //First
            Assert.AreEqual(120, sigFirst.MomList.Count);
            Assert.AreEqual(beg, sigFirst.MomList.Time(0));
            Assert.AreEqual(0.0, sigFirst.MomList.Real(0));
            Assert.IsNull(sigFirst.MomList.Error(0));
            Assert.AreEqual(beg.AddSeconds(30), sigFirst.MomList.Time(6));
            Assert.AreEqual(15, sigFirst.MomList.Real(6));
            Assert.AreEqual("15", sigFirst.MomList.String(6));
            Assert.IsNull(sigFirst.MomList.Error(6));
            Assert.AreEqual(beg.AddSeconds(55), sigFirst.MomList.Time(11));
            Assert.AreEqual(27.5, sigFirst.MomList.Real(11));
            Assert.AreEqual(28, sigFirst.MomList.Integer(11));
            Assert.IsNull(sigFirst.MomList.Error(11));
            Assert.AreEqual(beg.AddSeconds(595), sigFirst.MomList.Time(119));
            Assert.AreEqual(297.5, sigFirst.MomList.Real(119));
            Assert.IsNull(sigFirst.MomList.Error(119));

            //Last
            Assert.AreEqual(120, sigLast.MomList.Count);
            Assert.AreEqual(beg.AddSeconds(5), sigLast.MomList.Time(0));
            Assert.AreEqual(2.5, sigLast.MomList.Real(0));
            Assert.AreEqual(beg.AddSeconds(35), sigLast.MomList.Time(6));
            Assert.AreEqual(17.5, sigLast.MomList.Real(6));
            Assert.AreEqual("17,5", sigLast.MomList.String(6));
            Assert.AreEqual(beg.AddSeconds(60), sigLast.MomList.Time(11));
            Assert.AreEqual(30, sigLast.MomList.Real(11));
            Assert.AreEqual(30, sigLast.MomList.Integer(11));
            Assert.AreEqual(beg.AddSeconds(600), sigLast.MomList.Time(119));
            Assert.AreEqual(300, sigLast.MomList.Real(119));

            //Min
            Assert.AreEqual(120, sigMin.MomList.Count);
            Assert.AreEqual(beg, sigMin.MomList.Time(0));
            Assert.AreEqual(0.0, sigMin.MomList.Real(0));
            Assert.AreEqual(beg.AddSeconds(30), sigMin.MomList.Time(6));
            Assert.AreEqual(15, sigMin.MomList.Real(6));
            Assert.AreEqual("15", sigMin.MomList.String(6));
            Assert.AreEqual(beg.AddSeconds(55), sigMin.MomList.Time(11));
            Assert.AreEqual(27.5, sigMin.MomList.Real(11));
            Assert.AreEqual(28, sigMin.MomList.Integer(11));
            Assert.AreEqual(beg.AddSeconds(595), sigMin.MomList.Time(119));
            Assert.AreEqual(297.5, sigMin.MomList.Real(119));
            Assert.IsNull(sigMin.MomList.Error(119));

            //Max
            Assert.AreEqual(120, sigMax.MomList.Count);
            Assert.AreEqual(beg.AddSeconds(5), sigMax.MomList.Time(0));
            Assert.AreEqual(2.5, sigMax.MomList.Real(0));
            Assert.AreEqual(beg.AddSeconds(35), sigMax.MomList.Time(6));
            Assert.AreEqual(17.5, sigMax.MomList.Real(6));
            Assert.AreEqual("17,5", sigMax.MomList.String(6));
            Assert.AreEqual(beg.AddSeconds(60), sigMax.MomList.Time(11));
            Assert.AreEqual(30, sigMax.MomList.Real(11));
            Assert.AreEqual(30, sigMax.MomList.Integer(11));
            Assert.AreEqual(beg.AddSeconds(600), sigMax.MomList.Time(119));
            Assert.AreEqual(300, sigMax.MomList.Real(119));

            //Average
            Assert.AreEqual(120, sigAverage.MomList.Count);
            Assert.AreEqual(beg.AddSeconds(2.5), sigAverage.MomList.Time(0));
            Assert.AreEqual(1.0, sigAverage.MomList.Real(0));
            Assert.AreEqual(beg.AddSeconds(32.5), sigAverage.MomList.Time(6));
            Assert.AreEqual(16, sigAverage.MomList.Real(6));
            Assert.AreEqual("16", sigAverage.MomList.String(6));
            Assert.AreEqual(beg.AddSeconds(57.5), sigAverage.MomList.Time(11));
            Assert.AreEqual(28.5, sigAverage.MomList.Real(11));
            Assert.AreEqual(28, sigAverage.MomList.Integer(11));
            Assert.AreEqual(beg.AddSeconds(597.5), sigAverage.MomList.Time(119));
            Assert.AreEqual(298.5, sigAverage.MomList.Real(119));
            Assert.IsNull(sigAverage.MomList.Error(119));
        }

        [TestMethod]
        public void CalcValuesPartial()
        {
            SourceConnect connect = MakeFictiveConnect();
            var source = (FictiveSimpleSource)connect.Provider;
            connect.AddInitialSignal("Ob.Int", "Ob", DataType.Integer, "NumObject=1;Signal=Int;ValuesInterval=1000", true);
            connect.AddInitialSignal("Ob.Real", "Ob", DataType.Real, "NumObject=1;Signal=Real;ValuesInterval=1000", true);
            Assert.AreEqual(2, connect.Signals.Count);
            Assert.AreEqual(2, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);

            var sigBit = connect.AddCalcSignal("Ob.Bit2", "Ob", "Ob.Int", "Bit;1;2");
            var sigBitOr = connect.AddCalcSignal("Ob.Bit1Or2", "Ob", "Ob.Int", "BitOr;2;1;2");
            var sigBitAnd = connect.AddCalcSignal("Ob.Bit1And2", "Ob", "Ob.Int", "BitAnd;2;1;2");
            var sigFirst = connect.AddCalcSignal("Ob.First", "Ob", "Ob.Real", "First;1;1.5");
            var sigLast = connect.AddCalcSignal("Ob.Last", "Ob", "Ob.Real", "Last;2;1.5;1");
            var sigMin = connect.AddCalcSignal("Ob.Min", "Ob", "Ob.Real", "Min;1;1.5");
            var sigMax = connect.AddCalcSignal("Ob.Max", "Ob", "Ob.Real", "Max;2;1.5;1");
            var sigAverage = connect.AddCalcSignal("Ob.Average", "Ob", "Ob.Real", "Average;2;2.5;0.5");

            connect.Prepare();
            Assert.AreEqual(1, source.Objects.Count);
            var beg = new DateTime(2007, 11, 20, 10, 0, 0);
            var en = new DateTime(2007, 11, 20, 10, 1, 0);
            connect.GetValues(beg, en);

            //Bit
            Assert.AreEqual(61, sigBit.MomList.Count);
            Assert.AreEqual(beg, sigBit.MomList.Time(0));
            Assert.AreEqual(0, sigBit.MomList.Integer(0));
            Assert.AreEqual(0.0, sigBit.MomList.Real(0));
            Assert.IsNull(sigBit.MomList.Error(0));
            Assert.AreEqual(beg.AddSeconds(7), sigBit.MomList.Time(7));
            Assert.AreEqual(1, sigBit.MomList.Integer(7));
            Assert.AreEqual(true, sigBit.MomList.Boolean(7));
            Assert.IsNull(sigBit.MomList.Error(7));

            //BitOr
            Assert.AreEqual(beg.AddSeconds(10), sigBitOr.MomList.Time(10));
            Assert.AreEqual(true, sigBitOr.MomList.Boolean(10));
            Assert.IsNull(sigBitOr.MomList.Error(10));
            Assert.AreEqual(beg.AddSeconds(56), sigBitOr.MomList.Time(56));
            Assert.AreEqual(false, sigBitOr.MomList.Boolean(56));
            Assert.AreEqual("0", sigBitOr.MomList.String(56));
            Assert.IsNull(sigBitOr.MomList.Error(56));

            //BitAnd
            Assert.AreEqual(61, sigBitAnd.MomList.Count);
            Assert.AreEqual(beg.AddSeconds(3), sigBitAnd.MomList.Time(3));
            Assert.AreEqual(false, sigBitAnd.MomList.Boolean(3));
            Assert.AreEqual(0.0, sigBitAnd.MomList.Real(3));
            Assert.AreEqual(beg.AddSeconds(31), sigBitAnd.MomList.Time(31));
            Assert.AreEqual(1, sigBitAnd.MomList.Integer(31));
            Assert.AreEqual(true, sigBitAnd.MomList.Boolean(31));
            
            //First
            Assert.AreEqual(40, sigFirst.MomList.Count);
            Assert.AreEqual(beg, sigFirst.MomList.Time(0));
            Assert.AreEqual(0.0, sigFirst.MomList.Real(0));
            Assert.IsNull(sigFirst.MomList.Error(0));
            Assert.AreEqual(beg.AddSeconds(9), sigFirst.MomList.Time(6));
            Assert.AreEqual(4.5, sigFirst.MomList.Real(6));
            Assert.AreEqual("4,5", sigFirst.MomList.String(6));
            Assert.IsNull(sigFirst.MomList.Error(6));
            Assert.AreEqual(beg.AddSeconds(16.5), sigFirst.MomList.Time(11));
            Assert.AreEqual(8, sigFirst.MomList.Real(11));
            Assert.AreEqual(8, sigFirst.MomList.Integer(11));
            Assert.IsNull(sigFirst.MomList.Error(11));
            Assert.AreEqual(beg.AddSeconds(58.5), sigFirst.MomList.Time(39));
            Assert.AreEqual(29, sigFirst.MomList.Real(39));
            Assert.IsNull(sigFirst.MomList.Error(39));

            //Last
            Assert.AreEqual(40, sigLast.MomList.Count);
            Assert.AreEqual(beg.AddSeconds(1.5), sigLast.MomList.Time(0));
            Assert.AreEqual(0.5, sigLast.MomList.Real(0));
            Assert.AreEqual(beg.AddSeconds(10.5), sigLast.MomList.Time(6));
            Assert.AreEqual(5, sigLast.MomList.Real(6));
            Assert.AreEqual("5", sigLast.MomList.String(6));
            Assert.AreEqual(beg.AddSeconds(18), sigLast.MomList.Time(11));
            Assert.AreEqual(9, sigLast.MomList.Real(11));
            Assert.AreEqual(beg.AddSeconds(60), sigLast.MomList.Time(39));
            Assert.AreEqual(30, sigLast.MomList.Real(39));
            
            //Min
            Assert.AreEqual(40, sigMin.MomList.Count);
            Assert.AreEqual(beg, sigMin.MomList.Time(0));
            Assert.AreEqual(0.0, sigMin.MomList.Real(0));
            Assert.IsNull(sigMin.MomList.Error(0));
            Assert.AreEqual(beg.AddSeconds(9), sigMin.MomList.Time(6));
            Assert.AreEqual(4.5, sigMin.MomList.Real(6));
            Assert.AreEqual("4,5", sigMin.MomList.String(6));
            Assert.IsNull(sigMin.MomList.Error(6));
            Assert.AreEqual(beg.AddSeconds(16.5), sigMin.MomList.Time(11));
            Assert.AreEqual(8, sigMin.MomList.Real(11));
            Assert.AreEqual(8, sigMin.MomList.Integer(11));
            Assert.IsNull(sigMin.MomList.Error(11));
            Assert.AreEqual(beg.AddSeconds(58.5), sigMin.MomList.Time(39));
            Assert.AreEqual(29, sigMin.MomList.Real(39));
            Assert.IsNull(sigMin.MomList.Error(39));

            //Max
            Assert.AreEqual(40, sigMax.MomList.Count);
            Assert.AreEqual(beg.AddSeconds(1.5), sigMax.MomList.Time(0));
            Assert.AreEqual(0.5, sigMax.MomList.Real(0));
            Assert.AreEqual(beg.AddSeconds(10.5), sigMax.MomList.Time(6));
            Assert.AreEqual(5, sigMax.MomList.Real(6));
            Assert.AreEqual("5", sigMax.MomList.String(6));
            Assert.AreEqual(beg.AddSeconds(18), sigMax.MomList.Time(11));
            Assert.AreEqual(9, sigMax.MomList.Real(11));
            Assert.AreEqual(beg.AddSeconds(60), sigMax.MomList.Time(39));
            Assert.AreEqual(30, sigMax.MomList.Real(39));

            //Average
            Assert.AreEqual(24, sigAverage.MomList.Count);
            Assert.AreEqual(beg.AddSeconds(1.25), sigAverage.MomList.Time(0));
            Assert.AreEqual(0.4, sigAverage.MomList.Real(0));
            Assert.AreEqual(beg.AddSeconds(16.25), sigAverage.MomList.Time(6));
            Assert.AreEqual(7.9, sigAverage.MomList.Real(6));
            Assert.AreEqual(beg.AddSeconds(28.75), sigAverage.MomList.Time(11));
            Assert.IsTrue(Math.Abs(sigAverage.MomList.Real(11)-14.1) < 0.0001);
            Assert.AreEqual(beg.AddSeconds(58.75), sigAverage.MomList.Time(23));
            Assert.IsTrue(Math.Abs(sigAverage.MomList.Real(23) - 29.1) < 0.0001);
        }
    }
}
