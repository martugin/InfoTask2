using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Provider
{
    [Export(typeof(BaseProvider))]
    [ExportMetadata("Code", "OvationSource")]
    public class OvationSource : OleDbSource
    {
        //Код провайдера
        public override string Code { get { return "OvationSource"; } }

        protected override string Hash { get { return "OvationHistorian=" + _dataSource; } }
        //Настройки провайдера
        protected override void ReadInf(DicS<string> dic)
        {
            _dataSource = dic["DataSource"];
        }
        //Имя дропа
        private string _dataSource;
        
        //Строка соединения с OleDb
        protected override string ConnectionString
        {
            get
            {
                return "Provider=OvHOleDbProv.OvHOleDbProv.1;Persist Security Info=True;User ID='';" +
                            "Data Source=" + _dataSource + ";Location='';Mode=ReadWrite;Extended Properties=''";
            }
        }

        //Словарь объектов по Id в Historian
        private readonly DicI<OvationOut> _outsId = new DicI<OvationOut>();
        //Объекты сообщений
        private OvationMsgOut _alarmOut;
        private OvationMsgOut _soeOut;
        private OvationMsgOut _textOut;

        //Добавить выход в провайдер
        protected override SourceOut AddOut(InitialSignal sig)
        {
            var obType = sig.Inf.Get("ObjectType");
            switch (obType)
            {
                case "ALARM":
                    return _alarmOut ?? (_alarmOut = new OvationMsgOut(this, "ALARM"));
                case "SOE":
                    return _soeOut ?? (_soeOut = new OvationMsgOut(this, "SOE"));
                case "TEXT":
                    return _textOut ?? (_textOut = new OvationMsgOut(this, "TEXT"));
            }

            int id = sig.Inf.GetInt("Id");
            return _outsId.ContainsKey(id) 
                ? _outsId[id] 
                : _outsId.Add(id, new OvationOut(this, id));
        }

        //Удалить все выходы
        protected override void ClearOuts()
        {
            _outsId.Clear();
            _alarmOut = null;
            _soeOut = null;
            _textOut = null;
        }

        //Создание фабрики ошибок
        protected override IMomErrFactory MakeErrFactory()
        {
            var factory = new MomErrFactory(ProviderConnect.Name, MomErrType.Source);
            factory.AddGoodDescr(0);
            factory.AddDescr(1, "FAIR", ErrQuality.Warning);
            factory.AddDescr(2, "POOR", ErrQuality.Warning);
            factory.AddDescr(3, "BAD");
            factory.AddDescr(4, "Нет данных");
            return factory;
        }

        //Запрос значений из Historian по списку выходов и интервалу
        protected override IRecordRead QueryValues(IList<SourceOut> part, DateTime beg, DateTime en, bool isCut)
        {
            var sb = new StringBuilder("select ID, TIMESTAMP, TIME_NSEC, F_VALUE, RAW_VALUE, STS from PT_HF_HIST " + "where (");
            bool isFirst = true;
            foreach (OvationOut ob in part)
            {
                if (!isFirst) sb.Append(" or ");
                sb.Append("(ID=").Append(ob.Id).Append(")");
                isFirst = false;
            }
            sb.Append(") and").Append(TimeCondition(beg, en));
            var rec = new AdoReader(Connection, sb.ToString());
            if (en.Subtract(beg).TotalMinutes > 59 && !rec.HasRows)
                AddWarning("Значения из источника не получены", null, beg + " - " + en +"; " + part.First().Context + " и др.");
            return rec;
        }
        
        //Определение текущего считываемого выхода
        protected override SourceOut DefineOut(IRecordRead rec)
        {
            return _outsId[rec.GetInt("Id")];
        }

        //Запросы значений по сигналам сообщений разного типа
        protected IRecordRead QueryAlarmValues(IList<SourceOut> part, DateTime beg, DateTime en, bool isCut)
        {
            return new AdoReader(Connection, "select * from MSG_ALARM_HIST" + TimeCondition(beg, en));
        }
        protected IRecordRead QuerySoeValues(IList<SourceOut> part, DateTime beg, DateTime en, bool isCut)
        {
            return new AdoReader(Connection, "select * from MSG_SOE_HIST" + TimeCondition(beg, en));
        }
        protected IRecordRead QueryTextValues(IList<SourceOut> part, DateTime beg, DateTime en, bool isCut)
        {
            return new AdoReader(Connection, "select * from MSG_TEXT_HIST" + TimeCondition(beg, en));
        }

        //Преводит дату в формат для запросов Ovation Historian
        private static string DateToOvation(DateTime d)
        {
            DateTime dd = d.ToUniversalTime();
            CultureInfo ci = CultureInfo.CreateSpecificCulture("en-US");
            return "#" + dd.ToString("MM/dd/yyyy HH:mm:ss.fff", ci) + "#";
        }

        //Строка с условием по времени для запросов
        private static string TimeCondition(DateTime beg, DateTime en)
        {
            return " (TIMESTAMP >= " + DateToOvation(beg) + ") and (TIMESTAMP <= " + DateToOvation(en) + ") order by TIMESTAMP, TIME_NSEC";
        }

       //Чтение среза
        protected override ValuesCount ReadCut()
        {
            var vc = new ValuesCount();
            using (Start(0, 50)) //Срез по 4 минутам
                vc += ReadByParts(_outsId.Values, 200, PeriodBegin.AddMinutes(-4), PeriodBegin, true);
            if (vc.IsFail) return vc;
            using (Start(50, 100)) //Срез по 61 минуте
                vc += ReadByParts(_outsId.Values, 200, PeriodBegin.AddMinutes(-61), PeriodBegin.AddMinutes(-4), true);
            return vc;
        }

        //Чтение изменений
        protected override ValuesCount ReadChanges()
        {
            var vc = new ValuesCount();
            using (Start(0, 70))
                vc += ReadByParts(_outsId.Values, 200);

            using (Start(70, 80))
                vc += ReadOneOut(_alarmOut, QueryAlarmValues, "Чтение сигнализационных сообщений");
            using (Start(80, 90))
                vc += ReadOneOut(_soeOut, QuerySoeValues, "Чтение событий");
            using (Start(90, 100))
                vc += ReadOneOut(_textOut, QueryTextValues, "Чтение текстовых сообщений");
            return vc;
        }
    }
}
