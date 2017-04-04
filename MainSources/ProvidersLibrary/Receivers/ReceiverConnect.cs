using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Соединение - приемник
    public class ReceiverConnect : ProviderConnect
    {
        public ReceiverConnect(string name, string complect, Logger logger) 
            : base(name, complect, logger) { }

        //Тип провайдера
        public override ProviderType Type { get { return ProviderType.Receiver; } }

        //Текущий провайдер источника
        internal BaseReceiver Receiver { get { return (BaseReceiver)Provider; } }

        //Словарь сигналов приемников, ключи - коды
        private readonly DicS<ReceiverSignal> _signals = new DicS<ReceiverSignal>();
        public IDicSForRead<ReceiverSignal> Signals { get { return _signals; } }

        //Добавить сигнал
        public ReceiverSignal AddSignal(string fullCode, //Полный код сигнала
                                                         DataType dataType, //Тип данных
                                                         string infObject, //Свойства объекта
                                                         string infOut, //Свойства выхода относительно объекта
                                                         string infProp) //Свойства сигнала относительно выхода
        {
            if (_signals.ContainsKey(fullCode))
                return _signals[fullCode];
            Provider.IsPrepared = false;
            return _signals.Add(fullCode, new ReceiverSignal(this, fullCode, dataType, infObject, infOut, infProp));
        }

        //Очистка списка сигналов
        public void ClearSignals()
        {
            AddEvent("Очистка списка сигналов");
            Receiver.IsPrepared = false;
            Receiver.ClearOuts();
            _signals.Clear();
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
        private void PutValues()
        {
            try
            {
                using (Start(0, 10))
                    if (!Receiver.Connect() || !Receiver.Prepare()) return ;

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