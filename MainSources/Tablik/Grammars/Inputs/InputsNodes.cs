﻿using Antlr4.Runtime.Tree;
using CompileLibrary;

namespace Tablik
{
    //Простой вход
    internal class InputArgNode : Node
    {
        public InputArgNode(ITerminalNode codeToken, ITerminalNode dataTypeNode = null) 
            : base(null)
        {
            CodeToken = codeToken;
            if (dataTypeNode != null)
                TypeNode = new IdentNode(dataTypeNode);
        }

        protected override string NodeType { get { return "Arg"; } }

        //Тип данных
        public IdentNode TypeNode { get; private set; }
        //Имя входа
        public ITerminalNode CodeToken { get; private set; }
    }

    //-----------------------------------------------------------------------------------
    //Тип входа 
    internal enum InputType
    {
        Simple, //Обычный тип данных
        Signal, //Тип объекта
        Param //Расчетная функция
    }

    //-----------------------------------------------------------------------------------
    //Полное описание одного входа
    internal class InputNode : Node
    {
        public InputNode(ITerminalNode codeToken, InputType inputType, IdentNode typeNode, IdentNode subNode = null, ConstNode valueNode = null)
            : base(codeToken)
        {
            InputType = inputType;
            TypeNode = typeNode;
            SubTypeNode = subNode;
            ValueNode = valueNode;
        }

        //Тип входа
        public InputType InputType { get; private set; }
        //Тип данных, имя сигнала или параметра
        public IdentNode TypeNode { get; private set; }
        //Имя подпараметра
        public IdentNode SubTypeNode { get; private set; }
        //Значение по умолчанию
        public ConstNode ValueNode { get; private set; }

        protected override string NodeType { get { return "Input"; } }
    }
}