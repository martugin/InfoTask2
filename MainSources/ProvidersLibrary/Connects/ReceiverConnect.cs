using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Соединение - приемник
    public class ReceiverConnect : ProviderConnect, IWritingConnect
    {
        public ReceiverConnect(Logger logger, string code, string complect, string projectCode = "") 
            : base(logger, code, complect, projectCode) { }

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
       
        //Список сигналов, содержащих возвращаемые значения
        private readonly DicS<ReceiverSignal> _receiverSignals = new DicS<ReceiverSignal>();
        internal DicS<ReceiverSignal> ReceiverSignals {get { return _receiverSignals; }}
        public IDicSForRead<IWriteSignal> WritingSignals { get { return ReceiverSignals; } }

        //Добавить сигнал
        public ReceiverSignal AddSignal(string fullCode, //Полный код сигнала
                                                         DataType dataType, //Тип данных
                                                         SignalType signalType, //Тип сигнала
                                                         string infObject, //Свойства объекта
                                                         string infOut = "", //Свойства выхода относительно объекта
                                                         string infProp = "") //Свойства сигнала относительно выхода
        {
            if (ReceiverSignals.ContainsKey(fullCode))
                return ReceiverSignals[fullCode];
            Provider.IsPrepared = false;
            var contextOut = infObject + (infOut.IsEmpty() ? "" : ";" + infOut);
            var inf = infObject.ToPropertyDicS().AddDic(infOut.ToPropertyDicS()).AddDic(infProp.ToPropertyDicS());
            return ReceiverSignals.Add(fullCode, new ReceiverSignal(this, fullCode, dataType, contextOut, inf));
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