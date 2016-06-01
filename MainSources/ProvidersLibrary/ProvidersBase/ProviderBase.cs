using System;
using System.Collections.Generic;
using System.Threading;
using BaseLibrary;

namespace CommonTypes
{
    //Базовый класс для всех провайдеров
    public abstract class ProviderBase : ExternalLogger, IProvider
    {
        protected ProviderBase() : base (new Logger()){}
        protected ProviderBase(string name, Logger logger) : base (logger)
        {
            Logger = logger;
            Name = name;
        }

        //Имя провайдера
        public string Name { get; set; }
        //Тип провайдера
        public abstract ProviderType Type { get; }
        //Код провайдера 
        public abstract string Code { get; }
        //Свойства провайдера
        protected string ProviderInf { get; set; }
        //Кэш для идентификации соединения
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
                ReadDicS(dic);
            }
        }

        //Загрузка свойств из словаря
        protected abstract void ReadDicS(DicS<string> dic);

        //Контекст и имя объекта для записи комманд и ошибок, заданные по умолчанию
        public override string Context
        {
            get { return Name + ", " + Type.ToRussian() + ", " + Code + "; " + Hash; }
        }

        public virtual bool Check() { return true; }
        public virtual bool CheckConnection() { return true; }
        public virtual string CheckSettings(Dictionary<string, string> infDic, Dictionary<string, string> nameDic)
        { return ""; }

        //Cтрока для вывода сообщения о последней проверке соединения
        public string CheckConnectionMessage { get; set; }

        //Тип настройки
        public ProviderSetupType SetupType { get; protected set; }

        //True, пока идет настройка
        public bool IsSetup { get; set; }

        //Вызов окна настройки
        public string Setup()
        {
            if (MenuCommands == null)
            {
                MenuCommands = new DicS<Dictionary<string, IMenuCommand>>();
                AddMenuCommands();
            }
            IsSetup = true;
            new ProviderSetupForm {SetupType = SetupType, Provider = this}.ShowDialog();
            while (IsSetup) Thread.Sleep(500);
            return ProviderInf;
        }

        //Словарь комманд открытия дилогов, ключи - имена свойств, вторые ключи - названия пунктов меню
        public DicS<Dictionary<string, IMenuCommand>> MenuCommands { get; private set; }

        //Задание комманд, вызываемых из меню
        protected virtual void AddMenuCommands() { }

        //Возвращает выпадающий список для поля настройки, props - словарь значение свойств, propname - имя свойства для ячейки со списком
        public virtual List<string> ComboBoxList(Dictionary<string, string> props, string propname)
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

        //Подготовка, вызывается при загрузке проекта
        public virtual void Prepare() { }

        //Очистка ресурсов
        public virtual void Dispose() { }

        //Текущий период расчета, задается при чтениии из провайдера или при записис в провайдер
        public DateTime PeriodBegin { get; protected set; }
        public DateTime PeriodEnd { get; protected set; }
    }
}
