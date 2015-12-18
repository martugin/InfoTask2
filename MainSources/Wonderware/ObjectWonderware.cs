using System;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    //Объект
    internal class ObjectWonderware : SourceObject
    {
        public ObjectWonderware(string code) : base(code)
        {
            Inf = code;
        }

        //Добавляет сигнал в объект, если еще такого нет
        public SignalWonderware AddSignal(SignalWonderware signal)
        {
            if (ValueSignal != null) return ValueSignal;
            return ValueSignal = signal;
        }
        
        //Дискретный или аналоговый сигнал
        public SignalWonderware ValueSignal { get; private set; }

        //Тип данных значения объекта
        public DataType DataType
        {
            get
            {
                if (ValueSignal != null) return ValueSignal.DataType;
                return DataType.Integer;
            }
        }

        //Добавить срез
        public override int AddBegin(DateTime time)
        {
            return SignalsAddBegin(time, ValueSignal);
        }
        //Определен срез
        public override bool HasBegin(DateTime time)
        {
            return SignalsHasBegin(time, ValueSignal);
        }
    }
}