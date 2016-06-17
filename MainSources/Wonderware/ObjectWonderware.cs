﻿using System;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    //Объект
    internal class ObjectWonderware : SourceObject
    {
        public ObjectWonderware(WonderwareHistorianSource source, string tag) : base(source)
        {
            Inf = TagName = tag;
        }

        //Имя тэга 
        public string TagName { get; private set; }

        //Чтение значений по одному объекту из рекордсета источника
        //Возвращает количество сформированных значений
        public override int ReadValueFromRec(IRecordRead rec)
        {
            DateTime time = rec.GetTime("DateTime");
            var err = MakeError(rec.GetInt("QualityDetail"));
            if (ValueSignal.DataType.LessOrEquals(DataType.Real))
                return AddMom(ValueSignal, time, rec.GetDouble("Value"), err);
            return AddMom(ValueSignal, time, rec.GetString("vValue"), err);
        }
    }
}