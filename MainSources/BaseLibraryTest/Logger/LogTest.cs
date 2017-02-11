using System;
using System.Collections.Generic;
using System.Threading;
using BaseLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseLibraryTest
{
    [TestClass]
    public class LogTest : Logger
    {
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
            Assert.AreEqual("Op0 (Source)", TabloText(1));

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

            Command c = FinishLog("Results");
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
            Assert.AreEqual("", TabloText(1));

            StartLog("Op1", "Source", "Par1").Run(() =>
                {
                    Assert.IsNotNull(CommandLog);
                    Assert.AreSame(CommandLog, Command);
                    Assert.AreEqual(CommandQuality.Success, CommandLog.Quality);
                    Assert.IsFalse(CommandLog.IsFinished);
                    Assert.AreEqual("Op1 (Source)", TabloText(1));

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
            Assert.IsNull(Logs[1].Results);
            Assert.AreEqual(2, Logs[1].Events.Count);

            StartLog("Op2", "Source", "Par2");
            Assert.IsNotNull(CommandLog);
            Assert.AreEqual("Op2", CommandLog.Name);
            Assert.AreEqual("Op2 (Source)", TabloText(1));

            c = StartLog("Op3", "Source", "Par3").Run(() =>
                {
                    Assert.IsNotNull(CommandLog);
                    Assert.AreEqual("Op3", CommandLog.Name);
                    Assert.AreEqual("Op3 (Source)", TabloText(1));

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
                    SetLogResults("Res");
                });
            Assert.IsTrue(c.IsFinished);
            Assert.AreEqual("", TabloText(1));

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
            StartProgress("Процесс", "Progress", "Pars", DateTime.Now.AddHours(1));
            Assert.IsNotNull(CommandProgress);
            Assert.AreSame(CommandProgress, Command);
            Assert.AreEqual(1, Supers.Count);
            Assert.AreEqual("Progress", Supers[0].Command);
            Assert.AreEqual("Pars", Supers[0].Params);
            Assert.AreEqual("Запущено", Supers[0].Status);
            Assert.AreEqual(CommandProgress.StartTime, Supers[0].Time);
            Assert.AreEqual("Процесс", TabloText(0));
            
            Assert.AreEqual(0, Procent);
            Procent = 10;
            Assert.AreEqual(10, Procent);
            Assert.AreEqual(10, CommandProgress.Procent);
            Assert.AreEqual(10, TabloProcent);

            Start(20, 70);
            Assert.AreSame(CommandProgress, Command.Parent);
            Assert.AreEqual("Процесс", TabloText(0));
            Assert.AreEqual(20, CommandProgress.Procent);
            Assert.AreEqual(20, TabloProcent);
            Assert.AreEqual(0, Procent);
            Assert.AreEqual(0, Command.Procent);
            Procent = 20;
            Assert.AreEqual(30, CommandProgress.Procent);
            Assert.AreEqual(30, TabloProcent);
            Assert.AreEqual(20, Procent);
            Assert.AreEqual(20, Command.Procent);

            Assert.IsNull(TabloText(2));
            StartIndicatorText(20, 30, "Text3").Run(() =>
                {
                    Assert.AreEqual(30, CommandProgress.Procent);
                    Assert.AreEqual(30, TabloProcent);
                    Assert.AreEqual(0, Procent);
                    Assert.AreEqual("Text3", TabloText(2));
                });

            StartLog(40, 80, "Log", "Mod");
            Assert.IsNotNull(CommandLog);
            Assert.AreEqual(CommandProgress, CommandLog.Parent.Parent);
            Assert.AreEqual(Command, CommandLog);
            Assert.AreEqual(40, CommandProgress.Procent);
            Assert.AreEqual(40, TabloProcent);
            Assert.AreEqual(0, Procent);
            Assert.AreEqual(1, Supers[0].Logs.Count);
            Assert.AreEqual("Log", Supers[0].Logs[0].Command);
            Assert.AreEqual("", Supers[0].Logs[0].Params);
            Assert.AreEqual(1, Logs.Count);
            Assert.AreEqual("Процесс", TabloText(0));
            Assert.AreEqual("Log (Mod)", TabloText(1));

            AddEvent("Event", "Pars", 50);
            Assert.AreEqual(50, CommandProgress.Procent);
            Assert.AreEqual(50, TabloProcent);
            Assert.AreEqual(50, Procent);
            Assert.AreEqual(1, Logs[0].Events.Count);
            Assert.AreEqual("Event", Logs[0].Events[0].Description);
            Assert.AreEqual(CommandQuality.Success, CommandLog.Quality);
            Assert.AreEqual(CommandQuality.Success, CommandProgress.Quality);

            AddError("Error");
            Assert.AreEqual(50, CommandProgress.Procent);
            Assert.AreEqual(50, TabloProcent);
            Assert.AreEqual(50, Procent);
            Assert.AreEqual(2, Logs[0].Events.Count);
            Assert.AreEqual("Error", Logs[0].Events[1].Description);
            Assert.AreEqual(CommandQuality.Error, CommandLog.Quality);
            Assert.AreEqual(CommandQuality.Error, CommandProgress.Quality);
            
            Finish();
            Assert.AreEqual(60, CommandProgress.Procent);
            Assert.AreEqual(60, TabloProcent);
            Assert.AreEqual(80, Procent);
            Assert.AreEqual(CommandQuality.Error, CommandProgress.Quality);
            Assert.AreEqual("Процесс", TabloText(0));
            Assert.AreEqual("", TabloText(1));

            var c = StartLog(80, 100, "Log2", "Mod2").Run(() =>
                {
                    Assert.AreEqual("Процесс", TabloText(0));
                    Assert.AreEqual("Log2 (Mod2)", TabloText(1));
                });
            Assert.AreEqual(70, CommandProgress.Procent);
            Assert.AreEqual(70, TabloProcent);
            Assert.AreEqual(100, Procent);
            Assert.AreEqual(100, c.Procent);
            Assert.AreEqual(CommandQuality.Success, c.Quality);
            Assert.AreEqual(2, Logs.Count);
            Assert.AreEqual(2, Supers[0].Logs.Count);
            Assert.AreEqual(CommandQuality.Error, CommandProgress.Quality);

            StartIndicatorText(20, 30, "Text3_1");
            Assert.AreEqual("Процесс", TabloText(0));
            Assert.AreEqual("", TabloText(1));
            Assert.AreEqual("Text3_1", TabloText(2));
            FinishIndicatorText();

            c = FinishProgress();
            Assert.AreEqual("", TabloText(0));
            Assert.AreEqual("", TabloText(1));
            Assert.AreEqual("", TabloText(2));
            Assert.AreEqual(100, c.Procent);
            Assert.AreEqual(100, TabloProcent);
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
            Assert.IsNull(TabloText(1));

            var beg = new DateTime(2017, 1, 1);
            var en = new DateTime(2017, 1, 2);
            StartProgress(beg, en, "Mode", "Progress", "Pars");
            Assert.AreEqual(1, Supers.Count);
            Assert.AreSame(CommandCollect, CommandProgress.Parent);
            Assert.AreEqual(beg, Supers[0].BeginPeriod);
            Assert.AreEqual(en, Supers[0].EndPeriod);
            Assert.AreEqual("Mode", Supers[0].ModePeriod);
            Assert.AreEqual("Progress", Supers[0].Command);
            Assert.AreEqual(beg, BeginPeriod);
            Assert.AreEqual(en, EndPeriod);
            Assert.AreEqual("Mode", ModePeriod);

            StartLog("Log0", "Source", "Log0Pars");
            Assert.AreEqual(1, Supers[0].Logs.Count);
            Assert.AreSame(CommandProgress, CommandLog.Parent);
            Assert.AreEqual("Log0 (Source)", TabloText(1));
            Assert.AreEqual(beg, BeginPeriod);
            Assert.AreEqual(en, EndPeriod);
            
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
            Assert.AreEqual("Log1 (Source2)", TabloText(1));

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

            try { Command = CommandCollect.Parent.Parent;}
            catch (Exception ex)
            {
                AddError("Err3", ex, "Err3Pars");
                Assert.AreEqual(2, Logs[1].Events.Count);
                Assert.AreEqual("Err3", Logs[1].Events[1].Description);
                Assert.IsTrue(Logs[1].Events[1].Params.StartsWith("Err3Pars;" + Environment.NewLine + "Object reference not set to an instance of an object") ||
                    Logs[1].Events[1].Params.StartsWith("Err3Pars;" + Environment.NewLine + "Ссылка на объект не указывает на экземпляр объекта"));
            }

            Assert.AreEqual(2, Logs[1].Events.Count);
            Assert.AreEqual(4, Errors.Count);
            Assert.AreEqual(beg, Errors[3].BeginPeriod);
            Assert.AreEqual(en, Errors[3].EndPeriod);
            Assert.AreEqual("Source2", Errors[3].Context);
            Assert.AreEqual("Err3", Errors[3].Description);
            Assert.AreEqual("Log1", Errors[3].Command);
            Assert.AreEqual("Err3Pars", Errors[3].Params);
            Assert.AreEqual("Ошибка", Errors[3].Status);
            Assert.AreEqual(CommandQuality.Error, CommandCollect.Quality);

            FinishProgress();
            Assert.AreEqual("", TabloText(1));
            Assert.AreEqual("", TabloText(0));
            Assert.AreEqual(beg, BeginPeriod);
            Assert.AreEqual(en, EndPeriod);
            Assert.AreEqual("Mode", ModePeriod);

            Assert.AreEqual(1, Supers.Count);
            Assert.IsNull(CommandProgress);
            Assert.AreEqual(CommandQuality.Error, CommandCollect.Quality);

            var c = FinishCollect("Results");
            Assert.AreEqual("Results", CommandResults);
            Assert.IsNull(CommandCollect);
            Assert.IsNull(Command);
            Assert.AreEqual(CommandQuality.Error, c.Quality);
            Assert.AreEqual("Ошибка: Err2; Source2; Err2Pars" + Environment.NewLine +
                                     "Ошибка: Err3; Source2; Err3Pars" + Environment.NewLine +
                                     "Предупреждение: War0; Source; War0Pars" + Environment.NewLine + 
                                     "Предупреждение: War1; Source; War1Pars", c.ErrorMessage());
            Assert.AreEqual(ErrorMessage, c.ErrorMessage());
            Assert.AreEqual("Err2; Err2Pars" + Environment.NewLine +
                                     "Err3; Err3Pars" + Environment.NewLine +
                                     "War0; War0Pars" + Environment.NewLine +
                                     "War1; War1Pars", c.ErrorMessage(false, true, false));
            Assert.AreEqual(ErrorMessage, c.ErrorMessage());
            Assert.AreEqual("Ошибка: Err2" + Environment.NewLine +
                                     "Ошибка: Err3" + Environment.NewLine +
                                     "Предупреждение: War0" + Environment.NewLine +
                                     "Предупреждение: War1", c.ErrorMessage(false, false));
            Assert.AreEqual(ErrorMessage, c.ErrorMessage());
        }

        [TestMethod]
        public void BreakEx()
        {
            RunHistory();
            var c = (CommandCollect)StartCollect(false, true).Run(() =>
                {
                    WasBreaked = true;
                    StartLog("aa");
                },
                () => { SetCollectResults("Good"); });
            Assert.AreEqual("Good", CommandResults);
            Assert.IsTrue(c.IsBreaked);
            Assert.IsTrue(c.IsFinished);
            Assert.AreEqual(CommandQuality.Success, c.Quality);
            Assert.AreEqual("Прервано", c.Status);
            Assert.AreEqual(0, Logs.Count);
            Assert.AreEqual("", c.ErrorMessage());

            c = (CommandCollect)StartCollect(false, true).Run(() =>
                {
                    StartProgress("Progress", "Progress");
                    Assert.AreEqual("Progress", TabloText(0));
                    StartLog("Log");
                    Assert.AreEqual("Log", TabloText(1));
                    AddEvent("Event");
                    WasBreaked = true;
                    AddError("Error");
                });
            Assert.AreEqual("", TabloText(0));
            Assert.AreEqual("", TabloText(1));
            Assert.AreEqual(1, Logs.Count);
            Assert.AreEqual(2, Logs[0].Events.Count);
            Assert.AreEqual("Event", Logs[0].Events[0].Description);
            Assert.AreEqual("Прервано", Logs[0].Status);

            Assert.IsTrue(c.IsBreaked);
            Assert.IsTrue(c.IsFinished);
            Assert.AreEqual(CommandQuality.Success, c.Quality);
            Assert.AreEqual("Прервано", c.Status);
            Assert.AreEqual("", c.ErrorMessage());
        }

        [TestMethod]
        public void CommDanger()
        {
            RunHistory();
            var c = RunDanger(3, 2, LoggerDangerness.Single, false, true);
            Assert.AreEqual(9, Logs[0].Events.Count);
            Assert.AreEqual("Предупреждение: Повтор операции", c.ErrorMessage(false, false));
            Assert.AreEqual(CommandQuality.Repeat, c.Quality);

            c = RunDanger(3, 0, LoggerDangerness.Single, false, true);
            Assert.AreEqual(1, Logs[1].Events.Count);
            Assert.AreEqual("", c.ErrorMessage(false, false));
            Assert.AreEqual(CommandQuality.Success, c.Quality);

            c = RunDanger(3, 3, LoggerDangerness.Single, false, true);
            Assert.AreEqual(10, Logs[2].Events.Count);
            Assert.AreEqual("Ошибка: Исключение" + Environment.NewLine + "Предупреждение: Повтор операции", c.ErrorMessage(false, false));
            Assert.AreEqual(CommandQuality.Error, c.Quality);

            c = RunDanger(3, 2, LoggerDangerness.Single, true, true);
            Assert.AreEqual(9, Logs[3].Events.Count);
            Assert.AreEqual("Предупреждение: Повтор операции", c.ErrorMessage(false, false));
            Assert.AreEqual(CommandQuality.Repeat, c.Quality);

            c = RunDanger(3, 3, LoggerDangerness.Periodic, false, true);
            Assert.AreEqual(2, Logs[4].Events.Count);
            Assert.AreEqual("Ошибка: Исключение", c.ErrorMessage(false, false));
            Assert.AreEqual(CommandQuality.Error, c.Quality);

            c = RunDanger(7, 5, LoggerDangerness.RealTime, false, true);
            Assert.AreEqual(21, Logs[5].Events.Count);
            Assert.AreEqual("Предупреждение: Повтор операции", c.ErrorMessage(false, false));
            Assert.AreEqual(CommandQuality.Repeat, c.Quality);

            c = RunDanger(2, 1, LoggerDangerness.Single, false, true);
            Assert.AreEqual(5, Logs[6].Events.Count);
            Assert.AreEqual("Предупреждение: Повтор операции", c.ErrorMessage(false, false));
            Assert.AreEqual(CommandQuality.Repeat, c.Quality);

            c = RunDanger(3, 2, LoggerDangerness.Single, false, false);
            Assert.AreEqual(9, Logs[7].Events.Count);
            Assert.AreEqual("Предупреждение: Повтор операции", c.ErrorMessage(false, false));
            Assert.AreEqual(CommandQuality.Repeat, c.Quality);

            c = RunDanger(3, 3, LoggerDangerness.Single, false, false);
            Assert.AreEqual(10, Logs[8].Events.Count);
            Assert.AreEqual("Ошибка: Ошибка" + Environment.NewLine + "Предупреждение: Повтор операции", c.ErrorMessage(false, false));
            Assert.AreEqual(CommandQuality.Error, c.Quality);
        }

        private CommandCollect RunDanger(int reps, int repserr, LoggerDangerness dangerness, bool useThread, bool useException)
        {
            StartCollect(false, true);
            StartLog("Log");
            int num = Logs.Count - 1;
            Assert.AreEqual(0, Logs[num].Events.Count);
            int i = 0;
            StartDanger(reps, dangerness, "Исключение", "Повтор операции", useThread).Run(() =>
            {
                Assert.AreEqual(i * 4, Logs[num].Events.Count);
                AddEvent("Попытка");
                Assert.AreEqual(i * 4 + 1, Logs[num].Events.Count);
                if (i++ < repserr)
                {
                    if (useException) throw new Exception("Err");
                    AddError("Ошибка");
                }
            }, () =>
            {
                Assert.AreEqual(i * 4 - 1, Logs[num].Events.Count);
                AddEvent("Повтор");
                Assert.AreEqual(i * 4, Logs[num].Events.Count);
            });
            return FinishCollect();
        }

        [TestMethod]
        public void CommDangerComplex()
        {
            RunHistory();
            StartCollect(false, true);
            StartLog("Log");
            Assert.AreEqual(0, Logs[0].Events.Count);
            int i = 0;
            var cden = StartDanger(3, LoggerDangerness.Single, "Исключение", "Повтор операции", true);
            cden.Run(() =>
            {
                Assert.AreEqual(i * 4, Logs[0].Events.Count);
                Assert.AreSame(cden, Command);
                AddEvent("Попытка");
                var com = Start();
                Assert.AreSame(com, Command);
                Assert.AreEqual(i * 4 + 1, Logs[0].Events.Count);
                if (i++ < 2) throw new Exception("Err");
            }, () =>
            {
                Assert.AreEqual(i * 4 - 1, Logs[0].Events.Count);
                AddEvent("Повтор");
                Assert.AreEqual(i * 4, Logs[0].Events.Count);
            });
            Assert.AreSame(CommandLog, Command);
            var c = FinishCollect();
            Assert.AreEqual(9, Logs[0].Events.Count);
            Assert.AreEqual("Предупреждение: Повтор операции", c.ErrorMessage(false, false));
            Assert.AreEqual(CommandQuality.Repeat, c.Quality);

            StartCollect(false, true);
            StartLog("Log");
            Assert.AreEqual(0, Logs[1].Events.Count);
            int k = 0, n = 0;
            StartDanger(3, LoggerDangerness.Single, "Исключение", "Повтор операции").Run(() =>
                StartDanger(3, LoggerDangerness.Single, "Исключение", "Повтор операции").Run(() =>
                {
                    Assert.AreEqual(k*4 - n, Logs[1].Events.Count);
                    AddEvent("Попытка");
                    Assert.AreEqual(k*4 + 1 - n, Logs[1].Events.Count);
                    if (k++ < 5) throw new Exception("Err");
                }, () => 
                {
                    Assert.AreEqual(k*4 - 1 - n, Logs[1].Events.Count);
                    AddEvent("Повтор");
                    Assert.AreEqual(k*4 - n, Logs[1].Events.Count);
                }), () => n++);
            c = FinishCollect();
            Assert.AreEqual(20, Logs[1].Events.Count);
            Assert.AreEqual("Предупреждение: Повтор операции", c.ErrorMessage(false, false));
            Assert.AreEqual(CommandQuality.Repeat, c.Quality);

            StartCollect(false, true);
            StartLog("Log");
            Assert.AreEqual(0, Logs[2].Events.Count);
            k = 0; n = 0;
            StartDanger(2, LoggerDangerness.Single, "Исключение", "Повтор операции").Run(() =>
                StartDanger(2, LoggerDangerness.Single, "Исключение", "Повтор операции").Run(() =>
                {
                    Assert.AreEqual(k * 4 - n, Logs[2].Events.Count);
                    AddEvent("Попытка");
                    Assert.AreEqual(k * 4 + 1 - n, Logs[2].Events.Count);
                    if (k++ < 7) AddError("Ошибка");
                }, () =>
                {
                    Assert.AreEqual(k * 4 - 1 - n, Logs[2].Events.Count);
                    AddEvent("Повтор");
                    Assert.AreEqual(k * 4 - n, Logs[2].Events.Count);
                }), () => n++);
            c = FinishCollect();
            Assert.AreEqual(13, Logs[2].Events.Count);
            Assert.AreEqual("Ошибка: Ошибка" + Environment.NewLine + "Предупреждение: Повтор операции", c.ErrorMessage(false, false));
            Assert.AreEqual(CommandQuality.Error, c.Quality);

            StartCollect(false, true).Run(() =>
                {
                    StartLog("Log");
                    Assert.AreEqual(0, Logs[3].Events.Count);
                    var cd = StartDanger(2, LoggerDangerness.Single, "Исключение", "Повтор операции").Run(() =>
                        {
                            AddEvent("Событие");
                            Break();
                            Thread.Sleep(6000);
                            AddEvent("Событие");
                        });
                    Assert.IsTrue(cd.IsBreaked);
                    Assert.AreEqual(CommandQuality.Success, cd.Quality);
                });
            Assert.AreEqual(2, Logs[3].Events.Count);
            Assert.AreEqual("Событие", Logs[3].Events[0].Description);
            Assert.AreEqual("Прерывание команды", Logs[3].Events[1].Description);
        }
    }
}