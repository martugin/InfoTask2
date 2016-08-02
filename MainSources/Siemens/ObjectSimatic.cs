using System;
using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    //Объект
    internal class ObjectSimatic : SourceObject
    {
        internal ObjectSimatic(SimaticSource source, string archive, string tag, int id) : base(source)
        {
            Id = id;
            Archive = archive;
            FullCode = Archive + @"\" + tag;
        }

        //Сигналы: качество, флаги
        internal InitialSignal QualitySignal { get; set; }
        internal InitialSignal FlagsSignal { get; set; }
        
        //Имя архива
        internal string Archive { get; private set; }
        //Полное имя архивного тэга 
        internal string FullCode { get; private set; }
        //Id в таблице архива
        public int Id { get; private set; }

        //Добавление сигнала
        protected override InitialSignal AddNewSignal(InitialSignal sig)
        {
            switch (sig.Inf.Get("Prop", "").ToLower())
            {
                case "quality":
                    return QualitySignal = QualitySignal ?? sig;
                case "flags":
                    return FlagsSignal = FlagsSignal ?? sig;
                default:
                    return ValueSignal = ValueSignal ?? sig;
            }
        }

        //Чтение значений по одному объекту из рекордсета источника и добавление их в список или клон
        //Возвращает количество сформированных значений
        public override int ReadMoments(IRecordRead rec)
        {
            DateTime time = rec.GetTime("TimeStamp").ToLocalTime();
            var quality = rec.GetInt("Quality");
            var err = MakeError(quality);
            return AddMom(FlagsSignal, time, rec.GetInt("Flags"), err) +
                      AddMom(QualitySignal, time, quality, err) +
                      AddMom(ValueSignal, time, ((ReaderAdo)rec).Reader["RealValue"], err);
        }
    }
}