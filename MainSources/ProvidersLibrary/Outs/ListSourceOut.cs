using BaseLibrary;

namespace ProvidersLibrary
{
    //Выход архивного источника
    public abstract class ListSourceOut : SourceOut
    {
        protected ListSourceOut(ListSource source) 
            : base(source) { }

        //Основной сигнал объекта
        protected internal ListSignal ValueSignal { get; protected set; }

        //Добавить сигнал в выход
        protected override ProviderSignal AddNewSignal(ProviderSignal sig)
        {
            return AddSourceSignal((ListSignal)sig);
        }
        protected virtual ListSignal AddSourceSignal(ListSignal sig)
        {
            return ValueSignal = ValueSignal ?? sig;
        }

        //Для объекта опредлено значение среза
        internal bool HasBegin
        {
            get
            {
                bool res = true;
                foreach (ListSignal sig in Signals)
                    res &= sig.HasBegin;
                return res;
            }
        }
      
        //Добавление мгновенных значений во все сигналы объекта, используется только если источник - наследник AdoSource
        //Возвращает количество добавленных значений
        protected internal virtual int ReadMoments(IRecordRead rec) //Рекордсет, из которого читаются значения
        {
            return 0;
        }
    }
}