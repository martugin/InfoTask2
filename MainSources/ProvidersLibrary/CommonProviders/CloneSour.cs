using System;
using System.Collections.Generic;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Источник, читающий из клона
    public class CloneSour : AdoSour
    {
        //Код провайдера
        public override string Code { get { return "CloneSource"; } }

        //Файл клона
        private string _cloneFile;
        //Кэш для идентификации соединения
        public override string Hash { get { return "Db="+  _cloneFile; } }

        //Чтение настроек провайдера
        protected override void ReadInf(DicS<string> dic)
        {
            _cloneFile = dic["CloneDir"] + @"\Clone.accdb";
        }

        //Проверка соединения
        public override bool Check()
        {
            bool b = DaoDb.Check(_cloneFile, "InfoTaskClone");
            return b;
        }

        //Проверка соединения
        public override bool CheckConnection()
        {
            if (!Check())
            {
                AddError(CheckConnectionMessage = "Файл не найден или не является файлом клона");
                return false;    
            }
            if (SysTabl.ValueS(_cloneFile, "CloneComplect") != SourceConn.Complect)
            {
                AddError(CheckConnectionMessage = "Файл является клоном для другого несовместимого источника");
                return false;
            }
            CheckConnectionMessage = "Успешное соединение";
            return true;
        }

        public override string CheckSettings(DicS<string> infDic)
        {
            if (infDic["CloneDir"].IsEmpty())
                return "Не указан каталог клона";
            return "";
        }

        protected override void AddMenuCommands()
        {
            throw new NotImplementedException();
        }

        protected override TimeInterval GetSourceTime()
        {
            using (var sys = new SysTabl(_cloneFile, false))
                return new TimeInterval(sys.Value("BeginInterval").ToDateTime(), sys.Value("EndInterval").ToDateTime());
        }

        protected override void SetReadProperties()
        {
            ReconnectsCount = 1;
            MaxErrorCount = 1;
            MaxErrorDepth = 1;
        }

        //Словарь объектов, каждый содержит один сигнал, ключи - SignalId в клоне
        private readonly DicI<SourObject> _objectsId = new DicI<SourObject>();
        //Список объектов
        private readonly List<SourObject> _objects = new List<SourObject>();
        
        //Подготовка клона к чтению значений
        public override void Prepare()
        {
            using (var rec = new RecDao(_cloneFile, "SELECT SignalId, FullCode, Otm FROM Signals"))
                while (rec.Read())
                {
                    string code = rec.GetString("FullCode");
                    var id = rec.GetInt("SignalId");
                    if (SourceConn.ProviderSignals.ContainsKey(code))
                    {
                        SourceConn.ProviderSignals[code].IdInClone = id;
                        rec.Put("Otm", true);
                        var ob = new CloneObject(SourceConn);
                        _objectsId.Add(id, ob);
                        _objects.Add(ob);
                    }
                    else rec.Put("Otm", false);
                }
        }

        //Читать из таблицы строковых значений
        private bool _isStrTable;

        //Чтение среза
        protected override void ReadCut()
        {
            DateTime d = SourceConn.RemoveMinultes(PeriodBegin);
            AddEvent("Чтение среза действительных значений из таблицы изменений");
            _isStrTable = false;
            ReadPart(_objects, d, PeriodBegin, false);
            AddEvent("Чтение среза действительных значений из таблицы срезов");
            _isStrTable = false;
            ReadPart(_objects, d.AddSeconds(-1), d.AddSeconds(1), true);
            AddEvent("Чтение среза строковых значений из таблицы изменений");
            _isStrTable = true;
            ReadPart(_objects, d, PeriodBegin, false);
            AddEvent("Чтение среза строковых значений из таблицы срезов");
            _isStrTable = true;
            ReadPart(_objects, d.AddSeconds(-1), d.AddSeconds(1), true);
        }

        //Чтение изменений
        protected override void ReadChanges()
        {
            AddEvent("Чтение изменений действительных значений");
            _isStrTable = false;
            ReadPart(_objects, PeriodBegin, PeriodEnd, false);
            AddEvent("Чтение изменений строковых значений");
            _isStrTable = true;
            ReadPart(_objects, PeriodBegin, PeriodEnd, false);
        }

        //Запрос значений из клона
        protected override IRecordRead QueryPartValues(List<SourObject> part, DateTime beg, DateTime en, bool isCut)
        {
            string table = "Moment" + (_isStrTable ? "Str" : "") + "Values" + (isCut ? "Cut" : "");
            string timeField = (isCut ? "Cut" : "") + "Time";
            return new RecDao(_cloneFile, "SELECT " + table + ".* FROM Signals INNER JOIN " + table + " ON Signals.SignalId=" + table + ".SignalId" +
                                                             " WHERE (Signals.Otm=True) AND (" + table + "." + timeField + ">=" + beg.ToAccessString() + ") AND (" + table + "." + timeField + "<=" + en.ToAccessString() + ")");
        }

        //Определение объекта строки значений
        protected override SourObject DefineObject(IRecordRead rec)
        {
            return _objectsId[rec.GetInt("SignalId")];
        }
    }
}