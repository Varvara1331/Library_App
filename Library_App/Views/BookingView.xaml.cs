using Library_App.Models;
using Library_App.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Library_App.Views
{
    /// <summary>
    /// Логика взаимодействия для BookingView.xaml
    /// </summary>
    public partial class BookingView : Page
    {
        private Booking _selectedBooking;
        private Frame _frame;
        public BookingView(Booking booking, Frame frame)
        {
            InitializeComponent();
            _frame = frame;
            _selectedBooking = booking;
            this.DataContext = new AddBookingViewModel(_selectedBooking);
        }
        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new BookingsView(_frame));
        }
    }
}
