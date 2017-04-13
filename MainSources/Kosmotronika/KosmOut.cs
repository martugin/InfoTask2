using System;
using BaseLibrary;
using ProvidersLibrary;

namespace Kosmotronika
{
    //Структура для индекса словаря сигналов
    internal struct OutIndex
    {
        public OutIndex(int sn, int numType, int appartment, int numOut)
        {
            Sn = sn;
            NumType = numType;
            Appartment = appartment;
            Out = numOut;
        }

        //Системный номер
        internal int Sn;
        //Номер типа
        internal int NumType;
        //Подразделение
        internal int Appartment;
        //Номер выхода
        internal int Out;
    }

    //---------------------------------------------------------------------------------------------------------------------------------
    //Один выход для непосредственного считывания с архива космотроники
    //Для аналоговых - один ТМ, для выходов - один выход ТМ
    internal class KosmOut : ListSourceOut
    {
        internal KosmOut(KosmotronikaBaseSource source, OutIndex ind) : base(source)
        {
            Sn = ind.Sn; 
            NumType = ind.NumType;
            Appartment = ind.Appartment;
            Out = ind.Out;
        }

        //Добавить к выходу сигнал, если такого еще не было
        protected override InitialSignal AddInitialSignal(InitialSignal sig)
        {
            switch (sig.Inf["Prop"].ToUpper())
            {
                case "ND":
                    return StateSignal = StateSignal ?? sig;
                case "POK":
                    return PokSignal = PokSignal ?? sig;
            }
            return ValueSignal = ValueSignal ?? sig;
        }
        
        //Системный номер
        internal int Sn { get; private set; }
        //Номер типа
        internal int NumType { get; private set; }
        //Подразделение
        internal int Appartment { get; private set; }
        //Номер выхода
        internal int Out { get; private set; }

        //Сигнал недостоверности
        internal InitialSignal StateSignal { get; private set; }
        //Сигнал ПОК
        internal InitialSignal PokSignal { get; private set; }

        //Чтение значений по одному выходу из рекордсета источника
        //Возвращает количество сформированных значений
        protected override int ReadMoments(IRecordRead rec)
        {
            int dn = Source is KosmotronikaRetroSource ? 1 : 0;
            int nwrite = 0;
            DateTime time = rec.GetTime(2+dn);
            var isAnalog = ((KosmotronikaBaseSource)Source).IsAnalog;
            int ndint = rec.GetInt(isAnalog ? 5+dn : 7+dn);
            var err = MakeError(ndint);

            nwrite += AddMom(PokSignal, time, rec.GetInt(4+dn), err);
            nwrite += AddMom(StateSignal, time, ndint);
            if (isAnalog)
                nwrite += AddMom(ValueSignal, time, rec.GetDouble(7+dn), err);
            else
            {
                var strValue = rec.GetString(8+dn);
                if (strValue.IndexOf("0x", StringComparison.Ordinal) >= 0)
                    nwrite += AddMom(ValueSignal, time, Convert.ToUInt32(strValue, 16), err);
                else nwrite += AddMom(ValueSignal, time, Convert.ToDouble(strValue), err);
            }
            return nwrite;
        }
    }
}