using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Tablik
{
    //Тип объекта 
    public class ObjectType : ITablikSignalType
    {
        public ObjectType(int id, string code, string name, int signalCodeColumn)
        {
            Id = id;
            Code = code;
            Name = name;
            SignalCodeColumn = signalCodeColumn;
        }

        //Id в таблице ObjectTypes или ObjectTypesCalc
        public int Id { get; private set; }
        //Код типа объекта
        public string Code { get; private set; }
        //Имя
        public string Name { get; private set; }
        //Базовые типы для данного
        private readonly HashSet<ObjectType> _baseTypes = new HashSet<ObjectType>();
        public HashSet<ObjectType> BaseTypes { get { return _baseTypes; } }
        //Номер колонки в таблице сигналов
        internal int SignalCodeColumn { get; private set; }

        //Словарь сигналов
        private readonly DicS<ITablikSignalType> _signals = new DicS<ITablikSignalType>();
        public DicS<ITablikSignalType> Signals { get { return _signals; } }
        //Сигнал по умолчанию
        public TablikSignal DefaultSignal { get; set; }

        //Тип данных как сигнал
        public ITablikSignalType TablikSignalType { get { return this; } }
        //Сигнал по умолчанию
        public TablikSignal Signal { get { return DefaultSignal; } }
        //Тип данных
        public DataType DataType { get { return Signal.DataType; } }
    }
}