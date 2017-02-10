using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using BaseLibrary;

namespace ComClients
{
    //Оболочка логгера для вызова через COM
    public abstract class LoggerClient
    {
        protected LoggerClient()
        {
            Logger = new Logger();
            SubscribeEvents();
        }

        //Закрытие клиента
        public void Close()
        {
            try
            {
                if (Logger.History != null)
                    Logger.History.Close();
                UnsubscribeEvents();
            }
            catch { }
            Thread.Sleep(100);
            GC.Collect();
            IsClosed = true;
        }

        //Логгер
        internal protected Logger Logger { get; private set; }
        //Клиент уже был закрыт
        protected bool IsClosed { get; private set; }

        //Добавить событие в историю
        public void AddEvent(string text, //Описание
                                        string pars = "") //Дополнительная информация
        {
            Logger.AddEvent(text, pars);
        }

        //Добавить предупреждение в историю
        public void AddWarning(string text, //Описание
                                            string par = "") //Дополнительная информация
        {
            Logger.AddWarning(text, null, par);
        }

        //Добавить предупреждение в историю
        public void AddError(string text, //Описание
                                        string par = "") //Дополнительная информация
        {
            Logger.AddError(text, null, par);
        }

        //Запуск команды для записи в History
        public void StartLog(string name,  //Имя команды
                                       string pars = "",  //Дополнительная информация
                                       string context = "") //Контекст выполнения команды
        {
            Logger.StartLog(name, pars, context);
        }
        public void StartLog(double startProcent, double finishProcent, string name, string pars = "", string context = "") 
        {
            Logger.StartLog(startProcent, finishProcent, name, pars, context);
        }

        //Запуск команды для записи в SuperHistory
        public void StartProgress(string name,  //Имя команды
                                               string pars = "",  //Дополнительная информация
                                               string text = "") //Текст для отображения на индикаторе
        {
            Logger.StartProgress(text, name, pars);
        }
        public void StartProgress(string name,  //Имя команды
                                               string pars,  //Дополнительная информация
                                               DateTime beg, DateTime en) //Период расчета
        {
            Logger.StartProgress(beg, en, name, pars);
        }

        //Запуск команды, отображающей на форме индикатора текст 2-ого уровня
        public void StartProgressText(string text)
        {
            Logger.StartProgressText(text);
        }
        public void StartProgressText(double startProcent, double finishProcent, string text)
        {
            Logger.StartProgressText(startProcent, finishProcent, text);
        }

        //Завершение комманды
        public void Finish(string results = null)
        {
            Logger.Finish(results);
        }

        //Установить процент текущей комманды
        public void SetProcent(double procent)
        {
            Logger.Procent = procent;
        }

        //Запускает команду и возвращает строку с сообщением ошибки или ""
        protected string RunClientCommand(Action action, string errMess = "Ошибка выполнения команды")
        {
            return ((CommandCollect)Logger.StartCollect(false, true).Run(action)).ErrorMessage(true, false);
        }

        //Работа с формой индикатора
        #region
        //Подписка на события индикатора
        private void SubscribeEvents()
        {
            Logger.ShowIndicatorTexted += OnShowIndicatorTexted;
            Logger.ShowIndicatorTimed += OnShowIndicatorTimed;
            Logger.HideIndicator += OnHideIndicator;
            Logger.ChangeProcent += OnChangeProcent;
            Logger.ChangeTabloText += OnChangeTabloText;
            Logger.ChangePeriod += OnChangePeriod;

            Logger.ExecutionFinished += OnExecutionFinished;
        }

        private void UnsubscribeEvents()
        {
            Logger.ShowIndicatorTexted -= OnShowIndicatorTexted;
            Logger.ShowIndicatorTimed -= OnShowIndicatorTimed;
            Logger.HideIndicator -= OnHideIndicator;
            Logger.ChangeProcent -= OnChangeProcent;
            Logger.ChangeTabloText -= OnChangeTabloText;
            Logger.ChangePeriod -= OnChangePeriod;

            Logger.ExecutionFinished -= OnExecutionFinished;
        }

