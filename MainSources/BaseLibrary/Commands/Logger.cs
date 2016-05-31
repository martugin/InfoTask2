using System;
using System.Net;
using System.Threading;

namespace BaseLibrary
{
    //Интерфейс, логирующий и отображающий комманды и ошибки
    public class Logger
    {
        //Вызываются при завершении комманд
        internal protected virtual void FinishLogCommand() { }
        internal protected virtual void FinishSubLogCommand() { }
        internal protected virtual void FinishProgressCommand() { }

        //Другие действия при возникновении ошибки, кроме добавления в историю (отобразить на табло и т.п.)
        protected virtual void MessageError(ErrorCommand er) { }
        //Отображает процент на индикаторе процесса
        internal protected virtual void ViewProcent(double procent) { }

        //Процент текущей комманды
        public double Procent
        {
            get { return Command.Procent; }
            set { Command.Procent = value; }
        }

        //Работа с файлами истории
        #region
        //Файл истории и шаблон для него
        private string _historyFile;
        private string _historyTemplate;
        //База данных History
        protected DaoDb HistoryDb;
        //Строка с причиной создания нового файла истории
        private string _reasonUpdate;
        //Использовать SubHistory
        private bool _useSubHistory;
        //Использовать ErrorsList
        private bool _useErrorsList;

        //Рекордсет с таблицей SubHistory
        internal RecDao SubHistory { get; private set; }
        //Рекордсет с таблицей History
        internal RecDao History { get; private set; }
        //Рекордсет с таблицей SuperHistory
        private RecDao _superHistory;
        //Рекордсет с таблицей ErrorsList
        protected RecDao ErrorsRec { get; set; }

        //Текущие Id истории
        internal int LastHistoryId { private get; set; }
        
        //Задание файла истории historyFile и файла его шаблона historyTemplate, открытие истории, useSubHistory - использовать SubHistory
        public void OpenHistory(string historyFile = null, string historyTemplate = null, bool useSubHistory = false, bool useErrorsList = true)
        {
            try
            {
                _historyFile = historyFile;
                _historyTemplate = historyTemplate;
                _useSubHistory = useSubHistory;
                _useErrorsList = useErrorsList;
                if (_historyFile != null)
                {
                    if (_historyTemplate != null && DaoDb.FromTemplate(_historyTemplate, _historyFile, ReplaceByTemplate.IfNewVersion, true))
                            _reasonUpdate = "Новая версия файла истории";
                    OpenHistoryRecs();
                }
            }
            catch (Exception ex)
            {
                AddErrorAboutHistory(ex);
            }
        }

        //Сохранение старого файла истории и добавление нового
        public void UpdateHistory(bool openAfterUpdate)
        {
            try
            {
                if (LastHistoryId > 300000)
                {
                    _reasonUpdate = "Старый файл истории содержит более 300000 записей";
                    LastHistoryId = 0;
                }
                    
                if (_reasonUpdate != null)
                {
                    CloseHistory();
                    Thread.Sleep(1500);
                    DaoDb.FromTemplate(_historyTemplate, _historyFile, ReplaceByTemplate.Always, true);
                    Thread.Sleep(1500);
                    if (openAfterUpdate) OpenHistoryRecs();
                }
            }
            catch (Exception ex)
            {
                AddErrorAboutHistory(ex);
            }
        }

        //Закрывает историю, если надо то копирует новый файл
        public void CloseHistory()
        {
            try
            {
                if (SubHistory != null) SubHistory.Dispose();
                if (History != null) History.Dispose();
                if (_superHistory != null) _superHistory.Dispose();
                if (ErrorsRec != null) ErrorsRec.Dispose();
                if (HistoryDb != null) HistoryDb.Dispose();
                SubHistory = null;
                History = null;
                _superHistory = null;
                ErrorsRec = null;
                HistoryDb = null;
            }
            catch { }
        }

        //Открытие рекордсетов истории и добавление в историю первой записи после создания
        private void OpenHistoryRecs()
        {
            HistoryDb = new DaoDb(_historyFile);
            History = new RecDao(HistoryDb, "History");
            _superHistory = new RecDao(HistoryDb, "SuperHistory");
            if (_useSubHistory) SubHistory = new RecDao(HistoryDb, "SubHistory");
            if (_useErrorsList) ErrorsRec = new RecDao(HistoryDb, "ErrorsList");
            if (_reasonUpdate != null)
            {
                try
                {
                    StartLog("Создание нового файла истории", _reasonUpdate).Dispose();
                    _reasonUpdate = null;
                }
                catch (Exception ex)
                {
                    AddErrorAboutHistory(ex);
                }
                LastHistoryId = 0;    
            }
        }

        //Обрамление для операций с таблицами истории
        internal void RunHistoryOperation(RecDao rec, Action fun)
        {
            if (rec != null)
            {
                try
                {
                    fun();
                    rec.Update();
                }
                catch (Exception ex)
                {
                    AddErrorAboutHistory(ex);
                }
            }
        }

