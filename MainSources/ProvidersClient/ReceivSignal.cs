using System;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace ProvidersClient
{
    //Сигнал приемника для внешнего использования через COM
    //Обертка над ReceiverSignal
    public class ReceivSignal
    {
        internal ReceivSignal(ReceiverSignal signal)
        {
            _signal = signal;
        }

        //Ссылка на сигнал
        private readonly ReceiverSignal _signal;

        //Полный код сигнала
        public string Code { get { return _signal.Code; } }
        //Тип данных в виде строки
        public string DataType { get { return _signal.DataType.ToRussian(); } }
        //Строка свойств
        public string Inf { get { return _signal.Inf.ToPropertyString(); } }

        //Значения разного типа для записи
        public bool Boolean { get; set; }
        public int Integer { get; set; }
        public double Real { get; set; }
        public DateTime Date { get; set; }
        public string String { get; set; }
    }
}