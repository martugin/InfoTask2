namespace ProvidersLibrary
{
    //Базовый класс для провайдеров приемников 
    public abstract class Receiv : Prov
    {
        //Ссылка на соединение
        internal ReceivConn ReceivConn { get { return (ReceivConn)Conn; } }
    }
}