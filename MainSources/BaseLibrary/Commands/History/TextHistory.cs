using System;
using System.IO;
using System.Threading;

namespace BaseLibrary
{
    //История в текстовом файле
    public class TextHistory : IHistory
    {
        //Задание файла истории
        public TextHistory(Logger logger, //Ссылка на логгер
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
        private string _updateReason;

        //Вызывается, если при работе с историей произошла ошибка
        private void AddErrorAboutHistory(Exception ex)
        {
            _updateReason = ex.MessageString("Ошибка при работе с файлом истории");
            UpdateHistory();
        }

        //Открытие записи в историю
        private void OpenHistory()
        {
            _writer = new StreamWriter(_file, true);
            _errWriter = new StreamWriter(_file.Substring(0, _file.Length-4) + "Errors.txt", true);
            if (_updateReason != null)
            {
                try
                {
                    var commLog = new LogCommand(Logger, null, 0, 0, "Создание нового файла истории", "", _updateReason);
                    WriteStart(commLog);
                    WriteFinish(commLog, "");
                    _updateReason = null;
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
                    _updateReason = "Старый файл истории имеет размер более 100 МБ";

                if (_updateReason != null)
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

        public void WriteStartSuper(ProgressCommand command)
        {
            _writer.Write(command.Name + ", ");
            if (Logger.PeriodBegin != Different.MinDate)
                _writer.Write("Период: " + Logger.PeriodBegin + " - " + Logger.PeriodEnd + ", " + Logger.PeriodMode + ", ");
            _writer.WriteLine(command.StartTime);
        }

        public void WriteFinishSuper(ProgressCommand command, string results)
        {
            _writer.WriteLine(@"\" + command.Name + ", " + command.Status + ", Длительность: " + command.FromStart + (results.IsEmpty() ? "" : (", " + results)));
        }
        
        public void WriteStart(LogCommand command)
        {
            LogEventTime = DateTime.Now;
            _writer.Write(_shift + command.Name);
            if (!command.Context.IsEmpty())
                _writer.Write(", Контекст: " + command.Context);
            if (!command.Params.IsEmpty())
                _writer.Write(", " + command.Params);
            _writer.WriteLine(", " + command.StartTime);
        }

        public void WriteFinish(LogCommand command, string results)
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

        public void WriteError(CommandError error)
        {
            _writer.Write(_shift + "    " + error.Text + ", " + error.Quality.ToRussian());
            if (Logger.LogCommand != null)
                _writer.Write(", От предыдущего: " + FromEvent);
            _writer.WriteLine(", " + error.ToLog().Replace(Environment.NewLine, Environment.NewLine + _shift + "    ") + ", " + DateTime.Now);
        }

        public void WriteErrorToList(CommandError error)
        {
            _errWriter.WriteLine(error.Text + ", " + error.Quality.ToRussian() + ", " + DateTime.Now);
            if (!error.Params.IsEmpty() || error.Exeption != null) 
                _errWriter.WriteLine(error.ToLog());
            if (Logger.ProgressCommand != null && Logger.PeriodBegin != Different.MinDate)
                _errWriter.WriteLine(Logger.ProgressCommand.PeriodBegin + " - " + Logger.ProgressCommand.PeriodEnd + "  ");
            if (Logger.LogCommand != null)
                _errWriter.WriteLine(Logger.LogCommand.Name + ", " + Logger.LogCommand.Context);
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