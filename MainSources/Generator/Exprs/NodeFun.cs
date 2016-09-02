using System;
using System.Linq;
using Antlr4.Runtime.Tree;
using BaseLibrary;
using CommonTypes;

namespace Generator
{
    //Вычисление значения функции или операции
    internal class NodeFun : NodeKeeper, INodeExpr
    {
        public NodeFun(ParsingKeeper keeper, ITerminalNode terminal, //Имя функции
                                params INodeExpr[] args) //Аргументы
            : base(keeper, terminal)
        {
            _args = args;
        }

        public NodeFun(ParsingKeeper keeper, ITerminalNode terminal, //Имя функции
                                NodeList argsList) //Узел с аргументами
            : base(keeper, terminal)
        {
            _args = argsList.Children.Cast<INodeExpr>().ToArray();
        }

        protected override string NodeType { get { return "Fun"; } }

        //Добавить ошибку и вернуть пустое значение
        private Mean Error(string text)
        {
            AddError(text);
            return new MeanValue();
        }

        //Аргументы
        private readonly INodeExpr[] _args;
        //Делегат вычисления функции и его экземпляр
        private delegate Mean FunDelegate(params Mean[] pars);
        private FunDelegate _fun;

        public override string ToTestString()
        {
            return ToTestWithChildren(_args);
        }

        //Получение типа данных
        public DataType Check(TablStruct tabl)
        {
            throw new NotImplementedException();
        }

        //Вычисленное значение
        public Mean Generate(SubRows row)
        {
            if (_fun == orbb) return ProcessOr(_args[0], _args[1], row);
            if (_fun == andbb) return ProcessAnd(_args[0], _args[1], row);
            return _fun(_args.Select(x => x.Generate(row)).ToArray());
        }

        //Функции
        public Mean plusii(params Mean[] pars)
        {
            return new MeanInt(pars[0].Integer + pars[1].Integer);
        }
        public Mean plusrr(params Mean[] pars)
        {
            return new MeanReal(pars[0].Real + pars[1].Real);
        }
        public Mean plusss(params Mean[] pars)
        {
            return new MeanString(pars[0].String + pars[1].String);
        }

        public Mean minusi(params Mean[] pars)
        {
            return new MeanInt(-pars[0].Integer);
        }
        public Mean minusr(params Mean[] pars)
        {
            return new MeanReal(-pars[0].Real);
        }
        public Mean minusii(params Mean[] pars)
        {
            return new MeanInt(pars[0].Integer - pars[1].Integer);
        }
        public Mean minusrr(params Mean[] pars)
        {
            return new MeanReal(pars[0].Real - pars[1].Real);
        }

        public Mean multiplyii(params Mean[] pars)
        {
            return new MeanInt(pars[0].Integer * pars[1].Integer);
        }
        public Mean multiplyrr(params Mean[] pars)
        {
            return new MeanReal(pars[0].Real * pars[1].Real);
        }

        public Mean dividerr(params Mean[] pars)
        {
            if (pars[1].Real == 0)
                return Error("Деление на 0");
            return new MeanReal(pars[0].Real / pars[1].Real);
        }

        public Mean divii(params Mean[] pars)
        {
            if (pars[1].Integer == 0)
                return Error("Целочисленное деление на 0");
            return new MeanInt(pars[0].Integer / pars[1].Integer);
        }

        public Mean modii(params Mean[] pars)
        {
            if (pars[1].Integer == 0)
                return Error("Деление с остатком на 0");
            return new MeanInt(pars[0].Integer % pars[1].Integer);
        }

        public Mean equaluu(params Mean[] pars)
        {
            return new MeanBool(pars[0].ValueEquals(pars[1]));
        }

        public Mean notequaluu(params Mean[] pars)
        {
            return new MeanBool(!pars[0].ValueEquals(pars[1]));
        }

