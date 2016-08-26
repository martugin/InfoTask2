using System;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace ComClients
{
    //Сигнал источника для внешнего использования через COM
    //Обертка над SourceSignal
    public class SourSignal
    {
        internal SourSignal(SourceSignal signal)
        {
            _signal = signal;
        }

        //Ссылка на сигнал
        private readonly SourceSignal _signal;

        //Полный код сигнала
        public string Code { get { return _signal.Code; } }
        //Тип данных в виде строки
        public string DataType { get { return _signal.DataType.ToRussian(); } }
        //Строка свойств
        public string Inf { get { return _signal.Inf.ToPropertyString(); } }

        //Количество значений
        public int MomsCount { get { return _signal.MomList.Count; } }
        //Время i-ого значения
        public DateTime Time(int i)
        {
            return _signal.MomList.Time(i);
        }

        //Качество ошибки i-ого значения
        public int ErrQuality(int i)
        {
            if (_signal.MomList.Error(i) == null)
                return 0;
            return (int)_signal.MomList.Error(i).Quality;
        }
        //Номер ошибки i-ого значения
        public int ErrNumber(int i)
        {
            if (_signal.MomList.Error(i) == null)
                return 0;
            return _signal.MomList.Error(i).Number;
        }
        //Текст ошибки i-ого значения
        public string ErrText(int i)
        {
            if (_signal.MomList.Error(i) == null)
                return null;
            return _signal.MomList.Error(i).Text;
        }

        //Значения разного типа
        public bool Boolean(int i)
        {
            return _signal.MomList.Boolean(i);
        }
        public int Integer(int i)
        {
            return _signal.MomList.Integer(i);
        }
        public double Real(int i)
        {
            return _signal.MomList.Real(i);
        }
        public DateTime Date(int i)
        {
            return _signal.MomList.Date(i);
        }
        public string String(int i)
        {
            return _signal.MomList.String(i);
        }
    }
}