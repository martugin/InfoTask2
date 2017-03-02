using System;
using CommonTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Different = BaseLibrary.Different;

namespace CommonTypesTest
{
    [TestClass]
    public class MeanTest
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
        public void MeanBool()
        {
            var c = new ContextTest("Context");
            var pool = MakeErrPool();
            var err = pool.MakeError(2, c);

            var m = MFactory.NewMean(true);
            Assert.IsNull(m.Error);
            Assert.AreEqual(Different.MinDate, m.Time);
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
            Assert.AreEqual(true, m.BooleanI(0));
            Assert.AreEqual(1, m.IntegerI(0));
            Assert.AreEqual(1.0, m.RealI(0));
            Assert.AreEqual("1", m.StringI(0));
            Assert.IsNull(m.ErrorI(0));
            Assert.AreEqual(Different.MinDate, m.TimeI(0));

            m = m.ToMean();
            Assert.IsNull(m.Error);
            Assert.AreEqual(Different.MinDate, m.Time);
            Assert.IsNull(m.TotalError);
            Assert.AreEqual(true, m.Boolean);
            Assert.AreEqual(1, m.Integer);
            Assert.AreEqual(1.0, m.Real);
            Assert.AreEqual("1", m.String);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.Boolean, m.DataType);
            Assert.AreEqual(true, m.BooleanI(0));
            Assert.AreEqual(1, m.IntegerI(0));
            Assert.AreEqual(1.0, m.RealI(0));
            Assert.AreEqual("1", m.StringI(0));
            Assert.IsNull(m.ErrorI(0));
            Assert.AreEqual(Different.MinDate, m.TimeI(0));

            var m1 = m.ToMom(Different.MaxDate);
            Assert.AreEqual(Different.MaxDate, m1.Time);
            Assert.IsNull(m1.Error);
            Assert.IsNull(m1.TotalError);
            Assert.AreEqual(true, m1.Boolean);
            Assert.AreEqual(1, m1.Integer);
            Assert.AreEqual(1.0, m1.Real);
            Assert.AreEqual("1", m1.String);
            Assert.AreEqual(1, m1.Count);
            Assert.AreEqual(DataType.Boolean, m1.DataType);
            Assert.IsTrue(m1.ValueEquals(m));
            Assert.IsTrue(m1.ValueAndErrorEquals(m));
            Assert.IsTrue(m.ValueEquals(m1));
            Assert.IsTrue(m.ValueAndErrorEquals(m1));
            Assert.AreEqual(true, m1.BooleanI(0));
            Assert.AreEqual(1, m1.IntegerI(0));
            Assert.AreEqual(1.0, m1.RealI(0));
            Assert.AreEqual("1", m1.StringI(0));
            Assert.IsNull(m1.ErrorI(0));
            Assert.AreEqual(Different.MaxDate, m1.TimeI(0));

