﻿using BaseLibrary;
using ProcessingLibrary;
using ProvidersLibrary;

namespace AppLibrary
{
    //Внешнее приложение, вызывающее библиотеки
    public class App : ProcessApp
    {
        public App(string code, IIndicator indicator) 
            : base(code, indicator) { }

        //Создание соединения-клонера и присоединение провайдера
        public ClonerConnect LoadCloner(string providerCode, string providerInf)
        {
            _cloner = new ClonerConnect(this);
            _cloner.JoinProvider(ProvidersFactory.CreateProvider(this, providerCode, providerInf));
            return _cloner;
        }
        private ClonerConnect _cloner;

        //Очистка ресурсов
        protected override void DisposeLogger()
        {
                   
        }
    }
}