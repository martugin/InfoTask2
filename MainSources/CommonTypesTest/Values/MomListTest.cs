using System;
using BaseLibrary;
using CommonTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonTypesTest
{
    [TestClass]
    public class MomListTest
    {
        //Создание пула ошибок
        private static MomErrPool MakeErrPool()
        {
            var ef = new MomErrFactory("ErrSource", MomErrType.Source);
            ef.AddGoodDescr(0);
            ef.AddDescr(1, "Warning");
            ef.AddDescr(2, "Error");
            return new MomErrPool(ef);
        }

        //Время по заданным относительным минутам и секундам
        private DateTime RTime(int minutes, int seconds = 0)
        {
            return new DateTime(2016, 09, 14, 0, 0, 0).AddMinutes(minutes).AddSeconds(seconds);
        }
        
        [TestMethod]
        public void MomListBool()
        {
            var pool = MakeErrPool();
            var c = new ContextTest("Context");
            
            var list = new BoolMomList();
            Assert.AreEqual(0, list.Count);
            Assert.AreEqual(DataType.Boolean, list.DataType);
            Assert.AreEqual(0, list.CurNum);
            Assert.AreEqual(Static.MaxDate, list.NextTime);

            var err = pool.MakeError(2, c);
            list.AddMom(RTime(0), false);
            list.AddMom(RTime(1), false, err);
            list.AddMom(RTime(2), true, err);
            list.AddMom(RTime(3), true);
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(DataType.Boolean, list.DataType);
            list.CurNum = 0;
            Assert.AreEqual(0, list.CurNum);
            Assert.AreEqual(RTime(1), list.NextTime);
            Assert.AreEqual(false, list.Boolean);
            Assert.AreEqual(0, list.Integer);
            Assert.IsNull(list.Error);
            Assert.AreEqual(RTime(0), list.Time);
            list.CurNum = 1;
            Assert.AreEqual(1, list.CurNum);
            Assert.AreEqual(RTime(2), list.NextTime);
            Assert.AreEqual(false, list.Boolean);
            Assert.AreEqual(0, list.Real);
            Assert.IsNotNull(list.Error);
            Assert.AreEqual(2, list.Error.Number);
            Assert.AreEqual("Error", list.Error.Text);
            Assert.AreEqual(RTime(1), list.Time);
            list.CurNum = 3;
            Assert.AreEqual(3, list.CurNum);
            Assert.AreEqual(Static.MaxDate, list.NextTime);
            Assert.AreEqual(true, list.Boolean);
            Assert.AreEqual("1", list.String);
            Assert.IsNull(list.Error);
            Assert.AreEqual(RTime(3), list.Time);
            list.CurNum = 4;
            Assert.AreEqual(4, list.CurNum);
            Assert.AreEqual(Static.MaxDate, list.NextTime);
            list.CurNum = 2;
            Assert.AreEqual(2, list.CurNum);
            Assert.AreEqual(RTime(3), list.NextTime);
            Assert.AreEqual(true, list.Boolean);
            Assert.AreEqual(1, list.Integer);
            Assert.IsNotNull(list.Error);
            Assert.AreEqual(2, list.Error.Number);
            Assert.AreEqual("Error", list.Error.Text);
            Assert.AreEqual(RTime(2), list.Time);

            Assert.AreEqual(RTime(0), list.TimeI(0));
            Assert.AreEqual(RTime(1), list.TimeI(1));
            Assert.AreEqual(RTime(2), list.TimeI(2));
            Assert.AreEqual(RTime(3), list.TimeI(3));
            Assert.AreEqual(false, list.BooleanI(0));
            Assert.AreEqual(false, list.BooleanI(1));
            Assert.AreEqual(true, list.BooleanI(2));
            Assert.AreEqual(true, list.BooleanI(3));
            Assert.AreEqual(0, list.IntegerI(0));
            Assert.AreEqual(0, list.IntegerI(1));
            Assert.AreEqual(1, list.IntegerI(2));
            Assert.AreEqual(1, list.IntegerI(3));
            Assert.IsNull(list.ErrorI(0));
            Assert.IsNotNull(list.ErrorI(1));
            Assert.IsNotNull(list.ErrorI(2));
            Assert.IsNull(list.ErrorI(3));
            Assert.AreEqual(2, list.ErrorI(1).Number);
            Assert.AreEqual("Error", list.ErrorI(1).Text);
            Assert.AreEqual(ErrQuality.Error, list.ErrorI(1).Quality);
            Assert.AreEqual(2, list.ErrorI(2).Number);
            Assert.AreEqual("Error", list.ErrorI(2).Text);
            Assert.AreEqual(ErrQuality.Error, list.ErrorI(2).Quality);

            list.AddMom(RTime(2, 30), true);
            list.AddMom(RTime(1, 30), false);
            list.AddMom(RTime(0, 30), false);
            Assert.AreEqual(7, list.Count);
            Assert.AreEqual(RTime(0), list.TimeI(0));
            Assert.AreEqual(RTime(0, 30), list.TimeI(1));
            Assert.AreEqual(RTime(1), list.TimeI(2));
            Assert.AreEqual(RTime(1, 30), list.TimeI(3));
            Assert.AreEqual(RTime(2), list.TimeI(4));
            Assert.AreEqual(RTime(2, 30), list.TimeI(5));
            Assert.AreEqual(RTime(3), list.TimeI(6));
            Assert.AreEqual(false, list.BooleanI(0));
            Assert.AreEqual(false, list.BooleanI(1));
            Assert.AreEqual(false, list.BooleanI(2));
            Assert.AreEqual(false, list.BooleanI(3));
            Assert.AreEqual(true, list.BooleanI(4));
            Assert.AreEqual(true, list.BooleanI(5));
            Assert.AreEqual(true, list.BooleanI(6));
            Assert.IsNull(list.ErrorI(0));
            Assert.IsNull(list.ErrorI(1));
            Assert.IsNotNull(list.ErrorI(2));
            Assert.IsNull(list.ErrorI(3));
            Assert.IsNotNull(list.ErrorI(4));
            Assert.IsNull(list.ErrorI(5));
            Assert.IsNull(list.ErrorI(6));

            Assert.IsNotNull(list.TotalError);
            Assert.AreEqual(2, list.TotalError.Number);
            Assert.AreEqual("Error", list.TotalError.Text);
            Assert.AreEqual(ErrQuality.Error, list.TotalError.Quality);
            var m = list.LastMom;
            Assert.AreEqual(RTime(3), m.Time);
            Assert.AreEqual(true, m.Boolean);
            Assert.IsNull(m.Error);

            m = list.ToMeanI(0);
            Assert.AreEqual(false, m.Boolean);
            Assert.AreEqual(Static.MinDate, m.Time);
            Assert.IsNull(m.Error);
            m = list.ToMeanI(1, err);
            Assert.AreEqual(false, m.Boolean);
            Assert.AreEqual(Static.MinDate, m.Time);
            Assert.IsNotNull(m.Error);
            Assert.AreEqual(2, m.Error.Number);
            m = list.ToMomI(2);
            Assert.AreEqual(false, m.Boolean);
            Assert.AreEqual(RTime(1), m.Time);
            Assert.IsNotNull(m.Error);
            Assert.AreEqual(2, m.Error.Number);
            m = list.ToMomI(3, err);
            Assert.AreEqual(false, m.Boolean);
            Assert.AreEqual(RTime(1, 30), m.Time);
            Assert.IsNotNull(m.Error);
            Assert.AreEqual(2, m.Error.Number);
            m = list.ToMomI(5, RTime(5));
            Assert.AreEqual(true, m.Boolean);
            Assert.AreEqual(RTime(5), m.Time);
            Assert.IsNull(m.Error);
            m = list.ToMomI(6, RTime(6), err);
            Assert.AreEqual(true, m.Boolean);
            Assert.AreEqual(RTime(6), m.Time);
            Assert.IsNotNull(m.Error);
            Assert.AreEqual(2, m.Error.Number);

            list.CurNum = 3;
            Assert.AreEqual(RTime(2), list.NextTime);
            Assert.AreEqual(RTime(1, 30), list.Time);
            Assert.AreEqual(false, list.Boolean);
            Assert.AreEqual(0, list.Integer);
            Assert.IsNull(list.Error);
            m = list.ToMean();
            Assert.AreEqual(Static.MinDate, m.Time);
            Assert.AreEqual(false, m.Boolean);
            Assert.AreEqual(0, m.Integer);
            Assert.IsNull(m.Error);
            m = list.ToMean(err);
            Assert.AreEqual(Static.MinDate, m.Time);
            Assert.AreEqual(false, m.Boolean);
            Assert.AreEqual(0, m.Integer);
            Assert.IsNotNull(m.Error);
            Assert.AreEqual(2, m.Error.Number);
            m = list.ToMom();
            Assert.AreEqual(RTime(1, 30), m.Time);
            Assert.AreEqual(false, m.Boolean);
            Assert.AreEqual(0, m.Integer);
            Assert.IsNull(m.Error);
            m = list.ToMom(err);
            Assert.AreEqual(RTime(1, 30), m.Time);
            Assert.AreEqual(false, m.Boolean);
            Assert.AreEqual(0, m.Integer);
            Assert.IsNotNull(m.Error);
            Assert.AreEqual(2, m.Error.Number);
            m = list.ToMom(RTime(4, 30));
            Assert.AreEqual(RTime(4, 30), m.Time);
            Assert.AreEqual(false, m.Boolean);
            Assert.AreEqual(0, m.Integer);
            Assert.IsNull(m.Error);
            m = list.ToMom(RTime(4, 30), err);
            Assert.AreEqual(RTime(4, 30), m.Time);
            Assert.AreEqual(false, m.Boolean);
            Assert.AreEqual(0, m.Integer);
            Assert.IsNotNull(m.Error);
            Assert.AreEqual(2, m.Error.Number);

            list.Clear();
            Assert.AreEqual(0, list.Count);
            Assert.IsNull(list.LastMom);
            Assert.IsNull(list.TotalError);
        }

        [TestMethod]
        public void MomListInt()
        {
            var pool = MakeErrPool();
            var c = new ContextTest("Context");

            var list = new IntMomList();
            Assert.AreEqual(0, list.Count);
            Assert.AreEqual(DataType.Integer, list.DataType);
            Assert.AreEqual(0, list.CurNum);
            Assert.AreEqual(Static.MaxDate, list.NextTime);

            var err1 = pool.MakeError(1, c);
            var err2 = pool.MakeError(2, c);
            list.AddMom(RTime(0), 10);
            list.AddMom(RTime(0, 10), 11, err1);
            list.AddMom(RTime(0, 20), 12, err2);
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(RTime(0), list.TimeI(0));
            Assert.AreEqual(RTime(0, 10), list.TimeI(1));
            Assert.AreEqual(RTime(0, 20), list.TimeI(2));
            Assert.AreEqual(10, list.IntegerI(0));
            Assert.AreEqual(11, list.IntegerI(1));
            Assert.AreEqual(12, list.IntegerI(2));
            Assert.AreEqual(10.0, list.RealI(0));
            Assert.AreEqual("11", list.StringI(1));
            Assert.AreEqual(true, list.BooleanI(2));
            Assert.IsNull(list.ErrorI(0));
            Assert.IsNotNull(list.ErrorI(1));
            Assert.AreEqual(1, list.ErrorI(1).Number);
            Assert.AreEqual("Warning", list.ErrorI(1).Text);
            Assert.IsNotNull(list.ErrorI(2));
            Assert.AreEqual(2, list.ErrorI(2).Number);
            Assert.AreEqual("Error", list.ErrorI(2).Text);

            list.CurNum = 1;
            Assert.AreEqual(11, list.Integer);
            Assert.AreEqual(11.0, list.Real);
            Assert.AreEqual(1, list.Error.Number);
            Assert.AreEqual(RTime(0, 10), list.Time);

            var m = list.ToMom(RTime(1), err2);
            Assert.AreEqual(11, m.Integer);
            Assert.AreEqual(11.0, m.Real);
            Assert.AreEqual(1, m.Error.Number);
            Assert.AreEqual(RTime(1), m.Time);

            var me = new EditMom(DataType.Integer);
            me.Time = RTime(0, 5);
            me.Integer = 20;
            list.AddMom(me);
            me.Time = RTime(0, 15);
            me.Integer = 21;
            list.AddMom(me, err1);
            me.Time = RTime(0, 25);
            me.Integer = 22;
            list.AddMom(me, err2);
            Assert.AreEqual(6, list.Count);
            Assert.AreEqual(RTime(0), list.TimeI(0));
            Assert.AreEqual(RTime(0, 5), list.TimeI(1));
            Assert.AreEqual(RTime(0, 10), list.TimeI(2));
            Assert.AreEqual(RTime(0, 15), list.TimeI(3));
            Assert.AreEqual(RTime(0, 20), list.TimeI(4));
            Assert.AreEqual(RTime(0, 25), list.TimeI(5));
            Assert.AreEqual(10, list.IntegerI(0));
            Assert.AreEqual(20, list.IntegerI(1));
            Assert.AreEqual(11, list.IntegerI(2));
            Assert.AreEqual(21, list.IntegerI(3));
            Assert.AreEqual(12, list.IntegerI(4));
            Assert.AreEqual(22, list.IntegerI(5));
            Assert.IsNull(list.ErrorI(0));
            Assert.IsNull(list.ErrorI(1));
            Assert.AreEqual(1, list.ErrorI(2).Number);
            Assert.AreEqual(1, list.ErrorI(3).Number);
            Assert.AreEqual(2, list.ErrorI(4).Number);
            Assert.AreEqual(2, list.ErrorI(5).Number);
        }

        [TestMethod]
        public void MomListReal()
        {
            var pool = MakeErrPool();
            var c = new ContextTest("Context");

            var list = MFactory.NewList(DataType.Real);
            Assert.AreEqual(0, list.Count);
            Assert.AreEqual(DataType.Real, list.DataType);
            
            var err1 = pool.MakeError(1, c);
            var err2 = pool.MakeError(2, c);
            list.AddMom(RTime(0), 1.0);
            list.AddMom(RTime(0, 10), 1.1, err1);
            list.AddMom(RTime(0, 20), 1.2, err2);
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(RTime(0), list.TimeI(0));
            Assert.AreEqual(RTime(0, 10), list.TimeI(1));
            Assert.AreEqual(RTime(0, 20), list.TimeI(2));
            Assert.AreEqual(1.0, list.RealI(0));
            Assert.AreEqual(1.1, list.RealI(1));
            Assert.AreEqual(1.2, list.RealI(2));
            Assert.IsNull(list.ErrorI(0));
            Assert.IsNotNull(list.ErrorI(1));
            Assert.AreEqual(1, list.ErrorI(1).Number);
            Assert.AreEqual("Warning", list.ErrorI(1).Text);
            Assert.IsNotNull(list.ErrorI(2));
            Assert.AreEqual(2, list.ErrorI(2).Number);
            Assert.AreEqual("Error", list.ErrorI(2).Text);

            list.CurNum = 1;
            Assert.AreEqual(1.1, list.Real);
            Assert.AreEqual(1, list.Error.Number);
            Assert.AreEqual(RTime(0, 10), list.Time);

            var m = list.ToMom(RTime(1), err2);
            Assert.AreEqual(1.1, m.Real);
            Assert.AreEqual(1, m.Error.Number);
            Assert.AreEqual(RTime(1), m.Time);

            var me = new EditMom(DataType.Real);
            me.Time = RTime(0, 5);
            me.Real = 2;
            list.AddMom(me);
            me.Time = RTime(0, 15);
            me.Real = 2.1;
            list.AddMom(me, err1);
            me.Time = RTime(0, 25);
            me.Real = 2.2;
            list.AddMom(me, err2);
            Assert.AreEqual(6, list.Count);
            Assert.AreEqual(RTime(0), list.TimeI(0));
            Assert.AreEqual(RTime(0, 5), list.TimeI(1));
            Assert.AreEqual(RTime(0, 10), list.TimeI(2));
            Assert.AreEqual(RTime(0, 15), list.TimeI(3));
            Assert.AreEqual(RTime(0, 20), list.TimeI(4));
            Assert.AreEqual(RTime(0, 25), list.TimeI(5));
            Assert.AreEqual(1.0, list.RealI(0));
            Assert.AreEqual(2.0, list.RealI(1));
            Assert.AreEqual(1.1, list.RealI(2));
            Assert.AreEqual(2.1, list.RealI(3));
            Assert.AreEqual(1.2, list.RealI(4));
            Assert.AreEqual(2.2, list.RealI(5));
            Assert.IsNull(list.ErrorI(0));
            Assert.IsNull(list.ErrorI(1));
            Assert.AreEqual(1, list.ErrorI(2).Number);
            Assert.AreEqual(1, list.ErrorI(3).Number);
            Assert.AreEqual(2, list.ErrorI(4).Number);
            Assert.AreEqual(2, list.ErrorI(5).Number);
        }

        [TestMethod]
        public void MomListString()
        {
            var pool = MakeErrPool();
            var c = new ContextTest("Context");

            var list = MFactory.NewList(DataType.String);
            Assert.AreEqual(0, list.Count);
            Assert.AreEqual(DataType.String, list.DataType);

            var err1 = pool.MakeError(1, c);
            var err2 = pool.MakeError(2, c);
            list.AddMom(RTime(0), "s0");
            list.AddMom(RTime(0, 10), "s1", err1);
            list.AddMom(RTime(0, 20), "s2", err2);
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(RTime(0), list.TimeI(0));
            Assert.AreEqual(RTime(0, 10), list.TimeI(1));
            Assert.AreEqual(RTime(0, 20), list.TimeI(2));
            Assert.AreEqual("s0", list.StringI(0));
            Assert.AreEqual("s1", list.StringI(1));
            Assert.AreEqual("s2", list.StringI(2));
            Assert.IsNull(list.ErrorI(0));
            Assert.AreEqual(1, list.ErrorI(1).Number);
            Assert.AreEqual("Error", list.ErrorI(2).Text);

            list.CurNum = 1;
            Assert.AreEqual("s1", list.String);
            Assert.AreEqual(1, list.Error.Number);
            Assert.AreEqual(RTime(0, 10), list.Time);

            var m = list.ToMom(RTime(1), err2);
            Assert.AreEqual("s1", m.String);
            Assert.AreEqual(1, m.Error.Number);
            Assert.AreEqual(RTime(1), m.Time);

            var me = new EditMom(DataType.Real);
            me.Time = RTime(0, 5);
            me.Real = 2;
            list.AddMom(me);
            me.Time = RTime(0, 15);
            me.Real = 2.1;
            list.AddMom(me, err1);
            me.Time = RTime(0, 25);
            me.Real = 2.2;
            list.AddMom(me, err2);
            Assert.AreEqual(6, list.Count);
            Assert.AreEqual(RTime(0), list.TimeI(0));
            Assert.AreEqual(RTime(0, 5), list.TimeI(1));
            Assert.AreEqual(RTime(0, 10), list.TimeI(2));
            Assert.AreEqual(RTime(0, 15), list.TimeI(3));
            Assert.AreEqual(RTime(0, 20), list.TimeI(4));
            Assert.AreEqual(RTime(0, 25), list.TimeI(5));
            Assert.AreEqual("s0", list.StringI(0));
            Assert.AreEqual("2", list.StringI(1));
            Assert.AreEqual("s1", list.StringI(2));
            Assert.AreEqual("2,1", list.StringI(3));
            Assert.AreEqual("s2", list.StringI(4));
            Assert.AreEqual("2,2", list.StringI(5));
            Assert.IsNull(list.ErrorI(0));
            Assert.IsNull(list.ErrorI(1));
            Assert.AreEqual(1, list.ErrorI(2).Number);
            Assert.AreEqual(1, list.ErrorI(3).Number);
            Assert.AreEqual(2, list.ErrorI(4).Number);
            Assert.AreEqual(2, list.ErrorI(5).Number);
        }

        [TestMethod]
        public void MomListTime()
        {
            var pool = MakeErrPool();
            var c = new ContextTest("Context");

            var list = MFactory.NewList(DataType.Time);
            Assert.AreEqual(0, list.Count);
            Assert.AreEqual(DataType.Time, list.DataType);

            var err1 = pool.MakeError(1, c);
            var err2 = pool.MakeError(2, c);
            list.AddMom(RTime(0), RTime(0));
            list.AddMom(RTime(0, 10), RTime(1), err1);
            list.AddMom(RTime(0, 20), RTime(2), err2);
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(RTime(0), list.TimeI(0));
            Assert.AreEqual(RTime(0, 10), list.TimeI(1));
            Assert.AreEqual(RTime(0, 20), list.TimeI(2));
            Assert.AreEqual(RTime(0), list.DateI(0));
            Assert.AreEqual(RTime(1), list.DateI(1));
            Assert.AreEqual(RTime(2), list.DateI(2));
            Assert.IsNull(list.ErrorI(0));
            Assert.IsNotNull(list.ErrorI(1));
            Assert.AreEqual(1, list.ErrorI(1).Number);
            Assert.AreEqual("Warning", list.ErrorI(1).Text);
            Assert.IsNotNull(list.ErrorI(2));
            Assert.AreEqual(2, list.ErrorI(2).Number);
            Assert.AreEqual("Error", list.ErrorI(2).Text);

            list.CurNum = 1;
            Assert.AreEqual(RTime(1), list.Date);
            Assert.AreEqual(1, list.Error.Number);
            Assert.AreEqual(RTime(0, 10), list.Time);

            var m = list.ToMom(RTime(1), err2);
            Assert.AreEqual(RTime(1), m.Date);
            Assert.AreEqual(1, m.Error.Number);
            Assert.AreEqual(RTime(1), m.Time);

            var me = new EditMom(DataType.Time);
            me.Time = RTime(0, 5);
            me.Date = RTime(0, 30);
            list.AddMom(me);
            me.Time = RTime(0, 15);
            me.Date = RTime(1, 30);
            list.AddMom(me, err1);
            me.Time = RTime(0, 25);
            me.Date = RTime(2, 30);
            list.AddMom(me, err2);
            Assert.AreEqual(6, list.Count);
            Assert.AreEqual(RTime(0), list.TimeI(0));
            Assert.AreEqual(RTime(0, 5), list.TimeI(1));
            Assert.AreEqual(RTime(0, 10), list.TimeI(2));
            Assert.AreEqual(RTime(0, 15), list.TimeI(3));
            Assert.AreEqual(RTime(0, 20), list.TimeI(4));
            Assert.AreEqual(RTime(0, 25), list.TimeI(5));
            Assert.AreEqual(RTime(0), list.DateI(0));
            Assert.AreEqual(RTime(0, 30), list.DateI(1));
            Assert.AreEqual(RTime(1), list.DateI(2));
            Assert.AreEqual(RTime(1, 30), list.DateI(3));
            Assert.AreEqual(RTime(2), list.DateI(4));
            Assert.AreEqual(RTime(2, 30), list.DateI(5));
            Assert.IsNull(list.ErrorI(0));
            Assert.IsNull(list.ErrorI(1));
            Assert.AreEqual(1, list.ErrorI(2).Number);
            Assert.AreEqual(1, list.ErrorI(3).Number);
            Assert.AreEqual(2, list.ErrorI(4).Number);
            Assert.AreEqual(2, list.ErrorI(5).Number);
        }
    }
}