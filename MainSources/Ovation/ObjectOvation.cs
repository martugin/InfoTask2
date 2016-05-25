using CommonTypes;

namespace Provider
{
    //Один объект (дисктретная, аналоговая или упакованная точка)
    internal class ObjectOvation : SourceObject
    {
        internal ObjectOvation(int id, string code) : base(code)
        {
            Id = id;
            Inf = code + "; Id=" + Id;
        }

        //Дискретный или аналоговый сигнал
        internal SignalOvation ValueSignal { get; set; }
        //Сигнал со словом состояния
        internal SignalOvation StateSignal { get; set; }
        //Id в Historian
        internal int Id { get; private set; }

        //Возвращает, есть ли у объекта неопределенные срезы на время time 
        public override bool HasBegin
        {
            get { return SignalsHasBegin(ValueSignal, StateSignal); }
        }

        //Добавляет в сигналы объекта срез, если возможно, возвращает, сколько добавлено значений
        public override int AddBegin()
        {
            return SignalsAddBegin(ValueSignal, StateSignal);
        }
    }
}