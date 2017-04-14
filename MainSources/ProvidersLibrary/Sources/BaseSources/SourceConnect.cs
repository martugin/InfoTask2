using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Соединение-источник
    public abstract class SourceConnect : ProviderConnect
    {
        protected SourceConnect(string name, string complect, Logger logger) 
            : base(name, complect, logger) { }

        //Тип провайдера
        public override ProviderType Type { get { return ProviderType.Source;}}
        
        //Список сигналов, содержащих возвращаемые значения
        public IDicSForRead<ISourceSignal> Signals { get { return ProviderSignals; } }

        //Добавить сигнал
        public ISourceSignal AddSignal(string fullCode, //Полный код сигнала
                                                        DataType dataType, //Тип данных
                                                        string infObject, //Свойства объекта
                                                        string infOut, //Свойства выхода относительно объекта
                                                        string infProp = "") //Свойства сигнала относительно выхода
        {
            return AddProviderSignal(fullCode, dataType, infObject, infOut, infProp);
        }

        //Чтение значений из источника, возвращает true, если прочитались все значения или частично
        //В логгере дожны быть заданы начало и конец периода через CommandProgress
        public ValuesCount GetValues()
        {
            var vc = new ValuesCount();
            if (PeriodIsUndefined()) return vc;
            using (Start(0, 80))
            {
                vc += ReadValues();
                if (!vc.IsFail) return vc;
            }

            if (!ChangeProvider()) return vc;
            using (Start(80, 100))
                return ReadValues();
        }

        //Чтение значений из источника
        protected abstract ValuesCount ReadValues();
    }
}