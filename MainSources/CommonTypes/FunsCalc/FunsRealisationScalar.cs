using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using BaseLibrary;

namespace CommonTypes
{
    //Реализация скалярных функции
    public partial class FunctionsBase
    {
        //1 - Операции
        #region
        public void Plus_ii(IMom[] par)
        {
            ScalarRes.Integer = par[0].Integer + par[1].Integer;
        }

        public void Plus_dr(IMom[] par)
        {
            ScalarRes.Date = par[0].Date.AddSeconds(par[1].Real);
        }

        public void Plus_rd(IMom[] par)
        {
            ScalarRes.Date = par[1].Date.AddSeconds(par[0].Real); 
        }

        public void Plus_rr(IMom[] par)
        {
            ScalarRes.Real = par[0].Real + par[1].Real;
        }

        public void Plus_ss(IMom[] par)
        {
            ScalarRes.String = par[0].String + par[1].String;
        }

        public void Minus_ii(IMom[] par)
        {
            ScalarRes.Integer = par[0].Integer - par[1].Integer;
        }

        public void Minus_dd(IMom[] par)
        {
            ScalarRes.Real = par[0].Date.Minus(par[1].Date);
        }

        public void Minus_dr(IMom[] par)
        {
            ScalarRes.Date = par[0].Date.AddSeconds(-par[1].Real); 
        }

        public void Minus_rr(IMom[] par)
        {
            ScalarRes.Real = par[0].Real - par[1].Real;
        }

        public void Minus_i(IMom[] par)
        {
            ScalarRes.Integer = -par[0].Integer;
        }

        public void Minus_r(IMom[] par)
        {
            ScalarRes.Real = -par[0].Real;
        }

        public void Multiply_ii(IMom[] par)
        {
            ScalarRes.Integer = par[0].Integer * par[1].Integer;
            if (par[0].Integer == 0)
                ScalarRes.Error = par[1].Integer == 0 ? MinErr(par) : par[0].Error;
            else if (par[1].Integer == 0)
                ScalarRes.Error = par[1].Error;
        }

        public void Multiply_rr(IMom[] par)
        {
            ScalarRes.Real = par[0].Real * par[1].Real;
            if (par[0].Real == 0)
                ScalarRes.Error = par[1].Real == 0 ? MinErr(par) : par[0].Error;
            else if (par[1].Real == 0)
                ScalarRes.Error = par[1].Error;
        }

        public void Divide_rr(IMom[] par)
        {
            if (par[0].Real == 0)
                ScalarRes.Error = par[0].Error;
            if (par[1].Real == 0) PutErr("Деление на 0");
            else ScalarRes.Real = par[0].Real / par[1].Real;
        }

        public void Div_ii(IMom[] par)
        {
            if (par[0].Integer == 0)
                ScalarRes.Error = par[0].Error;
            if (par[1].Integer == 0) PutErr("Деление на 0");
            else ScalarRes.Integer = par[0].Integer / par[1].Integer;
        }

        public void Mod_ii(IMom[] par)
        {
            if (par[0].Integer == 0)
                ScalarRes.Error = par[0].Error;
            if (par[1].Integer == 0) PutErr("Деление с остатком на 0");
            else ScalarRes.Integer = par[0].Integer % par[1].Integer;
        }

        public void Power_rr(IMom[] par)
        {
            double m0 = par[0].Real, m1 = par[1].Real;
            if (m0 == 0)
                ScalarRes.Error = par[0].Error;
            double m = 1;
            if (m0 < 0)
            {
                if (Convert.ToInt32(m1) == m1)
                {
                    if (m1 > 0)
                        for (int i = 1; i <= m1; ++i) m = m * m0;
                    else for (int i = 1; i <= -m1; ++i) m = m / m0;
                }
                else PutErr("Недопустимое возведение отрицательного числа в степень");
            }
            else
            {
                if (m0 == 0)
                {
                    if (m1 == 0) PutErr("Возведение ноля в нолевую степень", 2, 1);
                    else m = 0;
                }
                else
                {
                    if (m1 == 0) m = 1;
                    else
                    {
                        if (Convert.ToInt32(m1) == m1)
                        {
                            if (m1 > 0)
                                for (int i = 1; i <= Convert.ToInt32(m1); ++i)
                                    m = m * m0;
                            else for (int i = 1; i <= -Convert.ToInt32(m1); ++i)
                                    m = m / m0;
                        }
                        else m = Math.Pow(m0, m1);
                    }
                }
            }
            ScalarRes.Real = m;
        }

