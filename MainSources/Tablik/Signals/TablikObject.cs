using BaseLibrary;
using Calculation;

namespace Tablik
{
    //Один объект для Tablik
    public class TablikObject : BaseObject
    {
        public TablikObject(ObjectType type, IRecordRead rec) : base(rec)
        {
            ObjectType = type;
        }

        //Тип объекта
        public ObjectType ObjectType { get; private set; }

        //Используемые сигналы
        private readonly DicS<TablikSignal> _usedSignals = new DicS<TablikSignal>();
        public DicS<TablikSignal> UsedSignals { get { return _usedSignals; } }
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
                sig.Signal.ToRecordset(recSignals, id, Code);
        }
    }
}