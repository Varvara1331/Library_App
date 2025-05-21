using Library_App.Models;
using Library_App.ViewModels;
using System.Windows.Controls;

namespace Library_App.Views
{
    /// <summary>
    /// Логика взаимодействия для PriceListView.xaml
    /// </summary>
    public partial class PriceListView : UserControl
    {
        private PriceListViewModel _viewModel;
        public PriceListView(Frame frame)
        {
            InitializeComponent();
            _viewModel = new PriceListViewModel(frame);
            this.DataContext = _viewModel;
        }

        public void RemoveSelectedServices()
        {
            var selectedServices = PriceListsDataGrid.SelectedItems.Cast<PriceList>().ToList();
            _viewModel.RemoveService(selectedServices);
        }

        public void ExportPriceList()
        {
            _viewModel.ExportPriceList();
        }
    }
}
