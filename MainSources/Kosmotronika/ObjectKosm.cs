using CommonTypes;

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
    internal class ObjectKosm : SourceObject
    {
        public ObjectKosm(ObjectIndex ind, string code) : base(code)
        {
            Sn = ind.Sn; 
            NumType = ind.NumType;
            Appartment = ind.Appartment;
            Out = ind.Out;
            int p = code.LastIndexOf(".");
            string s = p == -1 ? code : code.Substring(0, p);
            Inf = string.Concat("Code=", s, "; SN=", Sn, "; NumType=", NumType, ";Out=", Out, "; Appartment=", Appartment, ";");
        }

        //Добавить к объекту сигнал, если такого еще не было
        public SourceSignal AddSignal(SourceSignal sig)
        {
            if (sig.Inf["Prop"] == "ND")
                return StateSignal ?? (StateSignal = sig);
            if (sig.Inf["Prop"] == "POK")
                return PokSignal ?? (PokSignal = sig);
            return ValueSignal ?? (ValueSignal = sig);
        }
        
        //Системный номер
        internal int Sn { get; private set; }
        //Номер типа
        internal int NumType { get; private set; }
        //Подразделение
        internal int Appartment { get; private set; }
        //Номер выхода
        internal int Out { get; private set; }

        //Сигнал, содержащий значения самого выхода параметра
        internal SourceSignal ValueSignal { get; private set; }
        //Сигнал недостоврности
        internal SourceSignal StateSignal { get; private set; }
        //Сигнал ПОК
        internal SourceSignal PokSignal { get; private set; }

        //Для объекта определен срез
        public override bool HasBegin
        {
            get { return SignalsHasBegin(ValueSignal, StateSignal, PokSignal);}
        }

        //Добавляет в сигналы объекта срез, если возможно, возвращает, сколько добавлено значений
        public override int AddBegin()
        {
            return SignalsAddBegin(ValueSignal, StateSignal, PokSignal);
        }
    }
}