        public void Equal_uu(IMom[] par)
        {
            ScalarRes.Boolean = par[0].ValueEquals(par[1]);
        }

        public void NotEqual_uu(IMom[] par)
        {
            ScalarRes.Boolean = !par[0].ValueEquals(par[1]);
        }

        public void Less_uu(IMom[] par)
        {
            ScalarRes.Boolean = par[0].ValueLess(par[1]);
        }

        public void LessEqual_uu(IMom[] par)
        {
            ScalarRes.Boolean = par[0].ValueLess(par[1]) || par[0].ValueEquals(par[1]);
        }

        public void Greater_uu(IMom[] par)
        {
            ScalarRes.Boolean = !par[0].ValueLess(par[1]) && !par[0].ValueEquals(par[1]);
        }

        public void GreaterEqual_uu(IMom[] par)
        {
            ScalarRes.Boolean = !par[0].ValueLess(par[1]);
        }

        public void Not_b(IMom[] par)
        {
            ScalarRes.Boolean = !par[0].Boolean;
        }

        public void Xor_bb(IMom[] par)
        {
            ScalarRes.Boolean = par[0].Boolean ^ par[1].Boolean;
        }

        public void Xor_ii(IMom[] par)
        {
            ScalarRes.Integer = par[0].Integer ^ par[1].Integer;
        }

        public void Or_bb(IMom[] par)
        {
            bool m0 = par[0].Boolean;
            bool m1 = par[1].Boolean;
            ScalarRes.Boolean = m0 || m1;
            if (m0 && !m1)
                ScalarRes.Error = par[0].Error;
            if (!m0 && m1)
                ScalarRes.Error = par[1].Error;
            if (m0 && m1)
                ScalarRes.Error = MinErr(par);
        }

        public void Or_ii(IMom[] par)
        {
            ScalarRes.Integer = par[0].Integer | par[1].Integer;
        }

        public void And_bb(IMom[] par)
        {
            bool m0 = par[0].Boolean;
            bool m1 = par[1].Boolean;
            ScalarRes.Boolean = m0 && m1;
            if (!m0 && m1)
                ScalarRes.Error = par[0].Error;
            if (m0 && !m1)
                ScalarRes.Error = par[1].Error;
            if (!m0 && !m1)
                ScalarRes.Error = MinErr(par);
        }

        public void And_ii(IMom[] par)
        {
            ScalarRes.Integer = par[0].Integer & par[1].Integer;
        }

        public void Like_ss(IMom[] par)
        {
            string m1 = "^" + par[1].String.Replace("*", @"[\S|\s]*").Replace("?", @"[\S|\s]") + "$";
            string m0 = par[0].String;
            ScalarRes.Boolean = new Regex(m1).IsMatch(m0);
        }
        #endregion

        //2 - Логические
        #region
        public IVal TrueFun_()
        {
            return new MeanBool(true);
        }

        public IVal FalseFun_()
        {
            return new MeanBool(false);
        }

        private bool GetBit(IMom[] par, int num)
        {
            if (num < 0 || num > 31)
                PutErr("Номер бита должен быть от 0 до 31");
            return par[0].Integer.GetBit(par[num].Integer);
        }

        public void Bit_ii(IMom[] par)
        {
            ScalarRes.Boolean = GetBit(par, 1);
        }

        public void BitAnd_ii(IMom[] par)
        {
            var b = true;
            for (int i = 1; i < par.Length; i++)
                b &= GetBit(par, i); 
            ScalarRes.Boolean = b;
        }

