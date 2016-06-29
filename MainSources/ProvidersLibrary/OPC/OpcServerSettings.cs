using System;
using System.Threading;
using BaseLibrary;
using OPCAutomation;

namespace ProvidersLibrary
{
    //Подключение приемника к OPC-серверу
    public class OpcServerSettings : ProviderSettings
    {
        internal protected OpcServerSettings() 
        {
            Server = new OPCServer();
        }
        
        //Допускается передача списка мгновенных значений за один раз
        public bool AllowListValues { get { return false; } }

        //Настройки
        protected override void ReadInf(DicS<string> dic)
        {
            ServerName = dic["OPCServerName"];
            Node = dic["Node"];
        }
        //Хэш для идентификации настройки провайдера
        public override string Hash
        {
            get { return "OPCServer=" + ServerName + ";Node=" + Node; }
        }

        //Тип OPC-сервера
        public string ServerName { get; set; }
        //Имя компьютера
        public  string Node { get; set; }

        //OPC сервер
        internal OPCServer Server { get; private set; }
        //Состояние сервера
        public int State { get { return Server.ServerState; } }

        //Проверка соединения
        public override bool Connect()
        {
            try
            {
                IsConnected = false;
                Dispose();
                Thread.Sleep(500);
            }
            catch { }
            try
            {
                if (Node.IsEmpty()) Server.Connect(ServerName);
                else Server.Connect(ServerName, Node);
                if (State != 1)//Для Овации этой проверки не было !!!!!
                {
                    AddError("Ошибка соединения с OPC-сервером", null, "Состояние = " + State);
                    return IsConnected = false;
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка соединения с OPC-сервером", ex);
                return IsConnected = false;
            }
            return IsConnected = true;
        }

        //Проверка настроек
        public override string CheckSettings(DicS<string> inf)
        {
            return !inf["OPCServerName"].IsEmpty() ? "" : "Не задано имя OPC-сервера";
        }

        //Проверка соединения
        public override bool CheckConnection()
        {
            if (Connect())
            {
                CheckConnectionMessage = "Успешное соединение";
                return true;
            }
            AddError(CheckConnectionMessage = "Ошибка соединения с OPC-сервером");
            return false;
        }

        //Разрыв соединения
        public override void Dispose()
        {
            try
            {
                Server.OPCGroups.RemoveAll();
                Server.Disconnect();
                IsConnected = false;
            }
            catch { }
        }
    }
}