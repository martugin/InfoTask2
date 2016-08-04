using System;

namespace ProvidersLibrary
{
    //Провайдер - приемник
    public abstract class ReceiverBase : ProviderBase
    {
        //Ссылка на соединение
        public ReceiverConnect ReceiverConnect
        {
            get { return (ReceiverConnect)ProviderConnect; }
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
                    foreach (var sig in ReceiverConnect.Signals.Values)
                    {
                        var ob = AddObject(sig);
                        ob.Context = sig.CodeObject;
                        ob.AddSignal(sig);
                    }
                    Procent = 30;
                    Danger()
                    
                    try { PrepareReceiver();}
                    catch (Exception ex)
                    {
                        AddError("Ошибка при подготовке источника. Повторный запуск", ex);
                        PrepareReceiver();
                    }
                    
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
        internal protected abstract ReceiverObject AddObject(ReceiverSignal sig);
        //Подготовка источника
        public virtual void PrepareReceiver() { }

        //Запись значений в приемник
        protected internal abstract void WriteValues();
    }
}