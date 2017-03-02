using System;
using System.ComponentModel.Composition;
using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    [Export(typeof(BaseProvider))]
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

        protected override string Hash { get { return "RetroServer=" + _retroServerName; } }

        //Имя ретросервера
        private string _retroServerName;

        //Строка соединения с провайдером
        protected override string ConnectionString
        {
            get { return "Provider=RetroDB.RetroSQL;Data Source=" + _retroServerName; }
        }

        //Проверка настроек
        protected override string CheckSettings(DicS<string> inf)
        {
            return !inf["RetroServerName"].IsEmpty() ? "" : "Не задано имя Ретро-сервера";
        }

        protected override int PartSize 
        {
            get
            {
                double len = PeriodEnd.Subtract(PeriodBegin).TotalHours;
                int res = 3000;
                if (len > 0.0001) res = Math.Min(3000, Convert.ToInt32(2500 / len));
                if (res == 0) res = 1;
                return res;    
            }
        }
    }
}