        //Вызывается, если при работе с историей произошла ошибка
        private void AddErrorAboutHistory(Exception ex)
        {
            _reasonUpdate = "Ошибка при работе с файлом истории." + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
            UpdateHistory(true);
        }
        #endregion

        //Добавление событий и ошибок в историю
        #region
        //Добавляет событие в историю
        public void AddEvent(string description, string pars = "")
        {
            RunHistoryOperation(_superHistory, () =>
            {
                _superHistory.AddNew();
                _superHistory.Put("Description", description);
                _superHistory.Put("Params", pars);
                _superHistory.Put("Time", DateTime.Now);
                if (CommandLog != null)
                {
                    _superHistory.Put("HistoryId", CommandLog.HistoryId);
                    _superHistory.Put("FromStart", CommandLog.FromEvent);
                    CommandLog.LogEventTime = DateTime.Now;
                }
            });
        }
        //Добавляет событие в лог c указанием процентов текущей комманды
        public void AddEvent(string description, string pars, double procent)
        {
            AddEvent(description, pars);
            Procent = procent;
        }
        public void AddEvent(string description, double procent)
        {
            AddEvent(description);
            Procent = procent;
        }

        //Добавляет ошибку в комманду, лог и отображение, err - ошибка, 
        private void AddError(ErrorCommand err)
        {
            if (Command != null)
                Command.AddError(err, false);
            LogError(err);
            LogToErrorsList(err);
            MessageError(err);
        }

        //text - текст ошибки, ex - исключение, par - праметры ошибки
        public void AddError(string text, Exception ex = null, string par = "", string context = "")
        {
            var err = new ErrorCommand(text, ex, par, context);
            AddError(err);
        }

        //text - текст ошибки, ex - исключение, par - праметры ошибки
        public void AddWarning(string text, Exception ex = null, string par = "", string context = "")
        {
            var err = new ErrorCommand(text, ex, par, context, CommandQuality.Warning);
            AddError(err);
        }

        //Добавляет ошибку в лог
        private void LogError(ErrorCommand er)
        {
            RunHistoryOperation(_superHistory, () =>
            {
                _superHistory.AddNew();
                if (CommandLog != null) _superHistory.Put("HistoryId", CommandLog.HistoryId);
                _superHistory.Put("Description", er.Text);
                _superHistory.Put("Params", er.ToLog());
                _superHistory.Put("Time", DateTime.Now);
                if (CommandLog != null)
                    _superHistory.Put("FromStart", CommandLog.FromEvent);
                _superHistory.Put("Status", er.Quality.ToRussian());
            });
        }

        //Добавляет ошибку в ErrorsList
        private void LogToErrorsList(ErrorCommand er)
        {
            RunHistoryOperation(ErrorsRec, () =>
            {
                ErrorsRec.AddNew();
                ErrorsRec.Put("Status", er.Quality.ToRussian());
                ErrorsRec.Put("Description", er.Text);
                ErrorsRec.Put("Params", er.ToLog());
                ErrorsRec.Put("Time", DateTime.Now);
                if (CommandLog != null)
                {
                    ErrorsRec.Put("Command", CommandLog.Name);
                    ErrorsRec.Put("Context", CommandLog.Context);
                }
                if (CommandSubLog != null)
                {
                    ErrorsRec.Put("PeriodBegin", CommandSubLog.PeriodBegin);
                    ErrorsRec.Put("PeriodEnd", CommandSubLog.PeriodEnd);
                }
            });
        }
        #endregion

        //Запуск команд
        #region
        //Текущая выполняемая комманда
        public Command Command { get; internal set; }
        //Текущая команда, записывающая в History
        public CommandLog CommandLog { get; private set; }
        //Текущая команда, записывающая в SubHistory
        public CommandSubLog CommandSubLog { get; private set; }

        //Запуск простой комманды с процентами и без
        public Command Start(double start, double finish, CommandFlags flags = CommandFlags.Simple, string context = "")
        {
            return Command = new Command(this, Command, start, finish, flags, context);
        }
        public Command Start(CommandFlags flags = CommandFlags.Simple, string context = "")
        {
            return Command = new Command(this, Command, flags, context);
        }
        
        //Запуск простой комманды для записи в History с процентами и без
        public CommandLog StartLog(double start, double finish, string name, string pars = "", CommandFlags flags = CommandFlags.Simple, string context = "")
        {
            Command = CommandLog = new CommandLog(this, Command, start, finish, name, pars, flags, context);
            return CommandLog;
        }
        public CommandLog StartLog(string name, string pars = "", CommandFlags flags = CommandFlags.Simple, string context = "")
        {
            Command = CommandLog = new CommandLog(this, Command, name, pars, flags, context);
            return CommandLog;
        }

