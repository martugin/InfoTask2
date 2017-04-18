using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Соединение - приемник
    public class ReceiverConnect : ProviderConnect
    {
        protected ReceiverConnect(string name, string complect, Logger logger) 
            : base(name, complect, logger) { }

        //Тип провайдера
        public override ProviderType Type
        {
            get { return ProviderType.Receiver; }
        }
        //Текущий провайдер приемника
        internal Receiver Receiver
        {
            get { return (Receiver)Provider; }
        }
       
        //Добавить сигнал приемника
        protected override ProviderSignal AddConcreteSignal(string fullCode, DataType dataType, SignalType signalType, string contextOut, DicS<string> inf)
        {
            return new ReceiverSignal(this, fullCode, dataType, contextOut, inf);
        }

        //Запись значений в приемник
        public void WriteValues() 
        {
            if (PeriodIsUndefined()) return;
            if (Start(0, 80).Run(PutValues).IsSuccess) return;

            if (ChangeProvider())
                using (Start(80, 100))
                    PutValues();
        }

        //Запись значений в приемник
        protected void PutValues()
        {
            try
            {
                using (Start(0, 10))
                    if (!Receiver.Connect() || !Receiver.Prepare()) return;

                using (Start(10, 100))
                {
                    AddEvent("Запись значений в приемник");
                    Receiver.WriteValues();
                    AddEvent("Значения записаны в приемник");
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при записи значений в приемник", ex);
            }
        }
    }
}