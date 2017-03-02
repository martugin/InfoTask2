using System.ComponentModel.Composition;
using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    [Export(typeof(BaseProvider))]
    [ExportMetadata("Code", "KosmotronikaArchDbSource")]
    public class KosmotronikaArchDbSource : KosmotronikaBaseSource
    {
        //Код провайдера
        public override string Code { get { return "KosmotronikaArchDbSource"; } }
        //Настройки провайдера
        protected override void ReadInf(DicS<string> dic)
        {
            _dataSource = dic["ArchiveDir"] ?? "";
            _location = dic.GetInt("Location");
        }

        protected override string Hash { get { return "ArchDbArchive=" + _dataSource; } }

        //Имя ретро-сервера или путь к архиву
        private string _dataSource;
        //Временной сдвиг
        private int _location;

        //Строка соединения с провайдером
        protected override string ConnectionString
        {
            get { return "Provider=ArchDB.OpenSQL;Data Source=" + _dataSource + ";Location=" + _location; }
        }

        //Проверка настроек
        protected override string CheckSettings(DicS<string> inf)
        {
            return !inf["ArchiveDir"].IsEmpty() ? "" : "Не задан путь к каталогу архива";
        }

        protected override int PartSize { get { return 25; }}
    }
}