        //Запуск простой комманды для записи в SubHistory с процентами и без
        public CommandSubLog StartSubLog(double start, double finish, string name, DateTime periodBegin, DateTime periodEnd, string mode, CommandFlags flags = CommandFlags.Simple, string context = "")
        {
            Command = CommandSubLog = new CommandSubLog(this, Command, start, finish, name, periodBegin, periodEnd, mode, flags, context);
            return CommandSubLog;
        }
        public CommandSubLog StartSubLog(string name, DateTime periodBegin, DateTime periodEnd, string mode, CommandFlags flags = CommandFlags.Simple, string context = "")
        {
            Command = CommandSubLog = new CommandSubLog(this, Command, name, periodBegin, periodEnd, mode, flags, context);
            return CommandSubLog;
        }

        //Такие же запуски комманд только с выполнением действия и завершением комманды
        private bool RunAction(Command command, Action action, string errMess)
        {
            using (command)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    AddError(errMess, ex);
                }
                return !command.IsError;
            }    
        }

        public bool Start(Action action, double start, double finish, CommandFlags flags = CommandFlags.Simple, string errMess = "Ошибка")
        {
            return RunAction(Start(start, finish, flags), action, errMess);
        }
        public bool Start(Action action, CommandFlags flags = CommandFlags.Simple, string errMess = "Ошибка")
        {
            return RunAction(Start(flags), action, errMess);
        }
        
        public bool StartLog(Action action, double start, double finish, string name, string pars = "", CommandFlags flags = CommandFlags.Simple, string context = "", string errMess = "Ошибка")
        {
            return RunAction(StartLog(start, finish, name, pars, flags, context), action, errMess);
        }
        public bool StartLog(Action action, string name, string pars = "", CommandFlags flags = CommandFlags.Simple, string context = "", string errMess = "Ошибка")
        {
            return RunAction(StartLog(name, pars, flags, context), action, errMess);
        }

        public bool StartSubLog(Action action, double start, double finish, string name, DateTime periodBegin, DateTime periodEnd, string mode, CommandFlags flags = CommandFlags.Simple, string errMess = "Ошибка")
        {
            return RunAction(StartSubLog(start, finish, name, periodBegin, periodEnd, mode, flags), action, errMess);
        }
        public bool StartSubLog(Action action, string name, DateTime periodBegin, DateTime periodEnd, string mode, CommandFlags flags = CommandFlags.Simple, string errMess = "Ошибка")
        {
            return RunAction(StartSubLog(name, periodBegin, periodEnd, mode, flags), action, errMess);
        }
        
        //Завершает комманду, results - результаты для записи в лог или отображения, возвращает команду
        public Command Finish(string results = null, bool isBreaked = false)
        {
            return Command.Finish(results, isBreaked);
        }
        #endregion

        //Опасные операции
        #region
        // Однопоточный вариант
        //Возвращает true, если операция прошла успешно (может не с первого раза)
        public bool Danger(Func<bool> operation, //операция, которую нужно выполнить, 
                                    int repetitions, //сколько раз повторять, если не удалась (вместе с первым)
                                    int errorWaiting = 0, //сколько мс ждать при ошибке
                                    string errMess = "Не удалось выполнить опасную операцию", //сообшение об ошибке, если все повторения не удались
                                    Func<bool> errorOperation = null) //операция, выполняемя между повторами
        {
            //Выполняем первый раз
            if (RunDanger(operation, null)) return true;

            bool b = false;
            for (int i = 2; i <= repetitions && !b; i++)
            {   //Выполняем последующие разы
                WaitAfterError(errorWaiting);
                AddEvent("Повтор опасной операции", i + "-й раз");
                b |= RunDanger(operation, errorOperation);
            }

            if (!b) AddError(errMess);
            return b;
        }

        //Запуск опасной операции, возвращает комманду
        private bool RunDanger(Func<bool> operation, Func<bool> errorOperation)
        {
            using (Start(CommandFlags.Danger))
            {
                try
                {
                    bool b = errorOperation == null || errorOperation();
                    Procent = 20;
                    if (b) b &= operation();
                    return b;
                }
                catch (Exception ex)
                {
                    AddError("Ошибка при опасной операции", ex);
                    return false;
                }
            }
        }

        //Ожидение поле неудачного выполнения операции
        private void WaitAfterError(int errorWaiting)
        {
            if (errorWaiting < 3000) Thread.Sleep(errorWaiting);
            else
            {
                for (int j = 0; j < errorWaiting / 500; j++)
                {
                    Procent = j * 100 * (500.0 / errorWaiting);
                    Thread.Sleep(500);
                }
            }
        }
        #endregion

        //True, если операции выполняются без присутствия пользователя (периодический режим)
        public bool IsRepeat { get; protected set; }

        //Описание потока выполнения
        protected string ThreadName
        {
            set { FullThreadName = value + "_" + Dns.GetHostName(); }
        }
        //Полное описание потока, с указанием имени компьютера
        public string FullThreadName { get; private set; }
    }
}