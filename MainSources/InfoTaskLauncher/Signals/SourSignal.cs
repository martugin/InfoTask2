﻿using System;
using System.Runtime.InteropServices;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace ComLaunchers
{
    //Интерфейс для RSourSignal
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface SourSignal
    {
        string Code { get; }
        string DataType { get; }
        string Inf { get; }

        int MomsCount { get; }
        DateTime Time(int i);
        int ErrQuality(int i);
        int ErrNumber(int i);
        string ErrText(int i);

        bool Boolean(int i);
        int Integer(int i);
        double Real(int i);
        DateTime Date(int i);
        string String(int i);
    }

    //--------------------------------------------------------------------------------------------------------

    //Сигнал источника для внешнего использования через COM
    //Обертка над ListSourceSignal
    [ClassInterface(ClassInterfaceType.None)]
    public class RSourSignal : SourSignal
    {
        internal RSourSignal(ListSignal signal)
        {
            _signal = signal;
        }

        //Ссылка на сигнал
        private readonly ListSignal _signal;

        //Полный код сигнала
        public string Code { get { return _signal.Code; } }
        //Тип данных в виде строки
        public string DataType { get { return _signal.DataType.ToRussian(); } }
        //Строка свойств
        public string Inf { get { return _signal.Inf.ToPropertyString(); } }

        //Количество значений
        public int MomsCount { get { return _signal.OutValue.Count; } }
        //Время i-ого значения
        public DateTime Time(int i)
        {
            return _signal.OutValue.TimeI(i);
        }

        //Качество ошибки i-ого значения
        public int ErrQuality(int i)
        {
            return _signal.OutValue.ErrorI(i) == null ? 0 : (int) _signal.OutValue.ErrorI(i).Quality;
        }
        //Номер ошибки i-ого значения
        public int ErrNumber(int i)
        {
            return _signal.OutValue.ErrorI(i) == null ? 0 : _signal.OutValue.ErrorI(i).Number;
        }
        //Текст ошибки i-ого значения
        public string ErrText(int i)
        {
            return _signal.OutValue.ErrorI(i) == null ? null : _signal.OutValue.ErrorI(i).Text;
        }

        //Значения разного типа
        public bool Boolean(int i)
        {
            return _signal.OutValue.BooleanI(i);
        }
        public int Integer(int i)
        {
            return _signal.OutValue.IntegerI(i);
        }
        public double Real(int i)
        {
            return _signal.OutValue.RealI(i);
        }
        public DateTime Date(int i)
        {
            return _signal.OutValue.DateI(i);
        }
        public string String(int i)
        {
            return _signal.OutValue.StringI(i);
        }
    }
}