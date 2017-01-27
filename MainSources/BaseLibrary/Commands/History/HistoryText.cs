using System;
using System.IO;
using System.Threading;

namespace BaseLibrary
{
    //История в текстовом файле
    public class HistoryText : IHistory
    {
        //Задание файла истории
        public HistoryText(Logger logger, //Ссылка на логгер
                                     string historyFile, //Файл истории
                                     bool useSuperHistory) //Разрешить команды уровня Progress
        {
            try
            {
                Logger = logger;
                _file = historyFile;
                _shift = useSuperHistory ? "    " : "";
                OpenHistory();
            }
            catch (OutOfMemoryException) { }
            catch (Exception ex)
            {
                AddErrorAboutHistory(ex);
            }
        }
        
        //Логгер
        public Logger Logger { get; private set; }
        //Файл истории
        private readonly string _file;
        //Если команды уровня Progress разрешены, то сдвиг на 4 пробела
        private readonly string _shift;
        //Потоки записи истории и ошибок
        private StreamWriter _writer;
        private StreamWriter _errWriter;

        //Время логирования последнего события
        internal DateTime LogEventTime { private get; set; }
        //Разность времени между сейчас и временем предыдущего логирования
        public double FromEvent
        {
            get { return Math.Round(DateTime.Now.Subtract(LogEventTime).TotalSeconds, 2); }
        }

        //Строка с причиной создания нового файла истории
        private string _reasonUpdate;

        //Вызывается, если при работе с историей произошла ошибка
        private void AddErrorAboutHistory(Exception ex)
        {
            _reasonUpdate = ex.MessageString("Ошибка при работе с файлом истории");
            UpdateHistory();
        }

        //Открытие записи в историю
        private void OpenHistory()
        {
            _writer = new StreamWriter(_file, true);
            _errWriter = new StreamWriter(_file.Substring(0, _file.Length-4) + "Errors.txt", true);
            if (_reasonUpdate != null)
            {
                try
                {
                    var commLog = new CommandLog(Logger, null, 0, 0, "Создание нового файла истории", "", _reasonUpdate);
                    WriteStart(commLog);
                    WriteFinish(commLog, "");
                    _reasonUpdate = null;
                }
                catch (OutOfMemoryException) { }
                catch (Exception ex)
                {
                    AddErrorAboutHistory(ex);
                }
            }
        }

        //Сохранение старого файла истории и добавление нового, при достижении размера или при ошибке
        public void UpdateHistory()
        {
            try
            {
                if (new FileInfo(_file).Length > 100000000)
                    _reasonUpdate = "Старый файл истории имеет размер более 100 МБ";

                if (_reasonUpdate != null)
                {
                    Close();
                    Thread.Sleep(500);

                    bool b = true;
                    string s = _file.Substring(0, _file.Length - 4);
                    int i = 1;
                    string ss = "";
                    while (b && i < 1000)
                    {
                        ss = s + "_" + (i++) + ".txt";
                        b = new FileInfo(ss).Exists;
                    }
                    if (i <= 1000) new FileInfo(_file).MoveTo(ss);
                    else new FileInfo(_file).Delete();
                    
                    Thread.Sleep(500);
                    OpenHistory();
                }
            }
            catch (OutOfMemoryException) { }
            catch (Exception ex)
            {
                AddErrorAboutHistory(ex);
            }
        }

        public void WriteStartSuper(CommandProgress command)
        {
            _writer.Write(command.Name + ", ");
            if (Logger is LoggerTimed)
            {
                var logger = (LoggerTimed)Logger;
                _writer.Write("Период: " + logger.BeginPeriod + " - " + logger.EndPeriod + ", " + logger.ModePeriod + ", ");
            }
            _writer.WriteLine(command.StartTime);
        }

        public void WriteFinishSuper(CommandProgress command, string results)
        {
            _writer.WriteLine(@"\" + command.Name + ", " + command.Status + ", Длительность: " + command.FromStart + (results.IsEmpty() ? "" : (", " + results)));
        }
        
        public void WriteStart(CommandLog command)
        {
            LogEventTime = DateTime.Now;
            _writer.Write(_shift + command.Name);
            if (!command.Context.IsEmpty())
                _writer.Write(", Контекст: " + command.Context);
            if (!command.Params.IsEmpty())
                _writer.Write(", " + command.Params);
            _writer.WriteLine(", " + command.StartTime);
        }

        public void WriteFinish(CommandLog command, string results)
        {
            LogEventTime = DateTime.Now;
            _writer.WriteLine(_shift + @"\" + command.Name + ", " + command.Status + ", Длительность: " + command.FromStart + (results.IsEmpty() ? "" : (", " + results)));
        }

        public void WriteEvent(string description, string pars)
        {
            _writer.Write(_shift + "    " + description + ", От предыдущего: " + FromEvent);
            if (!pars.IsEmpty()) _writer.Write(", " + pars);
            _writer.WriteLine(", " + DateTime.Now);
        }

        public void WriteError(ErrorCommand error)
        {
            _writer.Write(_shift + "    " + error.Text + ", " + error.Quality.ToRussian());
            if (Logger.CommandLog != null)
                _writer.Write(", От предыдущего: " + FromEvent);
            _writer.WriteLine(", " + error.ToLog().Replace(Environment.NewLine, Environment.NewLine + _shift + "    ") + ", " + DateTime.Now);
        }

        public void WriteErrorToList(ErrorCommand error)
        {
            _errWriter.WriteLine(error.Text + ", " + error.Quality.ToRussian() + ", " + DateTime.Now);
            if (!error.Params.IsEmpty() || error.Exeption != null) 
                _errWriter.WriteLine(error.ToLog());
            if (Logger is LoggerTimed && Logger.CommandProgress != null)
                _errWriter.WriteLine(Logger.CommandProgress.BeginPeriod + " - " + Logger.CommandProgress.EndPeriod + "  ");
            if (Logger.CommandLog != null)
                _errWriter.WriteLine(Logger.CommandLog.Name + ", " + Logger.CommandLog.Context);
            _errWriter.WriteLine();
        }

        public void ClearErrorsList()
        {
            new FileInfo(_file.Substring(0, _file.Length - 4) + "Errors.txt").Delete();
        }

        //Закрывает историю
        public void Close()
        {
            try
            {
                _writer.Close();
                _errWriter.Close();
                _writer = null;
                _errWriter = null;
            }
            catch {}
        }
    }
}