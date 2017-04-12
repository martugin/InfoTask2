using System;
using BaseLibrary;

namespace ProvidersLibrary
{
    public abstract class MomReceiverConnect : ReceiverConnect
    {
        protected MomReceiverConnect(string name, string complect, Logger logger) 
            : base(name, complect, logger) { }

        //Текущий провайдер источника
        internal MomReceiver Receiver { get { return (MomReceiver)Provider; } }
        
        //Запись значений в приемник
        protected override void PutValues()
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