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
        private SourceConnect MakeFictiveConnect(bool makeReserve = false)
        {
            var factory = new ProvidersFactory();
            var connect = (SourceConnect)factory.CreateConnect(ProviderType.Source, "TestSource", "Fictive", new Logger());
            TestLib.CopyFile(@"ProvidersLibrary\Fictive.accdb");
            var source = (FictiveSource)factory.CreateProvider("FictiveSource", @"DbFile=" + TestLib.TestRunDir + @"ProvidersLibrary\Fictive.accdb");
            FictiveSource source2 = null;
            if (makeReserve)
                source2 = (FictiveSource)factory.CreateProvider("FictiveSource", @"DbFile=" + TestLib.TestRunDir + @"ProvidersLibrary\Fictive.accdb");
            connect.JoinProviders(source, source2);
            return connect;
        }

        [TestMethod]
        public void Signals()
        {
            var connect = MakeFictiveConnect();
            var source = (FictiveSource)connect.Provider;
            Assert.AreEqual("TestSource", connect.Name);
            Assert.AreEqual("Fictive", connect.Complect);
            Assert.AreEqual("Источник: TestSource", connect.Context);
            Assert.IsNotNull(source);
            Assert.AreEqual("FictiveSource", source.Code);
            Assert.AreEqual("Источник: TestSource, FictiveSource", source.Context);

            connect.AddInitialSignal("Ob1.StateSignal", "Ob1", DataType.Integer, "Table=Values;NumObject=1;Signal=State", true);
            connect.AddInitialSignal("Ob1.ValueSignal", "Ob1", DataType.Real, "Table=Values;NumObject=1;Signal=Value", true);
            connect.AddInitialSignal("Ob1.BoolSignal", "Ob1", DataType.Boolean, "Table=Values;NumObject=1;Signal=Bool", true);
            connect.AddInitialSignal("Ob1.IntSignal", "Ob1", DataType.Integer, "Table=Values;NumObject=1;Signal=Int", true);
            connect.AddInitialSignal("Ob1.RealSignal", "Ob1", DataType.Real, "Table=Values;NumObject=1;Signal=Real", true);
            connect.AddInitialSignal("Ob1.TimeSignal", "Ob1", DataType.Time, "Table=Values;NumObject=1;Signal=Time", true);
            connect.AddInitialSignal("Ob1.StringSignal", "Ob1", DataType.String, "Table=Values;NumObject=1;Signal=String", true);
            Assert.AreEqual(7, connect.Signals.Count);
            Assert.AreEqual(7, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);

            connect.AddInitialSignal("Ob2.StateSignal", "Ob2", DataType.Integer, "Table=Values;NumObject=2;Signal=State", true);
            connect.AddInitialSignal("Ob2.ValueSignal", "Ob2", DataType.Real, "Table=Values;NumObject=2;Signal=Value", true);
            connect.AddInitialSignal("Ob2.BoolSignal", "Ob2", DataType.Boolean, "Table=Values;NumObject=2;Signal=Bool", true);
            connect.AddInitialSignal("Ob2.IntSignal", "Ob2", DataType.Integer, "Table=Values;NumObject=2;Signal=Int", true);
            connect.AddInitialSignal("Ob2.RealSignal", "Ob2", DataType.Real, "Table=Values;NumObject=2;Signal=Real", true);
            connect.AddInitialSignal("Ob2.TimeSignal", "Ob2", DataType.Time, "Table=Values;NumObject=2;Signal=Time", true);
            connect.AddInitialSignal("Ob2.StringSignal", "Ob2", DataType.String, "Table=Values;NumObject=2;Signal=String", true);
            Assert.AreEqual(14, connect.Signals.Count);
            Assert.AreEqual(14, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);

            connect.AddInitialSignal("Ob3.StateSignal", "Ob3", DataType.Integer, "Table=Values;NumObject=3;Signal=State", true);
            connect.AddInitialSignal("Ob3.ValueSignal", "Ob3", DataType.Real, "Table=Values;NumObject=3;Signal=Value", true);
            connect.AddInitialSignal("Ob3.BoolSignal", "Ob3", DataType.Boolean, "Table=Values;NumObject=3;Signal=Bool", true);
            connect.AddInitialSignal("Ob3.IntSignal", "Ob3", DataType.Integer, "Table=Values;NumObject=3;Signal=Int", true);
            connect.AddInitialSignal("Ob3.RealSignal", "Ob3", DataType.Real, "Table=Values;NumObject=3;Signal=Real", true);
            connect.AddInitialSignal("Ob3.TimeSignal", "Ob3", DataType.Time, "Table=Values;NumObject=3;Signal=Time", true);
            connect.AddInitialSignal("Ob3.StringSignal", "Ob3", DataType.String, "Table=Values;NumObject=3;Signal=String", true);
            Assert.AreEqual(21, connect.Signals.Count);
            Assert.AreEqual(21, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);

            connect.AddInitialSignal("ObX.ValueSignal", "ObX", DataType.Real, "Table=Values2;NumObject=5;Signal=Value", true);
            connect.AddInitialSignal("ObX.Value2Signal", "ObX", DataType.Real, "Table=Values2;NumObject=5;Signal=Value2", true);
            Assert.AreEqual(23, connect.Signals.Count);
            Assert.AreEqual(23, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);

            connect.AddInitialSignal("ObY.ValueSignal", "ObY", DataType.Real, "Table=Values2;NumObject=6;Signal=Value", true);
            connect.AddInitialSignal("ObY.Value2Signal", "ObY", DataType.Real, "Table=Values2;NumObject=6;Signal=Value2", true);
            Assert.AreEqual(25, connect.Signals.Count);
            Assert.AreEqual(25, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);

            connect.AddInitialSignal("ObZ.ValueSignal", "ObZ", DataType.Real, "Table=Values2;NumObject=7;Signal=Value", true);
            connect.AddInitialSignal("ObZ.Value2Signal", "ObZ", DataType.Real, "Table=Values2;NumObject=7;Signal=Value2", true);
            Assert.AreEqual(27, connect.Signals.Count);
            Assert.AreEqual(27, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);

            connect.AddInitialSignal("Operator.CommandText", "Operator", DataType.String, "Table=Operator;NumObject=8;Signal=CommandText", false);
            connect.AddInitialSignal("Operator.CommandNumber", "Operator", DataType.Integer, "Table=Operator;NumObject=8;Signal=CommandNumber", false);
            Assert.AreEqual(29, connect.Signals.Count);
            Assert.AreEqual(29, connect.InitialSignals.Count);
            Assert.AreEqual(0, connect.CalcSignals.Count);

            connect.Prepare();
            Assert.AreEqual(3, source.Objects.Count);
            Assert.AreEqual(3, source.ObjectsId.Count);
            Assert.AreEqual(3, source.Objects2.Count);
            Assert.AreEqual(3, source.ObjectsId2.Count);
            Assert.IsNotNull(source.OperatorObject);

            Assert.IsTrue(source.ObjectsId.ContainsKey(11));
            Assert.IsTrue(source.ObjectsId.ContainsKey(12));
            Assert.IsTrue(source.ObjectsId.ContainsKey(13));
            Assert.IsTrue(source.Objects.ContainsKey("Ob1"));
            Assert.IsTrue(source.Objects.ContainsKey("Ob2"));
            Assert.IsTrue(source.Objects.ContainsKey("Ob3"));
            Assert.IsTrue(source.ObjectsId2.ContainsKey(15));
            Assert.IsTrue(source.ObjectsId2.ContainsKey(16));
            Assert.IsTrue(source.ObjectsId2.ContainsKey(17));
            Assert.IsTrue(source.Objects2.ContainsKey("ObX"));
            Assert.IsTrue(source.Objects2.ContainsKey("ObY"));
            Assert.IsTrue(source.Objects2.ContainsKey("ObZ"));
            Assert.IsNotNull(source.OperatorObject);
            Assert.IsNotNull(source.ErrPool);

            var ti = connect.GetTime();
            Assert.AreEqual(new DateTime(2016, 07, 08, 0, 0, 0), ti.Begin);
            Assert.AreEqual(new DateTime(2016, 07, 08, 0, 30, 0), ti.End);
        }
    }
}