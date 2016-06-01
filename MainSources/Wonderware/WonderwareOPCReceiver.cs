using System.ComponentModel.Composition;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    [Export(typeof(IProvider))]
    [ExportMetadata("Code", "WonderwareOPCReceiver")]
    public class WonderwareOPCReceiver : OpcServer
    {
        //Код
        public override string Code { get { return "WonderwareOPCReceiver"; } }
        //Серверный узел
        private string _serverNode;
        //Серверная группа
        private string _serverGroup;
        //Загрузка дополнительных настроек провайдера из Inf
        protected override void ReadDicS(DicS<string> dic)
        {
            base.ReadDicS(dic);
            _serverNode = dic["ServerNode"];
            _serverGroup = dic["ServerGroup"];
        }

        //Получение Tag точки по сигналу
        protected override string GetOpcItemTag(DicS<string> inf)
        {
            return _serverNode + "." + _serverGroup + "." + inf["TagName"];
        }
    }
}