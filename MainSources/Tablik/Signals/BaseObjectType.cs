using BaseLibrary;

namespace Tablik
{
    //Тип объекта (базовый из BaseObjectTypes)
    internal class BaseObjectType : ObjectTypeBase
    {
        public BaseObjectType(IRecordRead rec)
            : base(rec.GetInt("BaseTypeId"), rec.GetString("CodeType"), rec.GetString("NameType"))
        {
            SignalColumnNum = rec.GetInt("SignalColumnNum");
        }

        //Номер колонки в таблице сигналов
        internal int SignalColumnNum { get; private set; }

        //Словарь сигналов
        private readonly DicS<BaseTablikSignal> _signals = new DicS<BaseTablikSignal>();
        public DicS<BaseTablikSignal> Signals { get { return _signals; } }

        //Сигнал по умолчанию
        private BaseTablikSignal _signal;
        public BaseTablikSignal Signal
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
            if (type is BaseObjectType) return this == type;
            if (type is SimpleType) Simple.LessOrEquals(type);
            return false;
        }
    }
}