using System;
using System.Threading;

namespace BaseLibrary
{
    //Команда, обрамляющая опасную операцию
    public class DangerCommand : Command
    {
        internal DangerCommand(Logger logger, Command parent, double startProcent, double finishProcent, 
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
        private bool _isCorrect;
        //Идет последнее повторении операции
        private bool _lastRepeat;

        //Добавить ошибку 
        public override void AddError(CommandError err)
        {
            _isCorrect &= err.Quality != CommandQuality.Error;
            AddQuality(_lastRepeat ? err.Quality : CommandQuality.Repeat);
            if (_lastRepeat)
                Parent.AddError(err);
            else if (Logger.History != null)
                Logger.History.WriteError(err);
        }

        //Запуск операции, обрамляемой данной командой
        public override Command Run(Action action)
        {
            return Run(action, (Func<bool>) null);
        }
        public Command Run(Action action, Action erorrAction)
        {
            return Run(action, () => { erorrAction(); return true; });
        }
        public Command Run(Action action, Func<bool> erorrAction)
        {
            return Run(() => { action(); return true; }, erorrAction);
        }
        public Command Run(Func<bool> action, Action erorrAction)
        {
            return Run(action, () => { erorrAction(); return true; });
        }
        public Command Run(Func<bool> action, //операция
                                       Func<bool> erorrAction) //операция, выполняемя между повторами, возвращает true при успешном завершении
        {
            for (int i = 1; i <= _repetitions; i++)
            {
                Logger.CheckBreak();
                _lastRepeat = i == _repetitions;
                _isCorrect = true;
                if (!_useThread) //однопоточный вариант
                {
                    if (RunAction(action)) 
                        return Finish();
                }
                else //многопоточный
                {
                    var thread = new Thread(() => RunAction(action));
                    thread.Start();
                    while (thread.ThreadState == ThreadState.Running)
                    {
                        Thread.Sleep(50);
                        if (Logger.WasBreaked)
                        {
                            int n = 0;
                            while (thread.ThreadState == ThreadState.Running && (n += 50) < 2000)
                                Thread.Sleep(50);
                            if (thread.ThreadState == ThreadState.Running)
                            {
                                thread.Abort();
                                Thread.Sleep(100);
                            }
                            Logger.CheckBreak();
                            return this;
                        }
                    }
                    Logger.CheckBreak();
                    if (_isCorrect) return Finish();
                }

                if (i == _repetitions) return Finish();
                Parent.AddError(new CommandError(_repeatMess, null, "" , "", CommandQuality.Repeat));
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

                _isCorrect = true;
                if (erorrAction != null && !RunAction(erorrAction))
                    return Finish();
            }
            return this;
        }

        //Запуск действия
        private bool RunAction(Func<bool> action)
        {
            try
            {
                bool b = action(); //Если опустить присвоение, то не работает
                _isCorrect &= b;
                return _isCorrect;
            }
            catch (BreakException) { throw; }
            catch (Exception ex)
            {
                Logger.AddError(_errMess, ex);
                Logger.CheckBreak();
                while (Logger.Command != this)
                    Logger.Command.Finish();
                return _isCorrect = false;
            }
        }
    }
}