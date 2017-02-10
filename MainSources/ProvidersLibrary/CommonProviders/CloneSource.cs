using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Источник, читающий из клона
    public class CloneSource : AdoSource
    {
        //Код провайдера
        public override string Code { get { return "CloneSource"; } }

        //Файл клона
        internal string CloneFile { get; private set; }
        //Кэш для идентификации соединения
        protected override string Hash { get { return "Db=" + CloneFile; } }

        //Чтение настроек провайдера
        protected override void ReadInf(DicS<string> dic)
        {
            var dir = dic["CloneDir"];
            if (!dir.EndsWith(@"\")) dir += @"\";
            CloneFile = dir + @"Clone.accdb";
        }

        //Проверка соединения
        protected override bool ConnectProvider()
        {
            return DaoDb.Check(CloneFile, "InfoTaskClone");
        }

        //Проверка соединения
        protected override bool CheckConnection()
        {
            if (!Reconnect())
            {
                AddError(CheckConnectionMessage = "Файл не найден или не является файлом клона");
                return false;
            }
            if (SysTabl.ValueS(CloneFile, "CloneComplect") != SourceConnect.Complect)
            {
                AddError(CheckConnectionMessage = "Файл является клоном для другого, несовместимого источника");
                return false;
            }
            CheckConnectionMessage = "Успешное соединение";
            return true;
        }

        internal protected override string CheckSettings(DicS<string> infDic)
        {
            if (infDic["CloneDir"].IsEmpty())
                return "Не указан каталог клона";
            return "";
        }

        protected override void AddMenuCommands()
        {
            throw new NotImplementedException();
        }

        protected override TimeInterval GetTimeSource()
        {
            using (var sys = new SysTabl(CloneFile, false))
                return new TimeInterval(sys.Value("BeginInterval").ToDateTime(), sys.Value("EndInterval").ToDateTime());
        }

        //Словари объектов, каждый содержит один сигнал, ключи - SignalId в клоне и коды
        private readonly DicI<CloneOut> _objectsId = new DicI<CloneOut>();
        private readonly DicS<CloneOut> _objects = new DicS<CloneOut>();
        //Список объектов
        private readonly List<SourceOut> _objectsList = new List<SourceOut>();

        //Добавляет объект, содержащий один сигнал
        protected override SourceOut AddOut(InitialSignal sig)
        {
            return _objects.Add(sig.Code, new CloneOut(this));
        }

        //Очистка списка объектов
        protected override void ClearOuts()
        {
            _objectsId.Clear();
            _objects.Clear();
            _objectsList.Clear();
        }

        //Создание фабрики ошибок
        protected override IErrMomFactory MakeErrFactory()
        {
            AddEvent("Создание фабрики ошибок");
            var factory = new ErrMomFactory(ProviderConnect.Name, ErrMomType.Source);
            using (var rec = new RecDao(CloneFile, "MomentErrors"))
                while (rec.Read())
                {
                    int quality = rec.GetInt("Quality");
                    int num = rec.GetInt("NumError");
                    string text = rec.GetString("TextError");
                    factory.AddDescr(num, text, quality);
                }
            return factory;
        }

        //Отметка в клоне считывемых сигналов, получение Id сигналов
        protected override void PrepareSource()
        {
            AddEvent("Отметка в клоне считывемых сигналов");
            using (var rec = new RecDao(CloneFile, "SELECT SignalId, FullCode, Otm FROM Signals"))
                while (rec.Read())
                {
                    string code = rec.GetString("FullCode");
                    var id = rec.GetInt("SignalId");
                    if (_objects.ContainsKey(code))
                    {
                        rec.Put("Otm", true);
                        var ob = _objects[code];
                        _objectsId.Add(id, ob);
                        _objectsList.Add(ob);
                    }
                    else rec.Put("Otm", false);
                }
        }

        //Читать из таблицы строковых значений
        private bool _isStrTable;

        //Запрос значений из клона
        protected override IRecordRead QueryValues(IList<SourceOut> part, DateTime beg, DateTime en, bool isCut)
        {
            string table = "Moment" + (_isStrTable ? "Str" : "") + "Values" + (isCut ? "Cut" : "");
            string timeField = (isCut ? "Cut" : "") + "Time";
            return new RecDao(CloneFile, "SELECT " + table + ".* FROM Signals INNER JOIN " + table + " ON Signals.SignalId=" + table + ".SignalId" +
                                                             " WHERE (Signals.Otm=True) AND (" + table + "." + timeField + ">=" + beg.ToAccessString() + ") AND (" + table + "." + timeField + "<=" + en.ToAccessString() + ")");
        }

        //Определение объекта строки значений
        protected override SourceOut DefineOut(IRecordRead rec)
        {
            return _objectsId[rec.GetInt("SignalId")];
        }

        //Чтение среза
        internal protected override ValuesCount ReadCut()
        {
            var vc = new ValuesCount();
            DateTime d = SourceConnect.RemoveMinultes(PeriodBegin);
            AddEvent("Чтение среза действительных значений из таблицы изменений");
            _isStrTable = false;
            vc += ReadWhole(_objectsList, d, PeriodBegin, false);
            if (vc.IsFail) return vc;
            AddEvent("Чтение среза действительных значений из таблицы срезов");
            _isStrTable = false;
            vc += ReadWhole(_objectsList, d.AddSeconds(-1), d.AddSeconds(1), true);
            if (vc.IsFail) return vc;
            AddEvent("Чтение среза строковых значений из таблицы изменений");
            _isStrTable = true;
            vc += ReadWhole(_objectsList, d, PeriodBegin, false);
            if (vc.IsFail) return vc;
            AddEvent("Чтение среза строковых значений из таблицы срезов");
            _isStrTable = true;
            vc += ReadWhole(_objectsList, d.AddSeconds(-1), d.AddSeconds(1), true);
            return vc;
        }

        //Чтение изменений
        internal protected override ValuesCount ReadChanges()
        {
            var vc = new ValuesCount();
            AddEvent("Чтение изменений действительных значений");
            _isStrTable = false;
            vc += ReadWhole(_objectsList);
            if (vc.IsFail) return vc;
            AddEvent("Чтение изменений строковых значений");
            _isStrTable = true;
            vc += ReadWhole(_objectsList);
            return vc;
        }
    }
}