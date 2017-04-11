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
        public LogTest() 
            : base(new TestHistory(), new TestIndicator())
        { }

        private void RunHistory()
        {
            History = _history = new TestHistory();
            History.Logger = this;
            Indicator = _indicator = new TestIndicator();
        }
        private TestHistory _history;
        private List<TestCommandLog> Logs { get { return _history.Logs; } }
        private List<TestCommandSuper> Supers { get { return _history.Supers; } }
        private List<TestErrorLog> Errors { get { return _history.Errors; } }

        private TestIndicator _indicator;
        
        [TestMethod]
        public void CommLog()
        {
            RunHistory();
            StartLog("Op0", "Source", "Par0");
            Assert.IsNotNull(LogCommand);
            Assert.AreEqual(LogCommand, Command);
            Assert.IsNull(ProgressCommand);
            Assert.IsNull(CollectCommand);
            Assert.AreEqual(0, Procent);
            Assert.AreEqual(0, LogCommand.Procent);
            Assert.AreEqual(0, Command.Procent);
            Assert.AreEqual(CommandQuality.Success, LogCommand.Quality);
            Assert.IsTrue(LogCommand.IsSuccess);
            Assert.IsFalse(LogCommand.IsFinished);
            Assert.AreEqual("Op0 (Source)", _indicator.TabloText1);

            Assert.AreEqual(1, Logs.Count);
            Assert.AreEqual("Op0", Logs[0].Command);
            Assert.AreEqual("Par0", Logs[0].Params);
            Assert.AreEqual("Source", Logs[0].Context);
            Assert.AreEqual("Запущено", Logs[0].Status);
            Assert.AreEqual(null, Logs[0].Results);
            Assert.AreEqual(LogCommand.StartTime, Logs[0].Time);
            Assert.AreEqual(0, Logs[0].Events.Count);

            AddEvent("Event", "EventPar");
            Assert.AreEqual(1, Logs[0].Events.Count);
            Assert.AreEqual(CommandQuality.Success, LogCommand.Quality);
            Assert.AreEqual("Event", Logs[0].Events[0].Description);
            Assert.AreEqual("EventPar", Logs[0].Events[0].Params);
            Assert.AreEqual(null, Logs[0].Events[0].Status);

            AddWarning("Warning", null, "WarningPars");
            Assert.AreEqual(2, Logs[0].Events.Count);
            Assert.AreEqual(CommandQuality.Warning, LogCommand.Quality);
            Assert.AreEqual("Warning", Logs[0].Events[1].Description);
            Assert.AreEqual("WarningPars", Logs[0].Events[1].Params);
            Assert.AreEqual("Предупреждение", Logs[0].Events[1].Status);

            AddError("Error", null, "ErrorPars");
            Assert.AreEqual(3, Logs[0].Events.Count);
            Assert.AreEqual(CommandQuality.Error, LogCommand.Quality);
            Assert.IsFalse(LogCommand.IsSuccess);
            Assert.AreEqual("Error", Logs[0].Events[2].Description);
            Assert.AreEqual("ErrorPars", Logs[0].Events[2].Params);
            Assert.AreEqual("Ошибка", Logs[0].Events[2].Status);

            Command c = FinishLog("Results");
            Assert.IsTrue(c.IsFinished);
            Assert.IsNull(LogCommand);
            Assert.IsNull(Command);
            Assert.AreEqual(0, Procent);
            Assert.AreEqual(1, Logs.Count);
            Assert.AreEqual("Op0", Logs[0].Command);
            Assert.AreEqual("Par0", Logs[0].Params);
            Assert.AreEqual("Source", Logs[0].Context);
            Assert.AreEqual("Ошибка", Logs[0].Status);
            Assert.AreEqual("Results", Logs[0].Results);
            Assert.AreEqual(3, Logs[0].Events.Count);
            Assert.AreEqual("", _indicator.TabloText1);

            StartLog("Op1", "Source", "Par1").Run(() =>
                {
                    Assert.IsNotNull(LogCommand);
                    Assert.AreSame(LogCommand, Command);
                    Assert.AreEqual(CommandQuality.Success, LogCommand.Quality);
                    Assert.IsFalse(LogCommand.IsFinished);
                    Assert.AreEqual("Op1 (Source)", _indicator.TabloText1);

                    AddWarning("Warning", null, "WarningPars");
                    Assert.AreEqual(1, Logs[1].Events.Count);
                    Assert.AreEqual(CommandQuality.Warning, LogCommand.Quality);
                    Assert.AreEqual("Warning", Logs[1].Events[1].Description);
                    Assert.AreEqual("WarningPars", Logs[1].Events[1].Params);
                    Assert.AreEqual("Предупреждение", Logs[0].Events[1].Status);

                    var cc = Start(0, 100).Run(() =>
                    {
                        Assert.IsNotNull(Command);
                        Assert.AreNotEqual(LogCommand, Command);
                        Assert.AreEqual(LogCommand, Command.Parent);

                        AddEvent("Event", "EventPar");
                        Assert.AreEqual(2, Logs[1].Events.Count);
                        Assert.AreEqual(CommandQuality.Warning, LogCommand.Quality);
                        Assert.AreEqual("Event", Logs[1].Events[1].Description);
                        Assert.AreEqual("EventPar", Logs[1].Events[1].Params);
                        Assert.AreEqual(null, Logs[0].Events[1].Status);
                    });
                    Assert.IsTrue(cc.IsFinished);
                });

            Assert.IsNull(LogCommand);
            Assert.IsNull(Command);
            Assert.AreEqual(2, Logs.Count);
            Assert.AreEqual("Op1", Logs[1].Command);
            Assert.AreEqual("Par1", Logs[1].Params);
            Assert.AreEqual("Source", Logs[1].Context);
            Assert.AreEqual("Ошибка", Logs[1].Status);
            Assert.IsNull(Logs[1].Results);
            Assert.AreEqual(2, Logs[1].Events.Count);

            StartLog("Op2", "Source", "Par2");
            Assert.IsNotNull(LogCommand);
            Assert.AreEqual("Op2", LogCommand.Name);
            Assert.AreEqual("Op2 (Source)", _indicator.TabloText1);

            c = StartLog("Op3", "Source", "Par3").Run(() =>
                {
                    Assert.IsNotNull(LogCommand);
                    Assert.AreEqual("Op3", LogCommand.Name);
                    Assert.AreEqual("Op3 (Source)", _indicator.TabloText1);

                    Assert.AreEqual(4, Logs.Count);
                    Assert.AreEqual("Op3", Logs[3].Command);
                    Assert.AreEqual("Par3", Logs[3].Params);
                    Assert.AreEqual("Source", Logs[3].Context);
                    Assert.AreEqual("Запущено", Logs[3].Status);
                    Assert.AreEqual(null, Logs[3].Results);
                    Assert.AreEqual(0, Logs[3].Events.Count);

                    AddEvent("Event", "EventPar");
                    Assert.AreEqual(1, Logs[3].Events.Count);
                    Assert.AreEqual(CommandQuality.Success, LogCommand.Quality);
                    Assert.AreEqual("Event", Logs[3].Events[0].Description);
                    Assert.AreEqual("EventPar", Logs[3].Events[0].Params);
                    Assert.AreEqual(null, Logs[3].Events[0].Status);
                    SetLogResults("Res");
                });
            Assert.IsTrue(c.IsFinished);
            Assert.AreEqual("", _indicator.TabloText1);

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
            StartProgress("Процесс", "Pars", DateTime.Now.AddHours(1));
            Assert.IsNotNull(ProgressCommand);
            Assert.AreSame(ProgressCommand, Command);
            Assert.AreEqual(1, Supers.Count);
            Assert.AreEqual("Процесс", Supers[0].Command);
            Assert.AreEqual("Pars", Supers[0].Params);
            Assert.AreEqual("Запущено", Supers[0].Status);
            Assert.AreEqual(ProgressCommand.StartTime, Supers[0].Time);
            Assert.AreEqual("Процесс", _indicator.TabloText0);
            
            Assert.AreEqual(0, Procent);
            Procent = 10;
            Assert.AreEqual(10, Procent);
            Assert.AreEqual(10, ProgressCommand.Procent);
            Assert.AreEqual(10, _indicator.Procent);

            Start(20, 70);
            Assert.AreSame(ProgressCommand, Command.Parent);
            Assert.AreEqual("Процесс", _indicator.TabloText0);
            Assert.AreEqual(20, ProgressCommand.Procent);
            Assert.AreEqual(20, _indicator.Procent);
            Assert.AreEqual(0, Procent);
            Assert.AreEqual(0, Command.Procent);
            Procent = 20;
            Assert.AreEqual(30, ProgressCommand.Procent);
            Assert.AreEqual(30, _indicator.Procent);
            Assert.AreEqual(20, Procent);
            Assert.AreEqual(20, Command.Procent);

            Assert.IsNull(_indicator.TabloText2);
            StartIndicatorText(20, 30, "Text3").Run(() =>
                {
                    Assert.AreEqual(30, ProgressCommand.Procent);
                    Assert.AreEqual(30, _indicator.Procent);
                    Assert.AreEqual(0, Procent);
                    Assert.AreEqual("Text3", _indicator.TabloText2);
                });

            StartLog(40, 80, "Log", "Mod");
            Assert.IsNotNull(LogCommand);
            Assert.AreEqual(ProgressCommand, LogCommand.Parent.Parent);
            Assert.AreEqual(Command, LogCommand);
            Assert.AreEqual(40, ProgressCommand.Procent);
            Assert.AreEqual(40, _indicator.Procent);
            Assert.AreEqual(0, Procent);
            Assert.AreEqual(1, Supers[0].Logs.Count);
            Assert.AreEqual("Log", Supers[0].Logs[0].Command);
            Assert.AreEqual("", Supers[0].Logs[0].Params);
            Assert.AreEqual(1, Logs.Count);
            Assert.AreEqual("Процесс", _indicator.TabloText0);
            Assert.AreEqual("Log (Mod)", _indicator.TabloText1);

            AddEvent("Event", "Pars", 50);
            Assert.AreEqual(50, ProgressCommand.Procent);
            Assert.AreEqual(50, _indicator.Procent);
            Assert.AreEqual(50, Procent);
            Assert.AreEqual(1, Logs[0].Events.Count);
            Assert.AreEqual("Event", Logs[0].Events[0].Description);
            Assert.AreEqual(CommandQuality.Success, LogCommand.Quality);
            Assert.AreEqual(CommandQuality.Success, ProgressCommand.Quality);

            AddError("Error");
            Assert.AreEqual(50, ProgressCommand.Procent);
            Assert.AreEqual(50, _indicator.Procent);
            Assert.AreEqual(50, Procent);
            Assert.AreEqual(2, Logs[0].Events.Count);
            Assert.AreEqual("Error", Logs[0].Events[1].Description);
            Assert.AreEqual(CommandQuality.Error, LogCommand.Quality);
            Assert.AreEqual(CommandQuality.Error, ProgressCommand.Quality);
            
            Finish();
            Assert.AreEqual(60, ProgressCommand.Procent);
            Assert.AreEqual(60, _indicator.Procent);
            Assert.AreEqual(80, Procent);
            Assert.AreEqual(CommandQuality.Error, ProgressCommand.Quality);
            Assert.AreEqual("Процесс", _indicator.TabloText0);
            Assert.AreEqual("", _indicator.TabloText1);

            var c = StartLog(80, 100, "Log2", "Mod2").Run(() =>
                {
                    Assert.AreEqual("Процесс", _indicator.TabloText0);
                    Assert.AreEqual("Log2 (Mod2)", _indicator.TabloText1);
                });
            Assert.AreEqual(70, ProgressCommand.Procent);
            Assert.AreEqual(70, _indicator.Procent);
            Assert.AreEqual(100, Procent);
            Assert.AreEqual(100, c.Procent);
            Assert.AreEqual(CommandQuality.Success, c.Quality);
            Assert.AreEqual(2, Logs.Count);
            Assert.AreEqual(2, Supers[0].Logs.Count);
            Assert.AreEqual(CommandQuality.Error, ProgressCommand.Quality);

            StartIndicatorText(20, 30, "Text3_1");
            Assert.AreEqual("Процесс", _indicator.TabloText0);
            Assert.AreEqual("", _indicator.TabloText1);
            Assert.AreEqual("Text3_1", _indicator.TabloText2);
            FinishIndicatorText();

            c = FinishProgress();
            Assert.AreEqual("", _indicator.TabloText0);
            Assert.AreEqual("", _indicator.TabloText1);
            Assert.AreEqual("", _indicator.TabloText2);
            Assert.AreEqual(100, c.Procent);
            Assert.AreEqual(100, _indicator.Procent);
            Assert.AreEqual(CommandQuality.Error, c.Quality);
            Assert.AreEqual("Ошибка", c.Status);
        }

        [TestMethod]
        public void CommCollect()
        {
            RunHistory();
            History.ClearErrorsList(); 
            StartCollect(true, true);
            Assert.IsNotNull(CollectCommand);
            Assert.AreSame(Command, CollectCommand);
            Assert.AreEqual(0, Logs.Count);
            Assert.AreEqual(0, Supers.Count);
            Assert.IsNull(_indicator.TabloText1);

            var beg = new DateTime(2017, 1, 1);
            var en = new DateTime(2017, 1, 2);
            StartPeriod(beg, en, "Mode");
            StartProgress("Progress", "Pars");
            Assert.AreEqual(1, Supers.Count);
            Assert.AreSame(CollectCommand, ProgressCommand.Parent.Parent);
            Assert.AreEqual(beg, Supers[0].PeriodBegin);
            Assert.AreEqual(en, Supers[0].PeriodEnd);
            Assert.AreEqual("Mode", Supers[0].PeriodMode);
            Assert.AreEqual("Progress", Supers[0].Command);
            Assert.AreEqual(beg, PeriodBegin);
            Assert.AreEqual(en, PeriodEnd);
            Assert.AreEqual("Mode", PeriodMode);

            StartLog("Log0", "Source", "Log0Pars");
            Assert.AreEqual(1, Supers[0].Logs.Count);
            Assert.AreSame(ProgressCommand, LogCommand.Parent);
            Assert.AreEqual("Log0 (Source)", _indicator.TabloText1);
            Assert.AreEqual(beg, PeriodBegin);
            Assert.AreEqual(en, PeriodEnd);
            
            AddWarning("War0", null, "War0Pars");
            Assert.AreEqual(1, Logs[0].Events.Count);
            Assert.AreEqual(1, Errors.Count);
            Assert.AreEqual(beg, Errors[0].PeriodBegin);
            Assert.AreEqual(en, Errors[0].PeriodEnd);
            Assert.AreEqual("Source", Errors[0].Context);
            Assert.AreEqual("War0", Errors[0].Description);
            Assert.AreEqual("Log0", Errors[0].Command);
            Assert.AreEqual("War0Pars", Errors[0].Params);
            Assert.AreEqual("Предупреждение", Errors[0].Status);
            Assert.AreEqual(CommandQuality.Warning, CollectCommand.Quality);

            AddWarning("War1", null, "War1Pars");
            Assert.AreEqual(2, Logs[0].Events.Count);
            Assert.AreEqual(2, Errors.Count);
            Assert.AreEqual(beg, Errors[1].PeriodBegin);
            Assert.AreEqual(en, Errors[1].PeriodEnd);
            Assert.AreEqual("Source", Errors[1].Context);
            Assert.AreEqual("War1", Errors[1].Description);
            Assert.AreEqual("Log0", Errors[1].Command);
            Assert.AreEqual("War1Pars", Errors[1].Params);
            Assert.AreEqual("Предупреждение", Errors[1].Status);
            Assert.AreEqual(CommandQuality.Warning, CollectCommand.Quality);

            StartLog("Log1", "Source2", "Log1Pars");
            Assert.AreEqual(2, Logs.Count);
            Assert.AreSame(ProgressCommand, LogCommand.Parent);
            Assert.AreEqual("Log1 (Source2)", _indicator.TabloText1);

            AddError("Err2", null, "Err2Pars");
            Assert.AreEqual(1, Logs[1].Events.Count);
            Assert.AreEqual(3, Errors.Count);
            Assert.AreEqual(beg, Errors[2].PeriodBegin);
            Assert.AreEqual(en, Errors[2].PeriodEnd);
            Assert.AreEqual("Source2", Errors[2].Context);
            Assert.AreEqual("Err2", Errors[2].Description);
            Assert.AreEqual("Log1", Errors[2].Command);
            Assert.AreEqual("Err2Pars", Errors[2].Params);
            Assert.AreEqual("Ошибка", Errors[2].Status);
            Assert.AreEqual(CommandQuality.Error, CollectCommand.Quality);

            try { Command = CollectCommand.Parent.Parent;}
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
            Assert.AreEqual(beg, Errors[3].PeriodBegin);
            Assert.AreEqual(en, Errors[3].PeriodEnd);
            Assert.AreEqual("Source2", Errors[3].Context);
            Assert.AreEqual("Err3", Errors[3].Description);
            Assert.AreEqual("Log1", Errors[3].Command);
            Assert.AreEqual("Err3Pars", Errors[3].Params);
            Assert.AreEqual("Ошибка", Errors[3].Status);
            Assert.AreEqual(CommandQuality.Error, CollectCommand.Quality);

            FinishProgress();
            Assert.AreEqual("", _indicator.TabloText1);
            Assert.AreEqual("", _indicator.TabloText0);
            Assert.AreEqual(beg, PeriodBegin);
            Assert.AreEqual(en, PeriodEnd);
            Assert.AreEqual("Mode", PeriodMode);

            Assert.AreEqual(1, Supers.Count);
            Assert.IsNull(ProgressCommand);
            Assert.AreEqual(CommandQuality.Error, CollectCommand.Quality);

            var c = FinishCollect("Results");
            Assert.AreEqual("Results", CollectedResults);
            Assert.IsNull(CollectCommand);
            Assert.IsNull(Command);
            Assert.AreEqual(CommandQuality.Error, c.Quality);
            Assert.AreEqual("Ошибка: Err2; Source2; Err2Pars" + Environment.NewLine +
                                     "Ошибка: Err3; Source2; Err3Pars" + Environment.NewLine +
                                     "Предупреждение: War0; Source; War0Pars" + Environment.NewLine + 
                                     "Предупреждение: War1; Source; War1Pars", c.ErrorMessage());
            Assert.AreEqual(CollectedError, c.ErrorMessage());
            Assert.AreEqual("Err2; Err2Pars" + Environment.NewLine +
                                     "Err3; Err3Pars" + Environment.NewLine +
                                     "War0; War0Pars" + Environment.NewLine +
                                     "War1; War1Pars", c.ErrorMessage(false, true, false));
            Assert.AreEqual(CollectedError, c.ErrorMessage());
            Assert.AreEqual("Ошибка: Err2" + Environment.NewLine +
                                     "Ошибка: Err3" + Environment.NewLine +
                                     "Предупреждение: War0" + Environment.NewLine +
                                     "Предупреждение: War1", c.ErrorMessage(false, false));
            Assert.AreEqual(CollectedError, c.ErrorMessage());
        }

        [TestMethod]
        public void BreakEx()
        {
            RunHistory();
            var c = (CollectCommand)StartCollect(false, true).Run(() =>
                {
                    WasBreaked = true;
                    StartLog("aa");
                },
                () => { AddCollectResult("Good"); });
            Assert.AreEqual("Good", CollectedResults);
            Assert.IsTrue(c.IsBreaked);
            Assert.IsTrue(c.IsFinished);
            Assert.AreEqual(CommandQuality.Success, c.Quality);
            Assert.AreEqual("Прервано", c.Status);
            Assert.AreEqual(0, Logs.Count);
            Assert.AreEqual("", c.ErrorMessage());

            c = (CollectCommand)StartCollect(false, true).Run(() =>
                {
                    StartProgress("Progress", "Progress");
                    Assert.AreEqual("Progress", _indicator.TabloText0);
                    StartLog("Log");
                    Assert.AreEqual("Log", _indicator.TabloText1);
                    AddEvent("Event");
                    WasBreaked = true;
                    AddError("Error");
                });
            Assert.AreEqual("", _indicator.TabloText0);
            Assert.AreEqual("", _indicator.TabloText1);
            Assert.AreEqual(1, Logs.Count);
            Assert.AreEqual(2, Logs[0].Events.Count);
            Assert.AreEqual("Event", Logs[0].Events[0].Description);
            Assert.AreEqual("Прервано", Logs[0].Status);

            Assert.IsTrue(c.IsBreaked);
            Assert.IsTrue(c.IsFinished);
            Assert.AreEqual(CommandQuality.Success, c.Quality);
            Assert.AreEqual("Прервано", c.Status);
            Assert.AreEqual("", c.ErrorMessage());

            c = (CollectCommand)StartCollect(false, true).Run(
                () =>
                {
                    StartLog("L");
                    throw new Exception("Er");
                },
                () => { AddCollectResult("Bad"); });
            Assert.AreEqual("Ошибка: Ошибка", CollectedError);
            Assert.AreEqual("Bad", CollectedResults);
            Assert.IsTrue(c.IsFinished);
            Assert.AreEqual(CommandQuality.Error, c.Quality);
            Assert.AreEqual(2, Logs.Count);
            Assert.AreEqual(0, Logs[1].Events.Count);
            Assert.AreEqual("Ошибка: Ошибка", c.ErrorMessage());
        }

        [TestMethod]
        public void CommDanger()
        {
            RunHistory();
            var c = RunDanger(3, 2, LoggerStability.Single, false, true);
            Assert.AreEqual(10, Logs[0].Events.Count);
            Assert.AreEqual("Предупреждение: Операция. Повтор операции из-за ошибки", c.ErrorMessage(false, false));
            Assert.AreEqual(CommandQuality.Repeat, c.Quality);

            c = RunDanger(3, 0, LoggerStability.Single, false, true);
            Assert.AreEqual(2, Logs[1].Events.Count);
            Assert.AreEqual("", c.ErrorMessage(false, false));
            Assert.AreEqual(CommandQuality.Success, c.Quality);

            c = RunDanger(3, 3, LoggerStability.Single, false, true);
            Assert.AreEqual(11, Logs[2].Events.Count);
            Assert.AreEqual("Ошибка: Операция. Ошибка" + Environment.NewLine + "Предупреждение: Операция. Повтор операции из-за ошибки", c.ErrorMessage(false, false));
            Assert.AreEqual(CommandQuality.Error, c.Quality);

            c = RunDanger(3, 2, LoggerStability.Single, true, true);
            Assert.AreEqual(10, Logs[3].Events.Count);
            Assert.AreEqual("Предупреждение: Операция. Повтор операции из-за ошибки", c.ErrorMessage(false, false));
            Assert.AreEqual(CommandQuality.Repeat, c.Quality);

            c = RunDanger(3, 3, LoggerStability.Periodic, false, true);
            Assert.AreEqual(3, Logs[4].Events.Count);
            Assert.AreEqual("Ошибка: Операция. Ошибка", c.ErrorMessage(false, false));
            Assert.AreEqual(CommandQuality.Error, c.Quality);

            c = RunDanger(7, 5, LoggerStability.RealTimeSlow, false, true);
            Assert.AreEqual(22, Logs[5].Events.Count);
            Assert.AreEqual("Предупреждение: Операция. Повтор операции из-за ошибки", c.ErrorMessage(false, false));
            Assert.AreEqual(CommandQuality.Repeat, c.Quality);

            c = RunDanger(2, 1, LoggerStability.Single, false, true);
            Assert.AreEqual(6, Logs[6].Events.Count);
            Assert.AreEqual("Предупреждение: Операция. Повтор операции из-за ошибки", c.ErrorMessage(false, false));
            Assert.AreEqual(CommandQuality.Repeat, c.Quality);

            c = RunDanger(3, 2, LoggerStability.Single, false, false);
            Assert.AreEqual(10, Logs[7].Events.Count);
            Assert.AreEqual("Предупреждение: Операция. Повтор операции из-за ошибки", c.ErrorMessage(false, false));
            Assert.AreEqual(CommandQuality.Repeat, c.Quality);

            c = RunDanger(3, 3, LoggerStability.Single, false, false);
            Assert.AreEqual(11, Logs[8].Events.Count);
            Assert.AreEqual("Ошибка: Ошибка" + Environment.NewLine + "Предупреждение: Операция. Повтор операции из-за ошибки", c.ErrorMessage(false, false));
            Assert.AreEqual(CommandQuality.Error, c.Quality);
        }

        private CollectCommand RunDanger(int reps, int repserr, LoggerStability stability, bool useThread, bool useException)
        {
            StartCollect(false, true);
            StartLog("Log");
            int num = Logs.Count - 1;
            Assert.AreEqual(0, Logs[num].Events.Count);
            int i = 0;
            StartDanger(reps, stability, "Операция", useThread).Run(() =>
            {
                Assert.AreEqual(i * 4 + 1, Logs[num].Events.Count);
                AddEvent("Попытка");
                Assert.AreEqual(i * 4 + 2, Logs[num].Events.Count);
                if (i++ < repserr)
                {
                    if (useException) throw new Exception("Err");
                    AddError("Ошибка");
                }
            }, () =>
            {
                Assert.AreEqual(i * 4, Logs[num].Events.Count);
                AddEvent("Повтор");
                Assert.AreEqual(i * 4 + 1, Logs[num].Events.Count);
                return true;
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
            var cden = StartDanger(3, LoggerStability.Single, "Операция", true);
            cden.Run(() =>
            {
                Assert.AreEqual(i * 4 + 1, Logs[0].Events.Count);
                Assert.AreSame(cden, Command);
                AddEvent("Попытка");
                var com = Start(0, 100);
                Assert.AreSame(com, Command);
                Assert.AreEqual(i * 4 + 2, Logs[0].Events.Count);
                if (i++ < 2) throw new Exception("Err");
            }, () =>
            {
                Assert.AreEqual(i * 4, Logs[0].Events.Count);
                AddEvent("Повтор");
                Assert.AreEqual(i * 4 + 1, Logs[0].Events.Count);
                return true;
            });
            Assert.AreSame(LogCommand, Command);
            var c = FinishCollect();
            Assert.AreEqual(10, Logs[0].Events.Count);
            Assert.AreEqual("Предупреждение: Операция. Повтор операции из-за ошибки", c.ErrorMessage(false, false));
            Assert.AreEqual(CommandQuality.Repeat, c.Quality);

            StartCollect(false, true);
            StartLog("Log");
            Assert.AreEqual(0, Logs[1].Events.Count);
            int k = 0, n = 0;
            StartDanger(3, LoggerStability.Single, "Операция").Run(() =>
                StartDanger(3, LoggerStability.Single, "Операция").Run(() =>
                {
                    Assert.AreEqual(k*4 + 2, Logs[1].Events.Count);
                    AddEvent("Попытка");
                    Assert.AreEqual(k*4 + 3, Logs[1].Events.Count);
                    if (k++ < 5) throw new Exception("Err");
                }, () => 
                {
                    Assert.AreEqual(k*4 + 1 , Logs[1].Events.Count);
                    AddEvent("Повтор");
                    Assert.AreEqual(k*4 + 2, Logs[1].Events.Count);
                    return true;
                }), () => { n++; return true;});
            c = FinishCollect();
            Assert.AreEqual(23, Logs[1].Events.Count);
            Assert.AreEqual("Предупреждение: Операция. Повтор операции из-за ошибки", c.ErrorMessage(false, false));
            Assert.AreEqual(CommandQuality.Repeat, c.Quality);

            StartCollect(false, true);
            StartLog("Log");
            Assert.AreEqual(0, Logs[2].Events.Count);
            k = 0; n = 0;
            StartDanger(2, LoggerStability.Single, "Операция").Run(() =>
                StartDanger(2, LoggerStability.Single, "Операция").Run(() =>
                {
                    Assert.AreEqual(k*4 + 2, Logs[2].Events.Count);
                    AddEvent("Попытка");
                    Assert.AreEqual(k*4 + 3, Logs[2].Events.Count);
                    if (k++ < 7) AddError("Ошибка");
                }, () =>
                {
                    Assert.AreEqual(k*4 + 1, Logs[2].Events.Count);
                    AddEvent("Повтор");
                    Assert.AreEqual(k*4 + 2, Logs[2].Events.Count);
                    return true;
                }), () => { n++; return true;});
            c = FinishCollect();
            Assert.AreEqual(16, Logs[2].Events.Count);
            Assert.AreEqual("Ошибка: Ошибка" + Environment.NewLine + "Предупреждение: Операция. Повтор операции из-за ошибки", c.ErrorMessage(false, false));
            Assert.AreEqual(CommandQuality.Error, c.Quality);

            StartCollect(false, true).Run(() =>
                {
                    StartLog("Log");
                    Assert.AreEqual(0, Logs[3].Events.Count);
                    var cd = StartDanger(2, LoggerStability.Single, "Операция").Run(() =>
                        {
                            AddEvent("Событие");
                            Break();
                            Thread.Sleep(6000);
                            AddEvent("Событие");
                        });
                    Assert.IsTrue(cd.IsBreaked);
                    Assert.AreEqual(CommandQuality.Success, cd.Quality);
                });
            Assert.AreEqual(3, Logs[3].Events.Count);
            Assert.AreEqual("Операция", Logs[3].Events[0].Description);
            Assert.AreEqual("Событие", Logs[3].Events[1].Description);
            Assert.AreEqual("Прерывание команды", Logs[3].Events[2].Description);
        }
    }
}