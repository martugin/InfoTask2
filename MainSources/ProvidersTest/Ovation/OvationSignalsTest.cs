﻿using BaseLibrary;
using CommonTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Provider;
using ProvidersLibrary;

namespace ProvidersTest
{
    [TestClass]
    public class OvationSignalsTest
    {
        private SourceConnect MakeProviders()
        {
            var factory = new ProvidersFactory();
            var con = (SourceConnect)factory.CreateConnect(ProviderType.Source, "SourceCon", "Ovation", new Logger());
            var prov = factory.CreateProvider("OvationSource", "DataSource=DROP200");
            con.JoinProvider(prov);
            return con;
        }

        [TestMethod]
        public void Siganls()
        {
            var con = MakeProviders();
            var prov = (OvationSource)con.Source;
            Assert.AreEqual("SourceCon", con.Name);
            Assert.AreEqual("Ovation", con.Complect);
            Assert.AreEqual(ProviderType.Source, con.Type);
            Assert.IsNotNull(con.Logger);
            Assert.AreEqual(con.Context, "Источник: SourceCon");
            Assert.IsNotNull(con.Provider);
            Assert.IsTrue(con.Provider is OvationSource);
            Assert.AreEqual("OvationSource", prov.Code);
            Assert.AreEqual("Источник: SourceCon, OvationSource", prov.Context);
            Assert.AreEqual("DataSource=DROP200", prov.Inf);
            Assert.AreSame(con, prov.ProviderConnect);
            Assert.IsNotNull(prov.Logger);
            Assert.IsFalse(prov.IsConnected);
            Assert.IsFalse(prov.IsPrepared);

            Assert.AreEqual(0, con.Signals.Count);
            con.ClearSignals();
            Assert.AreEqual(0, con.Signals.Count);
            con.AddInitialSignal("11ASV00CT001.Пар", DataType.Real, "Id=45259");
            con.AddInitialSignal("11ASV00CT001.Stat", DataType.Integer, "Id=45259", "", "Prop=STAT");
            con.AddInitialSignal("11ASV00CT002.Пар", DataType.Real, "Id=45260");
            Assert.AreEqual(3, con.Signals.Count);
            con.AddInitialSignal("11BAT14CP051XG01.Пар", DataType.Boolean, "Id=46958");
            con.AddInitialSignal("11BAT14CP051XG01.Stat", DataType.Integer, "Id=46958", "", "Prop=STAT");
            Assert.AreEqual(5, con.Signals.Count);
            con.AddInitialSignal("11HHG50AA001-SOST1.Пар", DataType.Integer, "Id=50679");
            con.AddInitialSignal("11HHG50AA001-SOST2.Пар", DataType.Integer, "Id=50680");
            Assert.AreEqual(7, con.Signals.Count);
            Assert.AreEqual(7, con.InitialSignals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);
            Assert.IsTrue(con.Signals.ContainsKey("11ASV00CT001.Пар"));
            Assert.AreEqual(DataType.Real, con.Signals["11ASV00CT001.Пар"].DataType);
            Assert.AreEqual("ID=45259;", con.Signals["11ASV00CT001.Пар"].Inf.ToPropertyString());
            Assert.IsTrue(con.Signals.ContainsKey("11ASV00CT001.Stat"));
            Assert.AreEqual(DataType.Integer, con.Signals["11ASV00CT001.Stat"].DataType);
            Assert.AreEqual("ID=45259;PROP=STAT;", con.Signals["11ASV00CT001.Stat"].Inf.ToPropertyString());
            Assert.IsTrue(con.Signals.ContainsKey("11ASV00CT002.Пар"));
            Assert.AreEqual(DataType.Real, con.Signals["11ASV00CT002.Пар"].DataType);
            Assert.AreEqual("ID=45260;", con.Signals["11ASV00CT002.Пар"].Inf.ToPropertyString());
            Assert.IsTrue(con.Signals.ContainsKey("11BAT14CP051XG01.Пар"));
            Assert.AreEqual(DataType.Boolean, con.Signals["11BAT14CP051XG01.Пар"].DataType);
            Assert.AreEqual("ID=46958;", con.Signals["11BAT14CP051XG01.Пар"].Inf.ToPropertyString());
            Assert.IsTrue(con.Signals.ContainsKey("11BAT14CP051XG01.Stat"));
            Assert.AreEqual(DataType.Integer, con.Signals["11BAT14CP051XG01.Stat"].DataType);
            Assert.AreEqual("ID=46958;PROP=STAT;", con.Signals["11BAT14CP051XG01.Stat"].Inf.ToPropertyString());
            Assert.IsTrue(con.Signals.ContainsKey("11HHG50AA001-SOST1.Пар"));
            Assert.AreEqual(DataType.Integer, con.Signals["11HHG50AA001-SOST1.Пар"].DataType);
            Assert.IsTrue(con.Signals.ContainsKey("11HHG50AA001-SOST2.Пар"));
            Assert.AreEqual(DataType.Integer, con.Signals["11HHG50AA001-SOST2.Пар"].DataType);

            Assert.IsFalse(prov.IsPrepared);
            prov.Prepare(false);
            Assert.IsTrue(prov.IsPrepared);
            Assert.AreEqual(5, prov.OutsId.Count);
            Assert.IsTrue(prov.OutsId.ContainsKey(45259));
            Assert.AreEqual(45259, prov.OutsId[45259].Id);
            Assert.IsNotNull(prov.OutsId[45259].ValueSignal);
            Assert.AreEqual(DataType.Real, prov.OutsId[45259].ValueSignal.DataType);
            Assert.IsNotNull(prov.OutsId[45259].StateSignal);
            Assert.AreEqual(DataType.Integer, prov.OutsId[45259].StateSignal.DataType);

            Assert.IsTrue(prov.OutsId.ContainsKey(45260));
            Assert.AreEqual(45260, prov.OutsId[45260].Id);
            Assert.IsNotNull(prov.OutsId[45260].ValueSignal);
            Assert.AreEqual(DataType.Real, prov.OutsId[45260].ValueSignal.DataType);
            Assert.IsNull(prov.OutsId[45260].StateSignal);

            Assert.IsTrue(prov.OutsId.ContainsKey(46958));
            Assert.AreEqual(46958, prov.OutsId[46958].Id);
            Assert.IsNotNull(prov.OutsId[46958].ValueSignal);
            Assert.AreEqual(DataType.Boolean, prov.OutsId[46958].ValueSignal.DataType);
            Assert.IsNotNull(prov.OutsId[46958].StateSignal);
            Assert.AreEqual(DataType.Integer, prov.OutsId[46958].StateSignal.DataType);

            Assert.IsTrue(prov.OutsId.ContainsKey(50679));
            Assert.AreEqual(50679, prov.OutsId[50679].Id);
            Assert.IsNotNull(prov.OutsId[50679].ValueSignal);
            Assert.AreEqual(DataType.Integer, prov.OutsId[50679].ValueSignal.DataType);
            Assert.IsNull(prov.OutsId[50679].StateSignal);

            Assert.IsTrue(prov.OutsId.ContainsKey(50680));
            Assert.AreEqual(50680, prov.OutsId[50680].Id);
            Assert.IsNotNull(prov.OutsId[50680].ValueSignal);
            Assert.AreEqual(DataType.Integer, prov.OutsId[50680].ValueSignal.DataType);
            Assert.IsNull(prov.OutsId[50680].StateSignal);

            con.AddInitialSignal("AlarmMessages.MsgFlags", DataType.Integer, "ObjectType=Alarm", "", "Prop=MSG_FLAGS", false);
            con.AddInitialSignal("AlarmMessages.MsgType", DataType.Integer, "ObjectType=Alarm", "", "Prop=MSG_TYPE", false);
            con.AddInitialSignal("AlarmMessages.SubType", DataType.Integer, "ObjectType=Alarm", "", "Prop=SUB_TYPE", false);
            con.AddInitialSignal("AlarmMessages.System", DataType.Integer, "ObjectType=Alarm", "", "Prop=SYSTEM", false);
            con.AddInitialSignal("AlarmMessages.Node", DataType.String, "ObjectType=Alarm", "", "Prop=NODE", false);
            con.AddInitialSignal("AlarmMessages.AlmName", DataType.String, "ObjectType=Alarm", "", "Prop=ALM_NAME", false);
            con.AddInitialSignal("AlarmMessages.PrimText", DataType.String, "ObjectType=Alarm", "", "Prop=PRIM_TEXT", false);
            con.AddInitialSignal("AlarmMessages.SuppText", DataType.String, "ObjectType=Alarm", "", "Prop=SUPP_TEXT", false);
            con.AddInitialSignal("AlarmMessages.Info1", DataType.String, "ObjectType=Alarm", "", "Prop=INFO1", false);
            con.AddInitialSignal("AlarmMessages.Info2", DataType.String, "ObjectType=Alarm", "", "Prop=INFO2", false);

            con.AddInitialSignal("SoeMessages.MsgFlags", DataType.Integer, "ObjectType=Soe", "", "Prop=MSG_FLAGS", false);
            con.AddInitialSignal("SoeMessages.MsgType", DataType.Integer, "ObjectType=Soe", "", "Prop=MSG_TYPE", false);
            con.AddInitialSignal("SoeMessages.SubType", DataType.Integer, "ObjectType=Soe", "", "Prop=SUB_TYPE", false);
            con.AddInitialSignal("SoeMessages.System", DataType.Integer, "ObjectType=Soe", "", "Prop=SYSTEM", false);
            con.AddInitialSignal("SoeMessages.Node", DataType.String, "ObjectType=Soe", "", "Prop=NODE", false);
            con.AddInitialSignal("SoeMessages.AlmName", DataType.String, "ObjectType=Soe", "", "Prop=ALM_NAME", false);
            con.AddInitialSignal("SoeMessages.PrimText", DataType.String, "ObjectType=Soe", "", "Prop=PRIM_TEXT", false);
            con.AddInitialSignal("SoeMessages.SuppText", DataType.String, "ObjectType=Soe", "", "Prop=SUPP_TEXT", false);
            con.AddInitialSignal("SoeMessages.Info1", DataType.String, "ObjectType=Soe", "", "Prop=INFO1", false);
            con.AddInitialSignal("SoeMessages.Info2", DataType.String, "ObjectType=Soe", "", "Prop=INFO2", false);

            con.AddInitialSignal("TextMessages.MsgFlags", DataType.Integer, "ObjectType=Text", "", "Prop=MSG_FLAGS", false);
            con.AddInitialSignal("TextMessages.MsgType", DataType.Integer, "ObjectType=Text", "", "Prop=MSG_TYPE", false);
            con.AddInitialSignal("TextMessages.SubType", DataType.Integer, "ObjectType=Text", "", "Prop=SUB_TYPE", false);
            con.AddInitialSignal("TextMessages.System", DataType.Integer, "ObjectType=Text", "", "Prop=SYSTEM", false);
            con.AddInitialSignal("TextMessages.Node", DataType.String, "ObjectType=Text", "", "Prop=NODE", false);
            con.AddInitialSignal("TextMessages.AlmName", DataType.String, "ObjectType=Text", "", "Prop=ALM_NAME", false);
            con.AddInitialSignal("TextMessages.PrimText", DataType.String, "ObjectType=Text", "", "Prop=PRIM_TEXT", false);
            con.AddInitialSignal("TextMessages.SuppText", DataType.String, "ObjectType=Text", "", "Prop=SUPP_TEXT", false);
            con.AddInitialSignal("TextMessages.Info1", DataType.String, "ObjectType=Text", "", "Prop=INFO1", false);
            con.AddInitialSignal("TextMessages.Info2", DataType.String, "ObjectType=Text", "", "Prop=INFO2", false);

            Assert.AreEqual(37, con.Signals.Count);
            Assert.AreEqual(37, con.InitialSignals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);
            Assert.IsTrue(con.Signals.ContainsKey("AlarmMessages.MsgFlags"));
            Assert.IsTrue(con.Signals.ContainsKey("SoeMessages.PrimText"));
            Assert.IsTrue(con.Signals.ContainsKey("TextMessages.Info2"));
            Assert.IsTrue(con.InitialSignals.ContainsKey("AlarmMessages.MsgFlags"));
            Assert.IsTrue(con.InitialSignals.ContainsKey("SoeMessages.PrimText"));
            Assert.IsTrue(con.InitialSignals.ContainsKey("TextMessages.Info2"));

            Assert.IsFalse(prov.IsPrepared);
            prov.Prepare(false);
            Assert.IsTrue(prov.IsPrepared);
            Assert.AreEqual(5, prov.OutsId.Count);
            Assert.IsNotNull(prov.AlarmOut);
            Assert.IsNotNull(prov.SoeOut);
            Assert.IsNotNull(prov.TextOut);

            Assert.IsNotNull(prov.AlarmOut.MsgFlagsSignal);
            Assert.AreEqual(DataType.Integer, prov.AlarmOut.MsgFlagsSignal.DataType);
            Assert.AreEqual("OBJECTTYPE=Alarm;PROP=MSG_FLAGS;", prov.AlarmOut.MsgFlagsSignal.Inf.ToPropertyString());
            Assert.IsNotNull(prov.AlarmOut.MsgTypeSignal);
            Assert.AreEqual(DataType.Integer, prov.AlarmOut.MsgTypeSignal.DataType);
            Assert.IsNotNull(prov.AlarmOut.SubTypeSignal);
            Assert.AreEqual(DataType.Integer, prov.AlarmOut.SubTypeSignal.DataType);
            Assert.IsNotNull(prov.AlarmOut.SystemSignal);
            Assert.AreEqual(DataType.Integer, prov.AlarmOut.SystemSignal.DataType);
            Assert.IsNotNull(prov.AlarmOut.AlmNameSignal);
            Assert.AreEqual(DataType.String, prov.AlarmOut.AlmNameSignal.DataType);
            Assert.IsNotNull(prov.AlarmOut.Info1Signal);
            Assert.AreEqual(DataType.String, prov.AlarmOut.Info1Signal.DataType);
            Assert.IsNotNull(prov.AlarmOut.Info2Signal);
            Assert.AreEqual(DataType.String, prov.AlarmOut.Info2Signal.DataType);
            Assert.IsNotNull(prov.AlarmOut.NodeSignal);
            Assert.AreEqual(DataType.String, prov.AlarmOut.NodeSignal.DataType);
            Assert.IsNotNull(prov.AlarmOut.PrimTextSignal);
            Assert.AreEqual(DataType.String, prov.AlarmOut.PrimTextSignal.DataType);
            Assert.IsNotNull(prov.AlarmOut.SuppTextSignal);
            Assert.AreEqual(DataType.String, prov.AlarmOut.SuppTextSignal.DataType);

            Assert.IsNotNull(prov.AlarmOut.MsgFlagsSignal);
            Assert.AreEqual(DataType.Integer, prov.SoeOut.MsgFlagsSignal.DataType);
            Assert.AreEqual("OBJECTTYPE=Soe;PROP=MSG_FLAGS;", prov.SoeOut.MsgFlagsSignal.Inf.ToPropertyString());
            Assert.IsNotNull(prov.SoeOut.MsgTypeSignal);
            Assert.AreEqual(DataType.Integer, prov.SoeOut.MsgTypeSignal.DataType);
            Assert.IsNotNull(prov.SoeOut.AlmNameSignal);
            Assert.AreEqual(DataType.String, prov.SoeOut.AlmNameSignal.DataType);

            Assert.IsNotNull(prov.AlarmOut.MsgFlagsSignal);
            Assert.AreEqual(DataType.Integer, prov.TextOut.MsgFlagsSignal.DataType);
            Assert.AreEqual("OBJECTTYPE=Text;PROP=MSG_FLAGS;", prov.TextOut.MsgFlagsSignal.Inf.ToPropertyString());
            Assert.IsNotNull(prov.TextOut.MsgTypeSignal);
            Assert.AreEqual(DataType.Integer, prov.TextOut.MsgTypeSignal.DataType);
            Assert.IsNotNull(prov.TextOut.AlmNameSignal);
            Assert.AreEqual(DataType.String, prov.TextOut.AlmNameSignal.DataType);

            con.ClearSignals();
            Assert.IsFalse(prov.IsPrepared);
            Assert.AreEqual(0, con.Signals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);
            Assert.AreEqual(0, con.InitialSignals.Count);
            Assert.AreEqual(0, prov.OutsId.Count);
            Assert.IsNull(prov.AlarmOut);
            Assert.IsNull(prov.SoeOut);
            Assert.IsNull(prov.TextOut);

            prov = (OvationSource)new ProvidersFactory().CreateProvider("OvationSource", "DataSource=DropNo");
            con.JoinProvider(prov);
            Assert.IsFalse(prov.IsConnected);
            Assert.IsFalse(prov.IsPrepared);
            Assert.IsFalse(prov.Connect());
            Assert.IsFalse(prov.IsConnected);
            Assert.IsFalse(prov.IsPrepared);
            prov.Disconnect();
            Assert.IsFalse(prov.IsConnected);
        }
    }
}