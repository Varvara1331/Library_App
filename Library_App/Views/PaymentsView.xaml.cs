using Library_App.Models;
using Library_App.ViewModels;
using System.Windows.Controls;

namespace Library_App.Views
{
    /// <summary>
    /// Логика взаимодействия для PaymentsView.xaml
    /// </summary>
    public partial class PaymentsView : UserControl
    {
        private PaymentsViewModel _viewModel;
        public PaymentsView(Frame frame)
        {
            InitializeComponent();
            _viewModel = new PaymentsViewModel(frame);
            this.DataContext = _viewModel;
        }

        public void RemoveSelectedPayments()
        {
            var selectedPayments = PaymentsDataGrid.SelectedItems.Cast<Payment>().ToList();
            _viewModel.RemovePayment(selectedPayments);
        }

        public void ExportPayments()
        {
            _viewModel.ExportPayments();
        }
    }
}
