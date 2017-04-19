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
    public class FictiveSimpleTest
    {
        private SourceConnect MakeFictiveConnect(bool makeReserve = false)
        {
            var factory = new ProvidersFactory();
            var logger = new Logger(new TestHistory(), new AppIndicator());
            var connect = (SourceConnect)factory.CreateConnect(ProviderType.Source, "TestSource", "Fictive", logger);
            var source = (FictiveSimpleSource)factory.CreateProvider("FictiveSimpleSource", "Label=p1");
            FictiveSimpleSource source2 = null;
            if (makeReserve)
                source2 = (FictiveSimpleSource)factory.CreateProvider("FictiveSimpleSource", "Label=p2");
            connect.JoinProvider(source, source2);
            return connect;
        }

        private static void GetValues(SourceConnect connect, DateTime beg, DateTime en)
        {
            connect.Logger.StartPeriod(beg, en);
            connect.GetValues();
            connect.Logger.FinishPeriod();
        }

        [TestMethod]
        public void Signals()
        {
            var connect = MakeFictiveConnect();
            var beg = new DateTime(2007, 11, 20, 10, 30, 0);
            var en = new DateTime(2007, 11, 20, 10, 40, 0);

            var source = (FictiveSimpleSource)connect.Provider;
            Assert.AreEqual("p1", source.Label);
            Assert.AreEqual(0, connect.Signals.Count);
            var sig1 = connect.AddSignal("Ob.Int", DataType.Integer, SignalType.Uniform, "NumObject=1", "ValuesInterval=2000", "Signal=Int;");
            Assert.AreEqual(1, connect.Signals.Count);
            Assert.AreEqual(1, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);
            
            Assert.AreEqual(DataType.Integer, sig1.DataType);
            Assert.AreEqual(3, sig1.Inf.Count);
            Assert.AreEqual("1", sig1.Inf["NumObject"]);
            Assert.AreEqual("Int", sig1.Inf["Signal"]);
            Assert.AreEqual("2000", sig1.Inf["ValuesInterval"]);
            Assert.AreEqual("TestSource", sig1.Connect.Name);

            GetValues(connect, beg, en);
            Assert.AreEqual(1, source.Outs.Count);
            Assert.IsTrue(source.Outs.ContainsKey(1));
            FictiveOut ob1 = source.Outs[1];
            Assert.AreEqual("NumObject=1;ValuesInterval=2000", ob1.Context);
            Assert.IsNotNull(ob1.IntSignal);
            Assert.IsNull(ob1.RealSignal);
            Assert.IsNull(ob1.ValueSignal);
            Assert.IsTrue(ob1.IsInitialized);
            Assert.AreEqual(2000, ob1.ValuesInterval);

            var sig2 = connect.AddSignal("Ob.Value", DataType.Real, SignalType.Uniform, "NumObject=1", "ValuesInterval=2000", "Signal=Value");
            Assert.AreEqual(2, connect.Signals.Count);
            Assert.AreEqual(2, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);

            Assert.AreEqual(DataType.Real, sig2.DataType);
            Assert.AreEqual(3, sig2.Inf.Count);
            Assert.AreEqual("1", sig2.Inf["NumObject"]);
            Assert.AreEqual("Value", sig2.Inf["Signal"]);
            Assert.AreEqual("2000", sig2.Inf["ValuesInterval"]);

            var sig3 = connect.AddCalcSignal("Ob.Bit3", "Ob", "Int", "Bit;1;3");
            Assert.AreEqual(3, connect.Signals.Count);
            Assert.AreEqual(2, connect.InitialSignals.Count);
            Assert.AreEqual(1, connect.CalcSignals.Count);

            Assert.AreEqual(DataType.Boolean, sig3.DataType);
            Assert.IsNull(sig3.Inf);
            Assert.AreEqual("Bit", sig3.Calculate.Method.Name);
            Assert.AreEqual("TestSource", sig3.Connect.Name);

            connect.AddSignal("Ob2.Int", DataType.Integer, SignalType.Uniform, "NumObject=2", "ValuesInterval=1000", "Signal=Int");
            var sig4 = connect.AddCalcSignal("Ob2.Bit4or5", "Ob2", "Int", "BitOr;2;4;5");
            Assert.AreEqual(5, connect.Signals.Count);
            Assert.AreEqual(3, connect.InitialSignals.Count);
            Assert.AreEqual(2, connect.CalcSignals.Count);

            Assert.AreEqual(DataType.Boolean, sig4.DataType);
            Assert.IsNull(sig4.Inf);
            Assert.AreEqual("BitOr", sig4.Calculate.Method.Name);
            Assert.AreEqual("Ob2.Bit4or5", sig4.Code);
            Assert.AreEqual("TestSource", sig4.Connect.Name);

            GetValues(connect, beg, en);
            Assert.AreEqual(2, source.Outs.Count);
            Assert.IsTrue(source.Outs.ContainsKey(2));
            FictiveOut ob2 = source.Outs[2];
            Assert.AreEqual("NumObject=2;ValuesInterval=1000", ob2.Context);
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
            var connect = MakeFictiveConnect();
            var source = (FictiveSimpleSource)connect.Provider;
            source.Prepare();
            var beg = new DateTime(2007, 11, 20, 10, 30, 0);
            var en = new DateTime(2007, 11, 20, 10, 40, 0);
            GetValues(connect, beg, en);
            Assert.AreEqual(0, connect.Signals.Count);

            var sigi = connect.AddSignal("Ob.Int", DataType.Integer, SignalType.Uniform, "NumObject=1", "ValuesInterval=1000", "Signal=Int");
            var sigr = connect.AddSignal("Ob.Real", DataType.Real, SignalType.Uniform, "NumObject=1", "ValuesInterval=1000", "Signal=Real");
            Assert.AreEqual(2, connect.Signals.Count);
            connect.GetValues();
            GetValues(connect, beg, en);
            Assert.AreEqual(601, sigi.Value.Count);
            Assert.AreEqual(beg, sigi.Value.TimeI(0));
            Assert.AreEqual(0, sigi.Value.IntegerI(0));
            Assert.AreEqual(0.0, sigi.Value.RealI(0));
            Assert.IsNull(sigi.Value.ErrorI(0));
            Assert.AreEqual(beg.AddSeconds(1), sigi.Value.TimeI(1));
            Assert.AreEqual(1, sigi.Value.IntegerI(1));
            Assert.AreEqual(true, sigi.Value.BooleanI(1));
            Assert.IsNull(sigi.Value.ErrorI(1));
            Assert.AreEqual(beg.AddSeconds(10), sigi.Value.TimeI(10));
            Assert.AreEqual(10, sigi.Value.IntegerI(10));
            Assert.AreEqual(10.0, sigi.Value.RealI(10));
            Assert.IsNull(sigi.Value.ErrorI(10));
            Assert.AreEqual(beg.AddSeconds(599), sigi.Value.TimeI(599));
            Assert.AreEqual(599, sigi.Value.IntegerI(599));
            Assert.AreEqual("599", sigi.Value.StringI(599));
            Assert.IsNull(sigi.Value.ErrorI(599));

            Assert.AreEqual(601, sigr.Value.Count);
            Assert.AreEqual(beg, sigr.Value.TimeI(0));
            Assert.AreEqual(0.0, sigr.Value.RealI(0));
            Assert.IsNull(sigr.Value.ErrorI(0));
            Assert.AreEqual(beg.AddSeconds(1), sigr.Value.TimeI(1));
            Assert.AreEqual(0.5, sigr.Value.RealI(1));
            Assert.IsNull(sigr.Value.ErrorI(1));
            Assert.AreEqual(beg.AddSeconds(10), sigr.Value.TimeI(10));
            Assert.AreEqual(5.0, sigr.Value.RealI(10));
            Assert.IsNull(sigr.Value.ErrorI(10));
            Assert.AreEqual(beg.AddSeconds(599), sigr.Value.TimeI(599));
            Assert.AreEqual(299.5, sigr.Value.RealI(599));
            Assert.AreEqual("299,5", sigr.Value.StringI(599));
            Assert.IsNull(sigr.Value.ErrorI(599));
        }

        [TestMethod]
        public void CalcValuesFull()
        {
            var connect = MakeFictiveConnect();
            var source = (FictiveSimpleSource)connect.Provider;
            connect.AddSignal("Ob.Int", DataType.Integer, SignalType.Uniform, "NumObject=1", "ValuesInterval=1000", "Signal=Int");
            connect.AddSignal("Ob.Real", DataType.Real, SignalType.Uniform, "NumObject=1", "ValuesInterval=1000", "Signal=Real");
            Assert.AreEqual(2, connect.Signals.Count);
            Assert.AreEqual(2, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);
            
            var sigBit = connect.AddCalcSignal("Ob.Bit2", "Ob", "Int", "Bit;1;2");
            connect.AddCalcSignal("Ob.Bit2", "Ob", "Int", "Bit;1;2");
            var sigBitOr = connect.AddCalcSignal("Ob.Bit1Or2", "Ob", "Int", "BitOr;2;1;2");
            var sigBitAnd = connect.AddCalcSignal("Ob.Bit1And2", "Ob", "Int", "BitAnd;2;1;2");
            var sigFirst = connect.AddCalcSignal("Ob.First", "Ob", "Real", "First;1;5");
            var sigLast = connect.AddCalcSignal("Ob.Last", "Ob", "Real", "Last;2;5;1");
            var sigMin = connect.AddCalcSignal("Ob.Min", "Ob", "Real", "Min;1;5");
            var sigMax = connect.AddCalcSignal("Ob.Max", "Ob", "Real", "Max;2;5;1");
            var sigAverage = connect.AddCalcSignal("Ob.Average", "Ob", "Real", "Average;2;5;0.5");

            var beg = new DateTime(2007, 11, 20, 10, 30, 0);
            var en = new DateTime(2007, 11, 20, 10, 40, 0);
            GetValues(connect, beg, en);
            Assert.AreEqual(1, source.Outs.Count);

            //Bit
            Assert.AreEqual(601, sigBit.Value.Count);
            Assert.AreEqual(beg, sigBit.Value.TimeI(0));
            Assert.AreEqual(0, sigBit.Value.IntegerI(0));
            Assert.AreEqual(0.0, sigBit.Value.RealI(0));
            Assert.IsNull(sigBit.Value.ErrorI(0));
            Assert.AreEqual(beg.AddSeconds(5), sigBit.Value.TimeI(5));
            Assert.AreEqual(1, sigBit.Value.IntegerI(5));
            Assert.AreEqual(true, sigBit.Value.BooleanI(5));
            Assert.IsNull(sigBit.Value.ErrorI(5));
            Assert.AreEqual(beg.AddSeconds(10), sigBit.Value.TimeI(10));
            Assert.AreEqual(false, sigBit.Value.BooleanI(10));
            Assert.IsNull(sigBit.Value.ErrorI(10));
            Assert.AreEqual(beg.AddSeconds(599), sigBit.Value.TimeI(599));
            Assert.AreEqual(true, sigBit.Value.BooleanI(599));
            Assert.AreEqual("1", sigBit.Value.StringI(599));
            Assert.IsNull(sigBit.Value.ErrorI(599));

            //BitOr
            Assert.AreEqual(601, sigBitOr.Value.Count);
            Assert.AreEqual(beg, sigBitOr.Value.TimeI(0));
            Assert.AreEqual(false, sigBitOr.Value.BooleanI(0));
            Assert.AreEqual(0.0, sigBitOr.Value.RealI(0));
            Assert.IsNull(sigBitOr.Value.ErrorI(0));
            Assert.AreEqual(beg.AddSeconds(5), sigBitOr.Value.TimeI(5));
            Assert.AreEqual(1, sigBitOr.Value.IntegerI(5));
            Assert.AreEqual(true, sigBitOr.Value.BooleanI(5));
            Assert.IsNull(sigBitOr.Value.ErrorI(5));
            Assert.AreEqual(beg.AddSeconds(10), sigBitOr.Value.TimeI(10));
            Assert.AreEqual(true, sigBitOr.Value.BooleanI(10));
            Assert.IsNull(sigBitOr.Value.ErrorI(10));
            Assert.AreEqual(beg.AddSeconds(592), sigBitOr.Value.TimeI(592));
            Assert.AreEqual(false, sigBitOr.Value.BooleanI(592));
            Assert.AreEqual("0", sigBitOr.Value.StringI(592));
            Assert.IsNull(sigBitOr.Value.ErrorI(592));

            //BitAnd
            Assert.AreEqual(601, sigBitAnd.Value.Count);
            Assert.AreEqual(beg, sigBitAnd.Value.TimeI(0));
            Assert.AreEqual(false, sigBitAnd.Value.BooleanI(0));
            Assert.AreEqual(0.0, sigBitAnd.Value.RealI(0));
            Assert.IsNull(sigBitAnd.Value.ErrorI(0));
            Assert.AreEqual(beg.AddSeconds(7), sigBitAnd.Value.TimeI(7));
            Assert.AreEqual(1, sigBitAnd.Value.IntegerI(7));
            Assert.AreEqual(true, sigBitAnd.Value.BooleanI(7));
            Assert.IsNull(sigBitAnd.Value.ErrorI(7));
            Assert.AreEqual(beg.AddSeconds(10), sigBitAnd.Value.TimeI(10));
            Assert.AreEqual(false, sigBitAnd.Value.BooleanI(10));
            Assert.AreEqual("0", sigBitAnd.Value.StringI(10));
            Assert.IsNull(sigBitAnd.Value.ErrorI(10));
            Assert.AreEqual(beg.AddSeconds(599), sigBitAnd.Value.TimeI(599));
            Assert.AreEqual(true, sigBitAnd.Value.BooleanI(599));
            Assert.IsNull(sigBitAnd.Value.ErrorI(599));

            //First
            Assert.AreEqual(120, sigFirst.Value.Count);
            Assert.AreEqual(beg, sigFirst.Value.TimeI(0));
            Assert.AreEqual(0.0, sigFirst.Value.RealI(0));
            Assert.IsNull(sigFirst.Value.ErrorI(0));
            Assert.AreEqual(beg.AddSeconds(30), sigFirst.Value.TimeI(6));
            Assert.AreEqual(15, sigFirst.Value.RealI(6));
            Assert.AreEqual("15", sigFirst.Value.StringI(6));
            Assert.IsNull(sigFirst.Value.ErrorI(6));
            Assert.AreEqual(beg.AddSeconds(55), sigFirst.Value.TimeI(11));
            Assert.AreEqual(27.5, sigFirst.Value.RealI(11));
            Assert.AreEqual(28, sigFirst.Value.IntegerI(11));
            Assert.IsNull(sigFirst.Value.ErrorI(11));
            Assert.AreEqual(beg.AddSeconds(595), sigFirst.Value.TimeI(119));
            Assert.AreEqual(297.5, sigFirst.Value.RealI(119));
            Assert.IsNull(sigFirst.Value.ErrorI(119));

            //Last
            Assert.AreEqual(120, sigLast.Value.Count);
            Assert.AreEqual(beg.AddSeconds(5), sigLast.Value.TimeI(0));
            Assert.AreEqual(2.5, sigLast.Value.RealI(0));
            Assert.AreEqual(beg.AddSeconds(35), sigLast.Value.TimeI(6));
            Assert.AreEqual(17.5, sigLast.Value.RealI(6));
            Assert.AreEqual("17,5", sigLast.Value.StringI(6));
            Assert.AreEqual(beg.AddSeconds(60), sigLast.Value.TimeI(11));
            Assert.AreEqual(30, sigLast.Value.RealI(11));
            Assert.AreEqual(30, sigLast.Value.IntegerI(11));
            Assert.AreEqual(beg.AddSeconds(600), sigLast.Value.TimeI(119));
            Assert.AreEqual(300, sigLast.Value.RealI(119));

            //Min
            Assert.AreEqual(120, sigMin.Value.Count);
            Assert.AreEqual(beg, sigMin.Value.TimeI(0));
            Assert.AreEqual(0.0, sigMin.Value.RealI(0));
            Assert.AreEqual(beg.AddSeconds(30), sigMin.Value.TimeI(6));
            Assert.AreEqual(15, sigMin.Value.RealI(6));
            Assert.AreEqual("15", sigMin.Value.StringI(6));
            Assert.AreEqual(beg.AddSeconds(55), sigMin.Value.TimeI(11));
            Assert.AreEqual(27.5, sigMin.Value.RealI(11));
            Assert.AreEqual(28, sigMin.Value.IntegerI(11));
            Assert.AreEqual(beg.AddSeconds(595), sigMin.Value.TimeI(119));
            Assert.AreEqual(297.5, sigMin.Value.RealI(119));
            Assert.IsNull(sigMin.Value.ErrorI(119));

            //Max
            Assert.AreEqual(120, sigMax.Value.Count);
            Assert.AreEqual(beg.AddSeconds(5), sigMax.Value.TimeI(0));
            Assert.AreEqual(2.5, sigMax.Value.RealI(0));
            Assert.AreEqual(beg.AddSeconds(35), sigMax.Value.TimeI(6));
            Assert.AreEqual(17.5, sigMax.Value.RealI(6));
            Assert.AreEqual("17,5", sigMax.Value.StringI(6));
            Assert.AreEqual(beg.AddSeconds(60), sigMax.Value.TimeI(11));
            Assert.AreEqual(30, sigMax.Value.RealI(11));
            Assert.AreEqual(30, sigMax.Value.IntegerI(11));
            Assert.AreEqual(beg.AddSeconds(600), sigMax.Value.TimeI(119));
            Assert.AreEqual(300, sigMax.Value.RealI(119));

            //Average
            Assert.AreEqual(120, sigAverage.Value.Count);
            Assert.AreEqual(beg.AddSeconds(2.5), sigAverage.Value.TimeI(0));
            Assert.AreEqual(1.0, sigAverage.Value.RealI(0));
            Assert.AreEqual(beg.AddSeconds(32.5), sigAverage.Value.TimeI(6));
            Assert.AreEqual(16, sigAverage.Value.RealI(6));
            Assert.AreEqual("16", sigAverage.Value.StringI(6));
            Assert.AreEqual(beg.AddSeconds(57.5), sigAverage.Value.TimeI(11));
            Assert.AreEqual(28.5, sigAverage.Value.RealI(11));
            Assert.AreEqual(28, sigAverage.Value.IntegerI(11));
            Assert.AreEqual(beg.AddSeconds(597.5), sigAverage.Value.TimeI(119));
            Assert.AreEqual(298.5, sigAverage.Value.RealI(119));
            Assert.IsNull(sigAverage.Value.ErrorI(119));
        }

        [TestMethod]
        public void CalcValuesPartial()
        {
            var connect = MakeFictiveConnect();
            var source = (FictiveSimpleSource)connect.Provider;
            connect.AddSignal("Ob.Int", DataType.Integer, SignalType.Uniform, "NumObject=1", "ValuesInterval=1000", "Signal=Int");
            connect.AddSignal("Ob.Real", DataType.Real, SignalType.Uniform, "NumObject=1", "ValuesInterval=1000", "Signal=Real");
            Assert.AreEqual(2, connect.Signals.Count);
            Assert.AreEqual(2, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);

            var sigBit = connect.AddCalcSignal("Ob.Bit2", "Ob", "Int", "Bit;1;2");
            var sigBitOr = connect.AddCalcSignal("Ob.Bit1Or2", "Ob", "Int", "BitOr;2;1;2");
            var sigBitAnd = connect.AddCalcSignal("Ob.Bit1And2", "Ob", "Int", "BitAnd;2;1;2");
            var sigFirst = connect.AddCalcSignal("Ob.First", "Ob", "Real", "First;1;1.5");
            var sigLast = connect.AddCalcSignal("Ob.Last", "Ob", "Real", "Last;2;1.5;1");
            var sigMin = connect.AddCalcSignal("Ob.Min", "Ob", "Real", "Min;1;1.5");
            var sigMax = connect.AddCalcSignal("Ob.Max", "Ob", "Real", "Max;2;1.5;1");
            var sigAverage = connect.AddCalcSignal("Ob.Average", "Ob", "Real", "Average;2;2.5;0.5");

            
            var beg = new DateTime(2007, 11, 20, 10, 0, 0);
            var en = new DateTime(2007, 11, 20, 10, 1, 0);
            GetValues(connect, beg, en);
            Assert.AreEqual(1, source.Outs.Count);

            //Bit
            Assert.AreEqual(61, sigBit.Value.Count);
            Assert.AreEqual(beg, sigBit.Value.TimeI(0));
            Assert.AreEqual(0, sigBit.Value.IntegerI(0));
            Assert.AreEqual(0.0, sigBit.Value.RealI(0));
            Assert.IsNull(sigBit.Value.ErrorI(0));
            Assert.AreEqual(beg.AddSeconds(7), sigBit.Value.TimeI(7));
            Assert.AreEqual(1, sigBit.Value.IntegerI(7));
            Assert.AreEqual(true, sigBit.Value.BooleanI(7));
            Assert.IsNull(sigBit.Value.ErrorI(7));

            //BitOr
            Assert.AreEqual(beg.AddSeconds(10), sigBitOr.Value.TimeI(10));
            Assert.AreEqual(true, sigBitOr.Value.BooleanI(10));
            Assert.IsNull(sigBitOr.Value.ErrorI(10));
            Assert.AreEqual(beg.AddSeconds(56), sigBitOr.Value.TimeI(56));
            Assert.AreEqual(false, sigBitOr.Value.BooleanI(56));
            Assert.AreEqual("0", sigBitOr.Value.StringI(56));
            Assert.IsNull(sigBitOr.Value.ErrorI(56));

            //BitAnd
            Assert.AreEqual(61, sigBitAnd.Value.Count);
            Assert.AreEqual(beg.AddSeconds(3), sigBitAnd.Value.TimeI(3));
            Assert.AreEqual(false, sigBitAnd.Value.BooleanI(3));
            Assert.AreEqual(0.0, sigBitAnd.Value.RealI(3));
            Assert.AreEqual(beg.AddSeconds(31), sigBitAnd.Value.TimeI(31));
            Assert.AreEqual(1, sigBitAnd.Value.IntegerI(31));
            Assert.AreEqual(true, sigBitAnd.Value.BooleanI(31));
            
            //First
            Assert.AreEqual(40, sigFirst.Value.Count);
            Assert.AreEqual(beg, sigFirst.Value.TimeI(0));
            Assert.AreEqual(0.0, sigFirst.Value.RealI(0));
            Assert.IsNull(sigFirst.Value.ErrorI(0));
            Assert.AreEqual(beg.AddSeconds(9), sigFirst.Value.TimeI(6));
            Assert.AreEqual(4.5, sigFirst.Value.RealI(6));
            Assert.AreEqual("4,5", sigFirst.Value.StringI(6));
            Assert.IsNull(sigFirst.Value.ErrorI(6));
            Assert.AreEqual(beg.AddSeconds(16.5), sigFirst.Value.TimeI(11));
            Assert.AreEqual(8, sigFirst.Value.RealI(11));
            Assert.AreEqual(8, sigFirst.Value.IntegerI(11));
            Assert.IsNull(sigFirst.Value.ErrorI(11));
            Assert.AreEqual(beg.AddSeconds(58.5), sigFirst.Value.TimeI(39));
            Assert.AreEqual(29, sigFirst.Value.RealI(39));
            Assert.IsNull(sigFirst.Value.ErrorI(39));

            //Last
            Assert.AreEqual(40, sigLast.Value.Count);
            Assert.AreEqual(beg.AddSeconds(1.5), sigLast.Value.TimeI(0));
            Assert.AreEqual(0.5, sigLast.Value.RealI(0));
            Assert.AreEqual(beg.AddSeconds(10.5), sigLast.Value.TimeI(6));
            Assert.AreEqual(5, sigLast.Value.RealI(6));
            Assert.AreEqual("5", sigLast.Value.StringI(6));
            Assert.AreEqual(beg.AddSeconds(18), sigLast.Value.TimeI(11));
            Assert.AreEqual(9, sigLast.Value.RealI(11));
            Assert.AreEqual(beg.AddSeconds(60), sigLast.Value.TimeI(39));
            Assert.AreEqual(30, sigLast.Value.RealI(39));
            
            //Min
            Assert.AreEqual(40, sigMin.Value.Count);
            Assert.AreEqual(beg, sigMin.Value.TimeI(0));
            Assert.AreEqual(0.0, sigMin.Value.RealI(0));
            Assert.IsNull(sigMin.Value.ErrorI(0));
            Assert.AreEqual(beg.AddSeconds(9), sigMin.Value.TimeI(6));
            Assert.AreEqual(4.5, sigMin.Value.RealI(6));
            Assert.AreEqual("4,5", sigMin.Value.StringI(6));
            Assert.IsNull(sigMin.Value.ErrorI(6));
            Assert.AreEqual(beg.AddSeconds(16.5), sigMin.Value.TimeI(11));
            Assert.AreEqual(8, sigMin.Value.RealI(11));
            Assert.AreEqual(8, sigMin.Value.IntegerI(11));
            Assert.IsNull(sigMin.Value.ErrorI(11));
            Assert.AreEqual(beg.AddSeconds(58.5), sigMin.Value.TimeI(39));
            Assert.AreEqual(29, sigMin.Value.RealI(39));
            Assert.IsNull(sigMin.Value.ErrorI(39));

            //Max
            Assert.AreEqual(40, sigMax.Value.Count);
            Assert.AreEqual(beg.AddSeconds(1.5), sigMax.Value.TimeI(0));
            Assert.AreEqual(0.5, sigMax.Value.RealI(0));
            Assert.AreEqual(beg.AddSeconds(10.5), sigMax.Value.TimeI(6));
            Assert.AreEqual(5, sigMax.Value.RealI(6));
            Assert.AreEqual("5", sigMax.Value.StringI(6));
            Assert.AreEqual(beg.AddSeconds(18), sigMax.Value.TimeI(11));
            Assert.AreEqual(9, sigMax.Value.RealI(11));
            Assert.AreEqual(beg.AddSeconds(60), sigMax.Value.TimeI(39));
            Assert.AreEqual(30, sigMax.Value.RealI(39));

            //Average
            Assert.AreEqual(24, sigAverage.Value.Count);
            Assert.AreEqual(beg.AddSeconds(1.25), sigAverage.Value.TimeI(0));
            Assert.AreEqual(0.4, sigAverage.Value.RealI(0));
            Assert.AreEqual(beg.AddSeconds(16.25), sigAverage.Value.TimeI(6));
            Assert.AreEqual(7.9, sigAverage.Value.RealI(6));
            Assert.AreEqual(beg.AddSeconds(28.75), sigAverage.Value.TimeI(11));
            Assert.IsTrue(Math.Abs(sigAverage.Value.RealI(11)-14.1) < 0.0001);
            Assert.AreEqual(beg.AddSeconds(58.75), sigAverage.Value.TimeI(23));
            Assert.IsTrue(Math.Abs(sigAverage.Value.RealI(23) - 29.1) < 0.0001);
        }

        [TestMethod]
        public void ChangeProvider()
        {
            var connect = MakeFictiveConnect(true);
            var source = (FictiveSimpleSource)connect.Provider;
            Assert.AreEqual("p1", source.Label);
            var beg = new DateTime(2007, 11, 20, 10, 30, 0);
            var en = new DateTime(2007, 11, 20, 10, 40, 0);

            var sigi = connect.AddSignal("Ob.Int", DataType.Integer, SignalType.Uniform, "NumObject=1", "ValuesInterval=1000", "Signal=Int");
            var sigr = connect.AddSignal("Ob.Real", DataType.Real, SignalType.Uniform, "NumObject=1", "ValuesInterval=1000", "Signal=Real");
            Assert.AreEqual(2, connect.Signals.Count);
            Assert.AreEqual(2, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);
            source.MakeErrorOnTheNextReading();
            GetValues(connect, beg, en);
            var source2 = (FictiveSimpleSource)connect.Provider;
            Assert.AreEqual("p2", source2.Label);
            Assert.AreEqual(1, source2.Outs.Count);
            Assert.IsTrue(source2.Outs.ContainsKey(1));
            Assert.AreEqual("NumObject=1;ValuesInterval=1000", source2.Outs[1].Context);
            Assert.AreEqual("p1", source.Label);
            Assert.AreEqual(1, source.Outs.Count);
            Assert.IsTrue(source.Outs.ContainsKey(1));
            Assert.AreEqual("NumObject=1;ValuesInterval=1000", source2.Outs[1].Context);

            Assert.AreEqual(601, sigi.Value.Count);
            Assert.AreEqual(beg, sigi.Value.TimeI(0));
            Assert.AreEqual(0, sigi.Value.IntegerI(0));
            Assert.AreEqual(0.0, sigi.Value.RealI(0));
            Assert.IsNull(sigi.Value.ErrorI(0));
            Assert.AreEqual(beg.AddSeconds(1), sigi.Value.TimeI(1));
            Assert.AreEqual(1, sigi.Value.IntegerI(1));
            Assert.AreEqual(true, sigi.Value.BooleanI(1));
            Assert.IsNull(sigi.Value.ErrorI(1));
            Assert.AreEqual(beg.AddSeconds(10), sigi.Value.TimeI(10));
            Assert.AreEqual(10, sigi.Value.IntegerI(10));
            Assert.AreEqual(10.0, sigi.Value.RealI(10));
            Assert.IsNull(sigi.Value.ErrorI(10));
            Assert.AreEqual(beg.AddSeconds(599), sigi.Value.TimeI(599));
            Assert.AreEqual(599, sigi.Value.IntegerI(599));
            Assert.AreEqual("599", sigi.Value.StringI(599));
            Assert.IsNull(sigi.Value.ErrorI(599));

            Assert.AreEqual(601, sigr.Value.Count);
            Assert.AreEqual(beg, sigr.Value.TimeI(0));
            Assert.AreEqual(0.0, sigr.Value.RealI(0));
            Assert.IsNull(sigr.Value.ErrorI(0));
            Assert.AreEqual(beg.AddSeconds(1), sigr.Value.TimeI(1));
            Assert.AreEqual(0.5, sigr.Value.RealI(1));
            Assert.IsNull(sigr.Value.ErrorI(1));
            Assert.AreEqual(beg.AddSeconds(10), sigr.Value.TimeI(10));
            Assert.AreEqual(5.0, sigr.Value.RealI(10));
            Assert.IsNull(sigr.Value.ErrorI(10));
            Assert.AreEqual(beg.AddSeconds(599), sigr.Value.TimeI(599));
            Assert.AreEqual(299.5, sigr.Value.RealI(599));
            Assert.AreEqual("299,5", sigr.Value.StringI(599));
            Assert.IsNull(sigr.Value.ErrorI(599));
        }
    }
}