        public void BitOr_ii(IMom[] par)
        {
            var b = false;
            for (int i = 1; i < par.Length; i++)
                b |= GetBit(par, i); 
            ScalarRes.Boolean = b;
        }

        public void Sr_ii(IMom[] par)
        {
            ScalarRes.Integer = par[0].Integer >> par[1].Integer;
        }

        public void Sl_ii(IMom[] par)
        {
            ScalarRes.Integer = par[0].Integer << par[1].Integer;
        }
        #endregion

        //3 - Математические
        #region
        public IVal Random_()
        {
            return new MeanReal(new Random().NextDouble());
        }

        public void Random_ii(IMom[] par)
        {
            ScalarRes.Integer = new Random().Next(par[0].Integer, par[1].Integer);
        }

        public void Abs_i(IMom[] par)
        {
            ScalarRes.Integer = Math.Abs(par[0].Integer);
        }

        public void Abs_r(IMom[] par)
        {
            ScalarRes.Real = Math.Abs(par[0].Real);
        }

        public void Sign_i(IMom[] par)
        {
            ScalarRes.Integer = Math.Sign(par[0].Integer);
        }

        public void Sign_r(IMom[] par)
        {
            ScalarRes.Integer = Math.Sign(par[0].Real);
        }

        public void Round_r(IMom[] par)
        {
            ScalarRes.Integer = Convert.ToInt32(par[0].Real);
        }

        public void Round_ri(IMom[] par)
        {
            ScalarRes.Real = Math.Round(par[0].Real, par[1].Integer);
        }

        public void Min_u(IMom[] par)
        {
            IMom mv = par[0];
            for (int i = 1; i < par.Length; ++i)
                if (par[i].ValueLess(mv)) mv = par[i];
            ScalarRes.CopyValueFrom(mv);
        }

        public void Max_u(IMom[] par)
        {
            IMom mv = par[0];
            for (int i = 1; i < par.Length; ++i)
                if (mv.ValueLess(par[i])) mv = par[i];
            ScalarRes.CopyValueFrom(mv);
        }

        public IVal Pi_()
        {
            return new MeanReal(Math.PI);
        }

        public void Sqr_r(IMom[] par)
        {
            if (par[0].Real < 0) 
                PutErr("Извлечение корня из отрицательного числа");
            else ScalarRes.Real = Math.Sqrt(par[0].Real);
        }

        public void Cos_r(IMom[] par)
        {
            ScalarRes.Real = Math.Cos(par[0].Real);
        }

        public void Sin_r(IMom[] par)
        {
            ScalarRes.Real = Math.Sin(par[0].Real);
        }

        public void Tan_r(IMom[] par)
        {
            ScalarRes.Real = Math.Tan(par[0].Real);
        }

        public void Ctan_r(IMom[] par)
        {
            var tan = Math.Tan(par[0].Real);
            if (tan == 0) PutErr();
            ScalarRes.Real = 1 / tan;
        }

        public void Arccos_r(IMom[] par)
        {
            double d = par[0].Real;
            if (d < -1 || d > 1) PutErr();
            else ScalarRes.Real = Math.Acos(d);
        }

        public void Arcsin_r(IMom[] par)
        {
            double d = par[0].Real;
            if (d < -1 || d > 1) PutErr();
            else ScalarRes.Real = Math.Asin(d);
        }

        public void Arctan_r(IMom[] par)
        {
            ScalarRes.Real = Math.Atan(par[0].Real);
        }

        public void Sh_r(IMom[] par)
        {
            ScalarRes.Real = Math.Sinh(par[0].Real);
        }

        public void Ch_r(IMom[] par)
        {
            ScalarRes.Real = Math.Cosh(par[0].Real);
        }

        public void Th_r(IMom[] par)
        {
            ScalarRes.Real = Math.Tanh(par[0].Real);
        }

