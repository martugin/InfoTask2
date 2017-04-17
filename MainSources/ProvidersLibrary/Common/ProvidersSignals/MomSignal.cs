using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Сигнал с мгновенным значением
    public class MomSignal : ProviderSignal
    {
        public MomSignal(ProviderConnect connect, string code, DataType dataType, string contextOut, DicS<string> inf) 
            : base(connect, code, dataType, contextOut, inf)
        {
            Value = MFactory.NewMom(dataType);
        }

        public override SignalValueType ValueType
        {
            get { return SignalValueType.Mom; }
        }

        //Значение само в роли буферного
        internal override IMean BufMom
        {
            get { return Value; }
        }

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