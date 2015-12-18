using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using BaseLibrary;

namespace Provider
{
    internal class SimaticArchive 
    {
        //Одна база данных: основная или дублирующая
        internal SimaticArchive(SimaticSource source, string serverName, bool isReserve)
        {
            _source = source;
            _serverName = serverName;
            _hasArchive = !_serverName.IsEmpty();
            IsReserve = isReserve;
            Hash = !_hasArchive ? "" : ((!isReserve ? "SQLServer=" : "SQLReserveServer=") + _serverName);
            SuccessTime = Different.MinDate;
        }

        //Источник
        private readonly SimaticSource _source;
        //Архив задан
        private readonly bool _hasArchive;
        //Является резервным
        internal bool IsReserve { get; private set; }
        //Имя сервера
        private readonly string _serverName;
        //Хэш
        internal string Hash { get; private set; }

        //Соединение с базой
        private OleDbConnection _connection;
        //Время последнего успешного соединения
        internal DateTime SuccessTime { get; set; }

        //Установить соединение с архивом
        internal OleDbConnection Connnect()
        {
            if (!_hasArchive) return _connection = null;
            try
            {
                var list = SqlDb.SqlDatabasesList(_serverName);
                var dbName = "";
                foreach (var db in list)
                    if (db.StartsWith("CC_") && db.EndsWith("R"))
                        dbName = db;
                if (dbName.IsEmpty()) return null;
                SqlDb.Connect(_serverName, dbName).GetSchema();//Проверка
                var dic = new Dictionary<string, string>
                    {
                        {"Provider", "WinCCOLEDBProvider.1"},
                        {"Catalog", dbName},
                        {"Data Source", _serverName}
                    };
                _connection = new OleDbConnection(dic.ToPropertyString());
                _connection.Open();
                return _connection;
            }
            catch (Exception ex)
            {
                _source.AddWarning("Соединение с сервером не установлено", ex, _serverName);
                return null;
            }
        }

        //Закрытие соединений
        internal void Disconnect()
        {
            if (_connection == null) return;
            try {_connection.Close();} catch {}
            _connection = null;
        }

        //Проверка соединения
        internal bool CheckConnection()
        {
            string s = " с " + (IsReserve ? "резервным" : "основным") + " архивом " + _serverName;
            if (!_hasArchive)
                _source.CheckConnectionMessage += "Не задано соединение" + s;
            else
            {
                try
                {
                    if (Connnect().State == ConnectionState.Open)
                    {
                        _source.CheckConnectionMessage += "Успешное соединение" + s;
                        return true;
                    }
                    _source.CheckConnectionMessage += "Не удалось соединиться" + s;
                }
                catch { _source.CheckConnectionMessage += "Не удалось соединиться" + s; }
            }
            return false;
        }
    }
}