using BaseLibrary;
using CommonTypes;
using CompileLibrary;

namespace Tablik
{
    //Интерфес для сигналов для Tablik
    internal interface ITablikSignalType : ITablikType
    {
        //Код сигнала
        string Code { get; }
        //Имя сигнала
        string Name { get; }
        //Сигнал, задающий тип значения
        TablikSignal Signal { get; }
    }

    //---------------------------------------------------------------------------------------
    //Сигнал из Signals
    internal class TablikSignal : BaseSignal, ITablikSignalType
    {
        public TablikSignal(IRecordRead rec) : base(rec)
        {
            Simple = new SimpleType(DataType);
        }

        //Сигнал
        public TablikSignal Signal { get { return this; } }
        //Тип данных - сигнал
        public ITablikSignalType TablikSignalType { get { return this; } }
        //Тип данных - простой
        public SimpleType Simple { get; private set; }

        //Чвляется типом
        public bool IsOfType(ITablikType type)
        {
            if (type.TablikSignalType is TablikSignal)
                return this == type.TablikSignalType;
            return Simple.IsOfType(type);
        }
    }

    //---------------------------------------------------------------------------------------
    //Сигнал из SignalsCalc
    internal class TypeSignal : ITablikSignalType
    {
        public TypeSignal(string code, string name)
        {
            Code = code;
            Name = name;
        }

        //Код сигнала
        public string Code { get; private set; }
        //Имя сигнала
        public string Name { get; private set; }
        //Ссылка на сигнал из Signals
        public TablikSignal Signal { get; set; }

        //Тип данных
        public DataType DataType { get { return Signal.DataType; } }
        //Тип данных - сигнал
        public ITablikSignalType TablikSignalType { get { return Signal; } }
        //Тип данных - простой
        public SimpleType Simple { get; private set; }

        public bool IsOfType(ITablikType type)
        {
            return Signal.IsOfType(type);
        }
    }
}