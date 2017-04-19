using System;
using BaseLibrary;
using CommonTypes;

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

        //Список сигналов, содержащих возвращаемые значения
        protected internal readonly DicS<ProviderSignal> ProviderSignals = new DicS<ProviderSignal>();
        public IDicSForRead<ProviderSignal> Signals { get { return ProviderSignals; } }
        
        //Очистка списка сигналов
        public virtual void ClearSignals()
        {
            AddEvent("Очистка списка сигналов");
            Provider.IsPrepared = false;
            Provider.ClearOuts();
            ProviderSignals.Clear();
        }

        //Добавить сигнал
        public ProviderSignal AddSignal(string fullCode, //Полный код сигнала
                                                        DataType dataType, //Тип данных
                                                        SignalType signalType, //Тип сигнала
                                                        string infObject, //Свойства объекта
                                                        string infOut, //Свойства выхода относительно объекта
                                                        string infProp = "") //Свойства сигнала относительно выхода
        {
            if (ProviderSignals.ContainsKey(fullCode))
                return ProviderSignals[fullCode];
            Provider.IsPrepared = false;
            var contextOut = infObject + (infOut.IsEmpty() ? "" : ";" + infOut);
            var inf = infObject.ToPropertyDicS().AddDic(infOut.ToPropertyDicS()).AddDic(infProp.ToPropertyDicS());
            return ProviderSignals.Add(fullCode, AddConcreteSignal(fullCode, dataType, signalType, contextOut, inf));
        }

        //Переопределяемый метод для добавления сигналов конкретного типа
        protected abstract ProviderSignal AddConcreteSignal(string fullCode, DataType dataType, SignalType signalType, string contextOut, DicS<string> inf);
    }
}