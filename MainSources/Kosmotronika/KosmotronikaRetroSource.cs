using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.OleDb;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    [Export(typeof(IProvider))]
    [ExportMetadata("Code", "KosmotronikaRetroSource")]
    public class KosmotronikaRetroSource : SourceBase, ISource
    {
        //Код провайдера
        public override string Code { get { return "KosmotronikaRetroSource"; } }
        
        //Настройки провайдера
        protected override void GetInfDicS(DicS<string> dic)
        {
            _retroServerName = dic["RetroServerName"];
            Hash = "RetroServer=" + _retroServerName;
        }
        
        //Имя ретросервера
        private string _retroServerName;
        //Соединение с провайдером
        private OleDbConnection _connection;

        //Открытие соединения
        protected override bool Connect()
        {
            Dispose();
            if (_retroServerName.IsEmpty())
                return IsConnected = false;
            try
            {
                AddEvent("Соединение с Ретро-сервером");
                _connection = new OleDbConnection("Provider=RetroDB.RetroSQL;Data Source=" + _retroServerName);
                _connection.Open();
                return IsConnected = _connection.State == ConnectionState.Open;
            }
            catch (Exception ex)
            {
                AddError("Ошибка соединения с Ретро-сервером", ex);
                return IsConnected = false;
            }
        }

        //Проверка соединения с провайдером, вызывается в настройках, или когда уже произошла ошибка для повторной проверки соединения
        public bool Check()
        {
            return Danger(Connect, 2, 500, "Не удалось определить временной диапазон Ретро-сервера");
        }

        //Проверка настроек
        public string CheckSettings(Dictionary<string, string> inf, Dictionary<string, string> names)
        {
            return !inf["RetroServerName"].IsEmpty() ? "" : "Не задано имя Ретро-сервера";
        }

        //Проверка соединения
        public bool CheckConnection()
        {
            if (Check() && GetTime() != null && TimeIntervals.Count > 0)
            {
                CheckConnectionMessage = "Успешное соединение. Диапазон источника: " + TimeIntervals[0].Begin + " - " + TimeIntervals[0].End;
                return true;
            }
            AddError(CheckConnectionMessage = "Ошибка соединения с Ретро-сервером");
            return false;
        }
        
        //Освобождение ресурсов, занятых провайдером
        public override void Dispose()
        {
            try
            {
                if (_connection != null)
                {
                    _connection.Close();
                    _connection.Dispose();    
                }
            } 
            catch { }
        }

        //Получение времени архива, True - если успешно
        public override TimeInterval GetTime()
        {
            if (!Danger(TryGetTime, 2, 500, "Не удалось определить временной диапазон Ретро-сервера")) return null;
            return new TimeInterval(BeginTime, EndTime);
        }

        private bool TryGetTime()
        {
            if (!IsConnected && !Connect()) return false;
            try
            {
                AddEvent("Определение диапазона источника");
                using (var rec = new ReaderAdo(_connection, "Exec RT_ARCHDATE"))
                {
                    BeginTime = rec.GetTime(0);
                    EndTime = rec.GetTime(1);
                    TimeIntervals.Clear();
                    TimeIntervals.Add(new TimeInterval(BeginTime, EndTime));
                    AddEvent("Диапазон источника определен", BeginTime + " - " + EndTime);
                    return BeginTime.ToString() != "0:00:00";
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка определения диапазона Ретро-сервера", ex);
                return IsConnected = false;
            }
        }

        //Список сигналов
        #region
        //Словарь объектов. Один элемент словаря - один выход, для выхода список битов
        private readonly Dictionary<ObjectIndex, ObjectKosm> _outs = new Dictionary<ObjectIndex, ObjectKosm>();
        //Словарь аналоговых объектов
        private readonly Dictionary<ObjectIndex, ObjectKosm> _analogs = new Dictionary<ObjectIndex, ObjectKosm>();
        
        //Очистка списка сигналов
        public void ClearSignals()
        {
            ProviderSignals.Clear();
            _outs.Clear();
            _analogs.Clear();
        }

        //Добавляет один сигнал в список
        public SourceSignal AddSignal(string signalInf, string code, DataType dataType, bool skipRepeats = true, int idInClone = 0)
        {
            var sig = new SourceSignal(signalInf, code, dataType, this, skipRepeats, idInClone);
            //Заполнение SignalsLists
            var ind=new ObjectIndex
                        {
                            Sn = sig.Inf.GetInt("SysNum"),
                            NumType = sig.Inf.GetInt("NumType"),
                            Appartment = sig.Inf.GetInt("Appartment"),
                            Out = sig.Inf.GetInt("NumOut")
                        };
            ObjectKosm obj;
            if (ind.Out == 1 && (ind.NumType == 1 || ind.NumType == 3 || ind.NumType == 32))
            {
                if (_analogs.ContainsKey(ind))  obj = _analogs[ind];
                else _analogs.Add(ind, obj = new ObjectKosm(ind, sig.Code));
            }
            else
            {
                if (_outs.ContainsKey(ind)) obj = _outs[ind];
                else _outs.Add(ind, obj = new ObjectKosm(ind, sig.Code));
            }
            var nsig = obj.AddSignal(sig);
            if (nsig == sig) ProviderSignals.Add(sig.Code, nsig);
            return nsig;
        }
        #endregion

        //Чтение данных из архива
        #region
        //Создание фабрики ошибок
        protected override IErrMomFactory MakeErrFactory()
        {
            return new ErrMomFactoryKosm();
        }

        //Производится считывание аналоговых сигналов
        private bool _isAnalog;

        //Определяет размер блока для считывания, исходя из длины периода
        private int PartSize()
        {
            double len = PeriodEnd.Subtract(PeriodBegin).TotalHours;
            int res = 3000;
            if (len > 0.0001) res = Math.Min(3000, Convert.ToInt32(2500 / len));
            if (res == 0) res = 1;
            return res;
        }

         //Запрос значений по одному блоку сигналов
        protected override bool QueryPartValues(List<SourceObject> part, DateTime beg, DateTime en)
        {
            var nums = new ushort[part.Count,_isAnalog ? 3 : 4];
            for (int i = 0; i < part.Count; i++)
            {
                var ob = (ObjectKosm) part[i];
                nums[i, 0] = (ushort) ob.Sn;
                nums[i, 1] = (ushort) ob.NumType;
                nums[i, 2] = (ushort) ob.Appartment;
                if (!_isAnalog) nums[i, 3] = (ushort) ob.Out;
            }

            var parSysNums = new OleDbParameter("Sysnums", OleDbType.Variant) {Value = nums};
            var parBeginTime = new OleDbParameter("BeginTime", OleDbType.DBTimeStamp) {Value = beg};
            var parEndTime = new OleDbParameter("EndTime", OleDbType.DBTimeStamp) {Value = en};
            Rec = IsCutReading 
                ? new ReaderAdo(_connection, _isAnalog ? "Exec ST_ANALOG ?, ?" : "Exec ST_OUT ?, ?", parBeginTime, parSysNums) 
                : new ReaderAdo(_connection, _isAnalog ? "Exec RT_ANALOGREAD ? , ? , ?" : "Exec RT_EXTREAD ? , ? , ?", parBeginTime, parEndTime, parSysNums);

            if (IsCutReading && !Rec.HasRows)
            {
                AddWarning("Значения из источника не получены", null, part[0].Inf + " и др.");
                return IsConnected = false;
            }
            return true;
        }

        //Определение текущего считываемого объекта
        protected override SourceObject DefineObject()
        {
            var ind = new ObjectIndex
            {
                Sn = Rec.GetInt(0),
                NumType = Rec.GetInt(1),
                Appartment = Rec.GetInt(2),
                Out = _isAnalog ? 1 : Rec.GetInt(6)
            };
            if (_isAnalog && _analogs.ContainsKey(ind))
                return _analogs[ind];
            if (_outs.ContainsKey(ind))
                return _outs[ind];
            return null;
        }

        //Чтение значений по одному объекту из рекордсета источника
        //Возвращает количество сформированных значений
        protected override int ReadObjectValue(SourceObject obj)
        {
            var ob = (ObjectKosm)obj;
            int nwrite = 0;
            DateTime time = Rec.GetTime(3);
            int ndint = Rec.GetInt(_isAnalog ? 6 : 8);
            var err = MakeError(ndint, ob);

            nwrite += AddMom(ob.PokSignal, time, Rec.GetInt(5), err);
            nwrite += AddMom(ob.StateSignal, time, ndint);
            if (_isAnalog)
                nwrite += AddMom(ob.ValueSignal, time, Rec.GetDouble(8), err);
            else
            {
                var strValue = Rec.GetString(9);    
                if (strValue.IndexOf("0x", StringComparison.Ordinal) >= 0)
                    nwrite += ob.ValueSignal.AddMom(time, Convert.ToUInt32(strValue, 16), err);
                else nwrite += ob.ValueSignal.AddMom(time, Convert.ToDouble(strValue), err);
            }
            return nwrite;
        }

        private double AnalogsProcent()
        {
            if (_outs.Count + _analogs.Count == 0) return 0;
            return _analogs.Count * 100.0  / (_outs.Count + _analogs.Count);
        }

        //Чтение среза
        protected override void ReadCut()
        {
            _isAnalog = true;
            using (Start(0, AnalogsProcent()))
                ReadValuesByParts(_analogs.Values, PartSize(), PeriodBegin, PeriodEnd, true, "Срез данных по аналоговым сигналам");

            _isAnalog = false;
            using (Start(AnalogsProcent(), 100))
                ReadValuesByParts(_outs.Values, PartSize(), PeriodBegin, PeriodEnd, true, "Срез данных по выходам");
        }

        //Чтение изменений
        protected override void ReadChanges()
        {
            _isAnalog = true;
            using (Start(0, AnalogsProcent()))
                ReadValuesByParts(_analogs.Values, PartSize(), PeriodBegin, PeriodEnd, false, "Изменения значений по аналоговым сигналам");

            _isAnalog = false;
            using (Start(AnalogsProcent(), 100))
                ReadValuesByParts(_outs.Values, PartSize(), PeriodBegin, PeriodEnd, false, "Изменения значений по выходам");
        }
        #endregion
    }
}