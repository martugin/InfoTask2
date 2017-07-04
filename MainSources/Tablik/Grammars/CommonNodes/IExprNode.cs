using CompileLibrary;

namespace Tablik
{
    //Один элемент расчетного выражения
    internal interface IExprNode : INode
    {
        //Тип данных
        ITablikType Type { get; }
        //Определение типа данных
        void DefineType();
        //Запись в скомпилированое выражение
        string CompiledFullText();
    }
} 
