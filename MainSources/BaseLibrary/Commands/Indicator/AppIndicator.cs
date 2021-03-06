﻿using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace BaseLibrary
{
    //Индикатор с отображением формы в углу экрана
    public class AppIndicator : IIndicator
    {
        //Вызов обновлений формы
        private static void Invoke(Form form, Action action)
        {
            if (form != null)
            {
                form.Invoke(new FormDelegate(() => { action(); form.Refresh(); }));
                //Thread.Sleep(100);
                //form.Invoke(new FormDelegate(() => { action(); form.Refresh(); }));
            }
        }
        private delegate void FormDelegate();

        //Форма индикатора с указанием периода
        private TimedIndicatorForm _timedForm;
        //Форма индикатора с текстом
        private TextedIndicatorForm _textedForm;

        //Высвечивает форму индикатора
        private static void ShowIndicatorForm(Form form)
        {
            Thread.Sleep(20);
            Application.EnableVisualStyles();
            form.Show();
            Invoke(form, () =>
            {
                var screen = Screen.PrimaryScreen;
                form.Location = new Point(screen.WorkingArea.Width - form.Width - 1, screen.WorkingArea.Height - form.Height - 2);
                form.Text = "InfoTask";
            });
        }

        //Обработка событий показать форму индикатора
        public void ShowTextedIndicator()
        {
            _textedForm = new TextedIndicatorForm();
            ShowIndicatorForm(_textedForm);
        }
        public void ShowTimedIndicator()
        {
            _timedForm = new TimedIndicatorForm();
            ShowIndicatorForm(_timedForm);
        }

        //Обработка события скрыть форму индикатора
        public void HideIndicator()
        {
            Thread.Sleep(20);
            if (_timedForm != null)
                Invoke(_timedForm, () => _timedForm.Hide());
            if (_textedForm != null)
                Invoke(_textedForm, () => _textedForm.Hide());
        }

        //Текущий процент индикатора
        public double Procent { get; private set; }

        //Обработка события изменения уровня индикатора
        public void ChangeProcent(double procent)
        {
            Procent = procent;
            int p = Convert.ToInt32(procent);
            Invoke(_timedForm, () =>
            {
                _timedForm.Procent.Value = p <= 90 ? 10 + p : p; 
            });
            Invoke(_textedForm, () =>
            {
                _textedForm.Procent.Value = p <= 90 ? 10 + p : p; 
            });
        }

        public void SetTimedProcess(DateTime endTime) { }
        public void SetProcessUsual() { }

        //Обработка события изменения текста на табло
        public void ChangeTabloText(int num, string text)
        {
            Invoke(_timedForm, () =>
            {
                if (num == 1) _timedForm.Text1.Text = text;
                if (num == 2) _timedForm.Text2.Text = text;
            });
            Invoke(_textedForm, () =>
            {
                if (num == 0) _textedForm.Text0.Text = text;
                if (num == 1) _textedForm.Text1.Text = text;
                if (num == 2) _textedForm.Text2.Text = text;
            });
        }

        //Обработка события изменения периода обработки
        public void ChangePeriod(DateTime periodBegin, DateTime periodEnd, string periodMode)
        {
            Invoke(_timedForm, () =>
            {
                _timedForm.PeriodBegin.Text = periodBegin.ToString();
                _timedForm.PeriodEnd.Text = periodEnd.ToString();
            });
        }
    }
}