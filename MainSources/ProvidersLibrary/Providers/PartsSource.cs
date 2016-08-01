using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Источник c функцией чтения по блокам
    public abstract class PartsSource : SourceBase
    {
        //Чтение значений по блокам объектов
        protected ValuesCount ReadValuesByParts(IEnumerable<SourceObject> objects, //список объектов
                                                           int partSize, //Размер одного блока
                                                           DateTime beg, //Период считывания
                                                           DateTime en,
                                                           bool isCut,  //Считывается срез
                                                           Func<List<SourceObject>, DateTime, DateTime, bool, ValuesCount> readPartFun, //Функция чтения значений по одному блоку 
                                                           string msg = null, //Сообщение для истории о запуске чтения данных
                                                           int maxConsequentErrors = 3,  //Количество ошибочных блоков подряд, которое нужно считать, чтобы понять, что связи нет
                                                           int maxRecursionDepth = 2) //Максимальная глубина рекурсивного чтения блока, при условии что все части блока считались неудачно 
        {
            var valuesCount = new ValuesCount();
            try
            {
                int n = ObjectsToReadCount(objects, isCut);
                if (n == 0)
                {
                    AddEvent("Пустой список объектов для считывания" + (isCut ? " среза" : ""), beg + " - " + en);
                    return valuesCount;
                }
                if (!Check()) return new ValuesCount(false);
                
                AddEvent(msg ?? ("Чтение " + (isCut ? "среза" : "изменений") + " значений сигналов"), n + " объектов, " + beg + " - " + en);
                var parts = MakeParts(objects, partSize, isCut);
                var success = new bool[parts.Count];
                int errCount = 0;
                bool hasErrors = false;
                double d = 70.0 / parts.Count;
                for (int i = 0; i < parts.Count; i++)
                    using (Start(Procent, Procent + d, CommandFlags.Danger))
                    {
                        var vc = readPartFun(parts[i], beg, en, isCut);
                        success[i] = vc.IsSuccess;
                        if (vc.IsSuccess)
                        {
                            valuesCount += vc;
                            errCount = 0;
                            AddEvent("Значения блока объектов прочитаны", vc.ToString());
                        }
                        else
                        {
                            errCount++;
                            hasErrors = true;
                            AddWarning("Значения блока объектов не были прочитаны");
                            if (!Check())
                            {
                                valuesCount.IsSuccess = false;
                                return valuesCount;
                            }
                        }
                        if (errCount > maxConsequentErrors)
                        {
                            valuesCount.IsSuccess = false;
                            AddError("Значения с источника не были прочитаны", null, valuesCount.ToString());
                            return valuesCount;
                        }
                    }
                if (!hasErrors)
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
                valuesCount.IsSuccess = false;
            }
            return valuesCount;
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

        //Рекурсивное чтение значений по блоку
        private ValuesCount ReadPartRecursive(List<SourceObject> part, //Блок сигналов 
                                              DateTime beg, DateTime en, //Период считывания
                                              bool isCut, //Считывается срез 
                                              Func<List<SourceObject>, DateTime, DateTime, bool, ValuesCount> readPartFun) //Функция чтения значений по одному блоку 
        {
            int n = 0;
            bool hasSuccess = false;
            var valuesCount = new ValuesCount();
            var queue = new Queue<List<SourceObject>>();
            queue.Enqueue(part);
            while (queue.Count > 0)
            {
                var p = queue.Dequeue();
                n++;
                Procent = n < 10 ? 10*n : 90;
                if (n > 7 && !hasSuccess)
                {
                    AddError("Значения по блоку не прочитаны");
                    return new ValuesCount(false);
                }
                AddEvent("Чтение " + (isCut ? "среза" : "изменений") + " значений сигналов", p.Count + " объектов");
                var vc = readPartFun(p, beg, en, isCut);
                if (vc.IsSuccess)
                {
                    AddEvent("Значения успешно прочитаны", vc.ToString());
                    valuesCount += vc;
                    hasSuccess = true;
                }
                else
                {
                    AddWarning("Значения не прочитаны");
                    if (p.Count > 1)
                    {
                        if (!Check())
                        {
                            valuesCount.IsSuccess = false;
                            return valuesCount;
                        }
                        int m = p.Count/2;
                        queue.Enqueue(p.GetRange(0, m));
                        queue.Enqueue(p.GetRange(m, part.Count - m));
                    }
                    else SourceConnect.AddErrorObject(p[0].Context, Command.ErrorMessage(false, true, false));
                    
                }
            }
            return valuesCount;
        }
    }
}