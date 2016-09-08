using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для всех сигналов источников
    public abstract class SourceSignal : ProviderSignal
    {
        protected SourceSignal(SourceConnect connect, string code, string codeObject, DataType dataType, string signalInf)
            : base(code, codeObject, dataType, signalInf)
        {
            Connect = connect;
            MList = MFactory.NewList(dataType);
            MomList = new MomListRead(MList);
        }
        protected SourceSignal(SourceConnect connect, string code, string codeObject) 
            : base(code, codeObject)
        {
            Connect = connect;
        }

        //Соединение
        internal protected SourceConnect Connect { get; private set; }

        //Возвращаемый список значений
        protected MomList MList { get; set; }
        public IMomListRead MomList { get; protected set; }

        //Очистка списка значений
        internal virtual void ClearMoments(bool clearBegin)
        {
            MList.Clear();
        }
    }
}