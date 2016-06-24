using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using BaseLibrary;
using ProvidersLibrary;

namespace Provider
{
    [Export(typeof(Prov))]
    [ExportMetadata("Code", "OvationSource")]
    public class OvationSource : OleDbSour
    {
        //Соединение
        internal OvationConn OvationConn { get { return (OvationConn) Conn; } }
        //Код провайдера
        public override string Code { get { return "OvationSource"; } }

        public override string Hash { get { return "OvationHistorian=" + _dataSource; } }
        //Настройки провайдера
        protected override void ReadInf(DicS<string> dic)
        {
            _dataSource = dic["DataSource"];
        }
        //Имя дропа
        private string _dataSource;

        protected override string ConnectionString
        {
            get { return "Provider=OvHOleDbProv.OvHOleDbProv.1;Persist Security Info=True;User ID='';" +
                              "Data Source=" + _dataSource + ";Location='';Mode=ReadWrite;Extended Properties=''"; }
        }

                //Проверка соединения в настройке
        public override bool CheckConnection()
        {
            if (Check())
            {
                CheckConnectionMessage = "Успешное соединение с Historian";
                return true;
            }
            AddError(CheckConnectionMessage = "Ошибка соединения с Historian");
            return false;
        }

        //Чтение значений
        #region
        //Запрос значений из Historian по списку сигналов и интервалу
        protected override IRecordRead QueryPartValues(List<SourObject> part, DateTime beg, DateTime en, bool isCut)
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
            var rec = new ReaderAdo(Connection, sb.ToString());
            if (en.Subtract(beg).TotalMinutes > 59 && !rec.HasRows)
            {
                AddWarning("Значения из источника не получены", null, beg + " - " + en +"; " + part.First().CodeObject + " и др.");
                IsConnected = false;
                return null;
            }
            return rec;
        }

        //Определение текущего считываемого объекта
        protected override SourObject DefineObject(IRecordRead rec)
        {
            return OvationConn.ObjectsId[rec.GetInt("Id")];
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
                ReadValuesByParts(OvationConn.ObjectsId.Values, 200, PeriodBegin.AddMinutes(-4), PeriodBegin, true);
            using (Start(50, 100)) //Срез по 61 минуте
                ReadValuesByParts(OvationConn.ObjectsId.Values, 200, PeriodBegin.AddMinutes(-61), PeriodBegin.AddMinutes(-4), true);
        }

        //Чтение изменений
        protected override void ReadChanges()
        {
            ReadValuesByParts(OvationConn.ObjectsId.Values, 200, PeriodBegin, PeriodEnd, false);
        }
        #endregion
    }
}
