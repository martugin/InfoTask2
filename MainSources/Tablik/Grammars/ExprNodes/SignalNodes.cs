using System;
using Antlr4.Runtime.Tree;

namespace Tablik
{
    //Получение сигнала по полному коду
    internal class SignalNode : TablikKeeperNode
    {
        public SignalNode(TablikKeeper keeper, ITerminalNode terminal) 
            : base(keeper, terminal)
        {
            SignalCode = terminal.Symbol.Text;
            throw new NotImplementedException();
        }

        //Тип узла
        protected override string NodeType { get { return "Signal"; }}
        
        //Код сигнала
        public string SignalCode { get; private set; }

        //Сигнал
        public UsedSignal Signal { get; private set; }

        //Запись в скомпилированое выражение
        public override string CompiledText()
        {
            return "{" + Signal.FullCode + "}";
        }
    }

    //------------------------------------------------------------------------------------------------
    //Получение объекта по коду
    internal class ObjectNode : TablikKeeperNode
    {
        public ObjectNode(TablikKeeper keeper, ITerminalNode terminal, TablikObject ob) 
            : base(keeper, terminal)
        {
            Object = ob;
        }

        //Тип узла
        protected override string NodeType { get { return "Object"; } }

        //Объект
        public TablikObject Object { get; private set; }
        
        public override string CompiledText()
        {
            return "{" + Object.Code + "}";
        }
    }

    //------------------------------------------------------------------------------------------------
    //Получение сигнала от объекта или типа
    internal class GetSignalNode : TablikKeeperNode
    {
        //Тип узла
        public GetSignalNode(TablikKeeper keeper, ITerminalNode terminal, params IExprNode[] args) 
            : base(keeper, terminal, args) { }

        protected override string NodeType { get { return "GetSignal"; } }

        //Тип сигнала
        public TypeSignal Signal { get; private set; }

        public override string CompiledText()
        {
            return "{" + Signal.Code + "}!1";
        }
    }
}