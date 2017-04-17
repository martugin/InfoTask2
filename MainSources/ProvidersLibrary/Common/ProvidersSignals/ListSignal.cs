﻿using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для мгновенных сигналов
    public class ListSignal : ProviderSignal
    {
        protected ListSignal(ProviderConnect connect, string code, DataType dataType, string contextOut, DicS<string> inf)
            : base(connect, code, dataType, contextOut, inf)
        {
            MomList = MFactory.NewList(dataType);
            _bufMom = MFactory.NewMom(dataType);
        }
        protected ListSignal(ProviderConnect connect, string code) 
            : base(connect,  code) { }

        public override SignalValueType ValueType
        {
            get { return SignalValueType.List; }
        }

        //Список значений
        protected MomList MomList { get; set; }

        //Очистка списка значений
        internal virtual void ClearMoments(bool clearBegin)
        {
            MomList.Clear();
        }

        //Возвращаемый список значений
        public override IMean Value
        {
            get { return MomList; }
            set { MomList = (MomList)value; }
        }

        //Буферное значение для добавления
        private readonly IMean _bufMom;
        internal override IMean BufMom { get { return _bufMom; }}
        
        //Добавка мгновенного значения в список или клон
        //Возвращает количество реально добавленных значений 
        internal override int AddMom(DateTime time, MomErr err)
        {
            if (!CheckMomTime(time, err)) return 0;
            MomList.AddMom(BufMom); 
            return 1;
        }

        //Проверка времени мгновенного значения
        protected bool CheckMomTime(DateTime time, MomErr err)
        {
            if (time < Connect.PeriodBegin || time > Connect.PeriodEnd)
                return false;
            if (BufMom.Time == time && time == Connect.PeriodBegin)
                return false;
            BufMom.Time = time;
            BufMom.Error = err;
            return true;
        }
    }
}