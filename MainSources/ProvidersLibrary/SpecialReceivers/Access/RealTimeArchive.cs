using BaseLibrary;
using CommonTypes;

namespace ProvidersLibrary
{
    //Архив значений реального времени в формате клона
    public class RealTimeArchive : RealTimeAccessReceiver
    {
        //Код провайдера
        public override string Code { get { return "RealTimeArchive"; } }
        //Тип сигнала
        public override SignalType SignalType { get { return SignalType.Receiver;} }

        //Числовые и строковые выходы
        private readonly DicS<RealTimeArchiveOut> _outs = new DicS<RealTimeArchiveOut>();

        //Очистка списка выходов
        protected internal override void ClearOuts()
        {
            _outs.Clear();
        }
        //Добавить выход
        protected override ProviderOut AddOut(ProviderSignal sig)
        {
            var rout = new RealTimeArchiveOut(this);
            rout.AddSignal(sig);
            return _outs.Add(sig.Code, rout);
        }

        //Подготовка провайдера
        protected override void PrepareProvider()
        {
            var set = new SetS();
            using (var rec = new DaoRec(Db, "Signals"))
            {
                while (rec.Read())
                {
                    var code = rec.GetString("FillCode");
                    var isContains = _outs.ContainsKey(code);
                    rec.Put("IsActive", isContains);
                    if (isContains)
                    {
                        set.Add(code);
                        _outs[code].Id = rec.GetInt("SignalId");
                    }
                }
                foreach (var k in _outs.Keys)
                    if (!set.Contains(k))
                    {
                        rec.AddNew();
                        var o = _outs[k].ValueSignal;
                        rec.Put("FullCode", o.Code);
                        rec.Put("DataType", o.DataType.ToRussian());
                        rec.Put("SignalType", SignalType.List.ToRussian());
                        _outs[k].Id = rec.GetInt("SignalId");
                    }
            }
            CloneRec = new DaoRec(Db, "MomentValues");
            CloneStrRec = new DaoRec(Db, "MomentStrValues");
        }

        //Рекордсеты таблиц архива
        internal DaoRec CloneRec { get; private set; }
        internal DaoRec CloneStrRec { get; private set; }

        //Запись значений
        protected internal override void WriteValues()
        {
            foreach (var o in _outs.Values)
                o.ValueToRec();
        }

        //Закрытие соединения
        protected override void DisconnectProvider()
        {
            if (CloneRec != null)
                CloneRec.Dispose();
            if (CloneStrRec != null)
                CloneStrRec.Dispose();
            if (Db != null)
            {
                Db.Dispose();
                Db = null;
            }
        }
    }
}