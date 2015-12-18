using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace BaseLibrary
{
    [ValueConversion(typeof(DateTime?), typeof(string))]
    public class DateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null) return null;
                return ((DateTime)value).ToString();    
            }
            catch { return null; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { 
            try
            {
                if (value == null) return null;
                return System.Convert.ToDateTime((string)value);
            }
            catch { return null; }
        }
    }

    //-------------------------------------------------------------------------------

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
            }
            catch 
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Visibility)value) == Visibility.Visible;
        }
    }

    //-------------------------------------------------------------------------------

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return ((bool)value) ? Visibility.Collapsed : Visibility.Visible;
            }
            catch
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Visibility)value) == Visibility.Visible;
        }
    }

    //-------------------------------------------------------------------------------

    [ValueConversion(typeof(bool), typeof(Brush))]
    public class BoolToBlackConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return ((bool)value) ? Brushes.Black : Brushes.Red;
            }
            catch
            {
                return Brushes.Red;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == Brushes.Black;
        }
    }
}