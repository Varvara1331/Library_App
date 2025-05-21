using Library_App.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Library_App.Services
{
    public static class AuthService
    {
        private static User _currentUser;
        private static event Action _onCurrentUserChanged;

        public static event Action OnCurrentUserChanged
        {
            add => _onCurrentUserChanged += value;
            remove => _onCurrentUserChanged -= value;
        }

        public static User CurrentUser
        {
            get => _currentUser;
            private set
            {
                _currentUser = value;
                _onCurrentUserChanged?.Invoke();
            }
        }

        public static void Login(User user)
        {
            CurrentUser = user;
        }

        public static bool IsReader => CurrentUser?.IdRoleNavigation?.NameRole == "Читатель";
    }

    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = false;

            if (value is bool)
                boolValue = (bool)value;
            else if (value is bool?)
                boolValue = ((bool?)value).GetValueOrDefault();

            bool inverse = parameter?.ToString().ToLower() == "inverse";
            bool useHidden = parameter?.ToString().ToLower() == "hidden";

            if (inverse)
                boolValue = !boolValue;

            return boolValue
                ? Visibility.Visible
                : (useHidden ? Visibility.Hidden : Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                bool result = visibility == Visibility.Visible;
                if (parameter?.ToString().ToLower() == "inverse")
                    result = !result;
                return result;
            }
            return false;
        }
    }
}
