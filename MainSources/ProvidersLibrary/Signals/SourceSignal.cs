using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Сигнал источника
    public abstract class SourceSignal : ProviderSignal
    {
        //Значение или список значений
        protected SourceSignal(SourceConnect connect, string code, DataType dataType, string contextOut, DicS<string> inf) 
            : base(connect, code, dataType, contextOut, inf) { }

        protected SourceSignal(SourceConnect connect, string code) 
            : base(connect, code) { }

        //Соединение с источником
        public SourceConnect SourceConnect
        {
            get { return (SourceConnect)Connect; }
        }

        //Значение
        public abstract IMean Value { get; }
        //Мгновенное значение сигнала или буферное значение
        internal abstract IMean BufMom { get; }

        //Добавка мгновенного значения (BufMom) в список или клон
        //Возвращает количество реально добавленных значений 
        internal abstract int AddMom(DateTime time, MomErr err);
    }
}