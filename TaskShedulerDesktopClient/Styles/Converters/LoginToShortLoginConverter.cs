using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TaskShedulerDesktopClient.Styles.Converters {
    class LoginToShortLoginConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            string login = value.ToString();
            if (login.Length > 14) login = login.Substring(0, 13) + "...";
            return login;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return DependencyProperty.UnsetValue;
        }
    }
}