﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Provider
{
    //Провайдер источника Wonderware
    [Export(typeof(ProviderBase))]
    [ExportMetadata("Code", "WonderwareSource")]
    public class WonderwareSource : SqlServerSource
    {
        //Код провайдера
        public override string Code { get { return "WonderwareSource"; } }

        //Получение диапазона архива по блокам истории
        protected override TimeInterval GetTimeSource()
        {
            DateTime mind = Different.MaxDate, maxd = Different.MinDate;
            using (var rec = new ReaderAdo(SqlProps, "SELECT FromDate, ToDate FROM v_HistoryBlock ORDER BY FromDate, ToDate DESC"))
                while (rec.Read())
                {
                    var fromd = rec.GetTime("FromDate");
                    var tod = rec.GetTime("ToDate");
                    if (fromd < mind) mind = fromd;
                    if (maxd < tod) maxd = tod;
                }
            if (mind == Different.MaxDate && maxd == Different.MinDate)
                return TimeInterval.CreateDefault();
            return new TimeInterval(mind, maxd);
        }

        //Словарь объектов по TagName
        private readonly Dictionary<string, ObjectWonderware> _objects = new Dictionary<string, ObjectWonderware>();

        //Добавить объект в провайдер
        protected override SourceObject AddObject(InitialSignal sig)
        {
            string tag = sig.Inf["TagName"];
            if (!_objects.ContainsKey(tag))
                _objects.Add(tag, new ObjectWonderware(this, tag));
            return _objects[tag];
        }

        //Очистка списка сигналов
        protected override void ClearObjects()
        {
            _objects.Clear();
        }

        //Создание фабрики ошибок
        protected override IErrMomFactory MakeErrFactory()
        {
            var factory = new ErrMomFactory(ProviderConnect.Name, ErrMomType.Source);
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
        protected override IRecordRead QueryValues(IList<SourceObject> part, DateTime beg, DateTime en, bool isCut)
        {
            var sb = new StringBuilder("SELECT TagName, DateTime = convert(nvarchar, DateTime, 21), Value, vValue, Quality, QualityDetail FROM History WHERE  TagName IN (");
            for (var n = 0; n < part.Count; n++)
            {
                if (n != 0) sb.Append(", ");
                var ob = (ObjectWonderware)part[n];
                sb.Append("'").Append(ob.TagName).Append("'");
            }
            sb.Append(") AND wwRetrievalMode = 'Delta'");
            sb.Append(" AND DateTime >= ").Append(beg.ToSqlString());
            if (beg.ToSqlString() != en.ToSqlString())
                sb.Append(" AND DateTime <").Append(en.ToSqlString());
            sb.Append(" ORDER BY DateTime");

            return new ReaderAdo(SqlProps, sb.ToString(), 10000);
        }

        //Определение текущего считываемого объекта
        protected override SourceObject DefineObject(IRecordRead rec)
        {
            string code = rec.GetString("TagName");
            if (_objects.ContainsKey(code))
                return _objects[code];
            return null;
        }
        
        //Чтение данных из Historian за период
        protected override ValuesCount ReadChanges()
        {
            return ReadByParts(_objects.Values, 500);
        }
    }
}