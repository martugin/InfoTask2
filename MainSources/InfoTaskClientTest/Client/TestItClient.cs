﻿using System;
using System.Threading;
using BaseLibrary;
using ComClients;

namespace InfoTaskClientTest
{
    //Класс для тестового отображения формы индикатора
    public class TestItClient : ItClient
    {
        public TestItClient()
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
        
        public void RunTestForm()
        {
            StartPeriod(new DateTime(2017, 1, 1, 10, 0, 0), new DateTime(2017, 1, 1, 11, 0, 0));
            StartProgress("Process");
            StartLog(0, 20, "Command", "SSS");
            StartIndicatorText("Text");
            Thread.Sleep(1000);
            SetProcent(50);
            Thread.Sleep(1000);
            StartLog(20, 40, "Another");
            StartIndicatorText(0, 50, "More");
            Thread.Sleep(1000);
            StartIndicatorText(50, 100, "More More");
            Thread.Sleep(1000);
            StartLog(40, 70, "Third", "3");
            Thread.Sleep(1000);
            StartIndicatorText(30, 60, "ThirdText");
            Thread.Sleep(1000);
            StartIndicatorText(60, 100, "ThirdMore");
            Thread.Sleep(1000);
            StartLog(70, 100, "Last", "UUU", "Par");
            Thread.Sleep(1000);
            SetProcent(40);
            Thread.Sleep(1000);
            StartIndicatorText(70, 100, "Last");
            Thread.Sleep(1000);
            StartPeriod(new DateTime(2017, 1, 1, 11, 0, 0), new DateTime(2017, 1, 1, 12, 0, 0));
            StartProgress("Process");
            StartLog("Log");
            StartIndicatorText(0, 50, "ProgressText");
            Thread.Sleep(1000);
            SetProcent(50);
            Thread.Sleep(1000);
            StartIndicatorText(50, 100, "ProgressMore");
            Thread.Sleep(1000);
            SetProcent(50);
            Thread.Sleep(1000);
            Finish();
            Finish();
            FinishPeriod();
            StartProgress("Process");
            StartLog(0, 50, "Com", "xxx", "P");
            StartIndicatorText(0, 50, "Text");
            Thread.Sleep(1000);
            SetProcent(50);
            Thread.Sleep(1000);
            StartIndicatorText(50, 100, "TextText");
            Thread.Sleep(1000);
            SetProcent(50);
            Thread.Sleep(1000);
            StartLog(50, 100, "Com", "yyy", "P2");
            StartIndicatorText(0, 50, "TextTextText");
            Thread.Sleep(1000);
            SetProcent(50);
            Thread.Sleep(1000);
            StartIndicatorText(50, 100, "TextTextTextText");
            Thread.Sleep(1000);
            SetProcent(50);
            Thread.Sleep(1000);
            Finish();
            Finish();
            Finish();
        }
    }
}