using BaseLibrary;
using BaseLibraryTest;
using CommonTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ovation;
using ProvidersLibrary;

namespace ProvidersTest
{
    [TestClass]
    public class OvationSignalsTest
    {
        private SourceConnect MakeProviders()
        {
            var factory = new ProvidersFactory();
            var logger = new Logger(new TestHistory(), new AppIndicator());
            var con = (SourceConnect)factory.CreateConnect(logger, ProviderType.Source, "SourceCon", "Ovation");
            var prov = factory.CreateProvider(logger, "OvationSource", "DataSource=DROP200");
            con.JoinProvider(prov);
            return con;
        }

        [TestMethod]
        public void Siganls()
        {
            var con = MakeProviders();
            var prov = (OvationSource)con.Source;
            Assert.AreEqual("SourceCon", con.Code);
            Assert.AreEqual("Ovation", con.Complect);
            Assert.AreEqual(ProviderType.Source, con.Type);
            Assert.IsNotNull(con.Logger);
            Assert.AreEqual("SourceCon", con.Context);
            Assert.IsNotNull(con.Provider);
            Assert.IsTrue(con.Provider is OvationSource);
            Assert.AreEqual("OvationSource", prov.Code);
            Assert.AreEqual("SourceCon", prov.Context);
            Assert.AreEqual("DataSource=DROP200", prov.Inf);
            Assert.AreSame(con, prov.ProviderConnect);
            Assert.IsNotNull(prov.Logger);
            Assert.IsFalse(prov.IsConnected);
            Assert.IsFalse(prov.IsPrepared);

            Assert.AreEqual(0, con.ReadingSignals.Count);
            con.ClearSignals();
            Assert.AreEqual(0, con.ReadingSignals.Count);
            con.AddSignal("11ASV00CT001.Пар", DataType.Real, SignalType.Uniform,  "Id=45259");
            con.AddSignal("11ASV00CT001.Stat", DataType.Integer, SignalType.Uniform, "Id=45259", "", "Prop=STAT");
            con.AddSignal("11ASV00CT002.Пар", DataType.Real, SignalType.Uniform, "Id=45260");
            Assert.AreEqual(3, con.ReadingSignals.Count);
            con.AddSignal("11BAT14CP051XG01.Пар", DataType.Boolean, SignalType.Uniform, "Id=46958");
            con.AddSignal("11BAT14CP051XG01.Stat", DataType.Integer, SignalType.Uniform, "Id=46958", "", "Prop=STAT");
            Assert.AreEqual(5, con.ReadingSignals.Count);
            con.AddSignal("11HHG50AA001-SOST1.Пар", DataType.Integer, SignalType.Uniform, "Id=50679");
            con.AddSignal("11HHG50AA001-SOST2.Пар", DataType.Integer, SignalType.Uniform, "Id=50680");
            Assert.AreEqual(7, con.ReadingSignals.Count);
            Assert.AreEqual(7, con.InitialSignals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);
            Assert.IsTrue(con.ReadingSignals.ContainsKey("11ASV00CT001.Пар"));
            Assert.AreEqual(DataType.Real, con.ReadingSignals["11ASV00CT001.Пар"].DataType);
            Assert.IsTrue(con.ReadingSignals.ContainsKey("11ASV00CT001.Stat"));
            Assert.AreEqual(DataType.Integer, con.ReadingSignals["11ASV00CT001.Stat"].DataType);
            Assert.IsTrue(con.ReadingSignals.ContainsKey("11ASV00CT002.Пар"));
            Assert.AreEqual(DataType.Real, con.ReadingSignals["11ASV00CT002.Пар"].DataType);
            Assert.IsTrue(con.ReadingSignals.ContainsKey("11BAT14CP051XG01.Пар"));
            Assert.AreEqual(DataType.Boolean, con.ReadingSignals["11BAT14CP051XG01.Пар"].DataType);
            Assert.IsTrue(con.ReadingSignals.ContainsKey("11BAT14CP051XG01.Stat"));
            Assert.AreEqual(DataType.Integer, con.ReadingSignals["11BAT14CP051XG01.Stat"].DataType);
            Assert.IsTrue(con.ReadingSignals.ContainsKey("11HHG50AA001-SOST1.Пар"));
            Assert.AreEqual(DataType.Integer, con.ReadingSignals["11HHG50AA001-SOST1.Пар"].DataType);
            Assert.IsTrue(con.ReadingSignals.ContainsKey("11HHG50AA001-SOST2.Пар"));
            Assert.AreEqual(DataType.Integer, con.ReadingSignals["11HHG50AA001-SOST2.Пар"].DataType);

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

            con.AddSignal("AlarmMessages.MsgFlags", DataType.Integer, SignalType.List, "ObjectType=Alarm", "", "Prop=MSG_FLAGS");
            con.AddSignal("AlarmMessages.MsgType", DataType.Integer, SignalType.List, "ObjectType=Alarm", "", "Prop=MSG_TYPE");
            con.AddSignal("AlarmMessages.SubType", DataType.Integer, SignalType.List, "ObjectType=Alarm", "", "Prop=SUB_TYPE");
            con.AddSignal("AlarmMessages.System", DataType.Integer, SignalType.List, "ObjectType=Alarm", "", "Prop=SYSTEM");
            con.AddSignal("AlarmMessages.Node", DataType.String, SignalType.List, "ObjectType=Alarm", "", "Prop=NODE");
            con.AddSignal("AlarmMessages.AlmName", DataType.String, SignalType.List, "ObjectType=Alarm", "", "Prop=ALM_NAME");
            con.AddSignal("AlarmMessages.PrimText", DataType.String, SignalType.List, "ObjectType=Alarm", "", "Prop=PRIM_TEXT");
            con.AddSignal("AlarmMessages.SuppText", DataType.String, SignalType.List, "ObjectType=Alarm", "", "Prop=SUPP_TEXT");
            con.AddSignal("AlarmMessages.Info1", DataType.String, SignalType.List, "ObjectType=Alarm", "", "Prop=INFO1");
            con.AddSignal("AlarmMessages.Info2", DataType.String, SignalType.List, "ObjectType=Alarm", "", "Prop=INFO2");

            con.AddSignal("SoeMessages.MsgFlags", DataType.Integer, SignalType.List, "ObjectType=Soe", "", "Prop=MSG_FLAGS");
            con.AddSignal("SoeMessages.MsgType", DataType.Integer, SignalType.List, "ObjectType=Soe", "", "Prop=MSG_TYPE");
            con.AddSignal("SoeMessages.SubType", DataType.Integer, SignalType.List, "ObjectType=Soe", "", "Prop=SUB_TYPE");
            con.AddSignal("SoeMessages.System", DataType.Integer, SignalType.List, "ObjectType=Soe", "", "Prop=SYSTEM");
            con.AddSignal("SoeMessages.Node", DataType.String, SignalType.List, "ObjectType=Soe", "", "Prop=NODE");
            con.AddSignal("SoeMessages.AlmName", DataType.String, SignalType.List, "ObjectType=Soe", "", "Prop=ALM_NAME");
            con.AddSignal("SoeMessages.PrimText", DataType.String, SignalType.List, "ObjectType=Soe", "", "Prop=PRIM_TEXT");
            con.AddSignal("SoeMessages.SuppText", DataType.String, SignalType.List, "ObjectType=Soe", "", "Prop=SUPP_TEXT");
            con.AddSignal("SoeMessages.Info1", DataType.String, SignalType.List, "ObjectType=Soe", "", "Prop=INFO1");
            con.AddSignal("SoeMessages.Info2", DataType.String, SignalType.List, "ObjectType=Soe", "", "Prop=INFO2");

            con.AddSignal("TextMessages.MsgFlags", DataType.Integer, SignalType.List, "ObjectType=Text", "", "Prop=MSG_FLAGS");
            con.AddSignal("TextMessages.MsgType", DataType.Integer, SignalType.List, "ObjectType=Text", "", "Prop=MSG_TYPE");
            con.AddSignal("TextMessages.SubType", DataType.Integer, SignalType.List, "ObjectType=Text", "", "Prop=SUB_TYPE");
            con.AddSignal("TextMessages.System", DataType.Integer, SignalType.List, "ObjectType=Text", "", "Prop=SYSTEM");
            con.AddSignal("TextMessages.Node", DataType.String, SignalType.List, "ObjectType=Text", "", "Prop=NODE");
            con.AddSignal("TextMessages.AlmName", DataType.String, SignalType.List, "ObjectType=Text", "", "Prop=ALM_NAME");
            con.AddSignal("TextMessages.PrimText", DataType.String, SignalType.List, "ObjectType=Text", "", "Prop=PRIM_TEXT");
            con.AddSignal("TextMessages.SuppText", DataType.String, SignalType.List, "ObjectType=Text", "", "Prop=SUPP_TEXT");
            con.AddSignal("TextMessages.Info1", DataType.String, SignalType.List, "ObjectType=Text", "", "Prop=INFO1");
            con.AddSignal("TextMessages.Info2", DataType.String, SignalType.List, "ObjectType=Text", "", "Prop=INFO2");

            Assert.AreEqual(37, con.ReadingSignals.Count);
            Assert.AreEqual(37, con.InitialSignals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);
            Assert.IsTrue(con.ReadingSignals.ContainsKey("AlarmMessages.MsgFlags"));
            Assert.IsTrue(con.ReadingSignals.ContainsKey("SoeMessages.PrimText"));
            Assert.IsTrue(con.ReadingSignals.ContainsKey("TextMessages.Info2"));
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
            Assert.AreEqual(0, con.ReadingSignals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);
            Assert.AreEqual(0, con.InitialSignals.Count);
            Assert.AreEqual(0, prov.OutsId.Count);
            Assert.IsNull(prov.AlarmOut);
            Assert.IsNull(prov.SoeOut);
            Assert.IsNull(prov.TextOut);

            prov = (OvationSource)new ProvidersFactory().CreateProvider(new Logger(), "OvationSource", "DataSource=DropNo");
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