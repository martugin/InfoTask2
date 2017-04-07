﻿using BaseLibrary;
using BaseLibraryTest;
using CommonTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Provider;
using ProvidersLibrary;

namespace ProvidersTest
{
    [TestClass]
    public class SimaticSignalsTest
    {

        private SourceConnect MakeProviders()
        {
            var factory = new ProvidersFactory();
            var con = (SourceConnect)factory.CreateConnect(ProviderType.Source, "SourceCon", "Siemens", new Logger());
            var prov = factory.CreateProvider("SimaticSource", "SQLServer" + SysTabl.SubValueS(TestLib.TestRunDir + "TestsSettings.accdb", "SQLServerSettings", "SqlServer"));
            con.JoinProvider(prov);
            return con;
        }

        [TestMethod]
        public void Signals()
        {
            var con = MakeProviders();
            var prov = (SimaticSource)con.Provider;
            Assert.AreEqual("SourceCon", con.Name);
            Assert.AreEqual("Siemens", con.Complect);
            Assert.AreEqual(ProviderType.Source, con.Type);
            Assert.IsNotNull(con.Logger);
            Assert.AreEqual(con.Context, "Источник: SourceCon");
            Assert.IsNotNull(con.Provider);
            Assert.IsTrue(con.Provider is SimaticSource);
            Assert.AreEqual("SimaticSource", prov.Code);
            Assert.AreEqual("Источник: SourceCon, SimaticSource", prov.Context);
            Assert.AreEqual("SQLServer" + SysTabl.SubValueS(TestLib.TestRunDir + "TestsSettings.accdb", "SQLServerSettings", "SqlServer"), prov.Inf);
            Assert.AreSame(con, prov.ProviderConnect);
            Assert.IsNotNull(prov.Logger);
            Assert.IsFalse(prov.IsConnected);
            Assert.IsFalse(prov.IsPrepared);

            Assert.AreEqual(0, con.Signals.Count);
            con.ClearSignals();
            Assert.AreEqual(0, con.Signals.Count);

            con.AddInitialSignal("09ASV00CT002/Т$СК-2А.PV_Out#Value", DataType.Real, "Id=1136;Tag=09ASV00CT002/Т$СК-2А.PV_Out#Value;Archive=SystemArchive");
            con.AddInitialSignal("09ASV00CT002/Т$СК-2А.PV_Out#Value.Quality", DataType.Integer, "Id=1136;Tag=09ASV00CT002/Т$СК-2А.PV_Out#Value;Archive=SystemArchive", "", "Prop=Quality");
            con.AddInitialSignal("09ASV00CT002/Т$СК-2А.PV_Out#Value.Flags", DataType.Integer, "Id=1136;Tag=09ASV00CT002/Т$СК-2А.PV_Out#Value;Archive=SystemArchive", "", "Prop=Flags");
            con.AddInitialSignal("09LBA55CN001XG02/DI.Out#Value", DataType.Boolean, "Id=1947;Tag=09LBA55CN001XG02/DI.Out#Value;Archive=SystemArchive;");
            con.AddInitialSignal("09LBA55CN001XG02/DI.Out#Value.Quality", DataType.Integer, "Id=1947;Tag=09LBA55CN001XG02/DI.Out#Value;Archive=SystemArchive;", "", "Prop=Quality");
            
            Assert.AreEqual(5, con.Signals.Count);
            Assert.AreEqual(5, con.InitialSignals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);
            Assert.IsTrue(con.Signals.ContainsKey("09ASV00CT002/Т$СК-2А.PV_Out#Value"));
            Assert.AreEqual(DataType.Real, con.Signals["09ASV00CT002/Т$СК-2А.PV_Out#Value"].DataType);
            Assert.IsTrue(con.Signals.ContainsKey("09ASV00CT002/Т$СК-2А.PV_Out#Value.Quality"));
            Assert.AreEqual(DataType.Integer, con.Signals["09ASV00CT002/Т$СК-2А.PV_Out#Value.Quality"].DataType);
            Assert.IsTrue(con.InitialSignals.ContainsKey("09ASV00CT002/Т$СК-2А.PV_Out#Value.Flags"));
            Assert.AreEqual(DataType.Integer, con.InitialSignals["09ASV00CT002/Т$СК-2А.PV_Out#Value.Flags"].DataType);
            Assert.IsTrue(con.Signals.ContainsKey("09LBA55CN001XG02/DI.Out#Value"));
            Assert.AreEqual(DataType.Boolean, con.Signals["09LBA55CN001XG02/DI.Out#Value"].DataType);
            Assert.IsTrue(con.Signals.ContainsKey("09LBA55CN001XG02/DI.Out#Value.Quality"));
            Assert.AreEqual(DataType.Integer, con.Signals["09LBA55CN001XG02/DI.Out#Value.Quality"].DataType);

            Assert.IsFalse(prov.IsPrepared);
            prov.Prepare(false);
            Assert.IsTrue(prov.IsPrepared);

            Assert.AreEqual(2, prov.OutsId.Count);
            Assert.IsTrue(prov.OutsId.ContainsKey(1136));
            Assert.IsNotNull(prov.OutsId[1136].ValueSignal);
            Assert.AreEqual(DataType.Real, prov.OutsId[1136].ValueSignal.DataType);
            Assert.IsNotNull(prov.OutsId[1136].QualitySignal);
            Assert.AreEqual(DataType.Integer, prov.OutsId[1136].QualitySignal.DataType);
            Assert.IsNotNull(prov.OutsId[1136].FlagsSignal);
            Assert.AreEqual(DataType.Integer, prov.OutsId[1136].FlagsSignal.DataType);
            Assert.IsNotNull(prov.OutsId[1947].ValueSignal);
            Assert.AreEqual(DataType.Boolean, prov.OutsId[1947].ValueSignal.DataType);
            Assert.IsNotNull(prov.OutsId[1947].QualitySignal);
            Assert.AreEqual(DataType.Integer, prov.OutsId[1947].QualitySignal.DataType);

            Assert.IsTrue(prov.IsPrepared);
            con.ClearSignals();
            Assert.IsFalse(prov.IsPrepared);
            Assert.AreEqual(0, con.Signals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);
            Assert.AreEqual(0, con.InitialSignals.Count);
            Assert.AreEqual(0, prov.OutsId.Count);
        }
    }
}