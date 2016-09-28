using System;
using BaseLibrary;
using Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneratorTest
{
    [TestClass]
    public class RuleParseTest
    {
        private GenKeeper MakeKeeper()
        {
            return new GenKeeper(new TablGenerator(new Logger(), null, null, null, null, null, null));
        }

        //Разбор выражения GenRule таблицы и подтаблицы
        private string Parse(GenKeeper keeper, string formula)
        {
            keeper.Errors.Clear();
            return new RuleParsing(keeper, "поле", formula).ToTestString();
        }
        private string ParseSub(GenKeeper keeper, string formula)
        {
            keeper.Errors.Clear();
            return new SubRuleParsing(keeper, "поле", formula).ToTestString();
        }

        [TestMethod]
        public void Tabl()
        {
            var k = MakeKeeper();
            Assert.AreEqual("Tabl: aaa (, )", Parse(k, "aaa"));
            
            Assert.AreEqual("OverTabl: aaa", Parse(k, "aaa.OverTabl"));
            Assert.AreEqual("OverTabl: aaa", Parse(k, "aaa.НадТабл"));

            Assert.AreEqual("Tabl: aaa (, )", Parse(k, "aaa"));
            Assert.AreEqual("Tabl: aaa (Field: x, )", Parse(k, "aaa(x)"));

            Assert.AreEqual("Tabl: aaa (Field: x, )", Parse(k, "aaa(x)"));
            Assert.AreEqual("Tabl: Aaa (Field: Type, )", Parse(k, "Aaa ( Type )"));

            Assert.AreEqual("Tabl: aaa (, SubTabl: SubTabl (, ))", Parse(k, "aaa.SubTabl"));
            Assert.AreEqual("Tabl: aaa (, SubTabl: ПодТабл (, ))", Parse(k, "aaa. ПодТабл "));
            Assert.AreEqual("Tabl: aaa (, SubTabl: ПодТабл (, SubTabl: ПодТабл (, )))", Parse(k, "aaa.ПодТабл" + Environment.NewLine + ".ПодТабл"));

            Assert.AreEqual("Tabl: Bbb (Field: Type, SubTabl: SubTabl (, ))", Parse(k, "Bbb(Type).SubTabl"));
            Assert.AreEqual("Tabl: Aaa (Field: Type, SubTabl: SubTabl (Field: T, ))", Parse(k, "Aaa (Type) .SubTabl (T)"));
            Assert.AreEqual("Tabl: Tab (Field: type, SubTabl: subtabl (Field: typ, SubTabl: subtabl (, )))", Parse(k, "Tab(type).subtabl(typ).subtabl"));

            Assert.AreEqual("Tabl: Ddd (, )", Parse(k, @"/*sfjgs*/Ddd"));
            Assert.AreEqual("Tabl: Ddd (, )", Parse(k, @"//sfjgs" + Environment.NewLine + "Ddd"));
            Assert.AreEqual("Tabl: Ddd (Field: x, SubTabl: ПодТабл (Field: y, ))",
                 Parse(k, @"Ddd(/*ooo*/x/*ooo*/)" + Environment.NewLine + @"//sfjgs" + Environment.NewLine + @".ПодТабл(/*ooo*/y/*ooo*/)" + Environment.NewLine + @"/*dd*///ss"));
            Assert.AreEqual("Tabl: df (, )", Parse(k, @"/*df*///" + Environment.NewLine + "df" + Environment.NewLine + "//dd/**///"));
            Assert.AreEqual("Tabl: aa (, SubTabl: SubTabl (, ))", Parse(k, @"aa.SubTabl/*df//zdf*/"));

            Assert.AreEqual("SubTabl: SubTabl (, )", ParseSub(k, "SubTabl"));
            Assert.AreEqual("SubTabl: ПодТабл (, )", ParseSub(k, "ПодТабл " + Environment.NewLine));
            Assert.AreEqual("SubTabl: ПодТабл (Field: x, )", ParseSub(k, "ПодТабл(x)"));
            Assert.AreEqual("SubTabl: ПодТабл (, SubTabl: ПодТабл (Field: x, ))", ParseSub(k, "ПодТабл. ПодТабл (x)"));
            Assert.AreEqual("SubTabl: SubTabl (Field: x, )", ParseSub(k, @"//ddd" + Environment.NewLine + @"SubTabl(/*x*/x)" + Environment.NewLine + @"/*f/*ff*/"));
        }

        [TestMethod]
        public void Exprs()
        {
            var k = MakeKeeper();
            Assert.AreEqual("Tabl: Tabl (Fun: == (Field: Id, Integer: 3), )", Parse(k, "Tabl(Id==3)"));
            Assert.AreEqual("Tabl: Tabl (Fun: > (Field: Id, Boolean: 1), )", Parse(k, "Tabl(Id>1)"));
            Assert.AreEqual("Tabl: Tabl (Fun: <> (Field: Id, Fun: + (Field: a, Field: b)), )", Parse(k, "Tabl(Id<>a+b)"));
            Assert.AreEqual("Tabl: Tabl (Fun: and (Fun: > (Field: a, Integer: 2), Fun: < (Field: b, Real: 3.2)), )", Parse(k, "Tabl(a>2 and b < 3.2)"));
            Assert.AreEqual("Tabl: Tabl (Fun: не (Fun: == (Field: s, String: 'fff')), )", Parse(k, "Tabl(не s=='fff')"));
            Assert.AreEqual("Tabl: Tabl (Fun: не (Fun: <= (Field: s, Fun: * (Fun: - (Integer: 3), Fun: - (Integer: 2)))), )", Parse(k, "Tabl(не s<= -3 * -2)"));
            Assert.AreEqual("Tabl: Tabl (Fun: или (Fun: Не (Fun: TRUE), Fun: Не (Fun: FALSE)), )", Parse(k, "Tabl(Не TRUE или Не FALSE)"));
            Assert.AreEqual("Tabl: Tabl (Fun: xor (Fun: or (Fun: > (Field: a, Integer: 2), Fun: < (Field: b, Fun: Pi)), Fun: and (Fun: true, Fun: == (Field: c, String: 'sss'))), )", Parse(k, "Tabl((a > 2 or b < Pi) xor (true and c=='sss'))"));
            Assert.AreEqual("Tabl: Tabl (Fun: - (Integer: 2, Boolean: 1), )", Parse(k, "Tabl(2-1)"));
            Assert.AreEqual("Tabl: Tabl (Fun: - (Fun: + (Boolean: 1, Integer: 2), Boolean: 1), )", Parse(k, "Tabl(1+2-1)"));
            Assert.AreEqual("Tabl: Tabl (Fun: Div (Fun: * (Fun: Mod (Fun: - (Integer: 4), Integer: 3), Fun: ^ (Field: z, Integer: 2)), Integer: 5), )", Parse(k, "Tabl((-4 Mod 3)*z^2 Div 5)"));
            Assert.AreEqual("Tabl: Tabl (Fun: + (Fun: - (Field: x, Field: y), Fun: / (Fun: * (Integer: 4, Fun: ^ (Field: z, Integer: 2)), Integer: 5)), )", Parse(k, "Tabl(x-y+4*z^2 / 5)"));
            Assert.AreEqual("Tabl: Tabl (Fun: cos (Fun: * (Field: x, Field: y)), )", Parse(k, "Tabl(cos(((x)*((y)))))"));
            Assert.AreEqual("Tabl: Tabl (Fun: cos (Fun: * (Field: x, Field: y)), )", Parse(k, "Tabl(cos(((x)*((y)))))"));
            Assert.AreEqual("Tabl: A (Fun: like (Field: st, String: 's*'), )", Parse(k, "A(st like 's*')"));
            Assert.AreEqual("Tabl: Tabl (Field: x, SubTabl: Subtabl (Fun: < (Fun: + (Field: y, Fun: * (Boolean: 1, Fun: * (Field: x, Field: z))), Field: w), ))", Parse(k, "Tabl(x).Subtabl(y+1*(x*z)<w)"));
            Assert.AreEqual("Tabl: Tabl (Fun: < (Field: x, Boolean: 1), SubTabl: Subtabl (Fun: == (Field: yy, Integer: 4), SubTabl: SubTabl (Fun: + (Field: cc, Field: cc), )))", Parse(k, "Tabl(x<1).Subtabl(yy==4).SubTabl(cc+cc)"));
            Assert.AreEqual("Tabl: Tabl (Fun: < (Fun: cos (Field: x), Boolean: 1), )", Parse(k, "Tabl(cos(x)<1)"));
            Assert.AreEqual("Tabl: Tabl (Fun: == (Fun: cos (Fun: + (Field: x, Fun: sin (Field: y))), Fun: * (Field: x, Fun: StrLen (String: 'ddd'))), )", Parse(k, "Tabl(cos(x+sin(y))==x*StrLen('ddd'))"));
            Assert.AreEqual("Tabl: Tabl (Fun: cos (Fun: * (Field: x, Fun: Pi)), )", Parse(k, "Tabl(cos(x*Pi(/*daf*/)))"));
            Assert.AreEqual("Tabl: Tabl (Fun: cos (Fun: * (Field: x, Fun: Pi)), SubTabl: SubTabl (Field: y, ))", Parse(k, "Tabl(cos(x*Pi(/*daf*/))).SubTabl(y)"));
            Assert.AreEqual("Tabl: TTT (Field: y, )", Parse(k, "TTT(y) /*SubTabl(Abs(x-3.12*Exp(3))==0)*/"));
            Assert.AreEqual("Tabl: TTT (Fun: строка (Time: #02.03.2004 5:06:07#), )", Parse(k, "TTT(строка(#02.03.2004 5:06:07#))"));
            Assert.AreEqual("SubTabl: ПодТабл (Fun: > (Field: x, Integer: 2), )", ParseSub(k, "ПодТабл(x>2)"));
            Assert.AreEqual("SubTabl: ПодТабл (Fun: == (Fun: Abs (Fun: - (Field: x, Fun: * (Real: 3.12, Fun: Exp (Integer: 3)))), Boolean: 0), )", ParseSub(k, "ПодТабл(Abs(x-3.12*Exp(3))==0)"));
        }

        [TestMethod]
        public void Errors()
        {
            var k = MakeKeeper();
            Assert.AreEqual("Tabl: aa (, ) Недопустимая последовательность символов, ''" + Environment.NewLine + "' (поле, строка: 1, позиция: 3)", Parse(k, "aa'"));
            Assert.AreEqual("Tabl: aa (, ) Незакрытая скобка, '(' (поле, строка: 1, позиция: 3)", Parse(k, "aa("));
            Assert.AreEqual("Tabl: aa (Fun: + (Field: x, Boolean: 1), ) Незакрытая скобка, '(' (поле, строка: 1, позиция: 3)", Parse(k, "aa(x+1"));
            Assert.AreEqual("Tabl: aa (Fun: == (Field: x, String: 'ddd'), ) Лишняя закрывающаяся скобка, ')' (поле, строка: 1, позиция: 13)", Parse(k, "aa(x=='ddd'))"));
            Assert.AreEqual("Tabl: aa (, ) Лишняя закрывающаяся скобка, ')' (поле, строка: 1, позиция: 5)", Parse(k, "aa())"));
            Assert.AreEqual("Tabl: aaaa (, ) Недопустимое использование лексемы, '(' (поле, строка: 1, позиция: 1)", Parse(k, "(aaaa"));
            Assert.AreEqual(" Недопустимое использование лексемы, ')' (поле, строка: 1, позиция: 12)", Parse(k, "Tabl(sd+dd+)"));
            Assert.AreEqual("Tabl: Tabl (Fun: + (Field: ss, Real: 1.2), ) Недопустимое использование лексемы, '3' (поле, строка: 1, позиция: 13)", Parse(k, "Tabl(ss+1.2.3)"));
            Assert.AreEqual("Tabl: Tabl (Fun: + (Field: ss, Field: a), ) Недопустимое использование лексемы, '3' (поле, строка: 1, позиция: 11)", Parse(k, "Tabl(ss+a.3)"));
            Assert.AreEqual("Tabl: Tabl (, SubTabl: SubTabl (, )) Незакрытая скобка, '(' (поле, строка: 1, позиция: 13)", Parse(k, "Tabl.SubTabl("));
            Assert.AreEqual("Tabl: Tabl (, SubTabl: SubTabl (Fun: * (Field: x, Fun: cos (Field: x)), )) Незакрытая скобка, '(' (поле, строка: 1, позиция: 13)", Parse(k, "Tabl.SubTabl(x*cos(x"));
            Assert.AreEqual("Tabl: Tabl (, SubTabl: SubTabl (Field: x, )) Лишняя закрывающаяся скобка, ')' (поле, строка: 1, позиция: 16)", Parse(k, "Tabl.SubTabl(x))"));
            Assert.AreEqual("Tabl: Tabl (, SubTabl: SubTabl (, )) Недопустимое использование лексемы, 'абв' (поле, строка: 1, позиция: 14)", Parse(k, "Tabl.SubTabl абв+где)"));
            Assert.AreEqual("Tabl: Tabl (, SubTabl: SubTabl (Boolean: 987654321987, )) Недопустимое целое число, '987654321987' (поле, строка: 1, позиция: 14)", Parse(k, "Tabl.SubTabl(987654321987)"));
            Assert.AreEqual("Tabl: ghs (, ) Недопустимое использование лексемы, '/' (поле, строка: 1, позиция: 11)", Parse(k, "ghs(/*sd*//dd)"));
            Assert.AreEqual("Tabl: ghs (, ) Незакрытая скобка, '(' (поле, строка: 1, позиция: 4)", Parse(k, "ghs(/*sd*///dd)"));
            Assert.AreEqual(" Недопустимое использование лексемы, 'SubTabl' (поле, строка: 1, позиция: 1)", Parse(k, "SubTabl(s)"));
            Assert.AreEqual("Tabl: aaa (Fun: + (Field: x, Integer: 2), ) Незакрытая скобка, '(' (поле, строка: 1, позиция: 5)", Parse(k, "aaa((x+2)"));
            Assert.AreEqual("SubTabl: SubTabl (Field: Ч, ) Незакрытая скобка, '(' (поле, строка: 1, позиция: 8)", ParseSub(k, "SubTabl(Ч"));
            Assert.AreEqual("SubTabl: SubTabl (Field: Ч, ) Недопустимое использование лексемы, 'Н' (поле, строка: 1, позиция: 11)", ParseSub(k, "SubTabl(Ч Н)"));
            Assert.AreEqual("SubTabl: SubTabl (Field: aa, )", ParseSub(k, "SubTabl(/*ddd*/aa)//fff"));
            Assert.AreEqual(" Недопустимое использование лексемы, 'Tabl' (поле, строка: 1, позиция: 1)", ParseSub(k, "Tabl(s)"));
            Assert.AreEqual("SubTabl: SubTabl (Fun: + (Field: x, Integer: 20), ) Лишняя закрывающаяся скобка, ')' (поле, строка: 1, позиция: 14)", ParseSub(k, "SubTabl(x+20))"));
        }
    }
}