using System;
using System.Collections.Generic;
using System.Text;
using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    //Чтение данных из источника Wonderware
    public class WonderwareReader : AdoSourceReader
    {
        public WonderwareReader(SourceBase source) 
            : base(source) { }
    }
}