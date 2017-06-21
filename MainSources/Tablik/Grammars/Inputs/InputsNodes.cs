using Antlr4.Runtime.Tree;
using CompileLibrary;

namespace Tablik
{
    //Простой вход
    internal class InputsArgNode : Node
    {
        public InputsArgNode(ITerminalNode code, ITerminalNode dataType = null) 
            : base(code)
        {
            DataType = dataType;
        }

        protected override string NodeType { get { return "Arg"; } }

        //Тип данных
        public ITerminalNode DataType { get; private set; }
    }

    //-----------------------------------------------------------------------------------
    //Тип входа 
    internal enum InputType
    {
        DataType, //Обычный тип данных
        Object, //Тип объекта
        Param //Расчетная функция
    }

    //-----------------------------------------------------------------------------------
    //Полное описание одного входа
    internal class InputNode : KeeperNode
    {
        public InputNode(ParsingKeeper keeper, ITerminalNode code, InputType inputType, ListNode type, ConstNode defaultValue = null)
            : base(keeper, code)
        {
            InputType = inputType;
            Type = type;
            DefaultValue = defaultValue;
        }

        //Тип входа
        public InputType InputType { get; private set; }
        //Тип данных как цепочка идентификаторов
        public ListNode Type { get; private set; }
        //Значение по умолчанию
        public ConstNode DefaultValue { get; private set; }

        protected override string NodeType { get { return "Input"; } }
    }
}