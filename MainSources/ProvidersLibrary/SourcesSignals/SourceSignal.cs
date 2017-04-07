using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для всех сигналов источников
    public abstract class SourceSignal : ProviderSignal, ISourceSignal
    {
        protected SourceSignal(SourceConnect connect, string code, DataType dataType, string infObject, string infOut, string infProp)
            : base(code, dataType, infObject, infOut, infProp)
        {
            Connect = connect;
            MomList = MFactory.NewList(dataType);
        }
        protected SourceSignal(SourceConnect connect, string code) 
            : base(code)
        {
            Connect = connect;
        }

        //Соединение
        protected internal SourceConnect Connect { get; private set; }

        //Возвращаемый список значений
        public IMean Value { get { return MomList; } }
    }
}