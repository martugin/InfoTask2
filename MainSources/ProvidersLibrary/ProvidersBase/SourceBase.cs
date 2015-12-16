using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BaseLibrary;

namespace CommonTypes
{
    //Базовый класс для всех источников
    public abstract class SourceBase : ProviderBase
    {
        protected SourceBase()
        {
            SetDefaultReadProperties();
            ErrPool = new ErrMomPool(MakeErrFactory());
        }
        protected SourceBase(string name, Logger logger) : base(name, logger)
        {
            SetDefaultReadProperties();
            ErrPool = new ErrMomPool(MakeErrFactory());
        }

        //Тип провайдера
        public override ProviderType Type { get { return ProviderType.Source; } }
        
        //True, если соединение прошло успешно, становится False, если произошла ошибка
        protected bool IsConnected { get; set; }
        //Открытие соединения
        protected virtual bool Connect()
        {
            return true;
        }

        //Хранилище ошибок 
        protected ErrMomPool ErrPool { get; private set; }
        //Создание фабрики ошибок
        protected abstract IErrMomFactory MakeErrFactory();
        //Создание ошибки 
        protected ErrMom MakeError(int number, IContextable addr)
        {
            return ErrPool.MakeError(number, addr);
        }
        
        //Список сигналов, содержащих возвращаемые значения
        protected readonly DicS<SourceSignal> ProviderSignals = new DicS<SourceSignal>();
        public IDicSForRead<SourceSignal> Signals { get { return ProviderSignals; } }

        //Чтение данных из архива
        public void GetValues(DateTime beginRead, DateTime endRead)
        {
            foreach (var sig in ProviderSignals.Values)
                sig.Value = new MomList(sig.DataType);
            BeginRead = beginRead;
            EndRead = endRead;

            SetReadProperties();
            if (NeedCut)
                Start(ReadCut, 0, BeginRead < EndRead ? 30 : 100);
            if (!NeedCut || BeginRead < EndRead)
                Start(ReadChanges, Procent, 100);
        }
        protected DateTime BeginRead { get; private set; }
        protected DateTime EndRead { get; private set; }
        
        //Задание нестандартных свойств получения данных
        protected virtual void SetReadProperties() {}
        //Чтение среза
        protected virtual void ReadCut() {}
        //Чтение изменений
        protected virtual void ReadChanges() {}

        //Начало диапазона источника
        protected DateTime BeginTime { get; set; }
        //Конец диапазона источника
        protected DateTime EndTime { get; set; }
        //Список временных интервалов диапазона источника
        private readonly List<TimeInterval> _timeIntervals = new List<TimeInterval>();
        public List<TimeInterval> TimeIntervals { get { return _timeIntervals; } }
        
        //Создание клона
        #region
        //Рекордсет таблицы значений клона
        public RecDao CloneRec { get; private set; }
        //Рекордсет таблицы ошибок создания клона
        private RecDao _cloneErrorRec;

        //Настройки создания клона
        protected DicS<string> CloneInf { get; private set; }
        //Словарь ошибочных объектов, ключи - коды объектов
        private readonly Dictionary<string, string> _errorObjects = new Dictionary<string, string>();
        //Добавляет объект в ErrorObjects, inf - описание сигнала, errText - сообщение ошибки, ex - исключение
        protected void AddErrorObject(string inf, string errText, Exception ex = null)
        {
            if (!_errorObjects.ContainsKey(inf))
                _errorObjects.Add(inf, errText + (ex == null ? "" : (". " + ex.Message + ". " + ex.StackTrace)));
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
                        using (_cloneErrorRec = new RecDao(db, "SELECT * FROM ErrorsList"))
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
                foreach (var err in ErrPool.UsedErrors)
                    err.ToRecordset(rec);
        }
        #endregion

        //Чтение значений сигналов по блокам
        #region
        //Начало и конец текущего получения данных
        private DateTime _begin;
        private DateTime _end;
        //Выполняется чтение среза данных
        private bool _isCut;

