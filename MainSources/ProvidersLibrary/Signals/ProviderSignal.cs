using BaseLibrary;

namespace CommonTypes
{
    public class ProviderSignal : IContextable
    {
        public ProviderSignal(string signalInf, string code, DataType dataType)
        {
            Code = code;
            DataType = dataType;
            Inf = signalInf.ToPropertyDicS();
        }

        //Полный код сигнала
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
        protected bool IsReal; //Todo убрать
        //Словарь свойств
        public DicS<string> Inf { get; private set; }
    }
}