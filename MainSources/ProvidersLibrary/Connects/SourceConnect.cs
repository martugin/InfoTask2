using System.Management.Instrumentation;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Соединение с источником
    public class SourceConnect : ProviderConnect, IReadingConnect
    {
        public SourceConnect(Logger logger, string code, string complect, string projectCode = "") 
            : base(logger, code, complect, projectCode) { }

        //Тип провайдера
        public override ProviderType Type
        {
            get { return ProviderType.Source;}
        }
        //Текущий провайдер источника
        internal Source Source
        {
            get { return (Source)Provider; }
        }

        //Список сигналов, содержащих возвращаемые значения
        private readonly DicS<SourceSignal> _readingSignals = new DicS<SourceSignal>();
        public IDicSForRead<IReadSignal> ReadingSignals { get { return _readingSignals; } }
        //Словарь исходных сигналов
        private readonly DicS<SourceSignal> _initialSignals = new DicS<SourceSignal>();
        internal DicS<SourceSignal> InitialSignals { get { return _initialSignals; } }
        //Словарь расчетных сигналов
        private readonly DicS<CalcSignal> _calcSignals = new DicS<CalcSignal>();
        internal DicS<CalcSignal> CalcSignals { get { return _calcSignals; } }

        //Добавить сигнал
        public SourceSignal AddSignal(string fullCode, //Полный код сигнала
                                                      DataType dataType, //Тип данных
                                                      SignalType signalType, //Тип сигнала
                                                      string infObject, //Свойства объекта
                                                      string infOut = "", //Свойства выхода относительно объекта
                                                      string infProp = "") //Свойства сигнала относительно выхода
        {
            if (_readingSignals.ContainsKey(fullCode))
                return _readingSignals[fullCode];
            Provider.IsPrepared = false;
            var contextOut = infObject + (infOut.IsEmpty() ? "" : ";" + infOut);
            var inf = infObject.ToPropertyDicS().AddDic(infOut.ToPropertyDicS()).AddDic(infProp.ToPropertyDicS());
            SourceSignal sig = null;
            switch (signalType)
            {
                case SignalType.Mom:
                    sig = _initialSignals.Add(fullCode, new MomSignal(this, fullCode, dataType, contextOut, inf));
                    break;
                case SignalType.Uniform:
                    sig = _initialSignals.Add(fullCode, new UniformSignal(this, fullCode, dataType, contextOut, inf));
                    break;
                case SignalType.List:
                    sig = _initialSignals.Add(fullCode, new ListSignal(this, fullCode, dataType, contextOut, inf));
                    break;
                case SignalType.Clone:
                    sig =  _initialSignals.Add(fullCode, new CloneSignal((ClonerConnect)this, fullCode, dataType, contextOut, inf));
                    break;
                case SignalType.UniformClone:
                    sig = _initialSignals.Add(fullCode, new UniformCloneSignal((ClonerConnect)this, fullCode, dataType, contextOut, inf));
                    break;
            }
            return _readingSignals.Add(fullCode, sig);
        }

        //Добавить расчетный сигнал
        public CalcSignal AddCalcSignal(string fullCode, //Полный код сигнала
                                                        string objectCode, //Код объекта
                                                        string initialCode, //Код исходного сигнала без кода объекта
                                                        string formula) //Формула
        {
            if (CalcSignals.ContainsKey(fullCode))
                return CalcSignals[fullCode];
            string icode = objectCode + "." + initialCode;
            if (!_initialSignals.ContainsKey(icode))
                throw new InstanceNotFoundException("Не найден исходный сигнал " + icode);
            Provider.IsPrepared = false;
            var calc = new CalcSignal(fullCode, _initialSignals[icode], formula);
            _readingSignals.Add(fullCode, calc);
            return CalcSignals.Add(fullCode, calc);
        }

        //Очистка списка сигналов
        public override void ClearSignals()
        {
            base.ClearSignals();
            _readingSignals.Clear();
            InitialSignals.Clear();
            CalcSignals.Clear();
        }

        //Очистка значений сигналов
        public void ClearSignalsValues(bool clearBegin)
        {
            AddEvent("Очистка значений сигналов");
            foreach (var sig in _readingSignals.Values)
                if (sig is ListSignal)
                    ((ListSignal)sig).ClearMoments();
        }

        //Чтение значений из источника, возвращает true, если прочитались все значения или частично
        //В логгере дожны быть заданы начало и конец периода через CommandProgress
        public ValuesCount GetValues()
        {
            var vc = new ValuesCount();
            if (PeriodIsUndefined()) return vc;
            using (Start(0, 80))
            {
                vc += Source.ReadValues();
                if (!vc.IsFail) return vc;
            }

            if (!ChangeProvider()) return vc;
            using (Start(80, 100))
                return Source.ReadValues();
        }
    }
}