        public void Arcsh_r(IMom[] par)
        {
            double d = par[0].Real;
            d = Math.Log(d + Math.Sqrt(d * d + 1));
            ScalarRes.Real = d;
        }

        public void Arcch_r(IMom[] par)
        {
            double d = par[0].Real;
            d = Math.Log(d + Math.Sqrt(d * d - 1));
            ScalarRes.Real = d;
        }

        public void Arcth_r(IMom[] par)
        {
            double d = par[0].Real;
            if (d == 1 || (1 + d) / (1 - d) <= 0)
                PutErr();
            else ScalarRes.Real = Math.Log((1 + d) / (1 - d)) / 2;
        }

        public void Exp_r(IMom[] par)
        {
            ScalarRes.Real = Math.Exp(par[0].Real);
        }

        public void Ln_r(IMom[] par)
        {
            if (par[0].Real <= 0) 
                PutErr("Взятие логарифма от неположительного числа");
            else ScalarRes.Real = Math.Log(par[0].Real);
        }

        public void Log10_r(IMom[] par)
        {
            if (par[0].Real <= 0) 
                PutErr("Взятие логарифма от неположительного числа");
            else ScalarRes.Real = Math.Log10(par[0].Real);
        }

        public void Log_rr(IMom[] par)
        {
            double m0 = par[0].Real, m1 = par[1].Real;
            if (m0 == 1)
            {
                ScalarRes.Error = par[0].Error;
                ScalarRes.Real = 1;
            }
            else
            {
                if (m0 <= 0 || m1 <= 0)
                    PutErr("Взятие логарифма от неположительного числа или по неположительному основанию");
                else
                {
                    if (m1 == 1) PutErr("Взятие логарифма по основанию 1");
                    else ScalarRes.Real = Math.Log(m0, m1);
                }
            }
        }
        #endregion

        //4 - Недостоверность
        #region
        public void Quality_u(IMom[] par)
        {
            ScalarRes.Integer = par[0].Error == null ? 0 : (int)par[0].Error.Quality;
        }

        public void Error_u(IMom[] par)
        {
            ScalarRes.String = par[0].Error == null ? "" : par[0].Error.Text;
        }

        public IVal QuGood_()
        {
            return new MeanInt(0);
        }

        public IVal QuWarning_()
        {
            return new MeanInt(1);
        }

        public IVal QuError_()
        {
            return new MeanInt(2);
        }

        public void MakeError_u(IMom[] par)
        {
            ScalarRes.CopyValueFrom(par[0]);
            if (par.Length != 4 || par[3].Boolean)
                ScalarRes.Error = null;
            ScalarRes.AddError(new ErrMom(par[2].String, par[1].Integer));
            PutErr(par[2].String, par[1].Integer);
        }

        public void RemoveError_u(IMom[] par)
        {
            ScalarRes.CopyValueFrom(par[0]);
        }

        private double Certain(IEnumerable<IMean> par, IMean pn, double dn, double dp)
        {
            //Аппаратная недостоверность и сравнение с нормативным
            var dpar = (from p in par where (p.Error == null || p.Error.Quality != ErrQuality.Error) && Math.Abs(p.Real - pn.Real) <= dn select p.Real).ToList();
            if (dpar.Count == 0) return pn.Real;
            //Сравнение друг с другом
            while (dpar.Count > 2)
            {
                double m = dpar.Average();
                int k = 0;
                for (int i = 1; i < dpar.Count; i++)
                    if (Math.Abs(m - dpar[i]) > Math.Abs(m - dpar[k]))
                        k = i;
                if (Math.Abs(m - dpar[k]) > dp)
                    dpar.RemoveAt(k);
                else break;
            }
            if (dpar.Count == 2 && Math.Abs(dpar[1] - dpar[0]) > 2 * dp) return pn.Real;
            return dpar.Average();
        }

