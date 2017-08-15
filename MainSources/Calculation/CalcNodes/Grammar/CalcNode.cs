using System.Collections.Generic;
using CommonTypes;
using CompileLibrary;

namespace Calculation
{
    internal abstract class CalcNode : Node, ICalcNode
    {
        //Аргументы
        public ICalcNode[] ArgsArr { get; protected set; }
        public IEnumerable<ICalcNode> Args {get { return ArgsArr; }}
        //Вычислить значение
        public abstract IVal Calculate();

        //Запись в строку
        public override string ToTestString()
        {
            return ToTestWithChildren(ArgsArr);
        }
    }
}