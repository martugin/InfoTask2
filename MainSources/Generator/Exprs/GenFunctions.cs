using BaseLibrary;
using CommonTypes;

namespace Generator
{
    internal class GenFunctions : Functions, IContextable
    {
        
        
        //Контекст
        public string Context { get { return "Генерация"; } }
        protected override string ErrSourceCode { get { return "Генерация"; }}
        protected override IContextable Contextable { get { return this; } }
    }
}