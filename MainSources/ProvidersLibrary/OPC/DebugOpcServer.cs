using BaseLibrary;

namespace ProvidersLibrary
{
    //Отладочный OPC-сервер
    public class DebugOpcServer : OpcServer
    {
        public DebugOpcServer() { }

        public DebugOpcServer(string serverName, string node)
        {
            var conn = (OpcServerConnect)CreateConnect();
            conn.ServerName = serverName;
            conn.Node = node;
            Logger = new Logger();
        }

        public override string Complect { get { return "Debug"; } }
        public override string Code { get { return "DebugOpcServer"; } }

        protected override string GetOpcItemTag(DicS<string> inf) { return ""; }

        protected override ProviderConnect CreateConnect()
        {
            return new OpcServerConnect();
        }
    }
}