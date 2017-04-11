using System;
using System.IO;
using BaseLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseLibraryTest
{
    [TestClass]
    public class HistoryTextTest : ExternalLogger
    {
         //Открытие файла истории
         private void OpenHistory(string fileName, bool replace)
         {
             TestLib.CopyFile("Libraries", "CorrectHistory.txt", @"History\CorrectHistory.txt");
             TestLib.CopyFile("Libraries", "CorrectHistoryErrors.txt", @"History\CorrectHistoryErrors.txt");
             var file = TestLib.TestRunDir + @"Libraries\History\" + fileName + ".txt";
             var efile = TestLib.TestRunDir + @"Libraries\History\" + fileName + "Errors.txt";
             if (replace)
             {
                 new FileInfo(file).Delete();
                 new FileInfo(efile).Delete();
             }
             Logger = new Logger(new TextHistory(file, true), new TestIndicator());
         }

         private IHistory History { get { return Logger.History; }}

         [TestMethod]
         public void HistText()
         {
             OpenHistory("History", true);
             StartCollect(true, false);
             StartPeriod(new DateTime(2017, 1, 1), new DateTime(2017, 1, 2), "");
             StartProgress("Синхронный", "SuperCommand1");
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
             StartPeriod(new DateTime(2017, 1, 2), new DateTime(2017, 1, 3), "");
             StartProgress("Синхронный", "SuperCommand2");
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
             StartPeriod(new DateTime(2017, 1, 3), new DateTime(2017, 1, 4), "");
             StartProgress("Синхронный", "SuperCommand3");
             FinishCollect();
             History.Close();

             OpenHistory("History", false);
             StartPeriod(new DateTime(2017, 1, 4), new DateTime(2017, 1, 5), "").Run(() =>
             {
                 StartProgress("Синхронный", "SuperCommand4").Run(() =>
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
                 });
             });
             History.Close();

             string cfile = TestLib.TestRunDir + @"Libraries\History\CorrectHistory.txt";
             string file = TestLib.TestRunDir + @"Libraries\History\History.txt";
         }
    }
}