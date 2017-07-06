using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using CommonTypes;
using CompileLibrary;

namespace Tablik
{
    //Простой тип данных
    internal class DataTypeNode : TablikNode
    {
        public DataTypeNode(ITerminalNode terminal) //Тип вершины
            : base(terminal)
        {
            Type = new SimpleType(Token.Text.ToDataType());
        }
    }

    //-----------------------------------------------------------------------------------
    //Массив
    internal class ArrayTypeNode : TablikNode
    {
        public ArrayTypeNode(ITerminalNode array, ITerminalNode dataType) : base(array)
        {
            Type = new SimpleType(dataType.Symbol.Text.ToDataType(), Token.Text.ToArrayType());
        }
    }

    //-----------------------------------------------------------------------------------
    //Тип данных - сигнал
    internal class SignalTypeNode : TablikKeeperNode
    {
        public SignalTypeNode(TablikKeeper keeper, ITerminalNode terminal) 
            : base(keeper, terminal)
        {
            var signalCode = Token.Text.Substring(1, Token.Text.Length - 2);

            foreach (var con in Keeper.Module.LinkedSources)
            {
                if (con.ObjectsTypes.ContainsKey(signalCode))
                    Type = con.ObjectsTypes[signalCode];
                if (con.BaseTypes.ContainsKey(signalCode))
                    Type = con.BaseTypes[signalCode];
                if (con.Signals.ContainsKey(signalCode))
                    Type = con.Signals[signalCode];
            }
            if (Type == null)
                AddError("Не найден тип объекта или сигнала");
        }
    }

    //-----------------------------------------------------------------------------------
    //Тип данных - параметр
    internal class ParamTypeNode : TablikKeeperNode
    {
        public ParamTypeNode(TablikKeeper keeper, IEnumerable<ITerminalNode> codes)
            : base(keeper, codes.First())
        {
            ISubParams p = Keeper.Module;
            foreach (var code in codes)
            {
                if (p.Params.ContainsKey(code.Symbol.Text))
                    p = p.Params[code.Symbol.Text];
                else
                {
                    AddError("Не найден " + (p == Keeper.Module ? "" : "под") + "параметр", code.Symbol);
                    Type = new SimpleType();
                }
            }
            Type = (TablikParam)p;
        }
    }
}