using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Text;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    [Export(typeof(IProvider))]
    [ExportMetadata("Code", "OvationSource")]
    public class OvationSource : SourceBase, ISource
    {
        //Код провайдера
        public override string Code { get { return "OvationSource"; } }
        //Настройки провайдера
        public string Inf
        {
            get { return ProviderInf; }
            set
            {
                ProviderInf = value;
                var dic = ProviderInf.ToPropertyDicS();
                dic.DefVal = "";
                _dataSource = dic["DataSource"];
                Hash = "OvationHistorian=" + _dataSource;
            }
        }
        
        //Имя дропа
        private string _dataSource;
        //Соединение с провайдером Historian
        private OleDbConnection _connection;

        //Открытие соединения
        protected override bool Connect()
        {
            Dispose();
            try
            {
                AddEvent("Соединение с Historian");
                _connection = new OleDbConnection("Provider=OvHOleDbProv.OvHOleDbProv.1;Persist Security Info=True;User ID='';Data Source=" + _dataSource + ";Location='';Mode=ReadWrite;Extended Properties=''");
                _connection.Open();
                return IsConnected = _connection.State == ConnectionState.Open;
            }
            catch (Exception ex)
            {
                AddError("Ошибка соединения с Historian", ex);
                return IsConnected = false;
            }
        }

        //Проверка соединения с провайдером, вызывается в настройках, или когда уже произошла ошибка для повторной проверки соединения
        public bool Check()
        {
            return Danger(Connect, 2, 500, "Не удалось соединиться с Historian");
        }

        //Проверка настроек
        public string CheckSettings(Dictionary<string, string> inf, Dictionary<string, string> names)
        {
            return "";
        }

        //Проверка соединения
        public bool CheckConnection()
        {
            if (Check())
            {
                CheckConnectionMessage = "Успешное соединение с Historian";
                return true;
            }
            AddError(CheckConnectionMessage = "Ошибка соединения с Historian");
            return false;
        }

        //Освобождение ресурсов, занятых провайдером
        public override void Dispose()
        {
            try { if (_rec != null) _rec.Dispose(); } catch { }
            try 
            { 
                _connection.Close();
                _connection.Dispose();
            }
            catch { }
        }
       
        //Словарь объектов по Id
        private readonly DicI<ObjectOvation> _objectsId = new DicI<ObjectOvation>();

        //Добавить сигнал
        public SourceSignal AddSignal(string signalInf, string code, DataType dataType, int idInClone)
        {
            var sig = new SignalOvation(signalInf, code, dataType, this, idInClone);
            var ob = _objectsId.Add(sig.Id, new ObjectOvation(sig.Id, code));
            if (sig.IsState) //Слово состояния
            {
                if (ob.StateSignal == null) ProviderSignals.Add(sig.Code, sig);
                return ob.StateSignal ?? (ob.StateSignal = sig);
            }
            if (ob.ValueSignal == null) ProviderSignals.Add(sig.Code, sig);
            return ob.ValueSignal ?? (ob.ValueSignal = sig);
        }

        //Удалить все сигналы
        public void ClearSignals()
        {
            ProviderSignals.Clear();
            _objectsId.Clear();
        }

        //Создание фабрики ошибок
        protected override IErrMomFactory MakeErrFactory()
        {
            var factory = new ErrMomFactory(Code, ErrMomType.Source);
            factory.AddGoodDescr(0);
            factory.AddDescr(1, "FAIR", ErrorQuality.Warning);
            factory.AddDescr(2, "POOR", ErrorQuality.Warning);
            factory.AddDescr(3, "BAD");
            factory.AddDescr(4, "Нет данных");
            return factory;
        }
        
        //Получение времени источника
        public TimeInterval GetTime()
        {
            TimeIntervals.Clear();
            var t = new TimeInterval(Different.MinDate.AddYears(1), DateTime.Now);
            TimeIntervals.Add(t);
            return t;
        }

        //Чтение значений
        #region
        //Рекордсет получения данных
        private IRecordRead _rec;
        
        //Формирование ошибки мгновенных значений по значению слова недостоверности
        private ErrMom MakeError(IContextable ob, IRecordRead rec)
        {
            //Недостоверность 8 и 9 бит, 00 - good, 01 - fair(имитация), 10 - poor(зашкал), 11 - bad
            if (rec.IsNull("STS") || (rec.IsNull("F_VALUE") && rec.IsNull("RAW_VALUE")))
                return MakeError(4, ob);//нет данных
            int state = rec.GetInt("STS");
            bool b8 = state.GetBit(8), b9 = state.GetBit(9);
            if (!b8 && !b9) return null;
            if (!b8) return MakeError(1, ob);
            if (!b9) return MakeError(2, ob);
            return MakeError(3, ob);
        }
        
        //Получает значение из текущей строчки рекордсета
        private double RMean(IRecordRead rec)
        {
            return rec.GetDouble("F_VALUE", IMean(rec));
        }
        private int IMean(IRecordRead rec)
        {
            return rec.GetInt("RAW_VALUE");
        }

        //Получает время из текущей строчки рекордсета
        private DateTime Time(IRecordRead rec)
        {
            var time = rec.GetTime("TIMESTAMP");
            time = time.AddMilliseconds(rec.GetInt("TIME_NSEC") / 1000000.0);
            return time.ToLocalTime();
        }

        //Запрос значений из Historian по списку сигналов и интервалу
        protected override bool QueryPartValues(List<SourceObject> part, DateTime beg, DateTime en, bool isCut)
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
            _rec = new ReaderAdo(_connection, sb.ToString());
            if (en.Subtract(beg).TotalMinutes > 59 && (_rec == null || !_rec.HasRows))
            {
                AddWarning("Значения из источника не получены", null, beg + " - " + en +"; " + part.First().Inf + " и др.");
                return IsConnected = false;
            }
            return true;
        }

        //Считывает значения из рекордсета _rec
        //Возвращает количество прочитанных значений и сформированных значений
        protected override Tuple<int, int> ReadPartValues(bool isCut)
        {
            int nread = 0, nwrite = 0;
            using (_rec)
                while (_rec.Read())
                {
                    ObjectOvation ob = null;
                    try
                    {
                        nread++;
                        var id = _rec.GetInt("Id");
                        if (_objectsId.ContainsKey(id))
                        {
                            ob = _objectsId[id];
                            DateTime time = Time(_rec);
                            if (ob.StateSignal != null)
                                nwrite += ob.StateSignal.AddMom(time, _rec.GetInt("STS"), null, isCut);
                            if (ob.ValueSignal != null)
                            {
                                var mom = Mom.Create(ob.ValueSignal.DataType, time, RMean(_rec), MakeError(ob, _rec));
                                nwrite += ob.ValueSignal.AddMom(mom, isCut);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AddErrorObject(ob == null ? "" : ob.Inf, "Ошибка при чтении значений из рекордсета", ex);
                    }
                }
            return new Tuple<int, int>(nread, nwrite);
        }

        //Задание нестандартных свойств получения данных
        protected override void SetReadProperties()
        {
            MaxErrorCount = 10;
            MaxErrorDepth = 10;
        }

        //Чтение среза
        protected override void ReadCut()
        {
            using (Start(0, 50)) //Срез по 4 минутам
                ReadValuesByParts(_objectsId.Values, 200, BeginRead.AddMinutes(-4), BeginRead, true);
            using (Start(50, 100)) //Срез по 61 минуте
                ReadValuesByParts(_objectsId.Values, 200, BeginRead.AddMinutes(-61), BeginRead.AddMinutes(-4), true);
        }

        //Чтение изменений
        protected override void ReadChanges()
        {
            ReadValuesByParts(_objectsId.Values, 200, BeginRead, EndRead, false);
        }
        #endregion
    }
}
