using System.Collections.Generic;
using CommonTypes;

namespace Calculation
{
    //Информация об узле выражения
    internal class CalcNodeInfo : CalcNode
    {
        public CalcNodeInfo(string code, int argsCount, Stack<ICalcNode> stack)
        {
            Code = code;
            if (argsCount != 0) ArgsArr = new ICalcNode[argsCount];
            for (int i = 0; i < argsCount; i++)
                ArgsArr[argsCount - i] = stack.Pop();
        }

        //Код
        public string Code { get; private set; }

        public override IVal Calculate() { return null; }
    }
}