using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    //Провайдер источника Wonderware
    [Export(typeof(Prov))]
    [ExportMetadata("Code", "WonderwareSource")]
    public class WonderwareSour : SqlSourceConnect
    {
        //Код провайдера
        public override string Code { get { return "WonderwareSource"; } }
        //Ссылка на соединение
        public WonderwareConn WonderwareConn { get { return (WonderwareConn)Conn; } }

        //Получение диапазона архива по блокам истории
        protected override TimeInterval GetSourceTime()
        {
            DateTime mind = Different.MaxDate, maxd = Different.MinDate;
            try
            {
                using (var rec = new ReaderAdo(SqlProps, "SELECT FromDate, ToDate FROM v_HistoryBlock ORDER BY FromDate, ToDate DESC"))
                    while (rec.Read())
                    {
                        var fromd = rec.GetTime("FromDate");
                        var tod = rec.GetTime("ToDate");
                        if (fromd < mind) mind = fromd;
                        if (maxd < tod) maxd = tod;
                    }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при получении диапазона источника", ex);
            }
            if (mind == Different.MaxDate && maxd == Different.MinDate)
                return new TimeInterval(Different.MinDate, Different.MaxDate);
            AddEvent("Диапазон источника определен", mind + " - " + maxd);
            return new TimeInterval(mind, maxd);
        }

        //Запрос значений по одному блоку сигналов
        protected override IRecordRead QueryPartValues(List<SourceObject> part, DateTime beg, DateTime en, bool isCut)
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
            if ( WonderwareConn.Objects.ContainsKey(code))
                return WonderwareConn.Objects[code];
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
            ReadValuesByParts(WonderwareConn.Objects.Values, 500, PeriodBegin, PeriodEnd, false);
        }
    }
}