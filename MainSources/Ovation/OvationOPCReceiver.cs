using System.ComponentModel.Composition;
using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    [Export(typeof(Prov))]
    [ExportMetadata("Code", "OvationOpcReceiver")]
    public class OvationOpcReceiver : OpcServer
    {
        //Код
        public override string Code { get { return "OvationOPCReceiver"; }}
        
        //Получение Tag точки по сигналу
        protected override string GetOpcItemTag(DicS<string> inf)
        {
            return inf["CodeObject"];
        }
    }

    //------------------------------------------------------------------------------------------------

    [Export(typeof(Prov))]
    [ExportMetadata("Complect", "OvationOpcConn")]
    //Соединение приемника овации
    public class OvationOpcConn : OpcServerConn
    {
        //Комплект
        public override string Complect { get { return "Ovation"; } }
    }
}
