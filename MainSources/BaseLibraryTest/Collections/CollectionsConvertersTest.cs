using System.Collections.Generic;
using BaseLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseLibraryTest
{
    [TestClass]
    public class CollectionsConvertersTest
    {
        [TestMethod]
        public void Merge()
        {
            var dic = new Dictionary<string, string> {{"a", "aa"}, {"b", "bb"}};
            var dic2 = new Dictionary<string, string> { { "a", "aa" }, { "c", "cc" } };
            var res = dic.Merge(dic2);
            Assert.AreEqual(3, res.Count);
            Assert.IsTrue(res.ContainsKey("a"));
            Assert.IsTrue(res.ContainsKey("b"));
            Assert.IsTrue(res.ContainsKey("c"));
            Assert.AreEqual("aa", res["a"]);
            Assert.AreEqual("bb", res["b"]);
            Assert.AreEqual("cc", res["c"]);
            dic2.Clear();
            res = res.Merge(dic2);
            Assert.AreEqual(3, res.Count);
            Assert.IsTrue(res.ContainsKey("a"));
            Assert.IsTrue(res.ContainsKey("b"));
            Assert.IsTrue(res.ContainsKey("c"));
            Assert.AreEqual("aa", res["a"]);
            Assert.AreEqual("bb", res["b"]);
            Assert.AreEqual("cc", res["c"]);
        }

        [TestMethod]
        public void ColectionsFromString()
        {
            string s = "PropI=10;PropS=ddd;PropB=True;PropD=1.1";
            var dic = s.ToPropertyDicS();
            Assert.IsNotNull(dic);
            Assert.AreEqual(4, dic.Count);
            Assert.IsTrue(dic.ContainsKey("PropI"));
            Assert.IsTrue(dic.ContainsKey("PROPB"));
            Assert.IsFalse(dic.ContainsKey("propp"));
            Assert.AreEqual("10", dic.Get("PropI"));
            Assert.AreEqual("ddd", dic.Get("PropS", "a"));
            Assert.AreEqual("True", dic["PropB"]);
            Assert.AreEqual("1.1", dic["PropD"]);
            Assert.AreEqual(10, dic.GetInt("PropI"));
            Assert.AreEqual(0, dic.GetInt("PropD"));
            Assert.AreEqual(20, dic.GetInt("Prop", 20));
            Assert.IsTrue(dic.GetBool("PropB")); 

            Dictionary<string, string> dict = null;
            Assert.IsNull(dict.Get("ddd"));
            Assert.AreEqual("def", dict.Get("ddd", "def"));
            Assert.AreEqual(false, dict.GetBool("ddd"));
            Assert.AreEqual(30, dict.GetInt("ddd", 30));
            Assert.AreEqual(0, dict.GetInt("ddd"));
            dict = s.ToPropertyDictionary();
            Assert.IsNotNull(dict);
            Assert.AreEqual(4, dict.Count);
            Assert.IsTrue(dict.ContainsKey("PropI"));
            Assert.IsTrue(dict.ContainsKey("PropB"));
            Assert.IsFalse(dict.ContainsKey("PropP"));
            Assert.AreEqual("10", dict["PropI"]);
            Assert.AreEqual("ddd", dict["PropS"]);
            Assert.AreEqual("True", dict["PropB"]);
            Assert.AreEqual("1.1", dict["PropD"]);
            Assert.AreEqual(10, dict.GetInt("PropI"));
            Assert.AreEqual(0, dict.GetInt("PropD"));
            Assert.AreEqual(20, dict.GetInt("Prop", 20));
            Assert.IsTrue(dict.GetBool("PropB"));

            s = "PropI=10.PropS=ddd.PropB=True";
            dic = s.ToPropertyDicS(".");
            Assert.IsTrue(dic.ContainsKey("PropI"));
            Assert.IsTrue(dic.ContainsKey("PROPB"));
            Assert.IsFalse(dic.ContainsKey("propp"));
            Assert.AreEqual("10", dic.Get("PropI"));
            Assert.AreEqual("ddd", dic.Get("PropS", "a"));
            Assert.AreEqual("True", dic["PropB"]);

            s = "Prop1;Prop2";
            var list = s.ToPropertyList();
            Assert.AreEqual("Prop1", list[0]);
            Assert.AreEqual("Prop2", list[1]);
            Assert.AreEqual(2, list.Count);

            var set = s.ToPropertySetS();
            Assert.IsTrue(set.Contains("Prop1"));
            Assert.IsTrue(set.Contains("Prop2"));
            Assert.AreEqual(2, set.Count);

            var hset = s.ToPropertyHashSet();
            Assert.IsTrue(hset.Contains("Prop1"));
            Assert.IsTrue(hset.Contains("Prop2"));
            Assert.AreEqual(2, hset.Count);

            var sset = s.ToPropertySortedSet();
            Assert.IsTrue(sset.Contains("Prop1"));
            Assert.IsTrue(sset.Contains("Prop2"));
            Assert.AreEqual(2, sset.Count);
        }
        


        [TestMethod]
        public void StringFromCollections()
        {
            var dic = new DicS<string>();
            Assert.AreEqual("", dic.ToPropertyString());
            dic.Add("Prop1", "a");
            dic.Add("Prop2", "b");
            dic.Add("Prop3", "c");
            Assert.AreEqual("PROP1=a;PROP2=b;PROP3=c;", dic.ToPropertyString());
            var dict = new Dictionary<string, string>();
            Assert.AreEqual("", dict.ToPropertyString());
            dict.Add("Prop1", "a");
            dict.Add("Prop2", "b");
            dict.Add("Prop3", "c");
            Assert.AreEqual("Prop1=a;Prop2=b;Prop3=c;", dict.ToPropertyString());
            var list = new List<string>();
            Assert.AreEqual("", list.ToPropertyString());
            list.Add("Prop1");
            list.Add("Prop2");
            list.Add("Prop3");
            Assert.AreEqual("Prop1;Prop2;Prop3;", list.ToPropertyString());
            var set = new HashSet<string>();
            Assert.AreEqual("", set.ToPropertyString());
            set.Add("Prop1");
            Assert.AreEqual("Prop1;", set.ToPropertyString());
        }
    }
}