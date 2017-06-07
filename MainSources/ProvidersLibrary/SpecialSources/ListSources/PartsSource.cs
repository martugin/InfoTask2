using System;
using System.Collections.Generic;

namespace ProvidersLibrary
{
    //Источник c функцией чтения по блокам
    public abstract class PartsSource : ListSource
    {
        //Чтение значений по блокам объектов
        protected ValuesCount ReadByParts(IEnumerable<ListSourceOut> objects, //список объектов
                                                           int partSize, //Размер одного блока
                                                           DateTime beg, DateTime en, //Период считывания
                                                           bool isCut,  //Считывается срез
                                                           Func<List<ListSourceOut>, DateTime, DateTime, bool, ValuesCount> readPartFun, //Функция чтения значений по одному блоку 
                                                           string msg = null) //Сообщение для истории о запуске чтения данных
        {
            var valuesCount = new ValuesCount();
            try
            {
                int n;
                var parts = MakeParts(objects, partSize, isCut, out n);
                if (n == 0)
                {
                    AddEvent("Пустой список объектов для считывания" + (isCut ? " среза" : ""), beg + " - " + en);
                    return valuesCount;
                }
                if (!Connect()) return valuesCount.AddStatus(VcStatus.Fail);
                AddEvent(msg ?? "Чтение " + (isCut ? "среза" : "изменений") + " значений сигналов", n + " объектов, " + beg + " - " + en);
                    
                var success = new bool[parts.Count];
                int errCount = 0, consErrCount = 0;
                double d = 70.0 / parts.Count;
                for (int i = 0; i < parts.Count; i++)
                {
                    string text = "Чтение блока значений, " + parts[i][0].Context + (parts[i].Count == 0 ? "" : " и др.");
                    using (StartIndicatorText(Procent, Procent + d, text))
                        using (StartKeep(0, 100))
                        {
                            var vc = new ValuesCount();
                            try
                            {
                                vc += readPartFun(parts[i], beg, en, isCut);
                            }
                            catch (Exception ex)
                            {
                                vc.AddStatus(VcStatus.NoSuccess);
                                AddWarning("Ошибка при чтении блока значений", ex);
                            }
                            success[i] = vc.Status == VcStatus.Success || vc.Status == VcStatus.Undefined;
                            valuesCount += vc;
                            if (vc.Status != VcStatus.Fail && vc.Status != VcStatus.NoSuccess)
                            {
                                AddEvent("Значения блока объектов прочитаны", vc.ToString());
                                consErrCount = 0;
                            }
                            else
                            {
                                errCount++;
                                consErrCount++;
                                AddWarning("Значения блока объектов не были прочитаны", null, vc.ToString());
                                if (consErrCount == 1 && !Reconnect())
                                    return valuesCount.AddStatus(VcStatus.Fail);
                                if (consErrCount > 2)
                                {
                                    AddError("Значения с источника не были прочитаны", null, valuesCount.ToString());
                                    return valuesCount.AddStatus(VcStatus.Fail);
                                }
                            }
                        }
                }
                if (errCount == 0)
                {
                    AddEvent("Значения с источника прочитаны", valuesCount.ToString());
                    return valuesCount;
                }

                AddEvent("Рекурсивное чтение значений по блокам объектов");
                valuesCount.Status = VcStatus.Undefined;
                d = 30.0 / parts.Count;
                for (int i = 0; i < parts.Count; i++)
                    using (Start(Procent, Procent + d))
                        if (!success[i])
                            valuesCount += ReadPartRecursive(parts[i], beg, en, isCut, readPartFun);
            }
            catch (Exception ex)
            {
                AddError("Ошибка при получении данных", ex);
                return valuesCount.AddStatus(VcStatus.Fail);
            }
            return valuesCount;
        }
        //Чтение значений по блокам объектов без указания времени и признака среза
        protected ValuesCount ReadByParts(IEnumerable<ListSourceOut> objects, int partSize,
                                          Func<List<ListSourceOut>, DateTime, DateTime, bool, ValuesCount> readPartFun, string msg = null)
        {
            return ReadByParts(objects, partSize, PeriodBegin, PeriodEnd, false, readPartFun, msg);
        }

        //Разбиение списка объектов на блоки
        private List<List<ListSourceOut>> MakeParts(IEnumerable<ListSourceOut> outs, //Список выходов
                                                                          int partSize, //Размер одного блока
                                                                          bool isCut, //Выполняется считываение среза
                                                                          out int n) //Общее количество считываемых объектов  
        {
            AddEvent("Разбиение списка объектов на блоки");
            n = 0;
            var parts = new List<List<ListSourceOut>>();
            int i = 0;
            List<ListSourceOut> part = null;
            foreach (var ob in outs)
                if (!isCut || !ob.HasBegin)
                {
                    n++;
                    if (i++ % partSize == 0)
                        parts.Add(part = new List<ListSourceOut>());
                    part.Add(ob);
                }
            return parts;
        }

        //Рекурсивное чтение значений по блоку
        private ValuesCount ReadPartRecursive(List<ListSourceOut> part, //Блок сигналов 
                                                                   DateTime beg, DateTime en, //Период считывания
                                                                   bool isCut, //Считывается срез 
                                                                   Func<List<ListSourceOut>, DateTime, DateTime, bool, ValuesCount> readPartFun) //Функция чтения значений по одному блоку 
        {
            int n = 0;
            int errCount = 0;
            var valuesCount = new ValuesCount();
            var queue = new Queue<List<ListSourceOut>>();
            queue.Enqueue(part);
            while (queue.Count > 0)
            {
                string text = "Рекурсивное чтение блока значений, " + part[0].Context + (part.Count == 0 ? "" : " и др.");
                using (StartIndicatorText(n < 10 ? 10 * n : 90, n < 9 ? 10 * (n + 1) : 90, text))
                    using (StartKeep(0, 100))
                    {
                        var p = queue.Dequeue();
                        n++;
                        Procent = n < 10 ? 10*n : 90;
                        if (n == 8 && errCount == 7)
                        {
                            AddWarning("Значения по блоку объектов не прочитаны");
                            return valuesCount;
                        }
                        AddEvent("Чтение " + (isCut ? "среза" : "изменений") + " значений сигналов", p.Count + " выходов");
                        var vc = new ValuesCount();
                        try
                        {
                            vc = readPartFun(p, beg, en, isCut);
                        }
                        catch (Exception ex)
                        {
                            vc.AddStatus(VcStatus.NoSuccess);
                            AddWarning("Ошибка при чтении блока значений", ex);
                        }

                        valuesCount += vc;
                        if (!vc.IsFail)
                            AddEvent("Значения прочитаны", vc.ToString());
                        else
                        {
                            AddWarning("Значения не прочитаны");
                            errCount++;
                            if (p.Count > 1)
                            {
                                if (errCount == 1 && !Reconnect())
                                    return valuesCount.AddStatus(VcStatus.Fail);
                                int m = p.Count/2;
                                queue.Enqueue(p.GetRange(0, m));
                                queue.Enqueue(p.GetRange(m, part.Count - m));
                            }
                            else AddErrorOut(p[0].Context, KeepedError);
                        }
                    }
            }
            return valuesCount;
        }

         //Чтение значений по одному блоку списка объектов
        protected abstract ValuesCount ReadPart(IList<ListSourceOut> part, //Блок объектов
                                                                      DateTime beg, DateTime en, //Период считывания
                                                                      bool isCut); //Считывается срез
    }
}