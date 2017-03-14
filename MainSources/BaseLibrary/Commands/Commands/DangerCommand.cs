using System;
using System.Threading;

namespace BaseLibrary
{
    //Команда, обрамляющая опасную операцию
    public class DangerCommand : Command
    {
        internal DangerCommand(Logger logger, Command parent, double startProcent, double finishProcent, 
                                        int repetitions,
                                        LoggerStability stability, //Минимальная LoggerStability, начиная с которой выполняется более одного повторения операции
                                        string eventMess,
                                        bool useThread = false, 
                                        int errWaiting = 0)
            : base(logger, parent, startProcent, finishProcent)
        {
            _repetitions = Logger.Stability >= stability ? repetitions : 1;
            _useThread = useThread;
            _errWaiting = errWaiting;
            _eventMess = eventMess;
        }

        //Cколько раз повторять, если не удалась (вместе с первым)
        private readonly int _repetitions;
        //Запускать опасную операцию в другом потоке, чтобы была возможность ее жестко прервать
        private readonly bool _useThread;
        //Cколько мс ждать при ошибке
        private readonly int _errWaiting;
        //Сообщение для истории
        private readonly string _eventMess; 

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
            return Run(action, null);
        }
        public Command Run(Action action, //операция
                                      Func<bool> erorrAction) //операция, выполняемя между повторами
        {
            for (int i = 1; i <= _repetitions; i++)
            {
                Logger.CheckBreak();
                _lastRepeat = i == _repetitions;
                _isCorrect = true;
                if (!_useThread) //однопоточный вариант
                {
                    if (RunAction(() => { action(); return true; })) 
                        return Finish();
                }
                else //многопоточный
                {
                    var thread = new Thread(() => RunAction(() => { action(); return true; }));
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
                Parent.AddError(new CommandError(_eventMess + ". Повтор операции из-за ошибки", null, "", "", CommandQuality.Repeat));
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
                bool b = action();
                _isCorrect &= b;
            }
            catch (BreakException) { throw; }
            catch (Exception ex)
            {
                _isCorrect = false;
                Logger.AddError(_eventMess + ". Ошибка", ex);
                Logger.CheckBreak();
                while (Logger.Command != this)
                    Logger.Command.Finish();
            }
            return _isCorrect;
        }
    }
}