using CommonTypes;

namespace Provider
{
    //Объект
    internal class ObjectSimatic : SourceObject
    {
        internal ObjectSimatic(string archive, string code, int id) : base(code)
        {
            Id = id;
            Archive = archive;
            FullCode = Archive + @"\" + Code;
        }

        //Сигналы: качество, флаги
        internal SourceSignal QualitySignal { get; set; }
        internal SourceSignal FlagsSignal { get; set; }
        
        //Имя архива
        internal string Archive { get; private set; }
        //Полное имя архивного тэга 
        internal string FullCode { get; private set; }
        //Id в таблице архива
        public int Id { get; private set; }

        public override SourceSignal AddSignal(SourceSignal sig)
        {
            switch (sig.Inf["Prop"].ToLower())
            {
                case "quality":
                    return QualitySignal = QualitySignal ?? sig;
                case "flags":
                    return FlagsSignal = FlagsSignal ?? sig;
                default:
                    return ValueSignal = ValueSignal ?? sig;
            }
        }

        //Возвращает, есть ли у объекта неопределенные срезы
        public override bool HasBegin
        {
            get { return SignalsHasBegin(ValueSignal, QualitySignal, FlagsSignal); }
        }

        //Добавляет в сигналы объекта срез, если возможно, возвращает, сколько добавлено значений
        public override int AddBegin()
        {
            return SignalsAddBegin(ValueSignal, QualitySignal, FlagsSignal);
        }
    }
}