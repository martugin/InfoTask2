using System;
using System.Collections.Generic;
using System.Linq;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Источник с чтение данных из рекордсета
    public abstract class AdoSource : PartsSource
    {
        //Чтение значений по блокам объектов
        protected ValuesCount ReadByParts(IEnumerable<ListSourceOut> objects, //список объектов
                                            int partSize, //Размер одного блока
                                            DateTime beg, DateTime en, //Период считывания
                                            bool isCut, //Считывается срез
                                            Func<IList<ListSourceOut>, DateTime, DateTime, bool, IRecordRead> queryValuesFun, //Функция - запрос рекордсета по одному блоку, возвращает запрошенный рекорсет
                                            Func<IRecordRead, ListSourceOut> defineObjectFun, //Функция - определение текущего считываемого объекта
                                            string msg = null) //Сообщение для истории о запуске чтения данных
        {
            _queryValuesFun = queryValuesFun;
            _defineObjectFun = defineObjectFun;
            return ReadByParts(objects, partSize, beg, en, isCut, ReadPart, msg);
        }

        //Чтение значений по блокам объектов c использованием стандартных функций
        protected ValuesCount ReadByParts(IEnumerable<ListSourceOut> objects, int partSize,
                                                              DateTime beg, DateTime en, bool isCut, string msg = null)
        {
            return ReadByParts(objects, partSize, beg, en, isCut, QueryValues, DefineOut, msg);
        }

        //Чтение всех значений одним блоком
        protected ValuesCount ReadWhole(IEnumerable<ListSourceOut> part,
                                          DateTime beg, DateTime en, //Период считывания
                                          bool isCut, //Считывается срез
                                          Func<IList<ListSourceOut>, DateTime, DateTime, bool, IRecordRead> queryValuesFun, //Функция - запрос рекордсета по одному блоку, возвращает запрошенный рекорсет
                                          Func<IRecordRead, ListSourceOut> defineObjectFun) //Функция - определение текущего считываемого объекта
        {
            try
            {
                _queryValuesFun = queryValuesFun;
                _defineObjectFun = defineObjectFun;
                var list = part is IList<ListSourceOut> ? (IList<ListSourceOut>)part : part.ToList();
                return ReadPart(list, beg, en, isCut);
            }
            catch (Exception ex)
            {
                AddError("Ошибка при получении данных", ex);
                return new ValuesCount();
            }
        }
        protected ValuesCount ReadWhole(IEnumerable<ListSourceOut> part,
                                                           Func<IList<ListSourceOut>, DateTime, DateTime, bool, IRecordRead> queryValuesFun,
                                                           Func<IRecordRead, ListSourceOut> defineObjectFun)
        {
            return ReadWhole(part, PeriodBegin, PeriodEnd, false, queryValuesFun, defineObjectFun);
        }

        //Чтение всех значений одним блоком c использованием стандартных функций
        protected ValuesCount ReadWhole(IEnumerable<ListSourceOut> part, DateTime beg, DateTime en, bool isCut)
        {
            return ReadWhole(part, beg, en, isCut, QueryValues, DefineOut);
        }

        //Чтение значений по одному явно указанному объекту
        protected ValuesCount ReadOneOut(ListSourceOut ob, DateTime beg, DateTime en, bool isCut,
                                          Func<IList<ListSourceOut>, DateTime, DateTime, bool, IRecordRead> queryValuesFun, //Функция - запрос рекордсета 
                                          string msg = null) //Сообщение для истории о запуске чтения данных
        {
            if (ob == null) return new ValuesCount();
            AddEvent(msg ?? "Чтение значений объекта " + ob.Context);
            return ReadWhole(new[] {ob}, beg, en, isCut, queryValuesFun, rec => ob);
        }
        
        //Ссылки на используемые функции
        private Func<IList<ListSourceOut>, DateTime, DateTime, bool, IRecordRead> _queryValuesFun;
        private Func<IRecordRead, ListSourceOut> _defineObjectFun; 
        
        //Чтение значений по одному блоку списка объектов
        protected override ValuesCount ReadPart(IList<ListSourceOut> part,
                                                                      DateTime beg, DateTime en, //Период считывания
                                                                      bool isCut) //Считывается срез
        {
            IRecordRead rec = null;
            using (Start(0, 50))
            {
                try
                {
                    AddEvent("Чтение значений блока объектов", part.Count + " объектов");
                    rec = _queryValuesFun(part, beg, en, isCut);
                    if (rec == null) return new ValuesCount(VcStatus.NoSuccess);
                }
                catch (Exception ex)
                {
                    AddError("Ошибка при запросе данных из источника", ex);
                    if (rec!= null) rec.Dispose();
                    return new ValuesCount(VcStatus.NoSuccess);
                }
            }
            using (Start(50, 100))
            {
                try
                {
                    using (rec)
                    {
                        AddEvent("Распределение данных по сигналам", part.Count + " объектов");
                        var vc = ReadPartValues(rec);
                        AddEvent("Значения блока объектов прочитаны", vc.ToString());
                        return vc;
                    }
                }
                catch (Exception ex)
                {
                    AddError("Ошибка при формировании значений", ex);
                    return new ValuesCount(VcStatus.NoSuccess);
                }
            }
        }

        //Чтение значений из рекордсета по одному блоку
        private ValuesCount ReadPartValues(IRecordRead rec)
        {
            var vc = new ValuesCount();
            while (rec.Read())
            {
                vc.ReadCount++;
                ListSourceOut ob = null;
                try
                {
                    ob = _defineObjectFun(rec);
                    if (ob != null)
                        vc.WriteCount += ob.ReadMoments(rec);
                    vc.AddStatus(VcStatus.Success);
                }
                catch (Exception ex)
                {
                    AddErrorOut(ob == null ? "" : ob.Context, "Ошибка при чтении значений из рекордсета", ex);
                    vc.AddStatus(VcStatus.NoSuccess);
                }
            }
            return vc;
        }
        
        //Запрос рекордсета по одному блоку, возвращает запрошенный рекорсет, или null при неудаче
        protected abstract IRecordRead QueryValues(IList<ListSourceOut> part, //список объектов
                                                                             DateTime beg, DateTime en, //период считывания
                                                                             bool isCut); //считывается срез
        
        //Определение текущего считываемого объекта
        protected abstract ListSourceOut DefineOut(IRecordRead rec);
    }
}