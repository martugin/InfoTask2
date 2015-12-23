using System.Linq;
using BaseLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseLibraryTest
{
     [TestClass]
    public class SetSTest
    {
         [TestMethod]
         public void Simple()
         {
             var set = new SetS();
             Assert.AreEqual(0, set.Count);
             set.Clear();
             Assert.AreEqual(0, set.Count);
             Assert.AreEqual(0, set.Keys.Count);
             Assert.AreEqual(0, set.Values.Count);
             set.Add("a");
             set.Add("Bb");
             Assert.AreEqual(2, set.Count);
             Assert.IsTrue(set.Contains("a"));
             Assert.IsTrue(set.Contains("bB"));
             Assert.IsFalse(set.Contains("ff"));
             Assert.AreEqual("aBb", set.Values.Aggregate("", (current, v) => current + v));
             set.Add("BB", true);
             Assert.IsTrue(set.Contains("a"));
             Assert.IsTrue(set.Contains("bb"));
             Assert.IsFalse(set.Contains("ff"));
             Assert.AreEqual("aBB", set.Values.Aggregate("", (current, v) => current + v));
             set.Add("bb");
             set.Add("CCC");
             Assert.AreEqual("aBBCCC", set.Values.Aggregate("", (current, v) => current + v));
             Assert.IsTrue(set.Contains("a"));
             Assert.IsTrue(set.Contains("bb"));
             Assert.IsTrue(set.Contains("cCc"));
             Assert.AreEqual(3, set.Count);
             set.Remove("bb");
             Assert.IsFalse(set.Contains("bb"));
             Assert.IsTrue(set.Contains("ccc"));
             Assert.AreEqual(2, set.Count);
         }
    }
}