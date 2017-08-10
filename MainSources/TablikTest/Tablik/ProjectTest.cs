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
            var tablik = LoadProject("TablikProject", "Load", "Mod1", "Mod2");

            Assert.IsNotNull(tablik);
            Assert.IsNotNull(tablik.Project);
            Assert.IsNotNull(tablik.FunsChecker);
            Assert.IsNotNull(tablik.Logger);
            Assert.AreEqual("Constructor", tablik.Project.App.Code);
            Assert.AreEqual("TablikProject", tablik.Project.Code);

            Assert.AreEqual(2, tablik.Project.SchemeConnects.Count);
            Assert.AreEqual(2, tablik.Project.SchemeSources.Count);
            Assert.IsTrue(tablik.Project.SchemeSources.ContainsKey("Sour1"));
            var ss = tablik.Project.SchemeSources["Sour1"];
            Assert.AreEqual(ProviderType.Source, ss.Type);
            Assert.AreEqual("Sour1", ss.Code);
            Assert.AreEqual("Fictive", ss.Complect);
            Assert.IsTrue(tablik.Project.SchemeSources.ContainsKey("Sour2"));
            ss = tablik.Project.SchemeSources["Sour2"];
            Assert.AreEqual(ProviderType.Source, ss.Type);
            Assert.AreEqual("Sour2", ss.Code);
            Assert.AreEqual("Fictive", ss.Complect);

            Assert.AreEqual(2, tablik.Project.SchemeModules.Count);
            Assert.IsTrue(tablik.Project.SchemeModules.ContainsKey("Mod1"));
            var sm = tablik.Project.SchemeModules["Mod1"];
            Assert.AreEqual("Mod1", sm.Code);
            Assert.AreEqual(0, sm.LinkedModules.Count);
            Assert.AreEqual(2, sm.LinkedConnects.Count);
            Assert.IsTrue(sm.LinkedConnects.Contains("Sour1"));
            Assert.IsTrue(sm.LinkedConnects.Contains("Sour2"));

            Assert.IsTrue(tablik.Project.SchemeModules.ContainsKey("Mod2"));
            sm = tablik.Project.SchemeModules["Mod2"];
            Assert.AreEqual("Mod2", sm.Code);
            Assert.AreEqual(1, sm.LinkedModules.Count);
            Assert.IsTrue(sm.LinkedModules.Contains("Mod1"));
            Assert.AreEqual(1, sm.LinkedConnects.Count);
            Assert.IsTrue(sm.LinkedConnects.Contains("Sour1"));

            Assert.AreEqual(2, tablik.Modules.Count);
            Assert.IsTrue(tablik.Modules.ContainsKey("Mod1"));
            Assert.IsTrue(tablik.Modules.ContainsKey("Mod2"));
            Assert.AreEqual(2, tablik.Sources.Count);
            Assert.IsTrue(tablik.Sources.ContainsKey("Sour1"));
            Assert.IsTrue(tablik.Sources.ContainsKey("Sour2"));

            var source = tablik.Sources["Sour1"];
            Assert.AreEqual("Sour1", source.Code);
            Assert.AreEqual(2, source.ObjectsTypesId.Count);
            Assert.AreEqual(4, source.ObjectsTypes.Count);
            Assert.AreEqual(3, source.ObjectsCalcTypesId.Count);
            Assert.AreEqual(6, source.ObjectsCalcTypes.Count);
            Assert.AreEqual(4, source.Objects.Count);

            Assert.IsTrue(source.ObjectsTypes.ContainsKey("Large"));
            Assert.IsTrue(source.ObjectsTypes.ContainsKey("Sour1.Large"));
            var ot = source.ObjectsTypes["Large"];
            Assert.AreEqual("Large", ot.Code);
            Assert.AreEqual("Большой объект", ot.Name);
            Assert.AreEqual(3, ot.BaseTypes.Count);
            Assert.IsTrue(ot.BaseTypes.Contains(source.ObjectsCalcTypes["Analog"]));
            Assert.IsTrue(ot.BaseTypes.Contains(source.ObjectsCalcTypes["State"]));
            Assert.IsTrue(ot.BaseTypes.Contains(source.ObjectsCalcTypes["Discret"]));
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

            Assert.IsTrue(source.ObjectsTypes.ContainsKey("Small"));
            Assert.IsTrue(source.ObjectsTypes.ContainsKey("Sour1.Small"));
            ot = source.ObjectsTypes["Small"];
            Assert.AreEqual("Small", ot.Code);
            Assert.AreEqual("Маленький объект", ot.Name);
            Assert.AreEqual(2, ot.BaseTypes.Count);
            Assert.IsTrue(ot.BaseTypes.Contains(source.ObjectsCalcTypes["Analog"]));
            Assert.IsTrue(ot.BaseTypes.Contains(source.ObjectsCalcTypes["State"]));
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

            Assert.IsTrue(source.ObjectsCalcTypes.ContainsKey("Analog"));
            Assert.IsTrue(source.ObjectsCalcTypes.ContainsKey("Sour1.Analog"));
            ot = source.ObjectsCalcTypes["Analog"];
            Assert.AreEqual("Analog", ot.Code);
            Assert.AreEqual("Аналоговый сигнал", ot.Name);
            Assert.AreEqual(0, ot.BaseTypes.Count);
            Assert.AreEqual(2, ot.Signals.Count);

            Assert.IsTrue(ot.Signals.ContainsKey("State"));
            var tsig = ot.Signals["State"];
            Assert.AreEqual("State", tsig.Code);
            //Assert.AreEqual(tsig, tsig.Signal);
            Assert.AreEqual("Состояние", tsig.Name);
            Assert.AreEqual(DataType.Integer, tsig.DataType);
            Assert.AreEqual(DataType.Integer, tsig.Simple.DataType);
            Assert.AreEqual(ArrayType.Single, tsig.Simple.ArrayType);
            Assert.AreEqual(null, tsig.Simple.TablikSignalType);

            Assert.IsTrue(ot.Signals.ContainsKey("Value"));
            tsig = ot.Signals["Value"];
            Assert.AreEqual("Value", tsig.Code);
            //Assert.AreEqual(tsig, tsig.Signal);
            Assert.AreEqual("Значение", tsig.Name);
            Assert.AreEqual(DataType.Real, tsig.DataType);
            Assert.AreEqual(DataType.Real, tsig.Simple.DataType);
            Assert.AreEqual(ArrayType.Single, tsig.Simple.ArrayType);
            Assert.AreEqual(null, tsig.Simple.TablikSignalType);

            Assert.IsTrue(source.ObjectsCalcTypes.ContainsKey("State"));
            Assert.IsTrue(source.ObjectsCalcTypes.ContainsKey("Sour1.State"));
            ot = source.ObjectsCalcTypes["State"];
            Assert.AreEqual("State", ot.Code);
            Assert.AreEqual("Сигнал с соcтоянием", ot.Name);
            Assert.AreEqual(0, ot.BaseTypes.Count);
            Assert.AreEqual(1, ot.Signals.Count);

            Assert.IsTrue(ot.Signals.ContainsKey("State"));
            tsig = ot.Signals["State"];
            Assert.AreEqual("State", tsig.Code);
            //Assert.AreEqual(tsig, tsig.Signal);
            Assert.AreEqual("Состояние", tsig.Name);
            Assert.AreEqual(DataType.Integer, tsig.DataType);
            Assert.AreEqual(DataType.Integer, tsig.Simple.DataType);
            Assert.AreEqual(ArrayType.Single, tsig.Simple.ArrayType);
            Assert.AreEqual(null, tsig.Simple.TablikSignalType);

            Assert.IsTrue(source.ObjectsCalcTypes.ContainsKey("Discret"));
            Assert.IsTrue(source.ObjectsCalcTypes.ContainsKey("Sour1.Discret"));
            ot = source.ObjectsCalcTypes["Discret"];
            Assert.AreEqual("Discret", ot.Code);
            Assert.AreEqual("Дискретный сигнал", ot.Name);
            Assert.AreEqual(0, ot.BaseTypes.Count);
            Assert.AreEqual(2, ot.Signals.Count);

            Assert.IsTrue(ot.Signals.ContainsKey("Есть"));
            tsig = ot.Signals["Есть"];
            Assert.AreEqual("Есть", tsig.Code);
            //Assert.AreEqual(tsig, tsig.Signal);
            Assert.AreEqual("Значение", tsig.Name);
            Assert.AreEqual(DataType.Boolean, tsig.DataType);
            Assert.AreEqual(DataType.Boolean, tsig.Simple.DataType);
            Assert.AreEqual(ArrayType.Single, tsig.Simple.ArrayType);
            Assert.AreEqual(null, tsig.Simple.TablikSignalType);

            Assert.IsTrue(source.Objects.ContainsKey("Out1"));
            var ob = source.Objects["Out1"];
            Assert.AreEqual("Out1", ob.Code);
            Assert.AreEqual("Значения разного типа", ob.Name);
            Assert.AreEqual(DataType.Real, ob.DataType);
            Assert.AreEqual(source, ob.Connect);
            Assert.AreEqual(source.ObjectsTypes["Large"], ob.ObjectType);
            Assert.AreEqual(source.ObjectsTypes["Large"].Signals["Real"], ob.Signal);
            Assert.AreEqual(14, ob.Props.Count);
            Assert.AreEqual("Out1", ob.Props["CodeObject"].Mean.String);
            Assert.AreEqual(DataType.String, ob.Props["CodeObject"].DataType);
            Assert.AreEqual("Значения разного типа", ob.Props["NameObject"].Mean.String);
            Assert.AreEqual(DataType.String, ob.Props["NameObject"].DataType);
            Assert.AreEqual("действ", ob.Props["DataType"].Mean.String);
            Assert.AreEqual(DataType.String, ob.Props["DataType"].DataType);
            Assert.AreEqual(0, ob.Props["Min"].Mean.Real);
            Assert.AreEqual(DataType.Real, ob.Props["Min"].DataType);
            Assert.AreEqual(100, ob.Props["Max"].Mean.Real);
            Assert.AreEqual(DataType.Real, ob.Props["Max"].DataType);
            Assert.AreEqual(11, ob.Props["Num"].Mean.Integer);
            Assert.AreEqual(DataType.Integer, ob.Props["Num"].DataType);
            Assert.AreEqual("XXX", ob.Props["Charact"].Mean.String);
            Assert.AreEqual(DataType.String, ob.Props["Charact"].DataType);

            Assert.IsTrue(source.Objects.ContainsKey("Out2"));
            ob = source.Objects["Out2"];
            Assert.AreEqual("Out2", ob.Code);
            Assert.AreEqual("Значение и состояние", ob.Name);
            Assert.AreEqual(DataType.Real, ob.DataType);
            Assert.AreEqual(source, ob.Connect);
            Assert.AreEqual(source.ObjectsTypes["Small"], ob.ObjectType);
            Assert.AreEqual(source.ObjectsTypes["Small"].Signals["Value"], ob.Signal);
            Assert.AreEqual(14, ob.Props.Count);
            Assert.AreEqual("Out2", ob.Props["CodeObject"].Mean.String);
            Assert.AreEqual(DataType.String, ob.Props["CodeObject"].DataType);
            Assert.AreEqual("Значение и состояние", ob.Props["NameObject"].Mean.String);
            Assert.AreEqual(DataType.String, ob.Props["NameObject"].DataType);
            Assert.AreEqual("Analog;State;", ob.Props["Types"].Mean.String);
            Assert.AreEqual(DataType.String, ob.Props["Types"].DataType);
            Assert.AreEqual(-1000, ob.Props["Min"].Mean.Real);
            Assert.AreEqual(DataType.Real, ob.Props["Min"].DataType);
            Assert.AreEqual(1000, ob.Props["Max"].Mean.Real);
            Assert.AreEqual(DataType.Real, ob.Props["Max"].DataType);
            Assert.AreEqual(22, ob.Props["Num"].Mean.Integer);
            Assert.AreEqual(DataType.Integer, ob.Props["Num"].DataType);
            Assert.AreEqual("ZZZ", ob.Props["Charact"].Mean.String);
            Assert.AreEqual(DataType.String, ob.Props["Charact"].DataType);

            var module = tablik.Modules["Mod1"];
            module.LoadModule();
            Assert.AreEqual("Mod1", module.Code);
            Assert.AreEqual("Главный модуль", module.Name);
            Assert.AreEqual(tablik.Project, module.Project);
            Assert.AreEqual(TestLib.TestRunDir + @"Tablik\TablikProjectLoad\Modules\Mod1\", module.Dir);
            Assert.AreEqual(0, module.LinkedModules.Count);
            Assert.AreEqual(2, module.LinkedSources.Count);
            Assert.IsTrue(module.LinkedSources.Contains(source));

            Assert.AreEqual(2, module.Grafics.Count);
            Assert.IsTrue(module.Grafics.ContainsKey("Gr3D"));
            var gr = module.Grafics["Gr3D"];
            Assert.AreEqual("Gr3D", gr.Code);
            Assert.AreEqual(3, gr.Dim);
            Assert.AreEqual(GraficType.Grafic, gr.GraficType);
            Assert.IsTrue(module.Grafics.ContainsKey("Gr2D"));
            gr = module.Grafics["Gr2D"];
            Assert.AreEqual("Gr2D", gr.Code);
            Assert.AreEqual(2, gr.Dim);
            Assert.AreEqual(GraficType.Grafic, gr.GraficType);

            Assert.IsNotNull(module.Tabls);
            Assert.AreEqual(3, module.Tabls.Structs.Count);
            Assert.IsTrue(module.Tabls.Structs.ContainsKey("Tab1"));
            var tabl = module.Tabls.Structs["Tab1"];
            Assert.AreEqual("Tab1" , tabl.Code);
            Assert.AreEqual(3, tabl.Tabls.Count);
            Assert.AreEqual(0, tabl.Tabls[0].Level);
            Assert.AreEqual(6, tabl.Tabls[0].Fields.Count);
            Assert.IsTrue(tabl.Tabls[0].Fields.ContainsKey("Num")); 
            Assert.IsTrue(tabl.Tabls[0].Fields.ContainsKey("FieldReal"));
            Assert.AreEqual(1, tabl.Tabls[1].Level);
            Assert.AreEqual(7, tabl.Tabls[1].Fields.Count);
            Assert.IsTrue(tabl.Tabls[1].Fields.ContainsKey("Code"));
            Assert.IsTrue(tabl.Tabls[1].Fields.ContainsKey("SubInt"));

            tabl = module.Tabls.Structs["Tab3"];
            Assert.AreEqual("Tab3", tabl.Code);
            Assert.AreEqual(4, tabl.Tabls.Count);
            Assert.AreEqual(0, tabl.Tabls[0].Level);
            Assert.AreEqual(6, tabl.Tabls[0].Fields.Count);
            Assert.IsTrue(tabl.Tabls[0].Fields.ContainsKey("Num"));
            Assert.IsTrue(tabl.Tabls[0].Fields.ContainsKey("Int0"));
            Assert.AreEqual(1, tabl.Tabls[1].Level);
            Assert.AreEqual(7, tabl.Tabls[1].Fields.Count);
            Assert.IsTrue(tabl.Tabls[1].Fields.ContainsKey("Code"));
            Assert.IsTrue(tabl.Tabls[1].Fields.ContainsKey("Real1"));
            Assert.AreEqual(2, tabl.Tabls[2].Level);
            Assert.AreEqual(7, tabl.Tabls[2].Fields.Count);
            Assert.IsTrue(tabl.Tabls[2].Fields.ContainsKey("Id"));
            Assert.IsTrue(tabl.Tabls[2].Fields.ContainsKey("String2"));

            Assert.AreEqual(2, module.Params.Count);
            Assert.AreEqual(0, module.DerivedParams.Count);
            Assert.IsTrue(module.Params.ContainsKey("A01"));
            Assert.IsTrue(module.ParamsAll.ContainsKey("A01"));
            var par = module.Params["A01"];
            Assert.AreEqual("A01", par.Code);
            Assert.AreEqual("Параметр", par.Name);
            Assert.AreEqual("A01", par.FullCode);
            Assert.IsTrue(par.CalcOn);
            Assert.IsFalse(par.IsFun);
            Assert.IsNull(par.InputsStr);
            Assert.AreEqual(0, par.Inputs.Count);
            Assert.AreEqual(module, par.Module);
            Assert.IsNull(par.Owner);
            Assert.AreEqual("1", par.UserExpr1);
            Assert.AreEqual("Расчет", par.UserExpr2);
            Assert.AreEqual("-100", par.Min);
            Assert.AreEqual("100", par.Max);
            Assert.AreEqual("кг", par.Units);
            Assert.AreEqual(1, par.Params.Count);
            Assert.AreEqual(1, par.ParamsAll.Count);
            Assert.IsFalse(par.IsSubParam);

            Assert.IsTrue(par.Params.ContainsKey("Sub01"));
            Assert.IsTrue(par.ParamsAll.ContainsKey("Sub01"));
            var spar = par.Params["Sub01"];
            Assert.AreEqual(par, spar.Owner);
            Assert.AreEqual(module, spar.Module);
            Assert.AreEqual("Sub01", spar.Code);
            Assert.AreEqual("A01.Sub01", spar.FullCode);
            Assert.AreEqual("Подпараметр", spar.Name);
            Assert.IsTrue(spar.CalcOn);
            Assert.IsFalse(spar.IsFun);
            Assert.IsNull(spar.InputsStr);
            Assert.AreEqual(0, spar.Inputs.Count);
            Assert.AreEqual("2", spar.UserExpr1);
            Assert.AreEqual("Расчет", spar.UserExpr2);
            Assert.AreEqual("-0,5", spar.Min);
            Assert.AreEqual("0,5", spar.Max);
            Assert.AreEqual("кг", spar.Units);
            Assert.AreEqual(0, spar.Params.Count);
            Assert.AreEqual(0, spar.ParamsAll.Count);
            Assert.IsTrue(spar.IsSubParam);
        }

        private void ParseExpr(DicS<TablikParam> pars, string code, string result)
        {
            Assert.IsTrue(pars.ContainsKey(code));
            Assert.AreEqual("NodeList: (" + result + ")", pars[code].Expr1.ToTestString());
        }

        private void ParseSubExpr(DicS<TablikParam> pars, string code, string subCode, string result)
        {
            Assert.IsTrue(pars.ContainsKey(code));
            Assert.IsTrue(pars[code].Params.ContainsKey(subCode));
            Assert.AreEqual("NodeList: (" + result + ")", pars[code].Params[subCode].Expr1.ToTestString());
        }

        [TestMethod]
        public void FormulaParse()
        {
            var tablik = LoadProject("TablikProject", "FormulaParse", "Mod1", "Mod2");
            Assert.IsNotNull(tablik.Modules);
            Assert.IsTrue(tablik.Modules.ContainsKey("Mod1"));
            var module = tablik.Modules["Mod1"];
            module.LoadModule();
            module.Parse();
            var pars = module.Params;

            ParseExpr(pars, "A01", "Boolean: 1");
            ParseExpr(pars, "A02", "Fun: + (Integer: 10, Real: 2.5)");
            ParseExpr(pars, "A03", "Fun: + (Fun: - (Boolean: 1), Fun: * (Integer: 3, Integer: 5))");
            ParseExpr(pars, "A04", "Fun: Or (Fun: True, Fun: Not (Fun: false))");
            ParseExpr(pars, "A05", "Fun: и (Fun: и (Fun: и (Fun: и (Fun: и (Fun: < (Real: 2.1, Real: 3.3), Fun: >= (Integer: 4, Fun: - (Integer: 2))), Fun: == (Integer: 8, Integer: 8)), Fun: <> (Boolean: 1, Boolean: 0)), Fun: <= (Boolean: 1, Boolean: 1)), Fun: - (Real: 2,7, Real: 6.8))");
            ParseExpr(pars, "A06", "String: 'sss/*aa*/'");
            ParseExpr(pars, "A07", "Fun: And (Fun: > (Fun: + (Integer: 10, Fun: / (Integer: 8, Integer: 4)), Integer: 11), Fun: ИсклИли (Fun: Xor (Boolean: 1, Fun: < (Integer: 7, Integer: 3)), Boolean: 0))");
            ParseExpr(pars, "A08", "Fun: - (Integer: 2)");
            ParseExpr(pars, "A09", "Fun: + (Fun: + (Boolean: 1, Boolean: 1), Boolean: 1)");
            ParseExpr(pars, "A10", "Fun: Like (String: 'aabb', String: '*ab?')");
            ParseExpr(pars, "A11", "Fun: не (Fun: Not (Fun: НЕ (Fun: not (Boolean: 0))))");
            ParseExpr(pars, "A12", "Fun: Sr (Fun: sl (Fun: mod (Fun: div (Integer: 55, Integer: 22), Integer: 3), Integer: 2), Integer: 3)");
            ParseExpr(pars, "A13", "Fun: Или (Fun: Или (Fun: Или (Fun: Бит (Integer: 34, Boolean: 1), Fun: БитИли (Integer: 22, Boolean: 1, Integer: 4)), Fun: BitAnd (Integer: 13, Integer: 2, Integer: 3)), Fun: Ложь)");
            ParseExpr(pars, "A14", "Fun: + (Fun: ^ (Fun: + (Integer: 2, Boolean: 1), Integer: 3), Fun: * (Integer: 3, Fun: + (Integer: 4, Fun: - (Integer: 5, Integer: 2))))");
            ParseExpr(pars, "A15", "Fun: + (Fun: cos (Fun: Pi), Fun: sin (Fun: * (Integer: 2, Fun: Pi)))");
            ParseExpr(pars, "A16", "Fun: * (Fun: Min (Boolean: 1, Integer: 2, Integer: 3), Fun: Max (Integer: 4, Integer: 3, Integer: 2, Boolean: 1))");
            ParseExpr(pars, "A17", "Fun: Log (Fun: Ln (Integer: 3), Fun: Log10 (Integer: 4))");
            ParseExpr(pars, "A18", "Fun: * (Integer: 2, Fun: - (Integer: 3))");
            ParseExpr(pars, "A19", "Fun: ЧастьВремени (Fun: ВрЧас, Time: #12.11.2010 12:13:14#)");

            ParseExpr(pars, "B01", "Void: Пустой");
            ParseExpr(pars, "B02", "Assign: (Var: x, Integer: 10), Fun: + (Var: x, Integer: 20)");
            ParseExpr(pars, "B03", "Assign: (Var: aa, String: 'ss'), Assign: (Var: bb, Fun: <> (String: 'tt', Var: aa)), Assign: (Var: cc, Fun: or (Var: bb, Fun: True))");
            ParseExpr(pars, "B04", "Assign: (Type: Int, Var: x, Boolean: 1), Assign: (Type: Real, Var: y, Integer: 2), Fun: * (Var: x, Var: y)");
            ParseExpr(pars, "B05", "Assign: (Type: List(Real), Var: s, Fun: CreateList (Integer: 2, Integer: 3, Integer: 4)), Var: s");
            ParseExpr(pars, "B06", "Assign: (Var: a, Boolean: 1), Assign: (Var: a, Fun: + (Var: a, Integer: 2)), Assign: (Type: DicNumbers(Int), Var: d, Fun: СоздатьСловарьЧисла (Var: a, Fun: * (Var: a, Integer: 2))), Var: d");
            ParseExpr(pars, "B07", "If: Если (Boolean: 1, NodeList: (Integer: 2), NodeList: (Integer: 3))");
            ParseExpr(pars, "B08", "Assign: (Var: a, Real: 2.4), If: Если (Fun: < (Var: a, Integer: 2), NodeList: (Fun: * (Var: a, Integer: 2)), Fun: And (Fun: >= (Var: a, Integer: 2), Fun: < (Var: a, Integer: 4)), NodeList: (Var: a), NodeList: (Fun: / (Var: a, Integer: 2)))");
            ParseExpr(pars, "B09", "Assign: (Type: Int, Var: b, Boolean: 1), If: If (Fun: == (Var: b, Boolean: 0), NodeList: (Assign: (Var: a, Integer: 3), Assign: (Var: c, Integer: 2)), NodeList: (Assign: (Var: a, Integer: 2), Assign: (Var: c, Boolean: 1))), Fun: + (Var: a, Var: c)");
            ParseExpr(pars, "B10", "Assign: (Var: i, Boolean: 0), Assign: (Var: s, Boolean: 0), While: while (Fun: < (Var: i, Integer: 5), NodeList: (Assign: (Var: s, Fun: + (Var: s, Var: i)))), Var: s");
            ParseExpr(pars, "B11", "If: Если (Boolean: 1, NodeList: (Assign: (Var: n, Integer: 3), While: Пока (Fun: > (Var: n, Boolean: 0), NodeList: (Assign: (Var: n, Fun: - (Var: n, Integer: 2)))), Var: n), NodeList: (If: Если (Boolean: 0, NodeList: (Integer: 2), NodeList: (Integer: 3))))");
            ParseExpr(pars, "B12", "Fun: Sin (Fun: - (Grafic: Gr2D (Fun: Abs (Fun: * (Integer: 10, Fun: - (Integer: 2)))), Integer: 5))");
            ParseExpr(pars, "B13", "Grafic: Gr3D (Real: 1.3, Real: 2.34)");
            ParseExpr(pars, "B14", "Tabl: Табл(Tab1.SubReal, Fun: + (Boolean: 1, Boolean: 1), Fun: Abs (Fun: - (Integer: 2)))");
            ParseExpr(pars, "B15", "Fun: + (Tabl: TablList(Tab3.Int1, String: 'code'), String: 'aa')");
            ParseExpr(pars, "B16", "Tabl: TablContains(Tab2, String: 'ss')");

            ParseExpr(pars, "C01", "Boolean: 1");
            ParseSubExpr(pars, "C01", "S1", "Integer: 2");
            ParseSubExpr(pars, "C01", "S2", "Fun: Log10 (SubParam: S1)");
            ParseExpr(pars, "C02", "Fun: + (Param: C01, Fun: * (Integer: 2, Param: C01))");
            ParseExpr(pars, "C03", "Assign: (Var: b, Param: C01), Assign: (Var: a, Param: C02), Fun: + (Fun: + (Fun: + (Var: a, Var: b), Param: C01), Param: C02)");
            ParseExpr(pars, "C04", "Fun: * (Var: x, Var: y)");
            ParseSubExpr(pars, "C04", "S1", "Fun: + (Var: x, Var: y)");
            ParseSubExpr(pars, "C04", "S2", "Fun: + (Fun: + (Var: x, Var: y), Var: z)");
            ParseSubExpr(pars, "C04", "S3", "Fun: + (SubParam: S2 (Boolean: 1), SubParam: S1)");
            ParseSubExpr(pars, "C04", "S4", "Fun: * (Owner: Owner, Integer: 3)");
            ParseSubExpr(pars, "C04", "S5", "Fun: + (Param: C02, Met: S2 (Param: C01))");
            ParseExpr(pars, "C05", "Assign: (Var: a, Boolean: 1), Param: C04 (Fun: + (Integer: 3, Var: a), Fun: - (Integer: 4, Var: a))");
            ParseExpr(pars, "C06", "Param: C04 (Param: C01, Param: C02)");
            ParseExpr(pars, "C07", "Prev: Absolute (Param: C01, Boolean: 0)");
            ParseExpr(pars, "C08", "Fun: + (Met: S1 (Param: C01), Met: S1 (Param: C04 (Integer: 4, Integer: 5)))");
            ParseExpr(pars, "C09", "Met: S2 (Param: C04 (Real: 1,4, Fun: - (Real: 3,7)), Real: 2,9)");
            ParseExpr(pars, "C10", "SubParams: Подпараметры (Param: C04 (Boolean: 1, Boolean: 1)), Boolean: 1");
            ParseExpr(pars, "C11", "Fun: + (Met: S2 (Param: C10, Met: S3 (Param: C10)), Met: S4 (Param: C10))");
            ParseExpr(pars, "C12", "If: Если (Fun: > (Param: C06, Integer: 3), NodeList: (Param: C08), Param: C04 (Param: C09), NodeList: (If: If (Fun: > (Param: C03, Integer: 2), NodeList: (Integer: 4), NodeList: (Param: C11))))");
            ParseExpr(pars, "C13", "Assign: (Type: C04, Var: a, Param: C04 (Integer: 2, Integer: 3)), Var: a");
            ParseExpr(pars, "C14", "Fun: Элемент (Var: arr, Boolean: 1)");
            ParseExpr(pars, "C15", "Param: C14 (Fun: СоздатьСписок (Integer: 5, Integer: 6, Integer: 7, Integer: 8))");
            ParseExpr(pars, "C16", "Fun: + (Met: S1 (Var: p), Met: S2 (Var: p, Integer: 3))");
            ParseExpr(pars, "C17", "Param: C16 (Param: C04 (Boolean: 1, Integer: 2))");

            ParseExpr(pars, "D01", "Fun: + (Fun: + (Fun: + (Signal: {Out1}, Signal: {Out1.Int}), Signal: {Sour1.Out2}), Signal: {Sour1.Out2.State})");
            ParseExpr(pars, "D02", "Assign: (Var: a, Signal: {Out1}), Assign: (Var: b, MetSignal: {String} (Var: a)), Fun: + (Var: b, String: 'aaa')");
            ParseExpr(pars, "D03", "MetSignal: {Int} (Var: S)");
            ParseExpr(pars, "D04", "Fun: Or (MetSignal: {State} (Var: A), MetSignal: {State} (Var: S))");
            ParseExpr(pars, "D05", "Fun: + (Param: D03 (Signal: {Out1}), Param: D04 (Signal: {Out1}, Signal: {Out2}))");
            
        }
    }
}