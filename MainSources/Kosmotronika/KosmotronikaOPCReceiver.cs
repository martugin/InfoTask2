using System.ComponentModel.Composition;
using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    [Export(typeof(ProviderBase))]
    [ExportMetadata("Code", "KosmotronikaOpcReceiver")]
    public class KosmotronikaOpcReceiver : OpcServer 
    {
        //Код
        public override string Code { get { return "KosmotronikaOPCReceiver"; } }
        //Комплект
        public override string Complect { get { return "Kosmotronika"; } }

        //Создание подключения
        protected override ProviderConnect CreateConnect()
        {
            return new KosmotronikaOpcConnect();
        }
        //Ссылка на соединение
        public KosmotronikaOpcConnect Connect { get { return (KosmotronikaOpcConnect)CurConnect; } }

        //Получение Tag точки по сигналу
        protected override string GetOpcItemTag(DicS<string> inf)
        {
            return Connect.ServerGroup + ".point." + inf["SysNum"];
        }
    }

    //------------------------------------------------------------------------------------------------

    //Соединение приемника 
    public class KosmotronikaOpcConnect : OpcServerConnect
    {
        //Серверная группа
        internal string ServerGroup { get; private set; }

        //Загрузка дополнительных настроек провайдера из Inf
        protected override void ReadInf(DicS<string> dic)
        {
            ServerName = dic["OPCServerName"];
            Node = dic["Node"];
            ServerGroup = dic["ServerGroup"];
        }
    }
}
