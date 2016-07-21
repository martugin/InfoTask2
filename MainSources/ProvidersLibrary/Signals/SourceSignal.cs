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
        protected SourceSignal(SourceBase source, string code) 
            : base(code)
        {
            Source = source;
        }

        //Соединение - источник
        internal protected SourceBase Source { get; private set; }

        //Возвращаемый список значений
        internal protected MomList MList { get; protected set; }
        public IMomListReadOnly MomList { get; protected set; }

        //Очистка списка значений
        internal virtual void ClearMoments(bool clearBegin)
        {
            MList.Clear();
        }
    }
}