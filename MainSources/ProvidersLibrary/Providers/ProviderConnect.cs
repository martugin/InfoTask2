using System;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Соединение 
    public abstract class ProviderConnect : ExternalLogg
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
        private ProviderBase _mainProvider;
        private ProviderBase _reserveProvider;
        //Текущий провайдер
        public ProviderBase Provider { get; private set; }

        //Присвоение основного и резервного провайдеров 
        public void JoinProviders(ProviderBase mainProvider, ProviderBase reserveProvaider = null)
        {
            Provider = _mainProvider = mainProvider;
            if (mainProvider != null)
            {
                mainProvider.ProviderConnect = this;
                mainProvider.Logger = Logger;
            }
            _reserveProvider = reserveProvaider;
            if (reserveProvaider != null)
            {
                reserveProvaider.ProviderConnect = this;
                reserveProvaider.Logger = Logger;
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
                if (Provider is SourceBase)
                    ((SourceBase)Provider).Prepare();
                if (Provider is ReceiverBase)
                    ((ReceiverBase)Provider).Prepare();
            }
            catch (Exception ex)
            {
                AddError("Ошибка подготовки провайдера", ex);
            }
        }

        //Текущий период расчета
        public DateTime PeriodBegin { get; protected set; }
        public DateTime PeriodEnd { get; protected set; }
    }
}