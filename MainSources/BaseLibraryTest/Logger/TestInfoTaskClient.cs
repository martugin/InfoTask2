using System;
using System.Threading;
using ComClients;

namespace BaseLibrary
{
    //Тестовая подмена InfoTaskClient
    public class TestInfoTaskClient : LoggerClient
    {
         public void Run()
         {
             StartProgress("Process", "", new DateTime(2017, 1, 1, 10, 0, 0), new DateTime(2017, 1, 1 ,11, 0, 0));
                 StartLog(0, 20, "Command","", "SSS");
                    StartProgressText("Text");
                        Thread.Sleep(1000);
                        SetProcent(50);
                        Thread.Sleep(1000);
                 StartLog(20, 40, "Another");
                        StartProgressText(0, 50, "More");
                        Thread.Sleep(1000);
                    StartProgressText(50, 100, "More More");
                        Thread.Sleep(1000);
                 StartLog(40, 70, "Third", "", "3");
                        Thread.Sleep(1000);
                    StartProgressText(30, 60, "ThirdText");
                        Thread.Sleep(1000);
                    StartProgressText(60, 100, "ThirdMore");
                        Thread.Sleep(1000);
                 StartLog(70, 100, "Last", "Par", "UUU");
                        Thread.Sleep(1000);
                        SetProcent(40);
                        Thread.Sleep(1000);
                    StartProgressText(70, 100, "Last");
                        Thread.Sleep(1000);
            StartProgress("Process", "", new DateTime(2017, 1, 1, 11, 0, 0), new DateTime(2017, 1, 1, 12, 0, 0));
                StartLog("Log");
                    StartProgressText(0, 50, "ProgressText");
                        Thread.Sleep(1000);
                        SetProcent(50);
                        Thread.Sleep(1000);
                    StartProgressText(50, 100, "ProgressMore");
                        Thread.Sleep(1000);
                        SetProcent(50);
                        Thread.Sleep(1000);
                    Finish();
                Finish();
            StartProgress("Process", "", "Заголовок");
                StartLog(0, 50, "Com", "P", "xxx");
                    StartProgressText(0, 50, "Text");
                        Thread.Sleep(1000);
                        SetProcent(50);
                        Thread.Sleep(1000);
                    StartProgressText(50, 100, "TextText");
                        Thread.Sleep(1000);
                        SetProcent(50);
                        Thread.Sleep(1000);
                StartLog(50, 100, "Com", "P2", "yyy");
                    StartProgressText(0, 50, "TextTextText");
                        Thread.Sleep(1000);
                        SetProcent(50);
                        Thread.Sleep(1000);
                        StartProgressText(50, 100, "TextTextTextText");
                        Thread.Sleep(1000);
                        SetProcent(50);
                        Thread.Sleep(1000);
                    Finish();
                Finish();
             Finish();
         }
    }
}