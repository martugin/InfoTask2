using System.Threading;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Источник данных, получаемых из базы данных Access
    public abstract class AccessSource : AdoSource
    {
        //Файл со значениями
        protected string DbFile { get; private set; }
        //Чтение настроек
        protected override void ReadInf(DicS<string> dic)
        {
            DbFile = dic.Get("DbFile");
        }

        //Проверка соединения с файлом
        protected override void ConnectProvider()
        {
            using (var db = new DaoDb(DbFile))
                db.ConnectDao();
            Thread.Sleep(50);
        }
    }
}