using System.ComponentModel.Composition;
using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    [Export(typeof(Prov))]
    [ExportMetadata("Code", "WonderwareOpcReceiver")]
    public class WonderwareOpcReceiver : OpcServer
    {
        //Код
        public override string Code { get { return "WonderwareOpcReceiver"; } }
        //Серверный узел
        private string _serverNode;
        //Серверная группа
        private string _serverGroup;
        //Загрузка дополнительных настроек провайдера из Inf
        protected override void ReadInf(DicS<string> dic)
        {
            base.ReadInf(dic);
            _serverNode = dic["ServerNode"];
            _serverGroup = dic["ServerGroup"];
        }

        //Получение Tag точки по сигналу
        protected override string GetOpcItemTag(DicS<string> inf)
        {
            return _serverNode + "." + _serverGroup + "." + inf["TagName"];
        }
    }

    //--------------------------------------------------------------------------------------------------------------------

    [Export(typeof(ProvConn))]
    [ExportMetadata("Complect", "WonderwareOpcConn")]
    public class WonderwareOpcConn : ReceivConn
    {
        public override string Complect { get { return "Wonderware"; }}
    }
}