using System;
using System.Runtime.InteropServices;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace ComLaunchers
{
    //Интерфейс для ReceivConnect
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IReceivConnect
    {
        //Код соединения
        string Name { get; }
        //Комплект провайдеров
        string Complect { get; }
        //Присвоение основного и резервного провайдера 
        void JoinProvider(string mainCode, string mainInf, string reserveCode = null, string reserveInf = null);

        //Очистка списка сигналов
        void ClearSignals();

        //Добавить сигнал
        ReceivSignal AddSignal(string fullCode, //Полный код сигнала
                                            string dataType, //Тип данных
                                            string infObject, //Свойства объекта
                                            string infOut, //Свойства выхода относительно объекта
                                            string infProp); //Свойства сигнала относительно выхода

        //Запись значений в приемник
        void WriteValues(DateTime periodBegin, DateTime periodEnd); //Период обработки
        //Запись значений в приемник с отображением индикатора, при завершении вызывает событие
        void WriteValuesAsync(DateTime periodBegin, DateTime periodEnd); //Период обработки
    }

    //-----------------------------------------------------------------------------------------------------
    //Соединение с приемником для взаимодействия через COM
    [ClassInterface(ClassInterfaceType.None)]
    public class ReceivConnect : IReceivConnect
    {
        internal ReceivConnect(ReceiverConnect connect, ProvidersFactory factory)
        {
            Connect = connect;
            _factory = factory;
        }
           
        //Ссылка на соединение
        internal ReceiverConnect Connect { get; private set; }
        //Фабрика провайдеров
        private readonly ProvidersFactory _factory;
        //Ссылка на логгер
        private Logger Logger { get { return Connect.Logger; }}

        //Код соединения
        public string Name { get { return Connect.Name; } }
        //Комплект провайдеров
        public string Complect { get { return Connect.Complect; } }

        //Присвоение основного и резервного провайдера 
        public void JoinProvider(string mainCode, string mainInf, //Код и настройки основного провайдера
                                             string reserveCode = null, string reserveInf = null) //Код и настройки резервного провайдера
        {
            Logger.RunSyncCommand(() =>
                {
                    var main = _factory.CreateProvider(mainCode, mainInf);
                    var reserve = reserveCode == null ? null : _factory.CreateProvider(reserveCode, reserveInf);
                    Connect.JoinProvider(main, reserve);
                });
        }

        //Тип провайдера
        protected ProviderType Type { get { return ProviderType.Receiver;}}

        //Очистка списка сигналов
        public void ClearSignals()
        {
            Logger.RunSyncCommand(Connect.ClearSignals);
        }

        //Добавить сигнал
        public ReceivSignal AddSignal(string fullCode, //Полный код сигнала
                                                      string dataType, //Тип данных
                                                      string infObject, //Свойства объекта
                                                      string infOut, //Свойства выхода относительно объекта
                                                      string infProp) //Свойства сигнала относительно выхода
        {
            return new ReceivSignal(Connect.AddSignal(fullCode, dataType.ToDataType(), infObject, infOut, infProp));
        }

        //Запись значений в приемник
        public void WriteValues(DateTime periodBegin, DateTime periodEnd)
        {
            Logger.RunSyncCommand(periodBegin, periodEnd, () => Connect.WriteValues());
        }

        //Запись значений в приемник с отображением индикатора, при завершении вызывает событие
        public void WriteValuesAsync(DateTime periodBegin, DateTime periodEnd)
        {
            Logger.RunAsyncCommand(periodBegin, periodEnd, () => Connect.WriteValues());
        }
    }
}