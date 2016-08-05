using System.Collections.Generic;
using System.Linq;
using BaseLibrary;
using ProvidersLibrary;

namespace Logika
{
    public class ObjectProlog : SourceObject
    {
        public ObjectProlog(SourceBase source) 
            : base(source) { }

        //Словарь сигналов объекта
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
        protected internal override int ReadMoments(IRecordRead rec)
        {
            var t = rec.GetTime("Время");
            return _signals.Sum(sig => AddMom(sig.Value, t, rec.GetDouble(sig.Key)));
        }
    }
}