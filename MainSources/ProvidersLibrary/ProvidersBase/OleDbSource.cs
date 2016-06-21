using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Threading;
using BaseLibrary;

namespace CommonTypes
{
    //OleDb-источник с чтением значений сигналов по блокам
    public abstract class OleDbSource : SourceBase
    {
        protected OleDbSource()
        {
            SetReadProperties();
        }
        protected OleDbSource(string name, Logger logger) : base(name, logger)
        {
            SetReadProperties();
        }

        //Соединение с OleDb
        protected OleDbConnection Connection { get { return ((OleDbSourceConnect) ProviderConnect).Connection; } }
        //Соединение установлено
        protected bool IsConnected
        {
            get { return ((OleDbSourceConnect) ProviderConnect).IsConnected; } 
            set { ((OleDbSourceConnect) ProviderConnect).IsConnected = value; }
        }
        //Открытие соединения
        protected bool Connect()
        {
            return ((OleDbSourceConnect) ProviderConnect).Connect();
        }
        
        //Свойства чтения сигналов по блокам
        protected int ReconnectsCount { private get; set; } //Сколько раз повторять считывание одного блока
        protected int MaxErrorCount { private get; set; } //Количество блоков, которое нужно считать, чтобы понять, что связи нет, 0 - читать до конца
        protected int MaxErrorDepth { private get; set; } //Глубина, до которой нужно дробить первый блок, если так и не было успешного считывания
        protected int ErrorWaiting { private get; set; } //Время ожидания после ошибки считывания в мс

        //Задание стандартных свойств получения данных
        protected virtual void SetReadProperties()
        {
            NeedCut = true;
            ReconnectsCount = 2;
            MaxErrorCount = 3;
            MaxErrorDepth = 3;
            ErrorWaiting = 100;
            CloneCutFrequency = 10;
        }

        //Выполняется чтение среза данных
        protected bool IsCutReading { get; set; }

        //Общее количество прочитанных и сформированных значений
        protected int NumRead { get; set; }
        protected int NumWrite { get; set; }

        //Период считывания данных по текущему блоку 
        private DateTime _begin;
        private DateTime _end;
        //Рекордсет, запрашиваемый из архива
        protected IRecordRead Rec;

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

                NumRead = NumWrite = 0;
                _begin = beg;
                _end = en;
                IsCutReading = isCut;
                AddEvent(msg ?? ("Чтение " + (isCut ? "среза" : "изменений") + " значений сигналов"), n + " объектов, " + beg + " - " + en);
                var parts = MakeParts(objects, partSize);

                double d = 100.0 / parts.Count;
                for (int i = 0; i < parts.Count; i++)
                    using (Start(i * d, i * d + d, CommandFlags.Danger))
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
                    nadd += ProviderSignals.Values.Sum(sig => sig.MakeBegin());
                    AddEvent("Сформирован срез", nadd + " значений сформировано");
                }
                else
                {
                    nadd += ProviderSignals.Values.Sum(sig => sig.MakeEnd());
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
                : objects.Count(ob => ob.HasBegin);
        }

        //Разбиение списка объектов на блоки
        private List<List<SourceObject>> MakeParts(IEnumerable<SourceObject> objects, //Список объектов
                                                                            int partSize) //Размер одного блока
        {
            var parts = new List<List<SourceObject>>();
            int i = 0;
            List<SourceObject> part = null;
            foreach (var ob in objects)
                if (!IsCutReading || !ob.HasBegin)
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
            if (ErrorObjects.Count > 0)
            {
                int i = 0;
                string s = "";
                foreach (var ob in ErrorObjects)
                    if (++i < 10) s += ob.Key + ", ";
                AddWarning("Не удалось прочитать значения по некоторым объектам", null,
                           s + (ErrorObjects.Count > 10 ? " и др." : "") + "всего " + ErrorObjects.Count + " объектов не удалось прочитать");
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
                    if (!QueryPartValues(part, _begin, _end))
                        return IsConnected = false;
                }
                catch (Exception ex)
                {
                    AddError("Ошибка при запросе данных из источника", ex);
                    return IsConnected = false;
                }
            }
            using (Start(50, 100))
            {
                try
                {
                    AddEvent("Распределение данных по сигналам", part.Count + " объектов");
                    Tuple<int, int> pair = ReadPartValues();
                    NumRead += pair.Item1;
                    NumWrite += pair.Item2;
                    AddEvent("Значения блока объектов прочитаны", pair.Item1 + " значений прочитано, " + pair.Item2 + " значений сформировано");
                }
                catch (Exception ex)
                {
                    AddError("Ошибка при формировании значений", ex);
                }
            }
            return true;
        }

        //Хотя бы одно из чтений значений было успешным
        private bool _successRead;

        //Считывает значения по блоку сигналов, в случае ошибки рекурсивно считает для половин блока
        private bool ReadPartRecursive(List<SourceObject> part, //Блок сигналов 
                                                       bool useRecursion, //useRecursion - использовать рекурсивный вызов
                                                       int depth) //depth - глубина в дереве вызовов, начиная с 1
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

        //Запрос рекордсета по одному блоку, рекорсет должен быть записан в свойство Rec
        protected virtual bool QueryPartValues(List<SourceObject> part, //список объектов
                                                                   DateTime beg, //период считывания
                                                                   DateTime en)
        {
            Rec = null;
            return true;
        }

        //Чтение значений из рекордсета по одному блоку
        protected Tuple<int, int> ReadPartValues()
        {
            int nread = 0, nwrite = 0;
            while (Rec.Read())
            {
                nread++;
                SourceObject ob = null;
                try
                {
                    ob = DefineObject();
                    if (ob != null)
                    {
                        if (CloneRec == null)
                            nwrite += ob.MakeValueFromRec(Rec);
                        else nwrite += ob.ReadValueToClone(Rec, CloneRec, CloneCutRec);
                    }
                }
                catch (Exception ex)
                {
                    AddErrorObject(ob == null ? "" : ob.Inf, "Ошибка при чтении значений из рекордсета", ex);
                }
            }
            return new Tuple<int, int>(nread, nwrite);
        }
        
        //Определение текущего считываемого объекта
        protected virtual SourceObject DefineObject()
        {
            return null;
        }

        //Чтение значений по одному объекту из рекордсета источника
        //Возвращает количество сформированных значений
        protected virtual int ReadObjectValue(SourceObject obj)
        {
            return 0;
        }

        //Освобождение ресурсов, занятых провайдером
        public override void Dispose()
        {
            try { if (Rec != null) Rec.Dispose(); }
            catch { }
            try
            {
                if (ProviderConnect != null)
                    ProviderConnect.Dispose();
            }
            catch { }
        }
    }
}