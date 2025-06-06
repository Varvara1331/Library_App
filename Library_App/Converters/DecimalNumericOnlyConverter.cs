using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Library_App.Converters
{
    class DecimalNumericOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return FilterToDigits(value as string);
        }

        private string FilterToDigits(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var result = new StringBuilder();
            foreach (char c in input)
            {
                if (char.IsDigit(c) || c == ',')
                {
                    result.Append(c);
                }
                else
                    MessageBox.Show($"Разрешен ввод только чисел с дробной частью, отделенной запятой!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return result.ToString();
        }
    }
}