            m = MFactory.NewMean(false, err);
            Assert.IsNotNull(m.Error);
            Assert.IsNotNull(m.TotalError);
            Assert.AreEqual(2, m.Error.Number);
            Assert.AreEqual("Error", m.Error.Text);
            Assert.AreEqual(MomErrType.Source, m.Error.ErrType);
            Assert.AreEqual(2, m.TotalError.Number);
            Assert.AreEqual("Error", m.TotalError.Text);
            Assert.AreEqual(MomErrType.Source, m.TotalError.ErrType);
            Assert.AreEqual(false, m.Boolean);
            Assert.AreEqual(0, m.Integer);
            Assert.AreEqual(0.0, m.Real);
            Assert.AreEqual("0", m.String);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.Boolean, m.DataType);
            Assert.IsFalse(m1.ValueEquals(m));
            Assert.IsFalse(m1.ValueAndErrorEquals(m));
            Assert.IsFalse(m.ValueEquals(m1));
            Assert.IsFalse(m.ValueAndErrorEquals(m1));
            Assert.AreEqual(false, m.BooleanI(0));
            Assert.AreEqual(0, m.IntegerI(0));
            Assert.AreEqual(0.0, m.RealI(0));
            Assert.AreEqual("0", m.StringI(0));
            Assert.IsNotNull(m.ErrorI(0));
            Assert.AreEqual(2, m.ErrorI(0).Number);
            Assert.AreEqual("Error", m.ErrorI(0).Text);
            Assert.AreEqual(Different.MinDate, m.TimeI(0));

            var err1 = pool.MakeError(1, c);
            m = MFactory.NewMean(DataType.Boolean, 1, err1);
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
        }

        [TestMethod]
        public void MeanInt()
        {
            var c = new ContextTest("Context");
            var pool = MakeErrPool();
            var err = pool.MakeError(2, c);

            var m = MFactory.NewMean(24);
            Assert.IsNull(m.Error);
            Assert.IsNull(m.TotalError);
            Assert.AreEqual(true, m.Boolean);
            Assert.AreEqual(24, m.Integer);
            Assert.AreEqual(24.0, m.Real);
            Assert.AreEqual("24", m.String);
            Assert.AreEqual(Different.MinDate, m.Date);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.Integer, m.DataType);
            var v = m.Value;
            Assert.AreEqual(DataType.Integer, v.DataType);
            var cv = m.CalcValue;
            Assert.AreEqual(DataType.Integer, cv.DataType);
            Assert.IsNull(cv.TotalError);
            Assert.IsNull(m.ErrorI(0));
            Assert.AreEqual(true, m.BooleanI(0));
            Assert.AreEqual(24, m.IntegerI(0));
            Assert.AreEqual(24.0, m.RealI(0));
            Assert.AreEqual("24", m.StringI(0));
            Assert.AreEqual(Different.MinDate, m.TimeI(0));

            var e = pool.MakeError(4, c);
            var me = m.ToMean(e);
            Assert.IsNotNull(me.Error);
            Assert.IsNotNull(me.TotalError);
            Assert.AreEqual(4, me.Error.Number);
            Assert.AreEqual("Неопределенная ошибка", me.Error.Text);
            Assert.AreEqual(true, me.Boolean);
            Assert.AreEqual(24, me.Integer);
            Assert.AreEqual(24.0, me.Real);
            Assert.AreEqual("24", me.String);
            Assert.AreEqual(Different.MinDate, me.Date);
            Assert.AreEqual(1, me.Count);
            Assert.AreEqual(DataType.Integer, me.DataType);
            Assert.IsNotNull(me.ErrorI(0));
            Assert.AreEqual(4, me.ErrorI(0).Number);
            Assert.AreEqual(true, me.BooleanI(0));
            Assert.AreEqual(24, me.IntegerI(0));
            Assert.AreEqual(24.0, me.RealI(0));
            Assert.AreEqual("24", me.StringI(0));
            Assert.AreEqual(Different.MinDate, me.TimeI(0));

            me = m.ToMeanI(0, e);
            Assert.IsNotNull(me.Error);
            Assert.IsNotNull(me.TotalError);
            Assert.AreEqual(4, me.Error.Number);
            Assert.AreEqual("Неопределенная ошибка", me.Error.Text);
            Assert.AreEqual(true, me.Boolean);
            Assert.AreEqual(24, me.Integer);
            Assert.AreEqual(24.0, me.Real);
            Assert.AreEqual("24", me.String);
            Assert.AreEqual(Different.MinDate, me.Date);
            Assert.AreEqual(1, me.Count);
            Assert.AreEqual(DataType.Integer, me.DataType);
            

            var m1 = m.ToMom(Different.MaxDate, e);
            Assert.AreEqual(Different.MaxDate, m1.Time);
            Assert.IsNotNull(m1.Error);
            Assert.IsNotNull(m1.TotalError);
            Assert.AreEqual(4, m1.Error.Number);
            Assert.AreEqual("Неопределенная ошибка", m1.Error.Text);
            Assert.AreEqual(4, m1.TotalError.Number);
            Assert.AreEqual("Неопределенная ошибка", m1.TotalError.Text);
            Assert.AreEqual(4, m1.ErrorI(0).Number);
            Assert.AreEqual("Неопределенная ошибка", m1.ErrorI(0).Text);
            Assert.AreEqual(true, m1.Boolean);
            Assert.AreEqual(24, m1.Integer);
            Assert.AreEqual(24.0, m1.Real);
            Assert.AreEqual("24", m1.String);
            Assert.AreEqual(1, m1.Count);
            Assert.AreEqual(DataType.Integer, m1.DataType);
            Assert.IsTrue(m1.ValueEquals(m));
            Assert.IsFalse(m1.ValueAndErrorEquals(m));
            Assert.IsTrue(me.ValueEquals(m1));
            Assert.IsTrue(me.ValueAndErrorEquals(m1));

            m = MFactory.NewMean(-32, err);
            Assert.IsNotNull(m.Error);
            Assert.IsNotNull(m.TotalError);
            Assert.AreEqual(2, m.Error.Number);
            Assert.AreEqual("Error", m.Error.Text);
            Assert.AreEqual(MomErrType.Source, m.Error.ErrType);
            Assert.AreEqual(2, m.TotalError.Number);
            Assert.AreEqual("Error", m.TotalError.Text);
            Assert.AreEqual(MomErrType.Source, m.TotalError.ErrType);
            Assert.AreEqual(true, m.Boolean);
            Assert.AreEqual(-32, m.Integer);
            Assert.AreEqual(-32.0, m.Real);
            Assert.AreEqual("-32", m.String);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.Integer, m.DataType);
            Assert.IsFalse(m1.ValueEquals(m));
            Assert.IsFalse(m1.ValueAndErrorEquals(m));
            Assert.IsFalse(m.ValueEquals(m1));
            Assert.IsFalse(m.ValueAndErrorEquals(m1));
            Assert.IsTrue(m.ValueLess(m1));

            var err1 = pool.MakeError(1, c);
            m = MFactory.NewMean(DataType.Integer, "24", err1);
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
        }

        [TestMethod]
        public void MeanReal()
        {
            var c = new ContextTest("Context");
            var pool = MakeErrPool();
            var err = pool.MakeError(2, c);

            var m = MFactory.NewMean(33.4);
            Assert.IsNull(m.Error);
            Assert.IsNull(m.TotalError);
            Assert.AreEqual(true, m.Boolean);
            Assert.AreEqual(33, m.Integer);
            Assert.AreEqual(33.4, m.Real);
            Assert.AreEqual("33,4", m.String);
            Assert.AreEqual(Different.MinDate, m.Date);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.Real, m.DataType);
            var v = m.Value;
            Assert.AreEqual(DataType.Real, v.DataType);
            var cv = m.CalcValue;
            Assert.AreEqual(DataType.Real, cv.DataType);
            Assert.IsNull(cv.TotalError);
            Assert.IsNull(m.ErrorI(0));
            Assert.AreEqual(true, m.BooleanI(0));
            Assert.AreEqual(33, m.IntegerI(0));
            Assert.AreEqual(33.4, m.RealI(0));
            Assert.AreEqual("33,4", m.StringI(0));

            var mo = m.ToMom();
            Assert.AreEqual(Different.MinDate, mo.Time);
            Assert.IsNull(mo.Error);
            Assert.AreEqual(true, mo.Boolean);
            Assert.AreEqual(33, mo.Integer);
            Assert.AreEqual(33.4, mo.Real);
            Assert.AreEqual("33,4", mo.String);
            Assert.AreEqual(1, mo.Count);
            Assert.AreEqual(DataType.Real, mo.DataType);
            Assert.IsNull(mo.ErrorI(0));
            Assert.AreEqual(true, mo.BooleanI(0));
            Assert.AreEqual(33, mo.IntegerI(0));
            Assert.AreEqual(33.4, mo.RealI(0));
            Assert.AreEqual("33,4", mo.StringI(0));

            var m1 = m.ToMom(Different.MinDate, err);
            Assert.AreEqual(Different.MinDate, m1.Time);
            Assert.IsNotNull(m1.Error);
            Assert.IsNotNull(m1.TotalError);
            Assert.AreEqual(2, m1.Error.Number);
            Assert.AreEqual("Error", m1.Error.Text);
            Assert.AreEqual(MomErrType.Source, m1.Error.ErrType);
            Assert.AreEqual(true, m1.Boolean);
            Assert.AreEqual(33, m1.Integer);
            Assert.AreEqual(33.4, m1.Real);
            Assert.AreEqual("33,4", m1.String);
            Assert.AreEqual(1, m1.Count);
            Assert.AreEqual(DataType.Real, m1.DataType);
            Assert.IsTrue(m1.ValueEquals(m));
            Assert.IsFalse(m1.ValueAndErrorEquals(m));
            Assert.IsTrue(m.ValueEquals(m1));
            Assert.IsFalse(m.ValueAndErrorEquals(m1));

            m = MFactory.NewMean(33.4, err);
            Assert.IsNotNull(m.Error);
            Assert.IsNotNull(m.TotalError);
            Assert.AreEqual(2, m.Error.Number);
            Assert.AreEqual("Error", m.Error.Text);
            Assert.AreEqual(MomErrType.Source, m.Error.ErrType);
            Assert.AreEqual(true, m.Boolean);
            Assert.AreEqual(33, m.Integer);
            Assert.AreEqual(33.4, m.Real);
            Assert.AreEqual("33,4", m.String);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.Real, m.DataType);
            Assert.IsTrue(m1.ValueEquals(m));
            Assert.IsTrue(m1.ValueAndErrorEquals(m));
            Assert.IsTrue(m.ValueEquals(m1));
            Assert.IsTrue(m.ValueAndErrorEquals(m1));
            Assert.IsFalse(m.ValueLess(m1));

            var err1 = pool.MakeError(1, c);
            m = MFactory.NewMean(DataType.Real, "-2.7", err1);
            Assert.IsNotNull(m.Error);
            Assert.IsNotNull(m.TotalError);
            Assert.AreEqual(1, m.Error.Number);
            Assert.AreEqual("Warning", m.Error.Text);
            Assert.AreEqual(MomErrType.Source, m.Error.ErrType);
            Assert.AreEqual(true, m.Boolean);
            Assert.AreEqual(-3, m.Integer);
            Assert.AreEqual(-2.7, m.Real);
            Assert.AreEqual("-2,7", m.String);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.Real, m.DataType);
            Assert.IsFalse(m1.ValueEquals(m));
            Assert.IsFalse(m1.ValueAndErrorEquals(m));
            Assert.IsFalse(m.ValueEquals(m1));
            Assert.IsFalse(m.ValueAndErrorEquals(m1));
        }

        [TestMethod]
        public void MeanString()
        {
            var c = new ContextTest("Context");
            var pool = MakeErrPool();
            var err = pool.MakeError(2, c);

            var m = MFactory.NewMean("333");
            Assert.IsNull(m.Error);
            Assert.AreEqual(Different.MinDate, m.Time);
            Assert.IsNull(m.TotalError);
            Assert.AreEqual(true, m.Boolean);
            Assert.AreEqual(333, m.Integer);
            Assert.AreEqual(333.0, m.Real);
            Assert.AreEqual("333", m.String);
            Assert.AreEqual(Different.MinDate, m.Date);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.String, m.DataType);
            var v = m.Value;
            Assert.AreEqual(DataType.String, v.DataType);
            var cv = m.CalcValue;
            Assert.AreEqual(DataType.String, cv.DataType);
            Assert.IsNull(cv.TotalError);
            Assert.IsNull(m.ErrorI(0));
            Assert.AreEqual(true, m.BooleanI(0));
            Assert.AreEqual(333, m.IntegerI(0));
            Assert.AreEqual(333.0, m.RealI(0));
            Assert.AreEqual("333", m.StringI(0));
            Assert.AreEqual(Different.MaxDate, m.NextTime);
            m.CurNum = -1;
            Assert.AreEqual(Different.MinDate, m.NextTime);
            m.CurNum = 0;
            Assert.AreEqual(true, m.Boolean);
            Assert.AreEqual(333, m.Integer);
            Assert.AreEqual(333.0, m.Real);
            Assert.AreEqual("333", m.String);

            var m1 = m.ToMom(Different.MaxDate, err);
            Assert.AreEqual(Different.MaxDate, m1.Time);
            Assert.IsNotNull(m1.Error);
            Assert.IsNotNull(m1.TotalError);
            Assert.AreEqual(2, m1.Error.Number);
            Assert.AreEqual("Error", m1.Error.Text);
            Assert.AreEqual(MomErrType.Source, m1.Error.ErrType);
            Assert.AreEqual(true, m1.Boolean);
            Assert.AreEqual(333, m1.Integer);
            Assert.AreEqual(333.0, m1.Real);
            Assert.AreEqual("333", m1.String);
            Assert.AreEqual(1, m1.Count);
            Assert.AreEqual(DataType.String, m1.DataType);
            Assert.IsTrue(m1.ValueEquals(m));
            Assert.IsFalse(m1.ValueAndErrorEquals(m));
            Assert.IsTrue(m.ValueEquals(m1));
            Assert.IsFalse(m.ValueAndErrorEquals(m1));

            m = MFactory.NewMean("abc", err);
            Assert.IsNotNull(m.Error);
            Assert.IsNotNull(m.TotalError);
            Assert.AreEqual(2, m.Error.Number);
            Assert.AreEqual("Error", m.Error.Text);
            Assert.AreEqual(MomErrType.Source, m.Error.ErrType);
            Assert.AreEqual(true, m.Boolean);
            Assert.AreEqual(0, m.Integer);
            Assert.AreEqual(0.0, m.Real);
            Assert.AreEqual("abc", m.String);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.String, m.DataType);
            Assert.IsFalse(m1.ValueEquals(m));
            Assert.IsFalse(m1.ValueAndErrorEquals(m));
            Assert.IsFalse(m.ValueEquals(m1));
            Assert.IsFalse(m.ValueAndErrorEquals(m1));
            Assert.IsFalse(m.ValueLess(m1));
            Assert.IsTrue(m1.ValueLess(m));

            var err1 = pool.MakeError(1, c);
            m = MFactory.NewMean(DataType.String, -12.3, err1);
            Assert.IsNotNull(m.Error);
            Assert.IsNotNull(m.TotalError);
            Assert.AreEqual(1, m.Error.Number);
            Assert.AreEqual("Warning", m.Error.Text);
            Assert.AreEqual(MomErrType.Source, m.Error.ErrType);
            Assert.AreEqual(true, m.Boolean);
            Assert.AreEqual(0, m.Integer);
            Assert.AreEqual(-12.3, m.Real);
            Assert.AreEqual("-12,3", m.String);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.String, m.DataType);
            Assert.IsFalse(m1.ValueEquals(m));
            Assert.IsFalse(m1.ValueAndErrorEquals(m));
            Assert.IsFalse(m.ValueEquals(m1));
            Assert.IsFalse(m.ValueAndErrorEquals(m1));

            var d = new DateTime(2000, 2, 2, 15, 30, 0);
            m = MFactory.NewMean(DataType.String, d);
            Assert.IsNull(m.Error);
            Assert.AreEqual(d, m.Date);
        }

        [TestMethod]
        public void MeanTime()
        {
            var c = new ContextTest("Context");
            var pool = MakeErrPool();
            var err = pool.MakeError(2, c);

            var m = MFactory.NewMean(Different.MaxDate);
            Assert.IsNull(m.Error);
            Assert.IsNull(m.TotalError);
            Assert.AreEqual(false, m.Boolean);
            Assert.AreEqual(0, m.Integer);
            Assert.AreEqual(0.0, m.Real);
            Assert.AreEqual(Different.MaxDate, m.Date);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.Time, m.DataType);
            var v = m.Value;
            Assert.AreEqual(DataType.Time, v.DataType);
            var cv = m.CalcValue;
            Assert.AreEqual(DataType.Time, cv.DataType);
            Assert.IsNull(m.ErrorI(0));
            Assert.AreEqual(false, m.BooleanI(0));
            Assert.AreEqual(0, m.IntegerI(0));
            Assert.AreEqual(0.0, m.RealI(0));
            Assert.AreEqual(Different.MaxDate, m.DateI(0));

            var mo = m.ToMom(err);
            Assert.AreEqual(Different.MinDate, mo.Time);
            Assert.IsNotNull(mo.Error);
            Assert.IsNotNull(mo.TotalError);
            Assert.AreEqual(2, mo.Error.Number);
            Assert.AreEqual("Error", mo.Error.Text);
            Assert.AreEqual(MomErrType.Source, mo.Error.ErrType);
            Assert.AreEqual(false, mo.Boolean);
            Assert.AreEqual(0, mo.Integer);
            Assert.AreEqual(0.0, mo.Real);
            Assert.AreEqual(Different.MaxDate, mo.Date);
            Assert.AreEqual(1, mo.Count);
            Assert.AreEqual(DataType.Time, mo.DataType);
            Assert.IsTrue(mo.ValueEquals(m));
            Assert.IsFalse(mo.ValueAndErrorEquals(m));
            Assert.IsTrue(m.ValueEquals(mo));
            Assert.IsFalse(m.ValueAndErrorEquals(mo));

            var d = new DateTime(2000, 2, 2, 15, 30, 0);
            var m1 = m.ToMom(d, err);
            Assert.AreEqual(d, m1.Time);
            Assert.IsNotNull(m1.Error);
            Assert.IsNotNull(m1.TotalError);
            Assert.AreEqual(2, m1.Error.Number);
            Assert.AreEqual("Error", m1.Error.Text);
            Assert.AreEqual(MomErrType.Source, m1.Error.ErrType);
            Assert.AreEqual(false, m1.Boolean);
            Assert.AreEqual(0, m1.Integer);
            Assert.AreEqual(0.0, m1.Real);
            Assert.AreEqual(Different.MaxDate, m1.Date);
            Assert.AreEqual(1, m1.Count);
            Assert.AreEqual(DataType.Time, m1.DataType);
            Assert.IsTrue(m1.ValueEquals(m));
            Assert.IsFalse(m1.ValueAndErrorEquals(m));
            Assert.IsTrue(m.ValueEquals(m1));
            Assert.IsFalse(m.ValueAndErrorEquals(m1));

            m = MFactory.NewMean(d, err);
            Assert.IsNotNull(m.Error);
            Assert.IsNotNull(m.TotalError);
            Assert.AreEqual(2, m.Error.Number);
            Assert.AreEqual("Error", m.Error.Text);
            Assert.AreEqual(MomErrType.Source, m.Error.ErrType);
            Assert.AreEqual(false, m.Boolean);
            Assert.AreEqual(0, m.Integer);
            Assert.AreEqual(0.0, m.Real);
            Assert.AreEqual(d, m.Date);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.Time, m.DataType);
            Assert.IsFalse(m1.ValueEquals(m));
            Assert.IsFalse(m1.ValueAndErrorEquals(m));
            Assert.IsFalse(m.ValueEquals(m1));
            Assert.IsFalse(m.ValueAndErrorEquals(m1));
            Assert.IsTrue(m.ValueLess(m1));
            Assert.IsFalse(m1.ValueLess(m));

            var err1 = pool.MakeError(1, c);
            m = MFactory.NewMean(DataType.Time, "02.02.2000 15:30:00", err1);
            Assert.IsNotNull(m.Error);
            Assert.IsNotNull(m.TotalError);
            Assert.AreEqual(1, m.Error.Number);
            Assert.AreEqual("Warning", m.Error.Text);
            Assert.AreEqual(MomErrType.Source, m.Error.ErrType);
            Assert.AreEqual(false, m.Boolean);
            Assert.AreEqual(0, m.Integer);
            Assert.AreEqual(0.0, m.Real);
            Assert.AreEqual(d, m.Date);
            Assert.AreEqual(1, m.Count);
            Assert.AreEqual(DataType.Time, m.DataType);
            Assert.IsFalse(m1.ValueEquals(m));
            Assert.IsFalse(m1.ValueAndErrorEquals(m));
            Assert.IsFalse(m.ValueEquals(m1));
            Assert.IsFalse(m.ValueAndErrorEquals(m1));
        }
    }
}
