using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using CompileLibrary;

namespace Tablik
{
    //Накопление ошибок параметра в Tablik
    internal class TablikKeeper : ParsingKeeper
    {
        public TablikKeeper(TablikParam param)
        {
            Param = param;
        }

        //Текущий расчетный параметр
        internal TablikParam Param { get; private set; }
        //Текущий модуль
        internal TablikModule Module { get { return Param.Module; } }
        //Текущий проект
        internal TablikProject Project { get { return Module.TablikProject; } }

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

        //Текущий расчетный параметр
        internal TablikParam Param { get { return Keeper.Param; } }
        //Текущий модуль
        internal TablikModule Module { get { return Param.Module; } }
        //Текущий проект
        internal TablikProject Project { get { return Module.TablikProject; } }
    }
}