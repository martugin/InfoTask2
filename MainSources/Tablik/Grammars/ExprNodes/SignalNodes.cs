using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using CompileLibrary;

namespace Tablik
{
    //Получение сигнала по полному коду
    internal class SignalNode : KeeperNode, ISyntacticNode, IExprNode
    {
        public SignalNode(TablikKeeper keeper, ITerminalNode terminal) 
            : base(keeper, terminal)
        {
            SignalCode = terminal.Symbol.Text;
        }

        //Тип узла
        protected override string NodeType { get { return "Signal"; }}
        
        //Код сигнала
        public string SignalCode { get; private set; }

        public IExprNode DefineSemantic()
        {
            
        }

        //Сигнал
        public UsedSignal Signal { get; private set; }

        //Тип данных
        public ITablikType Type { get { return Signal; } }
        public ITablikType DefineType() { return Type; }

        //Запись в скомпилированое выражение
        public string CompiledText()
        {
            return "Signal!{" + Signal.FullCode + "}";
        }
    }

    //------------------------------------------------------------------------------------------------
    //Получение объекта по коду
    internal class ObjectNode : Node, IExprNode
    {
        public ObjectNode(IToken token, TablikObject ob) 
            : base(token)
        {
            Object = ob;
        }

        //Тип узла
        protected override string NodeType { get { return "Object"; } }

        //Объект
        public TablikObject Object { get; private set; }

        //Тип данных
        public ITablikType Type { get { return Object; } }
        public ITablikType DefineType() { return Type; }

        public string CompiledText()
        {
            return "Object!{" + Object.Code + "}";
        }
    }

    //------------------------------------------------------------------------------------------------
    //Получение сигнала от объекта или типа
    internal class GetSignalNode : Node, ISyntacticNode, IExprNode
    {
        //Тип узла
        protected override string NodeType { get { return "GetSignal"; } }

        public IExprNode DefineSemantic()
        {
            throw new System.NotImplementedException();
        }

        //Тип сигнала
        public TypeSignal Signal { get; private set; }

        //Тип данных
        public ITablikType Type { get { return Signal; } }
        public ITablikType DefineType() { return Signal;}

        public string CompiledText()
        {
            return "GetSignal!{" + Signal.Code + "}!1";
        }
    }
}