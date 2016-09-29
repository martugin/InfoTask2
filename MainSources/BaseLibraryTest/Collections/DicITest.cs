using BaseLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseLibraryTest
{
    [TestClass]
    public class DicITest
    {
        [TestMethod]
        public void Simple()
        {
            var dic = new DicI<string>("def");
            dic.Add(2, "22");
            dic.Add(3, "33");
            Assert.IsTrue(dic.ContainsKey(2));
            Assert.IsTrue(dic.ContainsKey(3));
            Assert.IsFalse(dic.ContainsKey(4));
            Assert.IsTrue(dic.Keys.Contains(3));
            Assert.AreEqual("22", dic[2]);
            Assert.AreEqual("22", dic.Get(2));
            Assert.AreEqual("33", dic.Dic[3]);
            Assert.AreEqual("def", dic[4]);
            Assert.AreEqual(2, dic.Count);
            Assert.AreEqual(2, dic.Values.Count);
            Assert.AreEqual(2, dic.Keys.Count);
            dic.Add(2, "222");
            dic.Add(3, "333", true);
            dic.Add(4, "444");
            Assert.IsTrue(dic.ContainsKey(2));
            Assert.IsTrue(dic.ContainsKey(3));
            Assert.IsTrue(dic.ContainsKey(4));
            Assert.AreEqual("22", dic[2] );
            Assert.AreEqual("333", dic[3]);
            Assert.AreEqual("444", dic[4]);
            Assert.AreEqual("444", dic.Get(4, "hh"));
            Assert.AreEqual("def", dic[5]);
            Assert.AreEqual("hh", dic.Get(5, "hh"));
            Assert.IsTrue(dic.ContainsValue("22"));
            Assert.IsFalse(dic.ContainsValue("33"));
            Assert.AreEqual(3, dic.Count);
            dic[4] = "44";
            Assert.IsTrue(dic.ContainsKey(4));
            Assert.AreEqual("44", dic[4]);
            Assert.IsTrue(dic.Remove(3));
            Assert.IsFalse(dic.Remove(5));
            Assert.IsFalse(dic.ContainsKey(3));
            Assert.IsFalse(dic.ContainsKey(5));
            dic.Clear();
            Assert.AreEqual(0, dic.Count);
            Assert.IsFalse(dic.ContainsKey(2));
        }

        [TestMethod]
        public void Complex()
        {
            var dic = new DicI<int>();
            dic.Add(1, 1);
            Assert.IsTrue(dic.ContainsKey(1));
            Assert.AreEqual(0, dic[2]);
            Assert.AreEqual(1, dic[1]);
            Assert.AreEqual(1, dic.Count);
            var dicAdd = new DicI<int>();
            dicAdd.Add(1, 11);
            dicAdd.Add(2, 22);
            dicAdd.Add(3, 333);
            dic.AddDic(dicAdd, false);
            Assert.IsTrue(dic.ContainsKey(1));
            Assert.IsTrue(dic.ContainsKey(2));
            Assert.IsTrue(dic.ContainsKey(3));
            Assert.AreEqual(3, dic.Count);
            Assert.AreEqual(1, dic[1]);
            Assert.AreEqual(22, dic[2]);
            Assert.AreEqual(333, dic[3]);
            dicAdd[3] = 33;
            dic.AddDic(dicAdd);
            Assert.AreEqual(3, dic.Count);
            Assert.IsTrue(dic.ContainsKey(1));
            Assert.IsTrue(dic.ContainsKey(2));
            Assert.IsTrue(dic.ContainsKey(3));
            Assert.AreEqual(22, dic[2]);
            Assert.AreEqual(33, dic[3]);
            dic.Remove((k, v) => v > 30);
            Assert.IsTrue(dic.ContainsKey(1));
            Assert.IsTrue(dic.ContainsKey(2));
            Assert.IsFalse(dic.ContainsKey(3));
            Assert.AreEqual(2, dic.Count);
        }
    }
}
