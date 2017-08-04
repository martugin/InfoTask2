using System.Text;
using BaseLibrary;
using Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tablik;

namespace TablikTest
{
    [TestClass]
    public class ParseTest
    {
        private TablikKeeper _keeper;

        //Разбор списка входов
        private void CheckInputs(string formula, params string[] inputs)
        {
            _keeper.Errors.Clear();
            var sb = new StringBuilder("NodeList: (");
            for (int i = 0; i < inputs.Length; i++)
            {
                if (i != 0) sb.Append(", ");
                sb.Append("Input" + inputs[i]);
            }
            sb.Append(")");
            Assert.AreEqual(sb.ToString(), new InputsParsing(_keeper, formula).ToTestString()); 
        }

        private void CheckInputsError(string formula, string error)
        {
            _keeper.Errors.Clear();
            Assert.AreEqual("NodeList: " + error, new InputsParsing(_keeper, formula).ToTestString()); 
        }
        
        [TestMethod]
        public void InputsParse()
        {
            _keeper = new TablikKeeper(null);
            CheckInputs("aaa", "Simple: aaa (, , )");
            CheckInputs("int aaa", "Simple: aaa (Ident: int, , )");
            CheckInputs("Целое abc", "Simple: abc (Ident: Целое, , )");
            CheckInputs("СТРОКА h", "Simple: h (Ident: СТРОКА, , )");

            CheckInputs("a=3", "Simple: a (, , Integer: 3)");
            CheckInputs("bb=3.14", "Simple: bb (, , Real: 3.14)");
            CheckInputs("действ bb=3,15", "Simple: bb (Ident: действ, , Real: 3,15)");
            CheckInputs("c=1", "Simple: c (, , Boolean: 1)");
            CheckInputs("целое c=0", "Simple: c (Ident: целое, , Boolean: 0)");
            CheckInputs("s='sss'", "Simple: s (, , String: 'sss')");
            CheckInputs("st=''", "Simple: st (, , String: '')");
            CheckInputs("d=#10.10.2010 10:10:10#", "Simple: d (, , Time: #10.10.2010 10:10:10#)");

            CheckInputs("List(int) aaa", "Simple: aaa (Ident: int, Ident: List, )");
            CheckInputs("Список(bool) aaa", "Simple: aaa (Ident: bool, Ident: Список, )");
            CheckInputs("СловарьЧисла(действ) dic", "Simple: dic (Ident: действ, Ident: СловарьЧисла, )");
            CheckInputs("DicStrings(string) dic", "Simple: dic (Ident: string, Ident: DicStrings, )");

            CheckInputs("{SType} aaa", "Signal: aaa (Ident: {SType}, , )");
            CheckInputs("{sig-hhh.v} sig", "Signal: sig (Ident: {sig-hhh.v}, , )");

            CheckInputs("Par aaa", "Param: aaa (Ident: Par, , )");
            CheckInputs("Par.Sub a", "Param: a (Ident: Par, Ident: Sub, )");

            CheckInputs("aaa; bbb", "Simple: aaa (, , )", "Simple: bbb (, , )");
            CheckInputs("int aaa; bool bbb; real ccc", "Simple: aaa (Ident: int, , )", "Simple: bbb (Ident: bool, , )", "Simple: ccc (Ident: real, , )");
            CheckInputs("real sim; {otype} ob; p.s par", "Simple: sim (Ident: real, , )", "Signal: ob (Ident: {otype}, , )", "Param: par (Ident: p, Ident: s, )");

            CheckInputsError("(aaa(", "(InputSimple: aaa (, , )) Недопустимое использование лексемы, '(' (входы, строка: 1, позиция: 1)");
            CheckInputsError("aaa]", "(InputSimple: aaa (, , )) Недопустимая последовательность символов, ']' (входы, строка: 1, позиция: 4)");
            CheckInputsError("{a} {b}", "(InputSignal: (Ident: {a}, , )) Недопустимое использование лексемы, '{b}' (входы, строка: 1, позиция: 5)");
            CheckInputsError("a.b.c", "(InputParam: c (Ident: a, Ident: b, )) Недопустимое использование лексемы, '.' (входы, строка: 1, позиция: 4)");
        }
    }
}
