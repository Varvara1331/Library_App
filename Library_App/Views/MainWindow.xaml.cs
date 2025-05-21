using Library_App.Views;
using System.Windows;
using Library_App.ViewModels;
using System.Windows.Controls;
using System.Windows.Navigation;
using Library_App.Models;

namespace Library_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;
        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            this.DataContext = new MainWindowViewModel(MainFrameTable, user);
            Welcome.Text = user.NameUser + "!";
            MainFrameTable.Navigated += MainFrameTable_Navigated;
            SetupUIForRole();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser.IdRoleNavigation.NameRole == "Администратор")
            {
                var registerWindow = new RegisterWindow();
                registerWindow.Owner = this;
                registerWindow.ShowDialog();
            }
        }

        private void SetupUIForRole()
        {
            switch (_currentUser.IdRoleNavigation.NameRole)
            {
                case "Читатель":
                    btnRegister.Visibility = Visibility.Collapsed;
                    btnAdd.Visibility = Visibility.Collapsed;
                    btnDel.Visibility = Visibility.Collapsed;
                    HideItems(new[] { "ЧИТАТЕЛИ", "ВЫДАЧА КНИГ", "БРОНИРОВАНИЕ КНИГ", "ОПЛАТА УСЛУГ" });
                    break;
                case "Библиотекарь":
                    btnRegister.Visibility = Visibility.Collapsed;
                    HideItems(new[] { "ЭКСПОНАТЫ", "МЕРОПРИЯТИЯ" });
                    break;
                case "Администратор":
                    btnRegister.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void HideItems(string[] itemsToHide)
        {
            foreach (var item in NavigationComboBox.Items.OfType<ComboBoxItem>().ToList())
            {
                if (itemsToHide.Contains(item.Content.ToString()))
                {
                    NavigationComboBox.Items.Remove(item);
                }
            }
        }

        private void MainFrameTable_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is ReaderView ||
                e.Content is BookView ||
                e.Content is ServiceView ||
                e.Content is PaymentView ||
                e.Content is BookingView ||
                e.Content is ExhibitView ||
                e.Content is BookDistributionView ||
                e.Content is EventView ||
                e.Content is null)
            {
                GridButton.Visibility = Visibility.Hidden;
            }
            else
            {
                GridButton.Visibility = Visibility.Visible;
            }
        }

        private void NavigationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainFrameTable == null || NavigationComboBox.SelectedItem == null)
                return;

            var selectedItem = (ComboBoxItem)NavigationComboBox.SelectedItem;
            string pageName = selectedItem.Content.ToString();

            switch (pageName)
            {
                case "ЧИТАТЕЛИ":
                    var readersView = new ReadersView(MainFrameTable);
                    readersView.DataContext = new ReadersViewModel(MainFrameTable);
                    MainFrameTable.Content = readersView;
                    break;
                case "КНИГИ":
                    var booksView = new BooksView(MainFrameTable);
                    booksView.DataContext = new BooksViewModel(MainFrameTable);
                    MainFrameTable.Content = booksView; break;
                case "ВЫДАЧА КНИГ":
                    var bookDistributionsView = new BookDistributionsView(MainFrameTable);
                    bookDistributionsView.DataContext = new BookDistributionsViewModel(MainFrameTable);
                    MainFrameTable.Content = bookDistributionsView; break;
                case "БРОНИРОВАНИЕ КНИГ":
                    var BookingsView = new BookingsView(MainFrameTable);
                    BookingsView.DataContext = new BookingsViewModel(MainFrameTable);
                    MainFrameTable.Content = BookingsView; break;
                case "ПРАЙС-ЛИСТ":
                    var priceListView = new PriceListView(MainFrameTable);
                    priceListView.DataContext = new PriceListViewModel(MainFrameTable);
                    MainFrameTable.Content = priceListView; break;
                case "ОПЛАТА УСЛУГ":
                    var paymentsView = new PaymentsView(MainFrameTable);
                    paymentsView.DataContext = new PaymentsViewModel(MainFrameTable);
                    MainFrameTable.Content = paymentsView; break;
                case "ВЫБОР ТАБЛИЦЫ":
                    GridButton.Visibility = Visibility.Hidden;
                    MainFrameTable.Content = null;
                    break;
                case "ЭКСПОНАТЫ":
                    var exhibitsView = new ExhibitsView(MainFrameTable);
                    exhibitsView.DataContext = new ExhibitsViewModel(MainFrameTable);
                    MainFrameTable.Content = exhibitsView; break;
                case "МЕРОПРИЯТИЯ":
                    var eventsView = new EventsView(MainFrameTable);
                    eventsView.DataContext = new EventsViewModel(MainFrameTable);
                    MainFrameTable.Content = eventsView; break;
            }
        }
    }
}