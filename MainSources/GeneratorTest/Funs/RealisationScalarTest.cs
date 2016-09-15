using System;
using CommonTypes;
using Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonTypesTest
{
    [TestClass]
    public class RealisationScalarTest
    {
        //Вычисление значения скалярной функции
        private IMean Calc(FunctionsGen funs, string funName, DataType dtype, params IMean[] pars)
        {
            var fun = (ScalarGenFun)(funs.CurFun = funs.Funs[funName]);
            return fun.Calculate(pars, dtype);
        }

        //Время по заданным относительным минутам и секундам
        private DateTime RTime(int minutes, int seconds = 0)
        {
            return new DateTime(2016, 09, 14, 0, 0, 0).AddMinutes(minutes).AddSeconds(seconds);
        }

        [TestMethod]
        public void Operations()
        {
            var funs = new FunctionsGen();
            IMean i1 = new MeanInt(20), i2 = new MeanInt(5), i3 = new MeanInt(0);
            IMean r1 = new MeanReal(2.4), r2 = new MeanReal(0.6), r3 = new MeanReal(120);
            IMean d1 = new MeanTime(RTime(60)), d2 = new MeanTime(RTime(70));
            IMean s1 = new MeanString("Abc"), s2 = new MeanString("sT");

            IMean m = Calc(funs, "Plus_ii", DataType.Integer, i1, i2);
            Assert.AreEqual(25, m.Integer);
            Assert.IsNull(m.Error);
            Assert.AreEqual(3.0, Calc(funs, "Plus_rr", DataType.Integer, r1, r2).Real);
            Assert.AreEqual(RTime(62), Calc(funs, "Plus_dr", DataType.Time, d1, r3).Date);
            Assert.AreEqual(RTime(62), Calc(funs, "Plus_rd", DataType.Time, r3, d1).Date);
            Assert.AreEqual("AbcsT", Calc(funs, "Plus_ss", DataType.String, s1, s2).String);

            Assert.AreEqual(-20, Calc(funs, "Minus_i", DataType.Integer, i1).Integer);
            Assert.AreEqual(-2.4, Calc(funs, "Minus_r", DataType.Real, r1).Real);
            Assert.AreEqual(15, Calc(funs, "Minus_ii", DataType.Integer, i1, i2).Integer);
            Assert.IsTrue(Math.Abs(1.8 - Calc(funs, "Minus_rr", DataType.Real, r1, r2).Real) < 0.001);
            Assert.AreEqual(RTime(58), Calc(funs, "Minus_dr", DataType.Time, d1, r3).Date);
            Assert.AreEqual(-600, Calc(funs, "Minus_dd", DataType.Real, d1, d2).Integer);

            Assert.AreEqual(4.0, Calc(funs, "Divide_rr", DataType.Real, i1, i2).Real);
            m = Calc(funs, "Divide_rr", DataType.Real, i1, i3);
            Assert.AreEqual(0, m.Real);
            Assert.IsNotNull(m.Error);
            Assert.AreEqual(501, m.Error.Number);
            Assert.AreEqual(ErrQuality.Error, m.Error.Quality);
            Assert.AreEqual("Деление на 0", m.Error.Text);

            m = Calc(funs, "Multiply_rr", DataType.Integer, i3, m);
            Assert.AreEqual(0, m.Real);
            Assert.IsNull(m.Error);
            Assert.AreEqual(100, Calc(funs, "Multiply_ii", DataType.Integer, i1, i2).Integer);
            Assert.AreEqual(12, Calc(funs, "Multiply_rr", DataType.Integer, r1, i2).Real);

            Assert.AreEqual(4, Calc(funs, "Div_ii", DataType.Integer, i1, i2).Integer);
            m = Calc(funs, "Div_ii", DataType.Integer, i1, i3);
            Assert.AreEqual(0, m.Integer);
            Assert.IsNotNull(m.Error);
            Assert.AreEqual(511, m.Error.Number);
            Assert.AreEqual(ErrQuality.Error, m.Error.Quality);
            Assert.AreEqual("Деление на 0", m.Error.Text);

            Assert.AreEqual(5, Calc(funs, "Mod_ii", DataType.Integer, i2, i1).Integer);
            m = Calc(funs, "Mod_ii", DataType.Integer, i1, i3);
            Assert.AreEqual(0, m.Integer);
            Assert.IsNotNull(m.Error);
            Assert.AreEqual(521, m.Error.Number);
            Assert.AreEqual(ErrQuality.Error, m.Error.Quality);

            IMean i4 = new MeanInt(-2), r4 = new MeanReal(-1.1);

            Assert.AreEqual(79.62624, Calc(funs, "Power_rr", DataType.Real, r1, i2).Real);
            Assert.AreEqual(0, Calc(funs, "Power_rr", DataType.Real, i3, i4).Real);
            Assert.AreEqual(-32, Calc(funs, "Power_rr", DataType.Real, i4, i2).Real);
            Assert.AreEqual(531, Calc(funs, "Power_rr", DataType.Real, i4, r4).Error.Number);
            Assert.AreEqual(532, Calc(funs, "Power_rr", DataType.Real, i3, i3).Error.Number);

            IMean s3 = new MeanString("A*c"), s4 = new MeanString("Ab?"), s5 = new MeanString("");
            Assert.IsTrue(Calc(funs, "Like_ss", DataType.Boolean, s1, s1).Boolean);
            Assert.IsTrue(Calc(funs, "Like_ss", DataType.Boolean, s1, s3).Boolean);
            Assert.IsTrue(Calc(funs, "Like_ss", DataType.Boolean, s1, s4).Boolean);
            Assert.IsFalse(Calc(funs, "Like_ss", DataType.Boolean, s1, s2).Boolean);
            Assert.IsFalse(Calc(funs, "Like_ss", DataType.Boolean, s1, s5).Boolean);
            Assert.IsFalse(Calc(funs, "Like_ss", DataType.Boolean, s5, s3).Boolean);
            Assert.IsTrue(Calc(funs, "Like_ss", DataType.Boolean, s5, s5).Boolean);

            Assert.IsTrue(Calc(funs, "Equal_uu", DataType.Integer, i1, i1).Boolean);
            Assert.IsFalse(Calc(funs, "Equal_uu", DataType.Integer, s4, s3).Boolean);
            Assert.IsTrue(Calc(funs, "Equal_uu", DataType.Integer, s1, s1).Boolean);

            Assert.IsFalse(Calc(funs, "NotEqual_uu", DataType.Integer, i1, i1).Boolean);
            Assert.IsTrue(Calc(funs, "NotEqual_uu", DataType.Integer, s4, s3).Boolean);
            Assert.IsFalse(Calc(funs, "NotEqual_uu", DataType.Integer, s1, s1).Boolean);

            Assert.IsFalse(Calc(funs, "Less_uu", DataType.Integer, i1, i2).Boolean);
            Assert.IsFalse(Calc(funs, "Less_uu", DataType.Integer, r2, r2).Boolean);
            Assert.IsTrue(Calc(funs, "Less_uu", DataType.Integer, s1, s2).Boolean);

            Assert.IsFalse(Calc(funs, "LessEqual_uu", DataType.Integer, i1, i2).Boolean);
            Assert.IsTrue(Calc(funs, "LessEqual_uu", DataType.Integer, r2, r2).Boolean);
            Assert.IsTrue(Calc(funs, "LessEqual_uu", DataType.Integer, s1, s2).Boolean);

            Assert.IsFalse(Calc(funs, "Greater_uu", DataType.Integer, i2, i1).Boolean);
            Assert.IsFalse(Calc(funs, "Greater_uu", DataType.Integer, r2, r2).Boolean);
            Assert.IsTrue(Calc(funs, "Greater_uu", DataType.Integer, s2, s1).Boolean);

            Assert.IsFalse(Calc(funs, "GreaterEqual_uu", DataType.Integer, i2, i1).Boolean);
            Assert.IsTrue(Calc(funs, "GreaterEqual_uu", DataType.Integer, r2, r2).Boolean);
            Assert.IsTrue(Calc(funs, "GreaterEqual_uu", DataType.Integer, s2, s1).Boolean);

            m = Calc(funs, "Divide_rr", DataType.Real, i1, i3);
            IMean b1 = new MeanBool(true), b2 = new MeanBool(false), b3 = new MeanErrBool(false, m.Error);

            m = Calc(funs, "Not_b", DataType.Boolean, b1);
            Assert.IsFalse(m.Boolean);
            Assert.IsNull(m.Error);
            m = Calc(funs, "Not_b", DataType.Boolean, b3);
            Assert.IsTrue(m.Boolean);
            Assert.IsNotNull(m.Error);
            Assert.AreEqual(501, m.Error.Number);

            m = Calc(funs, "Xor_bb", DataType.Boolean, b1, b2);
            Assert.IsTrue(m.Boolean);
            Assert.IsNull(m.Error);
            m = Calc(funs, "Xor_bb", DataType.Boolean, b2, b3);
            Assert.IsFalse(m.Boolean);
            Assert.IsNotNull(m.Error);
            Assert.AreEqual(501, m.Error.Number);
            m = Calc(funs, "Xor_bb", DataType.Boolean, b3, b3);
            Assert.IsFalse(m.Boolean);
            Assert.IsNotNull(m.Error);
            Assert.AreEqual(501, m.Error.Number);
            Assert.AreEqual(17, Calc(funs, "Xor_ii", DataType.Integer, i1, i2).Integer);

            m = Calc(funs, "Or_bb", DataType.Boolean, b1, b2);
            Assert.IsTrue(m.Boolean);
            Assert.IsNull(m.Error);
            m = Calc(funs, "Or_bb", DataType.Boolean, b1, b3);
            Assert.IsTrue(m.Boolean);
            Assert.IsNull(m.Error);
            m = Calc(funs, "Or_bb", DataType.Boolean, b2, b3);
            Assert.IsFalse(m.Boolean);
            Assert.IsNotNull(m.Error);
            Assert.AreEqual(501, m.Error.Number);
            Assert.AreEqual(21, Calc(funs, "Or_ii", DataType.Integer, i1, i2).Integer);

            m = Calc(funs, "And_bb", DataType.Boolean, b1, b2);
            Assert.IsFalse(m.Boolean);
            Assert.IsNull(m.Error);
            m = Calc(funs, "And_bb", DataType.Boolean, b1, b3);
            Assert.IsFalse(m.Boolean);
            Assert.IsNotNull(m.Error);
            Assert.AreEqual(501, m.Error.Number);
            m = Calc(funs, "And_bb", DataType.Boolean, b1, b1);
            Assert.IsTrue(m.Boolean);
            Assert.IsNull(m.Error);
            Assert.AreEqual(4, Calc(funs, "And_ii", DataType.Integer, i1, i2).Integer);
        }

        [TestMethod]
        public void Logic()
        {
            var funs = new FunctionsGen();
            IMean i1 = new MeanInt(21), i2 = new MeanInt(2), i3 = new MeanInt(0), i4 = new MeanInt(3);

            Assert.AreEqual(5, Calc(funs, "Sr_ii", DataType.Integer, i1, i2).Integer);
            Assert.AreEqual(84, Calc(funs, "Sl_ii", DataType.Integer, i1, i2).Integer);

            Assert.IsTrue(Calc(funs, "Bit_ii", DataType.Boolean, i1, i3).Boolean);
            Assert.IsTrue(Calc(funs, "Bit_ii", DataType.Boolean, i1, i2).Boolean);
            Assert.IsFalse(Calc(funs, "Bit_ii", DataType.Boolean, i1, i4).Boolean);

            Assert.IsTrue(Calc(funs, "BitOr_iii", DataType.Boolean, i1, i3, i2).Boolean);
            Assert.IsTrue(Calc(funs, "BitOr_iii", DataType.Boolean, i1, i2, i3, i4).Boolean);
            Assert.IsFalse(Calc(funs, "BitOr_iii", DataType.Boolean, i1, i4).Boolean);
            Assert.IsTrue(Calc(funs, "BitAnd_iii", DataType.Boolean, i1, i3, i2).Boolean);
            Assert.IsFalse(Calc(funs, "BitAnd_iii", DataType.Boolean, i1, i4, i2).Boolean);
        }

         [TestMethod]
         public void Mathematical()
         {
             var funs = new FunctionsGen();
             IMean r1 = new MeanReal(2.345), r2 = new MeanReal(-1.234);
             IMean i1 = new MeanInt(2), r3 = new MeanReal(0);

             Assert.AreEqual(2, Calc(funs, "Round_r", DataType.Integer, r1).Integer);
             Assert.AreEqual(-1, Calc(funs, "Round_r", DataType.Integer, r2).Integer);
             Assert.AreEqual(2.35, Calc(funs, "Round_ri", DataType.Real, r1, i1).Real);
             Assert.AreEqual(-1.23, Calc(funs, "Round_ri", DataType.Real, r2, i1).Real);

             Assert.AreEqual(2, Calc(funs, "Abs_i", DataType.Integer, i1).Integer);
             Assert.AreEqual(2.345, Calc(funs, "Abs_r", DataType.Real, r1).Real);
             Assert.AreEqual(1.234, Calc(funs, "Abs_r", DataType.Real, r2).Real);

             Assert.AreEqual(1, Calc(funs, "Sign_i", DataType.Integer, i1).Integer);
             Assert.AreEqual(1, Calc(funs, "Sign_r", DataType.Integer, r1).Integer);
             Assert.AreEqual(-1, Calc(funs, "Sign_r", DataType.Integer, r2).Integer);
             Assert.AreEqual(0, Calc(funs, "Sign_r", DataType.Integer, r3).Integer);

             Assert.AreEqual(Math.Sqrt(2.345), Calc(funs, "Sqr_r", DataType.Real, r1).Real);
             var m = Calc(funs, "Sqr_r", DataType.Real, r2);
             Assert.AreEqual(1001, m.Error.Number);

             IMean r4 = new MeanErrReal(-0.567, m.Error);
             IMean s1 = new MeanString("1111");

             m = Calc(funs, "Min_uu", DataType.Real, r1, r2, r3, r4);
             Assert.AreEqual(-1.234, m.Real);
             Assert.AreEqual(1001, m.Error.Number);
             m = Calc(funs, "Min_uu", DataType.Real, r1, r2, r3);
             Assert.AreEqual(-1.234, m.Real);
             Assert.IsNull(m.Error);
             m = Calc(funs, "Min_uu", DataType.Integer, i1, r3);
             Assert.AreEqual(0, m.Integer);
             Assert.IsNull(m.Error);
             m = Calc(funs, "Min_uu", DataType.String, i1, r1, s1);
             Assert.AreEqual("1111", m.String);
             Assert.IsNull(m.Error);

             m = Calc(funs, "Max_uu", DataType.Real, r1, r2, r3, r4);
             Assert.AreEqual(2.345, m.Real);
             Assert.AreEqual(1001, m.Error.Number);
             m = Calc(funs, "Max_uu", DataType.Real, r1, r2, r3);
             Assert.AreEqual(2.345, m.Real);
             Assert.IsNull(m.Error);
             m = Calc(funs, "Max_uu", DataType.Integer, i1, r3);
             Assert.AreEqual(2, m.Integer);
             Assert.IsNull(m.Error);
             m = Calc(funs, "Max_uu", DataType.String, i1, r1, s1);
             Assert.AreEqual("2,345", m.String);
             Assert.IsNull(m.Error);
         }
    }
}