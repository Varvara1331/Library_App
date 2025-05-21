using Library_App.Models;
using Library_App.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Library_App.Views
{
    /// <summary>
    /// Логика взаимодействия для PaymentView.xaml
    /// </summary>
    public partial class PaymentView : Page
    {
        private Payment _selectedPayment;
        private Frame _frame;
        public PaymentView(Payment payment, Frame frame)
        {
            InitializeComponent();
            _frame = frame;
            _selectedPayment = payment;
            this.DataContext = new AddPaymentViewModel(_selectedPayment);
        }
        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PaymentsView(_frame));
        }
    }
}
