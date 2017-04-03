using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using BaseLibrary;
using ProvidersLibrary;

namespace Logika
{
    [Export(typeof(BaseProvider))]
    [ExportMetadata("Code", "PrologSource")]
    public class PrologSource : AccessSource
    {
        //Код провайдера
        public override string Code { get { return "PrologSource"; } }

        //Проверка соединения с файлом
        protected override void ConnectProvider()
        {
            if (!DaoDb.Check(DbFile, new[] {"NODES", "ABONENTS"}))
                AddError("Недопустимая база данных системы Пролог");
        }

        //Словарь выходов, первый ключ - код таблицы, второй ключ - id объекта
        internal readonly DicS<DicI<PrologOut>> Outs = new DicS<DicI<PrologOut>>();
        //Словарь выходов ключ - id объекта
        internal readonly DicI<PrologOut> OutsId = new DicI<PrologOut>();

        //Добавить выход в источник
        protected override SourceOut AddOut(InitialSignal sig)
        {
            int id = sig.Inf.GetInt("NodeId");
            if (OutsId.ContainsKey(id))
                return OutsId[id];
            string tableName = sig.Inf["TableName"];
            if (!Outs.ContainsKey(tableName))
                Outs.Add(tableName, new DicI<PrologOut>());
            var ob = new PrologOut(this);
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
        protected override IRecordRead QueryValues(IList<SourceOut> part, DateTime beg, DateTime en, bool isCut)
        {
            return new DaoRec(DbFile, "SELECT * FROM " + _tableName + "_ARCHIVE " +
                                      "WHERE (TYPE = 0) AND (Время >= " + beg.ToAccessString() + ") AND (Время <= " + en.ToAccessString() + ")"); 
        }

        //Определение текущего выхода
        protected override SourceOut DefineOut(IRecordRead rec)
        {
            return OutsId[rec.GetInt("PARENT_ID")];
        }

        //Чтение значений, срез считывается вместе с изменениями
        protected override ValuesCount ReadChanges()
        {
            var vc = new ValuesCount();
            DateTime beg = PeriodBegin.AddMinutes(-PeriodBegin.Minute).AddSeconds(-PeriodBegin.Second - 1);
            DateTime en = PeriodEnd.AddSeconds(1);
            foreach (var tabl in Outs.Dic)
            {
                _tableName = tabl.Key;
                vc += ReadWhole(tabl.Value.Values, beg, en, false);
                if (vc.IsFail) return vc;
            }
            return vc;
        }
    }
}
