using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для всех источников
    public abstract class Source : Provider
    {
        //Создать пул ошибок
        protected override void MakeErrPool()
        {
            if (ErrPool == null)
                ErrPool = new MomErrPool(MakeErrFactory());
        }

        //Создание фабрики ошибок
        protected virtual IMomErrFactory MakeErrFactory()
        {
            var factory = new MomErrFactory(ProviderConnect.Name, MomErrType.Source);
            factory.AddGoodDescr(0);
            return factory;
        }
        //Хранилище ошибок 
        internal MomErrPool ErrPool { get; private set; }

        //Создание ошибки 
        internal MomErr MakeError(int number, IContextable addr)
        {
            return ErrPool.MakeError(number, addr);
        }

        //Конец предыдущего периода чтения значений
        internal DateTime PrevPeriodEnd { get; set; }
    }
}