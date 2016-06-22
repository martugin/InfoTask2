using System;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Базовый класс для соединений с источниками
    public abstract class SourceConnect : ProviderConnect, ISourceConnect
    {
        protected SourceConnect() { }
        protected SourceConnect(string name, Logger logger) : base(name, logger) { }

        //Тип провайдера
        public override ProviderType Type { get { return ProviderType.Source; } }

        //Начало диапазона источника
        protected DateTime BeginTime { get; set; }
        //Конец диапазона источника
        protected DateTime EndTime { get; set; }

        //Получение диапазона архива 
        public virtual TimeInterval GetTime()
        {
            return new TimeInterval(Different.MinDate.AddYears(1), DateTime.Now);
        }
    }
}