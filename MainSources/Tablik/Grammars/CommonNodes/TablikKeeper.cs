using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using BaseLibrary;
using CompileLibrary;

namespace Tablik
{
    //Накопление ошибок параметра в Tablik
    internal class TablikKeeper : ParsingKeeper
    {
        public TablikKeeper(TablikCalcParam param)
        {
            Param = param;
        }

        //Текущий расчетный параметр
        internal TablikCalcParam Param { get; private set; }
        //Текущий модуль
        internal TablikModule Module { get { return Param.Module; } }
        //Текущий проект
        internal TablikProject Project { get { return Module.TablikProject; } }
        //Таблик
        internal FunsChecker FunsChecker { get { return Project.FunsChecker; } }

        //Создание узлов - констант
        protected override Node MakeConstNode(ITerminalNode terminal, bool b)
        {
            return new TablikConstNode(terminal, b);
        }
        protected override Node MakeConstNode(ITerminalNode terminal, int i)
        {
            return new TablikConstNode(terminal, i);
        }
        protected override Node MakeConstNode(ITerminalNode terminal, double r)
        {
            return new TablikConstNode(terminal, r);
        }
        protected override Node MakeConstNode(ITerminalNode terminal, DateTime d)
        {
            return new TablikConstNode(terminal, d);
        }
        protected override Node MakeConstNode(ITerminalNode terminal, string s)
        {
            return new TablikConstNode(terminal, s);
        }

        //Получить список MetSignals от вершины
        public SetS GetMetSignals(IExprNode node)
        {
            if (node is VarNode)
                return ((VarNode) node).Var.MetSignals;
            if (node.Type is TablikCalcParam)
                return ((TablikCalcParam) node.Type).MetSignals;
            return null;
        }
        //Получить список MetProps от вершины
        public SetS GetMetProps(IExprNode node)
        {
            if (node is VarNode)
                return ((VarNode)node).Var.MetProps;
            if (node.Type is TablikCalcParam)
                return ((TablikCalcParam)node.Type).MetProps;
            return null;
        }

    }

    //------------------------------------------------------------------
    //Узел с возможностью записи ошибок
    internal abstract class KeeperNode : Node
    {
        protected KeeperNode(TablikKeeper keeper, ITerminalNode terminal)
            : base(terminal)
        {
            Keeper = keeper;
        }

        //Накопитель ошибок
        protected TablikKeeper Keeper { get; private set; }

        //Добавить ошибку и вернуть пустое значение
        protected void AddError(string text, IToken token)
        {
            Keeper.AddError(text, token);
        }
        protected void AddError(string text)
        {
            AddError(text, Token);
        }
    }
}