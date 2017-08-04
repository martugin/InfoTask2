using AppLibrary;
using BaseLibrary;
using BaseLibraryTest;
using CommonTypes;
using CompileLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tablik;

namespace TablikTest
{
    [TestClass]
    public class ProjectTest
    {
        private TablikProject LoadProject(string projectCode, string projectSuffix, params string[] modules)
        {
            TestLib.CopyDir("Tablik", projectCode, projectCode + projectSuffix);
            var app = new App("Constructor", new TestIndicator(), true);
            var proj = new AppProject(app, TestLib.TestRunDir + @"Tablik\" + projectCode + projectSuffix);
            var tablik = proj.Tablik;
            tablik.LoadAllSignals();
            foreach (var module in modules)
                tablik.AddModule(module);
            return tablik;
        }

        [TestMethod]
        public void TablikProjectLoad()
        {
            var t = LoadProject("TablikProject", "Load", "Mod1", "Mod2");

            Assert.IsNotNull(t);
            Assert.IsNotNull(t.Project);
            Assert.IsNotNull(t.FunsChecker);
            Assert.IsNotNull(t.Logger);
            Assert.AreEqual("Constructor", t.Project.App.Code);
            Assert.AreEqual("TablikProject", t.Project.Code);

            Assert.AreEqual(2, t.Project.SchemeConnects.Count);
            Assert.AreEqual(2, t.Project.SchemeSources.Count);
            Assert.IsTrue(t.Project.SchemeSources.ContainsKey("Sour1"));
            var ss = t.Project.SchemeSources["Sour1"];
            Assert.AreEqual(ProviderType.Source, ss.Type);
            Assert.AreEqual("Sour1", ss.Code);
            Assert.AreEqual("Fictive", ss.Complect);
            Assert.IsTrue(t.Project.SchemeSources.ContainsKey("Sour2"));
            ss = t.Project.SchemeSources["Sour2"];
            Assert.AreEqual(ProviderType.Source, ss.Type);
            Assert.AreEqual("Sour2", ss.Code);
            Assert.AreEqual("Fictive", ss.Complect);

            Assert.AreEqual(2, t.Project.SchemeModules.Count);
            Assert.IsTrue(t.Project.SchemeModules.ContainsKey("Mod1"));
            var sm = t.Project.SchemeModules["Mod1"];
            Assert.AreEqual("Mod1", sm.Code);
            Assert.AreEqual(0, sm.LinkedModules.Count);
            Assert.AreEqual(2, sm.LinkedConnects.Count);
            Assert.IsTrue(sm.LinkedConnects.Contains("Sour1"));
            Assert.IsTrue(sm.LinkedConnects.Contains("Sour2"));
            
            Assert.IsTrue(t.Project.SchemeModules.ContainsKey("Mod2"));
            sm = t.Project.SchemeModules["Mod2"];
            Assert.AreEqual("Mod2", sm.Code);
            Assert.AreEqual(1, sm.LinkedModules.Count);
            Assert.IsTrue(sm.LinkedModules.Contains("Mod1"));
            Assert.AreEqual(1, sm.LinkedConnects.Count);
            Assert.IsTrue(sm.LinkedConnects.Contains("Sour1"));

            Assert.AreEqual(2, t.Modules.Count);
            Assert.IsTrue(t.Modules.ContainsKey("Mod1"));
            Assert.IsTrue(t.Modules.ContainsKey("Mod2"));
            Assert.AreEqual(2, t.Sources.Count);
            Assert.IsTrue(t.Sources.ContainsKey("Sour1"));
            Assert.IsTrue(t.Sources.ContainsKey("Sour2"));

            var s = t.Sources["Sour1"];
            Assert.AreEqual("Sour1", s.Code);
            Assert.AreEqual(2, s.ObjectsTypesId.Count);
            Assert.AreEqual(4, s.ObjectsTypes.Count);
            Assert.AreEqual(3, s.ObjectsCalcTypesId.Count);
            Assert.AreEqual(6, s.ObjectsCalcTypes.Count);
            Assert.AreEqual(4, s.Objects.Count);

            Assert.IsTrue(s.ObjectsTypes.ContainsKey("Large"));
            Assert.IsTrue(s.ObjectsTypes.ContainsKey("Sour1.Large"));
            var ot = s.ObjectsTypes["Large"];
            Assert.AreEqual("Large", ot.Code);
            Assert.AreEqual("Большой объект", ot.Name);
            Assert.AreEqual(3, ot.BaseTypes.Count);
            Assert.IsTrue(ot.BaseTypes.Contains(s.ObjectsCalcTypes["Analog"]));
            Assert.IsTrue(ot.BaseTypes.Contains(s.ObjectsCalcTypes["State"]));
            Assert.IsTrue(ot.BaseTypes.Contains(s.ObjectsCalcTypes["Discret"]));
            Assert.AreEqual(5, ot.Signals.Count);
            Assert.IsTrue(ot.Signals.ContainsKey("Bool"));
            Assert.IsTrue(ot.Signals.ContainsKey("Int"));
            Assert.IsTrue(ot.Signals.ContainsKey("Real"));
            Assert.AreEqual(ot.Signal, ot.Signals["Real"]);
            Assert.IsTrue(ot.Signals.ContainsKey("Time"));
            Assert.IsTrue(ot.Signals.ContainsKey("String"));

            var sig = ot.Signals["Int"];
            Assert.AreEqual("Int", sig.Code);
            Assert.AreEqual(sig, sig.Signal);
            Assert.AreEqual("Целый сигнал", sig.Name);
            Assert.AreEqual(DataType.Integer, sig.DataType);
            Assert.AreEqual(DataType.Integer, sig.Simple.DataType);
            Assert.AreEqual(ArrayType.Single, sig.Simple.ArrayType);
            Assert.AreEqual(null, sig.Simple.TablikSignalType);
            sig = ot.Signals["Bool"];
            Assert.AreEqual("Bool", sig.Code);
            Assert.AreEqual(sig, sig.Signal);
            Assert.AreEqual("Логический сигнал", sig.Name);
            Assert.AreEqual(DataType.Boolean, sig.DataType);
            Assert.AreEqual(DataType.Boolean, sig.Simple.DataType);
            Assert.AreEqual(ArrayType.Single, sig.Simple.ArrayType);
            Assert.AreEqual(null, sig.Simple.TablikSignalType);

            Assert.IsTrue(s.ObjectsTypes.ContainsKey("Small"));
            Assert.IsTrue(s.ObjectsTypes.ContainsKey("Sour1.Small"));
            ot = s.ObjectsTypes["Small"];
            Assert.AreEqual("Small", ot.Code);
            Assert.AreEqual("Маленький объект", ot.Name);
            Assert.AreEqual(2, ot.BaseTypes.Count);
            Assert.IsTrue(ot.BaseTypes.Contains(s.ObjectsCalcTypes["Analog"]));
            Assert.IsTrue(ot.BaseTypes.Contains(s.ObjectsCalcTypes["State"]));
            Assert.AreEqual(2, ot.Signals.Count);
            Assert.IsTrue(ot.Signals.ContainsKey("State"));
            Assert.IsTrue(ot.Signals.ContainsKey("Value"));
            Assert.AreEqual(ot.Signal, ot.Signals["Value"]);

            sig = ot.Signals["State"];
            Assert.AreEqual("State", sig.Code);
            Assert.AreEqual(sig, sig.Signal);
            Assert.AreEqual("Состояние", sig.Name);
            Assert.AreEqual(DataType.Integer, sig.DataType);
            Assert.AreEqual(DataType.Integer, sig.Simple.DataType);
            Assert.AreEqual(ArrayType.Single, sig.Simple.ArrayType);
            Assert.AreEqual(null, sig.Simple.TablikSignalType);
            sig = ot.Signals["Value"];
            Assert.AreEqual("Value", sig.Code);
            Assert.AreEqual(sig, sig.Signal);
            Assert.AreEqual("Значение", sig.Name);
            Assert.AreEqual(DataType.Real, sig.DataType);
            Assert.AreEqual(DataType.Real, sig.Simple.DataType);
            Assert.AreEqual(ArrayType.Single, sig.Simple.ArrayType);
            Assert.AreEqual(null, sig.Simple.TablikSignalType);
        }
    }
}