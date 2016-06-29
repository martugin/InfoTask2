using System;
using System.Collections.Generic;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Настройки одного подключения провайдера (основного или резервного)
    public abstract class ProviderSettings : ExternalLogger, IDisposable
    {
        //Ссылка на провайдер
        public ProviderBase Provider { get; set; }
        //Контекст для логгера
        public override string Context { get { return Provider.Context; } }

        //Загрузка настроек провайдера
        public string Inf
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
        public abstract string Hash { get; }

        //Очистка ресурсов
        public virtual void Dispose() {}

        //Открытие подключения, возвращает true, если соединение установлено
        public virtual bool Connect() { return true; }
        //Соединение установлено
        protected bool IsConnected { get; set; }
        
        //Настройка
        #region
        //Проверка соединения в форме настроек возвращает true, если соединение успешное
        public virtual bool CheckConnection() { return true; }
        //Cтрока для вывода сообщения о последней проверке соединения
        public string CheckConnectionMessage { get; protected set; }
        //Проверка корректности настроек, возвращает строку с ошибками, на входе словарь настроек
        public virtual string CheckSettings(DicS<string> infDic) { return ""; }

        //Словарь комманд открытия дилогов, ключи - имена свойств, вторые ключи - названия пунктов меню
        internal DicS<Dictionary<string, IMenuCommand>> MenuCommands { get; private set; }
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