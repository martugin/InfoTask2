using CompileLibrary;

namespace Tablik
{
    //Один элемент синтаксического разбора
    internal interface ISyntacticNode : INode
    {
        //Преобразовать в узел с семантикой
        IExprNode DefineSemantic();
    }

    //-----------------------------------------------------------------------------------------------
    //Один элемент расчетного выражения
    internal interface IExprNode : INode
    {
        //Тип данных
        ITablikType Type { get; }
        //Определение типа данных
        ITablikType DefineType();
        //Запись в скомпилированое выражение
        string CompiledText();
    }
} 
