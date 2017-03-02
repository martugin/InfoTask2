using System;
using CommonTypes;
using Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneratorTest
{
    [TestClass]
    public class RealisationScalarTest
    {
        //Вычисление значения скалярной функции
        private IMean Calc(GenFunctions funs, string funName, DataType dtype, params IMean[] pars)
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
            var funs = new GenFunctions();
            IMean i1 = new IntMean(20), i2 = new IntMean(5), i3 = new IntMean(0);
            IMean r1 = new RealMean(2.4), r2 = new RealMean(0.6), r3 = new RealMean(120);
            IMean d1 = new TimeMean(RTime(60)), d2 = new TimeMean(RTime(70));
            IMean s1 = new StringMean("Abc"), s2 = new StringMean("sT");

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

            IMean i4 = new IntMean(-2), r4 = new RealMean(-1.1);

            Assert.AreEqual(79.62624, Calc(funs, "Power_rr", DataType.Real, r1, i2).Real);
            Assert.AreEqual(0, Calc(funs, "Power_rr", DataType.Real, i3, i4).Real);
            Assert.AreEqual(-32, Calc(funs, "Power_rr", DataType.Real, i4, i2).Real);
            Assert.AreEqual(531, Calc(funs, "Power_rr", DataType.Real, i4, r4).Error.Number);
            Assert.AreEqual(532, Calc(funs, "Power_rr", DataType.Real, i3, i3).Error.Number);

            IMean s3 = new StringMean("A*c"), s4 = new StringMean("Ab?"), s5 = new StringMean("");
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
            IMean b1 = new BoolMean(true), b2 = new BoolMean(false), b3 = new MeanErrBool(false, m.Error);

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
            var funs = new GenFunctions();
            IMean i1 = new IntMean(21), i2 = new IntMean(2), i3 = new IntMean(0), i4 = new IntMean(3);

            var c = (ConstGenFun)(funs.CurFun = funs.Funs["TrueFun_"]);
            var m = c.Calculate(new IMean[0], DataType.Boolean);
            Assert.IsTrue(m.Boolean);
            c = (ConstGenFun)(funs.CurFun = funs.Funs["FalseFun_"]);
            m = c.Calculate(new IMean[0], DataType.Boolean);
            Assert.IsFalse(m.Boolean);

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
             var funs = new GenFunctions();
             IMean r1 = new RealMean(2.345), r2 = new RealMean(-1.234);
             IMean i1 = new IntMean(2), r3 = new RealMean(0);

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
             IMean s1 = new StringMean("1111");

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

             Assert.AreEqual(Math.Exp(2), Calc(funs, "Exp_r", DataType.Real, i1).Real);
             Assert.AreEqual(1, Calc(funs, "Exp_r", DataType.Real, r3).Real);
             Assert.AreEqual(Math.Exp(-1.234), Calc(funs, "Exp_r", DataType.Real, r2).Real);

             Assert.AreEqual(Math.Log(2), Calc(funs, "Ln_r", DataType.Real, i1).Real);
             Assert.AreEqual(891, Calc(funs, "Ln_r", DataType.Real, r3).Error.Number);
             Assert.AreEqual(891, Calc(funs, "Ln_r", DataType.Real, r2).Error.Number);

             Assert.AreEqual(Math.Log10(2), Calc(funs, "Log10_r", DataType.Real, i1).Real);
             Assert.AreEqual(911, Calc(funs, "Log10_r", DataType.Real, r3).Error.Number);
             Assert.AreEqual(911, Calc(funs, "Log10_r", DataType.Real, r2).Error.Number);

             Assert.AreEqual(Math.Log(2.345, 2), Calc(funs, "Log_rr", DataType.Real, r1, i1).Real);
             Assert.AreEqual(901, Calc(funs, "Log_rr", DataType.Real, r2, r1).Error.Number);
             Assert.AreEqual(901, Calc(funs, "Log_rr", DataType.Real, r1, r2).Error.Number);

             var c = (ConstGenFun)(funs.CurFun = funs.Funs["Pi_"]);
             m = c.Calculate(new IMean[0], DataType.Real);
             Assert.AreEqual(Math.PI, m.Real);

             IMean r5 = new RealMean(0.789), r6 = new RealMean(-Math.PI/2), r7 = new IntMean(-1);
             Assert.AreEqual(0, Calc(funs, "Sin_r", DataType.Real, r3).Real);
             Assert.AreEqual(Math.Sin(0.789), Calc(funs, "Sin_r", DataType.Real, r5).Real);
             Assert.AreEqual(-1, Calc(funs, "Sin_r", DataType.Real, r6).Real);

             Assert.AreEqual(1, Calc(funs, "Cos_r", DataType.Real, r3).Real);
             Assert.AreEqual(Math.Cos(0.789), Calc(funs, "Cos_r", DataType.Real, r5).Real);
             Assert.IsTrue(Math.Abs(Calc(funs, "Cos_r", DataType.Real, r6).Real) < 0.001);

             Assert.AreEqual(0, Calc(funs, "Tan_r", DataType.Real, r3).Real);
             Assert.AreEqual(Math.Tan(0.789), Calc(funs, "Tan_r", DataType.Real, r5).Real);
             
             Assert.AreEqual(870, Calc(funs, "Ctan_r", DataType.Real, r3).Error.Number);
             Assert.IsTrue(Math.Abs(1 / Math.Tan(0.789) - Calc(funs, "Ctan_r", DataType.Real, r5).Real) < 0.001);
             Assert.IsTrue(Math.Abs(Calc(funs, "Ctan_r", DataType.Real, r6).Real) < 0.001);

             Assert.AreEqual(Math.Sinh(0), Calc(funs, "Sh_r", DataType.Real, r3).Real);
             Assert.AreEqual(Math.Sinh(0.789), Calc(funs, "Sh_r", DataType.Real, r5).Real);
             Assert.AreEqual(Math.Sinh(-Math.PI/2), Calc(funs, "Sh_r", DataType.Real, r6).Real);

             Assert.AreEqual(Math.Cosh(0), Calc(funs, "Ch_r", DataType.Real, r3).Real);
             Assert.AreEqual(Math.Cosh(0.789), Calc(funs, "Ch_r", DataType.Real, r5).Real);
             Assert.AreEqual(Math.Cosh(-Math.PI / 2), Calc(funs, "Ch_r", DataType.Real, r6).Real);

             Assert.AreEqual(Math.Tanh(0), Calc(funs, "Th_r", DataType.Real, r3).Real);
             Assert.AreEqual(Math.Tanh(0.789), Calc(funs, "Th_r", DataType.Real, r5).Real);

             Assert.AreEqual(0, Calc(funs, "Arcsin_r", DataType.Real, r3).Real);
             Assert.AreEqual(Math.Asin(0.789), Calc(funs, "Arcsin_r", DataType.Real, r5).Real);
             Assert.IsTrue(Math.Abs(- Math.PI / 2 - Calc(funs, "Arcsin_r", DataType.Real, r7).Real) < 0.001);

             Assert.IsTrue(Math.Abs(Math.PI / 2 - Calc(funs, "Arccos_r", DataType.Real, r3).Real) < 0.001);
             Assert.AreEqual(Math.Acos(0.789), Calc(funs, "Arccos_r", DataType.Real, r5).Real);
             Assert.IsTrue(Math.Abs(Calc(funs, "Arccos_r", DataType.Real, r6).Real) < 0.0001);

             Assert.AreEqual(0, Calc(funs, "Arctan_r", DataType.Real, r3).Real);
             Assert.AreEqual(Math.Atan(0.789), Calc(funs, "Arctan_r", DataType.Real, r5).Real);
         }

        [TestMethod]
        public void DataTypes()
        {
            var funs = new GenFunctions();
            IMean s1 = new StringMean("1"), s2 = new StringMean("-5"), s3 = new StringMean("23.456"), s4 = new StringMean("10.10.2010 10:10:10");
            IMean r1 = new RealMean(-0.1), i1 = new IntMean(0);

            Assert.IsTrue(Calc(funs, "IsInt_u", DataType.Boolean, s1).Boolean);
            Assert.IsTrue(Calc(funs, "IsInt_u", DataType.Boolean, s2).Boolean);
            Assert.IsFalse(Calc(funs, "IsInt_u", DataType.Boolean, s3).Boolean);
            Assert.IsFalse(Calc(funs, "IsInt_u", DataType.Boolean, s4).Boolean);
            Assert.IsTrue(Calc(funs, "IsInt_u", DataType.Boolean, i1).Boolean);
            Assert.IsFalse(Calc(funs, "IsInt_u", DataType.Boolean, r1).Boolean);

            Assert.IsTrue(Calc(funs, "IsReal_u", DataType.Boolean, s1).Boolean);
            Assert.IsTrue(Calc(funs, "IsReal_u", DataType.Boolean, s2).Boolean);
            Assert.IsTrue(Calc(funs, "IsReal_u", DataType.Boolean, s3).Boolean);
            Assert.IsFalse(Calc(funs, "IsReal_u", DataType.Boolean, s4).Boolean);
            Assert.IsTrue(Calc(funs, "IsReal_u", DataType.Boolean, i1).Boolean);
            Assert.IsTrue(Calc(funs, "IsReal_u", DataType.Boolean, r1).Boolean);

            Assert.IsFalse(Calc(funs, "IsTime_u", DataType.Boolean, s3).Boolean);
            Assert.IsTrue(Calc(funs, "IsTime_u", DataType.Boolean, s4).Boolean);
            Assert.IsFalse(Calc(funs, "IsTime_u", DataType.Boolean, i1).Boolean);

            Assert.IsTrue(Calc(funs, "Bool_u", DataType.Boolean, s1).Boolean);
            Assert.IsTrue(Calc(funs, "Bool_u", DataType.Boolean, s2).Boolean);
            Assert.IsFalse(Calc(funs, "Bool_u", DataType.Boolean, i1).Boolean);

            Assert.AreEqual(1, Calc(funs, "Int_u", DataType.Integer, s1).Integer);
            Assert.AreEqual(-5, Calc(funs, "Int_u", DataType.Integer, s2).Integer);
            Assert.AreEqual(-1, Calc(funs, "Int_u", DataType.Integer, r1).Integer);

            Assert.AreEqual(-5, Calc(funs, "Real_u", DataType.Real, s2).Real);
            Assert.AreEqual(23.456, Calc(funs, "Real_u", DataType.Real, s3).Real);

            Assert.AreEqual(new DateTime(2010, 10, 10, 10, 10, 10), Calc(funs, "Date_u", DataType.Time, s4).Date);

            Assert.AreEqual("10.10.2010 10:10:10", Calc(funs, "String_u", DataType.String, s4).String);
            Assert.AreEqual("-0,1", Calc(funs, "String_u", DataType.String, r1).String);
            Assert.AreEqual("0", Calc(funs, "String_u", DataType.String, i1).String);
        }

        [TestMethod]
        public void Strings()
        {
            var funs = new GenFunctions();
            IMean s1 = new StringMean(" AbAb AA  "), s2 = new StringMean("123Ab34 "), s3 = new StringMean(""), s4 = new StringMean(null);

            var c = (ConstGenFun)(funs.CurFun = funs.Funs["NewLine_"]);
            IMean m = c.Calculate(new IMean[0], DataType.String);
            Assert.AreEqual(Environment.NewLine, m.String);

            Assert.AreEqual(0, Calc(funs, "StrLen_s", DataType.Integer, s3).Integer);
            Assert.AreEqual(0, Calc(funs, "StrLen_s", DataType.Integer, s4).Integer);
            Assert.AreEqual(10, Calc(funs, "StrLen_s", DataType.Integer, s1).Integer);
            Assert.AreEqual(8, Calc(funs, "StrLen_s", DataType.Integer, s2).Integer);

            Assert.AreEqual("AbAb AA  ", Calc(funs, "StrLTrim_s", DataType.String, s1).String);
            Assert.AreEqual("123Ab34 ", Calc(funs, "StrLTrim_s", DataType.String, s2).String);
            Assert.AreEqual("", Calc(funs, "StrLTrim_s", DataType.String, s3).String);
            Assert.AreEqual("", Calc(funs, "StrLTrim_s", DataType.String, s4).String);

            Assert.AreEqual(" AbAb AA", Calc(funs, "StrRTrim_s", DataType.String, s1).String);
            Assert.AreEqual("123Ab34", Calc(funs, "StrRTrim_s", DataType.String, s2).String);
            Assert.AreEqual("", Calc(funs, "StrRTrim_s", DataType.String, s3).String);
            Assert.AreEqual("", Calc(funs, "StrRTrim_s", DataType.String, s4).String);

            Assert.AreEqual("AbAb AA", Calc(funs, "StrTrim_s", DataType.String, s1).String);
            Assert.AreEqual("123Ab34", Calc(funs, "StrTrim_s", DataType.String, s2).String);
            Assert.AreEqual("", Calc(funs, "StrTrim_s", DataType.String, s3).String);
            Assert.AreEqual("", Calc(funs, "StrTrim_s", DataType.String, s4).String);

            Assert.AreEqual(" abab aa  ", Calc(funs, "StrLCase_s", DataType.String, s1).String);
            Assert.AreEqual("123ab34 ", Calc(funs, "StrLCase_s", DataType.String, s2).String);
            Assert.AreEqual("", Calc(funs, "StrLCase_s", DataType.String, s3).String);
            Assert.AreEqual("", Calc(funs, "StrLCase_s", DataType.String, s4).String);

            Assert.AreEqual(" ABAB AA  ", Calc(funs, "StrUCase_s", DataType.String, s1).String);
            Assert.AreEqual("123AB34 ", Calc(funs, "StrUCase_s", DataType.String, s2).String);
            Assert.AreEqual("", Calc(funs, "StrUCase_s", DataType.String, s3).String);
            Assert.AreEqual("", Calc(funs, "StrUCase_s", DataType.String, s4).String);

            IMean i1 = new IntMean(3), i2 = new IntMean(0), i3 = new IntMean(5), i4 = new IntMean(-3);

            Assert.AreEqual(" Ab", Calc(funs, "StrLeft_si", DataType.String, s1, i1).String);
            Assert.AreEqual("123", Calc(funs, "StrLeft_si", DataType.String, s2, i1).String);
            Assert.AreEqual("", Calc(funs, "StrLeft_si", DataType.String, s3, i2).String);
            Assert.AreEqual(2480, Calc(funs, "StrLeft_si", DataType.String, s1, i4).Error.Number);

            Assert.AreEqual("A  ", Calc(funs, "StrRight_si", DataType.String, s1, i1).String);
            Assert.AreEqual("34 ", Calc(funs, "StrRight_si", DataType.String, s2, i1).String);
            Assert.AreEqual("", Calc(funs, "StrRight_si", DataType.String, s3, i2).String);
            Assert.AreEqual(2490, Calc(funs, "StrRight_si", DataType.String, s1, i4).Error.Number);

            Assert.AreEqual("bAb A", Calc(funs, "StrMid_sii", DataType.String, s1, i1, i3).String);
            Assert.AreEqual("b34", Calc(funs, "StrMid_sii", DataType.String, s2, i3, i1).String);
            Assert.AreEqual("", Calc(funs, "StrMid_sii", DataType.String, s3, i2, i2).String);
            Assert.AreEqual(2460, Calc(funs, "StrMid_sii", DataType.String, s2, i3, i3).Error.Number);
            Assert.AreEqual("b AA  ", Calc(funs, "StrMid_si", DataType.String, s1, i3).String);
            Assert.AreEqual(2470, Calc(funs, "StrMid_si", DataType.String, s3, i1).Error.Number);

            IMean s5 = new StringMean("Ab"), s6 = new StringMean("AA");
            Assert.AreEqual(2, Calc(funs, "StrFind_ssi", DataType.Integer, s5, s1).Integer);
            Assert.AreEqual(4, Calc(funs, "StrFind_ssi", DataType.Integer, s5, s2).Integer);
            Assert.AreEqual(0, Calc(funs, "StrFind_ssi", DataType.Integer, s6, s2).Integer);
            Assert.AreEqual(0, Calc(funs, "StrFind_ssi", DataType.Integer, s5, s4).Integer);
            Assert.AreEqual(4, Calc(funs, "StrFind_ssi", DataType.Integer, s5, s1, i1).Integer);
            Assert.AreEqual(0, Calc(funs, "StrFind_ssi", DataType.Integer, s5, s2, i3).Integer);

            Assert.AreEqual(4, Calc(funs, "StrFindLast_ssi", DataType.Integer, s5, s1).Integer);
            Assert.AreEqual(4, Calc(funs, "StrFindLast_ssi", DataType.Integer, s5, s2).Integer);
            Assert.AreEqual(0, Calc(funs, "StrFindLast_ssi", DataType.Integer, s6, s2).Integer);
            Assert.AreEqual(0, Calc(funs, "StrFindLast_ssi", DataType.Integer, s5, s4).Integer);
            Assert.AreEqual(2, Calc(funs, "StrFindLast_ssi", DataType.Integer, s5, s1, i1).Integer);
            Assert.AreEqual(0, Calc(funs, "StrFindLast_ssi", DataType.Integer, s6, s1, i3).Integer);

            Assert.AreEqual(" AAAA AA  ", Calc(funs, "StrReplace_sss", DataType.String, s1, s5, s6).String);
            Assert.AreEqual(" AbAA  ", Calc(funs, "StrRemove_sii", DataType.String, s1, i3, i1).String);
            Assert.AreEqual(1800, Calc(funs, "StrRemove_sii", DataType.String, s2, i3, i3).Error.Number);
            Assert.AreEqual(" AbA", Calc(funs, "StrRemove_si", DataType.String, s1, i3).String);
            Assert.AreEqual(" AbAAAb AA  ", Calc(funs, "StrInsert_ssi", DataType.String, s1, s6, i3).String);
            Assert.AreEqual(1780, Calc(funs, "StrInsert_ssi", DataType.String, s1, s6, i4).Error.Number);

            IMean s7 = new StringMean(".*Ab.*"), s8 = new StringMean("A.?");
            Assert.IsTrue(Calc(funs, "StrRegMatch_ss", DataType.Boolean, s1, s7).Boolean);
            Assert.IsFalse(Calc(funs, "StrRegMatch_ss", DataType.Boolean, s1, s8).Boolean);

            Assert.AreEqual(2, Calc(funs, "StrRegFind_ss", DataType.Integer, s1, s8).Integer);
            Assert.AreEqual(7, Calc(funs, "StrRegFind_ss", DataType.Integer, s1, s6).Integer);

            Assert.AreEqual("123AA34 ", Calc(funs, "StrRegReplace_sss", DataType.String, s2, s8, s6).String);

            Assert.AreEqual("AbAb_AA", Calc(funs, "ToIdent_s", DataType.String, s1).String);
            Assert.AreEqual("A__", Calc(funs, "ToIdent_s", DataType.String, s8).String);
        }
    }
}