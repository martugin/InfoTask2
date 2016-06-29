using System.ComponentModel.Composition;
using BaseLibrary;
using CommonTypes;
using ProvidersLibrary;

namespace Fictive
{
    //Фиктивный тестовый источник, реализация без чтения по блокам и OleDb
    [Export(typeof(ProviderBase))]
    [ExportMetadata("Code", "FictiveSource")]
    public class FictiveSource : SourceBase
    {
        public FictiveSource()
        {
            NeedCut = false;
        }

        //Код
        public override string Code { get { return "FictiveSource"; } }
        //Комплект
        public override string Complect { get { return "Fictive"; } }
        //Создание подключения
        protected override ProviderConnect CreateConnect()
        {
            return new FictiveConnect();
        }
        //Ссылка на подключение
        internal FictiveConnect Connect { get { return (FictiveConnect)CurConnect; } }

        //Словарь объектов, ключи - номера
        private readonly DicI<ObjectFictive> _objects = new DicI<ObjectFictive>();

        //Добавить объект по заданному сигналу
        protected override SourceObject AddObject(SourceSignal sig)
        {
            var num = sig.Inf.GetInt("NumObject");
            if (!_objects.ContainsKey(num))
                return _objects.Add(num, new ObjectFictive(this, sig.Inf.GetInt("ValuesInterval")));
            return _objects[num];
        }

        //Очистка списков объектов
        public override void ClearObjects()
        {
            _objects.Clear();
        }

        //Подготока источника
        public override void Prepare()
        {
            foreach (var ob in _objects.Values)
                ob.IsInitialized = true;
        }
        
        //Создание фабрики ошибок
        protected override IErrMomFactory MakeErrFactory()
        {
            var factory = new ErrMomFactory(Code, ErrMomType.Source);
            factory.AddGoodDescr(0);
            factory.AddDescr(1, "Предупреждение", ErrorQuality.Warning);
            factory.AddDescr(2, "Ошибка");
            return factory;
        }

        //Чтение списков равномерных значений
        protected override void ReadCut()
        {
            foreach (var ob in _objects.Values)
                ob.MakeUniformValues(PeriodBegin, PeriodBegin, true);
        }
        protected override void ReadChanges()
        {
            foreach (var ob in _objects.Values)
                ob.MakeUniformValues(PeriodBegin, PeriodEnd, false);
        }
    }
}
