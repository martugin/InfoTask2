namespace CommonTypes
{
    //Тип провайдера
    public enum ProviderType
    {
        Source,
        Receiver,
        HandInput,
        Archive,
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