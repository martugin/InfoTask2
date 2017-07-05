using BaseLibrary;
using CommonTypes;

namespace CompileLibrary
{
    //Прокси для схемы проекта
    public class SchemeProxy
    {
        public SchemeProxy(ProviderType type, string code)
        {
            Code = code;
            Type = type;
        }

        //Код
        public string Code { get; private set; }
        //Тип (Proxy или QueuedProxy)
        public ProviderType Type { get; private set; }

        //Коды связанных входящих и исходящих соединений
        private readonly SetS _inConnects = new SetS();
        public SetS InConnects { get { return _inConnects; } }
        private readonly SetS _outConnects = new SetS();
        public SetS OutConnects { get { return _outConnects; } }
    }
}