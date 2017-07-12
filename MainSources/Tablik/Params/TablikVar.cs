using BaseLibrary;
using CommonTypes;

namespace Tablik
{
    //Переменная в расчетном параметре или выход
    internal class TablikVar
    {
        public TablikVar(string code, ITablikType type = null, Mean defaultValue = null)
        {
            Code = code;
            Type = type;
            DefaultValue = defaultValue;
        }

        //Имя переменной
        public string Code { get; private set; }
        //Тип данных
        public ITablikType Type { get; set; }
        //Значение по умолчанию
        public Mean DefaultValue { get; private set; }

        //Список взятий сигналов для переменной типа объекта
        public SetS MetSignals { get; set; }
    }
}