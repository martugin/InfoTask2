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
        protected internal abstract ValuesCount ReadChanges();

        //Очистка значений сигналов
        protected override void ClearSignalsValues()
        {
            AddEvent("Очистка значений сигналов");
            foreach (ListSignal sig in SourceConnect.ReadingSignals.Values)
                sig.ClearMoments();
        }

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
                ValuesCount changes;
                using (Start(40, 85))
                    changes = ReadChanges();
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