using System.ComponentModel.Composition;
using BaseLibrary;
using ProvidersLibrary;

namespace Kosmotronika
{
    [Export(typeof(ProvidersLibrary.Provider))]
    [ExportMetadata("Code", "KosmotronikaOpcReceiver")]
    public class KosmotronikaOpcReceiver : OpcDaReceiver 
    {
        //Код
        public override string Code { get { return "KosmotronikaOPCReceiver"; } }

        //Загрузка дополнительных настроек провайдера из Inf
        protected override void ReadAdditionalInf(DicS<string> dic)
        {
            ServerGroup = dic["ServerGroup"];
        }

        //Серверная группа
        internal string ServerGroup { get; private set; }

        //Получение Tag точки по сигналу
        protected override string GetOpcItemTag(DicS<string> inf)
        {
            return ServerGroup + ".point." + inf["SysNum"];
        }
    }
}
