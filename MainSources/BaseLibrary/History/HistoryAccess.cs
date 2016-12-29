using System;
using System.Threading;

namespace BaseLibrary
{
    //История в файле Access
    public class HistoryAccess : IHistory
    {
        //Задание файла истории
        public HistoryAccess(Logg logger, //Ссылка на логгер
                                        string historyFile, //файл истории
                                        string historyTemplate, //шаблон для файла истории
                                        bool useSubHistory = false, //использовать SubHistory
                                        bool useErrorsList = true) //использовать ErrorsList
        {
            try
            {
                Logger = logger;
                _historyFile = historyFile;
                _historyTemplate = historyTemplate;
                _useSubHistory = useSubHistory;
                _useErrorsList = useErrorsList;
                if (_historyFile != null)
                {
                    if (_historyTemplate != null &&
                        DaoDb.FromTemplate(_historyTemplate, _historyFile, ReplaceByTemplate.IfNewVersion, true))
                        _reasonUpdate = "Новая версия файла истории";
                    OpenHistoryRecs();
                }
            }
            catch (OutOfMemoryException) { }
            catch (Exception ex)
            {
                AddErrorAboutHistory(ex);
            }
        }

        //Файл истории и шаблон для него
        private readonly string _historyFile;
        private readonly string _historyTemplate;
        //База данных History
        protected DaoDb HistoryDb;
        //Строка с причиной создания нового файла истории
        private string _reasonUpdate;
        //Использовать SubHistory
        private readonly bool _useSubHistory;
        //Использовать ErrorsList
        private readonly bool _useErrorsList;

        //Рекордсет с таблицей SubHistory
        private RecDao _subHistory;
        //Рекордсет с таблицей History
        private RecDao _history;
        //Рекордсет с таблицей SuperHistory
        private RecDao _superHistory;
        //Рекордсет с таблицей ErrorsList
        private RecDao _errorsRec;

        //Логгер
        public Logg Logger { get; private set; }
        //Текущие команды записи в History и SubHistory
        internal CommLog CommandLog { get { return Logger.CommandLog; } }
        internal CommProgress CommandProgress { get { return Logger.CommandProgress; } }
        
        //Текущие Id истории
        private int _historyId;
        private int _superHistoryId;
        
        //Время логирования последнего события
        internal DateTime LogEventTime { private get; set; }
        //Разность времени между сейчас и временем предыдущего логирования
        public double FromEvent
        {
            get { return Math.Round(DateTime.Now.Subtract(LogEventTime).TotalSeconds, 2); }
        }
        
        //Сохранение старого файла истории и добавление нового
        public void UpdateHistory(bool openAfterUpdate)
        {
            try
            {
                if (_historyId > 300000)
                {
                    _reasonUpdate = "Старый файл истории содержит более 300000 записей";
                    _historyId = 0;
                }

                if (_reasonUpdate != null)
                {
                    Close();
                    Thread.Sleep(1500);
                    DaoDb.FromTemplate(_historyTemplate, _historyFile, ReplaceByTemplate.Always, true);
                    Thread.Sleep(1500);
                    if (openAfterUpdate) OpenHistoryRecs();
                }
            }
            catch(OutOfMemoryException) { }
            catch (Exception ex)
            {
                AddErrorAboutHistory(ex);
            }
        }

