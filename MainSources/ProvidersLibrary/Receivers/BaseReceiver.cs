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
        internal void Prepare()
        {
            try
            {
                AddEvent("Подготовка приемника");
                ClearObjects();
                foreach (var sig in ReceiverConnect.Signals.Values)
                {
                    var ob = AddObject(sig);
                    ob.Context = sig.CodeOuts;
                    ob.AddSignal(sig);
                }
                Procent = 30;
                StartDanger(30, 100, 2, LoggerStability.Single, "Ошибка при подготовке приемника", "Повторная подготовка приемника")
                    .Run(() => PrepareReceiver(), () => Reconnect());
            }
            catch (Exception ex)
            {
                AddError("Ошибка при подготовке источника", ex);
            }
        }

        //Очистка списков объектов
        protected abstract void ClearObjects();
        //Добавить объект содержащий заданный сигнал
        protected abstract ReceiverObject AddObject(ReceiverSignal sig);
        //Подготовка источника
        protected virtual void PrepareReceiver() { }

        //Запись значений в приемник
        protected internal abstract void WriteValues();
    }
}