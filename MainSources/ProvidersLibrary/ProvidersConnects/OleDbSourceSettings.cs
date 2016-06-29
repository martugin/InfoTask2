using System;
using System.Data;
using System.Data.OleDb;

namespace ProvidersLibrary
{
    public abstract class OleDbSourceSettings : SourceSettings
    {
        //Соединение с провайдером OleDb
        public OleDbConnection Connection { get; set; }

        //Открытие соединения
        public override bool Connect()
        {
            try { Connection.Close(); } catch { }
            try
            {
                AddEvent("Соединение с провайдером", Hash);
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

        //Очистка ресурсов
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