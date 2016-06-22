using System.ComponentModel.Composition;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Provider
{
    [Export(typeof(IProvider))]
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

        protected override void MakeConnect(string name, Logger logger)
        {
            throw new System.NotImplementedException();
        }

        public override string Complect
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}
