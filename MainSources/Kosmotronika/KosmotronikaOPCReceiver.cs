using System.ComponentModel.Composition;
using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    [Export(typeof(Prov))]
    [ExportMetadata("Code", "KosmotronikaOpcReceiver")]
    public class KosmotronikaOpcReceiver : OpcServer 
    {
        //Код
        public override string Code { get { return "KosmotronikaOPCReceiver"; } }
        
        //Серверная группа
        private string _serverGroup;
        //Загрузка дополнительных настроек провайдера из Inf
        protected override void ReadInf(DicS<string> dic)
        {
            base.ReadInf(dic);
            _serverGroup = dic["ServerGroup"];
        }
        
        //Получение Tag точки по сигналу
        protected override string GetOpcItemTag(DicS<string> inf)
        {
            return _serverGroup + ".point." + inf["SysNum"];
        }
    }

    //------------------------------------------------------------------------------------------------

    [Export(typeof(Prov))]
    [ExportMetadata("Complect", "KosmotronikaOpcConn")]
    //Соединение приемника овации
    public class KosmotronikaOpcConn : OpcServerConn
    {
        //Комплект
        public override string Complect { get { return "Kosmotronika"; } }
    }
}
