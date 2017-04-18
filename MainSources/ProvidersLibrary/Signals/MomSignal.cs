using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Сигнал с мгновенным значением
    public class MomSignal : SourceSignal
    {
        public MomSignal(SourceConnect connect, string code, DataType dataType, string contextOut, DicS<string> inf) 
            : base(connect, code, dataType, contextOut, inf)
        {
            _value = MFactory.NewMom(dataType);
        }

        public override SignalType Type
        {
            get { return SignalType.Mom; }
        }

        //Занчение
        private readonly IMean _value;
        public override IMean Value { get { return _value; } }
        //Значение само в роли буферного
        internal override IMean BufMom { get { return _value; }}

        //Добавка мгновенного значения в список или клон
        //Возвращает количество реально добавленных значений 
        internal override int AddMom(DateTime time, MomErr err)
        {
            Value.Time = time;
            Value.Error = err;
            return 1;
        }
    }
}