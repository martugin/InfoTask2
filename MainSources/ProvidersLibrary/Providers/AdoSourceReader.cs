using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Чтение данных из источника Ado (чтение из рекордсетов, можно блоками)
    public class AdoSourceReader : ExternalLogger
    {
        public AdoSourceReader(AdoSource source)
        {
            Source = source;
            Logger = Source.Logger;
        }

        //Ссылка на источник
        protected AdoSource Source { get; private set; }
        //Контекст для записи в логгер
        public override string Context { get { return Source.Context; }}

        //Настройки получения данных
        //Свойства чтения сигналов по блокам
        protected int ReconnectsCount { get; set; } //Сколько раз повторять считывание одного блока
        protected int MaxErrorCount { get; set; } //Количество блоков, которое нужно считать, чтобы понять, что связи нет, 0 - читать до конца
        protected int MaxErrorDepth { get; set; } //Глубина, до которой нужно дробить первый блок, если так и не было успешного считывания
        protected int ErrorWaiting { get; set; } //Время ожидания после ошибки считывания в мс

        //Общее количество прочитанных и сформированных значений
        protected int NumRead { get; set; }
        protected int NumWrite { get; set; }

        private bool _isConnected;

        

        //Чтение значений по одному блоку списка объектов
        protected ValuesCount ReadPart(List<SourceObject> part,
                                             DateTime beg, DateTime en, //Период считывания
                                             bool isCut) //Считывается срез
        {
            IRecordRead rec;
            using (Start(0, 50))
            {
                try
                {
                    AddEvent("Чтение значений блока объектов", part.Count + " объектов");
                    rec = QueryValues(part, beg, en, isCut);
                    if (rec == null) return _isConnected = false;
                }
                catch (Exception ex)
                {
                    AddError("Ошибка при запросе данных из источника", ex);
                    return _isConnected = false;
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
                    Source.AddErrorObject(ob == null ? "" : ob.Context, "Ошибка при чтении значений из рекордсета", ex);
                }
            }
            return new Tuple<int, int>(nread, nwrite);
        }

        //Методы, которые нужно переопределять если истоник использует несколько ридеров

        //Запрос рекордсета по одному блоку, возвращает запрошенный рекорсет, или null при неудаче
        protected virtual IRecordRead QueryValues(List<SourceObject> part, //список объектов
                                                  DateTime beg, DateTime en, //период считывания
                                                  bool isCut) //считывается срез
        {
            return Source.QueryValues(part, beg, en, isCut);
        }

        //Определение текущего считываемого объекта
        protected virtual SourceObject DefineObject(IRecordRead rec)
        {
            return Source.DefineObject(rec);
        }
    }
}