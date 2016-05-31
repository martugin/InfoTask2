using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.OleDb;
using System.Text;
using System.Threading;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    [Export(typeof(IProvider))]
    [ExportMetadata("Code", "SimaticSource")]
    public class SimaticSource : SourceBase, ISource 
    {
        //Код провайдера
        public override string Code { get { return "SimaticSource"; } }

        //Настройки провайдера
        protected override void GetInfDicS(DicS<string> dic)
        {
            _mainArchive = new SimaticArchive(this, dic["SQLServer"], false);
            _reserveArchive = new SimaticArchive(this, dic["SQLServerReserve"], true);
            Hash = _mainArchive.Hash + ";" + _reserveArchive.Hash;
        }

        //Соединение с архивами
        #region
        //Путь к SimaticCommData
        private readonly string _commDataFile = DifferentIT.GetInfoTaskDir() + @"Providers\Siemens\SimaticCommData.accdb";

        //Основная и дублирующая базы данных
        private SimaticArchive _mainArchive;
        private SimaticArchive _reserveArchive;
        
        //Открытие соединения
        protected override bool Connect()
        {
            using (var sys = new SysTabl(_commDataFile))
            {
                _mainArchive.SuccessTime = sys.SubValue("SourceInfo", "MainArchiveSuccessTime").ToDateTime();
                _reserveArchive.SuccessTime = sys.SubValue("SourceInfo", "ReserveArchiveSuccessTime").ToDateTime();
            }
            var archives = new SimaticArchive[2];
            int b = _reserveArchive.SuccessTime > _mainArchive.SuccessTime ? 0 : 1;
            archives[b] = _reserveArchive;
            archives[1-b] = _mainArchive;
            for (int iter = 1; iter <= 2; iter++)
            {
                if (IsConnected) Disconnect();
                foreach (var ar in archives)
                {
                    AddEvent((iter == 1 ? "Соединение" : "Повторное соединение") + " с архивом", ar.IsReserve ? "Резервный" : "Основной");
                    var con = ar.Connnect();
                    if (con != null && con.State == ConnectionState.Open)
                    {
                        _conn = con;
                        SysTabl.PutSubValueS(_commDataFile, "SourceInfo", (ar.IsReserve ? "Reserve" : "Main") + "ArchiveSuccessTime", DateTime.Now.ToString());
                        return IsConnected = true;
                    }    
                }
                Thread.Sleep(30);
            }
            AddError("Не удалось соединиться ни с основным, не с резервным сервером архива");
            return IsConnected = false;
        }

        //Закрытие соединений
        private void Disconnect()
        {
            if (_mainArchive!= null)
                _mainArchive.Disconnect();
            if (_reserveArchive != null)
                _reserveArchive.Disconnect();
            IsConnected = false;
        }

        //Проверка соединения
        public bool Check()
        {
            return Connect();
        }

        //Проверка настроек
        public string CheckSettings(Dictionary<string, string> inf, Dictionary<string, string> names)
        {
            string err = "";
            if (!inf.ContainsKey("SQLServer") || inf["SQLServer"].IsEmpty()) 
                err += "Не указан основной архивный сервер" + Environment.NewLine;
            return err;
        }

        //Проверка соединения
        public bool CheckConnection()
        {
            CheckConnectionMessage = "";
            bool bres = _mainArchive.CheckConnection();
            CheckConnectionMessage += Environment.NewLine;
            return bres | _reserveArchive.CheckConnection();
        }

        //Закрыть все соединения
        public override void Dispose()
        {
            Disconnect();
        }
        #endregion

        //Словари сигналов, ключи полные коды и Id
        private readonly Dictionary<int, ObjectSimatic> _objectsId = new Dictionary<int, ObjectSimatic>();

        //Добавить сигнал в провайдер
        public SourceSignal AddSignal(string signalInf, string code, DataType dataType, bool skipRepeats, int idInClone = 0)
        {
            var sig = new SignalSimatic(signalInf, code, dataType, this, skipRepeats, idInClone);
            if (!_objectsId.ContainsKey(sig.Id))
            {
                var ob = new ObjectSimatic(sig);
                _objectsId.Add(sig.Id, ob);
                ProviderSignals.Add(sig.Code, sig);
                return sig;
            }
            var addsig = _objectsId[sig.Id].AddSignal(sig);
            return ProviderSignals.Add(addsig.Code, addsig);
        }

        //Очистка списка сигналов
        public void ClearSignals()
        {
            _objectsId.Clear();
            ProviderSignals.Clear();
        }

        //Создание фабрики ошибок
        protected override IErrMomFactory MakeErrFactory()
        {
            var factory = new ErrMomFactory(Code, ErrMomType.Source) {UndefinedErrorText = "Ошибка"};
            factory.AddGoodDescr(128);
            return factory;
        }

        //Чтение значений
        #region
        //Соединение, из которого считываются данные
        private OleDbConnection _conn;

        //Запрос значений по одному блоку сигналов
        protected override bool QueryPartValues(List<SourceObject> part, DateTime beg, DateTime en)
        {
            var sb = new StringBuilder("TAG:R, ");
            if (part.Count == 1)
                sb.Append(((ObjectSimatic)part[0]).Id);
            else
            {
                sb.Append("(").Append(((ObjectSimatic) part[0]).Id);
                for (int i = 1; i < part.Count; i++)
                    sb.Append(";").Append(((ObjectSimatic) part[i]).Id);
                sb.Append(";-2)");
            }
            sb.Append(", ").Append(beg.ToSimaticString());
            sb.Append(", ").Append(en.ToSimaticString());
            
            AddEvent("Запрос значений из архива", part.Count + " тегов");
            Rec = new ReaderAdo(_conn, sb.ToString());
            return true;
        }

        //Определение текущего считываемого объекта
        protected override SourceObject DefineObject()
        {
            int id = Rec.GetInt(0);
            if (_objectsId.ContainsKey(id))
                return _objectsId[id];
            return null;
        }

        //Чтение значений по одному объекту из рекордсета источника
        //Возвращает количество сформированных значений
        protected override int ReadObjectValue(SourceObject obj)
        {
            var ob = (ObjectSimatic)obj;
            DateTime time = Rec.GetTime(1).ToLocalTime();
            var quality = Rec.GetInt(3);
            var err = MakeError(quality, ob);

            return AddMom(ob.SignalFlags, time, Rec.GetInt(4), err) +
                      AddMom(ob.SignalQuality, time, quality, err) +
                      AddMom(ob.SignalValue, time, ((ReaderAdo)Rec).Reader[2], err);
        }
        
        //Чтение среза
        protected override void ReadCut()
        {
            ReadValuesByParts(_objectsId.Values, 500, PeriodBegin.AddSeconds(-60), PeriodBegin, true);
        }
        //Чтение изменений
        protected override void ReadChanges()
        {
            ReadValuesByParts(_objectsId.Values, 500, PeriodBegin, PeriodEnd, false);
        }
        #endregion
    }
}

