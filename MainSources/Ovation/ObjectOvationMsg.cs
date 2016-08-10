﻿using System;
using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    internal class ObjectOvationMsg : SourceObject
    {
        internal ObjectOvationMsg(SourceBase source, string objectType) : base(source)
        {
            ObjectType = objectType;
        }

        //Тип объекта (ALARM, SOE, TEXT)
        internal string ObjectType { get; private set; }

        //Флажки сообщений
        internal InitialSignal MsgFlagsSignal { get; private set; }
        //Тип сообщения
        internal InitialSignal MsgTypeSignal { get; private set; }
        //Подтип
        internal InitialSignal SubTypeSignal { get; private set; }
        //Номер системы
        internal InitialSignal SystemSignal { get; private set; }
        //Узел - источник сообщения
        internal InitialSignal NodeSignal { get; private set; }
        //Имя точки
        internal InitialSignal AlmNameSignal { get; private set; }
        //Текст сообщения
        internal InitialSignal PrimTextSignal { get; private set; }
        //Дополнительный текст
        internal InitialSignal SuppTextSignal { get; private set; }
        //Дополнительная информация
        internal InitialSignal Info1Signal { get; private set; }
        internal InitialSignal Info2Signal { get; private set; }

        //Добавить к объекту сигнал, если такого еще не было
        protected override InitialSignal AddNewSignal(InitialSignal sig)
        {
            if (sig.Inf["Prop"] == "MSG_FLAGS")
                return MsgFlagsSignal = MsgFlagsSignal ?? sig;
            if (sig.Inf["Prop"] == "MSG_TYPE")
                return MsgTypeSignal = MsgTypeSignal ?? sig;
            if (sig.Inf["Prop"] == "SUB_TYPE")
                return SubTypeSignal = SubTypeSignal ?? sig;
            if (sig.Inf["Prop"] == "SYSTEM")
                return SystemSignal = SystemSignal ?? sig;
            if (sig.Inf["Prop"] == "NODE")
                return NodeSignal = NodeSignal ?? sig;
            if (sig.Inf["Prop"] == "ALM_NAME")
                return AlmNameSignal = AlmNameSignal ?? sig;
            if (sig.Inf["Prop"] == "PRIM_TEXT")
                return PrimTextSignal = PrimTextSignal ?? sig;
            if (sig.Inf["Prop"] == "SUPP_TEXT")
                return SuppTextSignal = SuppTextSignal ?? sig;
            if (sig.Inf["Prop"] == "INFO1")
                return Info1Signal = Info1Signal ?? sig;
            if (sig.Inf["Prop"] == "INFO2")
                return Info2Signal = Info2Signal ?? sig;
            return null;
        }

        //Чтение значений по одному объекту из рекордсета источника и добавление их в список или клон
        //Возвращает количество сформированных значений
        protected override int ReadMoments(IRecordRead rec)
        {
            int nwrite = 0;
            var time = ReadTime(rec);
            nwrite += AddMomInt(MsgFlagsSignal, time, rec, "MSG_FLAGS");
            nwrite += AddMomInt(MsgTypeSignal, time, rec, "MSG_TYPE");
            nwrite += AddMomInt(SubTypeSignal, time, rec, "SUB_TYPE");
            nwrite += AddMomString(NodeSignal, time, rec, "NODE");
            nwrite += AddMomString(AlmNameSignal, time, rec, "ALM_NAME");
            nwrite += AddMomString(PrimTextSignal, time, rec, "PRIM_TEXT");
            nwrite += AddMomString(SuppTextSignal, time, rec, "SUPP_TEXT");
            if (Info1Signal != null)
                nwrite += AddMom(Info1Signal, time, rec.GetString(ObjectType == "TEXT" ? "SUPP_INFO1" : ObjectType + "_INFO1"));
            nwrite += AddMomString(Info2Signal, time, rec, "SUPP_INFO2"); 
            return nwrite;
        }

        //Чтение времени из рекордсета источника
        private DateTime ReadTime(IRecordRead rec)
        {
            return rec.GetTime("TIMESTAMP")
                .AddMilliseconds(rec.GetInt("TIME_NSEC") / 1000000.0)
                    .ToLocalTime();
        }
    }
}