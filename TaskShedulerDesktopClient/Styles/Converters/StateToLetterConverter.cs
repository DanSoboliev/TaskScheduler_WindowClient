using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TaskShedulerDesktopClient.Styles.Converters {
    class StateToLetterConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            bool? state;
            if (value == null) state = null;
            else state = System.Convert.ToBoolean(value);

            string result = "";
            switch (state) {
                case true:
                    result = "З";
                    break;
                case false:
                    result = "Н";
                    break;
                case null:
                    result = "В";
                    break;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return DependencyProperty.UnsetValue;
        }
    }
}