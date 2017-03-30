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
        internal BaseSource Source { get { return (BaseSource) Provider; } }

        //Получение диапазона времени источника
        //Возвращает Default интервал, если нет связи с источником
        //Возвращает TimeInterval(Static.MinDate, DateTime.Now) если источник не позволяет определять диапазон
        public TimeInterval GetTime()
        {
            var ti = Source.GetTime();
            if (ti.IsDefault && ChangeProvider())
                return Source.GetTime();
            return ti;
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
                                                               DataType dataType, //Тип данных
                                                               string infObject, //Свойства объекта
                                                               string infOut, //Свойства выхода относительно объекта
                                                               string infProp, //Свойства сигнала относительно выхода
                                                               bool needCut) //Нужно считывать срез значений
        {
            if (InitialSignals.ContainsKey(fullCode))
                return InitialSignals[fullCode];
            Provider.IsPrepared = false;
            var sig = needCut ? new UniformSignal(this, fullCode, dataType, infObject, infOut, infProp)
                                      : new InitialSignal(this, fullCode, dataType, infObject, infOut, infProp);
            _signals.Add(fullCode, sig);
            return InitialSignals.Add(fullCode, sig);
        }
        
        //Добавить расчетный сигнал
        public CalcSignal AddCalcSignal(string fullCode, //Полный код сигнала
                                                         string objectCode, //Код объекта
                                                         string initialSignalCode, //Код исходного сигнала без кода объекта
                                                         string formula) //Формула
        {
            if (CalcSignals.ContainsKey(fullCode))
                return CalcSignals[fullCode];
            string icode = objectCode + "." + initialSignalCode;
            if (!_initialSignals.ContainsKey(icode))
                throw new InstanceNotFoundException("Не найден исходный сигнал " + icode);
            Provider.IsPrepared = false;
            var calc = new CalcSignal(fullCode, _initialSignals[icode], formula);
            _signals.Add(fullCode, calc);
            return CalcSignals.Add(fullCode, calc);
        }

        //Очистка списка сигналов
        public void ClearSignals()
        {
            AddEvent("Очистка списка сигналов");
            Provider.IsPrepared = false;
            _signals.Clear();
            InitialSignals.Clear();
            CalcSignals.Clear();
        }

        //Чтение значений из источника, возвращает true, если прочитались все значения или частично
        //В логгере дожны быть заданы начало и конец периода через CommandProgress
        public ValuesCount GetValues()
        {
            var vc = new ValuesCount();
            if (PeriodIsUndefined()) return vc;
            using (Start(0, 80))
            {
                vc += ReadValues();
                if (!vc.IsFail) return vc;
            }

            if (!ChangeProvider()) return vc;
            using (Start(80, 100))
                return ReadValues();
        }

        private void ClearSignalsValues(bool clearBegin)
        {
            AddEvent("Очистка значений сигналов");
            foreach (var sig in _signals.Values)
                sig.ClearMoments(clearBegin);
        }
        
        //Чтение значений из источника
        private ValuesCount ReadValues()
        {
            var vcount = new ValuesCount();
            try
            {
                ClearSignalsValues(PeriodBegin != Source.PrevPeriodEnd);
                using (Start(5, 10))
                    if (!Source.Connect() || !Source.Prepare())
                        return new ValuesCount(VcStatus.Fail);

                AddEvent("Чтение среза значений");
                using (Start(10, PeriodBegin == PeriodEnd ? 90 : 40))
                    vcount += Source.ReadCut();
                foreach (var sig in InitialSignals.Values)
                    if (sig is UniformSignal)
                        vcount.WriteCount += ((UniformSignal) sig).MakeBegin();
                AddEvent("Срез значений получен", vcount.ToString());
                if (vcount.Status == VcStatus.Fail)
                    return vcount;

                //Чтение изменений
                if (PeriodBegin < PeriodEnd)
                {
                    AddEvent("Чтение изменений значений");
                    ValuesCount changes;
                    using (Start(40, 85))
                        changes = Source.ReadChanges();
                    foreach (var sig in InitialSignals.Values)
                        if (sig is UniformSignal)
                            changes.WriteCount += ((UniformSignal) sig).MakeEnd();
                    AddEvent("Изменения значений получены", changes.ToString());
                    vcount += changes;
                    if (vcount.IsFail) return vcount;
                    Procent = 90;
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
            finally
            {
                AddErrorObjectsWarning();
                Source.PrevPeriodEnd = PeriodEnd;
            }
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
        internal void AddErrorOut(string codeObject,  //Код сигнала
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
                    CloneErrorsRec.Put("OutContext", codeObject);
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
        private void ReadCloneSignals(DaoDb cloneDb)
        {
            AddEvent("Чтение сигналов клона");
            ClearSignals();
            using (var rec = new DaoRec(cloneDb, "Signals"))
                while (rec.Read())
                {
                    var sig = AddInitialSignal(rec.GetString("FullCode"), 
                                                           rec.GetString("DataType").ToDataType(), 
                                                           rec.GetString("InfObject"), 
                                                           rec.GetString("InfOut"), 
                                                           rec.GetString("InfProp"), 
                                                           rec.GetBool("NeedCut"));
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
                string dir = cloneDir;
                if (!dir.EndsWith(@"\")) dir += @"\";
                using (var db = new DaoDb(dir + @"Clone.accdb"))
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
            return d.AddMinutes(k * m);
        }
        #endregion
    }
}