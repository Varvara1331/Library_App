using System.Windows;
using System.Linq;
using Library_App.Models;
using Microsoft.EntityFrameworkCore;
using Library_App.Data;
using Library_App.Services;

namespace Library_App
{
    public partial class LoginWindow : Window
    {
        private readonly LibraryContext _context;

        public LoginWindow()
        {
            InitializeComponent();
            _context = new LibraryContext();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = txtLogin.Text;
            var password = txtPassword.Password;

            var user = _context.Users
                .Include(u => u.IdRoleNavigation)
                .FirstOrDefault(u => u.LoginUser == login && u.PasswordUser == password);
            if (user != null)
            {
                AuthService.Login(user);
                var mainWindow = new MainWindow(user);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка входа. Проверьте правильность логина и пароля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}