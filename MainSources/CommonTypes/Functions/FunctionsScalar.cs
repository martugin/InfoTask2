using System;
using System.Text.RegularExpressions;
using BaseLibrary;

namespace CommonTypes
{
    //Реализация скалярных функции
    public partial class Functions
    {
        //1 - Операции
        #region
        public void plusii(IMom[] par)
        {
            ScalarRes.Integer = par[0].Integer + par[1].Integer;
        }

        public void plusdr(IMom[] par)
        {
            ScalarRes.Date = par[0].Date.AddSeconds(par[1].Real);
        }

        public void plusrd(IMom[] par)
        {
            ScalarRes.Date = par[1].Date.AddSeconds(par[0].Real); 
        }

        public void plusrr(IMom[] par)
        {
            ScalarRes.Real = par[0].Real + par[1].Real;
        }

        public void plusss(IMom[] par)
        {
            ScalarRes.String = par[0].String + par[1].String;
        }

        public void minusii(IMom[] par)
        {
            ScalarRes.Integer = par[0].Integer - par[1].Integer;
        }

        public void minusdd(IMom[] par)
        {
            ScalarRes.Real = par[0].Date.Subtract(par[1].Date).TotalSeconds;
        }

        public void minusdr(IMom[] par)
        {
            ScalarRes.Date = par[0].Date.AddSeconds(-par[1].Real); 
        }

        public void minusrr(IMom[] par)
        {
            ScalarRes.Real = par[0].Real - par[1].Real;
        }

        public void minusi(IMom[] par)
        {
            ScalarRes.Integer = -par[0].Integer;
        }

        public void minusr(IMom[] par)
        {
            ScalarRes.Real = -par[0].Real;
        }

        public void multiplyii(IMom[] par)
        {
            ScalarRes.Integer = par[0].Integer * par[1].Integer;
            if (par[0].Integer == 0)
                ScalarRes.Error = par[1].Integer == 0 ? MinErr(par) : par[0].Error;
            else if (par[1].Integer == 0)
                ScalarRes.Error = par[1].Error;
        }

        public void multiplyrr(IMom[] par)
        {
            ScalarRes.Real = par[0].Real * par[1].Real;
            if (par[0].Real == 0)
                ScalarRes.Error = par[1].Real == 0 ? MinErr(par) : par[0].Error;
            else if (par[1].Real == 0)
                ScalarRes.Error = par[1].Error;
        }

        public void dividerr(IMom[] par)
        {
            if (par[0].Real == 0)
                ScalarRes.Error = par[0].Error;
            if (par[1].Real == 0) PutErr("Деление на 0");
            else ScalarRes.Real = par[0].Real / par[1].Real;
        }

        public void divii(IMom[] par)
        {
            if (par[0].Integer == 0)
                ScalarRes.Error = par[0].Error;
            if (par[1].Integer == 0) PutErr("Деление на 0");
            else ScalarRes.Integer = par[0].Integer / par[1].Integer;
        }

        public void modii(IMom[] par)
        {
            if (par[0].Integer == 0)
                ScalarRes.Error = par[0].Error;
            if (par[1].Integer == 0) PutErr("Деление с остатком на 0");
            else ScalarRes.Integer = par[0].Integer % par[1].Integer;
        }

        public void powerrr(IMom[] par)
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

        public void equaluu(IMom[] par)
        {
            ScalarRes.Boolean = par[0].ValueEquals(par[1]);
        }

        public void notequaluu(IMom[] par)
        {
            ScalarRes.Boolean = !par[0].ValueEquals(par[1]);
        }

        public void lessuu(IMom[] par)
        {
            ScalarRes.Boolean = par[0].ValueLess(par[1]);
        }

        public void lessequaluu(IMom[] par)
        {
            ScalarRes.Boolean = par[0].ValueLess(par[1]) || par[0].ValueEquals(par[1]);
        }

        public void greateruu(IMom[] par)
        {
            ScalarRes.Boolean = !par[0].ValueLess(par[1]) && !par[0].ValueEquals(par[1]);
        }

        public void greaterequaluu(IMom[] par)
        {
            ScalarRes.Boolean = !par[0].ValueLess(par[1]);
        }

        public void notb(IMom[] par)
        {
            ScalarRes.Boolean = !par[0].Boolean;
        }

        public void xorbb(IMom[] par)
        {
            ScalarRes.Boolean = par[0].Boolean ^ par[1].Boolean;
        }

        public void xorii(IMom[] par)
        {
            ScalarRes.Integer = par[0].Integer ^ par[1].Integer;
        }

        public void orbb(IMom[] par)
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

        public void orii(IMom[] par)
        {
            ScalarRes.Integer = par[0].Integer | par[1].Integer;
        }

        public void andbb(IMom[] par)
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

        public void andii(IMom[] par)
        {
            ScalarRes.Integer = par[0].Integer & par[1].Integer;
        }

        public void likess(IMom[] par)
        {
            string m1 = par[1].String.Replace("*", @"[\S|\s]*").Replace("?", @"[\S|\s]");
            string m0 = par[0].String;
            bool mean = false;
            var r = new Regex(m1);
            if (r.IsMatch(m0))
            {
                Match m = r.Match(m0);
                if (m0.Length == m.Value.Length) mean = true;
            }
            ScalarRes.Boolean = mean;
        }
        #endregion

        //2 - Логические
        #region
        public IMean truefun()
        {
            return new MeanBool(true);
        }

        public IMean falsefun()
        {
            return new MeanBool(false);
        }

