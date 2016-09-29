using System;

namespace BaseLibrary
{
    public static class Crypting
    {
        private static string _alphabet;

        private static void _alphabetFill()
        {
            _alphabet = "";
            for (int i = 32; i <= 125; i++)
            {
                _alphabet += char.ConvertFromUtf32(i);
            }

            _alphabet += "Ё«»ёАБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдежзийклмнопрстуфхцчшщъыьэюя";
        }

        public static string Encrypt(string str)
        {
            _alphabetFill();
            Random random = new Random();
            string crypted = _alphabet[(int)(random.NextDouble() * 162)].ToString();
            for (int i = 1; i <= str.Length; i++)
            {
                int rnd = (int)(random.NextDouble() * 162);
                crypted += _alphabet[rnd].ToString() +
                           _alphabet[(rnd + _alphabet.IndexOf(str[i - 1]) + 1) % 162].ToString();
            }
            if (crypted.Length == 1)
            {
                crypted = "";
                int rnd = (int)(random.NextDouble() * 50);
                for (int j = 0; j < rnd * 2; j++)
                {
                    crypted += _alphabet[(int)(random.NextDouble() * 162)].ToString();
                }
            }
            return crypted;
        }

        public static string Decrypt(string str)
        {
            _alphabetFill();
            if (str.Length % 2 == 0) return "";
            string uncrypted = "";
            for (int i = 1; i < str.Length / 2 + 1; i++)
            {
                string ch = str[(i - 1) * 2 + 1].ToString();
                int rc = _alphabet.IndexOf(ch);
                ch = str[i * 2].ToString();
                int rs = _alphabet.IndexOf(ch);
                uncrypted += _alphabet[(rs - rc + 324 - 1) % 162];
            }
            return uncrypted;
        }
    }
}
