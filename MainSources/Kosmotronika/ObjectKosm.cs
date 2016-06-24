using System;
using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    //Структура для индекса словаря сигналов
    internal struct ObjectIndex
    {
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
    //Один объект для непосредственного считывания с архива космотроники
    //Для аналоговых - один ТМ, для выходов - один выход ТМ
    internal class ObjectKosm : SourObject
    {
        public ObjectKosm(KosmotronikaConn conn, ObjectIndex ind) : base(conn)
        {
            Sn = ind.Sn; 
            NumType = ind.NumType;
            Appartment = ind.Appartment;
            Out = ind.Out;
        }

        //Добавить к объекту сигнал, если такого еще не было
        protected override SourInitSignal AddNewSignal(SourInitSignal sig)
        {
            if (sig.Inf["Prop"] == "ND")
                return StateSignal = StateSignal ?? sig;
            if (sig.Inf["Prop"] == "POK")
                return PokSignal = PokSignal ?? sig;
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
        internal SourInitSignal StateSignal { get; private set; }
        //Сигнал ПОК
        internal SourInitSignal PokSignal { get; private set; }

        //Чтение значений по одному объекту из рекордсета источника
        //Возвращает количество сформированных значений
        public override int ReadMoments(IRecordRead rec)
        {
            int nwrite = 0;
            DateTime time = rec.GetTime(3);
            var isAnalog = ((KosmotronikaBaseSource)SourceConn.Source).IsAnalog;
            int ndint = rec.GetInt(isAnalog ? 6 : 8);
            var err = MakeError(ndint);

            nwrite += AddMom(PokSignal, time, rec.GetInt(5), err);
            nwrite += AddMom(StateSignal, time, ndint);
            if (isAnalog)
                nwrite += AddMom(ValueSignal, time, rec.GetDouble(8), err);
            else
            {
                var strValue = rec.GetString(9);
                if (strValue.IndexOf("0x", StringComparison.Ordinal) >= 0)
                    nwrite += AddMom(ValueSignal, time, Convert.ToUInt32(strValue, 16), err);
                else nwrite += AddMom(ValueSignal, time, Convert.ToDouble(strValue), err);
            }
            return nwrite;
        }
    }
}