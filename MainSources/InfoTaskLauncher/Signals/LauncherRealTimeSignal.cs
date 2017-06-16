using System;
using System.Runtime.InteropServices;
using CommonTypes;

namespace ComLaunchers
{
    //Интерфейс для LauncherRealTimeSignal
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ILauncherRealTimeSignal
    {
        string Code { get; }
        string DataType { get; }

        DateTime Time { get; }
        int ErrQuality { get; }
        int ErrNumber { get; }
        string ErrText { get; }

        bool Boolean { get; }
        int Integer { get; }
        double Real { get; }
        DateTime Date { get; }
        string String { get; }
    }

    //--------------------------------------------------------------------------------------------------------

    //Сигнал источника реального времени для внешнего использования через COM
    //Обертка над IReadSignal
    [ClassInterface(ClassInterfaceType.None)]
    public class LauncherRealTimeSignal : ILauncherRealTimeSignal
    {
        internal LauncherRealTimeSignal(IReadSignal signal)
        {
            _signal = signal;
        }

        //Ссылка на сигнал
        private readonly IReadSignal _signal;

        //Полный код сигнала
        public string Code { get { return _signal.Code; } }
        //Тип данных в виде строки
        public string DataType { get { return _signal.DataType.ToRussian(); } }

        //Время значения
        public DateTime Time { get { return _signal.OutValue.Time; }}
        
        //Качество ошибки
        public int ErrQuality
        {
            get { return _signal.OutValue.Error == null ? 0 : (int) _signal.OutValue.Error.Quality;    }
        }
        //Номер ошибки
        public int ErrNumber
        {
            get { return _signal.OutValue.Error == null ? 0 : _signal.OutValue.Error.Number; }
        }
        //Текст ошибки
        public string ErrText
        {
            get { return _signal.OutValue.Error == null ? null : _signal.OutValue.Error.Text; }
        }

        //Значения разного типа
        public bool Boolean
        {
            get { return _signal.OutValue.Boolean; }
        }
        public int Integer
        {
            get { return _signal.OutValue.Integer; }
        }
        public double Real
        {
            get { return _signal.OutValue.Real; }
        }
        public DateTime Date
        {
            get { return _signal.OutValue.Date; }
        }
        public string String
        {
            get { return _signal.OutValue.String; }
        }
    }
}