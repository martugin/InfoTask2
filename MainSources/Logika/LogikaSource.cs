using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using BaseLibrary;
using ProvidersLibrary;

namespace Logika
{
    [Export(typeof(Provider))]
    [ExportMetadata("Code", "LogikaSource")]
    public class LogikaSource : AccessSource
    {
        //Код провайдера
        public override string Code { get { return "LogikaSource"; } }

        //Проверка соединения с файлом
        protected override void ConnectProvider()
        {
            if (!DaoDb.Check(DbFile, new[] {"NODES", "ABONENTS"}))
                AddError("Недопустимая база данных системы Пролог");
        }

        //Словарь выходов, первый ключ - код таблицы, второй ключ - id объекта
        internal readonly DicS<DicI<LogikaOut>> Outs = new DicS<DicI<LogikaOut>>();
        //Словарь выходов ключ - id объекта
        internal readonly DicI<LogikaOut> OutsId = new DicI<LogikaOut>();

        //Добавить выход в источник
        protected override ProviderOut AddOut(ProviderSignal sig)
        {
            int id = sig.Inf.GetInt("NodeId");
            if (OutsId.ContainsKey(id))
                return OutsId[id];
            string tableName = sig.Inf["TableName"];
            if (!Outs.ContainsKey(tableName))
                Outs.Add(tableName, new DicI<LogikaOut>());
            var ob = new LogikaOut(this);
            Outs[tableName].Add(id, ob);
            return OutsId.Add(id, ob);
        }

        //Очистка списка выходов
        protected override void ClearOuts()
        {
            Outs.Clear();
            OutsId.Clear();
        }

        //Имя текущей считываемой таблицы
        private string _tableName;

        //Запрос значений
        protected override IRecordRead QueryValues(IList<ListSourceOut> part, DateTime beg, DateTime en, bool isCut)
        {
            return new DaoRec(DbFile, "SELECT * FROM " + _tableName + "_ARCHIVE " +
                                      "WHERE (TYPE = 0) AND (Время >= " + beg.ToAccessString() + ") AND (Время <= " + en.ToAccessString() + ")"); 
        }

        //Определение текущего выхода
        protected override ListSourceOut DefineOut(IRecordRead rec)
        {
            return OutsId[rec.GetInt("PARENT_ID")];
        }

        //Чтение значений, срез считывается вместе с изменениями
        protected override ValuesCount ReadChanges(DateTime beg, DateTime en)
        {
            var vc = new ValuesCount();
            DateTime b = beg.AddMinutes(-beg.Minute).AddSeconds(-beg.Second - 1);
            DateTime e = en.AddSeconds(1);
            foreach (var tabl in Outs.Dic)
            {
                _tableName = tabl.Key;
                vc += ReadWhole(tabl.Value.Values, b, e, false);
                if (vc.IsFail) return vc;
            }
            return vc;
        }
    }
}
