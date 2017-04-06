using CommonTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Provider;

namespace ProvidersTest
{
    [TestClass]
    public class KosmValuesTest
    {
        [TestMethod]
        public void ErrFactory()
        {
            var efactory = new KosmMomErrFactory();
            Assert.AreEqual("KosmotronikaRetroSource", efactory.ErrSourceCode);
            Assert.IsNull(efactory.GetDescr(0));
            var ed = efactory.GetDescr(2);
            Assert.IsNotNull(ed);
            Assert.AreEqual(ErrQuality.Warning, ed.Quality);
            Assert.AreEqual("НД 2 из 3-х", ed.Text);
            Assert.AreEqual(MomErrType.Source, ed.ErrType);

            ed = efactory.GetDescr(32);
            Assert.IsNotNull(ed);
            Assert.AreEqual(ErrQuality.Error, ed.Quality);
            Assert.AreEqual("Условное заведение", ed.Text);
            Assert.AreEqual(MomErrType.Source, ed.ErrType);

            ed = efactory.GetDescr(32768);
            Assert.IsNotNull(ed);
            Assert.AreEqual(ErrQuality.Error, ed.Quality);
            Assert.AreEqual("НД по КТС", ed.Text);

            ed = efactory.GetDescr(24);
            Assert.IsNotNull(ed);
            Assert.AreEqual(ErrQuality.Warning, ed.Quality);
            Assert.AreEqual("Опробуемое значение; Имитация", ed.Text);
            Assert.AreEqual(MomErrType.Source, ed.ErrType);

            ed = efactory.GetDescr(32770);
            Assert.IsNotNull(ed);
            Assert.AreEqual(ErrQuality.Error, ed.Quality);
            Assert.AreEqual("НД по КТС; НД 2 из 3-х", ed.Text);

            ed = efactory.GetDescr(1);
            Assert.IsNotNull(ed);
            Assert.AreEqual(ErrQuality.Warning, ed.Quality);
            Assert.AreEqual("", ed.Text);
            Assert.AreEqual(MomErrType.Source, ed.ErrType);
        }
    }
}