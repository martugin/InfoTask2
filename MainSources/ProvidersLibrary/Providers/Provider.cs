using System;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Один провайдер
    public abstract class Provider : ExternalLogger
    {
        //Код соединения
        public string Name { get; set; }
        //Тип соединения
        public abstract ProviderType Type { get; }
        //Комплект провайдеров, допустимых для соединения
        public abstract string Complect { get; }
        //Код провайдера
        public abstract string Code { get; }

        //Контекст для логгера
        public override string CodeObject
        {
            get { return Type.ToRussian() + ": " + Name + ", " + Code; }
        }

        //Основное и резервное подключение
        protected ProviderConnect MainConnect { get; private set; }
        protected ProviderConnect ReserveConnect { get; private set; }
        //Текущее соединение 
        protected ProviderConnect CurConnect { get; set; }

        //Создание основного и резервного подключения по заданным настройкам
        public void AddMainConnect(string inf)
        {
            MainConnect = CreateConnect();
            MainConnect.Inf = inf;
            CurConnect = MainConnect;
        }
        public void AddReserveConnect(string inf)
        {
            ReserveConnect = CreateConnect();
            ReserveConnect.Inf = inf;
            if (CurConnect == null)
                CurConnect = ReserveConnect;
        }
        protected abstract ProviderConnect CreateConnect();
        
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