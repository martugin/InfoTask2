using BaseLibrary;

namespace ProvidersLibrary
{
    //Отладочный OPC-сервер
    public class DebugOpcServer : OpcServer
    {
        public DebugOpcServer() { }

        public DebugOpcServer(string serverName, string node)
        {
            ServerName = serverName;
            Node = node;
            Logger = new Logger();
        }

        public override string Code { get { return "DebugOpcServer"; } }

        protected override string GetOpcItemTag(DicS<string> inf) { return ""; }
    }
}