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
        public string Inf
        {
            get { return ProviderInf; }
            set
            {
                ProviderInf = value;
                var dic = ProviderInf.ToPropertyDicS();
                _retroServerName = dic["RetroServerName"] ?? "";
                Hash = "RetroServer=" + _retroServerName;
            }
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
        public TimeInterval GetTime()
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
        public SourceSignal AddSignal(string signalInf, string code, DataType dataType, int idInClone = 0)
        {
            var sig = new SourceSignal(signalInf, code, dataType, this, idInClone);
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

        //Рекордсет, запрашиваемый из архива
        private IRecordRead _rec;
        //Производится считывание аналоговых сигналов
        private bool _isAnalog;

        //Определяет размер блока для считывания, исходя из длины периода
        private int PartSize()
        {
            double len = EndRead.Subtract(BeginRead).TotalHours;
            int res = 3000;
            if (len > 0.0001) res = Math.Min(3000, Convert.ToInt32(2500 / len));
            if (res == 0) res = 1;
            return res;
        }

         //Запрос значений по одному блоку сигналов
        protected override bool QueryPartValues(List<SourceObject> part, DateTime beg, DateTime en, bool isCut)
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
            _rec = isCut 
                ? new ReaderAdo(_connection, _isAnalog ? "Exec ST_ANALOG ?, ?" : "Exec ST_OUT ?, ?", parBeginTime, parSysNums) 
                : new ReaderAdo(_connection, _isAnalog ? "Exec RT_ANALOGREAD ? , ? , ?" : "Exec RT_EXTREAD ? , ? , ?", parBeginTime, parEndTime, parSysNums);
            
            if (isCut && !_rec.HasRows)
            {
                AddWarning("Значения из источника не получены", null, part[0].Inf + " и др.");
                return IsConnected = false;
            }
            return true;
        }

        //Чтение срезов по одному блоку аналоговых сигналов
        protected override Tuple<int, int> ReadPartValues(bool isCut)
        {
            using (_rec)
            {
                if (_isAnalog) return ProcessAnalog(isCut);
                return ProcessOuts(isCut);
            }
        }

        //Получение значений выходов из рекорсета rec, который вернула комманда, 
        //Tсли time != null, то такое время устанавливается для всех значений
        private Tuple<int, int> ProcessOuts(bool isCut)
        {
            int nread = 0, nwrite = 0;
            while (_rec.Read())
            {
                nread++;
                ObjectKosm ob = null;
                try
                {
                    var curSignal = new ObjectIndex
                        {
                            Sn = _rec.GetInt(0),
                            NumType = _rec.GetInt(1),
                            Appartment = _rec.GetInt(2),
                            Out = _rec.GetInt(6)
                        };

                    if (_outs.ContainsKey(curSignal))
                    {
                        DateTime time = _rec.GetTime(3);
                        ob = _outs[curSignal];
                        int ndint = _rec.GetInt(8);
                        var strValue = _rec.GetString(9);
                        uint iMean = 0;
                        double dMean = 0;
                        bool isInt = false;
                        if (strValue.IndexOf("0x", StringComparison.Ordinal) >= 0)
                        {
                            iMean = Convert.ToUInt32(strValue, 16);
                            isInt = true;
                        }
                        else dMean = Convert.ToDouble(strValue);

                        if (ob.StateSignal != null)
                            nwrite += ob.StateSignal.AddMom(time, ndint, null, isCut);
                        if (ob.PokSignal != null)
                            nwrite += ob.PokSignal.AddMom(time, _rec.GetInt(5), MakeError(ndint, ob.PokSignal), isCut);
                        if (ob.ValueSignal != null)
                            switch (ob.ValueSignal.DataType)
                            {
                                case DataType.Boolean:
                                    nwrite += ob.ValueSignal.AddMom(time, isInt ? (iMean > 0) : (dMean > 0), MakeError(ndint, ob), isCut);
                                    break;
                                case DataType.Integer:
                                    nwrite += ob.ValueSignal.AddMom(time, isInt ? (int)iMean : Convert.ToInt32(dMean), MakeError(ndint, ob), isCut);
                                    break;
                                case DataType.Real:
                                    nwrite += ob.ValueSignal.AddMom(time, isInt ? iMean : dMean, MakeError(ndint, ob), isCut);
                                    break;
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

        //Получение значений аналоговых сигналов из рекорсета rec, который вернула комманда, 
        //Если begtime != null, то такое время устанавливается для всех значений
        private Tuple<int, int> ProcessAnalog(bool isCut)
        {
            int nread = 0, nwrite = 0;
            while (_rec.Read())
            {
                nread++;
                var curInd = new ObjectIndex
                {
                    Sn = _rec.GetInt(0),
                    NumType = _rec.GetInt(1),
                    Appartment = _rec.GetInt(2),
                    Out = 1
                };

                if (_analogs.ContainsKey(curInd))
                {
                    DateTime time = _rec.GetTime(3);
                    ObjectKosm ob = _analogs[curInd];
                    int ndint = _rec.GetInt(6);

                    if (ob.ValueSignal != null && ob.ValueSignal.DataType == DataType.Real)
                        nwrite += ob.ValueSignal.AddMom(time, _rec.GetInt(8), MakeError(ndint, ob), isCut);
                    if (ob.StateSignal != null)
                        nwrite += ob.StateSignal.AddMom(time, ndint, null, isCut);
                    if (ob.PokSignal != null)
                        nwrite += ob.PokSignal.AddMom(time, _rec.GetInt(5), null, isCut);    
                }
            }
            return new Tuple<int, int>(nread, nwrite);
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
                ReadValuesByParts(_analogs.Values, PartSize(), BeginRead, BeginRead, true, "Срез данных по аналоговым сигналам");

            _isAnalog = false;
            using (Start(AnalogsProcent(), 100))
                ReadValuesByParts(_outs.Values, PartSize(), BeginRead, BeginRead, true, "Срез данных по выходам");
        }

        //Чтение изменений
        protected override void ReadChanges()
        {
            _isAnalog = true;
            using (Start(0, AnalogsProcent()))
                ReadValuesByParts(_analogs.Values, PartSize(), BeginRead, EndRead, false, "Изменения значений по аналоговым сигналам");

            _isAnalog = false;
            using (Start(AnalogsProcent(), 100))
                ReadValuesByParts(_outs.Values, PartSize(), BeginRead, EndRead, false, "Изменения значений по выходам");
        }
        #endregion
    }
}