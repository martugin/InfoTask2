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
        protected override ProviderSettings CreateConnect()
        {
            return new WoderwareOpcSettings();
        }
        //Ссылка на соединение
        public WoderwareOpcSettings Settings { get { return (WoderwareOpcSettings)CurSettings; } }

        //Получение Tag точки по сигналу
        protected override string GetOpcItemTag(DicS<string> inf)
        {
            return Settings.ServerNode + "." + Settings.ServerGroup + "." + inf["TagName"];
        }
    }

    //----------------------------------------------------------------------------------------------
    public class WoderwareOpcSettings : OpcServerSettings
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