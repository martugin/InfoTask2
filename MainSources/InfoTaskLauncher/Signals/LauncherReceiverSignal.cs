using System;
using System.Runtime.InteropServices;
using CommonTypes;

namespace ComLaunchers
{
    //Интерфейс для LauncherReceiverSignal
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ILauncherReceiverSignal
    {
        string Code { get; }
        string DataType { get; }

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
    public class LauncherReceiverSignal : ILauncherReceiverSignal
    {
        internal LauncherReceiverSignal(IWriteSignal signal)
        {
            _signal = signal;
        }

        //Ссылка на сигнал
        private readonly IWriteSignal _signal;

        //Полный код сигнала
        public string Code { get { return _signal.Code; } }
        //Тип данных в виде строки
        public string DataType { get { return _signal.DataType.ToRussian(); } }

        //Значения разного типа для записи
        public bool Boolean { get; set; }
        public int Integer { get; set; }
        public double Real { get; set; }
        public DateTime Date { get; set; }
        public string String { get; set; }
    }
}