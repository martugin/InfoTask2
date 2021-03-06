﻿using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Ovation
{
    [Export(typeof(Provider))]
    [ExportMetadata("Code", "OvationSource")]
    public class OvationSource : OleDbSource
    {
        //Код провайдера
        public override string Code { get { return "OvationSource"; } }

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
        internal readonly DicI<OvationOut> OutsId = new DicI<OvationOut>();
        //Объекты сообщений
        internal OvationMsgOut AlarmOut;
        internal OvationMsgOut SoeOut;
        internal OvationMsgOut TextOut;

        //Добавить выход в провайдер
        protected override ProviderOut AddOut(ProviderSignal sig)
        {
            var obType = sig.Inf.Get("ObjectType").ToUpper();
            switch (obType)
            {
                case "ALARM":
                    return AlarmOut ?? (AlarmOut = new OvationMsgOut(this, "ALARM"));
                case "SOE":
                    return SoeOut ?? (SoeOut = new OvationMsgOut(this, "SOE"));
                case "TEXT":
                    return TextOut ?? (TextOut = new OvationMsgOut(this, "TEXT"));
            }

            int id = sig.Inf.GetInt("Id");
            return OutsId.ContainsKey(id) 
                ? OutsId[id] 
                : OutsId.Add(id, new OvationOut(this, id));
        }

        //Удалить все выходы
        protected override void ClearOuts()
        {
            OutsId.Clear();
            AlarmOut = null;
            SoeOut = null;
            TextOut = null;
        }

        //Создание фабрики ошибок
        protected override IMomErrFactory MakeErrFactory()
        {
            var factory = new MomErrFactory(ProviderConnect.Code, MomErrType.Source);
            factory.AddGoodDescr(0);
            factory.AddDescr(1, "FAIR", ErrQuality.Warning);
            factory.AddDescr(2, "POOR", ErrQuality.Warning);
            factory.AddDescr(3, "BAD");
            factory.AddDescr(4, "Нет данных");
            return factory;
        }

        //Запрос значений из Historian по списку выходов и интервалу
        protected override IRecordRead QueryValues(IList<ListSourceOut> part, DateTime beg, DateTime en, bool isCut)
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
        protected override ListSourceOut DefineOut(IRecordRead rec)
        {
            return OutsId[rec.GetInt("Id")];
        }

        //Запросы значений по сигналам сообщений разного типа
        protected IRecordRead QueryAlarmValues(IList<ListSourceOut> part, DateTime beg, DateTime en, bool isCut)
        {
            return new AdoReader(Connection, "select * from MSG_ALARM_HIST" + TimeCondition(beg, en));
        }
        protected IRecordRead QuerySoeValues(IList<ListSourceOut> part, DateTime beg, DateTime en, bool isCut)
        {
            return new AdoReader(Connection, "select * from MSG_SOE_HIST" + TimeCondition(beg, en));
        }
        protected IRecordRead QueryTextValues(IList<ListSourceOut> part, DateTime beg, DateTime en, bool isCut)
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
                vc += ReadByParts(OutsId.Values, 200, PeriodBegin.AddMinutes(-4), PeriodBegin, true);
            if (vc.IsFail) return vc;
            using (Start(50, 100)) //Срез по 61 минуте
                vc += ReadByParts(OutsId.Values, 200, PeriodBegin.AddMinutes(-61), PeriodBegin.AddMinutes(-4), true);
            return vc;
        }

        //Чтение изменений
        protected override ValuesCount ReadChanges(DateTime beg, DateTime en)
        {
            var vc = new ValuesCount();
            using (Start(0, 70))
                vc += ReadByParts(OutsId.Values, 200, beg, en, false);

            using (Start(70, 80))
                vc += ReadOneOut(AlarmOut, beg, en, false, QueryAlarmValues, "Чтение сигнализационных сообщений");
            using (Start(80, 90))
                vc += ReadOneOut(SoeOut, beg, en, false, QuerySoeValues, "Чтение событий");
            using (Start(90, 100))
                vc += ReadOneOut(TextOut, beg, en, false, QueryTextValues, "Чтение текстовых сообщений");
            return vc;
        }
    }
}