        //Вызов обновлений формы
        private void Invoke(Form form, Action action)
        {
            if (form != null)
                form.Invoke(new FormDelegate(() => { action(); form.Refresh(); }));
        }
        private delegate void FormDelegate();
        
        //Форма индикатора с указанием периода
        private IndicatorFormTimed _formTimed;
        //Форма индикатора с текстом
        private IndicatorFormTexted _formTexted;

        //Обработка событий показать форму индикатора
        private void OnShowIndicatorTexted(object sender, EventArgs e)
        {
            if (_formTexted == null)
                _formTexted = new IndicatorFormTexted();
            ShowIndicatorForm(_formTexted);
        }
        private void OnShowIndicatorTimed(object sender, EventArgs e)
        {
            if (_formTimed == null)
                _formTimed = new IndicatorFormTimed();
            ShowIndicatorForm(_formTimed);
        }
        //Высвечивает форму индикатора
        private void ShowIndicatorForm(Form form)
        {
            Application.EnableVisualStyles();
            form.Show();
            Invoke(form, () =>
            {
                var screen = Screen.PrimaryScreen;
                form.Location = new Point(screen.WorkingArea.Width - form.Width - 1, screen.WorkingArea.Height - form.Height - 2);
                form.Text = "InfoTask";
            });
        }

        //Обработка события скрыть форму индикатора
        private void OnHideIndicator(object sender, EventArgs e)
        {
            Invoke(_formTimed, () => _formTimed.Hide());
            Invoke(_formTexted, () => _formTexted.Hide());
        }

        //Обработка события изменения уровня индикатора
        private void OnChangeProcent(object sender, ChangeProcentEventArgs e)
        {
            int p = Convert.ToInt32(e.Procent);
            Invoke(_formTimed, () => { _formTimed.Procent.Value = p; });
            Invoke(_formTexted, () => { _formTexted.Procent.Value = p; });
        }

        //Обработка события изменения текста на табло
        private void OnChangeTabloText(object sender, ChangeTabloTextEventArgs e)
        {
            Invoke(_formTimed, () =>
                {
                    _formTimed.Text1.Text = e.TabloText[1];
                    _formTimed.Text2.Text = e.TabloText[2];
                });
            Invoke(_formTexted, () =>
                {
                    _formTexted.Text0.Text = e.TabloText[0];
                    _formTexted.Text1.Text = e.TabloText[1];
                    _formTexted.Text2.Text = e.TabloText[2];
                });
        }

        //Обработка события изменения периода обработки
        private void OnChangePeriod(object sender, ChangePeriodEventArgs e)
        {
            Invoke(_formTimed, () =>
                {
                    _formTimed.PeriodBegin.Text = e.BeginPeriod.ToString();
                    _formTimed.PeriodEnd.Text = e.EndPeriod.ToString();
                });
        }

        #endregion

        //Событие, сообщающее внешнему приложению, что выполнение было прервано
        public delegate void EvDelegate();
        public event EvDelegate Finished;

        //Прервать выполнение
        public void Break()
        {
            Logger.Break();
        }

        //Обработка события прерывания
        private void OnExecutionFinished(object sender, EventArgs e)
        {
            if (Finished != null) Finished();
        }

        public void TestMethod()
        {
            var t = new Thread((StartProcess));
            t.Start();
        }

        private void StartProcess()
        {
            Logger.StartCollect(false, false).Run(() =>
            {
                StartProgress("P", "P", "Процесс");
                StartLog(0, 30, "11111111");
                Thread.Sleep(1000);
                SetProcent(33);
                Thread.Sleep(1000);
                SetProcent(67);
                Thread.Sleep(1000);
                StartLog(30, 70, "22222222");
                Thread.Sleep(1000);
                SetProcent(33);
                Thread.Sleep(1000);
                SetProcent(67);
                Thread.Sleep(1000);
                StartLog(70, 100, "33333333");
                Thread.Sleep(1000);
                SetProcent(33);
                Thread.Sleep(1000);
                SetProcent(67);
                Thread.Sleep(1000);
            });
        }
    }
}