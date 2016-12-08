
namespace BaseLibrary
{
    //Простая комманда
    public class Comm
    {
        //Создание новой комманды 
        internal Comm(Logg logger, Comm parent, double startProcent, double finishProcent)
        {
            Logger = logger;
            Parent = parent;
            _startProcent = startProcent;
            _finishProcent = finishProcent;
        }

        //Указатель на родителя
        internal protected Comm Parent { get; private set; }
        //Указатель на Logger
        internal protected Logg Logger { get; private set; }

        //Качество команды
        private CommandQuality _quality = CommandQuality.Success;
        public CommandQuality Quality { get { lock (_qualityLock) return _quality; }}

        //Добавить ошибочность команды
        public CommandQuality AddQuality(CommandQuality quality)
        {
            lock (_qualityLock)
                if (_quality < quality) 
                    _quality = quality;
            if (Parent != null)
                Parent.AddQuality(quality);
            return _quality;
        }
        
        //Величина
        private double _procent;
        public virtual double Procent
        {
            get { lock (_procentLock) return _procent; }
            set
            {
                lock (_procentLock)
                {
                    _procent = value;
                    if (Parent != null)
                        Parent.Procent = _startProcent + value * (_finishProcent - _startProcent) / 100;
                }
            }
        }
        //Каким значениям родителя соответствует 0% и 100% счетчика
        private readonly double _startProcent;
        private readonly double _finishProcent;

        //Объекты для блокировки качества и процентов потоком
        private readonly object _qualityLock = new object();
        private readonly object _procentLock = new object();

        //Команда завершена
        public bool IsFinished { get; private set; }
        //Комманда была прервана
        public bool IsBreaked { get; private set; }

        //Завершает комманду и ее же возвращает
        public virtual Comm Finish(bool isBreaked = false) //Комманда была прервана
        {
            Procent = 100;
            IsFinished = true;
            IsBreaked = isBreaked;
            Logger.Command = Logger.Command.Parent;
            return this;
        }
    }
}