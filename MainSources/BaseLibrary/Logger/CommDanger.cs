using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BaseLibrary
{
    //Команда, обрамляющая опасную операцию
    public class CommDanger : Comm
    {
        internal CommDanger(Logg logger, Comm parent, double startProcent, double finishProcent, 
                                        int repetions,
                                        LoggerDangerness dangerness, //Минимальная LoggerDangerness, начиная с которой выполняется более одного повторения операции
                                        bool useThread = false, 
                                        int errWaiting = 0, 
                                        string errMess = "Не удалось выполнить опасную операцию")
            : base(logger, parent, startProcent, finishProcent)
        {
            _repetitions = Logger.Dangerness >= dangerness ? repetions : 1;
            _useThread = useThread;
            _errWaiting = errWaiting;
            _errMess = errMess;
        }

        //Cколько раз повторять, если не удалась (вместе с первым)
        private readonly int _repetitions;
        //Запускать опасную операцию в другом потоке, чтобы была возможность ее жестко прервать
        private readonly bool _useThread;
        //Cколько мс ждать при ошибке
        private readonly int _errWaiting;
        //Сообщение об ошибке 
        private readonly string _errMess; 

        //Список ошибок
        private readonly List<ErrorCommand> _errors = new List<ErrorCommand>();
        
        //Добавить ошибку 
        public override void AddError(ErrorCommand err)
        {
            _errors.Add(err);
            AddQuality(err.Quality);
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
                _errors.Clear();
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
                Parent.AddError(new ErrorCommand("Повтор операции", null, _errMess, "", CommandQuality.Repeat));
                
                Logger.CheckBreak();
                if (_errWaiting != 0)
                {
                    int n = 0;
                    while ((n += 50) < _errWaiting)
                    {
                        Thread.Sleep(Math.Min(50, _errWaiting));
                        Logger.CheckBreak();
                    }
                }

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
                return _errors.Count == 0;
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

        //Завершает комманду
        internal protected override void FinishCommand(string results, bool isBreaked)
        {
            base.FinishCommand(results, isBreaked);
            foreach (var err in _errors)
                Parent.AddError(err);
        }
    }
}