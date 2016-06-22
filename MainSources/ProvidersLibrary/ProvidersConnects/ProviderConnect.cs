using System;
using System.Collections.Generic;
using System.Threading;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Базовый класс для соединений с провайдерами
    public abstract class ProviderConnect : ExternalLogger, IProviderConnect
    {
        protected ProviderConnect() 
            : base (new Logger()){}
        protected ProviderConnect(string name, Logger logger)
            : base(logger)
        {
            Logger = logger;
            Name = name;
        }

        //Имя провайдера
        public string Name { get; set; }
        //Тип провайдера
        public abstract ProviderType Type { get; }
        //Комплект провайдера
        public abstract string Complect { get; }
        //Код провайдера 
        public abstract string Code { get; }
        //Свойства провайдера
        protected string ProviderInf { get; set; }
        //Хэш для идентификации соединения
        public string Hash { get; protected set; }

        //Загрузка настроек провайдера
        public string Inf
        {
            get { return ProviderInf; }
            set
            {
                ProviderInf = value;
                var dic = ProviderInf.ToPropertyDicS();
                dic.DefVal = "";
                ReadInf(dic);
            }
        }

        //Загрузка свойств из словаря
        protected abstract void ReadInf(DicS<string> dic);

        //Контекст и имя объекта для записи комманд и ошибок, заданные по умолчанию
        public override string Context
        {
            get { return Name + ", " + Type.ToRussian() + ", " + Complect + ", " + Code + "; " + Hash; }
        }

        //Проверка соединения с провайдером, вызывается в настройках, или когда уже произошла ошибка для повторной проверки соединения
        public virtual bool Check() { return true; }
        //Проверка соединения в настройке
        public virtual bool CheckConnection() { return true; }
        //Cтрока для вывода сообщения о последней проверке соединения
        public string CheckConnectionMessage { get; set; }
        //Проверка настроек
        public virtual string CheckSettings(Dictionary<string, string> infDic) //Словарь со значениями настроек
        { return ""; }

        //Вызов окна настройки
        public string Setup()
        {
            if (MenuCommands == null)
            {
                MenuCommands = new DicS<Dictionary<string, IMenuCommand>>();
                AddMenuCommands();
            }
            IsSetup = true;
            new ProviderSetupForm { Connect = this }.ShowDialog();
            while (IsSetup) Thread.Sleep(500);
            return ProviderInf;
        }

        //True, пока идет настройка
        internal bool IsSetup { get; set; }

        //Словарь комманд открытия дилогов, ключи - имена свойств, вторые ключи - названия пунктов меню
        internal DicS<Dictionary<string, IMenuCommand>> MenuCommands { get; private set; }

        //Задание комманд, вызываемых из меню
        protected virtual void AddMenuCommands() { }

        //Возвращает выпадающий список для поля настройки 
        internal virtual List<string> ComboBoxList(Dictionary<string, string> props, //props - словарь значение свойств 
                                                                     string propname) //propname - имя свойства для ячейки со списком
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

        //Очистка ресурсов
        public virtual void Dispose() { }
    }
}