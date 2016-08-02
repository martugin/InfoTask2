using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;
using OPCAutomation;

namespace ProvidersLibrary
{
    //Соединение с OPC-сервером
    public abstract class OpcServer : ReceiverBase
    {
        internal protected OpcServer() 
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
        protected override bool Connect()
        {
            try
            {
                if (Node.IsEmpty()) Server.Connect(ServerName);
                else Server.Connect(ServerName, Node);
                if (State != 1)//Для Овации этой проверки не было !!!!!
                {
                    AddError("Ошибка соединения с OPC-сервером", null, "Состояние = " + State);
                    return false;
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка соединения с OPC-сервером", ex);
                return false;
            }
            return true;
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
        protected override void Disconnect()
        {
            Server.OPCGroups.RemoveAll();
            Server.Disconnect();
        }

        //Массив корректно добавленых OPC-итемов
        private OpcItem[] _itemsList;


        //Ссылка на группу
        private OPCGroup _group;
        //Словарь OPC-итемов, ключи - коды тегов
        private readonly Dictionary<string, OpcItem> _items = new Dictionary<string, OpcItem>();

        //Список доступных OPC-серверов (не работает, а работает на VisualBasic)
        public List<string> ServersList(string node = null)
        {
            var list = new List<string>();
            Array arr = node == null ? Server.GetOPCServers() : Server.GetOPCServers(node);
            foreach (string a in arr)
                list.Add(a);
            return list;
        }

        //Создается группа с именем name
        public void AddGroup(string name)
        {
            if (Server.OPCGroups.Count > 0)
            {
                Server.OPCGroups.RemoveAll();
                _items.Clear();   
            }
            _group = Server.OPCGroups.Add(name);
           // _group.IsSubscribed = true;
            _group.UpdateRate = 1000;
            _group.IsActive = true;
            _group.DeadBand = 0;
        }

        //Добавить сигнал приемника
        public ReceiverSignal AddSignal(string signalInf, string code, string codeObject, DataType dataType)
        {
            if (ProviderSignals.ContainsKey(code)) return ProviderSignals[code];
            var item = new OpcItem(this, code, codeObject, dataType, signalInf);
            ProviderSignals.Add(code, item);
            item.Tag = GetOpcItemTag(item.Inf);
            item.ClientHandler = ProviderSignals.Count;
            if (!_items.ContainsKey(item.Tag))
                _items.Add(item.Tag, item);
            return item;
        }

        //Получение Tag точки по сигналу
        protected abstract string GetOpcItemTag(DicS<string> inf);

        //Добавить сигнал приемника, задав только тэг (для тестовой записи в настройках), сразу же передается значение как строка
        public ReceiverSignal AddSignalByTag(string tag, DataType dataType, string v)
        {
            if (ProviderSignals.ContainsKey(tag)) return ProviderSignals[tag];
            var item = new OpcItem(this, tag, tag, dataType, "");
            ProviderSignals.Add(tag, item);
            item.Tag = tag;
            item.ClientHandler = ProviderSignals.Count;
            item.Value = MFactory.NewMean(dataType, v); 
            if (!_items.ContainsKey(item.Tag))
                _items.Add(item.Tag, item);
            return item;
        }

        //Добавление итемов на сервер
        private bool TryPrepare()
        {
            if (!ConnectProvider(false)) return false;
            try
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
                        if (((int)errorsArr.GetValue(j)) == 0) m++;
                    _itemsList = new OpcItem[m];
                    int k = 1;
                    for (int j = 1; j <= n; j++)
                        if (((int)errorsArr.GetValue(j)) == 0)
                        {
                            _itemsList[k] = list[j];
                            _itemsList[k++].ServerHandler = (int)serverH.GetValue(j);
                        }
                }
                return true;
            }
            catch (Exception ex)
            {
                AddError("Ошибка подготовки серверной группы", ex);
                return false;
            }
        }

        public override void Prepare()
        {
            if (!Logger.IsRepeat) TryPrepare();
            else Danger(TryPrepare, 2, 1000, "Ошибка подготовки OPC-сервера");
        }

        //Запись значений
        private bool TryWriteValues()
        {
            try
            {
                if (_items.Count > 0)
                {
                    int m = _itemsList == null ? 0 : _itemsList.Length;//Количество корректно добавленых точек + 1
                    Array serverHandles = new int[m];
                    Array valuesArr = new object[m];
                    Array errorsArr = new object[m];
                    for (int i = 1; i < m; i++)
                    {
                        var item = _itemsList[i];
                        serverHandles.SetValue(item.ServerHandler, i);
                        valuesArr.SetValue(item.Value.Object, i);
                    }
                    _group.SyncWrite(m - 1, ref serverHandles, ref valuesArr, out errorsArr);
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка записи значений в OPC-сервер", ex);
                return false;
            }
            return true;
        }
        public void WriteValues()
        {
            if (!Logger.IsRepeat) Danger(TryWriteValues, 2, 500, "Ошибка записи значений в OPC-сервер");
            else Danger(() => Danger(TryWriteValues, 2, 500, "Ошибка записи значений в OPC-сервер"), 3, 10000, "Ошибка подготовки OPC-сервера", TryPrepare);
        }
    }
}
