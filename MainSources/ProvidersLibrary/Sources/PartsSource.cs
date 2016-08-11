using System;
using System.Collections.Generic;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Источник c функцией чтения по блокам
    public abstract class PartsSource : SourceBase
    {
        //Чтение значений по блокам объектов
        protected ValuesCount ReadByParts(IEnumerable<SourceObject> objects, //список объектов
                                                           int partSize, //Размер одного блока
                                                           DateTime beg, DateTime en, //Период считывания
                                                           bool isCut,  //Считывается срез
                                                           Func<List<SourceObject>, DateTime, DateTime, bool, ValuesCount> readPartFun, //Функция чтения значений по одному блоку 
                                                           string msg = null) //Сообщение для истории о запуске чтения данных
        {
            using (Start())
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
                    if (!Connect()) return valuesCount.MakeBad();
                    AddEvent(msg ?? ("Чтение " + (isCut ? "среза" : "изменений") + " значений сигналов"), n + " объектов, " + beg + " - " + en);
                    
                    var success = new bool[parts.Count];
                    int errCount = 0, consErrCount = 0;
                    double d = 70.0 / parts.Count;
                    for (int i = 0; i < parts.Count; i++)
                        using (Start(Procent, Procent + d, CommandFlags.Danger))
                        {
                            var vc = readPartFun(parts[i], beg, en, isCut);
                            success[i] = vc.Status == ValuesCountStatus.Success || vc.Status == ValuesCountStatus.Undefined;
                            valuesCount += vc;
                            if (!vc.IsBad)
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
                                    return valuesCount.MakeBad();
                                if (consErrCount > 2)
                                {
                                    AddError("Значения с источника не были прочитаны", null, valuesCount.ToString());
                                    valuesCount.Status = ValuesCountStatus.Fail;
                                    return valuesCount;
                                }
                            }
                        }
                    if (errCount == 0)
                    {
                        AddEvent("Значения с источника прочитаны", valuesCount.ToString());
                        return valuesCount;
                    }

                    AddEvent("Рекурсивное чтение значений по блокам объектов");
                    d = 30.0 / parts.Count;
                    for (int i = 0; i < parts.Count; i++)
                        using (Start(Procent, Procent + d, CommandFlags.Danger))
                            if (!success[i])
                                valuesCount += ReadPartRecursive(parts[i], beg, en, isCut, readPartFun);
                }
                catch (Exception ex)
                {
                    AddError("Ошибка при получении данных", ex);
                    return valuesCount.MakeBad();
                }
                return valuesCount;
            }
        }
        //Чтение значений по блокам объектов без указания времени и признака среза
        protected ValuesCount ReadByParts(IEnumerable<SourceObject> objects, int partSize,
                                          Func<List<SourceObject>, DateTime, DateTime, bool, ValuesCount> readPartFun, string msg = null)
        {
            return ReadByParts(objects, partSize, PeriodBegin, PeriodEnd, false, readPartFun, msg);
        }

        //Разбиение списка объектов на блоки
        private List<List<SourceObject>> MakeParts(IEnumerable<SourceObject> objects, //Список объектов
                                                                          int partSize, //Размер одного блока
                                                                          bool isCut, //Выполняется считываение среза
                                                                          out int n) //Общее количество считываемых объектов  
        {
            n = 0;
            var parts = new List<List<SourceObject>>();
            int i = 0;
            List<SourceObject> part = null;
            foreach (var ob in objects)
                if (!isCut || !ob.HasBegin)
                {
                    n++;
                    if (i++ % partSize == 0)
                        parts.Add(part = new List<SourceObject>());
                    part.Add(ob);
                }
            return parts;
        }

        //Рекурсивное чтение значений по блоку
        private ValuesCount ReadPartRecursive(List<SourceObject> part, //Блок сигналов 
                                                                   DateTime beg, DateTime en, //Период считывания
                                                                   bool isCut, //Считывается срез 
                                                                   Func<List<SourceObject>, DateTime, DateTime, bool, ValuesCount> readPartFun) //Функция чтения значений по одному блоку 
        {
            int n = 0;
            int errCount = 0;
            var valuesCount = new ValuesCount();
            var queue = new Queue<List<SourceObject>>();
            queue.Enqueue(part);
            while (queue.Count > 0)
            {
                var p = queue.Dequeue();
                n++;
                Procent = n < 10 ? 10*n : 90;
                if (n == 8 && errCount == 7)
                {
                    AddWarning("Значения по блоку объектов не прочитаны");
                    return valuesCount;
                }
                AddEvent("Чтение " + (isCut ? "среза" : "изменений") + " значений сигналов", p.Count + " объектов");
                var vc = readPartFun(p, beg, en, isCut);
                valuesCount += vc;
                if (!vc.IsBad)
                    AddEvent("Значения прочитаны", vc.ToString());
                else
                {
                    AddWarning("Значения не прочитаны");
                    errCount++;
                    if (p.Count > 1)
                    {
                        if (errCount == 1 && !Reconnect())
                            return valuesCount.MakeBad();
                        int m = p.Count/2;
                        queue.Enqueue(p.GetRange(0, m));
                        queue.Enqueue(p.GetRange(m, part.Count - m));
                    }
                    else SourceConnect.AddErrorObject(p[0].Context, Command.ErrorMessage(false, true, false));
                }
            }
            return valuesCount;
        }

         //Чтение значений по одному блоку списка объектов
        protected abstract ValuesCount ReadPart(IList<SourceObject> part, //Блок объектов
                                                                      DateTime beg, DateTime en, //Период считывания
                                                                      bool isCut); //Считывается срез
    }
}