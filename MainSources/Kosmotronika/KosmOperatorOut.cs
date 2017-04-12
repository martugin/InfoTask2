using System;
using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    //Выход для считывания действий оператора Космотроники
    internal class KosmOperatorOut : ListSourceOut
    {
        internal KosmOperatorOut(ListSource source) : base(source) { }

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
            switch (sig.Inf["Prop"].ToLower())
            {
                case "numws":
                    return NumWsSignal = NumWsSignal ?? sig;
                case "mode":
                    return ModeSignal = ModeSignal ?? sig;
                case "code":
                    return CodeSignal = CodeSignal ?? sig;
                case "sn":
                    return SnSignal = SnSignal ?? sig;
                case "numtype":
                    return NumTypeSignal = NumTypeSignal ?? sig;
                case "appartment":
                    return AppartmentSignal = AppartmentSignal ?? sig;
                case "params":
                    return ParamsSignal = ParamsSignal ?? sig;
                case "extcommand":
                    return ExtCommandSignal = ExtCommandSignal ?? sig;
                case "point":
                    return PointSignal = PointSignal ?? sig;
            }
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