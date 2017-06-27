using BaseLibrary;
using CommonTypes;

namespace Tablik
{
    //Используемый сигнал
    public class UsedSignal : ITablikSignalType
    {
        public UsedSignal(TablikSignal signal, TablikObject ob)
        {
            Signal = signal;
            Object = ob;
        }

        //Сигнал
        public TablikSignal Signal { get; private set; }
        //Объект
        public TablikObject Object { get; private set; }

        public DataType DataType { get { return Signal.DataType; } }
        public ITablikSignalType TablikSignalType { get { return this; } }
        public string Code { get { return Signal.Code; } }
        public string Name { get { return Signal.Name; } }
        //Полный код
        public string FullCode { get { return Object.Code + "." + Signal.Code; } }

        //Запись в рекордсет
        public void ToRecordset(DaoRec rec, int objectId)
        {
            rec.AddNew();
            rec.Put("ObjectId", objectId);
            rec.Put("FullCode", FullCode);
            rec.Put("CodeSignal", Code);
            rec.Put("NameSignal", Name);
            rec.Put("DataType", DataType.ToRussian());
            rec.Put("SignalType", Signal.SignalType);
            rec.Put("InfOut", Signal.InfOut);
            rec.Put("InfProp", Signal.InfProp);
            rec.Put("InitialSignals", Signal.InitialSignals);
            rec.Put("Formula", Signal.Formula);
            rec.Update();
        }
    }
}