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
        private ReceiverBase Receiver { get { return (ReceiverBase)Provider; } }

        //Словарь сигналов приемников, ключи - коды
        private readonly DicS<ReceiverSignal> _signals = new DicS<ReceiverSignal>();
        public IDicSForRead<ReceiverSignal> Signals { get { return _signals; } }

        //Добавить сигнал
        public ReceiverSignal AddSignal(string fullCode, //Полный код сигнала
                                                         string codeObject, //Код объекта
                                                         DataType dataType, //Тип данных
                                                         string signalInf) //Настройки сигнала
        {
            if (_signals.ContainsKey(fullCode))
                return _signals[fullCode];
            return _signals.Add(fullCode, new ReceiverSignal(this, fullCode, codeObject, dataType, signalInf));
        }

        //Очистка списка сигналов
        public void ClearSignals()
        {
            _signals.Clear();
        }

        //Приемник был подготовлен
        private bool _isPrepared;

        //Запись значений в приемник
        public bool WriteValues(DateTime periodBegin, DateTime periodEnd)
        {
            using (Start())
            {
                PeriodBegin = periodBegin;
                PeriodEnd = periodEnd;

                using (Start(5, 80))
                    if (WriteValuesReceiver()) return true;

                _isPrepared = false;
                if (ChangeProvider())
                    using (Start(80, 100))
                        return WriteValuesReceiver();
                return false;    
            }
        }
        public bool WriteValues()
        {
            return WriteValues(Different.MinDate, Different.MaxDate);
        }

        //Запись значений в приемник
        private bool WriteValuesReceiver()
        {
            try
            {
                if (!Receiver.Connect())
                    return false;
                if (!_isPrepared)
                {
                    Receiver.Prepare();
                    _isPrepared = true;
                }
                using (Start(0, PeriodBegin < PeriodEnd ? 30 : 100))
                {
                    AddEvent("Запись значений в приемник");
                    Receiver.WriteValues();
                    AddEvent("Значения записаны в приемник");
                }
                return true;
            }
            catch (Exception ex)
            {
                AddError("Ошибка при записи значений в приемник", ex);
                return false;
            }
        }
    }
}