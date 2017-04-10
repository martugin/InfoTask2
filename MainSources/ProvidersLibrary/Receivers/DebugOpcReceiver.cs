using BaseLibrary;

namespace ProvidersLibrary
{
    //Отладочный OPC-сервер
    public class DebugOpcReceiver : OpcReceiver
    {
        public DebugOpcReceiver() { }

        public DebugOpcReceiver(string serverName, string node) 
        {
            ServerName = serverName;
            Node = node;
        }

        public override string Code { get { return "DebugOpcReceiver"; } }

        protected override string GetOpcItemTag(DicS<string> inf) { return ""; }
    }
}