        public void CertainNP_rrrr(IMom[] par)
        {
            var parv = new IMean[par.Length - 3];
            for (int i = 0; i < parv.Length; i++)
                parv[i] = par[i + 3];
            ScalarRes.Error = par[0].Error.Add(par[1].Error).Add(par[2].Error);
            ScalarRes.Real = Certain(parv, par[0], par[1].Real, par[2].Real);
        }

        public void CertainN_rrr(IMom[] par)
        {
            var parv = new IMean[par.Length - 2];
            for (int i = 0; i < parv.Length; i++)
                parv[i] = par[i + 2];
            ScalarRes.Error = par[0].Error.Add(par[1].Error);
            ScalarRes.Real = Certain(parv, par[0], par[1].Real, double.MaxValue);
        }

        public void CertainP_rrr(IMom[] par)
        {
            var parv = new IMean[par.Length - 2];
            for (int i = 0; i < parv.Length; i++)
                parv[i] = par[i + 2];
            ScalarRes.Error = par[0].Error.Add(par[1].Error);
            ScalarRes.Real = Certain(parv, par[0], double.MaxValue, par[1].Real);
        }

        public void Certain_rr(IMom[] par)
        {
            var parv = new IMean[par.Length - 1];
            for (int i = 0; i < parv.Length; i++)
                parv[i] = par[i + 1];
            ScalarRes.Error = par[0].Error;
            ScalarRes.Real = Certain(parv, par[0], double.MaxValue, double.MaxValue);
        }
        #endregion

        //8 - Типы данных
        #region
        public void Bool_u(IMom[] par)
        {
            ScalarRes.Boolean = par[0].String != "0";
            if (par[0].String != "0" && par[0].String != "1")
                PutErr();
        }

        public void Int_u(IMom[] par)
        {
            if (par[0].DataType.LessOrEquals(DataType.Integer))
                ScalarRes.Integer = par[0].Integer;
            else
            {
                int i;
                if (par[0].DataType == DataType.Real) 
                    ScalarRes.Integer = Convert.ToInt32(Math.Floor(par[0].Real));
                else if (int.TryParse(par[0].String, out i)) 
                    ScalarRes.Integer = i;
                else PutErr();
            }
        }

        public void Real_u(IMom[] par)
        {
            ScalarRes.Real = par[0].DataType.LessOrEquals(DataType.Real) 
                                            ? par[0].Real 
                                            : (par[0].String ?? "0").ToDouble(double.NaN);
        }

        public void Date_u(IMom[] par)
        {
            ScalarRes.Date = par[0].String.ToDateTime();
            if (ScalarRes.Date == Different.MinDate)
                PutErr();
        }

        public void String_u(IMom[] par)
        {
            ScalarRes.String = par[0].String;
        }

        public void Value_u(IMom[] par)
        {
        }

        public void IsInt_u(IMom[] par)
        {
            int i;
            ScalarRes.Boolean = int.TryParse(par[0].String, out i);
        }

        public void IsReal_u(IMom[] par)
        {
            ScalarRes.Boolean = !double.IsNaN(par[0].String.ToDouble(double.NaN));
        }

        public void IsTime_u(IMom[] par)
        {
            ScalarRes.Boolean = par[0].String.ToDateTime() != Different.MinDate;
        }
        #endregion

        //9 - Время
        #region
        public IVal TiYear_()
        {
            return new MeanInt((int)TimeUnit.Year);
        }

        public IVal TiMonth_()
        {
            return new MeanInt((int)TimeUnit.Month);
        }

        public IVal TiDay_()
        {
            return new MeanInt((int)TimeUnit.Day);
        }

        public IVal TiHour()
        {
            return new MeanInt((int)TimeUnit.Hour);
        }

        public IVal TiMinute_()
        {
            return new MeanInt((int)TimeUnit.Minute);
        }

        public IVal TiSecond_()
        {
            return new MeanInt((int)TimeUnit.Second);
        }

        public IVal TiMsec_()
        {
            return new MeanInt((int)TimeUnit.MSec);
        }

