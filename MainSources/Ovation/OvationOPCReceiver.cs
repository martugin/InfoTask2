using System.ComponentModel.Composition;
using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    [Export(typeof(BaseProvider))]
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
}
