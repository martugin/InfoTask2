using System;
using System.Collections.Generic;
using System.Threading;
using BaseLibrary;
using OPCAutomation;

namespace CommonTypes
{
    //Соединение с OPC-сервером
    public abstract class OpcServer : ProviderBase, IReceiver
    {
        protected OpcServer() 
        {
            _server = new OPCServer();
        }
        protected OpcServer(string name, string inf, Logger logger) : base(name, logger)
        {
            _server = new OPCServer();
            Inf = inf;
        }

        //Тип
        public override ProviderType Type { get { return ProviderType.Receiver; } }
        //Допускается передача списка мгновенных значений за один раз
        public bool AllowListValues { get { return false; } }

        //Настройки
        protected override void GetInfDicS(DicS<string> dic)
        {
            ServerName = dic["OPCServerName"];
            Node = dic["Node"];
            Hash = "OPCServer=" + ServerName + ";Node=" + Node;
            GetAdditionalInf(dic);
        }

        //Загрузка дополнительных настроек провайдера из Inf
        protected virtual void GetAdditionalInf(DicS<string> inf) { }

        //Тип OPC-сервера
        public string ServerName { get; set; }
        //Имя компьютера
        public  string Node { get; set; }
        //Соединение установлено и не закрыто
        public bool IsConnected { get; protected set; }

        //OPC сервер
        private readonly OPCServer _server;
        //Состояние сервера
        public int State { get { return _server.ServerState; } }

        //Ссылка на группу
        private OPCGroup _group;
        //Словарь OPC-итемов, ключи - коды тегов
        private readonly Dictionary<string, OpcItem> _items = new Dictionary<string, OpcItem>();
        //Массив корректно добавленых OPC-итемов
        private OpcItem[] _itemsList;
        //Словарь сигналов приемников, ключи - коды в нижнем регистре
        private readonly Dictionary<string, OpcItem> _signalsCode = new Dictionary<string, OpcItem>();
        public Dictionary<string, OpcItem> SignalsCode { get { return _signalsCode; }}
        //Список сигналов приемника с мгновенными значениями
        private readonly DicS<ReceiverSignal> _signals = new DicS<ReceiverSignal>();
        public IDicSForRead<ReceiverSignal> Signals { get { return _signals; } }
        
        //Список доступных OPC-серверов (не работает, а работает на VisualBasic)
        public List<string> ServersList(string node = null)
        {
            var list = new List<string>();
            Array arr = node == null ? _server.GetOPCServers() : _server.GetOPCServers(node);
            foreach (string a in arr)
                list.Add(a);
            return list;
        }

        //Соединение
        public bool Connect()
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
                if (Node.IsEmpty()) _server.Connect(ServerName);
                else _server.Connect(ServerName, Node);
                if (State != 1)//Для Овации этой проверки не было !!!!!
                {
                    AddError("Ошибка соединения с OPC-сервером", null , "Состояние = " + State);
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

        //Проверка соединения
        public bool Check()
        {
            return Danger(Connect, 2, 500, "Ошибка соединения с OPC-сервером");
        }

        //Проверка настроек
        public string CheckSettings(Dictionary<string, string> inf, Dictionary<string, string> names)
        {
            return !inf["OPCServerName"].IsEmpty() ? "" : "Не задано имя OPC-сервера";
        }

        //Проверка соединения
        public bool CheckConnection()
        {
            if (Check())
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
                _server.OPCGroups.RemoveAll();
                _server.Disconnect();
                IsConnected = false;
            }
            catch { }
        }

        //Создается группа с именем name
        public void AddGroup(string name)
        {
            if (_server.OPCGroups.Count > 0)
            {
                _server.OPCGroups.RemoveAll();
                _items.Clear();   
            }
            _group = _server.OPCGroups.Add(name);
           // _group.IsSubscribed = true;
            _group.UpdateRate = 1000;
            _group.IsActive = true;
            _group.DeadBand = 0;
        }

        //Добавить сигнал приемника
        public ReceiverSignal AddSignal(string signalInf, string code, DataType dataType)
        {
            if (_signalsCode.ContainsKey(code)) return _signalsCode[code];
            var item = new OpcItem(signalInf, code, dataType, this);
            _signals.Add(item.Code, item);
            _signalsCode.Add(code, item);
            item.Tag = GetOpcItemTag(item.Inf);
            item.ClientHandler = _signals.Count;
            if (!_items.ContainsKey(item.Tag))
                _items.Add(item.Tag, item);
            return item;
        }

        //Получение Tag точки по сигналу
        protected abstract string GetOpcItemTag(DicS<string> inf);

        //Добавить сигнал приемника, задав только тэг (для тестовой записи в настройках), сразу же передается значение как строка
        public ReceiverSignal AddSignalByTag(string tag, DataType dataType, string v)
        {
            if (_signalsCode.ContainsKey(tag)) return _signalsCode[tag];
            var item = new OpcItem("", tag, dataType, this);
            _signals.Add(item.Code, item);
            _signalsCode.Add(tag, item);
            item.Tag = tag;
            item.ClientHandler = _signals.Count;
            item.Value = MFactory.NewMean(dataType, v); 
            if (!_items.ContainsKey(item.Tag))
                _items.Add(item.Tag, item);
            return item;
        }

        //Добавление итемов на сервер
        private bool TryPrepare()
        {
            if (!IsConnected && !Connect()) return false;
            try
            {
                if (_group == null) AddGroup("Gr" + (_server.OPCGroups.Count + 1));
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
                return IsConnected = false;
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
            if (!IsConnected) return false;
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
                return IsConnected = false;
            }
            return true;
        }
        public void WriteValues()
        {
            if (!Logger.IsRepeat) Danger(TryWriteValues, 2, 500, "Ошибка записи значений в OPC-сервер");
            else Danger(() => Danger(TryWriteValues, 2, 500, "Ошибка записи значений в OPC-сервер"), 3, 10000, "Ошибка подготовки OPC-сервера", TryPrepare);
        }
    }

    //--------------------------------------------------------------------------------------
    //Отладочный OPC-сервер
    public class DebugOpcServer : OpcServer
    {
        public DebugOpcServer(string serverName, string node)
        {
            Name = "Check";
            Inf = "OpcServerName=" + serverName + ";Node=" + (node ?? "");
            Logger = new Logger();
        }

        public override string Code { get { return "DebugOpcServer"; } }
        protected override string GetOpcItemTag(DicS<string> inf) { return ""; }
    }
}