        public void TimeAdd_idr(IMom[] par)
        {
            DateTime t = par[1].Date;
            var u = (TimeUnit)par[0].Integer;
            var p2 = par[2].Real;
            switch (u)
            {
                case TimeUnit.Second:
                    t = t.AddSeconds(p2);
                    break;
                case TimeUnit.MSec:
                    t = t.AddMilliseconds(p2);
                    break;
                case TimeUnit.Minute:
                    t = t.AddMinutes(p2);
                    break;
                case TimeUnit.Hour:
                    t = t.AddHours(p2);
                    break;
                case TimeUnit.Day:
                    t = t.AddDays(p2);
                    break;
                case TimeUnit.Month:
                    t = t.AddMonths(Convert.ToInt32(p2));
                    break;
                case TimeUnit.Year:
                    t = t.AddYears(Convert.ToInt32(p2));
                    break;
                default:
                    PutErr();
                    break;
            }
            ScalarRes.Date = t;
        }

        public void TimeDiff_idd(IMom[] par)
        {
            TimeSpan dif = par[1].Date.Subtract(par[2].Date);
            var u = (TimeUnit)par[0].Integer;
            switch (u)
            {
                case TimeUnit.Second:
                    ScalarRes.Real = dif.TotalSeconds;
                    break;
                case TimeUnit.MSec:
                    ScalarRes.Real = dif.TotalMilliseconds;
                    break;
                case TimeUnit.Minute:
                    ScalarRes.Real = dif.TotalMinutes;
                    break;
                case TimeUnit.Hour:
                    ScalarRes.Real = dif.TotalHours;
                    break;
                case TimeUnit.Day:
                    ScalarRes.Real = dif.TotalDays;
                    break;
                default:
                    PutErr();
                    break;
            }
        }

        public void TimePart_id(IMom[] par)
        {
            DateTime t = par[1].Date;
            var u = (TimeUnit)par[0].Integer;
            switch (u)
            {
                case TimeUnit.Second:
                    ScalarRes.Integer = t.Second;
                    break;
                case TimeUnit.MSec:
                    ScalarRes.Integer = t.Millisecond;
                    break;
                case TimeUnit.Minute:
                    ScalarRes.Integer = t.Minute;
                    break;
                case TimeUnit.Hour:
                    ScalarRes.Integer = t.Hour;
                    break;
                case TimeUnit.Day:
                    ScalarRes.Integer = t.Day;
                    break;
                case TimeUnit.Month:
                    ScalarRes.Integer = t.Month;
                    break;
                case TimeUnit.Year:
                    ScalarRes.Integer = t.Year;
                    break;
                default:
                    PutErr();
                    break;
            }
        }

        public void TimeSerial_iiiiiii(IMom[] par)
        {
            int p1 = par.Length > 1 ? par[1].Integer : 0;
            int p2 = par.Length > 1 ? par[2].Integer : 0;
            int p3 = par.Length > 1 ? par[3].Integer : 0;
            int p4 = par.Length > 1 ? par[4].Integer : 0;
            int p5 = par.Length > 1 ? par[5].Integer : 0;
            int p6 = par.Length > 1 ? par[6].Integer : 0;
            ScalarRes.Date = new DateTime(par[0].Integer, p1, p2, p3, p4, p5, p6);
        }

        public IVal Now_()
        {
            return new MeanTime(DateTime.Now);
        }
        #endregion

        //10 - Строковые 
        #region
        public IVal NewLine_()
        {
            return new MeanString(Environment.NewLine);
        }

        public void StrMid_si(IMom[] par)
        {
            int m1 = par[1].Integer - 1;
            string m0 = par[0].String;
            if (m1 < 0 || m1 >= m0.Length) PutErr();
            else ScalarRes.String = m0.Substring(m1);
        }

        public void StrMid_sii(IMom[] par)
        {
            int m1 = par[1].Integer - 1, m2 = par[2].Integer;
            string m0 = par[0].String;
            if (m1 < 0 || m1 + m2 > m0.Length || m2 < 0) PutErr();
            else ScalarRes.String = m0.Substring(m1, m2);
        }