        //Нужно считывать срез
        protected bool NeedCut { private get; set; }
        //Свойства чтения сигналов по блокам
        protected int ReconnectsCount { private get; set; } //Сколько раз повторять считывание одного блока
        protected int MaxErrorCount { private get; set; } //Количество блоков, которое нужно считать, чтобы понять, что связи нет, 0 - читать до конца
        protected int MaxErrorDepth { private get; set; } //Глубина, до которой нужно дробить первый блок, если так и не было успешного считывания
        protected int ErrorWaiting { private get; set; } //Время ожидания после ошибки считывания в мс

        private void SetDefaultReadProperties()
        {
            NeedCut = true;
            ReconnectsCount = 2;
            MaxErrorCount = 3;
            MaxErrorDepth = 3;
            ErrorWaiting = 100;
        }

        //Общее количество прочитанных и сформированных значений
        protected int NumRead { get; set; }
        protected int NumWrite { get; set; }

        //Запрос рекордсета по одному блоку
        protected virtual bool QueryPartValues(List<SourceObject> part, DateTime beg, DateTime en, bool isCut) 
        {
            return true;
        }
        //Чтение значений из рекордсета по одному блоку
        protected virtual Tuple<int, int> ReadPartValues(bool isCut)
        {
            return new Tuple<int, int>(0, 0);
        }

        //Чтение значений по блокам объектов
        protected void ReadValuesByParts(IEnumerable<SourceObject> objects, //список объектов
                                                           int partSize, //размер одного блока
                                                           DateTime beg, //период считывания
                                                           DateTime en,
                                                           bool isCut,  //считывается срез
                                                           string msg = null) //Сообщение для истории о запуске чтения данных
        {
            try
            {
                int n = ObjectsToReadCount(objects, isCut);
                if (n == 0)
                {
                    AddEvent("Пустой список объектов для считывания" + (isCut ? " среза" : ""), beg + " - " + en);
                    return;    
                }
                if (!IsConnected && !Connect()) return;

                //_successRead = false;
                NumRead = NumWrite = 0;
                _begin = beg;
                _end = en;
                _isCut = isCut;
                AddEvent(msg ?? ("Чтение " + (isCut ? "среза" : "изменений") + " значений сигналов"), n + " объектов, " + beg + " - " + en);
                var parts = MakeParts(objects, partSize);

                double d = 100.0/parts.Count;
                for (int i = 0; i < parts.Count; i++)
                    using (Start(i*d, i*d + d, CommandFlags.Danger))
                    {
                        if (i < ReconnectsCount)
                        {
                            IsConnected = _successRead = ReadPart(parts[i]);
                            if (!_successRead)
                            {
                                Thread.Sleep(ErrorWaiting);
                                _successRead |= ReadPartRecursive(parts[i], true, 1);
                            }
                        }
                        else if (i < MaxErrorCount || _successRead)
                            _successRead |= ReadPartRecursive(parts[i], _successRead, 1);
                    }
                int nadd = 0;
                if (isCut)
                {
                    nadd += ProviderSignals.Values.Sum(sig => sig.MakeBegin(BeginRead));
                    AddEvent("Сформирован срез", nadd + " значений сформировано");
                }
                else
                {
                    nadd += ProviderSignals.Values.Sum(sig => sig.MakeEnd(EndRead));
                    AddEvent("Добавлены значения в конец интервала", nadd + " значений сформировано");
                }
                NumWrite += nadd;

                WriteErrorObjects();
                IsConnected &= _successRead;
                if (_successRead)
                    AddEvent("Значения из источника прочитаны", NumRead + " значений прочитано, " + NumWrite + " значений сформировано");
            }
            catch (Exception ex)
            {
                AddError("Ошибка при получении данных", ex);
                IsConnected = false;
            }
        }

