using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Tablik
{
    //Узел, задающий тип данных переменной
    internal abstract class TypeNode : KeeperNode, IExprNode, ISyntacticNode
    {
        protected TypeNode(TablikKeeper keeper, ITerminalNode terminal) 
            : base(keeper, terminal) { }

        //Тип данных
        public ITablikType Type { get; protected set; }

        //Определение типа данных
        public abstract IExprNode DefineSemantic();

        public ITablikType DefineType()
        {
            return Type;
        }

        public string CompiledText() { return ""; }
    }

    //-----------------------------------------------------------------------------------
    //Простой тип данных
    internal class DataTypeNode : TypeNode
    {
        public DataTypeNode(ITerminalNode terminal) 
            : base(null, terminal) { }

        protected override string NodeType { get { return "DataType"; } }

        //Определение типа данных
        public override IExprNode DefineSemantic()
        {
            Type = new SimpleType(Token.Text.ToDataType());
            return this;
        }
    }

    //-----------------------------------------------------------------------------------
    //Массив
    internal class ArrayTypeNode : TypeNode
    {
        public ArrayTypeNode(ITerminalNode array, ITerminalNode dataType)
            : base(null, array)
        {
            _dataType = dataType.Symbol.Text.ToDataType();
        }

        protected override string NodeType { get { return "ArrayType"; } }

        //Тип данных
        private readonly DataType _dataType;
        
        //Определение типа данных
        //Определение типа данных
        public override IExprNode DefineSemantic()
        {
            Type = new SimpleType(_dataType, Token.Text.ToArrayType());
            return this;
        }
    }

    //-----------------------------------------------------------------------------------
    //Тип данных - сигнал
    internal class SignalTypeNode : TypeNode
    {
        public SignalTypeNode(TablikKeeper keeper, ITerminalNode terminal) 
            : base(keeper, terminal)
        {
            SignalCode = Token.Text.Substring(1, Token.Text.Length - 2);
        }

        protected override string NodeType { get { return "SignalType"; }}

        //Код сигнала или типа объекта
        public string SignalCode { get; private set;  }

        //Определение типа объекта
        public override IExprNode DefineSemantic()
        {
            foreach (var con in Module.LinkedSources)
            {
                if (con.ObjectsTypes.ContainsKey(SignalCode))
                {
                    Type = con.ObjectsTypes[SignalCode];
                    return this;
                }
                if (con.BaseTypes.ContainsKey(SignalCode))
                {
                    Type = con.BaseTypes[SignalCode];
                    return this;
                }
                if (con.Signals.ContainsKey(SignalCode))
                {
                    Type = con.Signals[SignalCode];
                    return this;
                }
            }
            AddError("Не найден тип объекта или сигнала");
            Type = new SimpleType();
            return this;
        }
    }

    //-----------------------------------------------------------------------------------
    //Тип данных - параметр
    internal class ParamTypeNode : TypeNode
    {
        public ParamTypeNode(TablikKeeper keeper, IEnumerable<ITerminalNode> codes)
            : base(keeper, codes.First())
        {
            _codes = codes;
        }

        protected override string NodeType { get { return "ParamType"; } }

        //Список идентификаторов
        private readonly IEnumerable<ITerminalNode> _codes;
        
        //Определение типа объекта
        public override IExprNode DefineSemantic()
        {
            ISubParams p = Keeper.Module;
            foreach (var code in _codes)
            {
                if (p.Params.ContainsKey(code.Symbol.Text))
                    p = p.Params[code.Symbol.Text];
                else
                {
                    AddError("Не найден " + (p == Module ? "" : "под") + "параметр", code.Symbol);
                    Type = new SimpleType();
                    return this;
                }
            }
            Type = (TablikParam)p;
            return this;
        }
    }
}