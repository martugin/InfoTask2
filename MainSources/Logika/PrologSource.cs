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

        //Словарь объектов, первый ключ - код таблицы, второй ключ - id объекта
        private readonly DicS<DicI<ObjectProlog>> _objects = new DicS<DicI<ObjectProlog>>();
        //Словарь объектов ключ - id объекта
        private readonly DicI<ObjectProlog> _objectsId = new DicI<ObjectProlog>();

        //Добавить объект в источник
        protected override SourceObject AddObject(InitialSignal sig)
        {
            int id = sig.Inf.GetInt("NodeId");
            if (_objectsId.ContainsKey(id))
                return _objectsId[id];
            string tableName = sig.Inf["TableName"];
            if (!_objects.ContainsKey(tableName))
                _objects.Add(tableName, new DicI<ObjectProlog>());
            var ob = new ObjectProlog(this);
            _objects[tableName].Add(id, ob);
            return _objectsId.Add(id, ob);
        }

        //Очистка списка объектов
        protected override void ClearObjects()
        {
            _objects.Clear();
            _objectsId.Clear();
        }

        //Чтение значений, срез считывается вместе с изменениями
        protected override ValuesCount ReadChanges()
        {
            var vc = new ValuesCount();
            DateTime beg = PeriodBegin.AddMinutes(-PeriodBegin.Minute).AddSeconds(-PeriodBegin.Second - 1);
            DateTime en = PeriodEnd.AddSeconds(1);
            foreach (var tabl in _objects.Dic)
            {
                _tableName = tabl.Key;
                vc += ReadWhole(tabl.Value.Values, beg, en, false);
                if (vc.NeedBreak) return vc;
            }
            return vc;
        }

        //Имя текущей считываемой таблицы
        private string _tableName;

        //Запрос значений
        protected override IRecordRead QueryValues(IList<SourceObject> part, DateTime beg, DateTime en, bool isCut)
        {
            return new RecDao(DbFile, "SELECT * FROM " + _tableName + "_ARCHIVE " +
                                      "WHERE (Время >= " + beg.ToAccessString() + ") AND (Время <= " +en.ToAccessString() + ")"); 
        }

        //Определение текущего объекта
        protected override SourceObject DefineObject(IRecordRead rec)
        {
            return _objectsId[rec.GetInt("PARENT_ID")];
        }
    }
}
