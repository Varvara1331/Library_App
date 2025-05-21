using Library_App.Models;
using Library_App.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Library_App.Views
{
    /// <summary>
    /// Логика взаимодействия для BookView.xaml
    /// </summary>
    public partial class BookView : Page
    {
        private Book _selectedBook;
        private Frame _frame;
        public BookView(Book book, Frame frame)
        {
            InitializeComponent();
            _frame = frame;
            _selectedBook = book;
            this.DataContext = new AddBookViewModel(_selectedBook);
        }
        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new BooksView(_frame));
        }
    }
}
