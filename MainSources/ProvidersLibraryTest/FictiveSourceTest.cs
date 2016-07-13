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
        private FictiveSource MakeFictiveSource()
        {
            var logger = new Logger();
            var factory = new ProvidersFactory();
            return (FictiveSource)factory.CreateProvider(logger, "FictiveSource", "TestSource", "");
        }

        [TestMethod]
        public void Signals()
        {
            FictiveSource source = MakeFictiveSource();
            Assert.AreEqual(0, source.Signals.Count);
            var sig1 = (InitialSignal)source.AddSignal("Ob.Int", "Ob", DataType.Integer, "NumObject=1;Signal=Int;ValuesInterval=2000");
            Assert.AreEqual(1, source.Signals.Count);
            Assert.AreEqual(1, source.ProviderSignals.Count);
            Assert.AreEqual(0, source.CalcSignals.Count);
            Assert.AreEqual(1, source.Objects.Count);
            
            Assert.AreEqual(DataType.Integer, sig1.DataType);
            Assert.AreEqual(3, sig1.Inf.Count);
            Assert.AreEqual("1", sig1.Inf["NumObject"]);
            Assert.AreEqual("Int", sig1.Inf["Signal"]);
            Assert.AreEqual("2000", sig1.Inf["ValuesInterval"]);
            Assert.AreEqual("FictiveSource", sig1.Source.Code);
            Assert.AreEqual("TestSource", sig1.Source.Name);

            Assert.IsTrue(source.Objects.ContainsKey(1));
            ObjectFictive ob1 = source.Objects[1];
            Assert.AreEqual("Ob", ob1.Context);
            Assert.IsNotNull(ob1.IntSignal);
            Assert.IsNull(ob1.RealSignal);
            Assert.IsNull(ob1.ValueSignal);
            Assert.IsFalse(ob1.IsInitialized);
            Assert.AreEqual(2000, ob1.ValuesInterval);
            
            var sig2 = (InitialSignal)source.AddSignal("Ob.Value", "Ob", DataType.Real, "NumObject=1;Signal=Value;ValuesInterval=2000");
            Assert.AreEqual(2, source.Signals.Count);
            Assert.AreEqual(2, source.ProviderSignals.Count);
            Assert.AreEqual(0, source.CalcSignals.Count);
            Assert.AreEqual(1, source.Objects.Count);

            Assert.AreEqual(DataType.Real, sig2.DataType);
            Assert.AreEqual(3, sig2.Inf.Count);
            Assert.AreEqual("1", sig2.Inf["NumObject"]);
            Assert.AreEqual("Value", sig2.Inf["Signal"]);
            Assert.AreEqual("2000", sig2.Inf["ValuesInterval"]);
            Assert.IsNotNull(ob1.ValueSignal);

            var sig3 = (CalcSignal)source.AddSignal("Ob.Bit3", "Ob", DataType.Integer, "NumObject=1;Signal=Int;ValuesInterval=2000", "Bit;1;3");
            Assert.AreEqual(3, source.Signals.Count);
            Assert.AreEqual(3, source.ProviderSignals.Count);
            Assert.AreEqual(1, source.CalcSignals.Count);
            Assert.AreEqual(1, source.Objects.Count);

            Assert.AreEqual(DataType.Boolean, sig3.DataType);
            Assert.IsNull(sig3.Inf);
            Assert.AreEqual("Bit", sig3.Calculate.Method.Name);
            Assert.AreEqual("FictiveSource", sig3.Source.Code);
            Assert.AreEqual("TestSource", sig3.Source.Name);

            var sig4 = (CalcSignal)source.AddSignal("Ob2.Bit4or5", "Ob2", DataType.Integer, "NumObject=2;Signal=Int;ValuesInterval=1000", "BitOr;2;4;5");
            Assert.AreEqual(4, source.Signals.Count);
            Assert.AreEqual(4, source.ProviderSignals.Count);
            Assert.AreEqual(2, source.CalcSignals.Count);
            Assert.AreEqual(2, source.Objects.Count);

            Assert.AreEqual(DataType.Boolean, sig4.DataType);
            Assert.IsNull(sig4.Inf);
            Assert.AreEqual("BitOr", sig4.Calculate.Method.Name);
            Assert.AreEqual("FictiveSource", sig3.Source.Code);
            Assert.AreEqual("TestSource", sig3.Source.Name);

            Assert.IsTrue(source.Objects.ContainsKey(2));
            ObjectFictive ob2 = source.Objects[2];
            Assert.AreEqual("Ob2", ob2.Context);
            Assert.IsNotNull(ob2.IntSignal);
            Assert.IsNull(ob2.ValueSignal);
            Assert.IsFalse(ob2.IsInitialized);
            Assert.AreEqual(1000, ob2.ValuesInterval);

            source.Prepare();
            Assert.AreEqual(new DateTime(2000, 1, 1), source.GetTime().Begin);
            Assert.AreEqual(new DateTime(2100, 1, 1), source.GetTime().End);
            Assert.IsTrue(ob1.IsInitialized);
            Assert.IsTrue(ob2.IsInitialized);

            source.ClearSignals();
            Assert.AreEqual(0, source.Signals.Count);
            Assert.AreEqual(0, source.ProviderSignals.Count);
            Assert.AreEqual(0, source.CalcSignals.Count);
            Assert.AreEqual(0, source.Objects.Count);
        }

        [TestMethod]
        public void InitialValues()
        {
            FictiveSource source = MakeFictiveSource();
            var beg = new DateTime(2007, 11, 20, 10, 30, 0);
            var en = new DateTime(2007, 11, 20, 10, 40, 0);
            source.GetValues(beg, en);
            Assert.AreEqual(0, source.Signals.Count);

            var sigi = source.AddSignal("Ob.Int", "Ob", DataType.Integer, "NumObject=1;Signal=Int;ValuesInterval=1000");
            var sigr = source.AddSignal("Ob.Real", "Ob", DataType.Real, "NumObject=1;Signal=Real;ValuesInterval=1000");
            Assert.AreEqual(2, source.Signals.Count);
            source.GetValues(beg, en);
            Assert.AreEqual(600, sigi.MomList.Count);
            Assert.AreEqual(beg.AddSeconds(1), sigi.MomList.Time(0));
            Assert.AreEqual(0, sigi.MomList.Integer(0));
            Assert.AreEqual(0.0, sigi.MomList.Real(0));
            Assert.IsNull(sigi.MomList.Error(0));
            Assert.AreEqual(beg.AddSeconds(2), sigi.MomList.Time(1));
            Assert.AreEqual(1, sigi.MomList.Integer(1));
            Assert.AreEqual(true, sigi.MomList.Boolean(1));
            Assert.IsNull(sigi.MomList.Error(1));
            Assert.AreEqual(beg.AddSeconds(11), sigi.MomList.Time(10));
            Assert.AreEqual(10, sigi.MomList.Integer(10));
            Assert.AreEqual(10.0, sigi.MomList.Real(10));
            Assert.IsNull(sigi.MomList.Error(10));
            Assert.AreEqual(beg.AddSeconds(600), sigi.MomList.Time(599));
            Assert.AreEqual(599, sigi.MomList.Integer(599));
            Assert.AreEqual("599", sigi.MomList.String(599));
            Assert.IsNull(sigi.MomList.Error(599));

            Assert.AreEqual(600, sigr.MomList.Count);
            Assert.AreEqual(beg.AddSeconds(1), sigr.MomList.Time(0));
            Assert.AreEqual(0.0, sigr.MomList.Real(0));
            Assert.IsNull(sigr.MomList.Error(0));
            Assert.AreEqual(beg.AddSeconds(2), sigr.MomList.Time(1));
            Assert.AreEqual(0.5, sigr.MomList.Real(1));
            Assert.IsNull(sigr.MomList.Error(1));
            Assert.AreEqual(beg.AddSeconds(11), sigr.MomList.Time(10));
            Assert.AreEqual(5.0, sigr.MomList.Real(10));
            Assert.IsNull(sigr.MomList.Error(10));
            Assert.AreEqual(beg.AddSeconds(600), sigr.MomList.Time(599));
            Assert.AreEqual(299.5, sigr.MomList.Real(599));
            Assert.AreEqual("299,5", sigr.MomList.String(599));
            Assert.IsNull(sigr.MomList.Error(599));
        }
    }
}
