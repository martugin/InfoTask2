using System;
using System.Runtime.InteropServices;
using AppLibrary;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace ComLaunchers
{
    //Интерфейс для LauncherSourceConnect
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ILauncherSourceConnect
    {
        //Код соединения
        string Name { get; }
        //Комплект провайдеров
        string Complect { get; }
        //Присвоение основного и резервного провайдера 
        void JoinProvider(string mainCode, string mainInf, string reserveCode = null, string reserveInf = null);

        //Получение диапазона времени источника
        void GetTime();

        //Диапазон времени источника
        DateTime BeginTime { get; }
        DateTime EndTime { get; }

        //Очистка списка сигналов
        void ClearSignals();
        
        //Добавить исходный сигнал
        LauncherSourceSignal AddSignal(string fullCode, //Полный код сигнала
                                            string dataType, //Тип данных
                                            string valueType, //Тип значений сигнала
                                            string infObject, //Свойства объекта
                                            string infOut, //Свойства выхода относительно объекта
                                            string infProp); //Свойства сигнала относительно выхода
        
        //Добавить расчетный сигнал
        LauncherSourceSignal AddCalcSignal(string fullCode, //Полный код сигнала
                                               string objectCode, //Код объекта
                                               string initialSignalCode, //Код исходного сигнала без кода объекта
                                               string formula); //Формул

        //Чтение значений из источника
        void GetValues(DateTime periodBegin, DateTime periodEnd);
        //Чтение значений из источника. Выполняется асинхронно, программа после вызова метода сразу освобождается 
        //Система узнает о завершении чтения через событие Finished
        void GetValuesAsync(DateTime periodBegin, DateTime periodEnd);
    }

    //-----------------------------------------------------------------------------------------------------
    //Соединение с источником для взаимодействия через COM
    [ClassInterface(ClassInterfaceType.None)]
    public class LauncherSourceConnect : ILauncherSourceConnect
    {
        internal LauncherSourceConnect(ClonerConnect connect, ProvidersFactory factory)
        {
            Connect = connect;
            _factory = factory;
        }
           
        //Ссылка на соединение
        internal ClonerConnect Connect { get; private set; }
        //Фабрика провайдеров
        private readonly ProvidersFactory _factory;
        //Ссылка на логгер
        private Logger Logger { get { return Connect.Logger; }}

        //Код соединения
        public string Name { get { return Connect.Code; } }
        //Комплект провайдеров
        public string Complect { get { return Connect.Complect; } }

        //Присвоение основного и резервного провайдера 
        public void JoinProvider(string mainCode, string mainInf, //Код и настройки основного провайдера
                                             string reserveCode = null, string reserveInf = null) //Код и настройки резервного провайдера
        {
            Logger.RunSyncCommand(() =>
                {
                    var main = _factory.CreateProvider(Logger, mainCode, mainInf);
                    var reserve = reserveCode == null ? null : _factory.CreateProvider(Logger, reserveCode, reserveInf);
                    Connect.JoinProvider(main, reserve);
                });
        }

        //Тип провайдера
        protected internal ProviderType Type { get { return ProviderType.Source;}}

        //Получение диапазона времени источника
        public void GetTime()
        {
            Logger.RunSyncCommand(() => {_interval = Connect.GetTime();});
        }
        private TimeInterval _interval = new TimeInterval();

        //Диапазон времени источника
        public DateTime BeginTime { get { return _interval.Begin; } }
        public DateTime EndTime { get { return _interval.End; } }

        //Очистка списка сигналов
        public void ClearSignals()
        {
            Logger.RunSyncCommand(Connect.ClearSignals);
        }

        //Добавить исходный сигнал
        public LauncherSourceSignal AddSignal(string fullCode, //Полный код сигнала
                                                     string dataType, //Тип данных
                                                     string valueType, //Тип значений сигнала
                                                     string infObject, //Свойства объекта
                                                     string infOut, //Свойства выхода относительно объекта
                                                     string infProp) //Свойства сигнала относительно выхода
        {
            return new LauncherSourceSignal((ListSignal)Connect.AddSignal(fullCode, dataType.ToDataType(), valueType.ToSignalType(), infObject, infOut, infProp));
        }

        //Добавить расчетный сигнал
        public LauncherSourceSignal AddCalcSignal(string fullCode, //Полный код сигнала
                                                         string objectCode, //Код объекта
                                                         string initialSignalCode, //Код исходного сигнала без кода объекта
                                                         string formula) //Формул
        {
            return new LauncherSourceSignal(Connect.AddCalcSignal(fullCode, objectCode, initialSignalCode, formula));
        }
        
        //Чтение значений из источника. Программа, вызвавшая метод, занята пока чтение не завершится
        public void GetValues(DateTime periodBegin, DateTime periodEnd)
        {
            Logger.RunSyncCommand(periodBegin, periodEnd, () => Connect.GetValues());
        }

        //Чтение значений из источника. Выполняется асинхронно, программа после вызова метода сразу освобождается 
        //Система узнает о завершении чтения через событие Finished
        public void GetValuesAsync(DateTime periodBegin, DateTime periodEnd)
        {
            Logger.RunAsyncCommand(periodBegin, periodEnd, () => Connect.GetValues());
        }
    }
}