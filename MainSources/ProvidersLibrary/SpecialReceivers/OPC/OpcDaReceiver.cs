﻿using System;
using System.Collections.Generic;
using System.Linq;
using BaseLibrary;
using OPCAutomation;

namespace ProvidersLibrary
{
    //Соединение с OPC-сервером DA
    public abstract class OpcDaReceiver : Receiver
    {
        protected internal OpcDaReceiver() 
        {
            Server = new OPCServer();
        }

        //Тип сигналов
        public override SignalType SignalType
        {
            get { return SignalType.Receiver;}
        }

        //Настройки
        protected override void ReadInf(DicS<string> dic)
        {
            ServerName = dic["OPCServerName"];
            Node = dic["Node"];
            ReadAdditionalInf(dic);
        }
        //Чтение дополнительных настроек
        protected virtual void ReadAdditionalInf(DicS<string> dic) { }

        //Тип OPC-сервера
        public string ServerName { get; set; }
        //Имя компьютера
        public  string Node { get; set; }

        //OPC сервер
        internal OPCServer Server { get; private set; }
        //Состояние сервера
        protected int State { get { return Server.ServerState; } }

        //Проверка соединения
        protected override void ConnectProvider()
        {
            if (Node.IsEmpty()) Server.Connect(ServerName);
            else Server.Connect(ServerName, Node);
            if (State != 1) //Для Овации этой проверки не было !!!!!
                AddError("Ошибка соединения с OPC-сервером", null, "Состояние = " + State);
        }

        //Разрыв соединения
        protected override void DisconnectProvider()
        {
            Server.OPCGroups.RemoveAll();
            Server.Disconnect();
        }

        //Проверка настроек
        protected internal override string CheckSettings(DicS<string> inf)
        {
            return !inf["OPCServerName"].IsEmpty() ? "" : "Не задано имя OPC-сервера";
        }

        //Список доступных OPC-серверов (не работает, а работает на VisualBasic)
        public List<string> ServersList(string node = null)
        {
            Array arr = node == null ? Server.GetOPCServers() : Server.GetOPCServers(node);
            return arr.Cast<string>().ToList();
        }

        //Словарь OPC-итемов, ключи - коды тегов
        private readonly Dictionary<string, OpcItem> _items = new Dictionary<string, OpcItem>();
        
        //Массив корректно добавленых OPC-итемов
        private OpcItem[] _itemsList;
        //Ссылка на группу
        private OPCGroup _group;

        //Очистка списков итемов
        protected internal override void ClearOuts()
        {
            _items.Clear();
            _group = null;
            _itemsList = null;
        }

        //Добавить итем содержащий заданный сигнал
        protected override ProviderOut AddOut(ProviderSignal sig)
        {
            var tag = GetOpcItemTag(sig.Inf);
            if (_items.ContainsKey(tag))
                return _items[tag];
            var item = new OpcItem(this, tag, _items.Count);
            _items.Add(item.Tag, item);
            return item;
        }

        //Получение Tag точки из информации по сигналу
        protected abstract string GetOpcItemTag(DicS<string> inf);

        //Создается группа
        private void AddGroup(string name) //имя группы
        {
            if (Server.OPCGroups.Count > 0)
                Server.OPCGroups.RemoveAll();
            _group = Server.OPCGroups.Add(name);
           // _group.IsSubscribed = true;
            _group.UpdateRate = 1000;
            _group.IsActive = true;
            _group.DeadBand = 0;
        }

        //Добавление итемов на сервер
        protected override void PrepareProvider()
        {
            if (_group == null) AddGroup("Gr" + (Server.OPCGroups.Count + 1));
            int n = _items.Count;
            var list = new OpcItem[n + 1];
            int i = 1;
            foreach (var item in _items.Values)
                list[i++] = item;
            if (n == 0) _itemsList = null;
            else
            {
                Array clientH = Array.CreateInstance(typeof(Int32), n + 1);
                var itemArr = Array.CreateInstance(typeof(string), n + 1);
                Array errorsArr = new object[n + 1];
                Array serverH = new object[n + 1];
                for (int j = 1; j <= n; j++)
                {
                    var item = list[j];
                    itemArr.SetValue(item.Tag, j);
                    clientH.SetValue(j, j);
                }
                _group.OPCItems.AddItems(n, ref itemArr, ref clientH, out serverH, out errorsArr);
                int m = 1;
                for (int j = 1; j <= n; j++)
                    if ((int)errorsArr.GetValue(j) == 0) m++;
                _itemsList = new OpcItem[m];
                int k = 1;
                for (int j = 1; j <= n; j++)
                    if ((int)errorsArr.GetValue(j) == 0)
                    {
                        _itemsList[k] = list[j];
                        _itemsList[k++].ServerHandler = (int)serverH.GetValue(j);
                    }
            }
        }

        //Запись значений
        protected internal override void WriteValues()
        {
            if (_items.Count > 0)
                StartDanger(2, LoggerStability.Single, "Запись в OPC-сервер", true, 500)
                    .Run(() =>
                        {
                            int m = _itemsList == null ? 0 : _itemsList.Length;
                                //Количество корректно добавленых точек + 1
                            Array serverHandles = new int[m];
                            Array valuesArr = new object[m];
                            Array errorsArr = new object[m];
                            for (int i = 1; i < m; i++)
                            {
                                var item = _itemsList[i];
                                serverHandles.SetValue(item.ServerHandler, i);
                                valuesArr.SetValue(item.ValueSignal.InValue.Object, i);
                            }
                            _group.SyncWrite(m - 1, ref serverHandles, ref valuesArr, out errorsArr);
                        });
        }
    }
}
