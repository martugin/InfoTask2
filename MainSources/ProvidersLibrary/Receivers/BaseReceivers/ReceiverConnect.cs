using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Соединение - приемник
    public abstract class ReceiverConnect : ProviderConnect
    {
        protected ReceiverConnect(string name, string complect, Logger logger) 
            : base(name, complect, logger) { }

        //Тип провайдера
        public override ProviderType Type { get { return ProviderType.Receiver; } }
        
        //Словарь сигналов приемников, ключи - коды
        public IDicSForRead<IWriteSignal> Signals { get { return ProviderSignals; } }

        //Добавить сигнал
        public IWriteSignal AddSignal(string fullCode, //Полный код сигнала
                                                                     DataType dataType, //Тип данных
                                                                     string infObject, //Свойства объекта
                                                                     string infOut = "", //Свойства выхода относительно объекта
                                                                     string infProp = "") //Свойства сигнала относительно выхода
        {
            return AddProviderSignal(fullCode, dataType, infObject, infOut, infProp);
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
        protected abstract void PutValues();
    }
}