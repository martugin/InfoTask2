using System;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Базовый класс для всех соединений
    public abstract class ProvConn : ExternalLogger, IDisposable
    {
        //Код соединения
        public string Name { get; set; }
        //Тип соединения
        public abstract ProviderType Type { get; }
        //Комплект провайдеров, допустимых длясоединения
        public abstract string Complect { get; }

        //Контекст для логгера
        public override string CodeObject
        {
            get { return Type.ToRussian() + ": " + Name + ", " + Complect; }
        }

        //Настройка провайдера (через форму), возвращает строку с новыми настройками
        public virtual string Setup()
        {
            throw new NotImplementedException();
        }

        //Соединения с главным и резервным провайдером
        public ProvBase MainProvider { get; set; }
        public ProvBase ReserveProvider { get; set; }

        //Текущий период расчета
        public DateTime PeriodBegin { get; protected set; }
        public DateTime PeriodEnd { get; protected set; }

        public virtual void Dispose() { }
    }
}