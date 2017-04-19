using System.ComponentModel.Composition;
using BaseLibrary;
using ProvidersLibrary;

namespace Wonderware
{
    //OPC-сервер Wonderware
    [Export(typeof(Provider))]
    [ExportMetadata("Code", "WonderwareOpcReceiver")]
    public class WonderwareOpcReceiver : OpcDaReceiver
    {
        //Код
        public override string Code { get { return "WonderwareOpcReceiver"; } }

        //Загрузка дополнительных настроек провайдера из Inf
        protected override void ReadAdditionalInf(DicS<string> dic)
        {
            ServerNode = dic["ServerNode"];
            ServerGroup = dic["ServerGroup"];
        }

        //Серверный узел
        internal string ServerNode { get; private set; }
        //Серверная группа
        internal string ServerGroup { get; private set; }

        //Получение Tag точки по сигналу
        protected override string GetOpcItemTag(DicS<string> inf)
        {
            return ServerNode + "." + ServerGroup + "." + inf["TagName"];
        }
    }
}