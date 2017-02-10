using System;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace ComClients
{
    //Соединение с источником для взаимодействия через COM
    public class SourConnect : ProvConnect
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
            RunShortCommand(() => {_interval = Connect.GetTime();});
        }
        private TimeInterval _interval;

        //Диапазон времени источника
        public DateTime TimeBegin { get { return _interval.Begin; } }
        public DateTime TimeEnd { get { return _interval.End; } }

        //Очистка списка сигналов
        public void ClearSignals()
        {
            Connect.ClearSignals();
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

        //Добавить исходный сигнал
        public SourSignal AddCalcSignal(string fullCode, //Полный код сигнала
                                                         string codeObject, //Код объекта
                                                         string initialSignal, //Код исходного сигнала
                                                         string formula) //Формула
        {
            return new SourSignal(Connect.AddCalcSignal(fullCode, codeObject, initialSignal, formula));
        }
        
        //Чтение значений из источника
        public void GetValues(DateTime periodBegin, DateTime periodEnd)
        {
            RunLongCommand(() => Connect.GetValues(periodBegin, periodEnd));
        }

        //Создание клона источника
        public void MakeClone(DateTime periodBegin, //Начало периода клона
                                          DateTime periodEnd, //Конец периода клона
                                          string cloneDir) //Каталог клона
        {
            RunLongCommand(() => Connect.MakeClone(periodBegin, periodEnd, cloneDir));
        }
    }
}