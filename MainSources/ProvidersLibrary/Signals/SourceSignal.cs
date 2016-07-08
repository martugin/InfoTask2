using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для всех сигналов источников
    public abstract class SourceSignal : ProviderSignal
    {
        protected SourceSignal(SourceBase source, DataType dataType, string signalInf)
            : base(dataType, signalInf)
        {
            Source = source;
            MList = MFactory.NewList(dataType);
            MomList = new MomListReadOnly(MList);
        }
        protected SourceSignal(SourceBase conn)
        {
            Source = conn;
        }

        //Соединение - источник
        internal protected SourceBase Source { get; private set; }

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