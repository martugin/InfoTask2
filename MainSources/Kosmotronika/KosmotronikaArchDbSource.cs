using System.ComponentModel.Composition;
using ProvidersLibrary;

namespace Provider
{
    [Export(typeof(ProviderBase))]
    [ExportMetadata("Code", "KosmotronikaArchDbSource")]
    public class KosmotronikaArchDbSource : KosmotronikaBaseSource
    {
        //Код провайдера
        public override string Code { get { return "KosmotronikaArchDbSource"; } }

        //Создание подключения
        protected override ProviderSettings CreateConnect()
        {
            return new KosmotronikaArchDbSettings();
        }
    }
}