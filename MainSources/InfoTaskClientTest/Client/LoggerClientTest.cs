using System;
using System.Threading;
using BaseLibrary;
using BaseLibraryTest;
using ComClients;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfoTaskClientTest
{
    [TestClass]
    public class LoggerClientTest : ItClient
    {
        public LoggerClientTest()
        {
            Logger.Indicator = new TestIndicator();
            Logger.History = new TestHistory(Logger);
        }

        public void MakeIndicatorEvents()
        {
            StartPeriod(new DateTime(2017, 1, 1, 10, 0, 0), new DateTime(2017, 1, 1, 11, 0, 0));
            StartProgress("Process");
            StartLog(0, 20, "Command", "SSS");
            StartIndicatorText(0, 100, "Text");
            Thread.Sleep(10);
            SetProcent(50);
            Thread.Sleep(10);
            StartLog(20, 40, "Another");
            StartIndicatorText(0, 50, "More");
            Thread.Sleep(10);
            StartIndicatorText(50, 100, "More More");
            Thread.Sleep(10);
            StartLog(40, 70, "Third", "3");
            Thread.Sleep(10);
            StartIndicatorText(30, 60, "ThirdText");
            Thread.Sleep(10);
            StartIndicatorText(60, 100, "ThirdMore");
            Thread.Sleep(10);
            StartLog(70, 100, "Last", "UUU", "Par");
            Thread.Sleep(10);
            SetProcent(40);
            Thread.Sleep(10);
            StartIndicatorText(70, 100, "Last");
            Thread.Sleep(10);
            Logger.StartPeriod(new DateTime(2017, 1, 1, 11, 0, 0), new DateTime(2017, 1, 1, 12, 0, 0), "Mode");
            Logger.StartProgress("Process", "", new DateTime(2017, 1, 1, 12, 0, 0));
            StartLog(0, 100, "Log");
            StartIndicatorText(0, 50, "ProgressText");
            Thread.Sleep(10);
            SetProcent(50);
            Thread.Sleep(10);
            StartIndicatorText(50, 100, "ProgressMore");
            Thread.Sleep(10);
            SetProcent(50);
            Thread.Sleep(10);
            Finish();
            Finish();
            FinishPeriod();
            StartProgress("Process", "");
            StartLog(0, 50, "Com", "xxx", "P");
            StartIndicatorText(0, 50, "Text");
            Thread.Sleep(10);
            SetProcent(50);
            Thread.Sleep(10);
            StartIndicatorText(50, 100, "TextText");
            Thread.Sleep(10);
            SetProcent(50);
            Thread.Sleep(10);
            StartLog(50, 100, "Com", "yyy", "P2");
            StartIndicatorText(0, 50, "TextTextText");
            Thread.Sleep(10);
            SetProcent(50);
            Thread.Sleep(10);
            StartIndicatorText(50, 100, "TextTextTextText");
            Thread.Sleep(10);
            SetProcent(50);
            Thread.Sleep(10);
            Finish();
            Finish();
            Finish();
        }

        [TestMethod]
        public void Indicator()
        {
            MakeIndicatorEvents();
            var ind = (TestIndicator)Logger.Indicator;
            Assert.AreEqual(86, ind.Events.Count);
            ind.Compare("ShowTimed");
            ind.Compare("PeriodBegin", "01.01.2017 10:00:00");
            ind.Compare("PeriodEnd", "01.01.2017 11:00:00");
            ind.Compare("PeriodMode");
            ind.Compare("Text0", "Process");
            ind.Compare("ProcessUsual");
            ind.Compare("Procent", "0");
            ind.Compare("Text1", "Command (SSS)");
            ind.Compare("Text2", "Text");
            ind.Compare("Procent", "10");
            ind.Compare("Text2");
            ind.Compare("Procent", "20");
            ind.Compare("Text1");
            ind.Compare("Text1", "Another");
            ind.Compare("Text2", "More");
            ind.Compare("Text2");
            ind.Compare("Procent", "30");
            ind.Compare("Text2", "More More");
            ind.Compare("Text2");
            ind.Compare("Procent", "40");
            ind.Compare("Text1");
            ind.Compare("Text1", "Third (3)");
            ind.Compare("Procent", "49");
            ind.Compare("Text2", "ThirdText");
            ind.Compare("Text2");
            ind.Compare("Procent", "58");
            ind.Compare("Text2", "ThirdMore");
            ind.Compare("Text2");
            ind.Compare("Procent", "70");
            ind.Compare("Text1");
            ind.Compare("Text1", "Last (UUU)");
            ind.Compare("Procent", "82");
            ind.Compare("Procent", "91");
            ind.Compare("Text2", "Last");
            ind.Compare("Text2");
            ind.Compare("Procent", "100");
            ind.Compare("Text1");
            ind.Compare("Text0");
            ind.Compare("Hide");
            ind.Compare("Procent", "0");
            ind.Compare("ShowTimed");
            ind.Compare("PeriodBegin", "01.01.2017 11:00:00");
            ind.Compare("PeriodEnd", "01.01.2017 12:00:00");
            ind.Compare("PeriodMode", "Mode");
            ind.Compare("Text0", "Process");
            ind.Compare("ProcessTimed", "01.01.2017 12:00:00");
            ind.Compare("Procent", "0");
            ind.Compare("Text1", "Log");
            ind.Compare("Text2", "ProgressText");
            ind.Compare("Procent", "25");
            ind.Compare("Text2");
            ind.Compare("Procent", "50");
            ind.Compare("Text2", "ProgressMore");
            ind.Compare("Procent", "75");
            ind.Compare("Text2");
            ind.Compare("Procent", "100");
            ind.Compare("Text1");
            ind.Compare("Text0");
            ind.Compare("Hide");
            ind.Compare("Procent", "0");
            ind.Compare("ShowTexted");
            ind.Compare("Text0", "Process");
            ind.Compare("ProcessUsual");
            ind.Compare("Procent", "0");
            ind.Compare("Text1", "Com (xxx)");
            ind.Compare("Text2", "Text");
            ind.Compare("Procent", "12,5");
            ind.Compare("Text2");
            ind.Compare("Procent", "25");
            ind.Compare("Text2", "TextText");
            ind.Compare("Procent", "37,5");
            ind.Compare("Text2");
            ind.Compare("Procent", "50");
            ind.Compare("Text1");
            ind.Compare("Text1", "Com (yyy)");
            ind.Compare("Text2", "TextTextText");
            ind.Compare("Procent", "62,5");
            ind.Compare("Text2");
            ind.Compare("Procent", "75");
            ind.Compare("Text2", "TextTextTextText");
            ind.Compare("Procent", "87,5");
            ind.Compare("Text2");
            ind.Compare("Procent", "100");
            ind.Compare("Text1");
            ind.Compare("Text0");
            ind.Compare("Hide");
        }

        [TestMethod]
        public void BreakIndicator()
        {
            Logger.StartCollect(false, true).Run(() =>
                Logger.StartProgress("T", "N").Run(() =>
                    Logger.StartLog(20, 60, "Log").Run(() =>
                    {
                        Logger.Procent = 50;
                        Break();
                        Thread.Sleep(1000);
                        AddEvent("Text");
                        Logger.Procent = 75;
                    })));
   
            var ind = (TestIndicator)Logger.Indicator;
            Assert.AreEqual(12, ind.Events.Count);
            ind.Compare("ShowTexted");
            ind.Compare("Text0", "T");
            ind.Compare("ProcessUsual");
            ind.Compare("Procent", "0");
            ind.Compare("Procent", "20");
            ind.Compare("Text1", "Log");
            ind.Compare("Procent", "40");
            ind.Compare("Procent", "60");
            ind.Compare("Text1");
            ind.Compare("Procent", "100");
            ind.Compare("Text0");
            ind.Compare("Hide");
        }
    }
}