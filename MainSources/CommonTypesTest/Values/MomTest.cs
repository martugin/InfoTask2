﻿using System;
using CommonTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Different = BaseLibrary.Different;

namespace CommonTypesTest
{
    [TestClass]
    public class MomTest
    {
        private static MomErrPool MakeErrPool()
        {
            var ef = new MomErrFactory("ErrSource", MomErrType.Source);
            ef.AddGoodDescr(0);
            ef.AddDescr(1, "Warning");
            ef.AddDescr(2, "Error");
            return new MomErrPool(ef);
        }

        [TestMethod]
        public void MomBool()
        {
            var c = new ContextTest("Context");
            var pool = MakeErrPool();
            var err = pool.MakeError(2, c);

            var d = new DateTime(2016, 06, 16, 11, 23, 00);
            var m = MFactory.NewMom(d, true);
            Assert.AreEqual(d, m.Time);
            Assert.IsNull(m.Error);
            Assert.IsNull(m.TotalError);
            Assert.AreEqual(true, m.Boolean);
            Assert.AreEqual(1, m.Integer);
            Assert.AreEqual(1.0, m.Real);
            Assert.AreEqual("1", m.String);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.Boolean, m.DataType);
            var v = m.Value;
            Assert.AreEqual(DataType.Boolean, v.DataType);
            var cv = m.CalcValue;
            Assert.AreEqual(DataType.Boolean, cv.DataType);
            Assert.IsNull(cv.TotalError);
            Assert.IsNotNull(m.LastMom);
            Assert.AreEqual(true, m.LastMom.Boolean);
            Assert.AreEqual(d, m.TimeI(0));
            Assert.IsNull(m.ErrorI(0));
            Assert.AreEqual(true, m.BooleanI(0));
            Assert.AreEqual(1, m.IntegerI(0));
            Assert.AreEqual(1.0, m.RealI(0));
            Assert.AreEqual("1", m.StringI(0));

            var mn = m.ToMean();
            Assert.AreEqual(Different.MinDate, mn.Time);
            Assert.IsNull(mn.Error);
            Assert.IsNull(mn.TotalError);
            Assert.AreEqual(true, mn.Boolean);
            Assert.AreEqual(1, mn.Integer);
            Assert.AreEqual(1.0, mn.Real);
            Assert.AreEqual("1", mn.String);
            Assert.AreEqual(1, mn.Count);
            Assert.AreEqual(DataType.Boolean, mn.DataType);
            v = mn.Value;
            Assert.AreEqual(DataType.Boolean, v.DataType);
            cv = mn.CalcValue;
            Assert.AreEqual(DataType.Boolean, cv.DataType);
            Assert.IsNull(cv.TotalError);
            Assert.IsNotNull(mn.LastMom);
            Assert.AreEqual(true, mn.LastMom.Boolean);
            Assert.AreEqual(Different.MinDate, mn.TimeI(0));
            Assert.IsNull(mn.ErrorI(0));
            Assert.AreEqual(true, mn.BooleanI(0));
            Assert.AreEqual(1, mn.IntegerI(0));
            Assert.AreEqual(1.0, mn.RealI(0));
            Assert.AreEqual("1", mn.StringI(0));

            var d1 = new DateTime(2016, 06, 16, 11, 26, 00);
            var m1 = m.ToMom(d1, err);
            Assert.AreEqual(d1, m1.Time);
            Assert.IsNotNull(m1.Error);
            Assert.IsNotNull(m1.TotalError);
            Assert.AreEqual(2, m1.Error.Number);
            Assert.AreEqual("Error", m1.Error.Text);
            Assert.AreEqual(MomErrType.Source, m1.Error.ErrType);
            Assert.AreEqual(true, m1.Boolean);
            Assert.AreEqual(1, m1.Integer);
            Assert.AreEqual(1.0, m1.Real);
            Assert.AreEqual("1", m1.String);
            Assert.AreEqual(1, m1.Count);
            Assert.AreEqual(DataType.Boolean, m1.DataType);
            Assert.IsTrue(m1.ValueEquals(m));
            Assert.IsFalse(m1.ValueAndErrorEquals(m));
            Assert.IsTrue(m.ValueEquals(m1));
            Assert.IsFalse(m.ValueAndErrorEquals(m1));

