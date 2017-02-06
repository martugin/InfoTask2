using System;
using System.Drawing;
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
            SubscribeIndicatorEvents();
        }

        //Логгер
        protected Logger Logger { get; private set; }

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

        //Завершение комманды
        public void Finish(string results = null)
        {
            Logger.Finish(results);
        }

        //Запускает команду и возвращает строку с сообщением ошибки или ""
        protected string RunClientCommand(Action action, string errMess = "Ошибка выполнения команды")
        {
            return ((CommandCollect)Logger.StartCollect(false, true).Run(action)).ErrorMessage(true, false);
        }

        //Работа с формой индикатора
        #region
        //Подписка на события индикатора
        private void SubscribeIndicatorEvents()
        {
            Logger.ShowIndicatorTexted += OnShowIndicatorTexted;
            Logger.ShowIndicatorTimed += OnShowIndicatorTimed;
            Logger.HideIndicator += OnHideIndicator;
            Logger.ChangeProcent += OnChangeProcent;
            Logger.ChangeTabloText += OnChangeTabloText;
            Logger.ChangePeriod += OnChangePeriod;
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
            var screen = Screen.PrimaryScreen;
            form.Location = new Point(screen.WorkingArea.Width - form.Width - 1, screen.WorkingArea.Height - form.Height - 2);
            form.Show();
            form.Text = "InfoTask";
            form.Refresh();
        }

        //Обработка события скрыть форму индикатора
        private void OnHideIndicator(object sender, EventArgs e)
        {
            if (_formTimed != null)
                _formTimed.Invoke(new FormDelegate(_formTimed.Hide));
            else if (_formTexted != null)
                _formTexted.Invoke(new FormDelegate(_formTexted.Hide));
        }

        //Обработка события изменения уровня индикатора
        private void OnChangeProcent(object sender, ChangeProcentEventArgs e)
        {
            int p = Convert.ToInt32(e.Procent);
            if (_formTimed != null)
                _formTimed.Invoke(new FormDelegate(() => { _formTimed.Procent.Value = p; }));
            else if (_formTexted != null)
                _formTexted.Invoke(new FormDelegate(() => { _formTexted.Procent.Value = p; }));
        }

        //Обработка события изменения текста на табло
        private void OnChangeTabloText(object sender, ChangeTabloTextEventArgs e)
        {
            if (_formTimed != null)
                _formTimed.Invoke(new FormDelegate(() =>
                {
                    _formTimed.Text1.Text = e.TabloText[1];
                    _formTimed.Text2.Text = e.TabloText[2];
                }));
            else if (_formTexted != null)
            {
                _formTexted.Invoke(new FormDelegate(() =>
                {
                    _formTexted.Text0.Text = e.TabloText[0];
                    _formTexted.Text1.Text = e.TabloText[1];
                    _formTexted.Text2.Text = e.TabloText[2];
                }));
            }
        }

        //Обработка события изменения периода обработки
        private void OnChangePeriod(object sender, ChangePeriodEventArgs e)
        {
            if (_formTimed != null)
                _formTimed.Invoke(new FormDelegate(() =>
                {
                    _formTimed.PeriodBegin.Text = e.BeginPeriod.ToString();
                    _formTimed.PeriodEnd.Text = e.EndPeriod.ToString();
                }));
        }
        #endregion
    }
}