using CommonTypes;

namespace Calculation
{
    //Информация об узле выражения
    internal class CalcNodeInfo : CalcNode
    {
        public CalcNodeInfo(string code, int argsCount = 0)
        {
            Code = code;
            ArgsCount = argsCount;
        }

        //Код
        public string Code { get; private set; }
        //Количество аргументов
        public int ArgsCount { get; private set; }

        public override IVal Value { get { return null; } }
    }
}