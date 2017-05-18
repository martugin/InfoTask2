using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Сигнал источника
    public abstract class SourceSignal : ProviderSignal, IReadSignal
    {
        //Значение или список значений
        protected SourceSignal(SourceConnect connect, string code, DataType dataType, string contextOut, DicS<string> inf) 
            : base(connect, code, dataType, contextOut, inf) { }

        protected SourceSignal(SourceConnect connect, string code) 
            : base(connect, code) { }

        //Значение
        public abstract IReadMean OutValue { get; }
        
        //Мгновенное значение сигнала или буферное значение
        internal abstract Mean BufMom { get; }

        //Добавка мгновенного значения (BufMom) в список или клон
        //Возвращает количество реально добавленных значений 
        internal abstract int AddMom(DateTime time, MomErr err);
    }
}