        public Mean lessuu(params Mean[] pars)
        {
            return new MeanBool(pars[0].ValueLess(pars[1]));
        }

        public Mean lessequaluu(params Mean[] pars)
        {
            return new MeanBool(pars[0].ValueEquals(pars[1]) || pars[0].ValueLess(pars[1]));
        }

        public Mean greaterequaluu(params Mean[] pars)
        {
            return new MeanBool(!pars[0].ValueLess(pars[1]));
        }

        public Mean greateruu(params Mean[] pars)
        {
            return new MeanBool(!pars[0].ValueEquals(pars[1]) && !pars[0].ValueLess(pars[1]));
        }

        public Mean andbb(params Mean[] pars) { return null;}
        private Mean ProcessAnd(INodeExpr p0, INodeExpr p1, SubRows row)
        {
            if (!p0.Generate(row).Boolean)
                return new MeanBool(false);
            return new MeanBool(p0.Generate(row).Boolean && p1.Generate(row).Boolean);
        }

        public Mean orbb(params Mean[] pars) { return null; }
        private Mean ProcessOr(INodeExpr p0, INodeExpr p1, SubRows row)
        {
            if (p0.Generate(row).Boolean)
                return new MeanBool(true);
            return new MeanBool(p0.Generate(row).Boolean || p1.Generate(row).Boolean);
        }
        
        public Mean notb(params Mean[] pars)
        {
            return new MeanBool(!pars[0].Boolean);
        }

        public Mean xorbb(params Mean[] pars)
        {
            return new MeanBool(pars[0].Boolean^pars[1].Boolean);
        }

        public Mean like(params Mean[] pars)
        {
            throw new NotImplementedException();
        }

        public Mean bitii(params Mean[] pars)
        {
            if (pars[1].Integer < 0 || pars[1].Integer > 32)
                return Error("Недопустимый номер бита");
            return new MeanBool(pars[0].Integer.GetBit(pars[1].Integer));
        }

        public Mean absi(params Mean[] pars)
        {
            return new MeanInt(Math.Abs(pars[0].Integer));
        }

        public Mean absr(params Mean[] pars)
        {
            return new MeanReal(Math.Abs(pars[0].Real));
        }

        public Mean roundr(params Mean[] pars)
        {
            return new MeanInt((int)Math.Round(pars[0].Real));
        }

        public Mean roundri(params Mean[] pars)
        {
            return new MeanReal(Math.Round(pars[0].Real, pars[1].Integer));
        }

        public Mean strings(params Mean[] pars)
        {
            return new MeanString(pars[0].String);
        }

        public Mean reals(params Mean[] pars)
        {
            return new MeanReal(pars[0].Real);
        }

        public Mean ints(params Mean[] pars)
        {
            return new MeanInt(pars[0].Integer);
        }

        public Mean isints(params Mean[] pars)
        {
            return new MeanBool(pars[0].Integer.ToString() == pars[0].String);
        }

        public Mean isreals(params Mean[] pars)
        {
            return new MeanBool(pars[0].Real.ToString() == pars[0].String);
        }

        public Mean strmidsi(params Mean[] pars)
        {
            return new MeanString(pars[0].String.Substring(pars[1].Integer));
        }

        public Mean strmidsii(params Mean[] pars)
        {
            return new MeanString(pars[0].String.Substring(pars[1].Integer, pars[2].Integer));
        }

        public Mean strleftsi(params Mean[] pars)
        {
            return new MeanString(pars[0].String.Substring(0, pars[1].Integer));
        }

        public Mean strrightsi(params Mean[] pars)
        {
            string s = pars[0].String;
            int i = pars[1].Integer;
            return new MeanString(s.Substring(s.Length-i, i));
        }

        public Mean strlens(params Mean[] pars)
        {
            return new MeanInt(pars[0].String.Length);
        }

        public Mean strfindss(params Mean[] pars)
        {
            return new MeanInt(pars[0].String.Length);
        }
    }
}