            m = MFactory.NewMom(d1, true, err);
            Assert.IsNotNull(m.Error);
            Assert.IsNotNull(m.TotalError);
            Assert.AreEqual(2, m.Error.Number);
            Assert.AreEqual("Error", m.Error.Text);
            Assert.AreEqual(MomErrType.Source, m.Error.ErrType);
            Assert.AreEqual(2, m.TotalError.Number);
            Assert.AreEqual("Error", m.TotalError.Text);
            Assert.AreEqual(MomErrType.Source, m.TotalError.ErrType);
            Assert.AreEqual(true, m.Boolean);
            Assert.AreEqual(1, m.Integer);
            Assert.AreEqual(1.0, m.Real);
            Assert.AreEqual("1", m.String);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.Boolean, m.DataType);
            Assert.IsTrue(m1.ValueEquals(m));
            Assert.IsTrue(m1.ValueAndErrorEquals(m));
            Assert.IsTrue(m.ValueEquals(m1));
            Assert.IsTrue(m.ValueAndErrorEquals(m1));

            var err1 = pool.MakeError(1, c);
            m = MFactory.NewMom(DataType.Boolean, d, 1, err1);
            Assert.AreEqual(d1, m1.Time);
            Assert.IsNotNull(m.Error);
            Assert.IsNotNull(m.TotalError);
            Assert.AreEqual(1, m.Error.Number);
            Assert.AreEqual("Warning", m.Error.Text);
            Assert.AreEqual(MomErrType.Source, m.Error.ErrType);
            Assert.AreEqual(1, m.TotalError.Number);
            Assert.AreEqual("Warning", m.TotalError.Text);
            Assert.AreEqual(MomErrType.Source, m.TotalError.ErrType);
            Assert.AreEqual(true, m.Boolean);
            Assert.AreEqual(1, m.Integer);
            Assert.AreEqual(1.0, m.Real);
            Assert.AreEqual("1", m.String);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.Boolean, m.DataType);
            Assert.IsTrue(m1.ValueEquals(m));
            Assert.IsFalse(m1.ValueAndErrorEquals(m));
            Assert.IsTrue(m.ValueEquals(m1));
            Assert.IsFalse(m.ValueAndErrorEquals(m1));

            var me = new EditMom(DataType.Boolean) {Boolean = true};
            Assert.AreEqual(true, me.Boolean);
            Assert.AreEqual(1, me.Integer);
            Assert.AreEqual(1.0, me.Real);
            Assert.AreEqual("1", me.String);
            Assert.AreEqual(1, me.Count);
            me.Integer = 0;
            Assert.AreEqual(false, me.Boolean);
            Assert.AreEqual(0, me.Integer);
            Assert.AreEqual(0.0, me.Real);
            Assert.AreEqual("0", me.String);
            Assert.AreEqual(1, me.Count);
            Assert.IsNotNull(me.LastMom);
            Assert.AreEqual(false, me.LastMom.Boolean);
        }

        [TestMethod]
        public void MomInt()
        {
            var c = new ContextTest("Context");
            var pool = MakeErrPool();
            var err = pool.MakeError(2, c);

            var d = new DateTime(2016, 06, 16, 11, 23, 00);
            var m = MFactory.NewMom(d, 24);
            Assert.AreEqual(d, m.Time);
            Assert.IsNull(m.Error);
            Assert.IsNull(m.TotalError);
            Assert.AreEqual(true, m.Boolean);
            Assert.AreEqual(24, m.Integer);
            Assert.AreEqual(24.0, m.Real);
            Assert.AreEqual("24", m.String);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.Integer, m.DataType);
            var v = m.Value;
            Assert.AreEqual(DataType.Integer, v.DataType);
            var cv = m.CalcValue;
            Assert.AreEqual(DataType.Integer, cv.DataType);
            Assert.IsNull(cv.TotalError);
            Assert.IsNotNull(m.LastMom);
            Assert.AreEqual(24, m.LastMom.Integer);
            Assert.AreEqual(d, m.TimeI(0));
            Assert.IsNull(m.ErrorI(0));
            Assert.AreEqual(true, m.BooleanI(0));
            Assert.AreEqual(24, m.IntegerI(0));
            Assert.AreEqual(24.0, m.RealI(0));
            Assert.AreEqual("24", m.StringI(0));

            var mn = m.ToMean(err);
            Assert.AreEqual(Different.MinDate, mn.Time);
            Assert.IsNotNull(mn.Error);
            Assert.IsNotNull(mn.TotalError);
            Assert.AreEqual(2, mn.Error.Number);
            Assert.AreEqual("Error", mn.Error.Text);
            Assert.AreEqual(MomErrType.Source, mn.Error.ErrType);
            Assert.AreEqual(true, mn.Boolean);
            Assert.AreEqual(24, mn.Integer);
            Assert.AreEqual(24.0, mn.Real);
            Assert.AreEqual("24", mn.String);
            Assert.AreEqual(1, mn.Count);
            Assert.AreEqual(DataType.Integer, m.DataType);
            Assert.IsNotNull(mn.LastMom);
            Assert.AreEqual(24, mn.LastMom.Integer);
            Assert.AreEqual(Different.MinDate, mn.TimeI(0));
            Assert.IsNotNull(mn.ErrorI(0));
            Assert.AreEqual(2, mn.ErrorI(0).Number);
            Assert.AreEqual("Error", mn.ErrorI(0).Text);
            Assert.AreEqual(MomErrType.Source, mn.ErrorI(0).ErrType);
            Assert.AreEqual(true, mn.BooleanI(0));
            Assert.AreEqual(24, mn.IntegerI(0));
            Assert.AreEqual(24.0, mn.RealI(0));
            Assert.AreEqual("24", mn.StringI(0));

            var d1 = new DateTime(2016, 06, 16, 11, 26, 00);
            var m1 = m.ToMom(d1, err);
            Assert.AreEqual(d1, m1.Time);
            Assert.IsNotNull(m1.Error);
            Assert.IsNotNull(m1.TotalError);
            Assert.AreEqual(2, m1.Error.Number);
            Assert.AreEqual("Error", m1.Error.Text);
            Assert.AreEqual(MomErrType.Source, m1.Error.ErrType);
            Assert.AreEqual(true, m1.Boolean);
            Assert.AreEqual(24, m1.Integer);
            Assert.AreEqual(24.0, m1.Real);
            Assert.AreEqual("24", m1.String);
            Assert.AreEqual(1, m1.Count);
            Assert.AreEqual(DataType.Integer, m1.DataType);
            Assert.IsTrue(m1.ValueEquals(m));
            Assert.IsFalse(m1.ValueAndErrorEquals(m));
            Assert.IsTrue(m.ValueEquals(m1));
            Assert.IsFalse(m.ValueAndErrorEquals(m1));

            m = MFactory.NewMom(d1, 24, err);
            Assert.IsNotNull(m.Error);
            Assert.IsNotNull(m.TotalError);
            Assert.AreEqual(2, m.Error.Number);
            Assert.AreEqual("Error", m.Error.Text);
            Assert.AreEqual(MomErrType.Source, m.Error.ErrType);
            Assert.AreEqual(2, m.TotalError.Number);
            Assert.AreEqual("Error", m.TotalError.Text);
            Assert.AreEqual(MomErrType.Source, m.TotalError.ErrType);
            Assert.AreEqual(true, m.Boolean);
            Assert.AreEqual(24, m.Integer);
            Assert.AreEqual(24.0, m.Real);
            Assert.AreEqual("24", m.String);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.Integer, m.DataType);
            Assert.IsTrue(m1.ValueEquals(m));
            Assert.IsTrue(m1.ValueAndErrorEquals(m));
            Assert.IsTrue(m.ValueEquals(m1));
            Assert.IsTrue(m.ValueAndErrorEquals(m1));

            var err1 = pool.MakeError(1, c);
            m = MFactory.NewMom(DataType.Integer, d, "24", err1);
            Assert.AreEqual(d, m.Time);
            Assert.IsNotNull(m.Error);
            Assert.IsNotNull(m.TotalError);
            Assert.AreEqual(1, m.Error.Number);
            Assert.AreEqual("Warning", m.Error.Text);
            Assert.AreEqual(MomErrType.Source, m.Error.ErrType);
            Assert.AreEqual(1, m.TotalError.Number);
            Assert.AreEqual("Warning", m.TotalError.Text);
            Assert.AreEqual(MomErrType.Source, m.TotalError.ErrType);
            Assert.AreEqual(true, m.Boolean);
            Assert.AreEqual(24, m.Integer);
            Assert.AreEqual(24.0, m.Real);
            Assert.AreEqual("24", m.String);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.Integer, m.DataType);
            Assert.IsTrue(m1.ValueEquals(m));
            Assert.IsFalse(m1.ValueAndErrorEquals(m));
            Assert.IsTrue(m.ValueEquals(m1));
            Assert.IsFalse(m.ValueAndErrorEquals(m1));

            var me = new EditMom(DataType.Integer) { Integer = 20};
            Assert.AreEqual(true, me.Boolean);
            Assert.AreEqual(20, me.Integer);
            Assert.AreEqual(20.0, me.Real);
            Assert.AreEqual("20", me.String);
            Assert.AreEqual(1, me.Count);
            me.String = "24";
            Assert.AreEqual(true, me.Boolean);
            Assert.AreEqual(24, me.Integer);
            Assert.AreEqual(24.0, me.Real);
            Assert.AreEqual("24", me.String);
            Assert.AreEqual(1, me.Count);
            Assert.IsNotNull(me.LastMom);
            Assert.AreEqual(24, me.LastMom.Integer);
            Assert.IsTrue(m.ValueEquals(me));
            Assert.IsTrue(me.ValueEquals(m));
            Assert.IsFalse(m.ValueLess(me));
            me.Time = d;
            Assert.AreEqual(d, me.Time);
            me.Error = err;
            Assert.IsNotNull(me.Error);
            Assert.AreEqual(2, me.Error.Number);
            Assert.AreEqual("Error", me.Error.Text);
            Assert.AreEqual(MomErrType.Source, me.Error.ErrType);
            Assert.IsFalse(m.ValueAndErrorEquals(me));
            Assert.IsFalse(me.ValueAndErrorEquals(m));
            me.CopyAllFrom(m);
            Assert.IsNotNull(me.Error);
            Assert.AreEqual(1, m.Error.Number);
            Assert.AreEqual("Warning", m.Error.Text);
            Assert.AreEqual(MomErrType.Source, m.Error.ErrType);
            Assert.AreEqual(d, me.Time);
            Assert.IsTrue(m.ValueEquals(me));
            Assert.IsTrue(me.ValueEquals(m));
            Assert.IsTrue(m.ValueAndErrorEquals(me));
            Assert.IsTrue(me.ValueAndErrorEquals(m));
        }

        [TestMethod]
        public void MomReal()
        {
            var c = new ContextTest("Context");
            var pool = MakeErrPool();
            var err = pool.MakeError(2, c);

            var d = new DateTime(2016, 06, 16, 11, 23, 00);
            var m = MFactory.NewMom(d, 2.1);
            Assert.AreEqual(d, m.Time);
            Assert.IsNull(m.Error);
            Assert.IsNull(m.TotalError);
            Assert.AreEqual(true, m.Boolean);
            Assert.AreEqual(2, m.Integer);
            Assert.AreEqual(2.1, m.Real);
            Assert.AreEqual("2,1", m.String);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.Real, m.DataType);
            var v = m.Value;
            Assert.AreEqual(DataType.Real, v.DataType);
            var cv = m.CalcValue;
            Assert.AreEqual(DataType.Real, cv.DataType);
            Assert.IsNull(cv.TotalError);
            Assert.IsNotNull(m.LastMom);
            Assert.AreEqual(2.1, m.LastMom.Real);
            Assert.AreEqual(d, m.TimeI(0));
            Assert.IsNull(m.ErrorI(0));
            Assert.AreEqual(true, m.BooleanI(0));
            Assert.AreEqual(2, m.IntegerI(0));
            Assert.AreEqual(2.1, m.RealI(0));
            Assert.AreEqual("2,1", m.StringI(0));
            
            var d1 = new DateTime(2016, 06, 16, 11, 26, 00);
            var m1 = m.ToMom(d1);
            Assert.AreEqual(d1, m1.Time);
            Assert.IsNull(m1.Error);
            Assert.IsNull(m1.TotalError);
            Assert.AreEqual(true, m1.Boolean);
            Assert.AreEqual(2, m1.Integer);
            Assert.AreEqual(2.1, m1.Real);
            Assert.AreEqual("2,1", m1.String);
            Assert.AreEqual(1, m1.Count);
            Assert.AreEqual(DataType.Real, m1.DataType);
            Assert.IsTrue(m1.ValueEquals(m));
            Assert.IsTrue(m1.ValueAndErrorEquals(m));
            Assert.IsTrue(m.ValueEquals(m1));
            Assert.IsTrue(m.ValueAndErrorEquals(m1));
            Assert.IsFalse(m.ValueLess(m1));
            Assert.IsFalse(m1.ValueLess(m));

            var m2 = m1.ToMom();
            Assert.AreEqual(d1, m1.Time);
            Assert.IsNull(m2.Error);
            Assert.IsNull(m2.TotalError);
            Assert.AreEqual(true, m2.Boolean);
            Assert.AreEqual(2, m2.Integer);
            Assert.AreEqual(2.1, m2.Real);
            Assert.AreEqual("2,1", m2.String);
            Assert.AreEqual(1, m2.Count);
            Assert.AreEqual(DataType.Real, m2.DataType);
            
            m = MFactory.NewMom(d1, -1.3e24, err);
            Assert.IsNotNull(m.Error);
            Assert.IsNotNull(m.TotalError);
            Assert.AreEqual(2, m.Error.Number);
            Assert.AreEqual("Error", m.Error.Text);
            Assert.AreEqual(MomErrType.Source, m.Error.ErrType);
            Assert.AreEqual(true, m.Boolean);
            Assert.AreEqual(0, m.Integer);
            Assert.AreEqual(-1.3e24, m.Real);
            Assert.AreEqual("-1,3E+24", m.String);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.Real, m.DataType);
            Assert.IsFalse(m1.ValueEquals(m));
            Assert.IsFalse(m1.ValueAndErrorEquals(m));
            Assert.IsFalse(m.ValueEquals(m1));
            Assert.IsFalse(m.ValueAndErrorEquals(m1));
            Assert.IsTrue(m.ValueLess(m1));
            Assert.IsFalse(m1.ValueLess(m));

            var err1 = pool.MakeError(1, c);
            m = MFactory.NewMom(DataType.Real, d, "5", err1);
            Assert.AreEqual(d1, m1.Time);
            Assert.IsNotNull(m.Error);
            Assert.IsNotNull(m.TotalError);
            Assert.AreEqual(1, m.Error.Number);
            Assert.AreEqual("Warning", m.Error.Text);
            Assert.AreEqual(MomErrType.Source, m.Error.ErrType);
            Assert.AreEqual(1, m.TotalError.Number);
            Assert.AreEqual("Warning", m.TotalError.Text);
            Assert.AreEqual(MomErrType.Source, m.TotalError.ErrType);
            Assert.AreEqual(true, m.Boolean);
            Assert.AreEqual(5, m.Integer);
            Assert.AreEqual(5, m.Real);
            Assert.AreEqual("5", m.String);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.Real, m.DataType);
            Assert.IsFalse(m1.ValueEquals(m));
            Assert.IsFalse(m1.ValueAndErrorEquals(m));
            Assert.IsFalse(m.ValueEquals(m1));
            Assert.IsFalse(m.ValueAndErrorEquals(m1));

            var me = new EditMom(DataType.Real, d) { Real = 0 };
            Assert.IsNull(me.Error);
            Assert.AreEqual(d, me.Time);
            Assert.AreEqual(false, me.Boolean);
            Assert.AreEqual(0, me.Integer);
            Assert.AreEqual(0.0, me.Real);
            Assert.AreEqual("0", me.String);
            Assert.AreEqual(Different.MinDate, me.Date);
            Assert.AreEqual(1, me.Count);
            Assert.AreEqual(DataType.Real, me.DataType);
            me.Integer = 45;
            Assert.AreEqual(d, me.Time);
            Assert.AreEqual(true, me.Boolean);
            Assert.AreEqual(45, me.Integer);
            Assert.AreEqual(45.0, me.Real);
            Assert.AreEqual("45", me.String);
            Assert.AreEqual(1, me.Count);
            Assert.IsNotNull(me.LastMom);
            Assert.AreEqual(45, me.LastMom.Integer);
            Assert.IsFalse(m.ValueEquals(me));
            Assert.IsFalse(me.ValueEquals(m));
            Assert.IsTrue(m.ValueLess(me));
            me.Time = d1;
            Assert.AreEqual(d1, me.Time);
            me.Error = err;
            Assert.IsNotNull(me.Error);
            Assert.AreEqual(2, me.Error.Number);
            Assert.AreEqual("Error", me.Error.Text);
            Assert.AreEqual(MomErrType.Source, me.Error.ErrType);
            cv = me.CalcValue;
            Assert.AreEqual(DataType.Real, cv.DataType);
            Assert.IsNotNull(cv.TotalError);
        }

        [TestMethod]
        public void MomString()
        {
            var c = new ContextTest("Context");
            var pool = MakeErrPool();
            var err = pool.MakeError(2, c);

            var d = new DateTime(2016, 06, 16, 11, 23, 00);
            var m = MFactory.NewMom(d, "sss");
            Assert.AreEqual(d, m.Time);
            Assert.IsNull(m.Error);
            Assert.IsNull(m.TotalError);
            Assert.AreEqual(true, m.Boolean);
            Assert.AreEqual(0, m.Integer);
            Assert.AreEqual(0.0, m.Real);
            Assert.AreEqual("sss", m.String);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.String, m.DataType);
            var v = m.Value;
            Assert.AreEqual(DataType.String, v.DataType);
            var cv = m.CalcValue;
            Assert.AreEqual(DataType.String, cv.DataType);
            Assert.IsNull(cv.TotalError);
            Assert.IsNotNull(m.LastMom);
            Assert.AreEqual("sss", m.LastMom.String);
            Assert.AreEqual(d, m.TimeI(0));
            Assert.IsNull(m.ErrorI(0));
            Assert.AreEqual(true, m.BooleanI(0));
            Assert.AreEqual(0, m.IntegerI(0));
            Assert.AreEqual(0.0, m.RealI(0));
            Assert.AreEqual("sss", m.StringI(0));

            var d1 = new DateTime(2016, 06, 16, 11, 26, 00);
            var m1 = m.ToMom(d1, err);
            Assert.AreEqual(d1, m1.Time);
            Assert.IsNotNull(m1.Error);
            Assert.IsNotNull(m1.TotalError);
            Assert.AreEqual(2, m1.Error.Number);
            Assert.AreEqual("Error", m1.Error.Text);
            Assert.AreEqual(MomErrType.Source, m1.Error.ErrType);
            Assert.AreEqual(true, m1.Boolean);
            Assert.AreEqual(0, m1.Integer);
            Assert.AreEqual(0, m1.Real);
            Assert.AreEqual("sss", m1.String);
            Assert.AreEqual(1, m1.Count);
            Assert.AreEqual(DataType.String, m1.DataType);
            Assert.IsTrue(m1.ValueEquals(m));
            Assert.IsFalse(m1.ValueAndErrorEquals(m));
            Assert.IsTrue(m.ValueEquals(m1));
            Assert.IsFalse(m.ValueAndErrorEquals(m1));

            var m2 = m.ToMomI(0, d1, err);
            Assert.AreEqual(d1, m2.Time);
            Assert.IsNotNull(m2.Error);
            Assert.IsNotNull(m2.TotalError);
            Assert.AreEqual(2, m2.Error.Number);
            Assert.AreEqual("Error", m2.Error.Text);
            Assert.AreEqual(MomErrType.Source, m2.Error.ErrType);
            Assert.AreEqual(true, m2.Boolean);
            Assert.AreEqual(0, m2.Integer);
            Assert.AreEqual(0, m2.Real);
            Assert.AreEqual("sss", m2.String);
            Assert.AreEqual(1, m2.Count);
            Assert.AreEqual(DataType.String, m2.DataType);
            Assert.IsTrue(m2.ValueEquals(m));
            Assert.IsFalse(m2.ValueAndErrorEquals(m));
            Assert.IsTrue(m.ValueEquals(m2));
            Assert.IsFalse(m.ValueAndErrorEquals(m2));

            m = MFactory.NewMom(d1, "45.67", err);
            Assert.IsNotNull(m.Error);
            Assert.IsNotNull(m.TotalError);
            Assert.AreEqual(2, m.Error.Number);
            Assert.AreEqual("Error", m.Error.Text);
            Assert.AreEqual(MomErrType.Source, m.Error.ErrType);
            Assert.AreEqual(2, m.TotalError.Number);
            Assert.AreEqual("Error", m.TotalError.Text);
            Assert.AreEqual(MomErrType.Source, m.TotalError.ErrType);
            Assert.AreEqual(true, m.Boolean);
            Assert.AreEqual(0, m.Integer);
            Assert.AreEqual(45.67, m.Real);
            Assert.AreEqual("45.67", m.String);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.String, m.DataType);
            Assert.IsFalse(m1.ValueEquals(m));
            Assert.IsFalse(m1.ValueAndErrorEquals(m));
            Assert.IsFalse(m.ValueEquals(m1));
            Assert.IsFalse(m.ValueAndErrorEquals(m1));

            var err1 = pool.MakeError(1, c);
            m = MFactory.NewMom(DataType.String, d, false, err1);
            Assert.AreEqual(d1, m1.Time);
            Assert.IsNotNull(m.Error);
            Assert.IsNotNull(m.TotalError);
            Assert.AreEqual(1, m.Error.Number);
            Assert.AreEqual("Warning", m.Error.Text);
            Assert.AreEqual(MomErrType.Source, m.Error.ErrType);
            Assert.AreEqual(1, m.TotalError.Number);
            Assert.AreEqual("Warning", m.TotalError.Text);
            Assert.AreEqual(MomErrType.Source, m.TotalError.ErrType);
            Assert.AreEqual(false, m.Boolean);
            Assert.AreEqual(0, m.Integer);
            Assert.AreEqual(0.0, m.Real);
            Assert.AreEqual("0", m.String);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.String, m.DataType);
            Assert.IsFalse(m1.ValueEquals(m));
            Assert.IsFalse(m1.ValueAndErrorEquals(m));
            Assert.IsFalse(m.ValueEquals(m1));
            Assert.IsFalse(m.ValueAndErrorEquals(m1));

            var me = new EditMom(DataType.String, d, err) { String = "123.456" };
            Assert.IsNotNull(me.Error);
            Assert.AreEqual(2, me.Error.Number);
            Assert.AreEqual("Error", me.Error.Text);
            Assert.AreEqual(MomErrType.Source, me.Error.ErrType);
            Assert.AreEqual(d, me.Time);
            Assert.AreEqual(true, me.Boolean);
            Assert.AreEqual(0, me.Integer);
            Assert.AreEqual(123.456, me.Real);
            Assert.AreEqual("123.456", me.String);
            Assert.AreEqual(Different.MinDate, me.Date);
            Assert.AreEqual(1, me.Count);
            Assert.AreEqual(DataType.String, me.DataType);
            me.Integer = 45;
            Assert.AreEqual(d, me.Time);
            Assert.AreEqual(true, me.Boolean);
            Assert.AreEqual(45, me.Integer);
            Assert.AreEqual(45.0, me.Real);
            Assert.AreEqual("45", me.String);
            Assert.AreEqual(1, me.Count);
            Assert.IsNotNull(me.LastMom);
            Assert.AreEqual(45, me.LastMom.Integer);
            Assert.IsFalse(m.ValueEquals(me));
            Assert.IsFalse(me.ValueEquals(m));
            Assert.IsTrue(m.ValueLess(me));
            me.Time = d1;
            Assert.AreEqual(d1, me.Time);
            me.Error = null;
            Assert.IsNull(me.Error);
            v = me.Value;
            Assert.AreEqual(DataType.String, v.DataType);
            Assert.IsTrue(m.ValueLess(me));
            me.CopyValueFrom(m);
            Assert.AreEqual("0", me.String);
        }

        [TestMethod]
        public void MomWeighted()
        {
            var c = new ContextTest("Context");
            var pool = MakeErrPool();
            var err = pool.MakeError(2, c);

            var d = new DateTime(2016, 06, 16, 11, 23, 00);
            var m = new WeightedMom(d, 2.1, 0.5);
            Assert.AreEqual(d, m.Time);
            Assert.IsNull(m.Error);
            Assert.IsNull(m.TotalError);
            Assert.AreEqual(0.5, m.Weight);
            Assert.AreEqual(true, m.Boolean);
            Assert.AreEqual(2, m.Integer);
            Assert.AreEqual(2.1, m.Real);
            Assert.AreEqual("2,1", m.String);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.Weighted, m.DataType);
            var v = m.Value;
            Assert.AreEqual(DataType.Weighted, v.DataType);
            var cv = m.CalcValue;
            Assert.AreEqual(DataType.Weighted, cv.DataType);
            Assert.IsNull(cv.TotalError);
            Assert.IsNotNull(m.LastMom);
            Assert.AreEqual(2.1, m.LastMom.Real);
            Assert.AreEqual(0.5, ((WeightedMom)m.LastMom).Weight);

            var d1 = new DateTime(2016, 06, 16, 11, 26, 00);
            var m1 = (WeightedMom)m.ToMom(d1, err);
            Assert.AreEqual(0.5, m1.Weight);
            Assert.AreEqual(d1, m1.Time);
            Assert.IsNotNull(m1.Error);
            Assert.IsNotNull(m1.TotalError);
            Assert.AreEqual(2, m1.Error.Number);
            Assert.AreEqual("Error", m1.Error.Text);
            Assert.AreEqual(MomErrType.Source, m1.Error.ErrType);
            Assert.AreEqual(true, m1.Boolean);
            Assert.AreEqual(2, m1.Integer);
            Assert.AreEqual(2.1, m1.Real);
            Assert.AreEqual("2,1", m1.String);
            Assert.AreEqual(1, m1.Count);
            Assert.AreEqual(DataType.Weighted, m1.DataType);
            Assert.IsTrue(m1.ValueEquals(m));
            Assert.IsFalse(m1.ValueAndErrorEquals(m));
            Assert.IsTrue(m.ValueEquals(m1));
            Assert.IsFalse(m.ValueAndErrorEquals(m1));
            Assert.IsFalse(m.ValueLess(m1));
            Assert.IsFalse(m1.ValueLess(m));
            
            var me = new EditMom(DataType.Real, d) { Real = 0 };
            Assert.IsNull(me.Error);
            Assert.AreEqual(d, me.Time);
            Assert.AreEqual(false, me.Boolean);
            Assert.AreEqual(0, me.Integer);
            Assert.AreEqual(0.0, me.Real);
            Assert.AreEqual("0", me.String);
            Assert.AreEqual(Different.MinDate, me.Date);
            Assert.AreEqual(1, me.Count);
            Assert.AreEqual(DataType.Real, me.DataType);
            me.Integer = 45;
            Assert.AreEqual(d, me.Time);
            Assert.AreEqual(true, me.Boolean);
            Assert.AreEqual(45, me.Integer);
            Assert.AreEqual(45.0, me.Real);
            Assert.AreEqual("45", me.String);
            Assert.AreEqual(1, me.Count);
            Assert.IsNotNull(me.LastMom);
            Assert.AreEqual(45, me.LastMom.Integer);
            Assert.IsFalse(m.ValueEquals(me));
            Assert.IsFalse(me.ValueEquals(m));
            Assert.IsTrue(m.ValueLess(me));
            me.Time = d1;
            Assert.AreEqual(d1, me.Time);
            me.Error = err;
            Assert.IsNotNull(me.Error);
            Assert.AreEqual(2, me.Error.Number);
            Assert.AreEqual("Error", me.Error.Text);
            Assert.AreEqual(MomErrType.Source, me.Error.ErrType);
            cv = me.CalcValue;
            Assert.AreEqual(DataType.Real, cv.DataType);
            Assert.IsNotNull(cv.TotalError);
        }
    }
}