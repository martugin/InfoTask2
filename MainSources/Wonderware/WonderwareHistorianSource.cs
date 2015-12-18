using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    [Export(typeof(IProvider))]
    [ExportMetadata("Code", "WonderwareHistorianSource")]
    public class WonderwareHistorianSource : SourceBase, ISource 
    {
        //Код провайдера
        public override string Code { get { return "WonderwareHistorianSource"; } }
        //Настройки провайдера 
        public string Inf
        {
            get { return ProviderInf; }
            set 
            { 
                ProviderInf = value;
                var dic = ProviderInf.ToPropertyDicS();
                dic.DefVal = "";
                bool e = dic["IndentType"].ToUpper() != "WINDOWS";
                string server = dic["SQLServer"], db = dic["Database"];
                _sqlProps = new SqlProps(server, db, e, dic["Login"], dic["Password"]);
                Hash = "SQLServer=" + server + ";Database=" + db;
            }
        }
        
        //Возвращает выпадающий список для поля настройки, props - словарь значение свойств, propname - имя свойства для ячейки со списком
        public override List<string> ComboBoxList(Dictionary<string, string> props, string propname)
        {
            try
            {
                bool hasServer = props.ContainsKey("SQLServer") && !props["SQLServer"].IsEmpty();
                var hasLogin = (props["IndentType"].ToUpper() == "WINDOWS" || (props.ContainsKey("Login") && !props["Login"].IsEmpty()));
                if (propname == "Database" && hasServer && hasLogin)
                    return SqlDb.SqlDatabasesList(props["SQLServer"], props["IndentType"].ToUpper() != "WINDOWS", props["Login"], props["Password"]);
            }
            catch { }
            return new List<string>();
        }

        //Настройки SQL Server
        private SqlProps _sqlProps;

        //Словарь объектов по TagName
        private readonly Dictionary<string, ObjectWonderware> _objects = new Dictionary<string, ObjectWonderware>();

        //Добавить сигнал в провайдер
        public SourceSignal AddSignal(string signalInf, string code, DataType dataType, int idInClone = 0)
        {
            var sig = new SignalWonderware(signalInf, code, dataType, this, idInClone);
            if (!_objects.ContainsKey(sig.TagName))
                _objects.Add(sig.TagName, new ObjectWonderware(sig.TagName));
            var ret = _objects[sig.TagName].AddSignal(sig);
            if (ret == sig) ProviderSignals.Add(sig.Code, sig);
            return ret;
        }

        //Очистка списка сигналов
        public void ClearSignals()
        {
            ProviderSignals.Clear();
            _objects.Clear();
        }
        
        //Проверка соединения
        public bool Check()
        {
            return Danger(TryCheck, 2, 500, "Не удалось соединиться с SQL-сервером");
        }
        private bool TryCheck()
        {
            try
            {
                using (SqlDb.Connect(_sqlProps))
                    return true;
            }
            catch (Exception ex)
            {
                AddError("Не удалось соединиться с SQL-сервером", ex);
                return false;
            }
        }
        
        //Проверка настроек
        public string CheckSettings(Dictionary<string, string> inf, Dictionary<string, string> names)
        {
            string err = "";
            if (inf["SQLServer"].IsEmpty()) err += "Не указано имя SQL-сервера" + Environment.NewLine;
            if (inf["IndentType"].IsEmpty()) err += "Не задан тип идентификации" + Environment.NewLine;
            if (inf["IndentType"] == "SqlServer" && inf["Login"].IsEmpty()) err += "Не задан логин" + Environment.NewLine;
            if (inf["Database"].IsEmpty()) err += "Не задано имя базы данных" + Environment.NewLine;
            return err;
        }

        //Проверка соединения
        public bool CheckConnection()
        {
            if (Check())
            {
                CheckConnectionMessage = "Успешное соединение";
                return true;
            }
            AddError(CheckConnectionMessage = "Не удалось соединиться с SQL-сервером");
            return false;
        }

        //Получение диапазона архива по блокам истории
        public TimeInterval GetTime()
        {
            TimeIntervals.Clear();
            DateTime mind = Different.MaxDate, maxd = Different.MinDate;
            DateTime mint = Different.MaxDate, maxt = Different.MinDate;
            try
            {
                using (var rec = new ReaderAdo(_sqlProps, "SELECT FromDate, ToDate FROM v_HistoryBlock ORDER BY FromDate, ToDate DESC"))
                    while (rec.Read())
                    {
                        var fromd = rec.GetTime("FromDate");
                        var tod = rec.GetTime("ToDate");
                        if (fromd < mind) mind = fromd;
                        if (fromd.Subtract(maxt).TotalMinutes > 1)
                        {
                            if (maxt != Different.MinDate) TimeIntervals.Add(new TimeInterval(mint, maxt));
                            mint = fromd;
                        }
                        if (maxd < tod) maxd = tod;
                        if (maxt < tod) maxt = tod;
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

        //Чтение значений
        #region
        //Создание фабрики ошибок
        protected override IErrMomFactory MakeErrFactory()
        {
            var factory = new ErrMomFactory(Code, ErrMomType.Source);
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

        //Рекордсет со значениями
        private ReaderAdo _rec;

        //Запрос значений по одному блоку сигналов
        protected override bool QueryPartValues(List<SourceObject> part, DateTime beg, DateTime en, bool isCut)
        {
            var sb = new StringBuilder("SELECT TagName, DateTime = convert(nvarchar, DateTime, 21), Value, vValue, Quality, QualityDetail FROM History WHERE  TagName IN (");
            for (var n = 0; n < part.Count; n++)
            {
                if (n != 0) sb.Append(", ");
                var ob = (ObjectWonderware) part[n];
                sb.Append("'").Append(ob.Code).Append("'");
            }
            sb.Append(") AND wwRetrievalMode = 'Delta'");
            sb.Append(" AND DateTime >= ").Append(beg.ToSqlString());
            if (beg.ToSqlString() != en.ToSqlString())
                sb.Append(" AND DateTime <").Append(en.ToSqlString());
            sb.Append(" ORDER BY DateTime");
            
            _rec = new ReaderAdo(_sqlProps, sb.ToString(), 10000);
            return true;
        }

        //Формирование значений по одному блоку сигналов
        protected override Tuple<int, int> ReadPartValues(bool isCut)
        {
            int nread = 0, nwrite = 0;
            using (_rec)
                while (_rec.Read())
                {
                    string code = "";
                    try
                    {
                        code = _rec.GetString("TagName");
                        if (_objects.ContainsKey(code))
                        {
                            var ob = _objects[code];
                            DateTime time = _rec.GetTime("DateTime");
                            int quality = _rec.GetInt("QualityDetail");
                            var d = _rec.GetDouble("Value");
                            nread++;
                            if (ob.ValueSignal != null)
                            {
                                var err = MakeError(quality, ob);
                                switch (ob.DataType)
                                {
                                    case DataType.Boolean:
                                        nwrite += ob.ValueSignal.AddMom(time, d != 0, err);
                                        break;
                                    case DataType.Real:
                                        nwrite += ob.ValueSignal.AddMom(time, d, err);
                                        break;
                                    case DataType.Integer:
                                        nwrite += ob.ValueSignal.AddMom(time, Convert.ToInt32(d), err);
                                        break;
                                    default: //String
                                        nwrite += ob.ValueSignal.AddMom(time, _rec.GetString("vValue"), err);
                                        break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AddErrorObject(code, "Ошибка при чтении значений из рекордсета", ex);
                    }
                }
            return new Tuple<int, int>(nread, nwrite);
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
            ReadValuesByParts(_objects.Values, 500, BeginRead, EndRead, false);
        }
        #endregion
    }
}
