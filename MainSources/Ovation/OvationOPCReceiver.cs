using System.ComponentModel.Composition;
using BaseLibrary;
using ProvidersLibrary;

namespace Ovation
{
    [Export(typeof(Provider))]
    [ExportMetadata("Code", "OvationOpcReceiver")]
    public class OvationOpcReceiver : OpcDaReceiver
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
