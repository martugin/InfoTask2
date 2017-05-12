namespace CommonTypes
{
    //Тип провайдера
    public enum ProviderType
    {
        Source,
        Receiver,
        HandInput,
        Archive,
        Module,
        Proxy,
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