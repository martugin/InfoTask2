using System;
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
        protected override bool ConnectProvider()
        {
            AddEvent("Открытие соединения с провайдером", Hash);
            Connection = new OleDbConnection(ConnectionString);
            Connection.Open();
            return Connection.State == ConnectionState.Open;
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