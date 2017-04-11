using System;
using System.Threading;
using BaseLibraryTest;
using ComLaunchers;
using Fictive;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProvidersLibrary;

namespace InfoTaskLauncherTest
{
    [TestClass]
    public class LauncherFictiveTest
    {
        [TestMethod]
        public void FictiveSimple()
        {
            var launcher = new TestItLauncher();
            launcher.Initialize("LauncherFictiveTest");
            launcher.LoadProjectByCode("FictiveSimple");
            Assert.AreEqual("LauncherFictiveTest", launcher.AppCode);
            Assert.AreEqual("FictiveSimple", launcher.ProjectCode);
            Assert.IsFalse(launcher.IsClosed);

            var con = (RSourConnect)launcher.CreateSourConnect("Sour", "Fictive");
            Assert.IsNotNull(con);
            Assert.AreEqual("Sour", con.Name);
            Assert.AreEqual("Fictive", con.Complect);
            Assert.AreEqual(ProviderType.Source, con.Type);
            con.JoinProvider("FictiveSimpleSource", "Label=fic");
            var source = (FictiveSimpleSource)con.Connect.Source;
            Assert.IsNotNull(source);
            Assert.AreEqual("FictiveSimpleSource", source.Code);

            con.GetTime();
            Assert.AreEqual(con.BeginTime, new DateTime(2000, 1, 1));
            Assert.AreEqual(con.EndTime, new DateTime(2100, 1, 1));

            con.ClearSignals();
            Assert.AreEqual(0, con.Connect.Signals.Count);
            var sigBool = con.AddInitialSignal("Out1.Bool", "bool", "NumObject=1;ValuesInterval=30000", "", "Signal=Bool");
            Assert.AreEqual("Out1.Bool", sigBool.Code);
            Assert.AreEqual("Логич", sigBool.DataType);
            Assert.AreEqual("NUMOBJECT=1;VALUESINTERVAL=30000;SIGNAL=Bool;", sigBool.Inf);
            var sigInt = con.AddInitialSignal("Out1.Int", "int", "NumObject=1;ValuesInterval=30000", "", "Signal=Int");
            Assert.AreEqual("Out1.Int", sigInt.Code);
            Assert.AreEqual("Целое", sigInt.DataType);
            var sigReal = con.AddInitialSignal("Out1.Real", "real", "NumObject=1;ValuesInterval=30000", "", "Signal=Real");
            Assert.AreEqual("Out1.Real", sigReal.Code);
            Assert.AreEqual("Действ", sigReal.DataType);
            var sigTime = con.AddInitialSignal("Out1.Time", "time", "NumObject=1;ValuesInterval=30000", "", "Signal=Time");
            Assert.AreEqual("Out1.Time", sigTime.Code);
            Assert.AreEqual("Время", sigTime.DataType);
            var sigString = con.AddInitialSignal("Out1.String", "string", "NumObject=1;ValuesInterval=30000", "", "Signal=String");
            Assert.AreEqual("Out1.String", sigString.Code);
            Assert.AreEqual("Строка", sigString.DataType);
            var sigValue = con.AddInitialSignal("Out2.Value", "real", "NumObject=2;ValuesInterval=10000", "", "Signal=Value");
            Assert.AreEqual("Out2.Value", sigValue.Code);
            Assert.AreEqual("Действ", sigValue.DataType);
            var sigState = con.AddInitialSignal("Out2.State", "int", "NumObject=2;ValuesInterval=10000", "", "Signal=State");
            Assert.AreEqual("Out2.State", sigState.Code);
            Assert.AreEqual("Целое", sigState.DataType);

            Assert.AreEqual(7, con.Connect.Signals.Count);
            Assert.IsTrue(con.Connect.Signals.ContainsKey("Out1.Bool"));
            Assert.IsTrue(con.Connect.Signals.ContainsKey("Out1.Int"));
            Assert.IsTrue(con.Connect.Signals.ContainsKey("Out1.Real"));
            Assert.IsTrue(con.Connect.Signals.ContainsKey("Out1.Time"));
            Assert.IsTrue(con.Connect.Signals.ContainsKey("Out1.String"));
            Assert.IsTrue(con.Connect.Signals.ContainsKey("Out2.Value"));
            Assert.IsTrue(con.Connect.Signals.ContainsKey("Out2.State"));

            con.GetValues(new DateTime(2017, 1, 1), new DateTime(2017, 1, 1, 0, 10, 0));
            Assert.AreEqual(2, source.Outs.Count);
            Assert.AreEqual("NumObject=1;ValuesInterval=30000", source.Outs[1].Context);
            Assert.AreEqual("NumObject=2;ValuesInterval=10000", source.Outs[2].Context);
            Assert.AreEqual(21, sigBool.MomsCount);

            DateTime d = new DateTime(2017, 1, 1);
            Assert.AreEqual(d, sigBool.Time(0));
            Assert.AreEqual(d, sigInt.Time(0));
            Assert.AreEqual(d, sigReal.Time(0));
            Assert.AreEqual(d, sigTime.Time(0));
            Assert.AreEqual(d, sigString.Time(0));
            Assert.AreEqual(d, sigValue.Time(0));
            Assert.AreEqual(d, sigState.Time(0));

            d = new DateTime(2017, 1, 1, 0, 0, 30);
            Assert.AreEqual(d, sigBool.Time(1));
            Assert.AreEqual(d, sigInt.Time(1));
            Assert.AreEqual(d, sigReal.Time(1));
            Assert.AreEqual(d, sigTime.Time(1));
            Assert.AreEqual(d, sigString.Time(1));

            d = new DateTime(2017, 1, 1, 0, 1, 0);
            Assert.AreEqual(d, sigBool.Time(2));
            Assert.AreEqual(d, sigInt.Time(2));
            Assert.AreEqual(d, sigReal.Time(2));
            Assert.AreEqual(d, sigTime.Time(2));
            Assert.AreEqual(d, sigString.Time(2));

            d = new DateTime(2017, 1, 1, 0, 1, 30);
            Assert.AreEqual(d, sigBool.Time(3));
            Assert.AreEqual(d, sigInt.Time(3));
            Assert.AreEqual(d, sigReal.Time(3));
            Assert.AreEqual(d, sigTime.Time(3));
            Assert.AreEqual(d, sigString.Time(3));

            d = new DateTime(2017, 1, 1, 0, 0, 10);
            Assert.AreEqual(d, sigValue.Time(1));
            Assert.AreEqual(d, sigState.Time(1));
            d = new DateTime(2017, 1, 1, 0, 0, 20);
            Assert.AreEqual(d, sigValue.Time(2));
            Assert.AreEqual(d, sigState.Time(2));
            d = new DateTime(2017, 1, 1, 0, 0, 30);
            Assert.AreEqual(d, sigValue.Time(3));
            Assert.AreEqual(d, sigState.Time(3));

            Assert.IsFalse(sigBool.Boolean(0));
            Assert.IsTrue(sigBool.Boolean(1));
            Assert.IsFalse(sigBool.Boolean(2));
            Assert.IsTrue(sigBool.Boolean(3));

            Assert.AreEqual(0, sigInt.Integer(0));
            Assert.AreEqual(1, sigInt.Integer(1));
            Assert.AreEqual(2, sigInt.Integer(2));
            Assert.AreEqual(3, sigInt.Integer(3));

            Assert.AreEqual(0.0, sigReal.Real(0));
            Assert.AreEqual(0.5, sigReal.Real(1));
            Assert.AreEqual(1.0, sigReal.Real(2));
            Assert.AreEqual(1.5, sigReal.Real(3));

            Assert.AreEqual(new DateTime(2017, 1, 1, 0, 0, 1), sigTime.Date(0));
            Assert.AreEqual(new DateTime(2017, 1, 1, 0, 0, 31), sigTime.Date(1));
            Assert.AreEqual(new DateTime(2017, 1, 1, 0, 1, 1), sigTime.Date(2));
            Assert.AreEqual(new DateTime(2017, 1, 1, 0, 1, 31), sigTime.Date(3));

            Assert.AreEqual("ss0", sigString.String(0));
            Assert.AreEqual("ss1", sigString.String(1));
            Assert.AreEqual("ss2", sigString.String(2));
            Assert.AreEqual("ss3", sigString.String(3));

            Assert.AreEqual(0, sigValue.Integer(0));
            Assert.AreEqual(1, sigValue.Integer(1));
            Assert.AreEqual(2, sigValue.Integer(2));
            Assert.AreEqual(3, sigValue.Integer(3));
            Assert.AreEqual(0, sigValue.ErrNumber(0));
            Assert.AreEqual(1, sigValue.ErrNumber(1));
            Assert.AreEqual(2, sigValue.ErrNumber(2));
            Assert.AreEqual(0, sigValue.ErrNumber(3));
            Assert.AreEqual(0, sigValue.ErrQuality(0));
            Assert.AreEqual(1, sigValue.ErrQuality(1));
            Assert.AreEqual(2, sigValue.ErrQuality(2));
            Assert.AreEqual(0, sigValue.ErrQuality(3));
            Assert.AreEqual(null, sigValue.ErrText(0));
            Assert.AreEqual("Предупреждение", sigValue.ErrText(1));
            Assert.AreEqual("Ошибка", sigValue.ErrText(2));
            Assert.AreEqual(null, sigValue.ErrText(3));

            Assert.AreEqual(10, sigState.Integer(0));
            Assert.AreEqual(10, sigState.Integer(1));
            Assert.AreEqual(10, sigState.Integer(2));
            Assert.AreEqual(10, sigState.Integer(3));

            con.ClearSignals();
            Assert.AreEqual(0, con.Connect.Signals.Count);

            launcher.Close();
            Assert.IsTrue(launcher.IsClosed);
        }

