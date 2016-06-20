using System;
using System.Data;
using System.Data.OleDb;
using BaseLibrary;

namespace CommonTypes
{
    public abstract class OleDbSourceConnect : SourceConnect
    {
        protected OleDbSourceConnect() { }
        //Соединение с провайдером Historian
        protected OleDbSourceConnect(string name, Logger logger) : base(name, logger) { }

        public OleDbConnection Connection { get; set; }
        
        //True, если соединение прошло успешно, становится False, если произошла ошибка
        internal protected bool IsConnected { get; set; }
        //Открытие соединения
        internal protected virtual bool Connect()
        {
            Dispose();
            try
            {
                AddEvent("Соединение с провайдером", Name + "; " + Code +"; " + Inf);
                Connection = new OleDbConnection(ConnectinString);
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
        protected abstract string ConnectinString { get; }

        public override void Dispose()
        {
            try { Connection.Close(); }
            catch {}
        }
    }
}