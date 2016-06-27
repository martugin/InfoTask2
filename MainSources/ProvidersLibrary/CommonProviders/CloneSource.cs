using System;
using System.Collections.Generic;
using BaseLibrary;

namespace ProvidersLibrary
{
    //Источник, читающий из клона
    public class CloneSource : AdoSource
    {
        public CloneSource(string complect)
        {
            _complect = complect;
        }

        //Подключение 
        internal CloneSourceConnect CloneConnect { get { return (CloneSourceConnect) CurConnect; } }

        //Код провайдера
        private readonly string _complect;
        public override string Complect { get { return _complect; } }

        public override string Code { get { return "CloneSource"; } }

        //Настройка свойств получения данных
        protected override void SetReadProperties()
        {
            ReconnectsCount = 1;
            MaxErrorCount = 1;
            MaxErrorDepth = 1;
        }

        //Словарь объектов, каждый содержит один сигнал, ключи - SignalId в клоне
        private readonly DicI<SourceObject> _objectsId = new DicI<SourceObject>();
        //Список объектов
        private readonly List<SourceObject> _objects = new List<SourceObject>();
        
        //Подготовка клона к чтению значений
        protected override ProviderConnect CreateConnect()
        {
            return new CloneSourceConnect {Complect = Complect};
        }

        public override void Prepare()
        {
            using (var rec = new RecDao(CloneConnect.CloneFile, "SELECT SignalId, FullCode, Otm FROM Signals"))
                while (rec.Read())
                {
                    string code = rec.GetString("FullCode");
                    var id = rec.GetInt("SignalId");
                    if (ProviderSignals.ContainsKey(code))
                    {
                        ProviderSignals[code].IdInClone = id;
                        rec.Put("Otm", true);
                        var ob = new CloneObject(this);
                        _objectsId.Add(id, ob);
                        _objects.Add(ob);
                    }
                    else rec.Put("Otm", false);
                }
        }

        //Читать из таблицы строковых значений
        private bool _isStrTable;

        //Чтение среза
        protected override SourceObject AddObject(SourceInitSignal sig)
        {
            throw new NotImplementedException();
        }

        protected override void ReadCut()
        {
            DateTime d = RemoveMinultes(PeriodBegin);
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
        protected override IRecordRead QueryPartValues(List<SourceObject> part, DateTime beg, DateTime en, bool isCut)
        {
            string table = "Moment" + (_isStrTable ? "Str" : "") + "Values" + (isCut ? "Cut" : "");
            string timeField = (isCut ? "Cut" : "") + "Time";
            return new RecDao(CloneConnect.CloneFile, "SELECT " + table + ".* FROM Signals INNER JOIN " + table + " ON Signals.SignalId=" + table + ".SignalId" +
                                                             " WHERE (Signals.Otm=True) AND (" + table + "." + timeField + ">=" + beg.ToAccessString() + ") AND (" + table + "." + timeField + "<=" + en.ToAccessString() + ")");
        }

        //Определение объекта строки значений
        protected override SourceObject DefineObject(IRecordRead rec)
        {
            return _objectsId[rec.GetInt("SignalId")];
        }
    }
}