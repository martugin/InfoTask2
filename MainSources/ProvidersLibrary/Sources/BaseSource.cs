﻿using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для всех источников
    public abstract class BaseSource : BaseProvider
    {
        //Ссылка на соединение
        internal SourceConnect SourceConnect
        {
            get { return (SourceConnect)ProviderConnect; }
        }

        //Получение диапазона времени источника
        //Возвращает Default интервал, если нет связи с источником
        //Возвращает TimeInterval(Static.MinDate, DateTime.Now) если источник не позволяет определять диапазон
        protected internal TimeInterval GetTime()
        {
            try
            {
                if (!Connect()) return TimeInterval.CreateDefault();
                AddEvent("Определение диапазона источника");
                var ti = GetTimeSource();
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
        protected virtual TimeInterval GetTimeSource()
        {
            return new TimeInterval(Static.MinDate, DateTime.Now);
        }

        //Подготовка источника
        protected internal bool Prepare()
        {
            try
            {
                AddEvent("Подготовка выходов");
                ClearOuts();
                foreach (var sig in SourceConnect.InitialSignals.Values)
                {
                    var ob = AddOut(sig);
                    ob.Context = sig.CodeOuts;
                    ob.AddSignal(sig);
                }
                Procent = 20;
                if (!Connect()) return false;
                IsPrepared = StartDanger(0, 100, 2, LoggerStability.Periodic, "Подготовка провайдера")
                                    .Run(PrepareProvider, Reconnect).IsSuccess;
                if (ErrPool == null)
                    ErrPool = new MomErrPool(MakeErrFactory());
                return IsPrepared;    
            }
            catch (Exception ex)
            {
                AddError("Ошибка при подготовке источника", ex);
                return false;
            }
        }

        //Очистка списков объектов
        protected abstract void ClearOuts();
        //Добавить объект содержащий заданный сигнал
        protected abstract SourceOut AddOut(InitialSignal sig);
        
        //Создание фабрики ошибок
        protected virtual IMomErrFactory MakeErrFactory()
        {
            var factory = new MomErrFactory(ProviderConnect.Name, MomErrType.Source);
            factory.AddGoodDescr(0);
            return factory;
        }
        //Хранилище ошибок 
        internal MomErrPool ErrPool { get; private set; }

        //Создание ошибки 
        internal MomErr MakeError(int number, IContextable addr)
        {
            return ErrPool.MakeError(number, addr);
        }

        //Чтение среза, возврашает количество прочитанных значений
        protected internal virtual ValuesCount ReadCut() { return new ValuesCount(); }
        //Чтение изменений, возврашает количество прочитанных и сформированных значений
        protected internal abstract ValuesCount ReadChanges();

        //Конец предыдущего периода чтения значений
        internal DateTime PrevPeriodEnd { get; set; }
    }
}