using System.Collections.Generic;
using System.ComponentModel.Composition;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Provider
{
    [Export(typeof(ProvConn))]
    [ExportMetadata("Complect", "KosmotronikaConn")]
    //Соединение-источник космотроники
    public class KosmotronikaConn : SourConn
    {
        //Комплект провайдеров
        public override string Complect { get { return "Kosmotronika"; }}

        //Словарь объектов. Один элемент словаря - один выход, для выхода список битов
        private readonly Dictionary<ObjectIndex, ObjectKosm> _outs = new Dictionary<ObjectIndex, ObjectKosm>();
        internal Dictionary<ObjectIndex, ObjectKosm> Outs { get { return _outs; } }
        //Словарь аналоговых объектов
        private readonly Dictionary<ObjectIndex, ObjectKosm> _analogs = new Dictionary<ObjectIndex, ObjectKosm>();
        internal Dictionary<ObjectIndex, ObjectKosm> Analogs { get { return _analogs; } }

        //Очистка списка сигналов
        public override void ClearSignals()
        {
            base.ClearSignals();
            _outs.Clear();
            _analogs.Clear();
        }

        //Добавляет один сигнал в список
        protected override SourceObject AddObject(SourceInitSignal sig)
        {
            var ind = new ObjectIndex
            {
                Sn = sig.Inf.GetInt("SysNum"),
                NumType = sig.Inf.GetInt("NumType"),
                Appartment = sig.Inf.GetInt("Appartment"),
                Out = sig.Inf.GetInt("NumOut")
            };
            ObjectKosm obj;
            if (ind.Out == 1 && (ind.NumType == 1 || ind.NumType == 3 || ind.NumType == 32))
            {
                if (_analogs.ContainsKey(ind)) obj = _analogs[ind];
                else _analogs.Add(ind, obj = new ObjectKosm(this, ind));
            }
            else
            {
                if (_outs.ContainsKey(ind)) obj = _outs[ind];
                else _outs.Add(ind, obj = new ObjectKosm(this, ind));
            }
            return obj;
        }

        //Создание фабрики ошибок
        protected override IErrMomFactory MakeErrFactory()
        {
            return new ErrMomFactoryKosm();
        }
    }
}