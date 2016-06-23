namespace ProvidersLibrary
{
    //Тип провайдера
    public enum ProviderType
    {
        Communicator,
        CommReceiver,
        Source,
        Archive,
        Receiver,
        Imitator,
        Error
    }

    //----------------------------------------------------------------------------------------------------------
    //Откуда и куда грузятся значения при чтении из источника
    public enum ValuesDirection
    {
        SourceToMemory, //Чтение из источника в мгновенные значения
        CloneToMemory, //Чтение из клона в мгновенные значения
        SourceToClone //Чтение из источника в клон
    }
}