        //Открытие рекордсетов истории и добавление в историю первой записи после создания
        private void OpenHistoryRecs()
        {
            HistoryDb = new DaoDb(_historyFile);
            _history = new RecDao(HistoryDb, "History");
            _superHistory = new RecDao(HistoryDb, "SuperHistory");
            if (_useSubHistory) _subHistory = new RecDao(HistoryDb, "SubHistory");
            if (_useErrorsList) _errorsRec = new RecDao(HistoryDb, "ErrorsList");
            if (_reasonUpdate != null)
            {
                try
                {
                    var commLog = new CommLog(Logger, null, 0, 0, "Создание нового файла истории", "", _reasonUpdate);
                    WriteStart(commLog);
                    WriteFinish(commLog, "");
                    _reasonUpdate = null;
                }
                catch (OutOfMemoryException) { }
                catch (Exception ex)
                {
                    AddErrorAboutHistory(ex);
                }
                _historyId = 0;
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
            _reasonUpdate = ex.MessageString("Ошибка при работе с файлом истории");
            UpdateHistory(true);
        }

        public void WriteStartSuper(CommProgress command)
        {
            RunHistoryOperation(_superHistory, () =>
            {
                _superHistory.AddNew();
                _superHistory.Put("Command", CommandProgress.Name);
                _superHistory.Put("Params", CommandProgress.Params);
                if (Logger is LoggerTimed)
                {
                    var logger = (LoggerTimed) Logger;
                    _superHistory.Put("BeginPeriod", logger.BeginPeriod);
                    _superHistory.Put("EndPeriod", logger.EndPeriod);
                    _superHistory.Put("ModePeriod", logger.ModePeriod);    
                }
                _superHistory.Put("Status", CommandProgress.Status);
                _superHistory.Put("Time", CommandProgress.StartTime);
                _superHistoryId = _history.GetInt("HistoryId");
            });
        }

        public void WriteFinishSuper(CommProgress command, string results = null)
        {
            RunHistoryOperation(_superHistory, () =>
            {
                _superHistory.AddNew();
                _superHistory.Put("Status", CommandProgress.Status);
                _superHistory.Put("Results", results);
                _superHistory.Put("ProcessLength", CommandProgress.FromStart);
                _superHistoryId = 0;
            });
        }

        public void WriteStart(CommLog command)
        {
            LogEventTime = DateTime.Now;
            RunHistoryOperation(_history, () =>
            {
                _history.AddNew();
                if (CommandProgress != null)
                    _history.Put("SuperHistoryId", _superHistoryId);
                _history.Put("Command", CommandLog.Name);
                _history.Put("Params", CommandLog.Params);
                _history.Put("Status", CommandLog.Status);
                _history.Put("Time", CommandLog.StartTime);
                _history.Put("Context", CommandLog.Context, true);
                _historyId = _history.GetInt("HistoryId");
            });
        }
        
        public void WriteFinish(CommLog command, string results)
        {
            LogEventTime = DateTime.Now;
            RunHistoryOperation(_history, () =>
            {
                _history.MoveLast();
                _history.Put("Status", CommandLog.Status);
                _history.Put("Results", results);
                _history.Put("ProcessLength", CommandLog.FromStart);
                _historyId = 0;
            });
        }

        public void WriteEvent(string description, string pars)
        {
            RunHistoryOperation(_superHistory, () =>
            {
                _superHistory.AddNew();
                _superHistory.Put("Description", description);
                _superHistory.Put("Params", pars);
                _superHistory.Put("Time", DateTime.Now);
                _superHistory.Put("HistoryId", _historyId);
                _superHistory.Put("FromStart", FromEvent);
                LogEventTime = DateTime.Now;
            });
        }

        public void WriteError(ErrorCommand error)
        {
            RunHistoryOperation(_superHistory, () =>
            {
                _superHistory.AddNew();
                if (CommandLog != null) _superHistory.Put("HistoryId", _historyId);
                _superHistory.Put("Description", error.Text);
                _superHistory.Put("Params", error.ToLog());
                _superHistory.Put("Time", DateTime.Now);
                if (CommandLog != null)
                    _superHistory.Put("FromStart", FromEvent);
                _superHistory.Put("Status", error.Quality.ToRussian());
            });
        }

        public void WriteErrorToList(ErrorCommand error)
        {
            RunHistoryOperation(_errorsRec, () =>
            {
                _errorsRec.AddNew();
                _errorsRec.Put("Status", error.Quality.ToRussian());
                _errorsRec.Put("Description", error.Text);
                _errorsRec.Put("Params", error.ToLog());
                _errorsRec.Put("Time", DateTime.Now);
                if (CommandLog != null)
                {
                    _errorsRec.Put("Command", CommandLog.Name);
                    _errorsRec.Put("Context", CommandLog.Context);
                }
                if (Logger is LoggerTimed && CommandProgress != null)
                {
                    _errorsRec.Put("BeginPeriod", CommandProgress.BeginPeriod);
                    _errorsRec.Put("EndPeriod", CommandProgress.EndPeriod);
                }
            });
        }

        //Закрывает историю
        public void Close()
        {
            try
            {
                if (_subHistory != null) _subHistory.Dispose();
                if (_history != null) _history.Dispose();
                if (_superHistory != null) _superHistory.Dispose();
                if (_errorsRec != null) _errorsRec.Dispose();
                if (HistoryDb != null) HistoryDb.Dispose();
                _subHistory = null;
                _history = null;
                _superHistory = null;
                _errorsRec = null;
                HistoryDb = null;
            }
            catch { }
        }
    }
}