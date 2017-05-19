using System;
using System.Threading;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Приемники, записывающие в Access
    public abstract class AccessReceiver : Receiver
    {
        //Файл со значениями
        protected string DbFile { get; private set; }
        //Чтение настроек
        protected override void ReadInf(DicS<string> dic)
        {
            DbFile = dic.Get("DbFile");
        }
    }

    //---------------------------------------------------------------------------------------------
    //Архивы мгновенных значений
    public abstract class RealTimeAccessReceiver : AccessReceiver
    {
        //База данных архива
        protected DaoDb Db { get; set; }

        //Соединение с файлом
        protected override void ConnectProvider()
        {
            Db = new DaoDb(DbFile);
            Db.ConnectDao();
            Thread.Sleep(50);
        }

        //Закрытие соединения
        protected override void DisconnectProvider()
        {
            Db.Dispose();
            Db = null;
        }
    }
}