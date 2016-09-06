using System;
using BaseLibrary;
using CommonTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Different = BaseLibrary.Different;

namespace CommonTypesTest
{
    [TestClass]
    public class MeanTest
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
        public void MeanBool()
        {
            var c = new ContextTest("Context");
            var pool = MakeErrPool();
            var err = pool.MakeError(2, c);

            var m = MFactory.NewMean(true);
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

            var m1 = m.CloneMom(Different.MinDate);
            Assert.AreEqual(Different.MinDate, m1.Time);
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

            m = MFactory.NewMean(false, err);
            Assert.IsNotNull(m.Error);
            Assert.IsNotNull(m.TotalError);
            Assert.AreEqual(2, m.Error.Number);
            Assert.AreEqual("Error", m.Error.Text);
            Assert.AreEqual(ErrMomType.Source, m.Error.ErrType);
            Assert.AreEqual(2, m.TotalError.Number);
            Assert.AreEqual("Error", m.TotalError.Text);
            Assert.AreEqual(ErrMomType.Source, m.TotalError.ErrType);
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

            var err1 = pool.MakeError(1, c);
            m = MFactory.NewMean(DataType.Boolean, 1, err1);
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

            var m1 = m.CloneMom(Different.MinDate);
            Assert.AreEqual(Different.MinDate, m1.Time);
            Assert.IsNull(m1.Error);
            Assert.IsNull(m1.TotalError);
            Assert.AreEqual(true, m1.Boolean);
            Assert.AreEqual(24, m1.Integer);
            Assert.AreEqual(24.0, m1.Real);
            Assert.AreEqual("24", m1.String);
            Assert.AreEqual(1, m1.Count);
            Assert.AreEqual(DataType.Integer, m1.DataType);
            Assert.IsTrue(m1.ValueEquals(m));
            Assert.IsTrue(m1.ValueAndErrorEquals(m));
            Assert.IsTrue(m.ValueEquals(m1));
            Assert.IsTrue(m.ValueAndErrorEquals(m1));

            m = MFactory.NewMean(-32, err);
            Assert.IsNotNull(m.Error);
            Assert.IsNotNull(m.TotalError);
            Assert.AreEqual(2, m.Error.Number);
            Assert.AreEqual("Error", m.Error.Text);
            Assert.AreEqual(ErrMomType.Source, m.Error.ErrType);
            Assert.AreEqual(2, m.TotalError.Number);
            Assert.AreEqual("Error", m.TotalError.Text);
            Assert.AreEqual(ErrMomType.Source, m.TotalError.ErrType);
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

            var m1 = m.CloneMom(Different.MinDate, err);
            Assert.AreEqual(Different.MinDate, m1.Time);
            Assert.IsNotNull(m1.Error);
            Assert.IsNotNull(m1.TotalError);
            Assert.AreEqual(2, m1.Error.Number);
            Assert.AreEqual("Error", m1.Error.Text);
            Assert.AreEqual(ErrMomType.Source, m1.Error.ErrType);
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
            Assert.AreEqual(ErrMomType.Source, m.Error.ErrType);
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
            Assert.AreEqual(ErrMomType.Source, m.Error.ErrType);
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

            var m1 = m.CloneMom(Different.MaxDate, err);
            Assert.AreEqual(Different.MaxDate, m1.Time);
            Assert.IsNotNull(m1.Error);
            Assert.IsNotNull(m1.TotalError);
            Assert.AreEqual(2, m1.Error.Number);
            Assert.AreEqual("Error", m1.Error.Text);
            Assert.AreEqual(ErrMomType.Source, m1.Error.ErrType);
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
            Assert.AreEqual(ErrMomType.Source, m.Error.ErrType);
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
            Assert.AreEqual(ErrMomType.Source, m.Error.ErrType);
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
            Assert.IsNull(cv.TotalError);

            var d = new DateTime(2000, 2, 2, 15, 30, 0);
            var m1 = m.CloneMom(d, err);
            Assert.AreEqual(d, m1.Time);
            Assert.IsNotNull(m1.Error);
            Assert.IsNotNull(m1.TotalError);
            Assert.AreEqual(2, m1.Error.Number);
            Assert.AreEqual("Error", m1.Error.Text);
            Assert.AreEqual(ErrMomType.Source, m1.Error.ErrType);
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
            Assert.AreEqual(ErrMomType.Source, m.Error.ErrType);
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
            Assert.AreEqual(ErrMomType.Source, m.Error.ErrType);
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
