using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для всех сигналов источников
    public abstract class SourceSignal : ProviderSignal
    {
        protected SourceSignal(SourceBase source, string code, DataType dataType, string signalInf)
            : base(code, dataType, signalInf)
        {
            Source = source;
            MList = MFactory.NewList(dataType);
            MomList = new MomListReadOnly(MList);
        }
        protected SourceSignal(SourceBase conn, string code, DataType dataType)
            : base(code, dataType)
        {
            Source = conn;
            MList = MFactory.NewList(dataType);
            MomList = new MomListReadOnly(MList);
        }

        //Соединение - источник
        protected SourceBase Source { get; private set; }

        //Возвращаемый список значений
        internal protected MomList MList { get; protected set; }
        public IMomListReadOnly MomList { get; private set; }

        //Очистка списка значений
        internal virtual void ClearMoments(bool clearBegin)
        {
            MList.Clear();
        }
    }
}