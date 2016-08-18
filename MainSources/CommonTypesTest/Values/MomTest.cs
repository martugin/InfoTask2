using System;
using BaseLibrary;
using CommonTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Different = BaseLibrary.Different;

namespace CommonTypesTest
{
    [TestClass]
    public class MomTest
    {
        private static ErrMomPool MakeErrPool()
        {
            var ef = new ErrMomFactory("ErrSource", ErrMomType.Source);
            ef.AddGoodDescr(0);
            ef.AddDescr(1, "Warning");
            ef.AddDescr(2, "Error");
            return new ErrMomPool(ef);
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
            Assert.IsNotNull(m.LastMean);
            Assert.AreEqual(true, m.LastMean.Boolean);

            var d1 = new DateTime(2016, 06, 16, 11, 26, 00);
            var m1 = m.Clone(d1, err);
            Assert.AreEqual(d1, m1.Time);
            Assert.IsNotNull(m1.Error);
            Assert.IsNotNull(m1.TotalError);
            Assert.AreEqual(2, m1.Error.Number);
            Assert.AreEqual("Error", m1.Error.Text);
            Assert.AreEqual(ErrMomType.Source, m1.Error.ErrType);
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
            Assert.AreEqual(ErrMomType.Source, m.Error.ErrType);
            Assert.AreEqual(2, m.TotalError.Number);
            Assert.AreEqual("Error", m.TotalError.Text);
            Assert.AreEqual(ErrMomType.Source, m.TotalError.ErrType);
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
            Assert.AreEqual(ErrMomType.Source, m.Error.ErrType);
            Assert.AreEqual(1, m.TotalError.Number);
            Assert.AreEqual("Warning", m.TotalError.Text);
            Assert.AreEqual(ErrMomType.Source, m.TotalError.ErrType);
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

            var me = new MomEdit(DataType.Boolean) {Boolean = true};
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
            Assert.IsNotNull(me.LastMean);
            Assert.AreEqual(false, me.LastMean.Boolean);
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
            Assert.IsNotNull(m.LastMean);
            Assert.AreEqual(24, m.LastMean.Integer);

            var d1 = new DateTime(2016, 06, 16, 11, 26, 00);
            var m1 = m.Clone(d1, err);
            Assert.AreEqual(d1, m1.Time);
            Assert.IsNotNull(m1.Error);
            Assert.IsNotNull(m1.TotalError);
            Assert.AreEqual(2, m1.Error.Number);
            Assert.AreEqual("Error", m1.Error.Text);
            Assert.AreEqual(ErrMomType.Source, m1.Error.ErrType);
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
            Assert.AreEqual(ErrMomType.Source, m.Error.ErrType);
            Assert.AreEqual(2, m.TotalError.Number);
            Assert.AreEqual("Error", m.TotalError.Text);
            Assert.AreEqual(ErrMomType.Source, m.TotalError.ErrType);
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
            Assert.AreEqual(ErrMomType.Source, m.Error.ErrType);
            Assert.AreEqual(1, m.TotalError.Number);
            Assert.AreEqual("Warning", m.TotalError.Text);
            Assert.AreEqual(ErrMomType.Source, m.TotalError.ErrType);
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

            var me = new MomEdit(DataType.Integer) { Integer = 20};
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
            Assert.IsNotNull(me.LastMean);
            Assert.AreEqual(24, me.LastMean.Integer);
            Assert.IsTrue(m.ValueEquals(me));
            Assert.IsTrue(me.ValueEquals(m));
            Assert.IsFalse(m.ValueLess(me));
            me.Time = d;
            Assert.AreEqual(d, me.Time);
            me.Error = err;
            Assert.IsNotNull(me.Error);
            Assert.AreEqual(2, me.Error.Number);
            Assert.AreEqual("Error", me.Error.Text);
            Assert.AreEqual(ErrMomType.Source, me.Error.ErrType);
            Assert.IsFalse(m.ValueAndErrorEquals(me));
            Assert.IsFalse(me.ValueAndErrorEquals(m));
            me.CopyAllFrom(m);
            Assert.IsNotNull(me.Error);
            Assert.AreEqual(1, m.Error.Number);
            Assert.AreEqual("Warning", m.Error.Text);
            Assert.AreEqual(ErrMomType.Source, m.Error.ErrType);
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
            Assert.IsNotNull(m.LastMean);
            Assert.AreEqual(2.1, m.LastMean.Real);

            var d1 = new DateTime(2016, 06, 16, 11, 26, 00);
            var m1 = m.Clone(d1);
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

            m = MFactory.NewMom(d1, -1.3e24, err);
            Assert.IsNotNull(m.Error);
            Assert.IsNotNull(m.TotalError);
            Assert.AreEqual(2, m.Error.Number);
            Assert.AreEqual("Error", m.Error.Text);
            Assert.AreEqual(ErrMomType.Source, m.Error.ErrType);
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
            Assert.AreEqual(ErrMomType.Source, m.Error.ErrType);
            Assert.AreEqual(1, m.TotalError.Number);
            Assert.AreEqual("Warning", m.TotalError.Text);
            Assert.AreEqual(ErrMomType.Source, m.TotalError.ErrType);
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

            var me = new MomEdit(DataType.Real, d) { Real = 0 };
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
            Assert.IsNotNull(me.LastMean);
            Assert.AreEqual(45, me.LastMean.Integer);
            Assert.IsFalse(m.ValueEquals(me));
            Assert.IsFalse(me.ValueEquals(m));
            Assert.IsTrue(m.ValueLess(me));
            me.Time = d1;
            Assert.AreEqual(d1, me.Time);
            me.Error = err;
            Assert.IsNotNull(me.Error);
            Assert.AreEqual(2, me.Error.Number);
            Assert.AreEqual("Error", me.Error.Text);
            Assert.AreEqual(ErrMomType.Source, me.Error.ErrType);
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
            Assert.IsNotNull(m.LastMean);
            Assert.AreEqual("sss", m.LastMean.String);

            var d1 = new DateTime(2016, 06, 16, 11, 26, 00);
            var m1 = m.Clone(d1, err);
            Assert.AreEqual(d1, m1.Time);
            Assert.IsNotNull(m1.Error);
            Assert.IsNotNull(m1.TotalError);
            Assert.AreEqual(2, m1.Error.Number);
            Assert.AreEqual("Error", m1.Error.Text);
            Assert.AreEqual(ErrMomType.Source, m1.Error.ErrType);
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

            m = MFactory.NewMom(d1, "45.67", err);
            Assert.IsNotNull(m.Error);
            Assert.IsNotNull(m.TotalError);
            Assert.AreEqual(2, m.Error.Number);
            Assert.AreEqual("Error", m.Error.Text);
            Assert.AreEqual(ErrMomType.Source, m.Error.ErrType);
            Assert.AreEqual(2, m.TotalError.Number);
            Assert.AreEqual("Error", m.TotalError.Text);
            Assert.AreEqual(ErrMomType.Source, m.TotalError.ErrType);
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
            Assert.AreEqual(ErrMomType.Source, m.Error.ErrType);
            Assert.AreEqual(1, m.TotalError.Number);
            Assert.AreEqual("Warning", m.TotalError.Text);
            Assert.AreEqual(ErrMomType.Source, m.TotalError.ErrType);
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

            var me = new MomEdit(DataType.String, d, err) { String = "123.456" };
            Assert.IsNotNull(me.Error);
            Assert.AreEqual(2, me.Error.Number);
            Assert.AreEqual("Error", me.Error.Text);
            Assert.AreEqual(ErrMomType.Source, me.Error.ErrType);
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
            Assert.IsNotNull(me.LastMean);
            Assert.AreEqual(45, me.LastMean.Integer);
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
            var m = new MomWeighted(d, 2.1, 0.5);
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
            Assert.IsNotNull(m.LastMean);
            Assert.AreEqual(2.1, m.LastMean.Real);
            Assert.AreEqual(0.5, ((MomWeighted)m.LastMean).Weight);

            var d1 = new DateTime(2016, 06, 16, 11, 26, 00);
            var m1 = (MomWeighted)m.Clone(d1, err);
            Assert.AreEqual(0.5, m1.Weight);
            Assert.AreEqual(d1, m1.Time);
            Assert.IsNotNull(m1.Error);
            Assert.IsNotNull(m1.TotalError);
            Assert.AreEqual(2, m1.Error.Number);
            Assert.AreEqual("Error", m1.Error.Text);
            Assert.AreEqual(ErrMomType.Source, m1.Error.ErrType);
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
            
            var me = new MomEdit(DataType.Real, d) { Real = 0 };
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
            Assert.IsNotNull(me.LastMean);
            Assert.AreEqual(45, me.LastMean.Integer);
            Assert.IsFalse(m.ValueEquals(me));
            Assert.IsFalse(me.ValueEquals(m));
            Assert.IsTrue(m.ValueLess(me));
            me.Time = d1;
            Assert.AreEqual(d1, me.Time);
            me.Error = err;
            Assert.IsNotNull(me.Error);
            Assert.AreEqual(2, me.Error.Number);
            Assert.AreEqual("Error", me.Error.Text);
            Assert.AreEqual(ErrMomType.Source, me.Error.ErrType);
            cv = me.CalcValue;
            Assert.AreEqual(DataType.Real, cv.DataType);
            Assert.IsNotNull(cv.TotalError);
        }
    }
}