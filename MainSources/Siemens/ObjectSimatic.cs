using System;
using CommonTypes;

namespace Provider
{
    //Объект
    internal class ObjectSimatic : SourceObject
    {
        internal ObjectSimatic(SignalSimatic signal) : base(signal.Inf["Tag"])
        {
            Id = signal.Id;
            Archive = signal.Inf["Archive"];
            FullCode = Archive + @"\" + Code;
            AddSignal(signal);
        }

        //Добавление сигнала
        internal SignalSimatic AddSignal(SignalSimatic signal)
        {
            switch (signal.Inf.Get("Prop", "").ToLower())
            {
                case "quality": 
                    return SignalQuality = SignalQuality ?? signal;
                case "flags": 
                    return SignalFlags = SignalFlags ?? signal; 
                default:
                    return SignalValue = SignalValue ?? signal; 
            }
        }

        //Сигналы: значение, качество, флаги
        internal SignalSimatic SignalValue { get; private set; }
        internal SignalSimatic SignalQuality { get; private set; }
        internal SignalSimatic SignalFlags { get; private set; }
        
        //Имя архива
        internal string Archive { get; private set; }
        //Полное имя архивного тэга 
        internal string FullCode { get; private set; }
        //Id в таблице архива
        public int Id { get; private set; }

        //Возвращает, есть ли у объекта неопределенные срезы на время time 
        public override bool HasBegin(DateTime time)
        {
            return SignalsHasBegin(time, SignalValue, SignalQuality, SignalFlags);
        }

        //Добавляет в сигналы объекта срез, если возможно, возвращает, сколько добавлено значений
        public override int AddBegin(DateTime time)
        {
            return SignalsAddBegin(time, SignalValue, SignalQuality, SignalFlags);
        }
    }
}