using System;

namespace BaseLibrary
{
    //Класс для переопределения операций логгера
    public abstract class ExternalLogger : ILogger, IContextable
    {
        protected ExternalLogger() { }
        protected ExternalLogger(Logger logger)
        {
            Logger = logger;
        }

        //Ссылка на логгер
        public Logger Logger { get; set; }

        //Контекст
        public virtual string Context { get { return ""; } }

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

        //Запуск простой комманды
        public Command Start(double startProcent, double finishProcent)
        {
            return Logger.Start(startProcent, finishProcent);
        }
        //Завершение простоя команды
        public Command Finish(string results = "")
        {
            return Logger.Finish();
        }
        
        //Запуск команды логирования
        public LogCommand StartLog(double startProcent, double finishProcent, string name, string context = "", string pars = "")
        {
            return Logger.StartLog(startProcent, finishProcent, name, context, pars);
        }
        public LogCommand StartLog(string name, string context = "", string pars = "")
        {
            return Logger.StartLog(name, context, pars);
        }
        //Завершение команды логирования
        public LogCommand FinishLog(string results = "")
        {
            return Logger.FinishLog(results);
        }
        public void SetLogCommandResults(string results)
        {
            Logger.SetLogCommandResults(results);
        }

        //Запуск команды логирования в SuperHistory и отображения индикатора
        public ProgressCommand StartProgress(string text, string name, string pars = "", DateTime? endTime = null)
        {
            return Logger.StartProgress(text, name, pars, endTime);
        }
        public ProgressCommand StartProgress(DateTime begin, DateTime end, string mode, string name, string pars = "", DateTime? endTime = null)
        {
            return Logger.StartProgress(begin, end, mode, name, pars, endTime);
        }
        //Завершение команды логирования в SuperHistory
        public ProgressCommand FinishProgress()
        {
            return Logger.FinishProgress();
        }

        //Начало, конец и режим периода обработки
        public DateTime PeriodBegin { get { return Logger.PeriodBegin; } }
        public DateTime PeriodEnd { get { return Logger.PeriodEnd; } }
        public string PeriodMode { get { return Logger.PeriodMode; } }

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

        //Запись результатов в команду Collect
        public void SetCollectCommandResults(string results)
        {
            Logger.SetCollectCommandResults(results);
        }
        //Итоговая ошибка комманды Collect
        public string CollectedErrorMessage 
        { 
            get { return Logger.CollectedErrorMessage; } 
        }
        //Результаты выполнения команды Collect
        public string CollectedResults
        {
            get { return Logger.CollectedResults; }
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
                                        string errMess, //Сообщение об ошибке 
                                        string repeatMess, //Сообщение о повторе
                                        bool useThread = false, //Запускать опасную операцию в другом потоке, чтобы была возможность ее жестко прервать
                                        int errWaiting = 0)  //Cколько мс ждать при ошибке
        {
            return Logger.StartDanger(startProcent, finishProcent, repetitions, stability, errMess, repeatMess, useThread, errWaiting);
        }
        public DangerCommand StartDanger(int repetitions, LoggerStability stability, string errMess, string repeatMess, bool useThread = false, int errWaiting = 0)
        {
            return Logger.StartDanger(repetitions, stability, errMess, repeatMess, useThread, errWaiting);
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