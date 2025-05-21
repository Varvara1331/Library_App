using Library_App.Models;
using Library_App.ViewModels;
using System.Windows.Controls;

namespace Library_App.Views
{
    /// <summary>
    /// Логика взаимодействия для BookDistributionsView.xaml
    /// </summary>
    public partial class BookDistributionsView : UserControl
    {
        private BookDistributionsViewModel _viewModel;
        public BookDistributionsView(Frame frame)
        {
            InitializeComponent();
            _viewModel = new BookDistributionsViewModel(frame);
            this.DataContext = _viewModel;
        }

        public void RemoveSelectedBookDistributions()
        {
            var selectedBookDistributions = BookDistributionsDataGrid.SelectedItems.Cast<BookDistribution>().ToList();
            _viewModel.RemoveBookDistribution(selectedBookDistributions);
        }

        public void ExportBookDistributions()
        {
            _viewModel.ExportBookDistributions();
        }
    }
}
