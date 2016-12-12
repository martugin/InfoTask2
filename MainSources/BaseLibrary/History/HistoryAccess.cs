using System;
using System.Threading;

namespace BaseLibrary
{
    //История в файле Access
    public class HistoryAccess : IHistory
    {
        //Задание файла истории
        public HistoryAccess(string historyFile, //файл истории
                                        string historyTemplate, //шаблон для файла истории
                                        bool useSubHistory = false, //использовать SubHistory
                                        bool useErrorsList = true) //использовать ErrorsList
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
        internal RecDao SubHistory { get; private set; }
        //Рекордсет с таблицей History
        internal RecDao History { get; private set; }
        //Рекордсет с таблицей SuperHistory
        private RecDao _superHistory;
        //Рекордсет с таблицей ErrorsList
        protected RecDao ErrorsRec { get; set; }

        //Текущие команды записи в History и SubHistory
        internal CommLog CommandLog { get; set; }
        internal CommLog CommandSub { get; set; }
        
        //Текущие Id истории
        private int _historyId;
        private int _subHistoryId;
        
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
            catch (Exception ex)
            {
                AddErrorAboutHistory(ex);
            }
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
                    var commLog = new CommLog(new Logg(), null, 0, 0, "Создание нового файла истории", "", _reasonUpdate);
                    WriteStart(commLog);
                    WriteFinish(commLog);
                    _reasonUpdate = null;
                }
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

        public void WriteStartSuper(CommLog command)
        {
            throw new System.NotImplementedException();
        }

        public void WriteFinishSuper(CommLog command, string results = null)
        {
            throw new System.NotImplementedException();
        }

        public void WriteStart(CommLog command)
        {
            LogEventTime = DateTime.Now;
            RunHistoryOperation(History, () =>
            {
                History.AddNew();
                if (CommandSub != null)
                    History.Put("SubHistoryId", _subHistoryId);
                History.Put("Command", CommandLog.Name);
                History.Put("Context", CommandLog.Context, true);
                History.Put("Params", CommandLog.Params);
                History.Put("Time", CommandLog.StartTime);
                History.Put("Status", CommandLog.Status);
                _historyId = History.GetInt("HistoryId");
            });
        }
        
        public void WriteFinish(CommLog command, string results = null)
        {
            LogEventTime = DateTime.Now;
            RunHistoryOperation(History, () =>
            {
                History.MoveLast();
                History.Put("Status", CommandLog.Status);
                History.Put("Params", new[] { CommandLog.Params, results }.ToPropertyString());
                History.Put("ProcessLength", CommandLog.FromStart);
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
            RunHistoryOperation(ErrorsRec, () =>
            {
                ErrorsRec.AddNew();
                ErrorsRec.Put("Status", error.Quality.ToRussian());
                ErrorsRec.Put("Description", error.Text);
                ErrorsRec.Put("Params", error.ToLog());
                ErrorsRec.Put("Time", DateTime.Now);
                if (CommandLog != null)
                {
                    ErrorsRec.Put("Command", CommandLog.Name);
                    ErrorsRec.Put("Context", CommandLog.Context);
                }
                if (CommandSub != null)
                    ErrorsRec.Put("CommandParams", CommandSub.Params);
            });
        }

        //Закрывает историю
        public void Close()
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
    }
}