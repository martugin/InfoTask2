using System;
using System.Data;
using System.Data.OleDb;

namespace ProvidersLibrary
{
    public abstract class OleDbSourceConnect : SourceConnect
    {
        //Соединение с провайдером OleDb
        public OleDbConnection Connection { get; set; }

        //Открытие соединения
        public override bool Check()
        {
            try { Connection.Close(); } catch { }
            try
            {
                AddEvent("Соединение с провайдером", Inf);
                Connection = new OleDbConnection(ConnectionString);
                Connection.Open();
                return IsConnected = Connection.State == ConnectionState.Open;
            }
            catch (Exception ex)
            {
                AddError("Ошибка соединения с провайдером", ex);
                return IsConnected = false;
            }
        }

        //Строка OleDb-соединения с провайдером
        protected abstract string ConnectionString { get; }

        public override void Dispose()
        {
            try
            {
                Connection.Close();
                Connection.Dispose();
            }
            catch { }
        }
    }
}