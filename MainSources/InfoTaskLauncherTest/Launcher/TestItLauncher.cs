using System;
using System.Threading;
using BaseLibrary;
using ComLaunchers;

namespace InfoTaskLouncherTest
{
    //Класс для тестового отображения формы индикатора
    public class TestItLauncher : ItLauncher
    {
        public TestItLauncher()
        {
            Logger.Indicator = new AppIndicator();
        }

        public void TestMethod()
        {
            var t = new Thread(StartProcess);
            t.Start();
        }

        private void StartProcess()
        {
            Logger.StartCollect(false, false).Run(() =>
            {
                StartProgress("P", "Процесс");
                StartLogProcent(0, 30, "11111111");
                Thread.Sleep(1000);
                SetProcent(33);
                Thread.Sleep(1000);
                SetProcent(67);
                Thread.Sleep(1000);
                StartLogProcent(30, 70, "22222222");
                Thread.Sleep(1000);
                SetProcent(33);
                Thread.Sleep(1000);
                SetProcent(67);
                Thread.Sleep(1000);
                StartLogProcent(70, 100, "33333333");
                Thread.Sleep(1000);
                SetProcent(33);
                Thread.Sleep(1000);
                SetProcent(67);
                Thread.Sleep(1000);
            });
        }
        
        public void RunTestForm()
        {
            StartPeriod(new DateTime(2017, 1, 1, 10, 0, 0), new DateTime(2017, 1, 1, 11, 0, 0));
            StartProgress("Process");
            StartLogProcent(0, 20, "Command", "SSS");
            StartIndicatorText("Text");
            Thread.Sleep(1000);
            SetProcent(50);
            Thread.Sleep(1000);
            StartLogProcent(20, 40, "Another");
            StartIndicatorTextProcent(0, 50, "More");
            Thread.Sleep(1000);
            StartIndicatorTextProcent(50, 100, "More More");
            Thread.Sleep(1000);
            StartLogProcent(40, 70, "Third", "3");
            Thread.Sleep(1000);
            StartIndicatorTextProcent(30, 60, "ThirdText");
            Thread.Sleep(1000);
            StartIndicatorTextProcent(60, 100, "ThirdMore");
            Thread.Sleep(1000);
            StartLogProcent(70, 100, "Last", "UUU", "Par");
            Thread.Sleep(1000);
            SetProcent(40);
            Thread.Sleep(1000);
            StartIndicatorTextProcent(70, 100, "Last");
            Thread.Sleep(1000);
            StartPeriod(new DateTime(2017, 1, 1, 11, 0, 0), new DateTime(2017, 1, 1, 12, 0, 0));
            StartProgress("Process");
            StartLog("Log");
            StartIndicatorTextProcent(0, 50, "ProgressText");
            Thread.Sleep(1000);
            SetProcent(50);
            Thread.Sleep(1000);
            StartIndicatorTextProcent(50, 100, "ProgressMore");
            Thread.Sleep(1000);
            SetProcent(50);
            Thread.Sleep(1000);
            Finish();
            Finish();
            FinishPeriod();
            StartProgress("Process");
            StartLogProcent(0, 50, "Com", "xxx", "P");
            StartIndicatorTextProcent(0, 50, "Text");
            Thread.Sleep(1000);
            SetProcent(50);
            Thread.Sleep(1000);
            StartIndicatorTextProcent(50, 100, "TextText");
            Thread.Sleep(1000);
            SetProcent(50);
            Thread.Sleep(1000);
            StartLogProcent(50, 100, "Com", "yyy", "P2");
            StartIndicatorTextProcent(0, 50, "TextTextText");
            Thread.Sleep(1000);
            SetProcent(50);
            Thread.Sleep(1000);
            StartIndicatorTextProcent(50, 100, "TextTextTextText");
            Thread.Sleep(1000);
            SetProcent(50);
            Thread.Sleep(1000);
            Finish();
            Finish();
            Finish();
        }
    }
}