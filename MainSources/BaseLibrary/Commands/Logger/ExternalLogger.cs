using System;

namespace BaseLibrary
{
    //Класс для переопределения операций логгера
    public abstract class ExternalLogger : ILogger, IContextable
    {
        protected ExternalLogger() { }
        protected ExternalLogger(Logger logger, //Логгер
                                               string logContext = null, //Контекст для команды Log
                                               string progressContext = null) //Контекст для команды Progress
        {
            Context = logContext;
            ProgressContext = progressContext;
            Logger = logger;
        }

        //Ссылка на логгер
        public Logger Logger { get; set; }

        //Режим работы потока
        public LoggerStability Stability { get { return Logger.Stability; } }

        //Контекст команды LogCommand
        public string Context { get; set; }
        //Контекст команды ProgressCommand
        public string ProgressContext { get; set; }

        //Запуск простой комманды
        public Command Start(double startProcent, double finishProcent)
        {
            return Logger.Start(startProcent, finishProcent);
        }
        public Command Start()
        {
            return Logger.Start();
        }
        //Завершение простоя команды
        public Command Finish(string results = "")
        {
            return Logger.Finish();
        }

        //Запуск команды, колекционирущей ошибки
        public CollectCommand StartCollect(bool isWriteHistory, //Записывать ошибки в ErrorsList
                                                              bool isCollect) //Формировать общую ошибку
        {
            return Logger.StartCollect(isWriteHistory, isCollect);
        }
        //Завершение команды, колекционирущей ошибки
        public CollectCommand FinishCollect(string results = null)
        {
            return Logger.FinishCollect(results);
        }

        //Запускает команду Collect и дожидается ее завершения
        public void RunSyncCommand(Action action)
        {
            Logger.RunSyncCommand(action);
        }
        //То же самое. только с запуском вложенной PeriodCommand
        public void RunSyncCommand(DateTime beg, DateTime en, Action action)
        {
            Logger.RunSyncCommand(beg, en, action);
        }
        //Запуск асинхронной команды Collect
        public void RunAsyncCommand(Action action)
        {
            Logger.RunAsyncCommand(action);
        }
        public void RunAsyncCommand(DateTime beg, DateTime en, Action action)
        {
            Logger.RunAsyncCommand(beg, en, action);
        }

        //Запись результатов в команду Collect
        public void AddCollectResult(string result)
        {
            Logger.AddCollectResult(result);
        }
        //Итоговая ошибка комманды Collect
        public string CollectedError
        {
            get { return Logger.CollectedError; }
        }
        //Результаты выполнения команды Collect
        public string CollectedResults
        {
            get { return Logger.CollectedResults; }
        }

        //Команда, задающая период обработки
        public PeriodCommand StartPeriod(DateTime begin, DateTime end, string mode)
        {
            return Logger.StartPeriod(begin, end, mode);
        }
        public PeriodCommand FinishPeriod()
        {
            return Logger.FinishPeriod();
        }

        //Начало, конец и режим периода обработки
        public virtual DateTime PeriodBegin { get { return Logger.PeriodBegin; } }
        public virtual DateTime PeriodEnd { get { return Logger.PeriodEnd; } }
        public string PeriodMode { get { return Logger.PeriodMode; } }

        //Запуск команды логирования в SuperHistory и отображения индикатора
        public ProgressCommand StartProgress(string name, string pars = "", string context = null, DateTime? endTime = null)
        {
            return Logger.StartProgress(name, pars, context ?? ProgressContext, endTime);
        }
        //Завершение команды логирования в SuperHistory
        public ProgressCommand FinishProgress()
        {
            return Logger.FinishProgress();
        }

        //Запуск команды логирования
        public LogCommand StartLog(double startProcent, double finishProcent, string name, string pars = null, string context = null)
        {
            return Logger.StartLog(startProcent, finishProcent, name, pars, context ?? Context);
        }
        public LogCommand StartLog(string name, string pars = null, string context = null)
        {
            return Logger.StartLog(name, pars, context ?? Context);
        }
        //Завершение команды логирования
        public LogCommand FinishLog(string results = "")
        {
            return Logger.FinishLog(results);
        }
        public void SetLogResults(string results)
        {
            Logger.SetLogResults(results);
        }

        //Запуск команды, отображающей на форме индикатора текст 2-ого уровня
        public IndicatorTextCommand StartIndicatorText(double startProcent, double finishProcent, string text)
        {
            return Logger.StartIndicatorText(startProcent, finishProcent, text);
        }
        public IndicatorTextCommand StartIndicatorText(string text)
        {
            return Logger.StartIndicatorText(text);
        }
        //Завершение команды, отображающей на форме индикатора текст 2-ого уровня
        public IndicatorTextCommand FinishIndicatorText()
        {
            return Logger.FinishIndicatorText();
        }

        //Запуск команды, которая копит ошибки, но не выдает из во вне
        public KeepCommand StartKeep(double startProcent, double finishProcent)
        {
            return Logger.StartKeep(startProcent, finishProcent);
        }
        public KeepCommand StartKeep()
        {
            return Logger.StartKeep();
        }
        public KeepCommand FinishKeep()
        {
            return Logger.FinishKeep();
        }

        //Ошибка, накопленная KeepCommand
        public string KeepedError { get { return Logger.KeepedError; } }

        //Запуск команды, обрамляющей опасную операцию
        public DangerCommand StartDanger(double startProcent, double finishProcent,
                                        int repetitions, //Cколько раз повторять, если не удалась (вместе с первым)
                                        LoggerStability stability, //Минимальная LoggerStability, начиная с которой выполняется более одного повторения операции
                                        string eventMess, //Сообщение о событии для записи в историю
                                        bool useThread = false, //Запускать опасную операцию в другом потоке, чтобы была возможность ее жестко прервать
                                        int errWaiting = 0)  //Cколько мс ждать при ошибке
        {
            return Logger.StartDanger(startProcent, finishProcent, repetitions, stability, eventMess, useThread, errWaiting);
        }
        public DangerCommand StartDanger(int repetitions, LoggerStability stability, string eventMess, bool useThread = false, int errWaiting = 0)
        {
            return Logger.StartDanger(repetitions, stability, eventMess, useThread, errWaiting);
        }

        //Добавляет событие в историю
        public void AddEvent(string description, string pars = "")
        {
            Logger.AddEvent(description, pars);
        }
        //Добавляет событие в лог c указанием процентов текущей комманды
        public void AddEvent(string description, string pars, double procent)
        {
            Logger.AddEvent(description, pars, procent);
        }
        public void AddEvent(string description, double procent)
        {
            Logger.AddEvent(description, procent);
        }

        //text - текст ошибки, ex - исключение, par - праметры ошибки
        public void AddError(string text, Exception ex = null, string pars = "", string context = null)
        {
            Logger.AddError(text, ex, pars, context);
        }

        //text - текст ошибки, ex - исключение, par - праметры ошибки
        public void AddWarning(string text, Exception ex = null, string pars = "", string context = null)
        {
            Logger.AddWarning(text, ex, pars, context);
        }

        //Процент текущей комманды
        public double Procent
        {
            get { return Logger.Procent; }
            set { Logger.Procent = value; }
        }
        
        //Прервать выполнение
        public void Break()
        {
            Logger.Break();
        }
        //Вызвать BreakException
        protected void CheckBreak()
        {
            Logger.CheckBreak();
        }
    }
}