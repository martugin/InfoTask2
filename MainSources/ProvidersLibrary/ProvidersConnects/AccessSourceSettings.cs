using BaseLibrary;

namespace ProvidersLibrary
{
    //Источник, использующий в качестве исходнных данных базу Access
    public class AccessSourceSettings :SourceSettings
    {
        //Файл со значениями
        public string DbFile { get; private set; }
        //Чтение настроек
        protected override void ReadInf(DicS<string> dic)
        {
            DbFile = dic.Get("DbFile");
        }
        public override string Hash { get { return "DbFile=" + DbFile; } }
    }
}