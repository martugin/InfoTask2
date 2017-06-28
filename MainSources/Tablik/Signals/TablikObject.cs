using BaseLibrary;
using Calculation;
using CommonTypes;

namespace Tablik
{
    //Один объект для Tablik
    internal class TablikObject : BaseObject, ITablikSignalType
    {
        public TablikObject(ObjectType type, IRecordRead rec) : base(rec)
        {
            ObjectType = type;
        }

        //Тип объекта
        public ObjectType ObjectType { get; private set; }

        //Используемые сигналы
        private readonly DicS<UsedSignal> _usedSignals = new DicS<UsedSignal>();
        public DicS<UsedSignal> UsedSignals { get { return _usedSignals; } }
        //Используемые свойства
        private readonly DicS<ObjectProp> _usedProps = new DicS<ObjectProp>();
        public DicS<ObjectProp> UsedProps { get { return _usedProps; } }

        //Запись объекта в рекорсеты вместе с сигналами и свойствами
        public void ToRecordsets(DaoRec recObjects, DaoRec recSignals, DaoRec recProps)
        {
            recObjects.AddNew();
            recObjects.Put("CodeObject", Code);
            recObjects.Put("NameObject", Name);
            recObjects.Put("InfObject", InfObject);
            int id = recObjects.GetInt("ObjectId");
            recObjects.Update();
            foreach (var prop in UsedProps.Values)
                prop.ToRecordset(recProps, id);
            foreach (var sig in UsedSignals.Values)
                sig.ToRecordset(recSignals, id);
        }

        //Сигнал по умолчанию
        public TablikSignal Signal { get { return ObjectType.DefaultSignal; } }
        //Тип данных
        public DataType DataType { get { return Signal.DataType; } }
        //Тип данных - простой
        public SimpleType Simple { get {return Signal.Simple;} }
        //Тип данных как сигнал
        public ITablikSignalType TablikSignalType { get { return this; } }

        //Является типом
        public bool IsOfType(ITablikType type)
        {
            if (type.TablikSignalType is ObjectType || type.TablikSignalType is TablikSignal)
                return ObjectType.IsOfType(type);
            return Simple.IsOfType(type);
        }
    }
}