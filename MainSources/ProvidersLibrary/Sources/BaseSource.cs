using System;
using BaseLibrary;
using CommonTypes;
using Different = BaseLibrary.Different;

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
        //Возвращает TimeInterval(Different.MinDate, DateTime.Now) если источник не позволяет определять диапазон
        protected internal TimeInterval GetTime()
        {
            try
            {
                AddEvent("Определение диапазона источника");
                if (!Connect()) return TimeInterval.CreateDefault();
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
            return new TimeInterval(Different.MinDate, DateTime.Now);
        }

        //Подготовка источника
        internal void Prepare()
        {
            try
            {
                using (Start())
                {
                    AddEvent("Подготовка источника");
                    ClearOuts();
                    foreach (var sig in SourceConnect.InitialSignals.Values)
                    {
                        var ob = AddOut(sig);
                        ob.Context = sig.CodeOuts;
                        ob.AddSignal(sig);
                    }
                    StartDanger(30, 100, 2, LoggerDangerness.Single, "Ошибка при подготовке источника", "Повтор подготовки источника")
                        .Run(() => PrepareSource(), () => Reconnect());
                    if (ErrPool == null)
                        ErrPool = new MomErrPool(MakeErrFactory());
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при подготовке источника", ex);
            }
        }

        //Очистка списков объектов
        protected abstract void ClearOuts();
        //Добавить объект содержащий заданный сигнал
        protected abstract SourceOut AddOut(InitialSignal sig);
        //Подготовка источника
        protected virtual void PrepareSource() {}
        
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
    }
}