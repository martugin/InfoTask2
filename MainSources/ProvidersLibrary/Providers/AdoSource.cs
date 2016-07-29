using System;
using System.Collections.Generic;
using BaseLibrary;

namespace ProvidersLibrary
{
    public abstract class AdoSource : SourceBase
    {
        protected AdoSource()
        {
            CreateReaders();
        }

        protected virtual void CreateReaders()
        {
            _defaultReader = new AdoSourceReader(this);
        }

        //Ридер по умолчанию
        private AdoSourceReader _defaultReader;

        //Запрос рекордсета по одному блоку, возвращает запрошенный рекорсет, или null при неудаче
        internal protected virtual IRecordRead QueryValues(List<SourceObject> part, //список объектов
                                                           DateTime beg, DateTime en, //период считывания
                                                           bool isCut) //считывается срез
        {
            return null;
        }

        //Определение текущего считываемого объекта
        internal protected virtual SourceObject DefineObject(IRecordRead rec)
        {
            return null;
        }
    }
}