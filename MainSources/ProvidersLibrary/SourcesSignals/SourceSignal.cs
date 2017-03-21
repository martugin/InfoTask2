using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для всех сигналов источников
    public abstract class SourceSignal : ProviderSignal
    {
        protected SourceSignal(SourceConnect connect, string code, DataType dataType, string infObject, string infOut, string infProp)
            : base(code, dataType, infObject, infOut, infProp)
        {
            Connect = connect;
            MList = MFactory.NewList(dataType);
        }
        protected SourceSignal(SourceConnect connect, string code) 
            : base(code)
        {
            Connect = connect;
        }

        //Соединение
        protected internal SourceConnect Connect { get; private set; }

        //Возвращаемый список значений
        protected MomList MList { get; set; }
        public IMean MomList { get { return MList; } }

        //Очистка списка значений
        internal virtual void ClearMoments(bool clearBegin)
        {
            MList.Clear();
        }
    }
}