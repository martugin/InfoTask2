using System;
using System.Management.Instrumentation;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Соединение-источник
    public class SourceConnect : ProviderConnect
    {
        protected SourceConnect(string name, string complect, Logger logger) 
            : base(name, complect, logger) { }

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

        //Словарь исходных сигналов
        private readonly DicS<SourceSignal> _initialSignals = new DicS<SourceSignal>();
        internal DicS<SourceSignal> InitialSignals { get { return _initialSignals; } }
        //Словарь расчетных сигналов
        private readonly DicS<CalcSignal> _calcSignals = new DicS<CalcSignal>();
        internal DicS<CalcSignal> CalcSignals { get { return _calcSignals; } }
        
        //Переопределяемый метод для добавления сигналов конкретного типа
        protected override ProviderSignal AddConcreteSignal(string fullCode, DataType dataType, SignalType signalType, string contextOut, DicS<string> inf)
        {
            switch (signalType)
            {
                case SignalType.Mom:
                    return _initialSignals.Add(fullCode, new MomSignal(this, fullCode, dataType, contextOut, inf));
                case SignalType.Uniform:
                    return _initialSignals.Add(fullCode, new UniformSignal(this, fullCode, dataType, contextOut, inf));
                case SignalType.List:
                    return _initialSignals.Add(fullCode, new ListSignal(this, fullCode, dataType, contextOut, inf));
                case SignalType.Clone:
                    return _initialSignals.Add(fullCode, new CloneSignal(this, fullCode, dataType, contextOut, inf));
                case SignalType.UniformClone:
                    return _initialSignals.Add(fullCode, new UniformCloneSignal(this, fullCode, dataType, contextOut, inf));
            }
            return null;
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
            ProviderSignals.Add(fullCode, calc);
            return CalcSignals.Add(fullCode, calc);
        }

        //Очистка списка сигналов
        public override void ClearSignals()
        {
            base.ClearSignals();
            InitialSignals.Clear();
            CalcSignals.Clear();
        }

        //Очистка значений сигналов
        internal void ClearSignalsValues(bool clearBegin)
        {
            AddEvent("Очистка значений сигналов");
            foreach (ListSignal sig in ProviderSignals.Values)
                sig.ClearMoments(clearBegin);
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

        //Чтение значений из источника
        protected ValuesCount ReadValues()
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
                    if (sig is UniformCloneSignal)
                        vcount.WriteCount += ((UniformCloneSignal)sig).MakeBegin();
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
                        if (sig is UniformCloneSignal)
                            changes.WriteCount += ((UniformCloneSignal)sig).MakeEnd();
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
                        calc += sig.Value.Count;
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
                AddErrorOutsWarning();
                Source.PrevPeriodEnd = PeriodEnd;
            }
            return vcount;
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

        //Словарь ошибочных объектов, ключи - коды объектов
        private readonly DicS<string> _errorOuts = new DicS<string>();

        //Добавляет объект в ErrorsObjects
        internal void AddErrorOut(string codeObject,  //Код сигнала
                                                string errText,        //Сообщение об ошибке
                                                Exception ex = null)  //Исключение
        {
            if (!_errorOuts.ContainsKey(codeObject))
            {
                var err = errText + (ex == null ? "" : ". " + ex.Message + ". " + ex.StackTrace);
                _errorOuts.Add(codeObject, err);
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
        internal void AddErrorOutsWarning()
        {
            if (_errorOuts.Count > 0)
            {
                int i = 0;
                string s = "";
                foreach (var ob in _errorOuts.Keys)
                    if (++i < 10) s += ob + ", ";
                AddWarning("Не удалось прочитать значения по некоторым сигналам", null,
                           s + (_errorOuts.Count > 10 ? " и др." : "") + "всего " + _errorOuts.Count + " выходов не удалось прочитать");
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
                    var sig = (CloneSignal)AddSignal(rec.GetString("FullCode"),
                                                                     rec.GetString("DataType").ToDataType(),
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