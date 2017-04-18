namespace ProvidersLibrary
{
    //Выход мгновенного источника
    public class MomSourceOut: SourceOut
    {
        protected MomSourceOut(MomSource source) 
            : base(source) { }

        //Основной сигнал объекта
        protected internal MomSignal ValueSignal { get; protected set; }

        //Добавить сигнал в выход
        protected override ProviderSignal AddNewSignal(ProviderSignal sig)
        {
            return AddSourceSignal((MomSignal)sig);
        }
        protected virtual MomSignal AddSourceSignal(MomSignal sig)
        {
            return ValueSignal = ValueSignal ?? sig;
        }
    }
}