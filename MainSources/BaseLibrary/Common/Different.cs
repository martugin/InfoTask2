using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Win32;
using System.Globalization;

namespace BaseLibrary
{
    //Разные хорошие внешние методы
    public static class Different
    {
        //Минимальное значение даты
        private static readonly DateTime _minDate = new DateTime(1980, 1, 1, 0, 0, 0);
        public static DateTime MinDate { get { return _minDate; } }

        //Максимальное значение даты
        private static readonly DateTime _maxDate = new DateTime(2200, 1, 1, 0, 0, 0);
        public static DateTime MaxDate { get { return _maxDate; } }

        //Сокращает запись string.IsNullOrEmpty
        public static bool IsEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }
        //Сокращает запись string.IsNullOrWhiteSpace
        public static bool IsWhiteSpace(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        //Преводит дату в формат для запросов Access
        public static string ToAccessString(this DateTime d)
        {
            var culture = new CultureInfo("");
            return "#" + Convert.ToString(d, culture) + "#";
        }

        //Преводит дату в формат для запросов SQL Server
        public static string ToSqlString(this DateTime d)
        {
            return "convert(datetime, '" + d.Year + "-" + d.Month + "-" + d.Day + " " + d.Hour + ":" + d.Minute + ":" + d.Second + "." + d.Millisecond + "', 21)";
        }

        //Переводит время в строку с милисекундами
        public static string ToStringWithMs(this DateTime t)
        {
            return t.ToString() + "," + (1000 + t.Millisecond).ToString().Substring(1);
        }

        //Сравнивает даты с точностью до секунды
        public static bool EqulasToSeconds(this DateTime t1, DateTime t2)
        {
            return Math.Abs(t1.Subtract(t2).TotalSeconds) < 0.5;
        }

        //Сравнивает даты с точностью до милисекунды
        public static bool EqulasToMilliSeconds(this DateTime t1, DateTime t2)
        {
            return Math.Abs(t1.Subtract(t2).TotalMilliseconds) < 0.5;
        }

        //Вычитает из даты другую дату, результат в секундах
        public static double Minus(this DateTime t1, DateTime t2)
        {
            return t1.Subtract(t2).TotalSeconds;
        }

        //Перведит строку в bool
        public static bool ToBool(this string s)
        {
            return s.ToUpper() == "TRUE";
        }

        //Переводит строку в double, если нельзя, то возвращает def
        public static double ToDouble(this string s, double def = 0.0)
        {
            if (s.IsEmpty()) return def;
            string st = s.Replace(",", ".");
            double d;
            double res = Double.TryParse(st, NumberStyles.Any, new NumberFormatInfo { NumberDecimalSeparator = "." }, out d) ? d : def;
            return res;
        }

        //Переводит строку в int, если нельзя, то возвращает def 
        public static int ToInt(this string s, int def = 0)
        {
            if (s.IsEmpty()) return def;
            int i;
            return int.TryParse(s, out i) ? i : 0;
        }

        //Переводит строку в DateTime, если нельзя, то возвращает Different.MinDate
        public static DateTime ToDateTime(this string s)
        {
            DateTime t;
            if (DateTime.TryParse(s, out t)) return t;
            return MinDate;
        }

        //Возвращает истинность i-ого бита числа n
        public static bool GetBit(this int n, int i)
        {
            return ((n >> i) & 1) == 1;
        }
        public static bool GetBit(this uint n, int i)
        {
            return ((n >> i) & 1) == 1;
        }

        //Получение строкового значения из ячейки DataGridView, cells - набор ячеек, column - имя колонки
        public static string Get(this DataGridViewCellCollection cells, string column)
        {
            var v = cells[column].Value;
            if (DBNull.Value.Equals(v) || v == null) return null;
            return v.ToString();
        }
        public static string Get(this DataGridViewRow row, string column)
        {
            return row.Cells.Get(column);
        }

        //Получение логического значения из ячейки DataGridView, cells - набор ячеек, column - имя колонки
        public static bool GetBool(this DataGridViewCellCollection cells, string column)
        {
            var v = cells[column].Value;
            if (DBNull.Value.Equals(v) || v == null) return false;
            return (bool)v;
        }
        public static bool GetBool(this DataGridViewRow row, string column)
        {
            return row.Cells.GetBool(column);
        }

        //Выводит информационное сообщение
        public static void MessageInfo(string message, string caption = "InfoTask", MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, icon);
        }

