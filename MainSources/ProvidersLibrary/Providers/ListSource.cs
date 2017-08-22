using System;
using System.Diagnostics.Eventing.Reader;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Базовый класс для всех источников чтения из архива
    public abstract class ListSource : Source
    {
        //Тип сигналов
        public override SignalType SignalType
        {
            get { return SignalType.List; }
        }

        //Чтение среза, возврашает количество прочитанных значений
        protected internal virtual ValuesCount ReadCut() { return new ValuesCount(); }
        //Чтение изменений, возврашает количество прочитанных и сформированных значений
        protected internal abstract ValuesCount ReadChanges(DateTime beg, DateTime en);
        //Ограничение на длину интервала для одного считывания
        private readonly TimeSpan _periodLimit = new TimeSpan(10000, 0, 0, 0);
        protected virtual TimeSpan PeriodLimit { get { return _periodLimit; } }

        //Чтение значений из провайдера
        protected override ValuesCount ReadProviderValues()
        {
            var vcount = new ValuesCount();
            AddEvent("Чтение среза значений");
            using (Start(10, PeriodBegin == PeriodEnd ? 90 : 40))
                vcount += ReadCut();
            foreach (ListSignal sig in SourceConnect.InitialSignals.Values)
                vcount.WriteCount += sig.MakeBegin();
            AddEvent("Срез значений получен", vcount.ToString());
            if (vcount.Status == VcStatus.Fail)
                return vcount;

            //Чтение изменений
            if (PeriodBegin < PeriodEnd)
            {
                AddEvent("Чтение изменений значений");
                var ts = PeriodEnd.Subtract(PeriodBegin).Subtract(new TimeSpan(0, 0, 0, 0, 1));
                var tts = new TimeSpan(0);
                int n = 0;
                while (tts < ts)
                {
                    tts = tts.Add(PeriodLimit);
                    n++;
                }

                var changes = new ValuesCount();
                using (Start(40, 85))
                {
                    DateTime beg = PeriodBegin;
                    DateTime en = Static.MinDate;
                    double proc = 0;
                    while (en < PeriodEnd)
                        using (Start(proc, proc += 100.0 / n))
                        {
                            en = beg.Add(PeriodLimit);
                            if (PeriodEnd < en) en = PeriodEnd;
                            changes = changes + ReadChanges(beg, en);
                            beg = en;    
                        }
                }
                foreach (ListSignal sig in SourceConnect.InitialSignals.Values)
                    changes.WriteCount += sig.MakeEnd();
                AddEvent("Изменения значений получены", changes.ToString());
                vcount += changes;
                if (vcount.IsFail) return vcount;
                Procent = 90;
            }
            return vcount;
        }
    }
}