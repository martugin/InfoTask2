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
        private readonly Dictionary<string, UniformSignal> _signals = new Dictionary<string, UniformSignal>();

        //Сигнал только добавляется в Signals
        protected override UniformSignal AddNewSignal(UniformSignal sig)
        {
            var code = sig.Inf["SignalCode"];
            if (!_signals.ContainsKey(code))
                _signals.Add(code, sig);
            return sig;
        }

        //Чтение из одной строчки значений
        public override int ReadMoments(IRecordRead rec)
        {
            return _signals.Sum(sig => AddMom(sig.Value, rec.GetTime("Время"), rec.GetDouble(sig.Key)));
        }
    }
}