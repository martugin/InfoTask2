using Antlr4.Runtime.Tree;

namespace Tablik
{
    //Получение сигнала по полному коду
    internal class SignalNode : TablikKeeperNode
    {
        public SignalNode(TablikKeeper keeper, ITerminalNode terminal) 
            : base(keeper, terminal)
        {
            var code = terminal.Symbol.Text;
            string ocode = null, scode = null;
            int pos = code.LastIndexOf('.');
            if (pos != -1)
            {
                scode = code.Substring(pos + 1);
                ocode = code.Substring(0, pos);
            }
                
            foreach (var source in Keeper.Project.Sources.Values)
            {
                var objs = source.Objects;
                var uobjs = Keeper.Module.UsedObjects;
                if (objs.ContainsKey(code))
                {
                    var ob = objs[code];
                    if (!uobjs.ContainsKey(code))
                        uobjs.Add(code, ob);
                    if (ob.UsedSignals.ContainsKey(ob.Signal.Code))
                        Signal = ob.UsedSignals[ob.Signal.Code];
                    else
                    {
                        Signal = new UsedSignal(ob.Signal, ob);
                        ob.UsedSignals.Add(Signal.Code, Signal);    
                    }
                }
                if (pos != -1)
                    if (objs.ContainsKey(ocode) && objs[ocode].ObjectType.Signals.ContainsKey(scode))
                    {
                        var ob = objs[ocode];
                        if (!uobjs.ContainsKey(ocode))
                            uobjs.Add(ocode, ob);
                        if (ob.UsedSignals.ContainsKey(scode))
                            Signal = ob.UsedSignals[scode];
                        else
                        {
                            Signal = new UsedSignal(ob.ObjectType.Signals[scode], ob);
                            ob.UsedSignals.Add(scode, Signal);    
                        }
                    }
            }
        }

        //Тип узла
        protected override string NodeType { get { return "Signal"; }}

        //Сигнал
        public UsedSignal Signal { get; private set; }

        //Запись в скомпилированое выражение
        public override string CompiledText()
        {
            return "{" + Signal.FullCode + "}";
        }

        public override string ToTestString() { return ToTestWithChildren(); }
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

        public override string ToTestString() { return ToTestWithChildren(); }
    }
}