        public void StrLeft_si(IMom[] par)
        {
            int m1 = par[1].Integer;
            string m0 = par[0].String;
            if (m1 < 0 || m1 > m0.Length) PutErr();
            else ScalarRes.String = m0.Substring(0, m1);
        }

        public void StrRight_si(IMom[] par)
        {
            int m1 = par[1].Integer;
            string m0 = par[0].String;
            if (m1 < 0 || m1 > m0.Length) PutErr();
            else ScalarRes.String = m0.Substring(m0.Length - m1, m1);
        }

        public void StrLen_s(IMom[] par)
        {
            ScalarRes.Integer = par[0].String.Length;
        }

        public void StrInsert_ssi(IMom[] par)
        {
            string m0 = par[0].String;
            string m1 = par[1].String;
            int m2 = par[2].Integer - 1;
            if (m2 < 0 || m2 >= m0.Length) PutErr();
            else ScalarRes.String = m0.Insert(m2, m1);
        }

        public void StrRemove_si(IMom[] par)
        {
            string m0 = par[0].String;
            int m1 = par[1].Integer - 1;
            if (m1 < 0 || m1 >= m0.Length) PutErr();
            else ScalarRes.String = m0.Remove(m1);
        }

        public void StrRemove_sii(IMom[] par)
        {
            string m0 = par[0].String;
            int m1 = par[1].Integer - 1;
            int m2 = par[2].Integer;
            if (m1 < 0 || m1 + m2 > m0.Length || m2 < 0) PutErr();
            else ScalarRes.String = m0.Remove(m1, m2);
        }

        public void StrReplace_sss(IMom[] par)
        {
            string m0 = par[0].String;
            string m1 = par[1].String;
            string m2 = par[2].String;
            ScalarRes.String = m0.Replace(m1, m2);
        }

        public void StrFind_ssi(IMom[] par)
        {
            int m2 = par.Length == 2 ? 0 : par[2].Integer - 1;
            string m0 = par[0].String;
            string m1 = par[1].String;
            if (m2 < 0 || m2 >= m1.Length) PutErr();
            else ScalarRes.Integer = m1.IndexOf(m0, m2) + 1;
        }

        public void StrFindLast_ssi(IMom[] par)
        {
            int m2 = par.Length == 2 ? -1 : par[2].Integer - 1;
            string m0 = par[0].String;
            string m1 = par[1].String;
            if (m2 < -1 || m2 >= m1.Length) PutErr();
            else ScalarRes.Integer = (m2 == -1) ? m1.LastIndexOf(m0) + 1 : m1.LastIndexOf(m0, m2) + 1;
        }

        public void StrTrim_s(IMom[] par)
        {
            ScalarRes.String = par[0].String.Trim();
        }

        public void StrLTrim_s(IMom[] par)
        {
            ScalarRes.String = par[0].String.TrimStart();
        }

        public void StrRTrim_s(IMom[] par)
        {
            ScalarRes.String = par[0].String.TrimEnd();
        }

        public void StrLCase_s(IMom[] par)
        {
            ScalarRes.String = par[0].String.ToLower();
        }

        public void StrUCase_s(IMom[] par)
        {
            ScalarRes.String = par[0].String.ToUpper();
        }

        public void StrRegReplace_sss(IMom[] par)
        {
            string m0 = par[0].String;
            var r1 = new Regex(par[1].String);
            string m2 = par[2].String;
            ScalarRes.String = r1.Replace(m0, m2);
        }

        public void StrRegFind_ss(IMom[] par)
        {
            string m0 = par[0].String;
            var r1 = new Regex(par[1].String);
            ScalarRes.Integer = r1.Match(m0).Index;
        }

        public void StrRegMatch_ss(IMom[] par)
        {
            string m0 = par[0].String;
            var r1 = new Regex("^" + par[1].String + "$");
            ScalarRes.Boolean = r1.Match(m0).Success;
        }
        #endregion
    }
}