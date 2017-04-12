namespace ProvidersLibrary
{
    //Базовый класс для всех мгновенных источников
    public abstract class MomSource : Source
    {
        //Ссылка на соединение
        internal MomSourceConnect SourceConnect
        {
            get { return (MomSourceConnect)ProviderConnect; }
        }
    }
}