namespace ProvidersLibrary
{
    //Базовый класс для всех источников чтения из архива
    public abstract class ListSource : Source
    {
        //Чтение среза, возврашает количество прочитанных значений
        protected internal virtual ValuesCount ReadCut() { return new ValuesCount(); }
        //Чтение изменений, возврашает количество прочитанных и сформированных значений
        protected internal abstract ValuesCount ReadChanges();
    }
}