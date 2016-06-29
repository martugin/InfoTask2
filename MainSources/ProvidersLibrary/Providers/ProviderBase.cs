using System;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Один провайдер
    public abstract class ProviderBase : ExternalLogger, IDisposable
    {
        //Код соединения
        public string Name { get; set; }
        //Тип провайдера
        public abstract ProviderType Type { get; }
        //Комплект провайдеров
        public abstract string Complect { get; }
        //Код провайдера
        public abstract string Code { get; }
        //Контекст для логгера
        public override string Context
        {
            get { return Type.ToRussian() + ": " + Name + ", " + Code; }
        }

        //Основное и резервное подключение
        protected ProviderSettings MainSettings { get; private set; }
        protected ProviderSettings ReserveSettings { get; private set; }
        //Текущее подключение 
        protected ProviderSettings CurSettings { get; set; }

        //Создание основного и резервного подключения по заданным настройкам
        public void AddMainConnect(string inf)
        {
            MainSettings = CreateConnect();
            MainSettings.Inf = inf;
            MainSettings.Provider = this;
            MainSettings.Logger = Logger;
            CurSettings = MainSettings;
        }
        public void AddReserveConnect(string inf)
        {
            ReserveSettings = CreateConnect();
            ReserveSettings.Inf = inf;
            ReserveSettings.Provider = this;
            MainSettings.Logger = Logger;
            if (CurSettings == null)
                CurSettings = ReserveSettings;
        }
        //Создание подключения, соответствующего провайдеру
        protected abstract ProviderSettings CreateConnect();
        
        //Проверка соединения с провайдером, вызывается когда уже произошла ошибка для повторной проверки соединения
        //Возвращает true, если соединение установлено
        public virtual bool Check() { return true; }
        //Соединение установлено
        protected bool IsConnected { get; set; }

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

        //Подготовка провайдера к работе (во время PrepareCalc)
        public virtual void Prepare() { }

        //Текущий период расчета
        public DateTime PeriodBegin { get; protected set; }
        public DateTime PeriodEnd { get; protected set; }

        //Очистка ресурсов
        public virtual void Dispose() { }
    }
}