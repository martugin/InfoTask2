namespace CommonTypes
{
    //Тип провайдера
    public enum ProviderType
    {
        Source, //Источник
        HandInput, //Ручной ввод
        Receiver, //Приемник
        Archive, //Архив
        Module, //Модуль
        Proxy, //Прокси, хранящий одно значение
        QueuedProxy, //Прокси, хранящий очередь значений
        Error
    }

    //------------------------------------------------------------------------------
    //Тип ошибки
    public enum ErrQuality
    {
        Good = 0,
        Warning = 1,
        Error = 2
    }
}