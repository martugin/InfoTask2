using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для всех сигналов, значение которых это список мгновенных значений
    public class ListSignal : ProviderSignal
    {
        protected ListSignal(ProviderConnect connect, string code, DataType dataType, string infObject, string infOut, string infProp)
            : base(connect, code, dataType, infObject, infOut, infProp)
        {
            MomList = MFactory.NewList(dataType);
        }
        protected ListSignal(ProviderConnect connect, string code) 
            : base(connect,  code) { }
        
        //Список значений
        protected MomList MomList { get; set; }

        //Очистка списка значений
        internal virtual void ClearMoments(bool clearBegin)
        {
            MomList.Clear();
        }

        //Возвращаемый список значений
        public override IMean Value
        {
            get { return MomList; }
            set { MomList = (MomList)value; }
        }
    }
}