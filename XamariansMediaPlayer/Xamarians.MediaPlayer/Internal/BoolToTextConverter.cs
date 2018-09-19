using System;
using System.Globalization;
using Xamarin.Forms;

namespace Xamarians.MediaPlayers
{
    internal class BoolToTextConverter : IValueConverter
    {
        public string TrueValue { get; set; }
        public string FalseValue { get; set; }

        public BoolToTextConverter(string trueValue,string falseValue)
        {
            TrueValue = trueValue;
            FalseValue = falseValue;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "true".Equals(value?.ToString(), StringComparison.OrdinalIgnoreCase) ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TrueValue?.Equals(value?.ToString(), StringComparison.OrdinalIgnoreCase) ?? false;
            //return "true".Equals(value?.ToString().ToLower()) ? TrueValue : FalseValue;
        }
    }
}
