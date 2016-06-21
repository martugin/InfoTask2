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
            Initialize();
        }
        protected SourceBase(string name, Logger logger) : base(name, logger)
        {
            Initialize();
        }

        private void Initialize()
        {
            NeedCut = true;
            CloneCutFrequency = 10;
            ErrPool = new ErrMomPool(MakeErrFactory());
        }

        //Соединение 
        public SourceConnect SourceConnect { get { return (SourceConnect) ProviderConnect; } }

        //Добавить сигнал
        public ISourceSignal AddSignal(string code, string context, DataType dataType, string signalInf, string formula = null)
        {
            if (ProviderSignals.ContainsKey(code))
                return ProviderSignals[code];
            var sig = new SourceSignal(this, code, dataType, signalInf);
            sig = AddObject(sig, context).AddSignal(sig);
            if (formula != null)
                return new CalcSignal(sig, code, dataType, formula);
            return sig;
        }
        //Добавить объект содержащий заданный сигнал
        protected abstract SourceObject AddObject(SourceSignal sig, string context);
        
        public virtual void ClearSignals()
        {
            ProviderSignals.Clear();
            CalcSignals.Clear();
            CloneObjects.Clear();
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
        //Частота сосздания срезов для клона в минутах, должно делить 60
        public int CloneCutFrequency { get; protected set; }

        //Рекордсет таблицы значений клона
        protected RecDao CloneRec { get; private set; }
        //Рекордсет таблицы  срезов клона
        protected RecDao CloneCutRec { get; private set; }
        //Рекордсет таблицы ошибок создания клона
        protected RecDao CloneErrorRec { get; private set; }

        //Словарь ошибочных объектов, ключи - коды объектов
        private readonly Dictionary<string, string> _errorObjects = new Dictionary<string, string>();
        protected Dictionary<string, string> ErrorObjects { get { return _errorObjects; }}

        //Добавляет объект в ErrorsObjects
        protected void AddErrorObject(string context, //context - описание сигнала
                                                        string errText, //errText - сообщение об ошибке
                                                        Exception ex = null) //ex - исключение
        {
            if (!ErrorObjects.ContainsKey(context))
            {
                var err = errText + (ex == null ? "" : (". " + ex.Message + ". " + ex.StackTrace));
                ErrorObjects.Add(context, err);
                CloneErrorRec.AddNew();
                CloneErrorRec.Put("SignalContext", context);
                CloneErrorRec.Put("ErrorDescription", err);
                CloneErrorRec.Update();
            }
        }

        //Словарь объектов клона, ключи Id в клоне
        private readonly DicI<SourceObject> _cloneObjects = new DicI<SourceObject>();
        public DicI<SourceObject> CloneObjects { get { return _cloneObjects; } }

        //Запись объектов в таблицу CloneSignals клона
        protected abstract void WriteObjectsToClone(RecDao rec);

        //Создание клона архива
        public void MakeClone(DateTime beginRead, //Начало периода клона
                                          DateTime endRead, //Конец периода клона
                                          string cloneFile) //Файл значений клона
        {
            try
            {
                using (var db = new DaoDb(cloneFile))
                {
                    using (var rec = new RecDao(db, "CloneSignals"))
                        WriteObjectsToClone(rec);
                    using (CloneRec = new RecDao(db, "SELECT * FROM CloneValues"))
                        using (CloneCutRec = new RecDao(db, "SELECT * FROM CutValues"))
                            using (CloneErrorRec = new RecDao(db, "SELECT * FROM ErrorsSignals"))
                                GetValues(beginRead, endRead);
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при создании клона", ex);
            }
            CloneRec = null;
        }
        #endregion
    }
}