using System.Windows;
using System.Linq;
using Library_App.Models;
using System.Text.RegularExpressions;
using Library_App.Data;

namespace Library_App
{
    public partial class RegisterWindow : Window
    {
        private readonly LibraryContext _context;

        public RegisterWindow()
        {
            InitializeComponent();
            _context = new LibraryContext();

            cmbRole.ItemsSource = _context.Roles.ToList();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                MessageBox.Show("Введите логин пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (txtPassword.Password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var password = txtPassword.Password;
            if (!IsValidPassword(password))
            {
                MessageBox.Show("Пароль должен содержать:\n- Только латинские буквы\n- Минимум 1 прописную букву\n- Минимум 1 цифру\n- Минимум один специальный символ из набора: ! @ # $ % ^ .", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (txtPassword.Password != txtConfirmPassword.Password)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите ФИО пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (cmbRole.SelectedItem == null)
            {
                MessageBox.Show("Выберите роль пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_context.Users.Any(u => u.LoginUser == txtLogin.Text))
            {
                MessageBox.Show("Пользователь с таким логином уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newUser = new User
            {
                LoginUser = txtLogin.Text,
                PasswordUser = txtPassword.Password,
                NameUser = txtName.Text,
                IdRole = ((Role)cmbRole.SelectedItem).IdRole
            };

            try
            {
                _context.Users.Add(newUser);
                _context.SaveChanges();

                MessageBox.Show("Пользователь успешно зарегистрирован", "Регистрация", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch
            {
                MessageBox.Show("Произошла ошибка при сохранении пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsValidPassword(string password)
        {
            var regex = new Regex(@"^(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^.]).+$");
            return regex.IsMatch(password);
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}