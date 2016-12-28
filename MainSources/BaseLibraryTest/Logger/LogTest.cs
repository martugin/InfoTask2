using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseLibrary
{
    [TestClass]
    public class LogTest : LoggerTimed
    {
        public LogTest() : base(LoggerDangerness.Single) { }

        private void RunHistory()
        {
            _history = new TestHistory(this);
            History = _history;
        }
        private TestHistory _history;
        private List<TestCommandLog> Logs { get { return _history.Logs; } }
        private List<TestCommandSuper> Supers { get { return _history.Supers; } }
        private List<TestErrorLog> Errors { get { return _history.Errors; } }

        [TestMethod]
        public void CommLog()
        {
            RunHistory();
            StartLog("Op0", "Source", "Par0");
            Assert.IsNotNull(CommandLog);
            Assert.AreEqual(CommandLog, Command);
            Assert.IsNull(CommandProgress);
            Assert.IsNull(CommandCollect);
            Assert.AreEqual(0, Procent);
            Assert.AreEqual(0, CommandLog.Procent);
            Assert.AreEqual(0, Command.Procent);
            Assert.AreEqual(CommandQuality.Success, CommandLog.Quality);
            Assert.IsFalse(CommandLog.IsFinished);

            Assert.AreEqual(1, Logs.Count);
            Assert.AreEqual("Op0", Logs[0].Command);
            Assert.AreEqual("Par0", Logs[0].Params);
            Assert.AreEqual("Source", Logs[0].Context);
            Assert.AreEqual("Запущено", Logs[0].Status);
            Assert.AreEqual(null, Logs[0].Results);
            Assert.AreEqual(CommandLog.StartTime, Logs[0].Time);
            Assert.AreEqual(0, Logs[0].Events.Count);

            AddEvent("Event", "EventPar");
            Assert.AreEqual(1, Logs[0].Events.Count);
            Assert.AreEqual(CommandQuality.Success, CommandLog.Quality);
            Assert.AreEqual("Event", Logs[0].Events[0].Description);
            Assert.AreEqual("EventPar", Logs[0].Events[0].Params);
            Assert.AreEqual(null, Logs[0].Events[0].Status);

            AddWarning("Warning", null, "WarningPars");
            Assert.AreEqual(2, Logs[0].Events.Count);
            Assert.AreEqual(CommandQuality.Warning, CommandLog.Quality);
            Assert.AreEqual("Warning", Logs[0].Events[1].Description);
            Assert.AreEqual("WarningPars", Logs[0].Events[1].Params);
            Assert.AreEqual("Предупреждение", Logs[0].Events[1].Status);

            AddError("Error", null, "ErrorPars");
            Assert.AreEqual(3, Logs[0].Events.Count);
            Assert.AreEqual(CommandQuality.Error, CommandLog.Quality);
            Assert.AreEqual("Error", Logs[0].Events[2].Description);
            Assert.AreEqual("ErrorPars", Logs[0].Events[2].Params);
            Assert.AreEqual("Ошибка", Logs[0].Events[2].Status);

            Comm c = FinishLog("Results");
            Assert.IsTrue(c.IsFinished);
            Assert.IsNull(CommandLog);
            Assert.IsNull(Command);
            Assert.AreEqual(0, Procent);
            Assert.AreEqual(1, Logs.Count);
            Assert.AreEqual("Op0", Logs[0].Command);
            Assert.AreEqual("Par0", Logs[0].Params);
            Assert.AreEqual("Source", Logs[0].Context);
            Assert.AreEqual("Ошибка", Logs[0].Status);
            Assert.AreEqual("Results", Logs[0].Results);
            Assert.AreEqual(3, Logs[0].Events.Count);

            StartLog("Op1", "Source", "Par1").Run(() =>
                {
                    Assert.IsNotNull(CommandLog);
                    Assert.AreSame(CommandLog, Command);
                    Assert.AreEqual(CommandQuality.Success, CommandLog.Quality);
                    Assert.IsFalse(CommandLog.IsFinished);

                    AddWarning("Warning", null, "WarningPars");
                    Assert.AreEqual(1, Logs[1].Events.Count);
                    Assert.AreEqual(CommandQuality.Warning, CommandLog.Quality);
                    Assert.AreEqual("Warning", Logs[1].Events[1].Description);
                    Assert.AreEqual("WarningPars", Logs[1].Events[1].Params);
                    Assert.AreEqual("Предупреждение", Logs[0].Events[1].Status);

                    var cc = Start().Run(() =>
                    {
                        Assert.IsNotNull(Command);
                        Assert.AreNotEqual(CommandLog, Command);
                        Assert.AreEqual(CommandLog, Command.Parent);

                        AddEvent("Event", "EventPar");
                        Assert.AreEqual(2, Logs[1].Events.Count);
                        Assert.AreEqual(CommandQuality.Warning, CommandLog.Quality);
                        Assert.AreEqual("Event", Logs[1].Events[1].Description);
                        Assert.AreEqual("EventPar", Logs[1].Events[1].Params);
                        Assert.AreEqual(null, Logs[0].Events[1].Status);
                    });
                    Assert.IsTrue(cc.IsFinished);
                });

            Assert.IsNull(CommandLog);
            Assert.IsNull(Command);
            Assert.AreEqual(2, Logs.Count);
            Assert.AreEqual("Op1", Logs[1].Command);
            Assert.AreEqual("Par1", Logs[1].Params);
            Assert.AreEqual("Source", Logs[1].Context);
            Assert.AreEqual("Ошибка", Logs[1].Status);
            Assert.AreEqual("", Logs[1].Results);
            Assert.AreEqual(2, Logs[1].Events.Count);

            StartLog("Op2", "Source", "Par2");
            Assert.IsNotNull(CommandLog);
            Assert.AreEqual("Op2", CommandLog.Name);

            c = StartLog("Op3", "Source", "Par3").Run(() =>
                {
                    Assert.IsNotNull(CommandLog);
                    Assert.AreEqual("Op3", CommandLog.Name);

                    Assert.AreEqual(4, Logs.Count);
                    Assert.AreEqual("Op3", Logs[3].Command);
                    Assert.AreEqual("Par3", Logs[3].Params);
                    Assert.AreEqual("Source", Logs[3].Context);
                    Assert.AreEqual("Запущено", Logs[3].Status);
                    Assert.AreEqual(null, Logs[3].Results);
                    Assert.AreEqual(0, Logs[3].Events.Count);

                    AddEvent("Event", "EventPar");
                    Assert.AreEqual(1, Logs[3].Events.Count);
                    Assert.AreEqual(CommandQuality.Success, CommandLog.Quality);
                    Assert.AreEqual("Event", Logs[3].Events[0].Description);
                    Assert.AreEqual("EventPar", Logs[3].Events[0].Params);
                    Assert.AreEqual(null, Logs[3].Events[0].Status);

                    return "Res";
                });
            Assert.IsTrue(c.IsFinished);

            Assert.AreEqual(4, Logs.Count);
            Assert.AreEqual("Op3", Logs[3].Command);
            Assert.AreEqual("Par3", Logs[3].Params);
            Assert.AreEqual("Source", Logs[3].Context);
            Assert.AreEqual("Успешно", Logs[3].Status);
            Assert.AreEqual("Res", Logs[3].Results);
            Assert.AreEqual(2, Logs[1].Events.Count);
        }

        [TestMethod]
        public void CommProgress()
        {
            RunHistory();
            StartProgress("Процесс", "Progress", "Pars");
            Assert.IsNotNull(CommandProgress);
            Assert.AreSame(CommandProgress, Command);
            Assert.AreEqual(1, Supers.Count);
            Assert.AreEqual("Progress", Supers[0].Command);
            Assert.AreEqual("Pars", Supers[0].Params);
            Assert.AreEqual("Запущено", Supers[0].Status);
            Assert.AreEqual(CommandProgress.StartTime, Supers[0].Time);
            
            Assert.AreEqual(0, Procent);
            Procent = 10;
            Assert.AreEqual(10, Procent);
            Assert.AreEqual(10, CommandProgress.Procent);

            Start(20, 70);
            Assert.AreSame(CommandProgress, Command.Parent);
            Assert.AreEqual(20, CommandProgress.Procent);
            Assert.AreEqual(0, Procent);
            Assert.AreEqual(0, Command.Procent);
            Procent = 20;
            Assert.AreEqual(30, CommandProgress.Procent);
            Assert.AreEqual(20, Procent);
            Assert.AreEqual(20, Command.Procent);

            StartLog(40, 80, "Log", "Mod");
            Assert.IsNotNull(CommandLog);
            Assert.AreEqual(CommandProgress, CommandLog.Parent.Parent);
            Assert.AreEqual(Command, CommandLog);
            Assert.AreEqual(40, CommandProgress.Procent);
            Assert.AreEqual(0, Procent);
            Assert.AreEqual(1, Supers[0].Logs.Count);
            Assert.AreEqual("Log", Supers[0].Logs[0].Command);
            Assert.AreEqual("", Supers[0].Logs[0].Params);
            Assert.AreEqual(1, Logs.Count);

            AddEvent("Event", "Pars", 50);
            Assert.AreEqual(50, CommandProgress.Procent);
            Assert.AreEqual(50, Procent);
            Assert.AreEqual(1, Logs[0].Events.Count);
            Assert.AreEqual("Event", Logs[0].Events[0].Description);
            Assert.AreEqual(CommandQuality.Success, CommandLog.Quality);
            Assert.AreEqual(CommandQuality.Success, CommandProgress.Quality);

            AddError("Error");
            Assert.AreEqual(50, CommandProgress.Procent);
            Assert.AreEqual(50, Procent);
            Assert.AreEqual(2, Logs[0].Events.Count);
            Assert.AreEqual("Error", Logs[0].Events[1].Description);
            Assert.AreEqual(CommandQuality.Error, CommandLog.Quality);
            Assert.AreEqual(CommandQuality.Error, CommandProgress.Quality);
            
            FinishLog();
            Assert.AreEqual(60, CommandProgress.Procent);
            Assert.AreEqual(80, Procent);
            Assert.AreEqual(CommandQuality.Error, CommandProgress.Quality);

            var c = StartLog(80, 100, "Log2", "Mod2").Run(()=>{});
            Assert.AreEqual(70, CommandProgress.Procent);
            Assert.AreEqual(100, Procent);
            Assert.AreEqual(100, c.Procent);
            Assert.AreEqual(CommandQuality.Success, c.Quality);
            Assert.AreEqual(2, Logs.Count);
            Assert.AreEqual(2, Supers[0].Logs.Count);
            Assert.AreEqual(CommandQuality.Error, CommandProgress.Quality);

            c = FinishProgress();
            Assert.AreEqual(100, c.Procent);
            Assert.AreEqual(CommandQuality.Error, c.Quality);
            Assert.AreEqual("Ошибка", c.Status);
        }

        [TestMethod]
        public void CommCollect()
        {
            RunHistory();
            StartCollect(true, true);
            Assert.IsNotNull(CommandCollect);
            Assert.AreSame(Command, CommandCollect);
            Assert.AreEqual(0, Logs.Count);
            Assert.AreEqual(0, Supers.Count);
            var beg = new DateTime(2017, 1, 1);
            var en = new DateTime(2017, 1, 2);
            StartProgress(beg, en, "Mode", "Progress", "Pars");
            Assert.AreEqual(1, Supers.Count);
            Assert.AreSame(CommandCollect, CommandProgress.Parent);
            Assert.AreEqual(beg, Supers[0].BeginPeriod);
            Assert.AreEqual(en, Supers[0].EndPeriod);
            Assert.AreEqual("Mode", Supers[0].ModePeriod);
            Assert.AreEqual("Progress", Supers[0].Command);
            StartLog("Log0", "Source", "Log0Pars");
            Assert.AreEqual(1, Supers[0].Logs.Count);
            Assert.AreSame(CommandProgress, CommandLog.Parent);
            
            AddWarning("War0", null, "War0Pars");
            Assert.AreEqual(1, Logs[0].Events.Count);
            Assert.AreEqual(1, Errors.Count);
            Assert.AreEqual(beg, Errors[0].BeginPeriod);
            Assert.AreEqual(en, Errors[0].EndPeriod);
            Assert.AreEqual("Source", Errors[0].Context);
            Assert.AreEqual("War0", Errors[0].Description);
            Assert.AreEqual("Log0", Errors[0].Command);
            Assert.AreEqual("War0Pars", Errors[0].Params);
            Assert.AreEqual("Предупреждение", Errors[0].Status);
            Assert.AreEqual(CommandQuality.Warning, CommandCollect.Quality);

            AddWarning("War1", null, "War1Pars");
            Assert.AreEqual(2, Logs[0].Events.Count);
            Assert.AreEqual(2, Errors.Count);
            Assert.AreEqual(beg, Errors[1].BeginPeriod);
            Assert.AreEqual(en, Errors[1].EndPeriod);
            Assert.AreEqual("Source", Errors[1].Context);
            Assert.AreEqual("War1", Errors[1].Description);
            Assert.AreEqual("Log0", Errors[1].Command);
            Assert.AreEqual("War1Pars", Errors[1].Params);
            Assert.AreEqual("Предупреждение", Errors[1].Status);
            Assert.AreEqual(CommandQuality.Warning, CommandCollect.Quality);

            StartLog("Log1", "Source2", "Log1Pars");
            Assert.AreEqual(2, Logs.Count);
            Assert.AreSame(CommandProgress, CommandLog.Parent);

            AddError("Err2", null, "Err2Pars");
            Assert.AreEqual(1, Logs[1].Events.Count);
            Assert.AreEqual(3, Errors.Count);
            Assert.AreEqual(beg, Errors[2].BeginPeriod);
            Assert.AreEqual(en, Errors[2].EndPeriod);
            Assert.AreEqual("Source2", Errors[2].Context);
            Assert.AreEqual("Err2", Errors[2].Description);
            Assert.AreEqual("Log1", Errors[2].Command);
            Assert.AreEqual("Err2Pars", Errors[2].Params);
            Assert.AreEqual("Ошибка", Errors[2].Status);
            Assert.AreEqual(CommandQuality.Error, CommandCollect.Quality);

            FinishProgress();
            Assert.AreEqual(1, Supers.Count);
            Assert.IsNull(CommandProgress);
            Assert.AreEqual(CommandQuality.Error, CommandCollect.Quality);

            FinishCollect();
            Assert.IsNull(CommandCollect);
            Assert.IsNull(Command);
        }
    }
}