using System;
using System.Collections.Generic;
using System.Threading;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Один провайдер
    public abstract class ProviderBase : ExternalLogger, IDisposable
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
        internal protected string Inf
        {
            set
            {
                var dic = value.ToPropertyDicS();
                dic.DefVal = "";
                ReadInf(dic);
            }
        }
        //Загрузка свойств из словаря
        protected abstract void ReadInf(DicS<string> dic);
        //Хэш для идентификации настройки провайдера
        protected abstract string Hash { get; }

        //Подготовка провайдера к работе
        internal protected abstract void Prepare();
        
        //Открытие подключения, возвращает true, если соединение установлено
        protected virtual bool ConnectProvider() { return true; }
        //Закрытие подключения
        protected virtual void DisconnectProvider() { }
        //Соединение было установлено
        private bool _isConnected;

        //Первичное подключение к провайдеру
        internal protected bool Connect()
        {
            if (_isConnected) return true;
            try { if (ConnectProvider()) return _isConnected = true; }
            catch (Exception ex)
            {
                AddWarning("Нет соединения с провайдером. Попытка повторного соединения", ex);
            }
            
            Thread.Sleep(300);
            Disconnect();
            Thread.Sleep(300);
            
            try { if (ConnectProvider()) return _isConnected = true; }
            catch (Exception ex)
            {
                AddError("Ошибка соединения с провайдером", ex);
            }
            Disconnect();
            return false;
        }

        //Отключение от провайдера
        internal protected void Disconnect()
        {
            try { DisconnectProvider();}
            catch {}
            _isConnected = false;
        }

        //Повторное подключение
        internal protected bool Reconnect()
        {
            if (_isConnected)
            {
                Disconnect();
                Thread.Sleep(300);
                _isConnected = false;
            }
            return Connect();
        }

        //Очистка ресурсов
        public virtual void Dispose()
        {
            Disconnect();
        }

        //Текущий период расчета
        public DateTime PeriodBegin { get { return ProviderConnect.PeriodBegin; } }
        public DateTime PeriodEnd { get { return ProviderConnect.PeriodEnd; } }

        //Настройка
        #region
        //Проверка соединения в форме настроек возвращает true, если соединение успешное
        public virtual bool CheckConnection() { return true; }
        //Cтрока для вывода сообщения о последней проверке соединения
        public string CheckConnectionMessage { get; protected set; }
        //Проверка корректности настроек, возвращает строку с ошибками, на входе словарь настроек
        public virtual string CheckSettings(DicS<string> infDic) { return ""; }

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