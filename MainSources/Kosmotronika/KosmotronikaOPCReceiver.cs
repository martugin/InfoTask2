using System.ComponentModel.Composition;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    [Export(typeof(IProvider))]
    [ExportMetadata("Code", "KosmotronikaOpcReceiver")]
    public class KosmotronikaOpcReceiver : OpcServer 
    {
        //Код
        public override string Code { get { return "KosmotronikaOPCReceiver"; } }
        
        //Серверная группа
        private string _serverGroup;
        //Загрузка дополнительных настроек провайдера из Inf
        protected override void ReadDicS(DicS<string> dic)
        {
            base.ReadDicS(dic);
            _serverGroup = dic["ServerGroup"];
        }
        
        //Получение Tag точки по сигналу
        protected override string GetOpcItemTag(DicS<string> inf)
        {
            return _serverGroup + ".point." + inf["SysNum"];
        }
    }
}
