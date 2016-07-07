using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using BaseLibrary;
using CommonTypes;
using Fictive;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProvidersLibrary;

namespace ProvidersLibraryTest
{
    [TestClass]
    public class SignalsTest
    {
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
            
            Assert.AreEqual("Ob.Int", sig1.Code);
            Assert.AreEqual(DataType.Integer, sig1.DataType);
            Assert.IsTrue(sig1.IsReal);
            Assert.AreEqual(3, sig1.Inf.Count);
            Assert.AreEqual("1", sig1.Inf["NumObject"]);
            Assert.AreEqual("Int", sig1.Inf["Signal"]);
            Assert.AreEqual("2000", sig1.Inf["ValuesInterval"]);

            Assert.IsTrue(source.Objects.ContainsKey(1));
            ObjectFictive ob1 = source.Objects[1];
            Assert.AreEqual("Ob", ob1.Context);
            Assert.IsNotNull(ob1.IntSignal);
            Assert.IsNull(ob1.RealSignal);
            Assert.IsNull(ob1.ValueSignal);
            Assert.AreEqual("Ob.Int", ob1.IntSignal.Code);
            Assert.AreEqual(false, ob1.IsInitialized);
            Assert.AreEqual(2000, ob1.ValuesInterval);

            var sig2 = (CalcSignal)source.AddSignal("Ob.Int", "Ob", DataType.Integer, "NumObject=2;Signal=Int;ValuesInterval=2000");
        }

        private FictiveSource MakeFictiveSource()
        {
            var logger = new Logger();
            var factory = new ProvidersFactory();
            return (FictiveSource)factory.CreateProvider(logger, "FictiveSource", "");
        }
    }
}
