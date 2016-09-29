using System;

namespace ProvidersLibrary
{
    //Провайдер - приемник
    public abstract class ReceiverBase : ProviderBase
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
                using (Start())
                {
                    ClearObjects();
                    foreach (var sig in ReceiverConnect.Signals.Values)
                    {
                        var ob = AddObject(sig);
                        ob.Context = sig.CodeObject;
                        ob.AddSignal(sig);
                    }
                    Procent = 30;
                    Danger(PrepareReceiver, 2, 0, "Ошибка при подготовке приемника", Reconnect);
                }
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
        internal protected abstract void WriteValues();
    }
}