﻿using System;
using CommonTypes;
using ProvidersLibrary;

namespace ComClients
{
    //Соединение с приемником для взаимодействия через COM
    public class ReceivConnect : ProvConnect
    {
        internal ReceivConnect(ReceiverConnect connect, ProvidersFactory factory) 
            : base(connect, factory) {}
           
        //Ссылка на соединение
        internal ReceiverConnect Connect
        {
            get { return (ReceiverConnect) ProviderConnect; }
        }

        //Тип провайдера
        protected override ProviderType Type { get { return ProviderType.Receiver;}}

        //Очистка списка сигналов
        public void ClearSignals()
        {
            Connect.ClearSignals();
        }

        //Добавить сигнал
        public ReceivSignal AddSignal(string fullCode, //Полный код сигнала
                                                      string codeOut, //Код выхода
                                                      string dataType, //Тип данных
                                                      string signalInf) //Настройки сигнала
        {
            return new ReceivSignal(Connect.AddSignal(fullCode, codeOut, dataType.ToDataType(), signalInf));
        }

        //Запись значений в приемник
        public void WriteValues(DateTime periodBegin, DateTime periodEnd)
        {
            RunLongCommand(() => Connect.WriteValues(periodBegin, periodEnd));
        }
        public void WriteValues()
        {
            RunLongCommand(() => Connect.WriteValues());
        }
    }
}