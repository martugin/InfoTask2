using System.ComponentModel.Composition;
using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    [Export(typeof(Prov))]
    [ExportMetadata("Code", "KosmotronikaRetroSource")]
    public class KosmotronikaRetroSource : KosmotronikaBaseSource
    {
        //Код провайдера
        public override string Code { get { return "KosmotronikaRetroSource"; } }
        
        //Настройки провайдера
        protected override void ReadInf(DicS<string> dic)
        {
            _retroServerName = dic["RetroServerName"];
        }
        public override string Hash { get { return "RetroServer=" + _retroServerName; }}

        //Имя ретросервера
        private string _retroServerName;

        //Строка соединения с провайдером
        protected override string ConnectionString
        {
            get { return "Provider=RetroDB.RetroSQL;Data Source=" + _retroServerName; }
        }

        //Проверка настроек
        public override string CheckSettings(DicS<string> inf)
        {
            return !inf["RetroServerName"].IsEmpty() ? "" : "Не задано имя Ретро-сервера";
        }
    }
}