using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using BaseLibrary;
using ProvidersLibrary;

namespace Logika
{
    [Export(typeof(ProviderBase))]
    [ExportMetadata("Code", "PrologSource")]
    public class PrologSource : AccessSource
    {
        //Код провайдера
        public override string Code { get { return "PrologSource"; } }

        //Проверка соединения с файлом
        protected override bool ConnectProvider()
        {
            return DaoDb.Check(DbFile, new[] {"NODES", "ABONENTS"});
        }

        //Словарь выходов, первый ключ - код таблицы, второй ключ - id объекта
        private readonly DicS<DicI<OutProlog>> _outs = new DicS<DicI<OutProlog>>();
        //Словарь выходов ключ - id объекта
        private readonly DicI<OutProlog> _outsId = new DicI<OutProlog>();

        //Добавить выход в источник
        protected override SourceOut AddOut(InitialSignal sig)
        {
            int id = sig.Inf.GetInt("NodeId");
            if (_outsId.ContainsKey(id))
                return _outsId[id];
            string tableName = sig.Inf["TableName"];
            if (!_outs.ContainsKey(tableName))
                _outs.Add(tableName, new DicI<OutProlog>());
            var ob = new OutProlog(this);
            _outs[tableName].Add(id, ob);
            return _outsId.Add(id, ob);
        }

        //Очистка списка выходов
        protected override void ClearOuts()
        {
            _outs.Clear();
            _outsId.Clear();
        }

        //Имя текущей считываемой таблицы
        private string _tableName;

        //Запрос значений
        protected override IRecordRead QueryValues(IList<SourceOut> part, DateTime beg, DateTime en, bool isCut)
        {
            return new RecDao(DbFile, "SELECT * FROM " + _tableName + "_ARCHIVE " +
                                      "WHERE (TYPE = 1) AND (Время >= " + beg.ToAccessString() + ") AND (Время <= " + en.ToAccessString() + ")"); 
        }

        //Определение текущего выхода
        protected override SourceOut DefineOut(IRecordRead rec)
        {
            return _outsId[rec.GetInt("PARENT_ID")];
        }

        //Чтение значений, срез считывается вместе с изменениями
        protected override ValuesCount ReadChanges()
        {
            var vc = new ValuesCount();
            DateTime beg = PeriodBegin.AddMinutes(-PeriodBegin.Minute).AddSeconds(-PeriodBegin.Second - 1);
            DateTime en = PeriodEnd.AddSeconds(1);
            foreach (var tabl in _outs.Dic)
            {
                _tableName = tabl.Key;
                vc += ReadWhole(tabl.Value.Values, beg, en, false);
                if (vc.IsFail) return vc;
            }
            return vc;
        }
    }
}
