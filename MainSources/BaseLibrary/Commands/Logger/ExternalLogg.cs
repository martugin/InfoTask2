using System;

namespace BaseLibrary
{
    //Класс для переопределения операций логгера
    public abstract class ExternalLogg : IContextable
    {
        protected ExternalLogg() { }
        protected ExternalLogg(Logger logger)
        {
            Logger = logger;
        }

        //Ссылка на логгер
        public Logger Logger { get; set; }
        //Ссылка на историю
        public IHistory History { get { return Logger.History; } }
        //Контекст
        public virtual string Context { get { return ""; } }

        //Процент индикатора
        public double TabloProcent
        {
            get { return Logger.TabloProcent; }
            set { Logger.TabloProcent = value; }
        }

        //Три уровня текста на форме индикатора
        public string TabloText(int number)
        {
            return Logger.TabloText(number);
        }
        public void SetTabloText(int number, string text)
        {
            Logger.SetTabloText(number, text);
        }

        //Период обработки
        public DateTime BeginPeriod { get { return Logger.BeginPeriod; } }
        public DateTime EndPeriod { get { return Logger.EndPeriod; } }
        public string ModePeriod { get { return Logger.ModePeriod; } }

        //Прервать выполнение
        public void Break()
        {
            Logger.Break();
        }
        //Вызвать BreakException
        internal protected void CheckBreak()
        {
            Logger.CheckBreak();
        }

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
        public Command Finish()
        {
            return Logger.Finish();
        }

        //Запуск команды логирования
        public CommandLog StartLog(double startProcent, double finishProcent, string name, string context = "", string pars = "")
        {
            return Logger.StartLog(startProcent, finishProcent, name, context, pars);
        }
        public CommandLog StartLog(string name, string context = "", string pars = "")
        {
            return Logger.StartLog(name, context, pars);
        }
        //Завершение команды логирования
        public CommandLog FinishLog(string results = "")
        {
            return Logger.FinishLog(results);
        }

        //Запуск команды логирования в SuperHistory и отображения индикатора
        public CommandProgress StartProgress(string text, string name, string pars = "", DateTime? endTime = null)
        {
            return Logger.StartProgress(text, name, pars, endTime);
        }
        public CommandProgress StartProgress(DateTime begin, DateTime end, string mode, string name, string pars = "", DateTime? endTime = null)
        {
            return Logger.StartProgress(begin, end, mode, name, pars, endTime);
        }
        //Завершение команды логирования в SuperHistory
        public CommandProgress FinishProgress()
        {
            return Logger.FinishProgress();
        }

        //Запуск команды, отображающей на форме индикатора текст 2-ого уровня
        public CommandIndicatorText StartProgressText(double startProcent, double finishProcent, string text)
        {
            return Logger.StartIndicatorText(startProcent, finishProcent, text);
        }
        //Завершение команды, отображающей на форме индикатора текст 2-ого уровня
        public CommandIndicatorText StartProgressText(string text)
        {
            return Logger.StartIndicatorText(text);
        }

        //Запуск команды, колекционирущей ошибки
        public CommandCollect StartCollect(bool isWriteHistory, //Записывать ошибки в ErrorsList
                                                         bool isCollect) //Формировать общую ошибку
        {
            return Logger.StartCollect(isWriteHistory, isCollect);
        }
        //Завершение команды, колекционирущей ошибки
        public CommandCollect FinishCollect()
        {
            return Logger.FinishCollect();
        }

        //Запуск команды, которая копит ошибки, но не выдает из во вне
        public CommandKeep StartKeep(double startProcent, double finishProcent)
        {
            return Logger.StartKeep(startProcent, finishProcent);
        }
        public CommandKeep StartKeep()
        {
            return Logger.StartKeep();
        }
        //Накопленная ошибка
        public string KeepedError { get { return Logger.KeepedError; } }

        //Запуск команды, обрамляющей опасную операцию
        public CommandDanger StartDanger(double startProcent, double finishProcent,
                                        int repetitions, //Cколько раз повторять, если не удалась (вместе с первым)
                                        LoggerDangerness dangerness, //Минимальная LoggerDangerness, начиная с которой выполняется более одного повторения операции
                                        string errMess, //Сообщение об ошибке 
                                        string repeatMess, //Сообщение о повторе
                                        bool useThread = false, //Запускать опасную операцию в другом потоке, чтобы была возможность ее жестко прервать
                                        int errWaiting = 0)  //Cколько мс ждать при ошибке
        {
            return Logger.StartDanger(startProcent, finishProcent, repetitions, dangerness, errMess, repeatMess, useThread, errWaiting);
        }
        public CommandDanger StartDanger(int repetitions, LoggerDangerness dangerness, string errMess, string repeatMess, bool useThread = false, int errWaiting = 0)
        {
            return Logger.StartDanger(repetitions, dangerness, errMess, repeatMess, useThread, errWaiting);
        }
        //Без повторов
        public CommandDanger StartDanger(double startProcent, double finishProcent)
        {
            return StartDanger(startProcent, finishProcent, 1, LoggerDangerness.Single, "", "");
        }
        public CommandDanger StartDanger()
        {
            return StartDanger(0, 100);
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
    }
}