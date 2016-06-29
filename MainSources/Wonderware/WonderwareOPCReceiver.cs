using System.ComponentModel.Composition;
using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    //OPC-сервер Wonderware
    [Export(typeof(ProviderBase))]
    [ExportMetadata("Code", "WonderwareOpcReceiver")]
    public class WonderwareOpcReceiver : OpcServer
    {
        //Комплект
        public override string Complect { get { return "Wonderware"; } }
        //Код
        public override string Code { get { return "WonderwareOpcReceiver"; } }
        //Создание подключения
        protected override ProviderConnect CreateConnect()
        {
            return new WoderwareOpcConnect();
        }
        //Ссылка на соединение
        public WoderwareOpcConnect Connect { get { return (WoderwareOpcConnect)CurConnect; } }

        //Получение Tag точки по сигналу
        protected override string GetOpcItemTag(DicS<string> inf)
        {
            return Connect.ServerNode + "." + Connect.ServerGroup + "." + inf["TagName"];
        }
    }

    //----------------------------------------------------------------------------------------------
    public class WoderwareOpcConnect : OpcServerConnect
    {
        //Загрузка дополнительных настроек провайдера из Inf
        protected override void ReadInf(DicS<string> dic)
        {
            ServerName = dic["OPCServerName"];
            Node = dic["Node"];
            ServerNode = dic["ServerNode"];
            ServerGroup = dic["ServerGroup"];
        }

        //Серверный узел
        internal string ServerNode { get; private set; }
        //Серверная группа
        internal string ServerGroup { get; private set; }
    }
}