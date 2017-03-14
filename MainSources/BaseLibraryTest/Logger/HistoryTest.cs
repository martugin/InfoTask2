using System;
using BaseLibrary;
using CommonTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseLibraryTest
{
    [TestClass]
    public class HistoryTest :  Logger
    {
        //Открытие файла истории
        private void OpenHistory(string fileName, bool replace)
        {
            var file = TestLib.TestRunDir + @"BaseLibrary\History\" + fileName + ".accdb";
            if (replace)
            {
                DaoDb.FromTemplate(ItStatic.HistoryTemplateFile, file, ReplaceByTemplate.Always);
                TestLib.CopyFile("BaseLibrary", "CorrectHistory.accdb", @"History\CorrectHistory.accdb");
            }
            History = new AccessHistory(this, file, ItStatic.HistoryTemplateFile);
        }

        [TestMethod]
        public void Hist()
        {
            OpenHistory("History", true);
            History.ClearErrorsList();
            StartCollect(true, false);
            StartPeriod(new DateTime(2017, 1, 1), new DateTime(2017, 1, 2), "Синхронный");
            StartProgress("SuperCommand1");
            StartLog("Command11", "Context1", "Pars11").Run(() =>
                {
                    AddEvent("Event111", "Pars111");
                    AddEvent("Event112", "Pars112");
                });
            StartLog("Command12", "Context2", "Pars12");
            Start(0, 100).Run(() =>
                {
                    AddEvent("Event121", "Pars121");
                    AddError("Error122", null, "Pars122");
                    AddEvent("Event123", "Pars123");
                });
            StartLog("Command13", "Context3", "Pars13");
            StartPeriod(new DateTime(2017, 1, 2), new DateTime(2017, 1, 3), "Синхронный");
            StartProgress("SuperCommand2");
            StartLog("Command21", "Context1", "Pars21");
            AddEvent("Event211");
            Start(0, 100);
            AddWarning("Error212");
            AddWarning("Error213", null, "Pars21", "ContextE");
            FinishLog("Results21");
            StartLog("Command22", "Context2", "Pars22");
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
                    StartLog("Command41", "Context4", "Pars41");
                    StartLog("Command42", "Context4", "Pars42");
                    AddEvent("Event421", "Pars421");
                    AddEvent("Event422");
                });
            });
            
            StartCollect(true, false).Run(() =>
                {
                    StartLog("Command01", "Context0", "Pars01");
                    StartLog("Command02", "Context0", "Pars02").Run(() =>
                        {
                            AddEvent("Event021");
                            AddEvent("Event022");
                            AddError("Error023");
                            SetLogResults("Res");
                        });
                });
            History.Close();

            string cfile = TestLib.TestRunDir + @"BaseLibrary\History\CorrectHistory.accdb";
            string file = TestLib.TestRunDir + @"BaseLibrary\History\History.accdb";
            TestLib.CompareTables(cfile, file, "SuperHistory", "SuperHistoryId", null, "Time", "ProcessLength");
            TestLib.CompareTables(cfile, file, "History", "HistoryId", null, "Time", "ProcessLength");
            TestLib.CompareTables(cfile, file, "SubHistory", "Id", null, "Time", "FromStart");
            TestLib.CompareTables(cfile, file, "ErrorsList", "Id", null, "Time");
        }
    }
}