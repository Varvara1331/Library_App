using Library_App.Models;
using Library_App.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Library_App.Views
{
    /// <summary>
    /// Логика взаимодействия для BookDistributionView.xaml
    /// </summary>
    public partial class BookDistributionView : Page
    {
        private BookDistribution _selectedBookDistribution;
        private Frame _frame;
        public BookDistributionView(BookDistribution bookDistribution, Frame frame)
        {
            InitializeComponent();
            _frame = frame;
            _selectedBookDistribution = bookDistribution;
            this.DataContext = new AddBookDistributionViewModel(_selectedBookDistribution);
        }
        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new BookDistributionsView(_frame));
        }
    }
}
