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

        //Чтение настроек провайдера
        protected override void ReadInf(DicS<string> dic)
        {
            CloneFile = dic["CloneDir"].EndDir() + "Clone.accdb";
        }

        //Проверка соединения
        protected override void ConnectProvider()
        {
            if (!DaoDb.Check(CloneFile, "InfoTaskClone"))
                AddError("Недопустимый файл клона", null , CloneFile);
            if (SysTabl.ValueS(CloneFile, "CloneComplect") != SourceConnect.Complect)
                AddError(CheckConnectionMessage = "Файл является клоном для другого, несовместимого источника");
        }
        
        protected internal override string CheckSettings(DicS<string> infDic)
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
        internal readonly DicI<CloneOut> ObjectsId = new DicI<CloneOut>();
        internal readonly DicS<CloneOut> Objects = new DicS<CloneOut>();
        //Список объектов
        internal readonly List<SourceOut> ObjectsList = new List<SourceOut>();

        //Добавляет объект, содержащий один сигнал
        protected override SourceOut AddOut(InitialSignal sig)
        {
            return Objects.Add(sig.Code, new CloneOut(this));
        }

        //Очистка списка объектов
        protected internal override void ClearOuts()
        {
            ObjectsId.Clear();
            Objects.Clear();
            ObjectsList.Clear();
        }

        //Создание фабрики ошибок
        protected override IMomErrFactory MakeErrFactory()
        {
            AddEvent("Создание фабрики ошибок");
            var factory = new MomErrFactory(ProviderConnect.Name, MomErrType.Source);
            using (var rec = new DaoRec(CloneFile, "MomentErrors"))
                while (rec.Read())
                {
                    int quality = rec.GetInt("Quality");
                    int num = rec.GetInt("ErrNum");
                    string text = rec.GetString("ErrText");
                    factory.AddDescr(num, text, quality);
                }
            return factory;
        }

        //Отметка в клоне считывемых сигналов, получение Id сигналов
        protected override void PrepareProvider()
        {
            AddEvent("Отметка в клоне считывемых сигналов");
            using (var rec = new DaoRec(CloneFile, "SELECT SignalId, FullCode, OtmReadClone FROM Signals"))
                while (rec.Read())
                {
                    string code = rec.GetString("FullCode");
                    var id = rec.GetInt("SignalId");
                    if (Objects.ContainsKey(code))
                    {
                        rec.Put("OtmReadClone", true);
                        var ob = Objects[code];
                        ObjectsId.Add(id, ob);
                        ObjectsList.Add(ob);
                    }
                    else rec.Put("OtmReadClone", false);
                }
        }

        //Читать из таблицы строковых значений
        private bool _useStrTable;
        //Читать из таблицы срезов
        private bool _useCutTable;

        //Запрос значений из клона
        protected override IRecordRead QueryValues(IList<SourceOut> part, DateTime beg, DateTime en, bool isCut)
        {
            string table = "Moment" + (_useStrTable ? "Str" : "") + "Values" + (_useCutTable ? "Cut" : "");
            string timeField = (_useCutTable ? "Cut" : "") + "Time";
            var stSql = "SELECT " + table + ".* FROM Signals INNER JOIN " + table + " ON Signals.SignalId=" + table + ".SignalId" +
                        " WHERE (Signals.OtmReadClone=True) AND (" + table + "." + timeField + ">=" + beg.ToAccessString() + ") AND (" + table + "." + timeField + "<=" + en.ToAccessString() + ")";
            return new DaoRec(CloneFile, stSql);
        }

        //Определение объекта строки значений
        protected override SourceOut DefineOut(IRecordRead rec)
        {
            return ObjectsId[rec.GetInt("SignalId")];
        }
        
        //Чтение среза
        protected internal override ValuesCount ReadCut()
        {
            var vc = new ValuesCount();
            DateTime d = SourceConnect.RemoveMinultes(PeriodBegin);
            AddEvent("Чтение среза действительных значений из таблицы изменений");
            _useStrTable = false;
            _useCutTable = false;
            vc += ReadWhole(ObjectsList, d, PeriodBegin, true);
            if (vc.IsFail) return vc;
            AddEvent("Чтение среза действительных значений из таблицы срезов");
            _useStrTable = false;
            _useCutTable = true;
            vc += ReadWhole(ObjectsList, d.AddSeconds(-1), PeriodBegin.AddSeconds(1), true);
            if (vc.IsFail) return vc;
            AddEvent("Чтение среза строковых значений из таблицы изменений");
            _useStrTable = true;
            _useCutTable = false;
            vc += ReadWhole(ObjectsList, d, PeriodBegin, true);
            if (vc.IsFail) return vc;
            AddEvent("Чтение среза строковых значений из таблицы срезов");
            _useStrTable = true;
            _useCutTable = true;
            vc += ReadWhole(ObjectsList, d.AddSeconds(-1), PeriodBegin.AddSeconds(1), true);
            return vc;
        }

        //Чтение изменений
        protected internal override ValuesCount ReadChanges()
        {
            var vc = new ValuesCount();
            AddEvent("Чтение изменений действительных значений");
            _useStrTable = false;
            _useCutTable = false;
            vc += ReadWhole(ObjectsList);
            if (vc.IsFail) return vc;
            AddEvent("Чтение изменений строковых значений");
            _useStrTable = true;
            _useCutTable = false;
            vc += ReadWhole(ObjectsList);
            return vc;
        }
    }
}