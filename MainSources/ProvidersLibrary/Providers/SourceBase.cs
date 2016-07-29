using System;
using System.Management.Instrumentation;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Базовый класс для всех источников
    public abstract class SourceBase : ProviderBase
    {
        protected SourceBase()
        {
            NeedCut = true;
            CloneCutFrequency = 10;
        }

        //Изменяемые настройки
        //Нужно считывать срез
        protected bool NeedCut { get; set; }
        //Частота в минутах фиксации среза в клоне, должна делить 60
        public int CloneCutFrequency { get; protected set; }

        //Тип провайдера
        public override ProviderType Type { get { return ProviderType.Source; } }
        //Текущее соединение
        protected SourceSettings SourceSettings { get { return (SourceSettings)CurSettings; } }

        //Получение диапазона времени источника
        public TimeInterval GetTime()
        {
            return SourceSettings.GetTime();
        }

        //Добавить исходный сигнал
        public InitialSignal AddInitialSignal(string fullCode, //Полный код сигнала
                                                                string codeObject, //Код объекта
                                                                DataType dataType, //Тип данных
                                                                string signalInf, //Настройки сигнала
                                                                bool needCut) //Нужно считывать срез значений
        {
            if (InitialSignals.ContainsKey(fullCode))
                return InitialSignals[fullCode];
            var sig = needCut ? new UniformSignal(this, fullCode, dataType, signalInf) 
                                      : new InitialSignal(this, fullCode, dataType, signalInf);
            var ob = AddObject(sig);
            ob.Context = codeObject;
            ProviderSignals.Add(fullCode, ob.AddSignal(sig));
            return InitialSignals.Add(fullCode, sig);
        }
        //Добавить объект содержащий заданный сигнал
        protected abstract SourceObject AddObject(InitialSignal sig);                                              

        //Добавить расчетный сигнал
        public CalcSignal AddCalcSignal(string fullCode, //Полный код сигнала
                                                         string initialSignal, //Код исходного сигнала
                                                         string formula) //Формула
        {
            if (CalcSignals.ContainsKey(fullCode))
                return CalcSignals[fullCode];
            if (!_initialSignals.ContainsKey(initialSignal))
                throw new InstanceNotFoundException("Не найден исходный сигнал " + initialSignal);
            var calc = new CalcSignal(fullCode, _initialSignals[initialSignal], formula);
            ProviderSignals.Add(fullCode, calc);
            return CalcSignals.Add(fullCode, calc);
        }

        //Очистка списка сигналов
        public void ClearSignals()
        {
            ProviderSignals.Clear();
            CalcSignals.Clear();
            InitialSignals.Clear();
            CloneSignalsId.Clear();
            ClearObjects();
        }
        //Очистка списков объектов
        protected abstract void ClearObjects();

        //Список сигналов, содержащих возвращаемые значения
        internal readonly DicS<SourceSignal> ProviderSignals = new DicS<SourceSignal>();
        public IDicSForRead<SourceSignal> Signals { get { return ProviderSignals; } }
        //Множество исходных сигналов
        private readonly DicS<InitialSignal> _initialSignals = new DicS<InitialSignal>();
        internal DicS<InitialSignal> InitialSignals { get { return _initialSignals; } }
        //Словарь расчетных сигналов
        private readonly DicS<CalcSignal> _calcSignals = new DicS<CalcSignal>();
        internal DicS<CalcSignal> CalcSignals { get { return _calcSignals; } }

        //Создание фабрики ошибок
        protected virtual IErrMomFactory MakeErrFactory()
        {
            var factory = new ErrMomFactory(Name, ErrMomType.Source);
            factory.AddGoodDescr(0);
            return factory;
        }
        //Хранилище ошибок 
        protected ErrMomPool ErrPool { get; private set; }

        //Создание ошибки 
        public ErrMom MakeError(int number, IContextable addr)
        {
            return ErrPool.MakeError(number, addr);
        }

        //Чтение значений из источника
        //Возвращает пару: количество прочитаных значений объектов и количество сформированных значений сигналов
        public ValuesCount GetValues(DateTime periodBegin, DateTime periodEnd)
        {
            var res = new ValuesCount();
            try
            {
                if (ErrPool == null)
                    ErrPool = new ErrMomPool(MakeErrFactory());
                foreach (var sig in ProviderSignals.Values)
                    sig.ClearMoments(periodBegin != PeriodEnd);
                PeriodBegin = periodBegin;
                PeriodEnd = periodEnd;

                if (NeedCut)
                    using (Start(5, PeriodBegin < PeriodEnd ? 30 : 100))
                    {
                        res.ReadCount = ReadCut();
                        Procent = 90;
                        res.WriteCount = 0;
                        foreach (var sig in InitialSignals.Values)
                            if (sig is UniformSignal)
                                ((UniformSignal) sig).MakeBegin();
                        AddEvent("Срез значений получен", res.ReadCount + " значений прочитано, " + res.WriteCount + " значений сформировано");
                    }

                if (!NeedCut || PeriodBegin < PeriodEnd)
                    using (Start(Procent, 90))
                    {
                        var changes = ReadChanges();
                        Procent = 90;
                        foreach (var sig in InitialSignals.Values)
                            if (sig is UniformSignal)
                                changes.WriteCount += ((UniformSignal)sig).MakeEnd();
                        AddEvent("Изменения значений получены", changes.ReadCount + " значений прочитано, " + changes.WriteCount + " значений сформировано");
                        res += changes;
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
                }
                AddEvent("Значения из источника прочитаны", res.ReadCount + " значений прочитано, " + res.WriteCount + " значений сформировано");
            }
            catch (Exception ex)
            {
                AddError("Ошибка при чтении значений из источника", ex);
            }
            return res;
        }

        //Чтение среза, возврашает количество прочитанных значений
        protected virtual int ReadCut() { return 0; }
        //Чтение изменений, возврашает количество прочитанных и сформированных значений
        protected abstract ValuesCount ReadChanges();

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
        private readonly DicI<UniformSignal> _cloneSignalsId = new DicI<UniformSignal>();
        public DicI<UniformSignal> CloneSignalsId { get { return _cloneSignalsId; } }
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
                foreach (var ed in ErrPool.UsedErrorDescrs)
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
            int k = m / CloneCutFrequency;
            var d = time.AddMinutes(-time.Minute).AddSeconds(-time.Second).AddMilliseconds(-time.Millisecond);
            return d.AddMinutes(k * m);
        }
        #endregion
    }

    //-----------------------------------------------------------------------------------------------------
    //Количество прочитанных и сформированных значений
    public class ValuesCount
    {
        public ValuesCount() { }
        public ValuesCount(int readCount, int writeCount)
        {
            ReadCount = readCount;
            WriteCount = writeCount;
        }

        //Количество прочитанных и сформированных значений
        public int ReadCount { get; set; }
        public int WriteCount { get; set; }

        //Покомпонентное сложение
        public static ValuesCount operator +(ValuesCount pair1, ValuesCount pair2)
        {
            return new ValuesCount(pair1.ReadCount + pair2.ReadCount, pair1.WriteCount + pair2.WriteCount);
        }
    }
}