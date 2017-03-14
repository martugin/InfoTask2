using System;
using BaseLibrary;

namespace InfoTaskClientTest
{
    //Тестовый индикатор процесса
    internal class TestIndicator : IIndicator
    {
        public bool TextedIndicatorIsVisible { get; private set; }
        public void ShowTextedIndicator()
        {
            TimedIndicatorIsVisible = false;
            TextedIndicatorIsVisible = true;
        }

        public bool TimedIndicatorIsVisible { get; private set; }
        public void ShowTimedIndicator()
        {
            TextedIndicatorIsVisible = false;
            TimedIndicatorIsVisible = true;
        }

        public void HideIndicator()
        {
            TextedIndicatorIsVisible = false;
            TimedIndicatorIsVisible = false;
        }

        public double Procent { get; private set; }
        public void ChangeProcent(double procent)
        {
            Procent = procent;
        }

        public DateTime TimedProcessEndTime { get; private set; }
        public bool ProcessIsTimed { get; private set; }
        public void SetTimedProcess(DateTime endTime)
        {
            TimedProcessEndTime = endTime;
            ProcessIsTimed = true;
        }

        public void SetProcessUsual()
        {
            TimedProcessEndTime = Static.MinDate;
            ProcessIsTimed = false;
        }

        public string TabloText0 { get; private set; }
        public string TabloText1 { get; private set; }
        public string TabloText2 { get; private set; }
        public void ChangeTabloText(int num, string text)
        {
            if (num == 0) TabloText0 = text;
            if (num == 1) TabloText1 = text;
            if (num == 2) TabloText2 = text;
        }

        public DateTime PeriodBegin { get; private set; }
        public DateTime PeriodEnd { get; private set; }
        public string PeriodMode { get; private set; }
        public void ChangePeriod(DateTime periodBegin, DateTime periodEnd, string periodMode)
        {
            PeriodBegin = periodBegin;
            PeriodEnd = periodEnd;
            PeriodMode = periodMode;
        }
    }
}