        //Количество объектов, для чтения значений
        private int ObjectsToReadCount(IEnumerable<SourceObject> objects, bool isCut)
        {
            return !isCut 
                ? objects.Count() 
                : objects.Count(ob => ob.HasBegin(BeginRead));
        }

        //Разбиение списка объектов на блоки
        private List<List<SourceObject>> MakeParts(IEnumerable<SourceObject> objects, //Список объектов
                                                                            int partSize) //Размер одного блока
        {
            var parts = new List<List<SourceObject>>();
            int i = 0;
            List<SourceObject> part = null;
            foreach (var ob in objects)
                if (!_isCut || !ob.HasBegin(BeginRead))
                {
                    if (i++ % partSize == 0)
                        parts.Add(part = new List<SourceObject>());
                    part.Add(ob);
                }
            return parts;
        }

        //Запись списка непрочитанных объектов в ErrorList клона и лог
        private void WriteErrorObjects()
        {
            if (_errorObjects.Count > 0)
            {
                int i = 0;
                string s = "";
                foreach (var ob in _errorObjects)
                {
                    if (++i < 10) s += ob.Key + ", ";
                    if (_cloneErrorRec != null)
                    {
                        _cloneErrorRec.AddNew();
                        _cloneErrorRec.Put("Signal", ob.Key);
                        _cloneErrorRec.Put("ErrorDescription", ob.Value);
                        _cloneErrorRec.Update();
                    }
                }
                AddWarning("Не удалось прочитать значения по некоторым объектам", null,
                           s + (_errorObjects.Count > 10 ? " и др." : "") + "всего " + _errorObjects.Count + " объектов не удалось прочитать");
            }
        }

        //Чтение значений по одному блоку
        private bool ReadPart(List<SourceObject> part)
        {
            using (Start(0, 50))
            {
                try
                {
                    AddEvent("Чтение значений блока объектов", part.Count + " объектов");
                    if (!QueryPartValues(part, _begin, _end, _isCut))
                        return IsConnected = false;
                }
                catch (Exception ex)
                {
                    AddError("Ошибка при запросе данных из источника", ex);
                    return IsConnected = false;
                }
            }
            try
            {
                AddEvent("Распределение данных по сигналам", part.Count + " объектов");
                Tuple<int, int> pair = ReadPartValues(_isCut);
                NumRead += pair.Item1;
                NumWrite += pair.Item2;
                AddEvent("Значения блока объектов прочитаны", pair.Item1 + " значений прочитано, " + pair.Item2 + " значений сформировано");
            }
            catch (Exception ex)
            {
                AddError("Ошибка при формировании значений", ex);
            }
            return true;
        }

        //Хотя бы одно из чтений значений было успешным
        private bool _successRead;

        //Считывает значения по блоку сигналов part, используя функцию _funReadValues, в случае ошибки читает блок по частям
        //useRecursion - использовать рекурсивный вызов, depth - глубина в дереве вызовов, начиная с 1
        private bool ReadPartRecursive(List<SourceObject> part, bool useRecursion, int depth)
        {
            if (!IsConnected && !Connect()) return false;
            bool b = ReadPart(part);
            if (b) return true;
            if (part.Count == 1 || !useRecursion || (!_successRead && depth >= MaxErrorDepth))
            {
                foreach (var ob in part)
                    AddErrorObject(ob.Inf, Command.ErrorMessage(false, true, false));
                return false;
            }
            Thread.Sleep(ErrorWaiting);
            int m = part.Count / 2;
            bool b1 = ReadPartRecursive(part.GetRange(0, m), true, depth + 1);
            _successRead |= b1;
            if (!b1) Thread.Sleep(ErrorWaiting);
            bool b2 = ReadPartRecursive(part.GetRange(m, part.Count - m), true, depth + 1);
            if (!b2) Thread.Sleep(ErrorWaiting);
            return b1 || b2;
        }
        #endregion
    }
}