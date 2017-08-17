using System.Collections.Generic;
using BaseLibrary;

namespace Tablik
{
    //Тип объекта (из ObjectTypes)
    internal class ObjectType : ObjectTypeBase
    {
        public ObjectType(TablikConnect connect, IRecordRead rec)
            : base(connect, rec.GetInt("TypeId"), rec.GetString("TypeCode"), rec.GetString("TypeName")) { }
        
        //Базовые типы для данного
        private readonly HashSet<BaseObjectType> _baseTypes = new HashSet<BaseObjectType>();
        public HashSet<BaseObjectType> BaseTypes { get { return _baseTypes; } }
        //Словарь сигналов
        private readonly DicS<TablikSignal> _signals = new DicS<TablikSignal>();
        public DicS<TablikSignal> Signals { get { return _signals; } }
        //Словарь базовых сигналов
        private readonly DicS<BaseTablikSignal> _baseSignals = new DicS<BaseTablikSignal>();
        public DicS<BaseTablikSignal> BaseSignals { get { return _baseSignals; } }

        //Сигнал по умолчанию
        private TablikSignal _signal;
        public TablikSignal Signal
        {
            get { return _signal; }
            set
            {
                _signal = value;
                Simple = new SimpleType(value.DataType);
            }
        }
        
        //Является типом
        public override bool LessOrEquals(ITablikType type)
        {
            if (type is ObjectType) return this == type;
            if (type is BaseObjectType)
                return BaseTypes.Contains((BaseObjectType)type);
            if (type is SimpleType) Simple.LessOrEquals(type);
            return false;
        }
    }
}