using System.ComponentModel.Composition;
using ProvidersLibrary;

namespace Provider
{
    [Export(typeof(ProviderBase))]
    [ExportMetadata("Code", "KosmotronikaRetroSource")]
    public class KosmotronikaRetroSource : KosmotronikaBaseSource
    {
        //Код провайдера
        public override string Code { get { return "KosmotronikaRetroSource"; } }

        //Создание подключения
        protected override ProviderConnect CreateConnect()
        {
            return new KosmotronikaRetroConnect();
        }
    }
}