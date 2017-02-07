using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для всех сигналов источников
    public abstract class SourceSignal : ProviderSignal
    {
        protected SourceSignal(SourceConnect connect, string code, string codeOut, DataType dataType, string signalInf)
            : base(code, codeOut, dataType, signalInf)
        {
            Connect = connect;
            MList = MFactory.NewList(dataType);
        }
        protected SourceSignal(SourceConnect connect, string code, string codeOut) 
            : base(code, codeOut)
        {
            Connect = connect;
        }

        //Соединение
        internal protected SourceConnect Connect { get; private set; }

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