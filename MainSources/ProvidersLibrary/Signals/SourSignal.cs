using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для всех сигналов источников
    public abstract class SourSignal : ProvSignal
    {
        protected SourSignal(SourConn conn, string code, DataType dataType, string signalInf)
            : base(code, dataType, signalInf)
        {
            SourceConn = conn;
            MList = MFactory.NewList(dataType);
            MomList = new MomListReadOnly(MList);
        }
        protected SourSignal(SourConn conn, string code, DataType dataType)
            : base(code, dataType)
        {
            SourceConn = conn;
            MList = MFactory.NewList(dataType);
            MomList = new MomListReadOnly(MList);
        }

        //Соединение - источник
        protected SourConn SourceConn { get; private set; }
        //Объект
        internal SourObject SourObject { get; set; }

        //Возвращаемый список значений
        protected MomList MList { get; private set; }
        public IMomListReadOnly MomList { get; private set; }
    }
}