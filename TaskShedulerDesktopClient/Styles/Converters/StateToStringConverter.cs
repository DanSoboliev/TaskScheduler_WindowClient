using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TaskShedulerDesktopClient.Styles.Converters {
    public class StateToStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            bool? state;
            if (value == null) state = null;
            else state = System.Convert.ToBoolean(value);

            string result = "";
            switch (state) {
                case true:
                    result = "Виконано";
                    break;
                case false:
                    result = "Не виконано";
                    break;
                case null:
                    result = "Виконується";
                    break;
                default:
                    result = "Невідомо";
                    break;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return DependencyProperty.UnsetValue;
        }
    }
}