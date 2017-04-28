using System;
using CommonTypes;

namespace ProcessingLibrary
{
    //Сигнал для прокси
    public class ProxySignal : IWriteSignal, IReadSignal
    {
        public ProxySignal(ISignal signal)
        {
            _signal = signal;
        }

        //Базовый сигнал
        private readonly ISignal _signal;
        //Код и тип данных
        public string Code { get { return _signal.Code; } }
        public DataType DataType { get { return _signal.DataType; } }

        //Передать значение
        public void PutValue(IReadMean value)
        {
            throw new NotImplementedException();
        }

        //Забрать значение
        public IReadMean GetValue()
        {
            throw new NotImplementedException();
        }
    }
}