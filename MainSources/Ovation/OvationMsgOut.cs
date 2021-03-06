﻿using System;
using BaseLibrary;
using ProvidersLibrary;

namespace Ovation
{
    internal class OvationMsgOut : ListSourceOut
    {
        internal OvationMsgOut(OvationSource source, string objectType) : base(source)
        {
            OutType = objectType;
        }

        //Тип выхода (ALARM, SOE, TEXT)
        internal string OutType { get; private set; }

        //Флажки сообщений
        internal ListSignal MsgFlagsSignal { get; private set; }
        //Тип сообщения
        internal ListSignal MsgTypeSignal { get; private set; }
        //Подтип
        internal ListSignal SubTypeSignal { get; private set; }
        //Номер системы
        internal ListSignal SystemSignal { get; private set; }
        //Узел - источник сообщения
        internal ListSignal NodeSignal { get; private set; }
        //Имя точки
        internal ListSignal AlmNameSignal { get; private set; }
        //Текст сообщения
        internal ListSignal PrimTextSignal { get; private set; }
        //Дополнительный текст
        internal ListSignal SuppTextSignal { get; private set; }
        //Дополнительная информация
        internal ListSignal Info1Signal { get; private set; }
        internal ListSignal Info2Signal { get; private set; }

        //Добавить к выходу сигнал, если такого еще не было
        protected override ListSignal AddSourceSignal(ListSignal sig)
        {
            switch (sig.Inf["Prop"])
            {
                case "MSG_FLAGS":
                    return MsgFlagsSignal = MsgFlagsSignal ?? sig;
                case "MSG_TYPE":
                    return MsgTypeSignal = MsgTypeSignal ?? sig;
                case "SUB_TYPE":
                    return SubTypeSignal = SubTypeSignal ?? sig;
                case "SYSTEM":
                    return SystemSignal = SystemSignal ?? sig;
                case "NODE":
                    return NodeSignal = NodeSignal ?? sig;
                case "ALM_NAME":
                    return AlmNameSignal = AlmNameSignal ?? sig;
                case "PRIM_TEXT":
                    return PrimTextSignal = PrimTextSignal ?? sig;
                case "SUPP_TEXT":
                    return SuppTextSignal = SuppTextSignal ?? sig;
                case "INFO1":
                    return Info1Signal = Info1Signal ?? sig;
                case "INFO2":
                    return Info2Signal = Info2Signal ?? sig;
            }
            return null;
        }

        //Чтение значений по одному выходу из рекордсета источника и добавление их в список или клон
        //Возвращает количество сформированных значений
        protected override int ReadMoments(IRecordRead rec)
        {
            int nwrite = 0;
            var time = ReadTime(rec);
            nwrite += AddMomInt(MsgFlagsSignal, time, rec, "MSG_FLAGS");
            nwrite += AddMomInt(MsgTypeSignal, time, rec, "MSG_TYPE");
            nwrite += AddMomInt(SubTypeSignal, time, rec, "SUB_TYPE");
            nwrite += AddMomInt(SystemSignal, time, rec, "SYSTEM");
            nwrite += AddMomString(NodeSignal, time, rec, "NODE");
            nwrite += AddMomString(AlmNameSignal, time, rec, "ALM_NAME");
            nwrite += AddMomString(PrimTextSignal, time, rec, "PRIM_TEXT");
            nwrite += AddMomString(SuppTextSignal, time, rec, "SUPP_TEXT");
            if (Info1Signal != null)
                nwrite += AddMom(Info1Signal, time, rec.GetString(OutType == "TEXT" ? "SUPP_INFO1" : OutType + "_INFO1"));
            nwrite += AddMomString(Info2Signal, time, rec, "SUPP_INFO2"); 
            return nwrite;
        }

        //Чтение времени из рекордсета источника
        private static DateTime ReadTime(IRecordRead rec)
        {
            return rec.GetTime("TIMESTAMP")
                .AddMilliseconds(rec.GetInt("TIME_NSEC") / 1000000.0)
                    .ToLocalTime();
        }
    }
}