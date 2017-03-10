using System;

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
        protected internal override bool Prepare()
        {
            try
            {
                AddEvent("Подготовка выходов");
                ClearOuts();
                foreach (var sig in ReceiverConnect.Signals.Values)
                {
                    var ob = AddOut(sig);
                    ob.Context = sig.CodeOuts;
                    ob.AddSignal(sig);
                }
                using (Start(20, 100))
                    return BasePrepare();
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
        protected abstract ReceiverOut AddOut(ReceiverSignal sig);

        //Запись значений в приемник
        protected internal abstract void WriteValues();
    }
}