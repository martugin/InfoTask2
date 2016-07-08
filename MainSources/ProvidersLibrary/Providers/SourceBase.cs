using System;
using System.Collections.Generic;
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

        //Добавить сигнал
        public SourceSignal AddSignal(string code, string codeObject, DataType dataType, string signalInf, string formula = null)
        {
            if (ProviderSignals.ContainsKey(code))
                return ProviderSignals[code];

            var sig = new InitialSignal(this, dataType, signalInf);
            var ob = this is CloneSource 
                         ? ((CloneSource)this).AddCloneObject(sig, code) 
                         : AddObject(sig);
            ob.Context = codeObject;
            sig = ob.AddSignal(sig);
            if (!_initialSignals.Contains(sig))
                _initialSignals.Add(sig);
            if (formula == null)
                return ProviderSignals.Add(code, sig);
            
            var calc = new CalcSignal(sig, formula);
            ProviderSignals.Add(code, calc);
            return CalcSignals.Add(code, calc);
        }
        //Добавить объект содержащий заданный сигнал
        protected abstract SourceObject AddObject(InitialSignal sig);

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
        private readonly HashSet<InitialSignal> _initialSignals = new HashSet<InitialSignal>();
        protected HashSet<InitialSignal> InitialSignals { get { return _initialSignals; } }
        //Словарь расчетных сигналов
        private readonly DicS<CalcSignal> _calcSignals = new DicS<CalcSignal>();
        public DicS<CalcSignal> CalcSignals { get { return _calcSignals; } }

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
        public void GetValues(DateTime periodBegin, DateTime periodEnd)
        {
            if (ErrPool == null)
                ErrPool = new ErrMomPool(MakeErrFactory());
            foreach (var sig in ProviderSignals.Values)
                sig.ClearMoments(periodBegin != PeriodEnd);
            PeriodBegin = periodBegin;
            PeriodEnd = periodEnd;
            if (NeedCut)
                Start(ReadCut, 0, PeriodBegin < PeriodEnd ? 30 : 100);
            if (!NeedCut || PeriodBegin < PeriodEnd)
                Start(ReadChanges, Procent, 100);
            foreach (var sig in CalcSignals.Values)
                sig.Calculate();
        }

        //Чтение среза
        protected virtual void ReadCut() { }
        //Чтение изменений
        protected abstract void ReadChanges();

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
                    if (ProviderSignals.ContainsKey(code) && ProviderSignals[code] is InitialSignal)
                        ((InitialSignal)ProviderSignals[code]).IdInClone = rec.GetInt("SignalId");
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
                    using (CloneRec = new RecDao(db, "SELECT * FROM MomentValues"))
                    using (CloneCutRec = new RecDao(db, "SELECT * FROM MomentValuesCut"))
                    using (CloneStrRec = new RecDao(db, "SELECT * FROM MomentStrValues"))
                    using (CloneStrCutRec = new RecDao(db, "SELECT * FROM MomentStrValuesCut"))
                    using (CloneErrorsRec = new RecDao(db, "SELECT * FROM ErrorsSignals"))
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
}