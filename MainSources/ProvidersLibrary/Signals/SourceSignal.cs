using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для всех сигналов источников
    public abstract class SourceSignal : ProviderSignal
    {
        protected SourceSignal(Source source, string code, DataType dataType, string signalInf)
            : base(code, dataType, signalInf)
        {
            Source = source;
            MList = MFactory.NewList(dataType);
            MomList = new MomListReadOnly(MList);
        }
        protected SourceSignal(Source conn, string code, DataType dataType)
            : base(code, dataType)
        {
            Source = conn;
            MList = MFactory.NewList(dataType);
            MomList = new MomListReadOnly(MList);
        }

        //Соединение - источник
        protected Source Source { get; private set; }
        //Объект
        internal SourceObject SourceObject { get; set; }

        //Возвращаемый список значений
        protected MomList MList { get; private set; }
        public IMomListReadOnly MomList { get; private set; }
    }
}