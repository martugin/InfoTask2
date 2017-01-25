using System;
using System.Threading;
using System.Threading.Tasks;

namespace BaseLibrary
{
    //Команда, обрамляющая опасную операцию
    public class CommDanger : Comm
    {
        internal CommDanger(Logg logger, Comm parent, double startProcent, double finishProcent, 
                                        int repetitions,
                                        LoggerDangerness dangerness, //Минимальная LoggerDangerness, начиная с которой выполняется более одного повторения операции
                                        string errMess, string repeatMess,
                                        bool useThread = false, 
                                        int errWaiting = 0)
            : base(logger, parent, startProcent, finishProcent)
        {
            _repetitions = Logger.Dangerness >= dangerness ? repetitions : 1;
            _useThread = useThread;
            _errWaiting = errWaiting;
            _errMess = errMess;
            _repeatMess = repeatMess;
        }

        //Cколько раз повторять, если не удалась (вместе с первым)
        private readonly int _repetitions;
        //Запускать опасную операцию в другом потоке, чтобы была возможность ее жестко прервать
        private readonly bool _useThread;
        //Cколько мс ждать при ошибке
        private readonly int _errWaiting;
        //Сообщения об ошибке и о повторе
        private readonly string _errMess; 
        private readonly string _repeatMess; 

        //При повторении операции случилась ошибка
        private bool _isError;
        //Идет последнее повторении операции
        private bool _lastRepeat;

        //Добавить ошибку 
        public override void AddError(ErrorCommand err)
        {
            _isError |= err.Quality == CommandQuality.Error;
            AddQuality(_lastRepeat ? err.Quality : CommandQuality.Repeat);
            if (_lastRepeat)
                Parent.AddError(err);
            else if (Logger.History != null)
                Logger.History.WriteError(err);
        }

        //Запуск операции, обрамляемой данной командой
        public override Comm Run(Func<string> func)
        {
            return Run(() => func(), null);
        }
        public Comm Run(Action action, //операция
                                 Action erorrAction) //операция, выполняемя между повторами, возвращает true при успешном завершении
        {
            for (int i = 1; i <= _repetitions; i++)
            {
                Logger.CheckBreak();
                _lastRepeat = i == _repetitions;
                _isError = false;
                if (!_useThread) //однопоточный вариант
                {
                    if (RunAction(action)) 
                        return Finish();
                }
                else //многопоточный
                {
                    var t = new Task<bool>(() => RunAction(action));
                    t.Start();
                    while (!t.IsCompleted)
                    {
                        Thread.Sleep(50);
                        if (Logger.WasBreaked)
                        {
                            int n = 0;
                            while (!t.IsCompleted && (n += 50) < 2000)
                                Thread.Sleep(50);
                            if (!t.IsCompleted) 
                                t.Dispose();
                            Logger.CheckBreak();
                            return this;
                        }
                    }
                    Logger.CheckBreak();
                    if (t.Result) return Finish();
                }

                if (i == _repetitions) return Finish();
                Parent.AddError(new ErrorCommand(_repeatMess, null, "" , "", CommandQuality.Repeat));
                Logger.CheckBreak();
                while (Logger.Command != this)
                    Logger.Finish();

                if (_errWaiting != 0)
                {
                    int n = 0;
                    while ((n += 50) < _errWaiting)
                    {
                        Thread.Sleep(Math.Min(50, _errWaiting));
                        Logger.CheckBreak();
                    }
                }

                _isError = false;
                if (erorrAction != null && !RunAction(erorrAction))
                    return Finish();
            }
            return this;
        }

        //Запуск действия
        private bool RunAction(Action action)
        {
            try
            {
                action();
                return !_isError;
            }
            catch (BreakException) { throw; }
            catch (Exception ex)
            {
                Logger.AddError(_errMess, ex);
                Logger.CheckBreak();
                while (Logger.Command != this)
                    Logger.Command.Finish();
                return false;
            }
        }
    }
}