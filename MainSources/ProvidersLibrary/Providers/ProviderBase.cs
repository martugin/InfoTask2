using System;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Базовый класс для всех провайдеров
    public abstract class ProviderBase : ExternalLogger, IProvider
    {
        protected ProviderBase() 
            : base(new Logger())
        {
            MakeConnect(null, Logger);
        }
        protected ProviderBase(string name, Logger logger)
            : base(logger)
        {
            MakeConnect(name, logger);
        }

        //Соединение 
        public IProviderConnect ProviderConnect { get; protected set; }
        //Резервное соединение 
        public IProviderConnect ReserveConnect { get; protected set; }

        //Связать провайдер с соединением
        protected abstract void MakeConnect(string name, Logger logger);
        //Комплект
        public abstract string Complect { get; }

        //Контекст и имя объекта для записи комманд и ошибок, заданные по умолчанию
        public override string Context
        {
            get { return ProviderConnect.Context; }
        }

        //Подготовка, вызывается при загрузке проекта
        public virtual void Prepare() { }

        //Текущий период расчета, задается при чтениии из провайдера или при записи в провайдер
        public DateTime PeriodBegin { get; protected set; }
        public DateTime PeriodEnd { get; protected set; }

        //Очистка ресурсов
        public virtual void Dispose()
        {
            try
            {
                if (ProviderConnect != null)
                    ProviderConnect.Dispose();
                ProviderConnect = null;    
            }
            catch { }
        }
    }
}
