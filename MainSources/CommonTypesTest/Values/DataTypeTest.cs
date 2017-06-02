using CommonTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonTypesTest
{
    [TestClass]
    public class DataTypeTest
    {
        [TestMethod]
        public void Compare()
        {
            Assert.IsTrue(DataType.Boolean.LessOrEquals(DataType.Boolean));
            Assert.IsTrue(DataType.Real.LessOrEquals(DataType.Real));
            Assert.IsTrue(DataType.Weighted.LessOrEquals(DataType.Weighted));
            Assert.IsTrue(DataType.Segments.LessOrEquals(DataType.Segments));
            Assert.IsFalse(DataType.String.LessOrEquals(DataType.Real));
            Assert.IsTrue(DataType.Real.LessOrEquals(DataType.String));
            Assert.IsTrue(DataType.Integer.LessOrEquals(DataType.String));
            Assert.IsFalse(DataType.Real.LessOrEquals(DataType.Time));
            Assert.IsFalse(DataType.Time.LessOrEquals(DataType.Integer));
            Assert.IsFalse(DataType.Real.LessOrEquals(DataType.Segments));
            Assert.IsFalse(DataType.Segments.LessOrEquals(DataType.String));
            Assert.IsTrue(DataType.Weighted.IsReal());
            Assert.IsTrue(DataType.Weighted.LessOrEquals(DataType.String));
            Assert.IsFalse(DataType.Weighted.LessOrEquals(DataType.Integer));
            Assert.IsFalse(DataType.Real.LessOrEquals(DataType.Weighted));
            Assert.IsTrue(DataType.Value.LessOrEquals(DataType.Integer));
            Assert.IsTrue(DataType.Value.LessOrEquals(DataType.String));
            Assert.IsTrue(DataType.Value.LessOrEquals(DataType.Weighted));
            Assert.IsTrue(DataType.Real.LessOrEquals(DataType.Variant));
            Assert.IsFalse(DataType.Value.LessOrEquals(DataType.Segments));
            Assert.IsFalse(DataType.Integer.LessOrEquals(DataType.Error));

            Assert.AreEqual(DataType.Integer, DataType.Integer.Add(DataType.Integer));
            Assert.AreEqual(DataType.String, DataType.Integer.Add(DataType.String));
            Assert.AreEqual(DataType.String, DataType.Integer.Add(DataType.Time));
            Assert.AreEqual(DataType.Real, DataType.Real.Add(DataType.Weighted));
            Assert.AreEqual(DataType.Real, DataType.Weighted.Add(DataType.Real));
            Assert.AreEqual(DataType.Error, DataType.Integer.Add(DataType.Segments));
            Assert.AreEqual(DataType.Error, DataType.Segments.Add(DataType.String));
            Assert.AreEqual(DataType.Error, DataType.Value.Add(DataType.Segments));
            Assert.AreEqual(DataType.Segments, DataType.Segments.Add(DataType.Segments));
        }

        [TestMethod]
        public void IsOfType()
        {
            Assert.IsTrue("1".IsOfType(DataType.Boolean));
            Assert.IsTrue("0".IsOfType(DataType.Real));
            Assert.IsTrue("333".IsOfType(DataType.Integer));
            Assert.IsTrue("-2134".IsOfType(DataType.Integer));
            Assert.IsTrue("14134".IsOfType(DataType.Real));
            Assert.IsTrue("1.233".IsOfType(DataType.Real));
            Assert.IsTrue("-1,233".IsOfType(DataType.Real));
            Assert.IsTrue("0.233e-345".IsOfType(DataType.Real));
            Assert.IsTrue("-0,233e-35".IsOfType(DataType.Real));
            Assert.IsTrue("1".IsOfType(DataType.String));
            Assert.IsTrue("saegawersfgh".IsOfType(DataType.String));
            Assert.IsTrue("2022.10.10".IsOfType(DataType.Time));
            Assert.IsTrue("10.10.2020 11:11:11".IsOfType(DataType.Time));

            Assert.IsFalse("45".IsOfType(DataType.Boolean));
            Assert.IsFalse("4wsgawe5".IsOfType(DataType.Boolean));
            Assert.IsFalse("4we5".IsOfType(DataType.Integer));
            Assert.IsFalse("45.4".IsOfType(DataType.Integer));
            Assert.IsFalse("wfgwarg".IsOfType(DataType.Real));
            Assert.IsFalse("10.10.2020 11:11:11".IsOfType(DataType.Real));
        }
    }
}
