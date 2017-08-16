using BaseLibrary;
using CommonTypes;
using CompileLibrary;

namespace Tablik
{
    //Сигнал из Signals
    internal class TablikSignal : BaseSignal, ITablikSignalType
    {
        public TablikSignal(IRecordRead rec) : base(rec)
        {
            Simple = new SimpleType(DataType);
        }

        //Тип данных - сигнал
        public ITablikSignalType TablikSignalType { get { return this; } }
        //Тип данных - простой
        public SimpleType Simple { get; private set; }
        
        //Чвляется типом
        public bool LessOrEquals(ITablikType type)
        {
            return Simple.LessOrEquals(type);
        }

        //Запись в строку
        public string ToResString()
        {
            return "{" + Code + "}" + "(" + DataType + ")";
        }

        //Запись в рекордсет
        public void ToRecordset(DaoRec rec, int objectId, string objectCode)
        {
            rec.AddNew();
            rec.Put("ObjectId", objectId);
            rec.Put("FullCode", objectCode + "." + Code);
            rec.Put("CodeSignal", Code);
            rec.Put("NameSignal", Name);
            rec.Put("DataType", DataType.ToRussian());
            rec.Put("SignalType", SignalType);
            rec.Put("InfOut", InfOut);
            rec.Put("InfProp", InfProp);
            rec.Put("InitialSignals", InitialSignals);
            rec.Put("Formula", Formula);
            rec.Update();
        }
    }
}