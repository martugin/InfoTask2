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
            var connect = (SourceConnect)factory.CreateConnect(logger, ProviderType.Source, "TestSource", "Fictive");
            var source = (FictiveSimpleSource)factory.CreateProvider(logger, "FictiveSimpleSource", "Label=p1");
            FictiveSimpleSource source2 = null;
            if (makeReserve)
                source2 = (FictiveSimpleSource)factory.CreateProvider(logger, "FictiveSimpleSource", "Label=p2");
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
            Assert.AreEqual(0, connect.ReadingSignals.Count);
            var sig1 = connect.AddSignal("Ob.Int", DataType.Integer, SignalType.Uniform, "NumObject=1", "ValuesInterval=2000", "Signal=Int;");
            Assert.AreEqual(1, connect.ReadingSignals.Count);
            Assert.AreEqual(1, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);
            
            Assert.AreEqual(DataType.Integer, sig1.DataType);
            Assert.AreEqual(3, sig1.Inf.Count);
            Assert.AreEqual("1", sig1.Inf["NumObject"]);
            Assert.AreEqual("Int", sig1.Inf["Signal"]);
            Assert.AreEqual("2000", sig1.Inf["ValuesInterval"]);
            Assert.AreEqual("TestSource", sig1.Connect.Code);

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
            Assert.AreEqual(2, connect.ReadingSignals.Count);
            Assert.AreEqual(2, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);

            Assert.AreEqual(DataType.Real, sig2.DataType);
            Assert.AreEqual(3, sig2.Inf.Count);
            Assert.AreEqual("1", sig2.Inf["NumObject"]);
            Assert.AreEqual("Value", sig2.Inf["Signal"]);
            Assert.AreEqual("2000", sig2.Inf["ValuesInterval"]);

            var sig3 = connect.AddCalcSignal("Ob.Bit3", "Ob", "Int", "Bit;1;3");
            Assert.AreEqual(3, connect.ReadingSignals.Count);
            Assert.AreEqual(2, connect.InitialSignals.Count);
            Assert.AreEqual(1, connect.CalcSignals.Count);

            Assert.AreEqual(DataType.Boolean, sig3.DataType);
            Assert.IsNull(sig3.Inf);
            Assert.AreEqual("Bit", sig3.Calculate.Method.Name);
            Assert.AreEqual("TestSource", sig3.Connect.Code);

            connect.AddSignal("Ob2.Int", DataType.Integer, SignalType.Uniform, "NumObject=2", "ValuesInterval=1000", "Signal=Int");
            var sig4 = connect.AddCalcSignal("Ob2.Bit4or5", "Ob2", "Int", "BitOr;2;4;5");
            Assert.AreEqual(5, connect.ReadingSignals.Count);
            Assert.AreEqual(3, connect.InitialSignals.Count);
            Assert.AreEqual(2, connect.CalcSignals.Count);

            Assert.AreEqual(DataType.Boolean, sig4.DataType);
            Assert.IsNull(sig4.Inf);
            Assert.AreEqual("BitOr", sig4.Calculate.Method.Name);
            Assert.AreEqual("Ob2.Bit4or5", sig4.Code);
            Assert.AreEqual("TestSource", sig4.Connect.Code);

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
            Assert.AreEqual(0, connect.ReadingSignals.Count);
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
            Assert.AreEqual(0, connect.ReadingSignals.Count);

            var sigi = connect.AddSignal("Ob.Int", DataType.Integer, SignalType.Uniform, "NumObject=1", "ValuesInterval=1000", "Signal=Int");
            var sigr = connect.AddSignal("Ob.Real", DataType.Real, SignalType.Uniform, "NumObject=1", "ValuesInterval=1000", "Signal=Real");
            Assert.AreEqual(2, connect.ReadingSignals.Count);
            connect.GetValues();
            GetValues(connect, beg, en);
            Assert.AreEqual(601, sigi.OutValue.Count);
            Assert.AreEqual(beg, sigi.OutValue.TimeI(0));
            Assert.AreEqual(0, sigi.OutValue.IntegerI(0));
            Assert.AreEqual(0.0, sigi.OutValue.RealI(0));
            Assert.IsNull(sigi.OutValue.ErrorI(0));
            Assert.AreEqual(beg.AddSeconds(1), sigi.OutValue.TimeI(1));
            Assert.AreEqual(1, sigi.OutValue.IntegerI(1));
            Assert.AreEqual(true, sigi.OutValue.BooleanI(1));
            Assert.IsNull(sigi.OutValue.ErrorI(1));
            Assert.AreEqual(beg.AddSeconds(10), sigi.OutValue.TimeI(10));
            Assert.AreEqual(10, sigi.OutValue.IntegerI(10));
            Assert.AreEqual(10.0, sigi.OutValue.RealI(10));
            Assert.IsNull(sigi.OutValue.ErrorI(10));
            Assert.AreEqual(beg.AddSeconds(599), sigi.OutValue.TimeI(599));
            Assert.AreEqual(599, sigi.OutValue.IntegerI(599));
            Assert.AreEqual("599", sigi.OutValue.StringI(599));
            Assert.IsNull(sigi.OutValue.ErrorI(599));

            Assert.AreEqual(601, sigr.OutValue.Count);
            Assert.AreEqual(beg, sigr.OutValue.TimeI(0));
            Assert.AreEqual(0.0, sigr.OutValue.RealI(0));
            Assert.IsNull(sigr.OutValue.ErrorI(0));
            Assert.AreEqual(beg.AddSeconds(1), sigr.OutValue.TimeI(1));
            Assert.AreEqual(0.5, sigr.OutValue.RealI(1));
            Assert.IsNull(sigr.OutValue.ErrorI(1));
            Assert.AreEqual(beg.AddSeconds(10), sigr.OutValue.TimeI(10));
            Assert.AreEqual(5.0, sigr.OutValue.RealI(10));
            Assert.IsNull(sigr.OutValue.ErrorI(10));
            Assert.AreEqual(beg.AddSeconds(599), sigr.OutValue.TimeI(599));
            Assert.AreEqual(299.5, sigr.OutValue.RealI(599));
            Assert.AreEqual("299,5", sigr.OutValue.StringI(599));
            Assert.IsNull(sigr.OutValue.ErrorI(599));
        }

        [TestMethod]
        public void CalcValuesFull()
        {
            var connect = MakeFictiveConnect();
            var source = (FictiveSimpleSource)connect.Provider;
            connect.AddSignal("Ob.Int", DataType.Integer, SignalType.Uniform, "NumObject=1", "ValuesInterval=1000", "Signal=Int");
            connect.AddSignal("Ob.Real", DataType.Real, SignalType.Uniform, "NumObject=1", "ValuesInterval=1000", "Signal=Real");
            Assert.AreEqual(2, connect.ReadingSignals.Count);
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
            Assert.AreEqual(601, sigBit.OutValue.Count);
            Assert.AreEqual(beg, sigBit.OutValue.TimeI(0));
            Assert.AreEqual(0, sigBit.OutValue.IntegerI(0));
            Assert.AreEqual(0.0, sigBit.OutValue.RealI(0));
            Assert.IsNull(sigBit.OutValue.ErrorI(0));
            Assert.AreEqual(beg.AddSeconds(5), sigBit.OutValue.TimeI(5));
            Assert.AreEqual(1, sigBit.OutValue.IntegerI(5));
            Assert.AreEqual(true, sigBit.OutValue.BooleanI(5));
            Assert.IsNull(sigBit.OutValue.ErrorI(5));
            Assert.AreEqual(beg.AddSeconds(10), sigBit.OutValue.TimeI(10));
            Assert.AreEqual(false, sigBit.OutValue.BooleanI(10));
            Assert.IsNull(sigBit.OutValue.ErrorI(10));
            Assert.AreEqual(beg.AddSeconds(599), sigBit.OutValue.TimeI(599));
            Assert.AreEqual(true, sigBit.OutValue.BooleanI(599));
            Assert.AreEqual("1", sigBit.OutValue.StringI(599));
            Assert.IsNull(sigBit.OutValue.ErrorI(599));

            //BitOr
            Assert.AreEqual(601, sigBitOr.OutValue.Count);
            Assert.AreEqual(beg, sigBitOr.OutValue.TimeI(0));
            Assert.AreEqual(false, sigBitOr.OutValue.BooleanI(0));
            Assert.AreEqual(0.0, sigBitOr.OutValue.RealI(0));
            Assert.IsNull(sigBitOr.OutValue.ErrorI(0));
            Assert.AreEqual(beg.AddSeconds(5), sigBitOr.OutValue.TimeI(5));
            Assert.AreEqual(1, sigBitOr.OutValue.IntegerI(5));
            Assert.AreEqual(true, sigBitOr.OutValue.BooleanI(5));
            Assert.IsNull(sigBitOr.OutValue.ErrorI(5));
            Assert.AreEqual(beg.AddSeconds(10), sigBitOr.OutValue.TimeI(10));
            Assert.AreEqual(true, sigBitOr.OutValue.BooleanI(10));
            Assert.IsNull(sigBitOr.OutValue.ErrorI(10));
            Assert.AreEqual(beg.AddSeconds(592), sigBitOr.OutValue.TimeI(592));
            Assert.AreEqual(false, sigBitOr.OutValue.BooleanI(592));
            Assert.AreEqual("0", sigBitOr.OutValue.StringI(592));
            Assert.IsNull(sigBitOr.OutValue.ErrorI(592));

            //BitAnd
            Assert.AreEqual(601, sigBitAnd.OutValue.Count);
            Assert.AreEqual(beg, sigBitAnd.OutValue.TimeI(0));
            Assert.AreEqual(false, sigBitAnd.OutValue.BooleanI(0));
            Assert.AreEqual(0.0, sigBitAnd.OutValue.RealI(0));
            Assert.IsNull(sigBitAnd.OutValue.ErrorI(0));
            Assert.AreEqual(beg.AddSeconds(7), sigBitAnd.OutValue.TimeI(7));
            Assert.AreEqual(1, sigBitAnd.OutValue.IntegerI(7));
            Assert.AreEqual(true, sigBitAnd.OutValue.BooleanI(7));
            Assert.IsNull(sigBitAnd.OutValue.ErrorI(7));
            Assert.AreEqual(beg.AddSeconds(10), sigBitAnd.OutValue.TimeI(10));
            Assert.AreEqual(false, sigBitAnd.OutValue.BooleanI(10));
            Assert.AreEqual("0", sigBitAnd.OutValue.StringI(10));
            Assert.IsNull(sigBitAnd.OutValue.ErrorI(10));
            Assert.AreEqual(beg.AddSeconds(599), sigBitAnd.OutValue.TimeI(599));
            Assert.AreEqual(true, sigBitAnd.OutValue.BooleanI(599));
            Assert.IsNull(sigBitAnd.OutValue.ErrorI(599));

            //First
            Assert.AreEqual(120, sigFirst.OutValue.Count);
            Assert.AreEqual(beg, sigFirst.OutValue.TimeI(0));
            Assert.AreEqual(0.0, sigFirst.OutValue.RealI(0));
            Assert.IsNull(sigFirst.OutValue.ErrorI(0));
            Assert.AreEqual(beg.AddSeconds(30), sigFirst.OutValue.TimeI(6));
            Assert.AreEqual(15, sigFirst.OutValue.RealI(6));
            Assert.AreEqual("15", sigFirst.OutValue.StringI(6));
            Assert.IsNull(sigFirst.OutValue.ErrorI(6));
            Assert.AreEqual(beg.AddSeconds(55), sigFirst.OutValue.TimeI(11));
            Assert.AreEqual(27.5, sigFirst.OutValue.RealI(11));
            Assert.AreEqual(28, sigFirst.OutValue.IntegerI(11));
            Assert.IsNull(sigFirst.OutValue.ErrorI(11));
            Assert.AreEqual(beg.AddSeconds(595), sigFirst.OutValue.TimeI(119));
            Assert.AreEqual(297.5, sigFirst.OutValue.RealI(119));
            Assert.IsNull(sigFirst.OutValue.ErrorI(119));

            //Last
            Assert.AreEqual(120, sigLast.OutValue.Count);
            Assert.AreEqual(beg.AddSeconds(5), sigLast.OutValue.TimeI(0));
            Assert.AreEqual(2.5, sigLast.OutValue.RealI(0));
            Assert.AreEqual(beg.AddSeconds(35), sigLast.OutValue.TimeI(6));
            Assert.AreEqual(17.5, sigLast.OutValue.RealI(6));
            Assert.AreEqual("17,5", sigLast.OutValue.StringI(6));
            Assert.AreEqual(beg.AddSeconds(60), sigLast.OutValue.TimeI(11));
            Assert.AreEqual(30, sigLast.OutValue.RealI(11));
            Assert.AreEqual(30, sigLast.OutValue.IntegerI(11));
            Assert.AreEqual(beg.AddSeconds(600), sigLast.OutValue.TimeI(119));
            Assert.AreEqual(300, sigLast.OutValue.RealI(119));

            //Min
            Assert.AreEqual(120, sigMin.OutValue.Count);
            Assert.AreEqual(beg, sigMin.OutValue.TimeI(0));
            Assert.AreEqual(0.0, sigMin.OutValue.RealI(0));
            Assert.AreEqual(beg.AddSeconds(30), sigMin.OutValue.TimeI(6));
            Assert.AreEqual(15, sigMin.OutValue.RealI(6));
            Assert.AreEqual("15", sigMin.OutValue.StringI(6));
            Assert.AreEqual(beg.AddSeconds(55), sigMin.OutValue.TimeI(11));
            Assert.AreEqual(27.5, sigMin.OutValue.RealI(11));
            Assert.AreEqual(28, sigMin.OutValue.IntegerI(11));
            Assert.AreEqual(beg.AddSeconds(595), sigMin.OutValue.TimeI(119));
            Assert.AreEqual(297.5, sigMin.OutValue.RealI(119));
            Assert.IsNull(sigMin.OutValue.ErrorI(119));

            //Max
            Assert.AreEqual(120, sigMax.OutValue.Count);
            Assert.AreEqual(beg.AddSeconds(5), sigMax.OutValue.TimeI(0));
            Assert.AreEqual(2.5, sigMax.OutValue.RealI(0));
            Assert.AreEqual(beg.AddSeconds(35), sigMax.OutValue.TimeI(6));
            Assert.AreEqual(17.5, sigMax.OutValue.RealI(6));
            Assert.AreEqual("17,5", sigMax.OutValue.StringI(6));
            Assert.AreEqual(beg.AddSeconds(60), sigMax.OutValue.TimeI(11));
            Assert.AreEqual(30, sigMax.OutValue.RealI(11));
            Assert.AreEqual(30, sigMax.OutValue.IntegerI(11));
            Assert.AreEqual(beg.AddSeconds(600), sigMax.OutValue.TimeI(119));
            Assert.AreEqual(300, sigMax.OutValue.RealI(119));

            //Average
            Assert.AreEqual(120, sigAverage.OutValue.Count);
            Assert.AreEqual(beg.AddSeconds(2.5), sigAverage.OutValue.TimeI(0));
            Assert.AreEqual(1.0, sigAverage.OutValue.RealI(0));
            Assert.AreEqual(beg.AddSeconds(32.5), sigAverage.OutValue.TimeI(6));
            Assert.AreEqual(16, sigAverage.OutValue.RealI(6));
            Assert.AreEqual("16", sigAverage.OutValue.StringI(6));
            Assert.AreEqual(beg.AddSeconds(57.5), sigAverage.OutValue.TimeI(11));
            Assert.AreEqual(28.5, sigAverage.OutValue.RealI(11));
            Assert.AreEqual(28, sigAverage.OutValue.IntegerI(11));
            Assert.AreEqual(beg.AddSeconds(597.5), sigAverage.OutValue.TimeI(119));
            Assert.AreEqual(298.5, sigAverage.OutValue.RealI(119));
            Assert.IsNull(sigAverage.OutValue.ErrorI(119));
        }

        [TestMethod]
        public void CalcValuesPartial()
        {
            var connect = MakeFictiveConnect();
            var source = (FictiveSimpleSource)connect.Provider;
            connect.AddSignal("Ob.Int", DataType.Integer, SignalType.Uniform, "NumObject=1", "ValuesInterval=1000", "Signal=Int");
            connect.AddSignal("Ob.Real", DataType.Real, SignalType.Uniform, "NumObject=1", "ValuesInterval=1000", "Signal=Real");
            Assert.AreEqual(2, connect.ReadingSignals.Count);
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
            Assert.AreEqual(61, sigBit.OutValue.Count);
            Assert.AreEqual(beg, sigBit.OutValue.TimeI(0));
            Assert.AreEqual(0, sigBit.OutValue.IntegerI(0));
            Assert.AreEqual(0.0, sigBit.OutValue.RealI(0));
            Assert.IsNull(sigBit.OutValue.ErrorI(0));
            Assert.AreEqual(beg.AddSeconds(7), sigBit.OutValue.TimeI(7));
            Assert.AreEqual(1, sigBit.OutValue.IntegerI(7));
            Assert.AreEqual(true, sigBit.OutValue.BooleanI(7));
            Assert.IsNull(sigBit.OutValue.ErrorI(7));

            //BitOr
            Assert.AreEqual(beg.AddSeconds(10), sigBitOr.OutValue.TimeI(10));
            Assert.AreEqual(true, sigBitOr.OutValue.BooleanI(10));
            Assert.IsNull(sigBitOr.OutValue.ErrorI(10));
            Assert.AreEqual(beg.AddSeconds(56), sigBitOr.OutValue.TimeI(56));
            Assert.AreEqual(false, sigBitOr.OutValue.BooleanI(56));
            Assert.AreEqual("0", sigBitOr.OutValue.StringI(56));
            Assert.IsNull(sigBitOr.OutValue.ErrorI(56));

            //BitAnd
            Assert.AreEqual(61, sigBitAnd.OutValue.Count);
            Assert.AreEqual(beg.AddSeconds(3), sigBitAnd.OutValue.TimeI(3));
            Assert.AreEqual(false, sigBitAnd.OutValue.BooleanI(3));
            Assert.AreEqual(0.0, sigBitAnd.OutValue.RealI(3));
            Assert.AreEqual(beg.AddSeconds(31), sigBitAnd.OutValue.TimeI(31));
            Assert.AreEqual(1, sigBitAnd.OutValue.IntegerI(31));
            Assert.AreEqual(true, sigBitAnd.OutValue.BooleanI(31));
            
            //First
            Assert.AreEqual(40, sigFirst.OutValue.Count);
            Assert.AreEqual(beg, sigFirst.OutValue.TimeI(0));
            Assert.AreEqual(0.0, sigFirst.OutValue.RealI(0));
            Assert.IsNull(sigFirst.OutValue.ErrorI(0));
            Assert.AreEqual(beg.AddSeconds(9), sigFirst.OutValue.TimeI(6));
            Assert.AreEqual(4.5, sigFirst.OutValue.RealI(6));
            Assert.AreEqual("4,5", sigFirst.OutValue.StringI(6));
            Assert.IsNull(sigFirst.OutValue.ErrorI(6));
            Assert.AreEqual(beg.AddSeconds(16.5), sigFirst.OutValue.TimeI(11));
            Assert.AreEqual(8, sigFirst.OutValue.RealI(11));
            Assert.AreEqual(8, sigFirst.OutValue.IntegerI(11));
            Assert.IsNull(sigFirst.OutValue.ErrorI(11));
            Assert.AreEqual(beg.AddSeconds(58.5), sigFirst.OutValue.TimeI(39));
            Assert.AreEqual(29, sigFirst.OutValue.RealI(39));
            Assert.IsNull(sigFirst.OutValue.ErrorI(39));

            //Last
            Assert.AreEqual(40, sigLast.OutValue.Count);
            Assert.AreEqual(beg.AddSeconds(1.5), sigLast.OutValue.TimeI(0));
            Assert.AreEqual(0.5, sigLast.OutValue.RealI(0));
            Assert.AreEqual(beg.AddSeconds(10.5), sigLast.OutValue.TimeI(6));
            Assert.AreEqual(5, sigLast.OutValue.RealI(6));
            Assert.AreEqual("5", sigLast.OutValue.StringI(6));
            Assert.AreEqual(beg.AddSeconds(18), sigLast.OutValue.TimeI(11));
            Assert.AreEqual(9, sigLast.OutValue.RealI(11));
            Assert.AreEqual(beg.AddSeconds(60), sigLast.OutValue.TimeI(39));
            Assert.AreEqual(30, sigLast.OutValue.RealI(39));
            
            //Min
            Assert.AreEqual(40, sigMin.OutValue.Count);
            Assert.AreEqual(beg, sigMin.OutValue.TimeI(0));
            Assert.AreEqual(0.0, sigMin.OutValue.RealI(0));
            Assert.IsNull(sigMin.OutValue.ErrorI(0));
            Assert.AreEqual(beg.AddSeconds(9), sigMin.OutValue.TimeI(6));
            Assert.AreEqual(4.5, sigMin.OutValue.RealI(6));
            Assert.AreEqual("4,5", sigMin.OutValue.StringI(6));
            Assert.IsNull(sigMin.OutValue.ErrorI(6));
            Assert.AreEqual(beg.AddSeconds(16.5), sigMin.OutValue.TimeI(11));
            Assert.AreEqual(8, sigMin.OutValue.RealI(11));
            Assert.AreEqual(8, sigMin.OutValue.IntegerI(11));
            Assert.IsNull(sigMin.OutValue.ErrorI(11));
            Assert.AreEqual(beg.AddSeconds(58.5), sigMin.OutValue.TimeI(39));
            Assert.AreEqual(29, sigMin.OutValue.RealI(39));
            Assert.IsNull(sigMin.OutValue.ErrorI(39));

            //Max
            Assert.AreEqual(40, sigMax.OutValue.Count);
            Assert.AreEqual(beg.AddSeconds(1.5), sigMax.OutValue.TimeI(0));
            Assert.AreEqual(0.5, sigMax.OutValue.RealI(0));
            Assert.AreEqual(beg.AddSeconds(10.5), sigMax.OutValue.TimeI(6));
            Assert.AreEqual(5, sigMax.OutValue.RealI(6));
            Assert.AreEqual("5", sigMax.OutValue.StringI(6));
            Assert.AreEqual(beg.AddSeconds(18), sigMax.OutValue.TimeI(11));
            Assert.AreEqual(9, sigMax.OutValue.RealI(11));
            Assert.AreEqual(beg.AddSeconds(60), sigMax.OutValue.TimeI(39));
            Assert.AreEqual(30, sigMax.OutValue.RealI(39));

            //Average
            Assert.AreEqual(24, sigAverage.OutValue.Count);
            Assert.AreEqual(beg.AddSeconds(1.25), sigAverage.OutValue.TimeI(0));
            Assert.AreEqual(0.4, sigAverage.OutValue.RealI(0));
            Assert.AreEqual(beg.AddSeconds(16.25), sigAverage.OutValue.TimeI(6));
            Assert.AreEqual(7.9, sigAverage.OutValue.RealI(6));
            Assert.AreEqual(beg.AddSeconds(28.75), sigAverage.OutValue.TimeI(11));
            Assert.IsTrue(Math.Abs(sigAverage.OutValue.RealI(11)-14.1) < 0.0001);
            Assert.AreEqual(beg.AddSeconds(58.75), sigAverage.OutValue.TimeI(23));
            Assert.IsTrue(Math.Abs(sigAverage.OutValue.RealI(23) - 29.1) < 0.0001);
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
            Assert.AreEqual(2, connect.ReadingSignals.Count);
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

            Assert.AreEqual(601, sigi.OutValue.Count);
            Assert.AreEqual(beg, sigi.OutValue.TimeI(0));
            Assert.AreEqual(0, sigi.OutValue.IntegerI(0));
            Assert.AreEqual(0.0, sigi.OutValue.RealI(0));
            Assert.IsNull(sigi.OutValue.ErrorI(0));
            Assert.AreEqual(beg.AddSeconds(1), sigi.OutValue.TimeI(1));
            Assert.AreEqual(1, sigi.OutValue.IntegerI(1));
            Assert.AreEqual(true, sigi.OutValue.BooleanI(1));
            Assert.IsNull(sigi.OutValue.ErrorI(1));
            Assert.AreEqual(beg.AddSeconds(10), sigi.OutValue.TimeI(10));
            Assert.AreEqual(10, sigi.OutValue.IntegerI(10));
            Assert.AreEqual(10.0, sigi.OutValue.RealI(10));
            Assert.IsNull(sigi.OutValue.ErrorI(10));
            Assert.AreEqual(beg.AddSeconds(599), sigi.OutValue.TimeI(599));
            Assert.AreEqual(599, sigi.OutValue.IntegerI(599));
            Assert.AreEqual("599", sigi.OutValue.StringI(599));
            Assert.IsNull(sigi.OutValue.ErrorI(599));

            Assert.AreEqual(601, sigr.OutValue.Count);
            Assert.AreEqual(beg, sigr.OutValue.TimeI(0));
            Assert.AreEqual(0.0, sigr.OutValue.RealI(0));
            Assert.IsNull(sigr.OutValue.ErrorI(0));
            Assert.AreEqual(beg.AddSeconds(1), sigr.OutValue.TimeI(1));
            Assert.AreEqual(0.5, sigr.OutValue.RealI(1));
            Assert.IsNull(sigr.OutValue.ErrorI(1));
            Assert.AreEqual(beg.AddSeconds(10), sigr.OutValue.TimeI(10));
            Assert.AreEqual(5.0, sigr.OutValue.RealI(10));
            Assert.IsNull(sigr.OutValue.ErrorI(10));
            Assert.AreEqual(beg.AddSeconds(599), sigr.OutValue.TimeI(599));
            Assert.AreEqual(299.5, sigr.OutValue.RealI(599));
            Assert.AreEqual("299,5", sigr.OutValue.StringI(599));
            Assert.IsNull(sigr.OutValue.ErrorI(599));
        }
    }
}
