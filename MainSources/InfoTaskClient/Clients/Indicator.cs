using System;
using System.Drawing;
using System.Windows.Forms;
using BaseLibrary;

namespace ComClients
{
    //Интерфейс для отображения индикатора
    internal interface IIndicator
    {
        //Обработчики событий отображения индикатора
        void OnShowTextedIndicator(object sender, EventArgs e);
        void OnShowTimedIndicator(object sender, EventArgs e);
        void OnHideIndicator(object sender, EventArgs e);
        void OnChangeProcent(object sender, ChangeProcentEventArgs e);
        void OnSetProcessTimed(object sender, SetProcentTimedEventArgs e);
        void OnSetProcessUsual(object sender, EventArgs e);
        void OnChangeTabloText(object sender, ChangeTabloTextEventArgs e);
        void OnChangePeriod(object sender, ChangePeriodEventArgs e);
    }

    //Оболочка логгера с отображением индикатора
    internal class Indicator : IIndicator
    {
        //Вызов обновлений формы
        private void Invoke(Form form, Action action)
        {
            if (form != null)
                form.Invoke(new FormDelegate(() => { action(); form.Refresh(); }));
        }
        private delegate void FormDelegate();

        //Форма индикатора с указанием периода
        private TimedIndicatorForm _timedForm;
        //Форма индикатора с текстом
        private TextedIndicatorForm _textedForm;

        //Обработка событий показать форму индикатора
        public void OnShowTextedIndicator(object sender, EventArgs e)
        {
            if (_textedForm == null)
                _textedForm = new TextedIndicatorForm();
            ShowIndicatorForm(_textedForm);
        }
        public void OnShowTimedIndicator(object sender, EventArgs e)
        {
            if (_timedForm == null)
                _timedForm = new TimedIndicatorForm();
            ShowIndicatorForm(_timedForm);
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
        public void OnHideIndicator(object sender, EventArgs e)
        {
            Invoke(_timedForm, () => _timedForm.Hide());
            Invoke(_textedForm, () => _textedForm.Hide());
        }

        //Обработка события изменения уровня индикатора
        public void OnChangeProcent(object sender, ChangeProcentEventArgs e)
        {
            int p = Convert.ToInt32(e.Procent);
            Invoke(_timedForm, () => { _timedForm.Procent.Value = p; });
            Invoke(_textedForm, () => { _textedForm.Procent.Value = p; });
        }

        public void OnSetProcessTimed(object sender, SetProcentTimedEventArgs e) {}
        public void OnSetProcessUsual(object sender, EventArgs e) {}

        //Обработка события изменения текста на табло
        public void OnChangeTabloText(object sender, ChangeTabloTextEventArgs e)
        {
            Invoke(_timedForm, () =>
            {
                _timedForm.Text1.Text = e.TabloText[1];
                _timedForm.Text2.Text = e.TabloText[2];
            });
            Invoke(_textedForm, () =>
            {
                _textedForm.Text0.Text = e.TabloText[0];
                _textedForm.Text1.Text = e.TabloText[1];
                _textedForm.Text2.Text = e.TabloText[2];
            });
        }

        //Обработка события изменения периода обработки
        public void OnChangePeriod(object sender, ChangePeriodEventArgs e)
        {
            Invoke(_timedForm, () =>
            {
                _timedForm.PeriodBegin.Text = e.BeginPeriod.ToString();
                _timedForm.PeriodEnd.Text = e.EndPeriod.ToString();
            });
        }
    }
}