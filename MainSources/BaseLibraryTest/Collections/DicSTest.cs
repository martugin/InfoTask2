using System.Linq;
using BaseLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseLibraryTest
{
    [TestClass]
    public class DicSTest
    {
        [TestMethod]
        public void Simple()
        {
            var dic = new DicS<string>();
            Assert.AreEqual(0, dic.Count);
            dic.Add("a", "sa");
            dic.Add("B", "sB");
            Assert.IsTrue(dic.ContainsKey("a"));
            Assert.IsTrue(dic.ContainsKey("b"));
            Assert.AreEqual("sa", dic["a"]);
            Assert.AreEqual("sB", dic["B"]);
            Assert.AreEqual("sa", dic.Get("A"));
            Assert.AreEqual("sB", dic.Get("b", "h"));
            Assert.IsNull(dic["c"]);
            Assert.AreEqual("h", dic.Get("c", "h"));
            Assert.AreEqual(2, dic.Count);
            dic.Add("b", "gghsdauh");
            Assert.AreEqual(2, dic.Count);
            Assert.AreEqual("sB", dic["B"]);
            dic["b"] = "gg";
            Assert.AreEqual(2, dic.Count);
            Assert.AreEqual("gg", dic["B"]);
            dic.Add("c", "SC");
            Assert.IsTrue(dic.ContainsKey("C"));
            Assert.AreEqual(3, dic.Count);
            dic.Add("b", "ssb", true);
            Assert.IsTrue(dic.ContainsKey("b"));
            Assert.IsTrue(dic.ContainsKey("c"));
            Assert.AreEqual("ssb", dic["b"]);
            Assert.IsTrue(dic.Keys.Contains("B"));
            Assert.IsTrue(dic.Values.Contains("ssb"));
            dic.Remove("C");
            Assert.AreEqual(2, dic.Count);
            Assert.IsFalse(dic.ContainsKey("c"));
            Assert.IsNull(dic["c"]);
            Assert.AreEqual("def", dic.Get("C", "def"));
            Assert.IsFalse(dic.Remove("C"));
            Assert.IsFalse(dic.ContainsKey("c"));
            Assert.AreEqual("def", dic.Get("C", "def"));
            Assert.IsFalse(dic.ContainsKey(null));
            dic.Clear();
            Assert.AreEqual(0, dic.Count);
            Assert.IsNull(dic["a"]);
            Assert.IsFalse(dic.ContainsKey("c"));
            Assert.IsFalse(dic.ContainsKey("a"));
        }

        [TestMethod]
        public void Complex()
        {
            var dic = new DicS<int>(1000);
            dic.Clear();
            Assert.AreEqual(0, dic.Count);
            dic.Add("First", 1);
            dic.Add("Second", 2);
            Assert.AreEqual(2, dic.Count);
            Assert.AreEqual(1, dic["first"]);
            Assert.AreEqual(1, dic.Get("FIRST"));
            Assert.AreEqual(2, dic["Second"]);
            var dicAdd = new DicS<int>();
            dicAdd.Add("second", 22);
            dicAdd.Add("third", 33);
            dic.AddDic(dicAdd);
            Assert.AreEqual(3, dic.Count);
            Assert.IsTrue(dic.ContainsKey("THIRD"));
            Assert.IsTrue(dic.ContainsKey("Second"));
            Assert.AreEqual(56, dic.Values.Sum());
            dic.AddDic(dicAdd);
            Assert.AreEqual(3, dic.Count);
            Assert.AreEqual(1, dic["first"]);
            Assert.AreEqual(22, dic["second"]);
            Assert.AreEqual(33, dic["third"]);
            dic.Remove((k, v) => v == 33 || k.ToLower() == "second");
            Assert.AreEqual(1, dic.Count);
            Assert.IsTrue(dic.ContainsKey("First"));
            Assert.IsFalse(dic.ContainsKey("Second"));
        }
    }
}