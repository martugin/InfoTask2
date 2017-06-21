using BaseLibrary;

namespace Tablik
{
    //Тип объекта 
    public class ObjectType
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
        private readonly DicS<ObjectType> _baseTypes = new DicS<ObjectType>();
        public DicS<ObjectType> BaseTypes { get { return _baseTypes; } }
        //Номер колонки в таблице сигналов
        internal int SignalCodeColumn { get; private set; }

        //Словарь сигналов
        private readonly DicS<ITablikSignal> _signals = new DicS<ITablikSignal>();
        public DicS<ITablikSignal> Signals { get { return _signals; } }
        //Сигнал по умолчанию
        public TablikSignal DefaultSignal { get; set; }
    }
}