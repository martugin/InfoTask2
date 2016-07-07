using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Источник с чтением значений из рекордсета
    public abstract class AdoSource : SourceBase
    {
        protected AdoSource()
        {
            NeedCut = true;
            CloneCutFrequency = 10;
            ReconnectsCount = 2;
            MaxErrorCount = 3;
            MaxErrorDepth = 3;
            ErrorWaiting = 100;
            SetReadProperties();
        }

        //Задание стандартных свойств получения данных
        protected virtual void SetReadProperties() { }

        //Свойства чтения сигналов по блокам
        protected int ReconnectsCount { private get; set; } //Сколько раз повторять считывание одного блока
        protected int MaxErrorCount { private get; set; } //Количество блоков, которое нужно считать, чтобы понять, что связи нет, 0 - читать до конца
        protected int MaxErrorDepth { private get; set; } //Глубина, до которой нужно дробить первый блок, если так и не было успешного считывания
        protected int ErrorWaiting { private get; set; } //Время ожидания после ошибки считывания в мс

        //Общее количество прочитанных и сформированных значений
        protected int NumRead { get; set; }
        protected int NumWrite { get; set; }

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
                if (!IsConnected && !Check()) return;
                NumRead = NumWrite = 0;

                AddEvent(msg ?? ("Чтение " + (isCut ? "среза" : "изменений") + " значений сигналов"), n + " объектов, " + beg + " - " + en);
                var parts = MakeParts(objects, partSize, isCut);
                var successRead = false;

                double d = 100.0 / parts.Count;
                for (int i = 0; i < parts.Count; i++)
                    using (Start(i * d, i * d + d, CommandFlags.Danger))
                    {
                        if (i < ReconnectsCount)
                        {
                            IsConnected = successRead = ReadPart(parts[i], beg, en, isCut);
                            if (!successRead)
                            {
                                Thread.Sleep(ErrorWaiting);
                                successRead |= ReadPartRecursive(parts[i], true, 1, beg, en, isCut, false);
                            }
                        }
                        else if (i < MaxErrorCount || successRead)
                            successRead |= ReadPartRecursive(parts[i], successRead, 1, beg, en, isCut, successRead);
                    }
                int nadd = 0;
                var signals = InitialSignals.Values;
                if (isCut)
                {
                    nadd += signals.Sum(sig => sig.MakeBegin());
                    AddEvent("Сформирован срез", nadd + " значений сформировано");
                }
                else
                {
                    foreach (var sig in signals)
                        sig.MakeEnd();
                    AddEvent("Добавлены значения в конец интервала");
                }
                NumWrite += nadd;

                AddErrorObjectsWarning();
                IsConnected &= successRead;
                if (successRead)
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
                                                                          int partSize, //Размер одного блока
                                                                          bool isCut) //Выполняется считываение среза
        {
            var parts = new List<List<SourceObject>>();
            int i = 0;
            List<SourceObject> part = null;
            foreach (var ob in objects)
                if (!isCut || !ob.HasBegin)
                {
                    if (i++ % partSize == 0)
                        parts.Add(part = new List<SourceObject>());
                    part.Add(ob);
                }
            return parts;
        }
        
        //Чтение значений по одному блоку списка объектов
        protected bool ReadPart(List<SourceObject> part,
                                             DateTime beg, DateTime en, //Период считывания
                                             bool isCut) //Считывается срез
        {
            IRecordRead rec;
            using (Start(0, 50))
            {
                try
                {
                    AddEvent("Чтение значений блока объектов", part.Count + " объектов");
                    rec = QueryPartValues(part, beg, en, isCut);
                    if (rec == null) return IsConnected = false;
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
                    using (rec)
                    {
                        AddEvent("Распределение данных по сигналам", part.Count + " объектов");
                        Tuple<int, int> pair = ReadPartValues(rec);
                        NumRead += pair.Item1;
                        NumWrite += pair.Item2;
                        AddEvent("Значения блока объектов прочитаны", pair.Item1 + " значений прочитано, " + pair.Item2 + " значений сформировано");    
                    }
                }
                catch (Exception ex)
                {
                    AddError("Ошибка при формировании значений", ex);
                }
            }
            return true;
        }

        //Считывает значения по блоку сигналов, в случае ошибки рекурсивно считает для половин блока
        private bool ReadPartRecursive(List<SourceObject> part, //Блок сигналов 
                                                       bool useRecursion, //Использовать рекурсивный вызов
                                                       int depth, //Глубина в дереве вызовов, начиная с 1
                                                       DateTime beg, DateTime en, //Период считывания
                                                       bool isCut, //Считывается срез 
                                                       bool successRead) //Хотя бы одно из чтений значений было успешным
        { 
            if (!IsConnected && !Check()) return false;
            bool b = ReadPart(part, beg, en, isCut);
            if (b) return true;
            if (part.Count == 1 || !useRecursion || (!successRead && depth >= MaxErrorDepth))
            {
                foreach (var ob in part)
                    AddErrorObject(ob.Context, Command.ErrorMessage(false, true, false));
                return false;
            }
            Thread.Sleep(ErrorWaiting);
            int m = part.Count / 2;
            bool b1 = ReadPartRecursive(part.GetRange(0, m), true, depth + 1, beg, en, isCut, successRead);
            if (!b1) Thread.Sleep(ErrorWaiting);
            bool b2 = ReadPartRecursive(part.GetRange(m, part.Count - m), true, depth + 1, beg, en, isCut, successRead || b1);
            if (!b2) Thread.Sleep(ErrorWaiting);
            return b1 || b2;
        }

        //Запрос рекордсета по одному блоку, возвращает запрошенный рекорсет, или null при неудаче
        protected abstract IRecordRead QueryPartValues(List<SourceObject> part, //список объектов
                                                         DateTime beg, DateTime en, //период считывания
                                                         bool isCut); //считывается срез

        //Чтение значений из рекордсета по одному блоку
        protected Tuple<int, int> ReadPartValues(IRecordRead rec)
        {
            int nread = 0, nwrite = 0;
            while (rec.Read())
            {
                nread++;
                SourceObject ob = null;
                try
                {
                    ob = DefineObject(rec);
                    if (ob != null)
                        nwrite += ob.ReadMoments(rec);
                }
                catch (Exception ex)
                {
                    AddErrorObject(ob == null ? "" : ob.Context, "Ошибка при чтении значений из рекордсета", ex);
                }
            }
            return new Tuple<int, int>(nread, nwrite);
        }

        //Определение текущего считываемого объекта
        protected abstract SourceObject DefineObject(IRecordRead rec);
    }
}