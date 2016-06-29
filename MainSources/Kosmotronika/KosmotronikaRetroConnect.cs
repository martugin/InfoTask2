using BaseLibrary;

namespace Provider
{
    //Подключение к Ретро-серверу
    public class KosmotronikaRetroConnect : KosmotronikaBaseConnect
    {
        //Настройки провайдера
        protected override void ReadInf(DicS<string> dic)
        {
            _retroServerName = dic["RetroServerName"];
        }
        public override string Hash { get { return "RetroServer=" + _retroServerName; } }

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