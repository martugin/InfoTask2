using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Logika
{
    [Export(typeof(ProviderBase))]
    [ExportMetadata("Code", "PrologSource")]
    public class PrologSource : AdoSource
    {
        //Код провайдера
        public override string Code { get { return "PrologSource"; } }
        //Комплект
        public override string Complect { get { return "Logika"; } }
        //Создание подключения
        protected override ProviderSettings CreateConnect()
        {
            return new AccessSourceSettings();
        }
        //Ссылка на соединение
        public AccessSourceSettings Settings { get { return (AccessSourceSettings)CurSettings; } }

        //Словарь объектов, первый ключ - код таблицы, второй ключ - id объекта
        private readonly DicS<DicI<ObjectProlog>> _objects = new DicS<DicI<ObjectProlog>>();
        //Словарь объектов ключ - id объекта
        private readonly DicI<ObjectProlog> _objectsId = new DicI<ObjectProlog>();

        //Добавить объект в источник
        protected override SourceObject AddObject(UniformSignal sig)
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

        protected override void ReadChanges()
        {
            throw new NotImplementedException();
        }

        protected override IRecordRead QueryPartValues(List<SourceObject> part, DateTime beg, DateTime en, bool isCut)
        {
            throw new NotImplementedException();
        }

        protected override SourceObject DefineObject(IRecordRead rec)
        {
            return _objectsId[rec.GetInt("PARENT_ID")];
        }
    }
}
