﻿using BaseLibrary;
using BaseLibraryTest;
using CommonTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Provider;
using ProvidersLibrary;

namespace ProvidersTest
{
    [TestClass]
    public class KosmSiganlsTest
    {
        private SourceConnect MakeProviders(bool isRetroBase)
        {
            var factory = new ProvidersFactory();
            var con = (SourceConnect)factory.CreateConnect(ProviderType.Source, "SourceCon", "Kosmotronika", new Logger());
            var prov = factory.CreateProvider("KosmotronikaRetroSource", "RetroServerName=RetroServerTest");
            var prov2 = factory.CreateProvider("KosmotronikaArchDbSource", "ArchiveDir=" + TestLib.InfoTaskDevelopDir + @"TestsBig\Kosmotronika\ArchiveKurganOld;Location=0");
            if (isRetroBase) con.JoinProvider(prov, prov2);
            else con.JoinProvider(prov2, prov);
            return con;
        }
        
        [TestMethod]
        public void Setings()
        {
            var con = MakeProviders(true);
            Assert.AreEqual("SourceCon", con.Name);
            Assert.AreEqual("Kosmotronika", con.Complect);
            Assert.AreEqual(ProviderType.Source, con.Type);
            Assert.IsNotNull(con.Logger);
            Assert.AreEqual(con.Context, "Источник: SourceCon");
            Assert.IsNotNull(con.Provider);
            Assert.IsTrue(con.Provider is KosmotronikaRetroSource);
            var prov = (KosmotronikaRetroSource)con.Provider;
            Assert.AreEqual("KosmotronikaRetroSource", prov.Code);
            Assert.AreEqual("Источник: SourceCon, KosmotronikaRetroSource", prov.Context);
            Assert.AreEqual("RetroServerName=RetroServerTest", prov.Inf);
            Assert.AreSame(con, prov.ProviderConnect);
            Assert.IsNotNull(prov.Logger);
            Assert.IsFalse(prov.IsConnected);
            Assert.IsFalse(prov.IsPrepared);

            con = MakeProviders(false);
            Assert.AreEqual("SourceCon", con.Name);
            Assert.AreEqual("Kosmotronika", con.Complect);
            Assert.AreEqual(ProviderType.Source, con.Type);
            Assert.IsNotNull(con.Logger);
            Assert.AreEqual(con.Context, "Источник: SourceCon");
            Assert.IsNotNull(con.Provider);
            Assert.IsTrue(con.Provider is KosmotronikaArchDbSource);
            var prov2 = (KosmotronikaArchDbSource)con.Provider;
            Assert.AreEqual("KosmotronikaArchDbSource", prov2.Code);
            Assert.AreEqual("Источник: SourceCon, KosmotronikaArchDbSource", prov2.Context);
            Assert.AreEqual("ArchiveDir=" + TestLib.InfoTaskDevelopDir + @"TestsBig\Kosmotronika\ArchiveKurganOld;Location=0", prov2.Inf);
            Assert.AreSame(con, prov2.ProviderConnect);
            Assert.IsNotNull(prov2.Logger);
            Assert.IsFalse(prov2.IsConnected);
            Assert.IsFalse(prov2.IsPrepared);

            prov = (KosmotronikaRetroSource)new ProvidersFactory().CreateProvider("KosmotronikaRetroSource", "RetroServerName=RetroServerNo");
            con.JoinProvider(prov);
            Assert.IsFalse(prov.IsConnected);
            Assert.IsFalse(prov.IsPrepared);
            Assert.IsTrue(prov.Connect());
            Assert.IsTrue(prov.IsConnected);
            Assert.IsFalse(prov.IsPrepared);
            prov.Disconnect();
            Assert.IsFalse(prov.IsConnected);
        }

