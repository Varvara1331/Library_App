using Library_App.Models;
using Library_App.ViewModels;
using System.Windows.Controls;

namespace Library_App.Views
{
    /// <summary>
    /// Логика взаимодействия для BooksView.xaml
    /// </summary>
    public partial class BooksView : UserControl
    {
        private BooksViewModel _viewModel;
        public BooksView(Frame frame)
        {
            InitializeComponent();
            _viewModel = new BooksViewModel(frame);
            this.DataContext = _viewModel;
        }

        public void RemoveSelectedBooks()
        {
            var selectedBooks = BooksDataGrid.SelectedItems.Cast<Book>().ToList();
            _viewModel.RemoveBook(selectedBooks);
        }

        public void ExportBooks()
        {
            _viewModel.ExportBooks();
        }
    }
}
