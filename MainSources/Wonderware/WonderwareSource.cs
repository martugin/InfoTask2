using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Provider
{
    [Export(typeof(IProvider))]
    [ExportMetadata("Code", "WonderwareSource")]
    public class WonderwareSource : OleDbSource
    {
        //Словарь объектов по TagName
        private readonly Dictionary<string, ObjectWonderware> _objects = new Dictionary<string, ObjectWonderware>();

        //Добавить сигнал в провайдер
        protected override SourceObject AddObject(SourceSignal sig, string context)
        {
            string tag = sig.Inf["TagName"];
            if (!_objects.ContainsKey(tag))
                _objects.Add(tag, new ObjectWonderware(this, tag));
            return _objects[tag];
        }
        
        //Очистка списка сигналов
        public override void ClearSignals()
        {
            ProviderSignals.Clear();
            _objects.Clear();
        }
        
        //Чтение значений
        #region
        //Создание фабрики ошибок
        protected override IErrMomFactory MakeErrFactory()
        {
            var factory = new ErrMomFactory(ProviderConnect.Code, ErrMomType.Source);
            factory.AddGoodDescr(192);
            factory.AddDescr(0, "Bad Quality of undetermined state");
            factory.AddDescr(1, "No data available, tag did not exist at the time");
            factory.AddDescr(2, "Running insert");
            factory.AddDescr(10, "Communication loss");
            factory.AddDescr(16, "Good value, received out of time sync (cyclic tag)");
            factory.AddDescr(17, "Duplicate time stamp; infinite slope");
            factory.AddDescr(20, "IDAS overflow recovery");
            factory.AddDescr(24, "IOServer communication failed");
            factory.AddDescr(33, "Violation of History Duration license feature");
            factory.AddDescr(44, "Pipe reconnect");
            factory.AddDescr(64, "Cannot convert");
            factory.AddDescr(150, "Storage startup");
            factory.AddDescr(151, "Store forward storage startup");
            factory.AddDescr(152, "Incomplete calculated value");
            factory.AddDescr(202, "Good point of version Latest", ErrorQuality.Warning);
            factory.AddDescr(212, "Counter rollover has occurred");
            factory.AddDescr(248, "First value received in store forward mode", ErrorQuality.Warning);
            factory.AddDescr(249, "Not a number");
            factory.AddDescr(252, "First value received from IOServer", ErrorQuality.Warning);
            factory.AddDescr(65536, "No data stored in history, NULL");
            return factory;
        }

        //Запрос значений по одному блоку сигналов
        protected override bool QueryPartValues(List<SourceObject> part, DateTime beg, DateTime en)
        {
            var sb = new StringBuilder("SELECT TagName, DateTime = convert(nvarchar, DateTime, 21), Value, vValue, Quality, QualityDetail FROM History WHERE  TagName IN (");
            for (var n = 0; n < part.Count; n++)
            {
                if (n != 0) sb.Append(", ");
                var ob = (ObjectWonderware) part[n];
                sb.Append("'").Append(ob.TagName).Append("'");
            }
            sb.Append(") AND wwRetrievalMode = 'Delta'");
            sb.Append(" AND DateTime >= ").Append(beg.ToSqlString());
            if (beg.ToSqlString() != en.ToSqlString())
                sb.Append(" AND DateTime <").Append(en.ToSqlString());
            sb.Append(" ORDER BY DateTime");
            
            Rec = new ReaderAdo(SqlProps, sb.ToString(), 10000);
            return true;
        }

        //Определение текущего считываемого объекта
        protected override SourceObject DefineObject()
        {
            string code = Rec.GetString("TagName");
            if (_objects.ContainsKey(code))
                return _objects[code];
            return null;
        }

        //Задание нестандартных свойств получения данных
        protected override void SetReadProperties()
        {
            NeedCut = false;
            ReconnectsCount = 1;
        }

        //Чтение данных из Historian за период
        protected override void ReadChanges()
        {
            ReadValuesByParts(_objects.Values, 500, PeriodBegin, PeriodEnd, false);
        }
        #endregion
    }
}
