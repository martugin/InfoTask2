﻿using System;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Соединение 
    public abstract class ProviderConnect : ExternalLogger
    {
        protected ProviderConnect(string name, string complect, Logger logger)
            : base(logger)
        {
            Name = name;
            Complect = complect;
        }
        
        //Код соединения
        public string Name { get; private set; }
        //Тип провайдера
        public abstract ProviderType Type { get; }
        //Комплект провайдеров
        public string Complect { get; private set; }
        //Контекст для логгера
        public override string Context
        {
            get { return Type.ToRussian() + ": " + Name; }
        }

        //Основной и резервный провайдеры
        public ProviderBase MainProvider { get; private set; }
        public ProviderBase ReserveProvider { get; private set; }
        //Текущий провайдер
        public ProviderBase CurProvider { get; private set; }

        //Присвоение основного и резервного провайдеров 
        public void JionProviders(ProviderBase mainProvider, ProviderBase reserveProvaider = null)
        {
            CurProvider = MainProvider = mainProvider;
            ReserveProvider = reserveProvaider;
        }

        //Переключение текущего провайдера, возвращает true, если переключение произошла
        protected bool ChangeCurProvider()
        {
            if (CurProvider == MainProvider && ReserveProvider != null)
            {
                CurProvider = ReserveProvider;
                AddEvent("Текущий провайдер изменен на резервный");
                return true;
            }
            if (CurProvider == ReserveProvider)
            {
                CurProvider = MainProvider;
                AddEvent("Текущий провайдер изменен на основной");
                return true;
            }
            return false;
        }

        //Вызов окна настройки
        public string Setup()
        {
            throw new NotImplementedException();
            //if (MenuCommands == null)
            //{
            //    MenuCommands = new DicS<Dictionary<string, IMenuCommand>>();
            //    AddMenuCommands();
            //}
            //IsSetup = true;
            //new ProviderSetupForm { Conn = this }.ShowDialog();
            //while (IsSetup) Thread.Sleep(500);
            //return ProviderInf;
        }

        //Подготовка провайдера
        public void Prepare()
        {
            try
            {
                CurProvider.Prepare();
            }
            catch (Exception ex)
            {
                AddError("Ошибка подготовки провайдера", ex);
            }
        }

        //Текущий период расчета
        public DateTime PeriodBegin { get; protected set; }
        public DateTime PeriodEnd { get; protected set; }
    }
}