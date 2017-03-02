using System;
using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    //Выход для считывания действий оператора Космотроники
    internal class KosmOperatorOut : SourceOut
    {
        internal KosmOperatorOut(BaseSource source) : base(source) { }

        //Сигналы (поля таблицы действий оператора)
        //Номер рабочей станции
        internal InitialSignal NumWsSignal { get; private set; }
        //Режим
        internal InitialSignal ModeSignal { get; private set; }
        //Код 
        internal InitialSignal CodeSignal { get; private set; }
        //Системный номер
        internal InitialSignal SnSignal { get; private set; }
        //Номер типа
        internal InitialSignal NumTypeSignal { get; private set; }
        //Подразделение
        internal InitialSignal AppartmentSignal { get; private set; }
        //Параметры 
        internal InitialSignal ParamsSignal { get; private set; }
        //
        internal InitialSignal ExtCommandSignal { get; private set; }
        //
        internal InitialSignal PointSignal { get; private set; }
        
        //Добавить сигнал
        protected override InitialSignal AddNewSignal(InitialSignal sig)
        {
            if (sig.Inf["Prop"] == "NumWS")
                return NumWsSignal = NumWsSignal ?? sig;
            if (sig.Inf["Prop"] == "Mode")
                return ModeSignal = ModeSignal ?? sig;
            if (sig.Inf["Prop"] == "Code")
                return CodeSignal = CodeSignal ?? sig;
            if (sig.Inf["Prop"] == "SN")
                return SnSignal = SnSignal ?? sig;
            if (sig.Inf["Prop"] == "NumType")
                return NumTypeSignal = NumTypeSignal ?? sig;
            if (sig.Inf["Prop"] == "Appartment")
                return AppartmentSignal = AppartmentSignal ?? sig;
            if (sig.Inf["Prop"] == "Params")
                return ParamsSignal = ParamsSignal ?? sig;
            if (sig.Inf["Prop"] == "ExtCommand")
                return ExtCommandSignal = ExtCommandSignal ?? sig;
            if (sig.Inf["Prop"] == "Point")
                return PointSignal = PointSignal ?? sig;
            return null;
        }

        //Чтение значений по одному выходу из рекордсета источника
        protected override int ReadMoments(IRecordRead rec)
        {
            int dn = Source is KosmotronikaRetroSource ? 1 : 0;
            int nwrite = 0;
            DateTime time = rec.GetTime(0);
            nwrite += AddMom(NumWsSignal, time, rec.GetInt(2));
            nwrite += AddMom(ModeSignal, time, rec.GetInt(3));
            nwrite += AddMom(CodeSignal, time, rec.GetInt(4));
            nwrite += AddMom(SnSignal, time, rec.GetInt(5));
            nwrite += AddMom(NumTypeSignal, time, rec.GetInt(6));
            if (dn == 1) nwrite += AddMom(AppartmentSignal, time, rec.GetInt(7));
            nwrite += AddMom(ParamsSignal, time, rec.GetString(7+dn));
            nwrite += AddMom(ExtCommandSignal, time, rec.GetString(8+dn));
            nwrite += AddMom(PointSignal, time, rec.GetString(9+dn));
            return nwrite;
        }
    } 
}