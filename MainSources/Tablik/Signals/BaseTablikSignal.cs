using BaseLibrary;
using CommonTypes;

namespace Tablik
{
    //Сигнал из BaseSignals
    internal class BaseTablikSignal : ITablikSignalType
    {
        public BaseTablikSignal(IRecordRead rec)
        {
            Code = rec.GetString("CodeSignal");
            Name = rec.GetString("NameSignal");
            DataType = rec.GetString("DataType").ToDataType();
            Simple = new SimpleType(DataType);
        }

        //Код сигнала
        public string Code { get; private set; }
        //Имя сигнала
        public string Name { get; private set; }

        //Тип данных
        public DataType DataType { get; private set; }
        //Тип данных - сигнал
        public ITablikSignalType TablikSignalType { get { return this; } }
        //Тип данных - простой
        public SimpleType Simple { get; private set; }

        public bool LessOrEquals(ITablikType type)
        {
            return Simple.LessOrEquals(type);
        }

        //Запись в строку
        public string ToResString()
        {
            return "{" + Code + "}" + "(" + DataType + ")";
        }
    }
}