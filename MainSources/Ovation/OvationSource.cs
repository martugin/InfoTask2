using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Provider
{
    [Export(typeof(ProviderBase))]
    [ExportMetadata("Code", "OvationSource")]
    public class OvationSource : AdoSource
    {
        //Код провайдера
        public override string Code { get { return "OvationSource"; } }
        //Комплект
        public override string Complect { get { return "Ovation"; } }
        //Создание подключения
        protected override ProviderSettings CreateConnect()
        {
            return new OvationSettings();
        }
        //Подключение
        internal OvationSettings Settings { get { return (OvationSettings)CurSettings; } }

        //Словарь объектов по Id в Historian
        private readonly DicI<ObjectOvation> _objectsId = new DicI<ObjectOvation>();

        //Добавить объект в провайдер
        protected override SourceObject AddObject(InitialSignal sig)
        {
            int id = sig.Inf.GetInt("Id");
            if (!_objectsId.ContainsKey(id))
                return _objectsId.Add(id, new ObjectOvation(this, id));
            return _objectsId[id];
        }

        //Удалить все сигналы
        protected override void ClearObjects()
        {
            _objectsId.Clear();
        }

        //Создание фабрики ошибок
        protected override IErrMomFactory MakeErrFactory()
        {
            var factory = new ErrMomFactory(Name, ErrMomType.Source);
            factory.AddGoodDescr(0);
            factory.AddDescr(1, "FAIR", ErrorQuality.Warning);
            factory.AddDescr(2, "POOR", ErrorQuality.Warning);
            factory.AddDescr(3, "BAD");
            factory.AddDescr(4, "Нет данных");
            return factory;
        }

        //Запрос значений из Historian по списку сигналов и интервалу
        protected override IRecordRead QueryPartValues(List<SourceObject> part, DateTime beg, DateTime en, bool isCut)
        {
            var sb = new StringBuilder("select ID, TIMESTAMP, TIME_NSEC, F_VALUE, RAW_VALUE, STS from PT_HF_HIST " + "where (");
            bool isFirst = true;
            foreach (ObjectOvation ob in part)
            {
                if (!isFirst) sb.Append(" or ");
                sb.Append("(ID=").Append(ob.Id).Append(")");
                isFirst = false;
            }
            sb.Append(") and ")
              .Append(" (TIMESTAMP >= ")
              .Append(beg.ToOvationString())
              .Append(") and (TIMESTAMP <= ")
              .Append(en.ToOvationString())
              .Append(") order by TIMESTAMP, TIME_NSEC");
            var rec = new ReaderAdo(Settings.Connection, sb.ToString());
            if (en.Subtract(beg).TotalMinutes > 59 && !rec.HasRows)
            {
                AddWarning("Значения из источника не получены", null, beg + " - " + en +"; " + part.First().Context + " и др.");
                IsConnected = false;
                return null;
            }
            return rec;
        }

        //Определение текущего считываемого объекта
        protected override SourceObject DefineObject(IRecordRead rec)
        {
            return _objectsId[rec.GetInt("Id")];
        }

        //Задание нестандартных свойств получения данных
        protected override void SetReadProperties()
        {
            MaxErrorCount = 5;
            MaxErrorDepth = 5;
        }

        //Чтение среза
        protected override void ReadCut()
        {
            using (Start(0, 50)) //Срез по 4 минутам
                ReadValuesByParts(_objectsId.Values, 200, PeriodBegin.AddMinutes(-4), PeriodBegin, true);
            using (Start(50, 100)) //Срез по 61 минуте
                ReadValuesByParts(_objectsId.Values, 200, PeriodBegin.AddMinutes(-61), PeriodBegin.AddMinutes(-4), true);
        }

        //Чтение изменений
        protected override void ReadChanges()
        {
            ReadValuesByParts(_objectsId.Values, 200, PeriodBegin, PeriodEnd, false);
        }
    }
}
