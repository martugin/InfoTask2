using System.Collections.Generic;
using CommonTypes;
using CompileLibrary;

namespace Calculation
{
    //Интерфейс для узла расчетного выражения
    public interface ICalcNode : INode 
    {
        //Возвращаемое значение
        IVal Value { get; }
        //Входные аргументы
        List<ICalcNode> Args { get; }
    }
}