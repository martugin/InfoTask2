using System;
using System.Collections.Generic;
using BaseLibrary;

namespace CommonTypes
{
    //Базовый класс для всех источников
    public abstract class SourceBase : ProviderBase, ISource
    {
        protected SourceBase()
        {
            NeedCut = true;
            ErrPool = new ErrMomPool(MakeErrFactory());
        }
        protected SourceBase(string name, Logger logger) 
            : base(name, logger)
        {
            NeedCut = true;
            ErrPool = new ErrMomPool(MakeErrFactory());
        }

        //Соединение 
        public SourceConnect SourceConnect { get { return (SourceConnect) ProviderConnect; } }

        //Добавить сигнал
        public ISourceSignal AddSignal(string code, string context, DataType dataType, string signalInf, bool skipRepeats = false, string formula = null)
        {
            if (ProviderSignals.ContainsKey(code))
                return ProviderSignals[code];
            var sig = new SourceSignal(this, code, dataType, signalInf, skipRepeats);
            sig = AddObject(sig).AddSignal(sig);
            if (formula != null)
                return new CalcSignal(sig, code, dataType, formula);
            return sig;
        }
        //Добавить объект по содержащий заданный сигнал
        protected abstract SourceObject AddObject(SourceSignal sig);
        
        public virtual void ClearSignals()
        {
            ProviderSignals.Clear();
            CalcSignals.Clear();
        }

        //Список сигналов, содержащих возвращаемые значения
        protected readonly DicS<SourceSignal> ProviderSignals = new DicS<SourceSignal>();
        public IDicSForRead<SourceSignal> Signals { get { return ProviderSignals; } }
        //Словарь расчетных сигналов
        private readonly DicS<SourceSignal> _calcSignals = new DicS<SourceSignal>();
        public DicS<SourceSignal> CalcSignals { get { return _calcSignals; } }
        //Словарь объектов с ключами Id для клона и для источников, где ключ объекта - число
        private readonly DicI<SourceObject> _objectsId = new DicI<SourceObject>();
        public DicI<SourceObject> ObjectsId { get { return _objectsId; }}

        //Подготовка объектов в файле клона к заполнению
        public virtual void PropareClone() {}

        //Создание фабрики ошибок
        protected virtual IErrMomFactory MakeErrFactory()
        {
            var factory = new ErrMomFactory(SourceConnect.Code, ErrMomType.Source);
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

        //Нужно считывать срез
        protected bool NeedCut { get; set; }

        //Чтение данных из архива
        public virtual void GetValues(DateTime periodBegin, DateTime periodEnd)
        {
            foreach (var sig in ProviderSignals.Values)
                sig.ClearMoments(periodBegin != PeriodEnd);
            PeriodBegin = periodBegin;
            PeriodEnd = periodEnd;
            Procent = 5;

            if (NeedCut)
                Start(ReadCut, 0, PeriodBegin < PeriodEnd ? 30 : 100);
            if (!NeedCut || PeriodBegin < PeriodEnd)
                Start(ReadChanges, Procent, 100);
        }
        
        //Чтение среза
        protected virtual void ReadCut() {}
        //Чтение изменений
        protected virtual void ReadChanges() {}
        
        //Создание клона
        #region
        //Рекордсет таблицы значений клона
        public RecDao CloneRec { get; private set; }
        //Рекордсет таблицы ошибок создания клона
        protected RecDao CloneErrorRec { get; private set; }

        //Настройки создания клона
        protected DicS<string> CloneInf { get; private set; }
        //Словарь ошибочных объектов, ключи - коды объектов
        private readonly Dictionary<string, string> _errorObjects = new Dictionary<string, string>();
        protected Dictionary<string, string> ErrorObjects { get { return _errorObjects; }}

        //Добавляет объект в ErrorObjects, inf - описание сигнала, errText - сообщение ошибки, ex - исключение
        protected void AddErrorObject(string inf, string errText, Exception ex = null)
        {
            if (!ErrorObjects.ContainsKey(inf))
                ErrorObjects.Add(inf, errText + (ex == null ? "" : (". " + ex.Message + ". " + ex.StackTrace)));
        }

        //Создание клона архива
        public void MakeClone(DateTime beginRead, DateTime endRead, string cloneFile = "", string cloneProps = "")
        {
            try
            {
                using (var db = new DaoDb(cloneFile))
                {
                    using (var sys = new SysTabl(db))
                        CloneInf = (sys.Value("CloneInf") ?? "").ToPropertyDicS();
                    using (CloneRec = new RecDao(db, "SELECT * FROM MomentsValues"))
                        using (CloneErrorRec = new RecDao(db, "SELECT * FROM ErrorsList"))
                            GetValues(beginRead, endRead);
                    WriteErrors(db);
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при создании клона", ex);
            }
            CloneRec = null;
        }

        //Запись в клон списка ошибок
        private void WriteErrors(DaoDb db)
        {
            using (var rec = new RecDao(db, "MomentsError"))
                foreach (var err in ErrPool.UsedErrorDescrs)
                    err.ToRecordset(rec);
        }
        #endregion

        
    }
}