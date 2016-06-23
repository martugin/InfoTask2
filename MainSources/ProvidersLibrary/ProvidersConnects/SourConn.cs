using System;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Соединение - источник
    public abstract class SourConn : ProvConn
    {
        protected SourConn()
        {
            CloneCutFrequency = 10;
            ErrPool = new ErrMomPool(MakeErrFactory());
        }

        //Тип провайдера
        public override ProviderType Type { get { return ProviderType.Source;}}

        //Текущий провайдер-источник
        private SourBase _source;
        public SourBase Source 
        { 
            get
            {
                if (_source == null && MainProvider != null)
                    _source = (SourBase)MainProvider;
                return _source;
            }
        }

        //Получение диапазона времени источника
        public TimeInterval GetTime()
        {
            return Source.GetTime();
        }

        //Добавить сигнал
        public SourSignal AddSignal(string code, string codeObject, DataType dataType, string signalInf, string formula = null)
        {
            if (ProviderSignals.ContainsKey(code))
                return ProviderSignals[code];
            var sig = new SourInitSignal(this, code, dataType, signalInf);
            sig = AddObject(sig, codeObject).AddSignal(sig);
            if (formula != null)
                return new SourCalcSignal(this, sig, code, dataType, formula);
            return sig;
        }
        //Добавить объект содержащий заданный сигнал
        protected abstract SourObject AddObject(SourInitSignal sig, string context);

        //Очистка списка сигналов
        public virtual void ClearSignals()
        {
            ProviderSignals.Clear();
            CalcSignals.Clear();
            CloneSignalsId.Clear();
        }

        //Откуда и куда грузятся значения при чтении из источника
        protected ValuesDirection ValuesDirection { get; private set; }

        //Список сигналов, содержащих возвращаемые значения
        internal readonly DicS<SourInitSignal> ProviderSignals = new DicS<SourInitSignal>();
        public IDicSForRead<SourInitSignal> Signals { get { return ProviderSignals; } }
        //Словарь расчетных сигналов
        private readonly DicS<SourCalcSignal> _calcSignals = new DicS<SourCalcSignal>();
        public DicS<SourCalcSignal> CalcSignals { get { return _calcSignals; } }
        //Словарь сигналов клона, ключи Id в клоне, используется и при чтении из клона, и при записи в клон
        private readonly DicI<SourInitSignal> _cloneSignalsId = new DicI<SourInitSignal>();
        public DicI<SourInitSignal> CloneSignalsId { get { return _cloneSignalsId; } }

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
            foreach (var sig in ProviderSignals.Values)
                sig.ClearMoments(periodBegin != PeriodEnd);
            PeriodBegin = periodBegin;
            PeriodEnd = periodEnd;
            Source.GetValues();
        }
        
        //Создание клона
        #region
        //Частота в минутах фиксации среза в клоне, должна делить 60
        public int CloneCutFrequency { get; private set; }

        //Рекордсеты таблиц значений клона
        internal RecDao CloneRec { get; private set; }
        internal RecDao CloneCutRec { get; private set; }
        internal RecDao CloneStrRec { get; private set; }
        internal RecDao CloneStrCutRec { get; private set; }
        //Рекордсет таблицы ошибок создания клона
        internal RecDao CloneErrorsRec { get; private set; }

        //Словарь ошибочных объектов, ключи - коды объектов
        private readonly DicS<string> _errorObjects = new DicS<string>();
        protected DicS<string> ErrorObjects { get { return _errorObjects; } }

        //Добавляет объект в ErrorsObjects
        protected void AddErrorObject(string codeObject,  //Код сигнала
                                                        string errText,        //Сообщение об ошибке
                                                        Exception ex = null)  //Исключение
        {
            if (!ErrorObjects.ContainsKey(codeObject))
            {
                var err = errText + (ex == null ? "" : (". " + ex.Message + ". " + ex.StackTrace));
                ErrorObjects.Add(codeObject, err);
                CloneErrorsRec.AddNew();
                CloneErrorsRec.Put("CodeObject", codeObject);
                CloneErrorsRec.Put("ErrorDescription", err);
                CloneErrorsRec.Update();
            }
        }

        //Чтение Id сигналов клона
        protected void ReadCloneSignalsId(DaoDb cloneDb)
        {
            using (var rec = new RecDao(cloneDb, "Signals"))
                while (rec.Read())
                {
                    var code = rec.GetString("FullCode");
                    if (ProviderSignals.ContainsKey(code))
                        ProviderSignals[code].IdInClone = rec.GetInt("SignalId");
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