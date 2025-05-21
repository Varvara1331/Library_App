using Library_App.Models;
using Library_App.ViewModels;
using System.Windows.Controls;

namespace Library_App.Views
{
    /// <summary>
    /// Логика взаимодействия для ReadersView.xaml
    /// </summary>
    public partial class ReadersView : UserControl
    {
        private ReadersViewModel _viewModel;
        public ReadersView(Frame frame)
        {
            InitializeComponent();
            _viewModel = new ReadersViewModel(frame);
            this.DataContext = _viewModel;
        }

        public void RemoveSelectedReaders()
        {
            var selectedReaders = ReadersDataGrid.SelectedItems.Cast<Reader>().ToList();
            _viewModel.RemoveReader(selectedReaders);
        }

        public void ExportReaders()
        {
            _viewModel.ExportReaders();
        }
    }
}