using System;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Provider
{
    //Выход
    internal class WonderwareOut : ListSourceOut
    {
        internal WonderwareOut(WonderwareSource source, string tag) : base(source)
        {
            TagName = tag;
        }

        //Имя тэга 
        internal string TagName { get; private set; }

        //Чтение значений по одному выходу из рекордсета источника
        //Возвращает количество сформированных значений
        protected override int ReadMoments(IRecordRead rec)
        {
            DateTime time = rec.GetTime("DateTime");
            var err = MakeError(rec.GetInt("QualityDetail"));
            if (ValueSignal.DataType.LessOrEquals(DataType.Real))
                return AddMomReal(ValueSignal, time, rec, "Value", err);
            return AddMomString(ValueSignal, time, rec, "vValue", err);
        }
    }
}