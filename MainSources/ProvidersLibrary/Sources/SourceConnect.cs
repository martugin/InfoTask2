using System;
using System.Management.Instrumentation;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Соединение-источник
    public class SourceConnect : ProviderConnect
    {
        public SourceConnect(string name, string complect, Logger logger) 
            : base(name, complect, logger) { }

        //Тип провайдера
        public override ProviderType Type { get { return ProviderType.Source;}}

        //Текущий провайдер источника
        private BaseSource Source { get { return (BaseSource) Provider; } }

        //Получение диапазона времени источника
        //Возвращает Default интервал, если нет связи с источником
        //Возвращает TimeInterval(Different.MinDate, DateTime.Now) если источник не позволяет определять диапазон
        public TimeInterval GetTime()
        {
            return Source.GetTime();
        }

        //Список сигналов, содержащих возвращаемые значения
        private readonly DicS<SourceSignal> _signals = new DicS<SourceSignal>();
        public IDicSForRead<SourceSignal> Signals { get { return _signals; } }
        //Множество исходных сигналов
        private readonly DicS<InitialSignal> _initialSignals = new DicS<InitialSignal>();
        internal DicS<InitialSignal> InitialSignals { get { return _initialSignals; } }
        //Словарь расчетных сигналов
        private readonly DicS<CalcSignal> _calcSignals = new DicS<CalcSignal>();
        internal DicS<CalcSignal> CalcSignals { get { return _calcSignals; } }

        //Добавить исходный сигнал
        public InitialSignal AddInitialSignal(string fullCode, //Полный код сигнала
                                                                string codeObject, //Код объекта
                                                                DataType dataType, //Тип данных
                                                                string signalInf, //Настройки сигнала
                                                                bool needCut) //Нужно считывать срез значений
        {
            if (InitialSignals.ContainsKey(fullCode))
                return InitialSignals[fullCode];
            var sig = needCut ? new UniformSignal(this, fullCode, codeObject, dataType, signalInf)
                                      : new InitialSignal(this, fullCode, codeObject, dataType, signalInf);
            _signals.Add(fullCode, sig);
            return InitialSignals.Add(fullCode, sig);
        }
        
        //Добавить расчетный сигнал
        public CalcSignal AddCalcSignal(string fullCode, //Полный код сигнала
                                                         string codeObject, //Код объекта
                                                         string initialSignal, //Код исходного сигнала
                                                         string formula) //Формула
        {
            if (CalcSignals.ContainsKey(fullCode))
                return CalcSignals[fullCode];
            if (!_initialSignals.ContainsKey(initialSignal))
                throw new InstanceNotFoundException("Не найден исходный сигнал " + initialSignal);
            var calc = new CalcSignal(fullCode, codeObject, _initialSignals[initialSignal], formula);
            _signals.Add(fullCode, calc);
            return CalcSignals.Add(fullCode, calc);
        }

        //Очистка списка сигналов
        public void ClearSignals()
        {
            AddEvent("Очистка списка сигналов");
            _signals.Clear();
            InitialSignals.Clear();
            CalcSignals.Clear();
            _isPrepared = false;
        }

        //Источник был подготовлен
        private bool _isPrepared;
        
        //Конец предыдущего периода расчета
        internal DateTime PrevPeriodEnd { get; private set; }

        //Чтение значений из источника, возвращает true, если прочитались все значения или частично
        public ValuesCount GetValues(DateTime periodBegin, DateTime periodEnd)
        {
            var valuesCount = new ValuesCount();
            using (Start())
            {
                try
                {
                    PeriodBegin = periodBegin;
                    PeriodEnd = periodEnd;
                    ClearSignalsValues(PeriodBegin != PrevPeriodEnd);
                    using (Start(5, 80))
                    {
                        valuesCount += GetValues();
                        if (!valuesCount.IsFail) return valuesCount;
                    }
                    _isPrepared = false;
                    if (!ChangeProvider()) return valuesCount;
                    using (Start(80, 100))
                    {
                        ClearSignalsValues(true);
                        return GetValues();
                    }
                }
                catch (Exception ex)
                {
                    AddError("Ошибка при чтении значений из источника", ex);
                    return new ValuesCount(VcStatus.Fail);
                }
                finally { PrevPeriodEnd = periodEnd; }
            }
        }

        private void ClearSignalsValues(bool clearBegin)
        {
            AddEvent("Очистка значений сигналов");
            foreach (var sig in _signals.Values)
                sig.ClearMoments(clearBegin);
        }
        
        //Чтение значений из источника
        private ValuesCount GetValues()
        {
            var vcount = new ValuesCount();
            try
            {
                if (!Source.Connect()) 
                    return new ValuesCount(VcStatus.Fail);
                if (!_isPrepared)
                {
                    Source.Prepare();
                    _isPrepared = true;
                }

                //Чтение среза
                using (Start(0, PeriodBegin < PeriodEnd ? 30 : 100))
                {
                    AddEvent("Чтение среза значений");
                    vcount += Source.ReadCut();
                    Procent = 90;
                    foreach (var sig in InitialSignals.Values)
                        if (sig is UniformSignal)
                            vcount.WriteCount += ((UniformSignal)sig).MakeBegin();
                    AddEvent("Срез значений получен", vcount.ToString());
                    if (vcount.Status == VcStatus.Fail) 
                        return vcount;
                }
                
                //Чтение изменений
                if (PeriodBegin < PeriodEnd)
                    using (Start(Procent, 90))
                    {
                        AddEvent("Чтение изменений значений");
                        var changes = Source.ReadChanges();
                        Procent = 90;
                        foreach (var sig in InitialSignals.Values)
                            if (sig is UniformSignal)
                                changes.WriteCount += ((UniformSignal)sig).MakeEnd();
                        AddEvent("Изменения значений получены", changes.ToString());
                        vcount += changes;
                        if (vcount.IsFail) return vcount;
                    }

                //Вычисление значений расчетных сигналов
                if (CalcSignals.Count > 0)
                {
                    AddEvent("Вычисление значений расчетных сигналов");
                    int calc = 0;
                    foreach (var sig in CalcSignals.Values)
                    {
                        sig.Calculate();
                        calc += sig.MomList.Count;
                    }
                    AddEvent("Значения расчетных сигналов прочитаны", calc + " значений сформировано");
                    vcount.WriteCount += calc;
                }
                AddEvent("Значения из источника прочитаны", vcount.ToString());
            }
            catch (Exception ex)
            {
                AddError("Ошибка при чтении значений из источника", ex);
                return vcount.AddStatus(VcStatus.Fail);
            }
            finally {AddErrorObjectsWarning();}
            return vcount;
        }

        //Создание клона
        #region
        //Рекордсеты таблиц значений клона
        internal DaoRec CloneRec { get; private set; }
        internal DaoRec CloneCutRec { get; private set; }
        internal DaoRec CloneStrRec { get; private set; }
        internal DaoRec CloneStrCutRec { get; private set; }
        //Рекордсет таблицы ошибок создания клона
        internal DaoRec CloneErrorsRec { get; private set; }

        //Словарь ошибочных объектов, ключи - коды объектов
        private readonly DicS<string> _errorObjects = new DicS<string>();

        //Добавляет объект в ErrorsObjects
        internal void AddErrorObject(string codeObject,  //Код сигнала
                                                     string errText,        //Сообщение об ошибке
                                                     Exception ex = null)  //Исключение
        {
            if (!_errorObjects.ContainsKey(codeObject))
            {
                var err = errText + (ex == null ? "" : (". " + ex.Message + ". " + ex.StackTrace));
                _errorObjects.Add(codeObject, err);
                if (CloneErrorsRec != null)
                {
                    CloneErrorsRec.AddNew();
                    CloneErrorsRec.Put("CodeObject", codeObject);
                    CloneErrorsRec.Put("ErrorDescription", err);
                    CloneErrorsRec.Update();
                }
            }
        }

        //Запись списка непрочитанных объектов в лог
        internal void AddErrorObjectsWarning()
        {
            if (_errorObjects.Count > 0)
            {
                int i = 0;
                string s = "";
                foreach (var ob in _errorObjects.Keys)
                    if (++i < 10) s += ob + ", ";
                AddWarning("Не удалось прочитать значения по некоторым объектам", null,
                           s + (_errorObjects.Count > 10 ? " и др." : "") + "всего " + _errorObjects.Count + " объектов не удалось прочитать");
            }
        }

        //Чтение Id сигналов клона
        private void ReadCloneSignalsId(DaoDb cloneDb)
        {
            AddEvent("Чтение Id сигналов клона");
            using (var rec = new DaoRec(cloneDb, "Signals"))
                while (rec.Read())
                {
                    var code = rec.GetString("FullCode");
                    if (_signals.ContainsKey(code) && _signals[code] is UniformSignal)
                        ((UniformSignal)_signals[code]).IdInClone = rec.GetInt("SignalId");
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
        public void MakeClone(DateTime periodBegin, //Начало периода клона
                                          DateTime periodEnd, //Конец периода клона
                                          string cloneDir) //Каталог клона
        {
            try
            {
                string dir = cloneDir;
                if (!dir.EndsWith(@"\")) dir += @"\";
                using (var db = new DaoDb(dir + @"Clone.accdb"))
                {
                    ReadCloneSignalsId(db);
                    using (CloneRec = new DaoRec(db, "MomentValues"))
                    using (CloneCutRec = new DaoRec(db, "MomentValuesCut"))
                    using (CloneStrRec = new DaoRec(db, "MomentStrValues"))
                    using (CloneStrCutRec = new DaoRec(db, "MomentStrValuesCut"))
                    using (CloneErrorsRec = new DaoRec(db, "ErrorsObjects"))
                        GetValues(periodBegin, periodEnd);
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
            return d.AddMinutes(k * m);
        }
        #endregion
    }
}