using System;
using System.Collections.Generic;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Базовый класс для всех провайдеров
    public abstract class Prov : ExternalLogger
    {
        //Ссылка на соединение
        public ProvConn Conn { get; set; }
        //Код провайдера
        public abstract string Code { get; }
        //Хэш для идентификации настройки провайдера
        public abstract string Hash { get; }

        //Контекст для логгера
        public override string CodeObject
        {
            get { return Conn.Type.ToRussian() + ": " + Conn.Name + ", " + Code; }
        }

        //Загрузка настроек провайдера
        public string Inf
        {
            get { return ProviderInf; }
            set
            {
                ProviderInf = value;
                var dic = value.ToPropertyDicS();
                dic.DefVal = "";
                ReadInf(dic);
            }
        }
        protected string ProviderInf { get; set; }
        //Загрузка свойств из словаря
        protected abstract void ReadInf(DicS<string> dic);

        //Очистка ресурсов
        public virtual void Dispose() { }
        //Подготовка провайдера к работе (во время PrepareCalc)
        public virtual void Prepare() { }
       
        //Проверка соединения с провайдером, вызывается когда уже произошла ошибка для повторной проверки соединения
        //Возвращает true, если соединение установлено
        public virtual bool Check() { return true; }
        //Соединение установлено
        protected bool IsConnected { get; set; }

        //Текущий период расчета
        public DateTime PeriodBegin { get { return Conn.PeriodBegin; } }
        public DateTime PeriodEnd { get { return Conn.PeriodEnd; } }

        //Настройка
        #region
        //Проверка соединения в форме настроек возвращает true, если соединение успешное
        public virtual bool CheckConnection() { return true; }
        //Cтрока для вывода сообщения о последней проверке соединения
        public string CheckConnectionMessage { get; protected set; }
        //Проверка корректности настроек, возвращает строку с ошибками, на входе словарь настроек
        public virtual string CheckSettings(DicS<string> infDic) { return ""; }

        //Вызов окна настройки
        public string Setup()
        {
            throw new NotImplementedException();
            //if (MenuCommands == null)
            //{
            //    MenuCommands = new DicS<Dictionary<string, IMenuCommand>>();
            //    AddMenuCommands();
            //}
            //IsSetup = true;
            //new ProviderSetupForm { Conn = this }.ShowDialog();
            //while (IsSetup) Thread.Sleep(500);
            //return ProviderInf;
        }

        //True, пока идет настройка
        internal bool IsSetup { get; set; }

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