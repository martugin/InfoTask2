using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Один сигнал 
    public class SourceSignal : ProviderSignal, ISourceSignal
    {
        public SourceSignal(ISource source, string code, DataType dataType, string signalInf)
            : base(code, dataType, signalInf)
        {
            _source = source;
            _momList = MFactory.NewList(dataType);
            MomList = new MomListReadOnly(_momList);
            BufMom = new MomEdit(dataType);
            _beginMom = new MomEdit(dataType);
            _endMom = new MomEdit(dataType);
        }

        //Источник
        private readonly ISource _source;
        
        //Возвращаемый список значений
        private readonly MomList _momList;
        public IMomListReadOnly MomList { get; private set; }

        //Значение среза на начало периода
        private readonly MomEdit _beginMom;
        //Значение среза для следующего периода
        private readonly MomEdit _endMom;
        //Буферное значение для добавления в список
        internal MomEdit BufMom { get; private set; }

        //Добавка мгновенного значения в список 
        //Возвращает количество реально добавленных значений 
        internal int PutMom(DateTime time, ErrMom err)  
        {
            BufMom.Time = time;
            BufMom.Error = err;
            if (time <= _source.PeriodBegin && _beginMom.Time <= time)
                _beginMom.CopyAllFrom(BufMom);
            else if (time <= _source.PeriodEnd && _endMom.Time <= time)
            {
                _endMom.CopyAllFrom(BufMom);
                _momList.AddMom(BufMom);
                return 1;
            }
            return 0;
        }

        //Очистка списка значений
        internal void ClearMoments(bool clearBegin)
        {
            _momList.Clear();
            if (clearBegin) _beginMom.Time = Different.MinDate;
        }

        //Добавляет значение среза на начало периода в список или клон, возвращает 1, если срез был получен, иначе 0
        internal int MakeBegin()
        {
            if (_beginMom.Time == Different.MinDate) return 0;
            _momList.AddMom(_beginMom);
            return 1;
        }

        //Формирует значение на конец периода и дополняет значения в клоне до конца периода
        internal void MakeEnd()
        {
            if (_endMom.Time != Different.MinDate)
                _beginMom.CopyAllFrom(_endMom);
        }
    }
}