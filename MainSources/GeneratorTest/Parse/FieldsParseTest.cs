using System;
using BaseLibrary;
using BaseLibraryTest;
using Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneratorTest
{
    [TestClass]
    public class FieldsParseTest
    {
        private GenKeeper MakeKeeper()
        {
            var logger = new Logger(new AppIndicator());
            logger.History = new TestHistory(logger);
            return new GenKeeper(new ModuleGenerator(logger, null, null, null));
        }

        //Разбор выражения GenRule таблицы и подтаблицы
        private string Parse(GenKeeper keeper, string formula)
        {
            keeper.Errors.Clear();
            return new FieldsParsing(keeper, "поле", formula).ToTestString();
        }

        [TestMethod]
        public void Modes()
        {
            var k = MakeKeeper();
            Assert.AreEqual("NodeList: (String: aaa)", Parse(k, "aaa"));
            Assert.AreEqual("NodeList: (String: xdf234>)7sfkl 2->kjawd" + Environment.NewLine + "zfj easfae aef--=23 wdJJCFv " + Environment.NewLine + "asdvsdgv)",
                Parse(k, "xdf234>)7sfkl 2->kjawd" + Environment.NewLine + "zfj easfae aef--=23 wdJJCFv " + Environment.NewLine + "asdvsdgv"));
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , Field: aa))", Parse(k, "[aa]"));
            Assert.AreEqual("NodeList: (String: bb , ValueProg: (VoidProg: , Field: aa), String:  cc)", Parse(k, "bb [aa] cc"));
            Assert.AreEqual("NodeList: (String: bb , ValueProg: (VoidProg: , Field: aa), String:  , ValueProg: (VoidProg: , Field: hh), String: /*sdd*/, ValueProg: (VoidProg: , Field: gg))", 
                Parse(k, "bb [ aa ] [ hh ]/*sdd*/[gg]"));
            Assert.AreEqual("NodeList: (String: sss, ValueProg: (VoidProg: , Fun: + (Fun: + (Field: a, NodeList: (String: ttt)), Field: a)))", Parse(k, "sss[a+[ttt]+a]"));
            Assert.AreEqual("NodeList: (String: sss, ValueProg: (VoidProg: , Fun: + (Field: a, NodeList: (String: ttt))))",
                Parse(k, "sss[" + Environment.NewLine + @"//yyy" + Environment.NewLine + @"a+[ttt]/*xxx*/]"));
            Assert.AreEqual("NodeList: (String: a, ValueProg: (VoidProg: , Fun: + (Field: b, NodeList: (String: c+, ValueProg: (VoidProg: , Fun: + (Field: d, NodeList: (String: e)))))))", 
                Parse(k, "a[b+[c+[d+[e]]]]"));
            Assert.AreEqual("NodeList: (String: //a, ValueProg: (VoidProg: , Fun: + (Field: b, NodeList: (String: c))), String: " + Environment.NewLine +
                "a, ValueProg: (VoidProg: , Field: b), String: " + Environment.NewLine + "a, ValueProg: (VoidProg: , Field: b))",
                Parse(k, "//a[b + [c]]" + Environment.NewLine + "a[b /*+[c+[d+[e]]]*/]" + Environment.NewLine + "a[ //dfff" + Environment.NewLine + "b]"));
            Assert.AreEqual("NodeList: (String: x, ValueProg: (VoidProg: , Boolean: 1), ValueProg: (VoidProg: , Integer: 2), String: y, ValueProg: (VoidProg: , Integer: 3), ValueProg: (VoidProg: , Integer: 4), String: z)", 
                Parse(k, "x[1][2]y[3][4]z"));
        }

        [TestMethod]
        public void Exprs()
        {
            var k = MakeKeeper();
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , Fun: == (Field: a, Fun: - (Fun: + (Fun: mod (Integer: 10, Fun: div (Fun: - (Integer: 4), Integer: 2)), Fun: * (Real: 2.1, Fun: ^ (Integer: 4, Integer: 2))), Fun: / (Real: 33.8, Fun: - (Integer: 15, Integer: 7))))))", 
                Parse(k, "[a == 10 mod (-4 div 2)+2.1*4^2-33.8/(15-7)]"));
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , Fun: Xor (Fun: Или (Fun: > (Field: Id, Integer: 3), Fun: И (Fun: == (Field: x, Fun: True), Fun: Not (Fun: Подобно (Field: z, String: '*ff*')))), Fun: False)))",
                Parse(k, "[Id >3 Или (x == True И Not z Подобно '*ff*') Xor False]"));
            Assert.AreEqual("NodeList: (String: ss, ValueProg: (VoidProg: (VarSet: a (Integer: 123), VarSet: b (Fun: + (Var: a, Integer: 35))), Var: b))", 
                Parse(k, "ss[a=123: b=a+35 : b]"));
            Assert.AreEqual("NodeList: (VoidProg: (VarSet: a (Real: 12.3)), String:  Text , ValueProg: (VoidProg: , Var: a))",
                Parse(k, "[a=12.3/*v1*/] Text [ a /*v2*/ ]"));
            Assert.AreEqual("NodeList: (String: d, ValueProg: (VoidProg: , Fun: <= (Fun: cos (Field: x), Fun: / (Fun: sin (Fun: * (Field: y, Fun: Pi)), Real: 2.4))))",
                Parse(k, "d[cos(x)<=sin(y*Pi)/2.4]"));
            Assert.AreEqual("NodeList: (String: Text: , ValueProg: (VoidProg: (VarSet: var (String: 's*')), Fun: Like (String: 'stytf', Fun: + (Var: var, NodeList: (String: hhh, ValueProg: (VoidProg: , Integer: 12))))), String:  :Text)",
                Parse(k, "Text: [var = 's*' : 'stytf' Like var + [hhh[12]]] :Text"));
            Assert.AreEqual("NodeList: (String: ---, VoidProg: (VarSet: переменная_ (Fun: StrInsert (String: 'ddaa', Fun: + (Integer: 2, Integer: 2), Fun: + (String: 'ee', String: 'kk')))), String: ---, ValueProg: (VoidProg: , Var: переменная_))",
                Parse(k, "---[переменная_=StrInsert('ddaa';2+2;'ee'+'kk')]---[переменная_]"));
            Assert.AreEqual("NodeList: (String: ss, ValueProg: (VoidProg: , Fun: + (Fun: + (Field: w, Field: x), Field: y)))",
                Parse(k, "ss[w+x/*+[sss]*/+y]"));
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , Fun: StrUCase (Fun: StrLeft (Field: NameField, Integer: 2))))",
                Parse(k, "[StrUCase(StrLeft(NameField;2))]"));
        }

        [TestMethod]
        public void Operators()
        {
            var k = MakeKeeper();
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: (IfVoid: If (Fun: > (Field: x, Boolean: 1), VoidProg: (VarSet: a (Integer: 2)), VoidProg: (VarSet: a (Integer: 3)))), Fun: + (Var: a, Boolean: 1)))",
                Parse(k, "[If(x>1;a=2;a=3):a+1]"));
            Assert.AreEqual("NodeList: (VoidProg: (IfVoid: If (Fun: == (Field: x, Boolean: 1), Fun: == (Field: x, Integer: 2), VoidProg: (VarSet: a (Integer: 2)), VoidProg: (VarSet: a (Integer: 3)), VoidProg: (VarSet: a (Field: x)))), String: ---, ValueProg: (VoidProg: , Var: a))",
                Parse(k, "[If(x==1;a=2;x==2;a=3;a=x)]---[a]"));
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , If: Если (Fun: True, ValueProg: (VoidProg: , NodeList: (String: true)), ValueProg: (VoidProg: , NodeList: (String: false)))))",
                Parse(k, "[Если(True();[true];[false])]"));
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , If: Если (Fun: == (Field: a, Boolean: 1), ValueProg: (VoidProg: , If: Если (Fun: == (Field: b, Real: 2.2), ValueProg: (VoidProg: , Time: #02.02.2002 00:00:00#), ValueProg: (VoidProg: , Time: #03.03.2003 00:00:00#))), ValueProg: (VoidProg: , Time: #01.01.2001 00:00:00#))))",
                Parse(k, "[Если(a==1;Если(b==2.2;#02.02.2002 00:00:00#;#03.03.2003 00:00:00#);#01.01.2001 00:00:00#)]"));
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , If: If (Boolean: 1, ValueProg: (VoidProg: , NodeList: (String: sss, ValueProg: (VoidProg: , If: If (Boolean: 1, ValueProg: (VoidProg: , Field: a), ValueProg: (VoidProg: , Field: b))))), ValueProg: (VoidProg: , NodeList: (String: rrr, ValueProg: (VoidProg: , If: If (Boolean: 1, Boolean: 0, Fun: or (Boolean: 0, Boolean: 1), ValueProg: (VoidProg: , Field: a), ValueProg: (VoidProg: , Field: b), ValueProg: (VoidProg: , Field: c), ValueProg: (VoidProg: , Field: d))))))))",
                Parse(k, "[If(1;[sss[If(1;a;b)]];[rrr[If(1;a;0;b;0 or 1;c;d)]])]"));

            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: (VarSet: x (Integer: 5), WhileVoid: While (Fun: > (Var: x, Boolean: 0), VoidProg: (VarSet: x (Fun: - (Var: x, Boolean: 1))))), Var: x))",
                Parse(k, "[x=5:While(x>0;x=x-1):x]"));
            Assert.AreEqual("NodeList: (String: aa , VoidProg: (VarSet: i (Boolean: 0)), String:  bb , ValueProg: (VoidProg: , Fun: * (While: While (Fun: < (Var: i, Field: n), ValueProg: (VoidProg: (VarSet: i (Fun: + (Var: i, Boolean: 1))), Fun: cos (Var: i)), Fun: sin (Field: n)), Fun: Log (Var: i, Real: 2.3))), String:  cc)",
                Parse(k, "aa [i=0] bb [While(i<n;i=i+1:cos(i);sin(n))*Log(i;2.3)] cc"));
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: (VarSet: i (Boolean: 0), VarSet: s (String: ''), WhileVoid: While (Fun: < (Var: i, Field: n), VoidProg: (VarSet: i (Fun: + (Var: i, Boolean: 1)), VarSet: s (Fun: + (Var: s, NodeList: (String: aa)))))), Var: s))",
                Parse(k, "[i=0:s='':While(i<n;i=i+1:s=s+[aa]):s]"));

            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , Over: OverTabl (ValueProg: (VoidProg: , Fun: + (Field: a, Field: b)))))",
                Parse(k, "[OverTabl(a+b)]"));
            Assert.AreEqual("NodeList: (String: sss_, ValueProg: (VoidProg: , Fun: + (Fun: + (Field: Field, String: '_'), Over: OverTabl (ValueProg: (VoidProg: , Field: UpField)))), String: _xxx)",
                Parse(k, "sss_[Field+ '_' + OverTabl(UpField)]_xxx"));
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: (OverVoid: OverTabl (VoidProg: (VarSet: a (Field: UpField)))), Var: a))",
                Parse(k, "[OverTabl(a=UpField):a]"));

            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , Sub: SubTabl (ValueProg: (VoidProg: , Fun: + (Field: a, Fun: NewLine)))))",
                Parse(k, "[SubTabl(a + NewLine)]"));
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , Sub: SubTabl (ValueProg: (VoidProg: , Fun: > (Field: x, Boolean: 1)), ValueProg: (VoidProg: , Field: a), ValueProg: (VoidProg: , String: ';'))))",
                Parse(k, "[SubTabl(x>1;a;';')]"));
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , Sub: SubTabl (ValueProg: (VoidProg: , NodeList: (String: sss_, ValueProg: (VoidProg: , Field: SubFied))), ValueProg: (VoidProg: , NodeList: (String: ';')))))",
                Parse(k, "[ SubTabl ([sss_[SubFied]];[';'])]"));
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , Sub: ПодТабл (ValueProg: (VoidProg: , Fun: + (Field: Поле, Over: НадТабл (ValueProg: (VoidProg: , Field: Поле)))), ValueProg: (VoidProg: , NodeList: (String: ';')))))",
                Parse(k, "[ПодТабл (Поле + НадТабл(Поле) ;[';'])]"));
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , Sub: ПодТабл (ValueProg: (VoidProg: , Fun: True), ValueProg: (VoidProg: , Fun: + (If: Если (Fun: and (Fun: == (Field: type, String: 'int'), Fun: == (Field: x, Boolean: 1)), ValueProg: (VoidProg: , Integer: 16), ValueProg: (VoidProg: , Integer: 32)), String: 'd')))), String: ooo)",
                Parse(k, "[ПодТабл(True;Если(type=='int' and x==1; 16; 32)+'d')]ooo"));
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , While: while (If: If (Fun: > (Field: x, Boolean: 1), ValueProg: (VoidProg: , Boolean: 1), ValueProg: (VoidProg: , Boolean: 0)), ValueProg: (VoidProg: , Field: a), Field: b)))",
                Parse(k, "[while(If(x>1;1;0);a;b)]"));
        }

        [TestMethod]
        public void Errors()
        {
            var k = MakeKeeper();
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , Field: rrr)) Незакрытая квадратная скобка, '[' (поле, строка: 1, позиция: 1)", Parse(k, "[rrr"));
            Assert.AreEqual("NodeList: (, ) Недопустимое использование лексемы, ']' (поле, строка: 1, позиция: 6)", Parse(k, "[rrr+]"));
            Assert.AreEqual("NodeList: () Лишняя закрывающаяся квадратная скобка, ']' (поле, строка: 1, позиция: 6)", Parse(k, "[www]]"));
            Assert.AreEqual("NodeList: (String: ss, ValueProg: (VoidProg: , Fun: + (Fun: + (Field: w, Field: x), NodeList: (String: sss))), String: +y) Недопустимое использование лексемы, ']' (поле, строка: 1, позиция: 16)",
                Parse(k, "ss[w+x+[sss]]+y]"));
            Assert.AreEqual("NodeList: (String: ss, ValueProg: (VoidProg: , Fun: + (Fun: + (Fun: + (Field: w, Field: x), NodeList: (String: sss)), Field: y))) Незакрытая квадратная скобка, '[' (поле, строка: 1, позиция: 3)",
                Parse(k, "ss[w+x+[sss]+y"));
            Assert.AreEqual("NodeList: (String: ss) Недопустимое использование лексемы, ']' (поле, строка: 1, позиция: 3)", Parse(k, "ss]"));
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , Fun: + (Field: x, Field: a))) Незакрытая скобка, '(' (поле, строка: 1, позиция: 4)", Parse(k, "[x+(a]"));
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , Fun: + (Field: x, Fun: + (Field: a, Integer: 2)))) Недопустимая последовательность символов, '}' (поле, строка: 1, позиция: 8)", 
                Parse(k, "[x+(a+2}]"));
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , Fun: + (Fun: * (Field: x, Fun: cos (Field: a)), Integer: 3))) Недопустимое использование лексемы, '2' (поле, строка: 1, позиция: 12)", 
                Parse(k, "[x*cos(a+3 2)]"));
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , Fun: cos (Field: asa))) Лишняя закрывающаяся скобка, ')' (поле, строка: 1, позиция: 10)",
                Parse(k, "[cos(asa))]"));
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , Fun: strMid (String: 'ddd'))) Недопустимая последовательность символов, ',' (поле, строка: 1, позиция: 14)",
                Parse(k, "[strMid('ddd',2))]"));

            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , If: If (Fun: > (Field: x, Integer: 3), ValueProg: (VoidProg: , Integer: 11), ValueProg: (VoidProg: , Integer: 12)))) Незакрытая скобка, '(' (поле, строка: 1, позиция: 4)",
                Parse(k, "[If(x>3;11;12]"));
            Assert.AreEqual("NodeList: (, ) Недопустимое использование лексемы, '1' (поле, строка: 1, позиция: 8)",
                Parse(k, "[while 1;2;3]]"));
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , While: while (If: If (Fun: > (Field: x, Boolean: 1), ValueProg: (VoidProg: , Boolean: 1), ValueProg: (VoidProg: , Boolean: 0)), ValueProg: (VoidProg: , Field: a), Field: b))) Незакрытая скобка, '(' (поле, строка: 1, позиция: 10)",
                Parse(k, "[while(If(x>1;1;0;a;b)]"));

            Assert.AreEqual("NodeList: (, ) Недопустимое использование лексемы, ')' (поле, строка: 1, позиция: 11)",
                Parse(k, "[OverTabl()]"));
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , Over: OverTabl (ValueProg: (VoidProg: , Field: Field)))) Недопустимое использование лексемы, ';' (поле, строка: 1, позиция: 18)",
                Parse(k, "[OverTabl( Field ;1)]"));
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , Over: OverTabl (ValueProg: (VoidProg: , Field: Field)))) Лишняя закрывающаяся скобка, ')' (поле, строка: 1, позиция: 18)",
                Parse(k, "[OverTabl( Field))]"));

            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , Fun: + (Field: a, Sub: SubTabl (ValueProg: (VoidProg: , Fun: True), ValueProg: (VoidProg: , Field: b), ValueProg: (VoidProg: , String: ':'))))) Недопустимое использование лексемы, ';' (поле, строка: 1, позиция: 24)",
                Parse(k, "[a + SubTabl(True;b;':';c]"));
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , Fun: + (Field: a, ))) Недопустимое использование лексемы, ';' (поле, строка: 1, позиция: 21)",
                Parse(k, "[a + SubTabl(True;b);':')]"));
            Assert.AreEqual("NodeList: (ValueProg: (VoidProg: , Fun: + (Field: a, Sub: SubTabl (ValueProg: (VoidProg: , Fun: True), ValueProg: (VoidProg: , Fun: + (Field: b, Boolean: 1)))))) Незакрытая скобка, '(' (поле, строка: 1, позиция: 13)",
                Parse(k, "[a + SubTabl(True;b+1]"));
        }
    }
}