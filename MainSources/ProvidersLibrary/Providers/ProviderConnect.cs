using System;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Соединение 
    public abstract class ProviderConnect : ExternalLogger
    {
        protected ProviderConnect(string name, string complect, Logger logger)
            : base(logger)
        {
            Name = name;
            Complect = complect;
        }
        
        //Код соединения
        public string Name { get; private set; }
        //Тип провайдера
        public abstract ProviderType Type { get; }
        //Комплект провайдеров
        public string Complect { get; private set; }
        //Контекст для логгера
        public override string Context
        {
            get { return Type.ToRussian() + ": " + Name; }
        }

        //Основной и резервный провайдеры
        private BaseProvider _mainProvider;
        private BaseProvider _reserveProvider;
        //Текущий провайдер
        public BaseProvider Provider { get; private set; }

        //Присвоение основного и резервного провайдеров 
        public void JoinProviders(BaseProvider mainProvider, BaseProvider reserveProvider = null)
        {
            Provider = _mainProvider = mainProvider;
            if (mainProvider != null)
            {
                AddEvent("Присоединение основного провайдера", mainProvider.Code + "; " + mainProvider.Inf);
                mainProvider.ProviderConnect = this;
                mainProvider.Logger = Logger;
            }
            _reserveProvider = reserveProvider;
            if (reserveProvider != null)
            {
                AddEvent("Присоединение резервного провайдера", reserveProvider.Code + "; " + reserveProvider.Inf);
                reserveProvider.ProviderConnect = this;
                reserveProvider.Logger = Logger;
            }
        }

        //Переключение текущего провайдера, возвращает true, если переключение произошло
        protected bool ChangeProvider()
        {
            if (Provider == _mainProvider && _reserveProvider != null)
            {
                Provider = _reserveProvider;
                AddEvent("Текущий провайдер изменен на резервный");
                return true;
            }
            if (Provider == _reserveProvider && _mainProvider != null)
            {
                Provider = _mainProvider;
                AddEvent("Текущий провайдер изменен на основной");
                return true;
            }
            return false;
        }

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

        //Подготовка соединения
        public void Prepare()
        {
            try
            {
                if (Provider is BaseSource)
                    ((BaseSource)Provider).Prepare();
                if (Provider is BaseReceiver)
                    ((BaseReceiver)Provider).Prepare();
            }
            catch (Exception ex)
            {
                AddError("Ошибка подготовки провайдера", ex);
            }
        }

        //Проверяет, что в логгере задан период обработки
        protected bool CheckPeriodIsDefined()
        {
            if (PeriodBegin != Different.MinDate) return true;
            AddError("Не задан период обработки");
            return false;
        }
    }
}