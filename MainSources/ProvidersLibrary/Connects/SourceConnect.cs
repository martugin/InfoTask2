using System;
using System.Management.Instrumentation;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Соединение-источник
    public class SourceConnect : ProviderConnect, IReadConnect
    {
        public SourceConnect(BaseProject project, string code, string complect) 
            : base(project, code, complect) { }

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
        private readonly DicS<SourceSignal> _outSignals = new DicS<SourceSignal>();
        public IDicSForRead<IReadSignal> ReadingSignals { get { return _outSignals; } }
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
            if (_outSignals.ContainsKey(fullCode))
                return _outSignals[fullCode];
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
                    sig =  _initialSignals.Add(fullCode, new CloneSignal(this, fullCode, dataType, contextOut, inf));
                    break;
                case SignalType.UniformClone:
                    sig = _initialSignals.Add(fullCode, new UniformCloneSignal(this, fullCode, dataType, contextOut, inf));
                    break;
            }
            return _outSignals.Add(fullCode, sig);
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
            _outSignals.Add(fullCode, calc);
            return CalcSignals.Add(fullCode, calc);
        }

        //Очистка списка сигналов
        public override void ClearSignals()
        {
            base.ClearSignals();
            _outSignals.Clear();
            InitialSignals.Clear();
            CalcSignals.Clear();
        }

        //Очистка значений сигналов
        internal void ClearSignalsValues(bool clearBegin)
        {
            AddEvent("Очистка значений сигналов");
            foreach (ListSignal sig in _outSignals.Values)
                sig.ClearMoments();
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

        //Создание клона
        #region Clone
        //Рекордсеты таблиц значений клона
        internal DaoRec CloneRec { get; private set; }
        internal DaoRec CloneCutRec { get; private set; }
        internal DaoRec CloneStrRec { get; private set; }
        internal DaoRec CloneStrCutRec { get; private set; }
        //Рекордсет таблицы ошибок создания клона
        internal DaoRec CloneErrorsRec { get; private set; }
        
        //Чтение Id сигналов клона
        private void ReadCloneSignals(DaoDb cloneDb)
        {
            AddEvent("Чтение сигналов клона");
            ClearSignals();
            using (var rec = new DaoRec(cloneDb, "Signals"))
                while (rec.Read())
                {
                    var sig = (CloneSignal)AddSignal(rec.GetString("FullCode"),
                                                                     rec.GetString("DataType").ToDataType(),
                                                                     rec.GetString("SignalType").ToSignalType() == SignalType.Uniform ? SignalType.UniformClone : SignalType.Clone,
                                                                     rec.GetString("InfObject"),
                                                                     rec.GetString("InfOut"),
                                                                     rec.GetString("InfProp"));
                    sig.IdInClone = rec.GetInt("SignalId");
                }
        }

        //Запись в клон списка описаний ошибок
        private void WriteMomentErrors(DaoDb cloneDb)
        {
            AddEvent("Запись описаний ошибок");
            using (var rec = new DaoRec(cloneDb, "MomentErrors"))
                foreach (var ed in Source.ErrPool.UsedErrorDescrs)
                    ed.ToRecordset(rec);
        }

        //Создание клона источника
        public void MakeClone(string cloneDir) //Каталог клона
        {
            try
            {
                if (PeriodIsUndefined()) return;
                using (var db = new DaoDb(cloneDir.EndDir() + "Clone.accdb"))
                {
                    ReadCloneSignals(db);
                    using (CloneRec = new DaoRec(db, "MomentValues"))
                    using (CloneCutRec = new DaoRec(db, "MomentValuesCut"))
                    using (CloneStrRec = new DaoRec(db, "MomentStrValues"))
                    using (CloneStrCutRec = new DaoRec(db, "MomentStrValuesCut"))
                    using (CloneErrorsRec = new DaoRec(db, "ErrorsObjects"))
                        GetValues();
                    WriteMomentErrors(db);
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при создании клона", ex);
            }
        }

        //Определяет время среза в клоне для указанного момента времени 
        internal DateTime RemoveMinultes(DateTime time)
        {
            int m = time.Minute;
            int k = m / 10;
            var d = time.AddMinutes(-time.Minute).AddSeconds(-time.Second).AddMilliseconds(-time.Millisecond);
            return d.AddMinutes(k * 10);
        }
        #endregion
    }
}