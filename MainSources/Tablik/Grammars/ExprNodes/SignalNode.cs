using Antlr4.Runtime.Tree;

namespace Tablik
{
    //Получение сигнала по полному коду
    internal class SignalNode : TablikKeeperNode
    {
        public SignalNode(TablikKeeper keeper, ITerminalNode terminal) 
            : base(keeper, terminal)
        {
            var code = terminal.Symbol.Text.Substring(1, terminal.Symbol.Text.Length - 2);
            string ocode = null, scode = null;
            int pos = code.LastIndexOf('.');
            if (pos != -1)
            {
                scode = code.Substring(pos + 1);
                ocode = code.Substring(0, pos);
            }
            else ocode = code.Substring(1, code.Length - 2);
                
            foreach (var source in Keeper.Project.Sources.Values)
            {
                var objs = source.Objects;
                var uobjs = Keeper.Module.UsedObjects;
                if (objs.ContainsKey(code))
                {
                    if (Object != null) AddError("Указанный объект присутствует в нескольких источниках");
                    Object = objs[ocode = code];
                    Signal = Object.Signal;
                    _isObject = true;
                }
                else if (pos != -1 && objs.ContainsKey(ocode) && objs[ocode].ObjectType.Signals.ContainsKey(scode))
                {
                    if (Object != null) AddError("Указанный сигнал присутствует в нескольких источниках");
                    Object = objs[ocode];
                    Signal = Object.ObjectType.Signals[scode];
                    _isObject = false;
                }
                if (!uobjs.ContainsKey(ocode))
                    uobjs.Add(ocode, Object);
                if (!Object.UsedSignals.Contains(Signal))
                    Object.UsedSignals.Add(Signal);    
            }
        }

        //Тип узла
        private readonly bool _isObject;
        protected override string NodeType { get { return _isObject ? "Object" : "Signal"; }}

        //Объект
        public TablikObject Object { get; private set; }
        //Сигнал
        public TablikSignal Signal { get; private set; }

        //Запись в скомпилированое выражение
        public override string CompiledText()
        {
            string ob = "{" + Object.Connect.Code + "}.{" + Object.Code + "}";
            return _isObject ? ob : ob + ".{" + Signal.Code +"}";
        }

        public override string ToTestString() { return ToTestWithChildren(); }
    }
}