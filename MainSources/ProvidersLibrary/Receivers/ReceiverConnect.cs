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
        public ReceiverBase CurReceiver { get { return (ReceiverBase)CurProvider; } }

        //Словарь сигналов приемников, ключи - коды
        protected readonly DicS<ReceiverSignal> ProviderSignals = new DicS<ReceiverSignal>();
        public IDicSForRead<ReceiverSignal> Signals { get { return ProviderSignals; } }

        //Добавить сигнал
        public ReceiverSignal AddSignal(string fullCode, //Полный код сигнала
                                                     string codeObject, //Код объекта
                                                     DataType dataType, //Тип данных
                                                     string signalInf) //Настройки сигнала
        {
            if (ProviderSignals.ContainsKey(fullCode))
                return ProviderSignals[fullCode];
            return ProviderSignals.Add(fullCode, new ReceiverSignal(this, fullCode, codeObject, dataType, signalInf));
        }

        //Очистка списка сигналов
        public void ClearSignals()
        {
            ProviderSignals.Clear();
        }

        //Запись значений в приемник
         public bool WriteValues(DateTime periodBegin, DateTime periodEnd)
        {
            using (Start())
            {
                PeriodBegin = periodBegin;
                PeriodEnd = periodEnd;

                using (Start(5, 80))
                    if (WriteValues()) return true;

                if (ChangeCurProvider())
                    using (Start(80, 100))
                        return WriteValues();
                return false;    
            }
        }

        //Чтение значений из источника
        public bool WriteValues()
        {
            try
            {
                if (!CurReceiver.Connect(false))
                    return false;
                if (!CurReceiver.IsPrepared)
                    CurReceiver.Prepare();
                using (Start(0, PeriodBegin < PeriodEnd ? 30 : 100))
                    CurReceiver.WriteValues();
                AddEvent("Значения записаны в приемник");
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