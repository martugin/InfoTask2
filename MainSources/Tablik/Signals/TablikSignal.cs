using BaseLibrary;
using Calculation;
using CommonTypes;

namespace Tablik
{
    //Интерфес для сигналов для Tablik
    public interface ITablikSignalType : ITablikType
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
    public class TablikSignal : BaseSignal, ITablikSignalType
    {
        public TablikSignal(IRecordRead rec) 
            : base(rec) { }

        //Сигнал
        public TablikSignal Signal { get { return this; } }
        //Тип данных - сигнал
        public ITablikSignalType TablikSignalType { get { return this; } }
    }

    //---------------------------------------------------------------------------------------
    //Сигнал из SignalsCalc
    public class TypeSignal : ITablikSignalType
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

        public DataType DataType { get { return Signal.DataType; } }
        public ITablikSignalType TablikSignalType { get { return Signal; } }
    }
}