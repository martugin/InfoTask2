using System;
using BaseLibrary;

namespace ProvidersLibrary
{
    public class MomSourceConnect : SourceConnect
    {
        public MomSourceConnect(string name, string complect, Logger logger) 
            : base(name, complect, logger) { }

        //Текущий провайдер источника
        internal MomSource Source { get { return (MomSource)Provider; } }

        //Чтение значений из источника
        protected override ValuesCount ReadValues()
        {
            throw new NotImplementedException();
        }
    }
}