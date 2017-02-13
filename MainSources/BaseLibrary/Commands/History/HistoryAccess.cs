using System;
using System.Threading;

namespace BaseLibrary
{
    //История в файле Access
    public class HistoryAccess : IHistory
    {
        //Задание файла истории
        public HistoryAccess(Logger logger, //ссылка на логгер
                                        string historyFile, //файл истории
                                        string historyTemplate) //шаблон для файла истории
        {
            try
            {
                Logger = logger;
                _historyFile = historyFile;
                _historyTemplate = historyTemplate;
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

        //Рекордсет с таблицей SubHistory
        private RecDao _subHistory;
        //Рекордсет с таблицей History
        private RecDao _history;
        //Рекордсет с таблицей SuperHistory
        private RecDao _superHistory;
        //Рекордсет с таблицей ErrorsList
        private RecDao _errorsRec;

        //Логгер
        public Logger Logger { get; private set; }
        //Текущие команды записи в History и SubHistory
        internal CommandLog CommandLog { get { return Logger.CommandLog; } }
        internal CommandProgress CommandProgress { get { return Logger.CommandProgress; } }
        
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
        public void UpdateHistory()
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
                    OpenHistoryRecs();
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
            _subHistory = new RecDao(HistoryDb, "SubHistory");
            _history = new RecDao(HistoryDb, "History");
            _superHistory = new RecDao(HistoryDb, "SuperHistory");
            _errorsRec = new RecDao(HistoryDb, "ErrorsList");
            if (_reasonUpdate != null)
            {
                try
                {
                    var commLog = new CommandLog(Logger, null, 0, 0, "Создание нового файла истории", "",  _reasonUpdate);
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
            UpdateHistory();
        }

        public void WriteStartSuper(CommandProgress command)
        {
            RunHistoryOperation(_superHistory, () =>
            {
                _superHistory.AddNew();
                _superHistory.Put("Command", command.Name);
                _superHistory.Put("Params", command.Params);
                if (Logger.BeginPeriod != Different.MinDate)
                {
                    _superHistory.Put("BeginPeriod", Logger.BeginPeriod);
                    _superHistory.Put("EndPeriod", Logger.EndPeriod);
                    _superHistory.Put("ModePeriod", Logger.ModePeriod);    
                }
                _superHistory.Put("Status", command.Status);
                _superHistory.Put("Time", command.StartTime);
                _superHistoryId = _superHistory.GetInt("SuperHistoryId");
            });
        }

        public void WriteFinishSuper(CommandProgress command, string results = null)
        {
            RunHistoryOperation(_superHistory, () =>
            {
                _superHistory.MoveLast();
                _superHistory.Put("Status", command.Status);
                _superHistory.Put("Results", results);
                _superHistory.Put("ProcessLength", command.FromStart);
                _superHistoryId = 0;
            });
        }

        public void WriteStart(CommandLog command)
        {
            LogEventTime = DateTime.Now;
            RunHistoryOperation(_history, () =>
            {
                _history.AddNew();
                if (CommandProgress != null)
                    _history.Put("SuperHistoryId", _superHistoryId);
                _history.Put("Command", command.Name);
                _history.Put("Params", command.Params);
                _history.Put("Status", command.Status);
                _history.Put("Time", command.StartTime);
                _history.Put("Context", command.Context, true);
                _historyId = _history.GetInt("HistoryId");
            });
        }
        
        public void WriteFinish(CommandLog command, string results)
        {
            LogEventTime = DateTime.Now;
            RunHistoryOperation(_history, () =>
            {
                _history.MoveLast();
                _history.Put("Status", command.Status);
                _history.Put("Results", results);
                _history.Put("ProcessLength", command.FromStart);
                _historyId = 0;
            });
        }

        public void WriteEvent(string description, string pars)
        {
            RunHistoryOperation(_subHistory, () =>
            {
                _subHistory.AddNew();
                _subHistory.Put("Description", description);
                _subHistory.Put("Params", pars);
                _subHistory.Put("Time", DateTime.Now);
                _subHistory.Put("HistoryId", _historyId);
                _subHistory.Put("FromStart", FromEvent);
                LogEventTime = DateTime.Now;
            });
        }

        public void WriteError(ErrorCommand error)
        {
            RunHistoryOperation(_superHistory, () =>
            {
                _subHistory.AddNew();
                if (CommandLog != null) _subHistory.Put("HistoryId", _historyId);
                _subHistory.Put("Description", error.Text);
                _subHistory.Put("Params", error.ToLog());
                _subHistory.Put("Time", DateTime.Now);
                if (CommandLog != null)
                    _subHistory.Put("FromStart", FromEvent);
                _subHistory.Put("Status", error.Quality.ToRussian());
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
                if (Logger.BeginPeriod != Different.MinDate && CommandProgress != null)
                {
                    _errorsRec.Put("BeginPeriod", CommandProgress.BeginPeriod);
                    _errorsRec.Put("EndPeriod", CommandProgress.EndPeriod);
                }
            });
        }

        public void ClearErrorsList()
        {
            HistoryDb.Execute("DELEETE * FROM ErrorsList");
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