using System;
using CommonTypes;

namespace ComLaunchers
{
    //Сигнал источника реального времени для внешнего использования через COM
    public class RealTimeSignal
    {
        internal RealTimeSignal(string code, string dataType, string inf)
        {
            Code = code;
            DataType = dataType;
            Inf = inf;
        }

        //Полный код сигнала
        public string Code { get; private set; }
        //Тип данных в виде строки
        public string DataType { get; private set; }
        //Строка свойств
        public string Inf { get; private set; }

        //Текущее значение
        internal IMean Mom { get; set; }

        //Время значения
        public DateTime Time()
        {
            return Mom.Time;
        }

        //Качество ошибки
        public int ErrQuality()
        {
            return Mom == null ? 0 : (int) Mom.Error.Quality;
        }
        //Номер ошибки
        public int ErrNumber()
        {
            return Mom == null ? 0 : Mom.Error.Number;
        }
        //Текст ошибки
        public string ErrText()
        {
            return Mom == null ? null : Mom.Error.Text;
        }

        //Значения разного типа
        public bool Boolean()
        {
            return Mom.Boolean;
        }
        public int Integer()
        {
            return Mom.Integer;
        }
        public double Real()
        {
            return Mom.Real;
        }
        public DateTime Date()
        {
            return Mom.Date;
        }
        public string String()
        {
            return Mom.String;
        }
    }
}