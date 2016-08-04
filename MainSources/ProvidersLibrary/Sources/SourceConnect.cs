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
        public SourceBase CurSource { get { return (SourceBase) CurProvider; } }

        //Список сигналов, содержащих возвращаемые значения
        internal readonly DicS<SourceSignal> ProviderSignals = new DicS<SourceSignal>();
        public IDicSForRead<SourceSignal> Signals { get { return ProviderSignals; } }
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
            ProviderSignals.Add(fullCode, sig);
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
            ProviderSignals.Add(fullCode, calc);
            return CalcSignals.Add(fullCode, calc);
        }

        //Очистка списка сигналов
        public void ClearSignals()
        {
            ProviderSignals.Clear();
            InitialSignals.Clear();
            CalcSignals.Clear();
            CloneSignalsId.Clear();
        }

        //Чтение значений, возвращает true, если прочитались все значения или частично
        public bool GetValues(DateTime periodBegin, DateTime periodEnd)
        {
            try
            {
                using (Start())
                {
                    foreach (var sig in ProviderSignals.Values)
                        sig.ClearMoments(periodBegin != PeriodEnd);
                    PeriodBegin = periodBegin;
                    PeriodEnd = periodEnd;

                    using (Start(5, 80))
                        if (GetValues()) return true;

                    if (ChangeCurProvider())
                        using (Start(80, 100))
                            return GetValues();
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при чтении значений из источника", ex);
            }
            return false;
        }

        //Чтение значений из источника
        public bool GetValues()
        {
            try
            {
                if (!CurSource.ConnectProvider(false))
                    return false;
                if (!CurSource.IsPrepared)
                    CurSource.Prepare();
                var vcount = new ValuesCount();
                using (Start(0, PeriodBegin < PeriodEnd ? 30 : 100))
                {
                    vcount += CurSource.ReadCut();
                    Procent = 90;
                    foreach (var sig in InitialSignals.Values)
                        if (sig is UniformSignal)
                            vcount.WriteCount += ((UniformSignal)sig).MakeBegin();
                    AddEvent("Срез значений получен", vcount.ReadCount + " значений прочитано, " + vcount.WriteCount + " значений сформировано");
                    if (vcount.NeedBreak) return false;
                }
                
                if (PeriodBegin < PeriodEnd)
                    using (Start(Procent, 90))
                    {
                        var changes = CurSource.ReadChanges();
                        Procent = 90;
                        foreach (var sig in InitialSignals.Values)
                            if (sig is UniformSignal)
                                changes.WriteCount += ((UniformSignal)sig).MakeEnd();
                        AddEvent("Изменения значений получены", changes.ReadCount + " значений прочитано, " + changes.WriteCount + " значений сформировано");
                        vcount += changes;
                        if (vcount.NeedBreak) return false;
                    }

                if (CalcSignals.Count > 0)
                {
                    int calc = 0;
                    foreach (var sig in CalcSignals.Values)
                    {
                        sig.Calculate();
                        calc += sig.MList.Count;
                    }
                    AddEvent("Значения расчетных сигналов прочитаны", calc + " значений сформировано");
                    vcount.WriteCount += calc;
                }
                AddEvent("Значения из источника прочитаны", vcount.ReadCount + " значений прочитано, " + vcount.WriteCount + " значений сформировано");
            }
            catch (Exception ex)
            {
                AddError("Ошибка при чтении значений из источника", ex);
                return false;
            }
            return true;
        }

        //Создание клона
        #region
        //Рекордсеты таблиц значений клона
        internal RecDao CloneRec { get; private set; }
        internal RecDao CloneCutRec { get; private set; }
        internal RecDao CloneStrRec { get; private set; }
        internal RecDao CloneStrCutRec { get; private set; }
        //Рекордсет таблицы ошибок создания клона
        internal RecDao CloneErrorsRec { get; private set; }

        //Словарь сигналов клона, ключи Id в клоне, используется при записи в клон
        private readonly DicI<InitialSignal> _cloneSignalsId = new DicI<InitialSignal>();
        public DicI<InitialSignal> CloneSignalsId { get { return _cloneSignalsId; } }
        //Словарь ошибочных объектов, ключи - коды объектов
        private readonly DicS<string> _errorObjects = new DicS<string>();
        protected DicS<string> ErrorObjects { get { return _errorObjects; } }

        //Добавляет объект в ErrorsObjects
        internal void AddErrorObject(string codeObject,  //Код сигнала
                                                        string errText,        //Сообщение об ошибке
                                                        Exception ex = null)  //Исключение
        {
            if (!ErrorObjects.ContainsKey(codeObject))
            {
                var err = errText + (ex == null ? "" : (". " + ex.Message + ". " + ex.StackTrace));
                ErrorObjects.Add(codeObject, err);
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
            if (ErrorObjects.Count > 0)
            {
                int i = 0;
                string s = "";
                foreach (var ob in _errorObjects.Keys)
                    if (++i < 10) s += ob + ", ";
                AddWarning("Не удалось прочитать значения по некоторым объектам", null,
                           s + (ErrorObjects.Count > 10 ? " и др." : "") + "всего " + ErrorObjects.Count + " объектов не удалось прочитать");
            }
        }

        //Чтение Id сигналов клона
        protected void ReadCloneSignalsId(DaoDb cloneDb)
        {
            using (var rec = new RecDao(cloneDb, "Signals"))
                while (rec.Read())
                {
                    var code = rec.GetString("FullCode");
                    if (ProviderSignals.ContainsKey(code) && ProviderSignals[code] is UniformSignal)
                        ((UniformSignal)ProviderSignals[code]).IdInClone = rec.GetInt("SignalId");
                }
        }

        //Подготовка клона к записи 
        private void WriteMomentErrors(DaoDb cloneDb)
        {
            using (var rec = new RecDao(cloneDb, "MomentErrors"))
                foreach (var ed in CurSource.ErrPool.UsedErrorDescrs)
                    ed.ToRecordset(rec);
        }

        //Создание клона архива
        public void MakeClone(DateTime beginRead, //Начало периода клона
                                          DateTime endRead, //Конец периода клона
                                          string cloneFile) //Файл значений клона
        {
            try
            {
                using (var db = new DaoDb(cloneFile))
                {
                    ReadCloneSignalsId(db);
                    using (CloneRec = new RecDao(db, "MomentValues"))
                    using (CloneCutRec = new RecDao(db, "MomentValuesCut"))
                    using (CloneStrRec = new RecDao(db, "MomentStrValues"))
                    using (CloneStrCutRec = new RecDao(db, "MomentStrValuesCut"))
                    using (CloneErrorsRec = new RecDao(db, "ErrorsObjects"))
                        GetValues(beginRead, endRead);
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