using Library_App.Models;
using Library_App.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Library_App.Views
{
    /// <summary>
    /// Логика взаимодействия для ServiceView.xaml
    /// </summary>
    public partial class ServiceView : Page
    {
        private PriceList _selectedService;
        private Frame _frame;
        public ServiceView(PriceList service, Frame frame)
        {
            InitializeComponent();
            _frame = frame;
            _selectedService = service;
            this.DataContext = new AddServiceViewModel(_selectedService);
        }
        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PriceListView(_frame));
        }
    }
}
