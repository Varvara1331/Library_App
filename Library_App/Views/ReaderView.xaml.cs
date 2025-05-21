using System.Windows;
using System.Windows.Controls;
using Library_App.ViewModels;
using System.Windows.Navigation;
using Library_App.Models;

namespace Library_App.Views
{
    /// <summary>
    /// Логика взаимодействия для ReaderView.xaml
    /// </summary>
    public partial class ReaderView : Page
    {
        private Reader _selectedReader;
        private Frame _frame;
        public ReaderView(Reader reader, Frame frame)
        {
            InitializeComponent();
            _frame = frame;
            _selectedReader = reader;
            this.DataContext = new AddReaderViewModel(_selectedReader);
        }
        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ReadersView(_frame));
        }
    }
}
