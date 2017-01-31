using System;
using System.IO;
using BaseLibraryTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseLibrary
{
    [TestClass]
    public class HistoryTextTest : LoggerTimed
    {
         public HistoryTextTest() : base(LoggerDangerness.Single) { }

         //Открытие файла истории
         private void OpenHistory(string fileName, bool replace)
         {
             var file = TestLib.TestRunDir + @"BaseLibrary\History\" + fileName + ".txt";
             var efile = TestLib.TestRunDir + @"BaseLibrary\History\" + fileName + "Errors.txt";
             if (replace)
             {
                 new FileInfo(file).Delete();
                 new FileInfo(efile).Delete();
             }
             History = new HistoryText(this, file, true);
         }

         [TestMethod]
         public void HistText()
         {
             OpenHistory("History", true);
             StartCollect(true, false);
             StartProgress(new DateTime(2017, 1, 1), new DateTime(2017, 1, 2), "Синхронный", "SuperCommand1");
             StartLog("Command11", "Context1", "Pars11").Run(() =>
             {
                 AddEvent("Event111", "Pars111");
                 AddEvent("Event112", "Pars112");
             });
             StartLog("Command12", "Context2", "Pars12");
             Start().Run(() =>
             {
                 AddEvent("Event121", "Pars121");
                 AddError("Error122", null, "Pars122");
                 AddEvent("Event123", "Pars123");
             });
             StartLog("Command13", "Context3", "Pars13");
             StartProgress(new DateTime(2017, 1, 2), new DateTime(2017, 1, 3), "Синхронный", "SuperCommand2");
             StartLog("Command21", "Context1", "Pars21");
             AddEvent("Event211");
             Start();
             AddWarning("Error212");
             AddWarning("Error213", null, "Pars21", "ContextE");
             FinishLog("Results21");
             StartLog("Command22", "Context2", "Pars22");
             AddEvent("Event221");
             AddError("Error222", new Exception("Text"));
             FinishLog("Results22");
             StartProgress(new DateTime(2017, 1, 3), new DateTime(2017, 1, 4), "Синхронный", "SuperCommand3");
             FinishCollect();
             History.Close();

             OpenHistory("History", false);
             StartProgress(new DateTime(2017, 1, 4), new DateTime(2017, 1, 5), "Синхронный", "SuperCommand4").Run(() =>
             {
                 StartLog("Command41", "Context4", "Pars41");
                 StartLog("Command42", "Context4", "Pars42");
                 AddEvent("Event421", "Pars421");
                 AddEvent("Event422");
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

             string cfile = TestLib.TestRunDir + @"BaseLibrary\History\CorrectHistory.txt";
             string file = TestLib.TestRunDir + @"BaseLibrary\History\History.txt";
         }
    }
}