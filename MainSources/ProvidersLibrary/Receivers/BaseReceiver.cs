using System;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Провайдер - приемник
    public abstract class BaseReceiver : BaseProvider
    {
        //Ссылка на соединение
        internal ReceiverConnect ReceiverConnect
        {
            get { return (ReceiverConnect)ProviderConnect; }
        }

        //Подготовка приемника 
        protected internal bool Prepare()
        {
            try
            {
                if (IsPrepared) return true;
                AddEvent("Подготовка выходов");
                ClearOuts();
                foreach (var sig in ReceiverConnect.Signals.Values)
                {
                    var ob = AddOut(sig);
                    ob.Context = sig.CodeOut;
                    ob.AddSignal(sig);
                }
                Procent = 20;
                if (!Connect()) return false;
                return IsPrepared = StartDanger(0, 100, 2, LoggerStability.Periodic, "Подготовка провайдера")
                                    .Run(PrepareProvider, Reconnect).IsSuccess;
            }
            catch (Exception ex)
            {
                AddError("Ошибка при подготовке источника", ex);
                return false;
            }
        }

        //Очистка списков объектов
        protected internal abstract void ClearOuts();
        //Добавить объект содержащий заданный сигнал
        protected abstract ReceiverOut AddOut(ReceiverSignal sig);

        //Запись значений в приемник
        protected internal abstract void WriteValues();
    }
}