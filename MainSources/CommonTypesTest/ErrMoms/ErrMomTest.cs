﻿using System.Linq;
using BaseLibrary;
using CommonTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonTypesTest
{
    [TestClass]
    public class ErrMomTest
    {
        [TestMethod]
        public void Simple()
        {
            var ed = new ErrDescr(100, "sss", ErrQuality.Error, MomErrType.Calc);
            Assert.AreEqual(100, ed.Number);
            Assert.AreEqual("sss", ed.Text);
            Assert.AreEqual(ErrQuality.Error, ed.Quality);
            Assert.AreEqual(MomErrType.Calc, ed.ErrType);

            var cont = new ContextTest("SimpleContext");
            var em = new MomErr(ed, cont);
            Assert.IsNotNull(em.AddressLink);
            Assert.IsNotNull(em.ErrDescr);
            Assert.AreEqual("SimpleContext", em.Address);
            Assert.AreEqual(100, em.Number);
            Assert.AreEqual(MomErrType.Calc, em.ErrType);
            Assert.AreEqual(ErrQuality.Error, em.Quality);
            Assert.AreEqual("sss", em.Text);

            var ef = new MomErrFactory("ErrSource", MomErrType.Source) {UndefinedErrorText = "Ошибка"};
            Assert.AreEqual(MomErrType.Source, ef.MomErrType);
            Assert.AreEqual(MomErrType.Source, ef.MomErrType);
            Assert.AreEqual("Ошибка", ef.UndefinedErrorText);
            ef.AddGoodDescr(0);
            ef.AddDescr(1, "Ошибка1", ErrQuality.Warning);
            ef.AddDescr(2, "Ошибка2");
            ef.AddDescr(3, "Ошибка3");

            Assert.IsNull(ef.GetDescr(0));
            Assert.IsNotNull(ef.GetDescr(1));
            var desc = ef.GetDescr(1);
            Assert.AreEqual(1, desc.Number);
            Assert.AreEqual("Ошибка1", desc.Text);
            Assert.AreEqual(ErrQuality.Warning, desc.Quality);
            Assert.AreEqual(MomErrType.Source, desc.ErrType);
            Assert.IsNotNull(ef.GetDescr(2));
            desc = ef.GetDescr(2);
            Assert.AreEqual(2, desc.Number);
            Assert.AreEqual("Ошибка2", desc.Text);
            Assert.AreEqual(ErrQuality.Error, desc.Quality);
            Assert.AreEqual(MomErrType.Source, desc.ErrType);
            Assert.IsNotNull(ef.GetDescr(3));
            desc = ef.GetDescr(3);
            Assert.AreEqual(3, desc.Number);
            Assert.AreEqual("Ошибка3", desc.Text);
            Assert.AreEqual(ErrQuality.Error, desc.Quality);
            Assert.AreEqual(MomErrType.Source, desc.ErrType);
            desc = ef.GetDescr(4);
            Assert.AreEqual(4, desc.Number);
            Assert.AreEqual("Ошибка", desc.Text);
            Assert.AreEqual(ErrQuality.Error, desc.Quality);
            Assert.AreEqual(MomErrType.Source, desc.ErrType);
        }

        [TestMethod]
        public void Pool()
        {
            var ef = new MomErrFactory("ErrSource", MomErrType.Source);
            ef.AddGoodDescr(0);
            ef.AddDescr(1, "Ошибка1", ErrQuality.Warning);
            ef.AddDescr(2, "Ошибка2");
            ef.AddDescr(3, "Ошибка3");

            var c1 = new ContextTest("Context1");
            var c2 = new ContextTest("Context2");
            var pool = new MomErrPool(ef);
            Assert.AreEqual(0, pool.UsedErrorDescrs.Count());
            var em = pool.MakeError(1, c1);
            Assert.AreEqual("Context1", em.Address);
            Assert.AreEqual(1, em.Number);
            Assert.AreEqual(ErrQuality.Warning, em.Quality);
            Assert.AreEqual("Ошибка1", em.Text);
            em = pool.MakeError(1, c2);
            Assert.AreEqual("Context2", em.Address);
            Assert.AreEqual(1, em.Number);
            Assert.AreEqual(ErrQuality.Warning, em.Quality);
            Assert.AreEqual("Ошибка1", em.Text);
            em = pool.MakeError(1, c1);
            Assert.AreEqual("Context1", em.Address);
            Assert.AreEqual(1, em.Number);
            Assert.AreEqual(ErrQuality.Warning, em.Quality);
            Assert.AreEqual("Ошибка1", em.Text);
            Assert.IsNull(pool.MakeError(0, c2));
            em = pool.MakeError(2, c1);
            Assert.AreEqual("Context1", em.Address);
            Assert.AreEqual(2, em.Number);
            Assert.AreEqual(ErrQuality.Error, em.Quality);
            Assert.AreEqual("Ошибка2", em.Text);
            Assert.AreEqual(2, pool.UsedErrorDescrs.Count());
            pool.ClearErrors();
            em = pool.MakeError(2, c1);
            Assert.AreEqual("Context1", em.Address);
            Assert.AreEqual(2, em.Number);
            Assert.AreEqual(ErrQuality.Error, em.Quality);
            Assert.AreEqual("Ошибка2", em.Text);
            em = pool.MakeError(3, c2);
            Assert.AreEqual("Context2", em.Address);
            Assert.AreEqual(3, em.Number);
            Assert.AreEqual(ErrQuality.Error, em.Quality);
            Assert.AreEqual("Ошибка3", em.Text);
            Assert.AreEqual(2, pool.UsedErrorDescrs.Count());
        }
    }

    //------------------------------------------------------------------------------------------------
    //Вспомогалеьный класс с контекстом
    internal class ContextTest: IContextable
    {
        public ContextTest(string context)
        {
            Context = context;
        }
        public string Context { get; private set; }
    }
}