        [TestMethod]
        public void FictiveSimpleClone()
        {
            TestLib.CopyDir(@"Providers\Fictive", "FictiveSimpleClone");
            var launcher = new TestItLauncher();
            launcher.Initialize("LauncherFictiveTest");
            launcher.LoadProjectByCode("FictiveSimpleClone");
            Assert.IsFalse(launcher.IsClosed);

            var con = (RSourConnect)launcher.CreateSourConnect("Sour", "Fictive");
            con.JoinProvider("FictiveSimpleSource", "Label=fic");
            var cloneDir = TestLib.TestRunDir + @"Providers\Fictive\FictiveSimpleClone";
            con.MakeClone(new DateTime(2017, 1, 1), new DateTime(2017, 1, 1, 0, 10, 0), cloneDir);
            launcher.Close();
            Assert.IsTrue(launcher.IsClosed);

            TestLib.CompareClones(cloneDir + @"\Clone.accdb", cloneDir + @"\CorrectClone.accdb");
        }

        private DateTime Time(int m, int s = 0)
        {
            return new DateTime(2016, 7, 8).AddMinutes(m).AddSeconds(s);
        }

        [TestMethod]
        public void Fictive()
        {
            TestLib.CopyFile(@"Providers\Fictive", "Fictive.accdb", "FictiveLauncher.accdb");
            var launcher = new TestItLauncher();
            launcher.Initialize("LauncherFictiveTest");
            launcher.LoadProjectByCode("Fictive");
            var con = (RSourConnect)launcher.CreateSourConnect("Sour", "Fictive");
            con.JoinProvider("FictiveSource", "DbFile=" + TestLib.TestRunDir + @"Providers\Fictive\FictiveLauncher.accdb");

            var source = (FictiveSource)con.Connect.Source;
            Assert.IsNotNull(source);
            Assert.AreEqual("FictiveSource", source.Code);

            con.GetTime();
            Assert.AreEqual(con.BeginTime, Time(0));
            Assert.AreEqual(con.EndTime, Time(30));

            con.ClearSignals();
            Assert.AreEqual(0, con.Connect.Signals.Count);
            var sigBool = con.AddInitialSignal("Out1.Bool", "bool", "Table=MomValues;ObjectCode=Ob1", "", "Signal=Bool");
            Assert.AreEqual("Out1.Bool", sigBool.Code);
            Assert.AreEqual("Логич", sigBool.DataType);
            var sigInt = con.AddInitialSignal("Out1.Int", "int", "Table=MomValues;ObjectCode=Ob1", "", "Signal=Int");
            Assert.AreEqual("Out1.Int", sigInt.Code);
            Assert.AreEqual("Целое", sigInt.DataType);
            var sigReal = con.AddInitialSignal("Out1.Real", "real", "Table=MomValues;ObjectCode=Ob1", "", "Signal=Real");
            Assert.AreEqual("Out1.Real", sigReal.Code);
            Assert.AreEqual("Действ", sigReal.DataType);
            var sigTime = con.AddInitialSignal("Out1.Time", "time", "Table=MomValues;ObjectCode=Ob1", "", "Signal=Time");
            Assert.AreEqual("Out1.Time", sigTime.Code);
            Assert.AreEqual("Время", sigTime.DataType);
            var sigString = con.AddInitialSignal("Out1.String", "string", "Table=MomValues;ObjectCode=Ob1", "", "Signal=String");
            Assert.AreEqual("Out1.String", sigString.Code);
            Assert.AreEqual("Строка", sigString.DataType);
            var sigValue = con.AddInitialSignal("Out2.Value", "real", "Table=MomValues;ObjectCode=Ob2", "", "Signal=Value");
            Assert.AreEqual("Out2.Value", sigValue.Code);
            Assert.AreEqual("Действ", sigValue.DataType);
            var sigState = con.AddInitialSignal("Out2.State", "int", "Table=MomValues;ObjectCode=Ob2", "", "Signal=State");
            Assert.AreEqual("Out2.State", sigState.Code);
            Assert.AreEqual("Целое", sigState.DataType);
            var sigValueX = con.AddInitialSignal("OutX.Value", "real", "Table=MomValues2;ObjectCode=ObX", "", "Signal=Value");
            Assert.AreEqual("OutX.Value", sigValueX.Code);
            Assert.AreEqual("Действ", sigValueX.DataType);
            var sigValue2X = con.AddInitialSignal("OutX.Value2", "real", "Table=MomValues2;ObjectCode=ObX", "", "Signal=Value2");
            Assert.AreEqual("OutX.Value2", sigValue2X.Code);
            Assert.AreEqual("Действ", sigValue2X.DataType);

            Assert.AreEqual(9, con.Connect.Signals.Count);
            con.GetValues(Time(10), Time(20));
            Assert.AreEqual(2, source.Outs.Count);
            Assert.IsTrue(source.Outs.ContainsKey("Ob1"));
            Assert.IsTrue(source.Outs.ContainsKey("Ob2"));
            Assert.AreEqual(1, source.Outs2.Count);
            Assert.IsTrue(source.Outs2.ContainsKey("ObX"));

            Assert.AreEqual(11, sigBool.MomsCount);
            Assert.AreEqual(11, sigInt.MomsCount);
            Assert.AreEqual(11, sigReal.MomsCount);
            Assert.AreEqual(11, sigTime.MomsCount);
            Assert.AreEqual(11, sigString.MomsCount);
            Assert.AreEqual(11, sigValue.MomsCount);
            Assert.AreEqual(11, sigState.MomsCount);
            Assert.AreEqual(2, sigValueX.MomsCount);
            Assert.AreEqual(2, sigValue2X.MomsCount);
            
            Assert.AreEqual(Time(0), sigValueX.Time(0));
            Assert.AreEqual(Time(15), sigValueX.Time(1));
            Assert.AreEqual(Time(0), sigValue2X.Time(0));
            Assert.AreEqual(Time(15), sigValue2X.Time(1));
            Assert.AreEqual(1, sigValueX.Real(0));
            Assert.AreEqual(2, sigValueX.Real(1));
            Assert.AreEqual(2, sigValue2X.Real(0));
            Assert.AreEqual(4, sigValue2X.Real(1));

            Assert.AreEqual(Time(10), sigInt.Time(0));
            Assert.AreEqual(Time(11), sigInt.Time(1));
            Assert.AreEqual(Time(13), sigInt.Time(2));
            Assert.AreEqual(Time(14), sigInt.Time(3));
            Assert.AreEqual(Time(15, 30), sigInt.Time(4));
            Assert.AreEqual(Time(16), sigInt.Time(5));
            Assert.AreEqual(Time(16, 30), sigInt.Time(6));
            Assert.AreEqual(Time(18), sigInt.Time(7));
            Assert.AreEqual(Time(19), sigInt.Time(8));
            Assert.AreEqual(Time(19, 30), sigInt.Time(9));
            Assert.AreEqual(Time(20), sigInt.Time(10));

            Assert.AreEqual(Time(10), sigString.Time(0));
            Assert.AreEqual(Time(11), sigString.Time(1));
            Assert.AreEqual(Time(13), sigString.Time(2));
            Assert.AreEqual(Time(14), sigString.Time(3));
            Assert.AreEqual(Time(15, 30), sigString.Time(4));
            Assert.AreEqual(Time(16), sigString.Time(5));
            Assert.AreEqual(Time(16, 30), sigString.Time(6));
            Assert.AreEqual(Time(18), sigString.Time(7));
            Assert.AreEqual(Time(19), sigString.Time(8));
            Assert.AreEqual(Time(19, 30), sigString.Time(9));
            Assert.AreEqual(Time(20), sigString.Time(10));

            Assert.AreEqual(0, sigReal.Real(0));
            Assert.AreEqual(-1.5, sigReal.Real(1));
            Assert.AreEqual(4, sigReal.Real(2));
            Assert.AreEqual(4, sigReal.Real(3));
            Assert.AreEqual(4.5, sigReal.Real(4));
            Assert.AreEqual(4.5, sigReal.Real(5));
            Assert.AreEqual(4.5, sigReal.Real(6));
            Assert.AreEqual(3, sigReal.Real(7));
            Assert.AreEqual(3, sigReal.Real(8));
            Assert.AreEqual(2.5, sigReal.Real(9));
            Assert.AreEqual(2.5, sigReal.Real(10));

            Assert.AreEqual(0, sigReal.ErrNumber(0));
            Assert.AreEqual(0, sigReal.ErrQuality(0));
            Assert.AreEqual(2, sigReal.ErrNumber(2));
            Assert.AreEqual(2, sigReal.ErrQuality(2));
            Assert.AreEqual(0, sigReal.ErrNumber(3));
            Assert.AreEqual(0, sigReal.ErrQuality(3));
            Assert.AreEqual(0, sigReal.ErrNumber(8));
            Assert.AreEqual(0, sigReal.ErrQuality(8));
            Assert.AreEqual(1, sigReal.ErrNumber(9));
            Assert.AreEqual(1, sigReal.ErrQuality(9));
            Assert.AreEqual(1, sigReal.ErrNumber(10));
            Assert.AreEqual(1, sigReal.ErrQuality(10));

            Assert.AreEqual("0s", sigString.String(0));
            Assert.AreEqual("-1,5s", sigString.String(1));
            Assert.AreEqual("4s", sigString.String(2));
            Assert.AreEqual("4s", sigString.String(3));
            Assert.AreEqual("4,5s", sigString.String(4));
            Assert.AreEqual("4,5s", sigString.String(5));
            Assert.AreEqual("4,5s", sigString.String(6));
            Assert.AreEqual("3s", sigString.String(7));
            Assert.AreEqual("3s", sigString.String(8));
            Assert.AreEqual("2,5s", sigString.String(9));
            Assert.AreEqual("2,5s", sigString.String(10));

            con.ClearSignals();
            Assert.AreEqual(0, con.Connect.Signals.Count);
            launcher.Close();
        }

        [TestMethod]
        public void FictiveClone()
        {
            TestLib.CopyDir(@"Providers\Fictive", "FictiveClone");
            Thread.Sleep(500);
            TestLib.CopyFile(@"Providers\Fictive", "Fictive.accdb", @"FictiveClone\Fictive.accdb");
            var launcher = new TestItLauncher();
            launcher.Initialize("LauncherFictiveTest");
            launcher.LoadProjectByCode("FictiveClone");
            var con = (RSourConnect)launcher.CreateSourConnect("Sour", "Fictive");
            string cloneDir = TestLib.TestRunDir + @"Providers\Fictive\FictiveClone\";
            con.JoinProvider("FictiveSource", "DbFile=" + cloneDir + "Fictive.accdb");
            
            con.MakeClone(new DateTime(2016, 7, 8), new DateTime(2016, 7, 8, 0, 30, 0), cloneDir);
            launcher.Close();
            Assert.IsTrue(launcher.IsClosed);
            TestLib.CompareClones(cloneDir + @"\Clone.accdb", cloneDir + @"\CorrectClone.accdb");
        }
    }
}