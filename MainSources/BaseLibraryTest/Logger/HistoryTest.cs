using System;
using BaseLibrary;
using CommonTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseLibraryTest
{
    [TestClass]
    public class HistoryTest : ExternalLogger
    {
        //Открытие файла истории
        private void OpenHistory(string fileName, bool replace)
        {
            var file = TestLib.TestRunDir + @"Libraries\History\" + fileName + ".accdb";
            var template = ItStatic.InfoTaskDir() + @"Templates\LocalData\History.accdb";
            if (replace)
            {
                DaoDb.FromTemplate(template, file, ReplaceByTemplate.Always);
                TestLib.CopyFile("Libraries", "CorrectHistory.accdb", @"History\CorrectHistory.accdb");
            }
            var history = new AccessHistory(file, template);
            Logger = new Logger(history, new TestIndicator());
        }

        private IHistory History { get { return Logger.History; } }

        [TestMethod]
        public void Hist()
        {
            OpenHistory("History", true);
            History.ClearErrorsList();
            StartCollect(true, false);
            StartPeriod(new DateTime(2017, 1, 1), new DateTime(2017, 1, 2), "Синхронный");
            StartProgress("SuperCommand1");
            StartLog("Command11", "Pars11", "Context1").Run(() =>
                {
                    AddEvent("Event111", "Pars111");
                    AddEvent("Event112", "Pars112");
                });
            StartLog("Command12", "Pars12", "Context2");
            Start(0, 100).Run(() =>
                {
                    AddEvent("Event121", "Pars121");
                    AddError("Error122", null, "Pars122");
                    AddEvent("Event123", "Pars123");
                });
            StartLog("Command13", "Pars13", "Context3");
            StartPeriod(new DateTime(2017, 1, 2), new DateTime(2017, 1, 3), "Синхронный");
            StartProgress("SuperCommand2");
            StartLog("Command21", "Pars21", "Context1");
            AddEvent("Event211");
            Start(0, 100);
            AddWarning("Error212");
            AddWarning("Error213", null, "Pars21", "ContextE");
            FinishLog("Results21");
            StartLog("Command22", "Pars22", "Context2");
            AddEvent("Event221");
            AddError("Error222", new Exception("Text"));
            FinishLog("Results22");
            StartPeriod(new DateTime(2017, 1, 3), new DateTime(2017, 1, 4), "Синхронный");
            StartProgress("SuperCommand3");
            FinishCollect();
            History.Close();

            OpenHistory("History", false);
            StartPeriod(new DateTime(2017, 1, 4), new DateTime(2017, 1, 5), "Синхронный").Run(() =>
            {
                StartProgress("SuperCommand4").Run(() =>
                {
                    StartLog("Command41", "Pars41", "Context4");
                    StartLog("Command42", "Pars42", "Context4");
                    AddEvent("Event421", "Pars421");
                    AddEvent("Event422");
                });
            });
            
            StartCollect(true, false).Run(() =>
                {
                    StartLog("Command01", "Pars01", "Context0");
                    StartLog("Command02", "Pars02", "Context0").Run(() =>
                        {
                            AddEvent("Event021");
                            AddEvent("Event022");
                            AddError("Error023");
                            SetLogResults("Res");
                        });
                });
            History.Close();

            string cfile = TestLib.TestRunDir + @"Libraries\History\CorrectHistory.accdb";
            string file = TestLib.TestRunDir + @"Libraries\History\History.accdb";
            TestLib.CompareHistories(cfile, file);
        }
    }
}