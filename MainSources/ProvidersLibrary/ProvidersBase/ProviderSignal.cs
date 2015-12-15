using BaseLibrary;

namespace CommonTypes
{
    public class ProviderSignal : IContextable
    {
        public ProviderSignal(string signalInf, string code, DataType dataType, IProvider provider)
        {
            Code = code;
            DataType = dataType;
            Inf = signalInf.ToPropertyDicS();
            Provider = provider;
        }

        //Код
        public string Code { get; private set; }
        //Адрес для формирования ошибок мгновенных значений
        public virtual string Context { get { return "{" + Code + "}" ; } }
        //Тип данных 
        private DataType _dataType;
        public DataType DataType 
        { 
            get { return _dataType; }
            protected set 
            { 
                _dataType = value;
                IsReal = DataType.LessOrEquals(DataType.Real);
            } 
        }
        //Тип данных можно записать как число
        protected bool IsReal;
        //Словарь свойств
        public DicS<string> Inf { get; private set; }
        //Ссылка на провайдер
        public IProvider Provider { get; private set; }
    }

    //----------------------------------------------------------------------------------------
    //Сигнал приемника
    public class ReceiverSignal : ProviderSignal
    {
        public ReceiverSignal(string signalInf, string code, DataType dataType, IProvider provider) 
            :base(signalInf, code, dataType, provider)
        {
        }

        //Передаваемое значение
        public IMomentsVal Value { get; set; }
    }
}