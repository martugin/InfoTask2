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

        //Проверка соединения в настройке
        protected override bool CheckConnection()
        {
            if (Reconnect())
            {
                CheckConnectionMessage = "Успешное соединение с Historian";
                return true;
            }
            AddError(CheckConnectionMessage = "Ошибка соединения с Historian");
            return false;
        }

        //Словарь объектов по Id в Historian
        private readonly DicI<ObjectOvation> _objectsId = new DicI<ObjectOvation>();
        //Объекты сообщений
        private ObjectOvationMsg _alarmObject;
        private ObjectOvationMsg _soeObject;
        private ObjectOvationMsg _textObject;

        //Добавить объект в провайдер
        protected override SourceObject AddObject(InitialSignal sig)
        {
            if (sig.Inf.Get("ObjectType") == "ALARM")
                return _alarmObject ?? (_alarmObject = new ObjectOvationMsg(this, "ALARM"));
            if (sig.Inf.Get("ObjectType") == "SOE")
                return _soeObject ?? (_soeObject = new ObjectOvationMsg(this, "SOE"));
            if (sig.Inf.Get("ObjectType") == "TEXT")
                return _textObject ?? (_textObject = new ObjectOvationMsg(this, "TEXT"));

            int id = sig.Inf.GetInt("Id");
            if (!_objectsId.ContainsKey(id))
                return _objectsId.Add(id, new ObjectOvation(this, id));
            return _objectsId[id];
        }

        //Удалить все сигналы
        protected override void ClearObjects()
        {
            _objectsId.Clear();
            _alarmObject = null;
            _soeObject = null;
            _textObject = null;
        }

        //Создание фабрики ошибок
        protected override IErrMomFactory MakeErrFactory()
        {
            var factory = new ErrMomFactory(ProviderConnect.Name, ErrMomType.Source);
            factory.AddGoodDescr(0);
            factory.AddDescr(1, "FAIR", ErrorQuality.Warning);
            factory.AddDescr(2, "POOR", ErrorQuality.Warning);
            factory.AddDescr(3, "BAD");
            factory.AddDescr(4, "Нет данных");
            return factory;
        }

        //Запрос значений из Historian по списку сигналов и интервалу
        protected override IRecordRead QueryValues(IList<SourceObject> part, DateTime beg, DateTime en, bool isCut)
        {
            var sb = new StringBuilder("select ID, TIMESTAMP, TIME_NSEC, F_VALUE, RAW_VALUE, STS from PT_HF_HIST " + "where (");
            bool isFirst = true;
            foreach (ObjectOvation ob in part)
            {
                if (!isFirst) sb.Append(" or ");
                sb.Append("(ID=").Append(ob.Id).Append(")");
                isFirst = false;
            }
            sb.Append(") and").Append(TimeCondition(beg, en));
            var rec = new ReaderAdo(Connection, sb.ToString());
            if (en.Subtract(beg).TotalMinutes > 59 && !rec.HasRows)
            {
                AddWarning("Значения из источника не получены", null, beg + " - " + en +"; " + part.First().Context + " и др.");
                return null;
            }
            return rec;
        }
        
        //Определение текущего считываемого объекта
        protected override SourceObject DefineObject(IRecordRead rec)
        {
            return _objectsId[rec.GetInt("Id")];
        }

        //Запросы значений по сигналам сообщений разного типа
        protected IRecordRead QueryValuesAlarm(IList<SourceObject> part, DateTime beg, DateTime en, bool isCut)
        {
            return new ReaderAdo(Connection, "select * from MSG_ALARM_HIST" + TimeCondition(beg, en));
        }
        protected IRecordRead QueryValuesSoe(IList<SourceObject> part, DateTime beg, DateTime en, bool isCut)
        {
            return new ReaderAdo(Connection, "select * from MSG_SOE_HIST" + TimeCondition(beg, en));
        }
        protected IRecordRead QueryValuesText(IList<SourceObject> part, DateTime beg, DateTime en, bool isCut)
        {
            return new ReaderAdo(Connection, "select * from MSG_TEXT_HIST" + TimeCondition(beg, en));
        }

        //Строка с условием по времнеи для запросов
        private string TimeCondition(DateTime beg, DateTime en)
        {
            return " (TIMESTAMP >= " + beg.ToOvationString() + ") and (TIMESTAMP <= " + en.ToOvationString() + ") order by TIMESTAMP, TIME_NSEC";
        }

       //Чтение среза
        protected override ValuesCount ReadCut()
        {
            var vc = new ValuesCount();
            using (Start(0, 50)) //Срез по 4 минутам
                vc += ReadByParts(_objectsId.Values, 200, PeriodBegin.AddMinutes(-4), PeriodBegin, true);
            if (vc.IsBad) return vc;
            using (Start(50, 100)) //Срез по 61 минуте
                vc += ReadByParts(_objectsId.Values, 200, PeriodBegin.AddMinutes(-61), PeriodBegin.AddMinutes(-4), true);
            return vc;
        }

        //Чтение изменений
        protected override ValuesCount ReadChanges()
        {
            var vc = new ValuesCount();
            using (Start(0, 70))
                vc += ReadByParts(_objectsId.Values, 200);

            using (Start(70, 80))
                vc += ReadOneObject(_alarmObject, QueryValuesAlarm, "Чтение сигнализационных сообщений");
            using (Start(80, 90))
                vc += ReadOneObject(_soeObject, QueryValuesSoe, "Чтение событий");
            using (Start(90, 100))
                vc += ReadOneObject(_textObject, QueryValuesText, "Чтение текстовых сообщений");
            return vc;
        }
    }
}
