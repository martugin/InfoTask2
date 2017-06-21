using BaseLibrary;
using Calculation;
using CommonTypes;

namespace Tablik
{
    //Интерфес для сигналов для Tablik
    public interface ITablikSignal
    {
        //Код сигнала
        string Code { get; }
        //Имя сигнала
        string Name { get; }
        //Ссылка на сигнал из Signals
        TablikSignal Signal { get; }
    }

    //---------------------------------------------------------------------------------------
    //Сигнал из Signals
    public class TablikSignal : BaseSignal, ITablikSignal
    {
        public TablikSignal(IRecordRead rec) 
            : base(rec) { }

        public TablikSignal Signal { get { return this; } }

        //Запись в рекордсет
        public void ToRecordset(DaoRec rec, int objectId, string objectCode)
        {
            rec.AddNew();
            rec.Put("ObjectId", objectId);
            rec.Put("FullCode", objectCode + "." + Code);
            rec.Put("CodeSignal", Code);
            rec.Put("NameSignal", Code);
            rec.Put("DataType", DataType.ToRussian());
            rec.Put("SignalType", SignalType);
            rec.Put("InfOut", InfOut);
            rec.Put("InfProp", InfProp);
            rec.Put("InitialSignals", InitialSignals);
            rec.Put("Formula", Formula);
            rec.Update();
        }
    }

    //---------------------------------------------------------------------------------------
    //Сигнал из SignalsCalc
    public class TypeSignal : ITablikSignal
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
    }
}