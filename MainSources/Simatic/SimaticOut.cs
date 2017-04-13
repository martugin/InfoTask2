using System;
using BaseLibrary;
using ProvidersLibrary;

namespace Simatic
{
    //Объект
    internal class SimaticOut : ListSourceOut
    {
        internal SimaticOut(SimaticSource source, string archive, string tag, int id) : base(source)
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
        internal int Id { get; private set; }

        //Добавление сигнала
        protected override InitialSignal AddInitialSignal(InitialSignal sig)
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

        //Чтение значений по одному выходу из рекордсета источника и добавление их в список или клон
        //Возвращает количество сформированных значений
        protected override int ReadMoments(IRecordRead rec)
        {
            DateTime time = rec.GetTime("TimeStamp").ToLocalTime();
            var quality = rec.GetInt("Quality");
            var err = MakeError(quality);
            return AddMom(FlagsSignal, time, rec.GetInt("Flags"), err) +
                      AddMom(QualitySignal, time, quality, err) +
                      AddMom(ValueSignal, time, ((AdoReader)rec).Reader["RealValue"], err);
        }
    }
}