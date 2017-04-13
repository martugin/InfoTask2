using System;
using System.Collections.Generic;
using System.Threading;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Один провайдер
    public abstract class Provider : ExternalLogger, IDisposable
    {
        //Ссылка на соединение
        public ProviderConnect ProviderConnect { get; set; }
        
        //Код провайдера
        public abstract string Code { get; }
        
        //Контекст для логгера
        public override string Context
        {
            get { return ProviderConnect.Context + ", " + Code; }
        }

        //Загрузка настроек провайдера
        private string _inf;
        internal string Inf
        {
            get { return _inf; }
            set
            {
                Disconnect();
                IsPrepared = false;
                _inf = value;
                var dic = value.ToPropertyDicS();
                dic.DefVal = "";
                ReadInf(dic);
            }
        }
        //Загрузка свойств из словаря
        protected abstract void ReadInf(DicS<string> dic);

        //Открытие подключения, возвращает true, если соединение установлено
        protected virtual void ConnectProvider() { }
        //Закрытие подключения
        protected virtual void DisconnectProvider() { }
        //Подготовка сигналов провайдера
        protected virtual void PrepareProvider() { }
        //Соединение было установлено
        protected internal bool IsConnected { get; set; }
        //Сигналы провайдера подготовлены
        protected internal bool IsPrepared { get; set; }

        //Время в мс, которое нудно ожидать после неудачного соединения
        protected virtual int ConnectErrorWaitingTime { get { return 300; } }

        //Первичное подключение к провайдеру, true - соединение удачное
        protected internal bool Connect()
        {
            if (IsConnected) return true;
            return IsConnected = StartDanger(2, LoggerStability.Single, "Соединение с провайдером", false, ConnectErrorWaitingTime)
                                                            .Run(ConnectProvider, Disconnect).IsSuccess;
        }

        //Отключение от провайдера
        protected internal bool Disconnect()
        {
            try
            {
                if (IsConnected)
                {
                    AddEvent("Закрытие соединения с провадером");
                    Start().Run(DisconnectProvider);
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка закрытия соединения с провайдером", ex);
            }
            IsConnected = false;
            return true;
        }

        //Повторное подключение, true - соединение удачное
        protected internal bool Reconnect()
        {
            if (IsConnected)
            {
                Disconnect();
                Thread.Sleep(ConnectErrorWaitingTime);
            }
            if (Connect() && this is ListSource && Logger.Stability >= LoggerStability.Single)
                return !((ListSource)this).GetTime().IsDefault;
            return false;
        }

        //Подготовка источника
        protected internal bool Prepare(bool connectBefore = true)
        {
            try
            {
                AddEvent("Подготовка выходов");
                ClearOuts();
                foreach (var sig in ProviderConnect.ProviderSignals.Values)
                    if (sig.IsInitial)
                    {
                        var ob = AddOut(sig);
                        ob.Context = sig.ContextOut;
                        ob.AddSignal(sig);
                    }
                Procent = 20;
                if (connectBefore && !Connect()) return false;
                IsPrepared = StartDanger(0, 100, 2, LoggerStability.Periodic, "Подготовка провайдера")
                                                  .Run(PrepareProvider, Reconnect).IsSuccess;
                MakeErrPool();
                return IsPrepared;
            }
            catch (Exception ex)
            {
                AddError("Ошибка при подготовке провайдера", ex);
                return false;
            }
        }

        //Создать пул ошибок
        protected virtual void MakeErrPool() { }

        //Очистка списков объектов
        protected internal abstract void ClearOuts();
        //Добавить объект содержащий заданный сигнал
        protected abstract ProviderOut AddOut(ProviderSignal sig);

        //Очистка ресурсов
        public virtual void Dispose()
        {
            Disconnect();
        }

        //Настройка
        #region
        //Проверка соединения в форме настроек возвращает true, если соединение успешное
        //Проверка соединения
        protected bool CheckConnection()
        {
            if (Reconnect())
            {
                if (!(this is ListSource))
                {
                    CheckConnectionMessage = "Успешное соединение";
                    return true;
                }
                var ti = ((ListSource)this).GetTime();
                if (ti != null && !ti.IsDefault)
                {
                    CheckConnectionMessage = "Успешное соединение" + (ti.Begin == Static.MinDate ? "" : ". Диапазон источника: " + ti.Begin + " - " + ti.End);
                    return true;
                }
            }
            AddError(CheckConnectionMessage = "Ошибка соединения");
            return false;
        }
        //Cтрока для вывода сообщения о последней проверке соединения
        protected internal string CheckConnectionMessage { get; protected set; }

        //Проверка корректности настроек, возвращает строку с ошибками, на входе словарь настроек
        protected internal virtual string CheckSettings(DicS<string> infDic) { return ""; }

        //Словарь комманд открытия дилогов, ключи - имена свойств, вторые ключи - названия пунктов меню
        private readonly DicS<Dictionary<string, IMenuCommand>> _menuCommands = new DicS<Dictionary<string, IMenuCommand>>();
        internal DicS<Dictionary<string, IMenuCommand>> MenuCommands { get { return _menuCommands; } }
        //Задание комманд, вызываемых из меню
        protected virtual void AddMenuCommands() { }
        //Возвращает выпадающий список для поля настройки 
        internal virtual List<string> ComboBoxList(Dictionary<string, string> props, //словарь значение свойств 
                                                                     string propname) //имя свойства для ячейки со списком
        {
            return new List<string>();
        }

        //Возвращет сообщение, если значение свойства prop не является целым, или не лежит в диапазоне от min до max
        //props - словарь свойств, names - словарь имен свойств
        protected string PropIsInt(string prop, Dictionary<string, string> props, Dictionary<string, string> names, int min, int max)
        {
            if (!props.ContainsKey(prop) || !names.ContainsKey(prop) || props[prop].IsEmpty()) return "";
            string err = "Значение свойства '" + names[prop] + "' должно быть целым числом";
            int res;
            if (!int.TryParse(props[prop], out res)) return err + ", а задано равным '" + props[prop] + "'" + Environment.NewLine;
            if (res < min || res > max) return err + " в диапазоне от " + min + " до " + max + Environment.NewLine;
            return "";
        }
        #endregion
    }
}