        private bool GetBit(IMom[] par, int num)
        {
            if (num < 0 || num > 31)
                PutErr("Номер бита должен быть от 0 до 31");
            return par[0].Integer.GetBit(par[num].Integer);
        }

        public void bitii(IMom[] par)
        {
            ScalarRes.Boolean = GetBit(par, 1);
        }

        public void bitandii(IMom[] par)
        {
            var b = true;
            for (int i = 1; i < par.Length; i++)
                b &= GetBit(par, i); 
            ScalarRes.Boolean = b;
        }

        public void bitorii(IMom[] par)
        {
            var b = true;
            for (int i = 1; i < par.Length; i++)
                b |= GetBit(par, i); 
            ScalarRes.Boolean = b;
        }

        public void srii(IMom[] par)
        {
            ScalarRes.Integer = par[0].Integer >> par[1].Integer;
        }

        public void slii(IMom[] par)
        {
            ScalarRes.Integer = par[0].Integer << par[1].Integer;
        }
        #endregion

        //3 - Математические
        #region
        public void random(IMom[] par)
        {
            ScalarRes.Real = new Random().NextDouble();
        }

        public void randomii(IMom[] par)
        {
            ScalarRes.Integer = new Random().Next(par[0].Integer, par[1].Integer);
        }

        public void absi(IMom[] par)
        {
            ScalarRes.Integer = Math.Abs(par[0].Integer);
        }

        public void absr(IMom[] par)
        {
            ScalarRes.Real = Math.Abs(par[0].Real);
        }

        public void signi(IMom[] par)
        {
            ScalarRes.Integer = Math.Sign(par[0].Integer);
        }

        public void signr(IMom[] par)
        {
            ScalarRes.Integer = Math.Sign(par[0].Real);
        }

        public void roundr(IMom[] par)
        {
            ScalarRes.Integer = Convert.ToInt32(par[0].Real);
        }

        public void roundri(IMom[] par)
        {
            ScalarRes.Real = Math.Round(par[0].Real, par[1].Integer);
        }

        public void minu(IMom[] par)
        {
            IMom mv = par[0];
            for (int i = 1; i < par.Length; ++i)
                if (par[i].ValueLess(mv)) mv = par[i];
            ScalarRes.CopyValueFrom(mv);
        }

        public void maxu(IMom[] par)
        {
            IMom mv = par[0];
            for (int i = 1; i < par.Length; ++i)
                if (mv.ValueLess(par[i])) mv = par[i];
            ScalarRes.CopyValueFrom(mv);
        }

        public IVal pi()
        {
            return new MeanReal(Math.PI);
        }

        public void sqrr(IMom[] par)
        {
            if (par[0].Real < 0) 
                PutErr("Извлечение корня из отрицательного числа");
            else ScalarRes.Real = Math.Sqrt(par[0].Real);
        }

        public void cosr(IMom[] par)
        {
            ScalarRes.Real = Math.Cos(par[0].Real);
        }

        public void sinr(IMom[] par)
        {
            ScalarRes.Real = Math.Sin(par[0].Real);
        }

        public void tanr(IMom[] par)
        {
            ScalarRes.Real = Math.Tan(par[0].Real);
        }

        public void ctanr(IMom[] par)
        {
            var tan = Math.Tan(par[0].Real);
            if (tan == 0) PutErr();
            ScalarRes.Real = 1 / tan;
        }

        public void arccosr(IMom[] par)
        {
            double d = par[0].Real;
            if (d < -1 || d > 1) PutErr();
            else ScalarRes.Real = Math.Acos(d);
        }

        public void arcsinr(IMom[] par)
        {
            double d = par[0].Real;
            if (d < -1 || d > 1) PutErr();
            else ScalarRes.Real = Math.Asin(d);
        }

        public void arctanr(IMom[] par)
        {
            ScalarRes.Real = Math.Atan(par[0].Real);
        }

        public void shr(IMom[] par)
        {
            ScalarRes.Real = Math.Sinh(par[0].Real);
        }

        public void chr(IMom[] par)
        {
            ScalarRes.Real = Math.Cosh(par[0].Real);
        }

        public void thr(IMom[] par)
        {
            ScalarRes.Real = Math.Tanh(par[0].Real);
        }

        public void arcshr(IMom[] par)
        {
            double d = par[0].Real;
            d = Math.Log(d + Math.Sqrt(d * d + 1));
            ScalarRes.Real = d;
        }

        public void arcchr(IMom[] par)
        {
            double d = par[0].Real;
            d = Math.Log(d + Math.Sqrt(d * d - 1));
            ScalarRes.Real = d;
        }

        public void arcthr(IMom[] par)
        {
            double d = par[0].Real;
            if (d == 1 || (1 + d) / (1 - d) <= 0)
                PutErr();
            else ScalarRes.Real = Math.Log((1 + d) / (1 - d)) / 2;
        }

        public void expr(IMom[] par)
        {
            ScalarRes.Real = Math.Exp(par[0].Real);
        }

        public void lnr(IMom[] par)
        {
            if (par[0].Real <= 0) 
                PutErr("Взятие логарифма от неположительного числа");
            else ScalarRes.Real = Math.Log(par[0].Real);
        }

        public void log10r(IMom[] par)
        {
            if (par[0].Real <= 0) 
                PutErr("Взятие логарифма от неположительного числа");
            else ScalarRes.Real = Math.Log10(par[0].Real);
        }

        public void logrr(IMom[] par)
        {
            double m0 = par[0].Real, m1 = par[1].Real;
            if (m0 == 1)
                ScalarRes.Error = par[0].Error;
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
    }
}