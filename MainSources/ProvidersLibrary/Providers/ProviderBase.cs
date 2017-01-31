using System;
using System.Collections.Generic;
using System.Threading;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Один провайдер
    public abstract class ProviderBase : ExternalLogg, IDisposable
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
        internal string Inf
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
            using (Start())
            {
                try
                {
                    if (ConnectProvider())
                        return _isConnected = true;
                }
                catch (Exception ex)
                {
                    AddWarning("Нет соединения с провайдером. Попытка повторного соединения", ex);
                }

                Procent = 30;
                Thread.Sleep(300);
                Disconnect();
                Procent = 60;
                Thread.Sleep(300);
                Procent = 70;

                try
                {
                    if (ConnectProvider())
                        return _isConnected = true;
                }
                catch (Exception ex)
                {
                    AddError("Ошибка соединения с провайдером", ex);
                }
                Procent = 90;
                Disconnect();
            }
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
            using (Start())
            {
                if (_isConnected)
                {
                    Disconnect();
                    Procent = 10;
                    Thread.Sleep(300);
                    Procent = 30;
                }
                if (!Connect()) return false;
                if (!(this is SourceBase)) return true;

                AddEvent("Получение времени источника", 70);
                if (!((SourceBase)this).GetTime().IsDefault)
                    return true;
            }
            return false;
        }

        //Очистка ресурсов
        public virtual void Dispose()
        {
            Disconnect();
        }

        //Текущий период расчета
        protected DateTime PeriodBegin { get { return ProviderConnect.PeriodBegin; } }
        protected DateTime PeriodEnd { get { return ProviderConnect.PeriodEnd; } }

        //Настройка
        #region
        //Проверка соединения в форме настроек возвращает true, если соединение успешное
        protected virtual bool CheckConnection() { return true; }
        //Cтрока для вывода сообщения о последней проверке соединения
        internal protected string CheckConnectionMessage { get; protected set; }
        //Проверка корректности настроек, возвращает строку с ошибками, на входе словарь настроек
        internal protected virtual string CheckSettings(DicS<string> infDic) { return ""; }

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