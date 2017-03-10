using System.Data;
using System.Data.OleDb;

namespace ProvidersLibrary
{
    //Источник с подключением через OleDb
    public abstract class OleDbSource : AdoSource
    {
        //Соединение OleDb
        protected OleDbConnection Connection { get; private set; }

        //Открытие соединения
        protected override void ConnectProvider()
        {
            AddEvent("Открытие соединения с провайдером", Hash);
            Connection = new OleDbConnection(ConnectionString);
            Connection.Open();
            if (Connection.State != ConnectionState.Open)
                AddError("Ошибка при открытии соедиения");
        }
        
        //Открытие соединения
        protected override void DisconnectProvider()
        {
            Connection.Close(); 
            Connection.Dispose();
            Connection = null;
        }

        //Строка OleDb-соединения с провайдером
        protected abstract string ConnectionString { get; }
    }
}