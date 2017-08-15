using System.Collections.Generic;
using CommonTypes;
using CompileLibrary;

namespace Calculation
{
    //Интерфейс для узла расчетного выражения
    public interface ICalcNode : INode 
    {
        //Входные аргументы
        IEnumerable<ICalcNode> Args { get; }
        //Вычислить значение
        IVal Calculate();
    }
}