using System;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Базовый класс для соединений с провайдерами
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
        private Provider _mainProvider;
        private Provider _reserveProvider;
        //Текущий провайдер
        internal Provider Provider { get; private set; }

        //Присвоение основного и резервного провайдеров 
        public void JoinProvider(Provider mainProvider, Provider reserveProvider = null)
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
            if (mainProvider != null && reserveProvider != null && mainProvider.SignalType != reserveProvider.SignalType)
                AddError("Разный тип значений у основного и резервного провайдера", null, _mainProvider.SignalType.ToEnglish() + ", " + _reserveProvider.SignalType.ToEnglish());
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

        //Получение диапазона времени провайдера
        //Возвращает Default интервал, если нет связи с провайдером
        //Возвращает TimeInterval(Static.MinDate, DateTime.Now) если провайдер не позволяет определять диапазон
        public TimeInterval GetTime()
        {
            var ti = Provider.GetTime();
            if (ti.IsDefault && ChangeProvider())
                return Provider.GetTime();
            return ti;
        }

        //Проверяет, что в логгере задан период обработки
        protected bool PeriodIsUndefined()
        {
            if (PeriodBegin != Static.MinDate && PeriodBegin != Static.MaxDate) return false;
            AddError("Не задан период обработки");
            return true;
        }

        //Очистка списка сигналов
        public virtual void ClearSignals()
        {
            AddEvent("Очистка списка сигналов");
            Provider.IsPrepared = false;
            Provider.ClearOuts();
        }
    }
}