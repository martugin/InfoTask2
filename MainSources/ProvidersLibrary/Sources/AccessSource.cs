﻿using System.Threading;
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
        protected override string Hash { get { return "DbFile=" + DbFile; } }

        //Проверка соединения с файлом
        protected override bool ConnectProvider()
        {
            using (var db = new DaoDb(DbFile))
                db.ConnectDao();
            Thread.Sleep(50);
            return true;
        }
    }
}