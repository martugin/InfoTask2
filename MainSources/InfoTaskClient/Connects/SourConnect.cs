﻿using System;
using System.Runtime.InteropServices;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace ComClients
{
    //Интерфейс для SourConnect
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ISourConnect : IProvConnect
    {
        //Получение диапазона времени источника
        void GetTime();

        //Диапазон времени источника
        DateTime BeginTime { get; }
        DateTime EndTime { get; }

        //Очистка списка сигналов
        void ClearSignals();

        //Добавить исходный сигнал
        SourSignal AddInitialSignal(string fullCode, //Полный код сигнала
                string codeOut, //Код выхода
                string dataType, //Тип данных
                string signalInf, //Настройки сигнала
                bool needCut); //Нужно считывать срез значений

        //Добавить расчетный сигнал
        SourSignal AddCalcSignal(string fullCode, //Полный код сигнала
                string codeObject, //Код объекта
                string initialSignal, //Код исходного сигнала
                string formula); //Формула

        //Чтение значений из источника
        void GetValues(DateTime periodBegin, DateTime periodEnd);
        //Чтение значений из источника. Выполняется асинхронно, программа после вызова метода сразу освобождается 
        //Система узнает о завершении чтения через событие Finished
        void GetValuesAsync(DateTime periodBegin, DateTime periodEnd);
        //То же самое, но время берется из логгера
        void GetValues();
        void GetValuesAsync();

        //Создание клона
        void MakeClone(DateTime periodBegin, //Начало периода клона
                                DateTime periodEnd, //Конец периода клона
                                string cloneDir); //Каталог клона
        //То же самое, но время берется из логгера
        void MakeClone(string cloneDir); //Каталог клона
    }

    //-----------------------------------------------------------------------------------------------------
    //Соединение с источником для взаимодействия через COM
    [ClassInterface(ClassInterfaceType.None),
    ComSourceInterfaces(typeof(ILoggerClientEvents))]
    public class SourConnect : ProvConnect, ISourConnect
    {
        internal SourConnect(SourceConnect connect, ProvidersFactory factory) 
            : base(connect, factory) {}
           
        //Ссылка на соединение
        internal SourceConnect Connect
        {
            get { return (SourceConnect) ProviderConnect; }
        }

        //Тип провайдера
        protected override ProviderType Type { get { return ProviderType.Source;}}

        //Получение диапазона времени источника
        public void GetTime()
        {
            RunSyncCommand(() => {_interval = Connect.GetTime();});
        }
        private TimeInterval _interval;

        //Диапазон времени источника
        public DateTime BeginTime { get { return _interval.Begin; } }
        public DateTime EndTime { get { return _interval.End; } }

        //Очистка списка сигналов
        public void ClearSignals()
        {
            RunSyncCommand(Connect.ClearSignals);
        }

        //Добавить исходный сигнал
        public SourSignal AddInitialSignal(string fullCode, //Полный код сигнала
                                                             string codeOut, //Код выхода
                                                             string dataType, //Тип данных
                                                             string signalInf, //Настройки сигнала
                                                             bool needCut) //Нужно считывать срез значений
        {
            return new SourSignal(Connect.AddInitialSignal(fullCode, codeOut, dataType.ToDataType(), signalInf, needCut));
        }

        //Добавить расчетный сигнал
        public SourSignal AddCalcSignal(string fullCode, //Полный код сигнала
                                                         string codeObject, //Код объекта
                                                         string initialSignal, //Код исходного сигнала
                                                         string formula) //Формула
        {
            return new SourSignal(Connect.AddCalcSignal(fullCode, codeObject, initialSignal, formula));
        }
        
        //Чтение значений из источника. Программа, вызвавшая метод, занята пока чтение не завершится
        public void GetValues(DateTime periodBegin, DateTime periodEnd)
        {
            RunSyncCommand(periodBegin, periodEnd, () => Connect.GetValues());
        }
        //Чтение значений из источника. Выполняется асинхронно, программа после вызова метода сразу освобождается 
        //Система узнает о завершении чтения через событие Finished
        public void GetValuesAsync(DateTime periodBegin, DateTime periodEnd)
        {
            RunAsyncCommand(periodBegin, periodEnd, () => Connect.GetValues());
        }
        //То же самое, но время берется из логгера
        public void GetValues()
        {
            RunSyncCommand(() => Connect.GetValues());
        }
        public void GetValuesAsync()
        {
            RunAsyncCommand(() => Connect.GetValues());
        }

        //Создание клона источника, всегда выполняется асинхронно
        public void MakeClone(DateTime periodBegin, //Начало периода клона
                                          DateTime periodEnd, //Конец периода клона
                                          string cloneDir) //Каталог клона
        {
            RunAsyncCommand(periodBegin, periodEnd, () => Connect.MakeClone(cloneDir));
        }
        //То же самое, но время берется из логгера
        public void MakeClone(string cloneDir) //Каталог клона
        {
            RunAsyncCommand(() => Connect.MakeClone(cloneDir));
        }
    }
}