        //Выводит сообщение об ошибке
        public static string MessageError(string message, //Текст сообщения
                                                           string caption = "InfoTask", //Заголовок окна сообщения
                                                           MessageBoxIcon icon = MessageBoxIcon.Error) //Иконка
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, icon);
            return message;
        }

        //Выводит вопросительное сообщение, возвращает True, если нажали Yes
        public static bool MessageQuestion(string message, string caption = "InfoTask")
        {
            return MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        //Возвращает строку - сообщение об ошибке
        public static string MessageString(this Exception ex, string message = null)
        {
            var sb = new StringBuilder();
            sb.Append(message.IsEmpty() ? "" : (message + Environment.NewLine))
              .Append(ex.GetType())
              .Append(Environment.NewLine)
              .Append(ex.Message)
              .Append(Environment.NewLine)
              .Append(ex.StackTrace)
              .Append(Environment.NewLine)
              .Append(ex.Data);
            return sb.ToString();
        }

        //Выводит сообщение об ошибке 
        public static string MessageError(this Exception ex, //Исключение, вызвашее ошибку
                                                           string message = null, //Текст сообщения
                                                           string caption = "InfoTask", //Заголовок окна сообщения
                                                           MessageBoxIcon icon = MessageBoxIcon.Error) //Иконка
        {
            var mess = ex.MessageString(message);
            MessageBox.Show(mess, caption, MessageBoxButtons.OK, icon);
            return mess;
        }

        //Заменяет в регулярном выражении все символы на коды, чтобы избежать символов типа . или \
        //Если saveAsterix - то не заменятет * и ?
        private static string EscapeCharacters(this string s, bool saveAsterix = false)
        {
            if (s.IsEmpty())
                return String.Empty;

            var builder = new StringBuilder(s.Length * 6);
            foreach (var c in s)
                if (!saveAsterix || (c != '*' && c != '?'))
                    builder.Append("\\u").Append(((int) c).ToString("X4"));
                else builder.Append(c);
            return builder.ToString();
        }

        //Проверка, удовлетворяет ли поле заданному условию, сравение производится без учета регистра
        //fieldValue - значение поля, filterValue - строка фильтра, может содержать * и ?
        //Если не содержит * или ?, то фильтр устанавливается как *filterValue*
        //Можно использовать ключевое слово <Все> для фильтра всех
        //nullIsTrue - считать, что не заполненное поле фильтра означает отсутствие фильтра
        public static bool CheckFilter(this string fieldValue, string filterValue, bool nullIsTrue = true)
        {
            bool b = filterValue == "<Все>";
            if (nullIsTrue) b |= filterValue.IsEmpty();
            string fv = filterValue.EscapeCharacters(true);
            if (fv.Contains("*") || fv.Contains("?"))
                fv = fv.Replace("*", @".*").Replace("?", @".");
            fv = "^" + fv + "$";
            b |= Regex.IsMatch(fieldValue ?? "", fv, RegexOptions.IgnoreCase);
            return b;
        }

        //Чтение значения из реестра
        public static string GetRegistry(string path, //path - путь к папке свойства
                                                        string par) //par - имя свойства
        {
            try
            {
                RegistryKey readKey = Registry.LocalMachine.OpenSubKey(path);
                if (readKey == null) return "";
                var res = (string)readKey.GetValue(par);
                readKey.Close();
                return res;
            }
            catch { return ""; }
        }

        //Открытие документа при помощи стандартной программы
        public static void OpenDocument(string file)
        {
            var fi = new FileInfo(file);
            if (!fi.Exists)
            {
                MessageError("Не найден файл " + file);
                return;
            }
            if (fi.Extension.IsEmpty())
            {
                MessageError("Файл " + file + " не имеет расширения");
                return;
            }
            try
            {
                Process.Start(file);
            }
            catch
            {
                MessageError("Не удалось открыть файл " + file + ". Возможно, на компьютере не установлено приложение для просмотра файлов типа " + fi.Extension);
            }
        }

        //Копирование каталога, файлы заменяются на новые
        public static void CopyDir(string fromDir, string toDir)
        {
            var d = new DirectoryInfo(fromDir);
            if (Directory.Exists(toDir) != true)
                Directory.CreateDirectory(toDir);
            foreach (DirectoryInfo dir in d.GetDirectories())
                CopyDir(dir.FullName, toDir + "\\" + dir.Name);
            foreach (string file in Directory.GetFiles(fromDir))
                File.Copy(file, toDir + "\\" + file.Substring(file.LastIndexOf('\\') + 1), true);
        }

        //Заполняет значение контрола pick, по значению поля text
        public static void ChangePickerValue(this TextBox text, DateTimePicker pick)
        {
            try
            {
                var d = text.Text.ToDateTime();
                if (d != MinDate) pick.Value = d;
            }
            catch { }
        }

        //Работа с XML
        #region
        //Получение значения атрибута узла
        public static string GetAttr(this XElement node, string attributeName, string defaultValue = null)
        {
            if (node == null || node.Attribute(attributeName) == null)
                return defaultValue;
            return node.Attribute(attributeName).Value;
        }
        //Получение имени узла
        public static string GetName(this XElement node)
        {
            return node.Name.ToString();
        }

        #endregion
    }
}
