using System.ComponentModel.Composition;
using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    [Export(typeof(ProviderBase))]
    [ExportMetadata("Code", "OvationOpcReceiver")]
    public class OvationOpcReceiver : OpcServer
    {
        //Комплект
        public override string Complect { get { return "Ovation"; }}
        //Код
        public override string Code { get { return "OvationOPCReceiver"; }}

        //Получение Tag точки по сигналу
        protected override string GetOpcItemTag(DicS<string> inf)
        {
            return inf["CodeObject"];
        }
    }
}