        [TestMethod]
        public void Signals()
        {
            var con = MakeProviders(true);
            var prov = (KosmotronikaRetroSource)con.Provider;
            Assert.IsFalse(prov.IsConnected);
            Assert.IsFalse(prov.IsPrepared);
            Assert.AreEqual(0, con.Signals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);
            Assert.AreEqual(0, con.InitialSignals.Count);
            Assert.AreEqual(0, prov.Analogs.Count);
            Assert.AreEqual(0, prov.Outs.Count);
            Assert.IsNull(prov.OperatorOut);
            con.ClearSignals();
            Assert.AreEqual(0, con.Signals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);
            Assert.AreEqual(0, con.InitialSignals.Count);

            con.AddInitialSignal("01MKA10CE001XQ01.1_Пар", DataType.Real, "SysNum=10827;NumType=1;Appartment=0", "NumOut=1");
            con.AddInitialSignal("01MKA10CE001XQ01.НД", DataType.Integer, "SysNum=10827;NumType=1;Appartment=0", "NumOut=1", "Prop=ND");
            con.AddInitialSignal("01MKA10CE001XQ01.ПОК", DataType.Integer, "SysNum=10827;NumType=1;Appartment=0", "NumOut=1", "Prop=POK");
            Assert.AreEqual(3, con.Signals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);
            Assert.AreEqual(3, con.InitialSignals.Count);
            Assert.IsTrue(con.Signals.ContainsKey("01MKA10CE001XQ01.1_Пар"));
            Assert.IsTrue(con.Signals.ContainsKey("01MKA10CE001XQ01.НД"));
            Assert.IsTrue(con.Signals.ContainsKey("01MKA10CE001XQ01.ПОК"));
            
            con.AddInitialSignal("02MKA10CE001XQ01.1_Пар", DataType.Real, "SysNum=11581;NumType=1;Appartment=0", "NumOut=1");
            Assert.AreEqual(4, con.Signals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);
            Assert.AreEqual(4, con.InitialSignals.Count);
            Assert.IsTrue(con.Signals.ContainsKey("02MKA10CE001XQ01.1_Пар"));

            con.AddInitialSignal("01MKA10CE001XQ01.8_Сост", DataType.Integer, "SysNum=10827;NumType=1;Appartment=0", "NumOut=8");
            con.AddInitialSignal("02MKA10CE001XQ01.8_Сост", DataType.Integer, "SysNum=11581;NumType=1;Appartment=0", "NumOut=8");
            Assert.AreEqual(6, con.Signals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);
            Assert.AreEqual(6, con.InitialSignals.Count);
            Assert.IsTrue(con.Signals.ContainsKey("01MKA10CE001XQ01.8_Сост"));
            Assert.IsTrue(con.Signals.ContainsKey("02MKA10CE001XQ01.8_Сост"));
            
            con.AddInitialSignal("Оператор.NumWS", DataType.Integer, "ObjectType=Operator", "", "Prop=NumWS", false);
            con.AddInitialSignal("Оператор.Mode", DataType.Integer, "ObjectType=Operator", "", "Prop=Mode", false);
            con.AddInitialSignal("Оператор.Code", DataType.Integer, "ObjectType=Operator", "", "Prop=Code", false);
            con.AddInitialSignal("Оператор.Sn", DataType.Integer, "ObjectType=Operator", "", "Prop=Sn", false);
            con.AddInitialSignal("Оператор.NumType", DataType.Integer, "ObjectType=Operator", "", "Prop=NumType", false);
            con.AddInitialSignal("Оператор.Appartment", DataType.Integer, "ObjectType=Operator", "", "Prop=Appartment", false);
            con.AddInitialSignal("Оператор.Params", DataType.String, "ObjectType=Operator", "", "Prop=Params", false);
            con.AddInitialSignal("Оператор.ExtCommand", DataType.String, "ObjectType=Operator", "", "Prop=ExtCommand", false);
            con.AddInitialSignal("Оператор.Point", DataType.String, "ObjectType=Operator", "", "Prop=Point", false);
            Assert.AreEqual(15, con.Signals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);
            Assert.AreEqual(15, con.InitialSignals.Count);
            Assert.IsTrue(con.Signals.ContainsKey("Оператор.NumWS"));
            Assert.IsTrue(con.Signals.ContainsKey("Оператор.Mode"));
            Assert.IsTrue(con.Signals.ContainsKey("Оператор.Code"));
            Assert.IsTrue(con.Signals.ContainsKey("Оператор.Sn"));
            Assert.IsTrue(con.Signals.ContainsKey("Оператор.NumType"));
            Assert.IsTrue(con.Signals.ContainsKey("Оператор.Appartment"));
            Assert.IsTrue(con.Signals.ContainsKey("Оператор.Params"));
            Assert.IsTrue(con.Signals.ContainsKey("Оператор.ExtCommand"));
            Assert.IsTrue(con.Signals.ContainsKey("Оператор.Point"));

            con.AddInitialSignal("01LCW12AA001YB.3_Сост", DataType.Integer, "SysNum=34000;NumType=6;Appartment=0", "NumOut=3");
            con.AddInitialSignal("01LCW12AA001YB.7_Сост", DataType.Integer, "SysNum=34000;NumType=6;Appartment=0", "NumOut=7");
            Assert.AreEqual(17, con.Signals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);
            Assert.AreEqual(17, con.InitialSignals.Count);
            Assert.IsTrue(con.Signals.ContainsKey("01LCW12AA001YB.3_Сост"));
            Assert.IsTrue(con.Signals.ContainsKey("01LCW12AA001YB.7_Сост"));

            Assert.IsFalse(prov.IsPrepared);
            prov.Prepare(false);
            Assert.IsTrue(prov.IsPrepared);
            Assert.AreEqual(2, prov.Analogs.Count);
            Assert.AreEqual(4, prov.Outs.Count);
            Assert.IsNotNull(prov.OperatorOut);

            var outIndex1 = new OutIndex(10827, 1, 0, 1);
            Assert.IsTrue(prov.Analogs.ContainsKey(outIndex1));
            Assert.AreEqual(10827, prov.Analogs[outIndex1].Sn);
            Assert.AreEqual(1, prov.Analogs[outIndex1].NumType);
            Assert.AreEqual(0, prov.Analogs[outIndex1].Appartment);
            Assert.AreEqual(1, prov.Analogs[outIndex1].Out);
            Assert.AreEqual("SysNum=10827;NumType=1;Appartment=0;NumOut=1", prov.Analogs[outIndex1].Context);
            Assert.IsNotNull(prov.Analogs[outIndex1].ValueSignal);
            Assert.AreEqual(DataType.Real, prov.Analogs[outIndex1].ValueSignal.DataType);
            Assert.AreEqual("01MKA10CE001XQ01.1_Пар", prov.Analogs[outIndex1].ValueSignal.Code);
            Assert.AreEqual("SysNum=10827;NumType=1;Appartment=0;NumOut=1", prov.Analogs[outIndex1].ValueSignal.CodeOut);
            Assert.AreEqual("SYSNUM=10827;NUMTYPE=1;APPARTMENT=0;NUMOUT=1;", prov.Analogs[outIndex1].ValueSignal.Inf.ToPropertyString());
            Assert.AreEqual(DataType.Real, prov.Analogs[outIndex1].ValueSignal.DataType);
            Assert.IsNotNull(prov.Analogs[outIndex1].StateSignal);
            Assert.AreEqual(DataType.Integer, prov.Analogs[outIndex1].StateSignal.DataType);
            Assert.AreEqual("01MKA10CE001XQ01.НД", prov.Analogs[outIndex1].StateSignal.Code);
            Assert.AreEqual("SysNum=10827;NumType=1;Appartment=0;NumOut=1", prov.Analogs[outIndex1].StateSignal.CodeOut);
            Assert.AreEqual("SYSNUM=10827;NUMTYPE=1;APPARTMENT=0;NUMOUT=1;PROP=ND;", prov.Analogs[outIndex1].StateSignal.Inf.ToPropertyString());
            Assert.AreEqual(DataType.Integer, prov.Analogs[outIndex1].StateSignal.DataType);
            Assert.IsNotNull(prov.Analogs[outIndex1].PokSignal);
            Assert.AreEqual(DataType.Integer, prov.Analogs[outIndex1].PokSignal.DataType);

            var outIndex2 = new OutIndex(11581, 1, 0, 1);
            Assert.IsTrue(prov.Analogs.ContainsKey(outIndex2));
            Assert.AreEqual(11581, prov.Analogs[outIndex2].Sn);
            Assert.AreEqual(1, prov.Analogs[outIndex2].NumType);
            Assert.AreEqual(0, prov.Analogs[outIndex2].Appartment);
            Assert.AreEqual(1, prov.Analogs[outIndex2].Out);
            Assert.AreEqual("SysNum=11581;NumType=1;Appartment=0;NumOut=1", prov.Analogs[outIndex2].Context);
            Assert.IsNotNull(prov.Analogs[outIndex2].ValueSignal);
            Assert.AreEqual(DataType.Real, prov.Analogs[outIndex2].ValueSignal.DataType);
            Assert.IsNull(prov.Analogs[outIndex2].StateSignal);
            Assert.IsNull(prov.Analogs[outIndex2].PokSignal);

            outIndex1 = new OutIndex(10827, 1, 0, 8);
            outIndex2 = new OutIndex(11581, 1, 0, 8);
            Assert.IsTrue(prov.Outs.ContainsKey(outIndex1));
            Assert.AreEqual(10827, prov.Outs[outIndex1].Sn);
            Assert.AreEqual(1, prov.Outs[outIndex1].NumType);
            Assert.AreEqual(0, prov.Outs[outIndex1].Appartment);
            Assert.AreEqual(8, prov.Outs[outIndex1].Out);
            Assert.AreEqual("SysNum=10827;NumType=1;Appartment=0;NumOut=8", prov.Outs[outIndex1].Context);
            Assert.IsTrue(prov.Outs.ContainsKey(outIndex2));
            Assert.AreEqual(11581, prov.Outs[outIndex2].Sn);
            Assert.AreEqual(1, prov.Outs[outIndex2].NumType);
            Assert.AreEqual(0, prov.Outs[outIndex2].Appartment);
            Assert.AreEqual(8, prov.Outs[outIndex2].Out);
            Assert.AreEqual("SysNum=11581;NumType=1;Appartment=0;NumOut=8", prov.Outs[outIndex2].Context);
            Assert.IsNotNull(prov.Outs[outIndex1].ValueSignal);
            Assert.AreEqual(DataType.Integer, prov.Outs[outIndex1].ValueSignal.DataType);
            Assert.IsNotNull(prov.Outs[outIndex2].ValueSignal);
            Assert.AreEqual(DataType.Integer, prov.Outs[outIndex2].ValueSignal.DataType);
            Assert.IsNull(prov.Outs[outIndex1].StateSignal);
            Assert.IsNull(prov.Outs[outIndex1].PokSignal);
            Assert.IsNull(prov.Outs[outIndex2].StateSignal);
            Assert.IsNull(prov.Outs[outIndex2].PokSignal);

            outIndex1 = new OutIndex(34000, 6, 0, 3);
            outIndex2 = new OutIndex(34000, 6, 0, 7);
            Assert.IsTrue(prov.Outs.ContainsKey(outIndex1));
            Assert.AreEqual(34000, prov.Outs[outIndex1].Sn);
            Assert.AreEqual(6, prov.Outs[outIndex1].NumType);
            Assert.AreEqual(0, prov.Outs[outIndex1].Appartment);
            Assert.AreEqual(3, prov.Outs[outIndex1].Out);
            Assert.AreEqual("SysNum=34000;NumType=6;Appartment=0;NumOut=3", prov.Outs[outIndex1].Context);
            Assert.IsTrue(prov.Outs.ContainsKey(outIndex2));
            Assert.AreEqual(34000, prov.Outs[outIndex2].Sn);
            Assert.AreEqual(6, prov.Outs[outIndex2].NumType);
            Assert.AreEqual(0, prov.Outs[outIndex2].Appartment);
            Assert.AreEqual(7, prov.Outs[outIndex2].Out);
            Assert.AreEqual("SysNum=34000;NumType=6;Appartment=0;NumOut=7", prov.Outs[outIndex2].Context);
            Assert.IsNotNull(prov.Outs[outIndex1].ValueSignal);
            Assert.AreEqual(DataType.Integer, prov.Outs[outIndex1].ValueSignal.DataType);
            Assert.IsNotNull(prov.Outs[outIndex2].ValueSignal);
            Assert.AreEqual(DataType.Integer, prov.Outs[outIndex2].ValueSignal.DataType);
            Assert.IsNull(prov.Outs[outIndex1].StateSignal);
            Assert.IsNull(prov.Outs[outIndex1].PokSignal);
            Assert.IsNull(prov.Outs[outIndex2].StateSignal);
            Assert.IsNull(prov.Outs[outIndex2].PokSignal);

            Assert.IsNotNull(prov.OperatorOut.NumWsSignal);
            Assert.IsNotNull(prov.OperatorOut.ModeSignal);
            Assert.IsNotNull(prov.OperatorOut.CodeSignal);
            Assert.IsNotNull(prov.OperatorOut.SnSignal);
            Assert.IsNotNull(prov.OperatorOut.NumTypeSignal);
            Assert.IsNotNull(prov.OperatorOut.AppartmentSignal);
            Assert.IsNotNull(prov.OperatorOut.ParamsSignal);
            Assert.IsNotNull(prov.OperatorOut.ExtCommandSignal);
            Assert.IsNotNull(prov.OperatorOut.PointSignal);
            Assert.AreEqual("ObjectType=Operator", prov.OperatorOut.Context);

            con.ClearSignals();
            Assert.IsFalse(prov.IsPrepared);
            Assert.AreEqual(0, con.Signals.Count);
            Assert.AreEqual(0, con.CalcSignals.Count);
            Assert.AreEqual(0, con.InitialSignals.Count);
            Assert.AreEqual(0, prov.Outs.Count);
            Assert.AreEqual(0, prov.Analogs.Count);
            Assert.IsNull(prov.OperatorOut);
        }
    }
}