using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    [Export(typeof(IProvider))]
    [ExportMetadata("Code", "OvationSource")]
    public class OvationSource : OleDbSource
    {
        protected override void MakeConnect(string name, Logger logger)
        {
            ProviderConnect = new OvationSourceConnect(name, logger);
        }

        //Словарь объектов по Id
        private readonly DicI<ObjectOvation> _objectsId = new DicI<ObjectOvation>();

        //Добавить объект
        protected override SourceObject AddObject(SourceSignal sig)
        {
            int id = sig.Inf.GetInt("Id");
            if (!_objectsId.ContainsKey(id))
                return _objectsId.Add(id, new ObjectOvation(this, id, sig.Inf["CodeObject"]));
            return _objectsId[id];
        }
        
        //Удалить все сигналы
        public override void ClearSignals()
        {
            ProviderSignals.Clear();
            _objectsId.Clear();
        }

        //Чтение объектов из файла клона, перед заполнением клона
        public override void PropareClone()
        {
            
        }

        //Создание фабрики ошибок
        protected override IErrMomFactory MakeErrFactory()
        {
            var factory = new ErrMomFactory(ProviderConnect.Code, ErrMomType.Source);
            factory.AddGoodDescr(0);
            factory.AddDescr(1, "FAIR", ErrorQuality.Warning);
            factory.AddDescr(2, "POOR", ErrorQuality.Warning);
            factory.AddDescr(3, "BAD");
            factory.AddDescr(4, "Нет данных");
            return factory;
        }

        //Чтение значений
        #region
        //Запрос значений из Historian по списку сигналов и интервалу
        protected override bool QueryPartValues(List<SourceObject> part, DateTime beg, DateTime en)
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
            Rec = new ReaderAdo(Connection, sb.ToString());
            if (en.Subtract(beg).TotalMinutes > 59 && (Rec == null || !Rec.HasRows))
            {
                AddWarning("Значения из источника не получены", null, beg + " - " + en +"; " + part.First().Inf + " и др.");
                return IsConnected = false;
            }
            return true;
        }

        //Определение текущего считываемого объекта
        protected override SourceObject DefineObject()
        {
            return _objectsId[Rec.GetInt("Id")];
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
        #endregion
    }
}
