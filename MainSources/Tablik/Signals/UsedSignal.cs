using BaseLibrary;
using CommonTypes;

namespace Tablik
{
    //Используемый сигнал
    public class UsedSignal : ITablikSignalType
    {
        public UsedSignal(TablikSignal signal, TablikObject o)
        {
            Signal = signal;
            Object = o;
        }

        //Сигнал
        public TablikSignal Signal { get; private set; }
        //Объект
        public TablikObject Object { get; private set; }

        public DataType DataType { get { return Signal.DataType; } }
        public ITablikSignalType TablikSignalType { get { return this; } }
        public string Code { get { return Signal.Code; } }
        public string Name { get { return Signal.Name; } }

        //Запись в рекордсет
        public void ToRecordset(DaoRec rec, int objectId)
        {
            rec.AddNew();
            rec.Put("ObjectId", objectId);
            rec.Put("FullCode", Object.Code + "." + Signal.Code);
            rec.Put("CodeSignal", Signal.Code);
            rec.Put("NameSignal", Signal.Code);
            rec.Put("DataType", Signal.DataType.ToRussian());
            rec.Put("SignalType", Signal.SignalType);
            rec.Put("InfOut", Signal.InfOut);
            rec.Put("InfProp", Signal.InfProp);
            rec.Put("InitialSignals", Signal.InitialSignals);
            rec.Put("Formula", Signal.Formula);
            rec.Update();
        }
    }
}