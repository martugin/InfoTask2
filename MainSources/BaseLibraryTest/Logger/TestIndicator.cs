using System;
using System.Collections.Generic;
using BaseLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseLibraryTest
{
    //Тестовый индикатор процесса
    internal class TestIndicator : IIndicator
    {
        //Список событий, произошедших с индикатором
        private readonly List<Tuple<string, string>> _events = new List<Tuple<string, string>>();
        public List<Tuple<string, string>> Events { get { return _events; } }

        private void AddEvent(string ev, string text = "")
        {
            _events.Add(new Tuple<string, string>(ev, text));
        }

        //Текущий номер для проверки
        private int _curNum;
        //Проверка события на правильность
        internal void Compare(string ev, string text = "")
        {
            Assert.IsTrue(_events.Count > _curNum);
            Assert.AreEqual(ev, _events[_curNum].Item1);
            Assert.AreEqual(text, _events[_curNum].Item2);
            _curNum++;
        }

        public bool TextedIndicatorIsVisible { get; private set; }
        public void ShowTextedIndicator()
        {
            TimedIndicatorIsVisible = false;
            TextedIndicatorIsVisible = true;
            AddEvent("ShowTexted");
        }

        public bool TimedIndicatorIsVisible { get; private set; }
        public void ShowTimedIndicator()
        {
            TextedIndicatorIsVisible = false;
            TimedIndicatorIsVisible = true;
            AddEvent("ShowTimed");
        }

        public void HideIndicator()
        {
            TextedIndicatorIsVisible = false;
            TimedIndicatorIsVisible = false;
            AddEvent("HideIndicator");
        }

        public double Procent { get; private set; }
        public void ChangeProcent(double procent)
        {
            Procent = procent;
            AddEvent("Procent", procent.ToString());
        }

        public DateTime TimedProcessEndTime { get; private set; }
        public bool ProcessIsTimed { get; private set; }
        public void SetTimedProcess(DateTime endTime)
        {
            TimedProcessEndTime = endTime;
            ProcessIsTimed = true;
            AddEvent("ProcessTimed", endTime.ToString());
        }

        public void SetProcessUsual()
        {
            TimedProcessEndTime = Static.MinDate;
            ProcessIsTimed = false;
            AddEvent("ProcessUsual");
        }

        public string TabloText0 { get; private set; }
        public string TabloText1 { get; private set; }
        public string TabloText2 { get; private set; }
        public void ChangeTabloText(int num, string text)
        {
            if (num == 0)
            {
                TabloText0 = text;
                AddEvent("Text0", text);
            }
            if (num == 1)
            {
                TabloText1 = text;
                AddEvent("Text1", text);
            }
            if (num == 2)
            {
                TabloText2 = text;
                AddEvent("Text2", text);
            }
        }

        public DateTime PeriodBegin { get; private set; }
        public DateTime PeriodEnd { get; private set; }
        public string PeriodMode { get; private set; }
        public void ChangePeriod(DateTime periodBegin, DateTime periodEnd, string periodMode)
        {
            PeriodBegin = periodBegin;
            AddEvent("PeriodBegin", periodBegin.ToString());
            PeriodEnd = periodEnd;
            AddEvent("PeriodEnd", periodEnd.ToString());
            PeriodMode = periodMode;
            AddEvent("PeriodMode", periodMode);
        }
    }
}
