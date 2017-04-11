using System.Collections.Generic;
using System.Linq;
using BaseLibrary;
using ProvidersLibrary;

namespace Logika
{
    internal class LogikaOut : SourceOut
    {
        internal LogikaOut(BaseSource source) 
            : base(source) { }

        //Словарь сигналов выхода
        private readonly Dictionary<string, InitialSignal> _signals = new Dictionary<string, InitialSignal>();

        //Сигнал только добавляется в Signals
        protected override InitialSignal AddNewSignal(InitialSignal sig)
        {
            var code = sig.Inf["SignalCode"];
            if (!_signals.ContainsKey(code))
                _signals.Add(code, sig);
            return sig;
        }

        //Чтение из одной строчки значений
        protected override int ReadMoments(IRecordRead rec)
        {
            var t = rec.GetTime("Время");
            return _signals.Sum(sig => AddMomReal(sig.Value, t, rec, sig.Key));
        }
    }
}