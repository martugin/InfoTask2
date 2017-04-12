namespace ProvidersLibrary
{
    public class MomSourceOut: ProviderOut
    {
        protected MomSourceOut(MomSource source) 
            : base(source) { }

        //Ссылка на источник
        protected MomSource Source { get { return (MomSource) Provider; } }

        //Основной сигнал объекта
        protected internal ProviderSignal ValueSignal { get; set; }

        //Добавить сигнал в выход
        protected override ProviderSignal AddNewSignal(ProviderSignal sig)
        {
            return ValueSignal = ValueSignal ?? sig;
        }
    }
}