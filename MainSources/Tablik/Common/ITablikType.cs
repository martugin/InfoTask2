using CommonTypes;

namespace Tablik
{
    //Интерфейс для типов данных элементов выражения
    public interface ITablikType
    {
        //Тип данных
        DataType DataType { get; }
        //Тип данных - сигнал
        ITablikSignalType TablikSignalType { get; }
    }

    //---------------------------------------------------------------------------------
    //Простой тип данных
    public class SimpleType : ITablikType
    {
        public SimpleType(DataType dataType)
        {
            DataType = dataType;
        }

        //Тип данных
        public DataType DataType { get; private set; }
        //Тип сигнала
        public ITablikSignalType TablikSignalType { get { return null; } }
    }
}