﻿using System;
using BaseLibrary;
using BaseLibraryTest;
using CommonTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProvidersLibrary;

namespace ProvidersTest
{
    [TestClass]
    public class CloneTest
    {
        private SourceConnect MakeCloneConnect(string prefix)
        {
            TestLib.CopyDir(@"Providers\Fictive", "TestClone", "Clone" + prefix);
            var factory = new ProvidersFactory();
            var logger = new Logger(new AppIndicator());
            logger.History = new TestHistory(logger);
            var connect = factory.CreateConnect(logger, ProviderType.Source, "TestSource", "Clones");
            connect.JoinProvider(factory.CreateProvider(logger, "CloneSource", "CloneDir=" + TestLib.TestRunDir + @"Providers\Fictive\Clone" + prefix));
            return (SourceConnect)connect;
        }

        private static void GetValues(SourceConnect connect, DateTime beg, DateTime en)
        {
            connect.Logger.StartPeriod(beg, en);
            connect.GetValues();
            connect.Logger.FinishPeriod();
        }

        private DateTime D(int m, int s = 0)
        {
            return new DateTime(2016, 7, 8).AddMinutes(m).AddSeconds(s);
        }

        [TestMethod]
        public void CloneProps()
        {
            var connect = MakeCloneConnect("Props");
            var source = (CloneSource)connect.Provider;
            Assert.IsNotNull(connect);
            Assert.AreEqual(ProviderType.Source, connect.Type);
            Assert.AreEqual("TestSource", connect.Code);
            Assert.AreEqual("CloneSource", connect.Provider.Code);
            Assert.AreEqual("Clones", connect.Complect);
            Assert.AreEqual("TestSource", connect.Context);
            Assert.IsNotNull(connect.Logger);
            Assert.AreEqual(0, connect.CalcSignals.Count);
            Assert.AreEqual(0, connect.ReadingSignals.Count);

            Assert.AreEqual(TestLib.TestRunDir + @"Providers\Fictive\CloneProps\Clone.accdb", source.CloneFile);
            Assert.AreEqual("TestSource", connect.Context);
            Assert.AreEqual(source, connect.Provider);
            Assert.IsTrue(source.Connect());

            Assert.AreEqual(D(0), connect.GetTime().Begin);
            Assert.AreEqual(D(30), connect.GetTime().End);
        }
    }
}