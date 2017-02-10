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
        void OnShowIndicatorTexted(object sender, EventArgs e);
        void OnShowIndicatorTimed(object sender, EventArgs e);
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
        private IndicatorFormTimed _formTimed;
        //Форма индикатора с текстом
        private IndicatorFormTexted _formTexted;

        //Обработка событий показать форму индикатора
        public void OnShowIndicatorTexted(object sender, EventArgs e)
        {
            if (_formTexted == null)
                _formTexted = new IndicatorFormTexted();
            ShowIndicatorForm(_formTexted);
        }
        public void OnShowIndicatorTimed(object sender, EventArgs e)
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
        public void OnHideIndicator(object sender, EventArgs e)
        {
            Invoke(_formTimed, () => _formTimed.Hide());
            Invoke(_formTexted, () => _formTexted.Hide());
        }

        //Обработка события изменения уровня индикатора
        public void OnChangeProcent(object sender, ChangeProcentEventArgs e)
        {
            int p = Convert.ToInt32(e.Procent);
            Invoke(_formTimed, () => { _formTimed.Procent.Value = p; });
            Invoke(_formTexted, () => { _formTexted.Procent.Value = p; });
        }

        public void OnSetProcessTimed(object sender, SetProcentTimedEventArgs e) {}
        public void OnSetProcessUsual(object sender, EventArgs e) {}

        //Обработка события изменения текста на табло
        public void OnChangeTabloText(object sender, ChangeTabloTextEventArgs e)
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
        public void OnChangePeriod(object sender, ChangePeriodEventArgs e)
        {
            Invoke(_formTimed, () =>
            {
                _formTimed.PeriodBegin.Text = e.BeginPeriod.ToString();
                _formTimed.PeriodEnd.Text = e.EndPeriod.ToString();
            });
        }
    }
}