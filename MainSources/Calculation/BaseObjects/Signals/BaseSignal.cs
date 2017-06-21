using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Базовый класс для сигналов расчета и компиляции
    public class BaseSignal
    {
        public BaseSignal(IRecordRead rec)
        {
            Code = rec.GetString("CodeSignal");
            Name = rec.GetString("NameSignal");
            DataType = rec.GetString("DataType").ToDataType();
            InfOut = rec.GetString("InfOut");
            InfProp = rec.GetString("InfProp");
            SignalType = rec.GetString("SignalType");
            InitialSignals = rec.GetString("InitialSignals");
            Formula = rec.GetString("Formula");
        }

        //Код сигнала
        public string Code { get; private set; }
        //Имя сигнала
        public string Name { get; private set; }
        //Тип данных
        public DataType DataType { get; private set; }
        //Свойства выхода и сигнала для источника
        public string InfOut { get; private set; }
        public string InfProp { get; private set; }
        //Тип сигнала
        public string SignalType { get; private set; }
        //Список исходных сигналов, используемых в расчете
        public string InitialSignals { get; private set; }
        //Расчетная формула в прямой польской записи
        public string Formula { get; private set; }
    }
}