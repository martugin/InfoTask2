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
        protected ValuesCount ReadByParts(IEnumerable<SourceObject> objects, //список объектов
                                            int partSize, //Размер одного блока
                                            DateTime beg, DateTime en, //Период считывания
                                            bool isCut, //Считывается срез
                                            Func<IList<SourceObject>, DateTime, DateTime, bool, IRecordRead> queryValuesFun, //Функция - запрос рекордсета по одному блоку, возвращает запрошенный рекорсет
                                            Func<IRecordRead, SourceObject> defineObjectFun, //Функция - определение текущего считываемого объекта
                                            string msg = null) //Сообщение для истории о запуске чтения данных
        {
            _queryValuesFun = queryValuesFun;
            _defineObjectFun = defineObjectFun;
            return ReadByParts(objects, partSize, beg, en, isCut, ReadPart, msg);
        }

        //Чтение значений по блокам объектов c использованием стандартных функций
        protected ValuesCount ReadByParts(IEnumerable<SourceObject> objects, int partSize,
                                                                       DateTime beg, DateTime en, bool isCut, string msg = null)
        {
            return ReadByParts(objects, partSize, beg, en, isCut, QueryValues, DefineObject, msg);
        }

        //Чтение всех значений одним блоком
        protected ValuesCount ReadWhole(IEnumerable<SourceObject> part,
                                          DateTime beg, DateTime en, //Период считывания
                                          bool isCut, //Считывается срез
                                          Func<List<SourceObject>, DateTime, DateTime, bool, IRecordRead> queryValuesFun, //Функция - запрос рекордсета по одному блоку, возвращает запрошенный рекорсет
                                          Func<IRecordRead, SourceObject> defineObjectFun) //Функция - определение текущего считываемого объекта
        {
            try
            {
                _queryValuesFun = QueryValues;
                _defineObjectFun = DefineObject;
                var list = part is IList<SourceObject> ? ((IList<SourceObject>)part) : part.ToList();
                return ReadPart(list, beg, en, isCut);
            }
            catch (Exception ex)
            {
                AddError("Ошибка при получении данных", ex);
                return new ValuesCount();
            }
        }

        //Чтение всех значений одним блоком
        protected ValuesCount ReadWhole(IEnumerable<SourceObject> part, DateTime beg, DateTime en, bool isCut)
        {
            return ReadWhole(part, beg, en, isCut, QueryValues, DefineObject);
        }

        //Ссылки на используемые функции
        private Func<IList<SourceObject>, DateTime, DateTime, bool, IRecordRead> _queryValuesFun;
        private Func<IRecordRead, SourceObject> _defineObjectFun; 
        
        //Чтение значений по одному блоку списка объектов
        protected override ValuesCount ReadPart(IList<SourceObject> part,
                                                                      DateTime beg, DateTime en, //Период считывания
                                                                      bool isCut) //Считывается срез
        {
            IRecordRead rec;
            using (Start(0, 50))
            {
                try
                {
                    AddEvent("Чтение значений блока объектов", part.Count + " объектов");
                    rec = _queryValuesFun(part, beg, en, isCut);
                    if (rec == null) return new ValuesCount().MakeBad();
                }
                catch (Exception ex)
                {
                    AddError("Ошибка при запросе данных из источника", ex);
                    return new ValuesCount().MakeBad();
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
                    return new ValuesCount().MakeBad();
                }
            }
        }

        //Чтение значений из рекордсета по одному блоку
        private ValuesCount ReadPartValues(IRecordRead rec)
        {
            var vc = new ValuesCount(ValuesCountStatus.Success);
            while (rec.Read())
            {
                vc.ReadCount++;
                SourceObject ob = null;
                try
                {
                    ob = _defineObjectFun(rec);
                    if (ob != null)
                        vc.WriteCount += ob.ReadMoments(rec);
                }
                catch (Exception ex)
                {
                    SourceConnect.AddErrorObject(ob == null ? "" : ob.Context, "Ошибка при чтении значений из рекордсета", ex);
                    vc.Status = ValuesCountStatus.Partial;
                }
            }
            return vc;
        }
        
        //Запрос рекордсета по одному блоку, возвращает запрошенный рекорсет, или null при неудаче
        protected abstract IRecordRead QueryValues(IList<SourceObject> part, //список объектов
                                                            DateTime beg, DateTime en, //период считывания
                                                            bool isCut); //считывается срез
        
        //Определение текущего считываемого объекта
        protected abstract SourceObject DefineObject(IRecordRead rec);
    }
}