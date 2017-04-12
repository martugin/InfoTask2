using System;
using System.Runtime.InteropServices;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace ComLaunchers
{
    //Интерфейс для RReceivSignal
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ReceivSignal
    {
        string Code { get; }
        string DataType { get; }
        string Inf { get; }

        bool Boolean { get; set; }
        int Integer { get; set; }
        double Real { get; set; }
        DateTime Date { get; set; }
        string String { get; set; }
    }
    
    //-------------------------------------------------------------------------------------------------

    //Сигнал приемника для внешнего использования через COM
    //Обертка над ReceiverSignal
    [ClassInterface(ClassInterfaceType.None)]
    public class RReceivSignal : ReceivSignal
    {
        internal RReceivSignal(ProviderSignal signal)
        {
            _signal = signal;
        }

        //Ссылка на сигнал
        private readonly ProviderSignal _signal;

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