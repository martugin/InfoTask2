using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для всех источников
    public abstract class SourceBase : ProviderBase
    {
        //Ссылка на соединение
        public SourceConnect SourceConnect
        {
            get { return (SourceConnect)ProviderConnect; }
        }

        //Получение диапазона времени источника
        //Возвращает Default интервал, если нет связи с источником
        //Возвращает TimeInterval(Different.MinDate, DateTime.Now) если источник не позволяет определять диапазон
        internal protected TimeInterval GetTime()
        {
            try
            {
                AddEvent("Определение диапазона источника");
                if (!ConnectProvider(false)) return TimeInterval.CreateDefault();
                var ti = GetSourceTime();
                if (!ti.IsDefault)
                    AddEvent("Диапазон источника определен", ti.Begin + " - " + ti.End);
                else AddError("Диапазон источника не определен");
                return ti;
            }
            catch (Exception ex)
            {
                AddError("Ошибка определения временного диапазона источника", ex);
                return TimeInterval.CreateDefault();
            }
        }
        //Получение времени источника 
        protected virtual TimeInterval GetSourceTime()
        {
            return new TimeInterval(Different.MinDate, DateTime.Now);
        }

        //Источник был подготовлен
        internal bool IsPrepared { get; set; }
        //Подготовка провайдера
        public override void Prepare()
        {
            try
            {
                using (Start())
                {
                    IsPrepared = false;
                    ClearObjects();
                    foreach (var sig in SourceConnect.InitialSignals.Values)
                    {
                        var ob = AddObject(sig);
                        ob.Context = sig.CodeObject;
                        ob.AddSignal(sig);
                    }
                    Procent = 30;
                    PrepareSource();
                    if (ErrPool == null)
                        ErrPool = new ErrMomPool(MakeErrFactory());
                    IsPrepared = true;    
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при подготовке источника", ex);
            }
        }

        //Очистка списков объектов
        internal protected abstract void ClearObjects();
        //Добавить объект содержащий заданный сигнал
        internal protected abstract SourceObject AddObject(InitialSignal sig);
        //Подготовка источника
        public virtual void PrepareSource() {}
        
        //Создание фабрики ошибок
        protected virtual IErrMomFactory MakeErrFactory()
        {
            var factory = new ErrMomFactory(ProviderConnect.Name, ErrMomType.Source);
            factory.AddGoodDescr(0);
            return factory;
        }
        //Хранилище ошибок 
        internal ErrMomPool ErrPool { get; private set; }

        //Создание ошибки 
        public ErrMom MakeError(int number, IContextable addr)
        {
            return ErrPool.MakeError(number, addr);
        }

        //Чтение среза, возврашает количество прочитанных значений
        internal protected virtual ValuesCount ReadCut() { return new ValuesCount(); }
        //Чтение изменений, возврашает количество прочитанных и сформированных значений
        internal protected abstract ValuesCount ReadChanges();
    }
}