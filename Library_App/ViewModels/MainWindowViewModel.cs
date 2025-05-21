using Library_App.Models;
using Library_App.Views;
using System.Windows.Controls;
using System.Windows.Input;

namespace Library_App.ViewModels
{
    class MainWindowViewModel : BaseViewModel
    {
        private readonly Frame _navigationFrame;
        private readonly User _currentUser;

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ExportCommand { get; }

        public MainWindowViewModel(Frame navigationFrame, User user)
        {
            _navigationFrame = navigationFrame;
            _currentUser = user;
            AddCommand = new RelayCommand(AddItem, CanAddItem);
            DeleteCommand = new RelayCommand(DeleteItem, CanDeleteItem);
            ExportCommand = new RelayCommand(ExportItem);
        }

        private bool CanAddItem()
        {
            return _currentUser.IdRoleNavigation.NameRole != "Читатель";
        }

        private bool CanDeleteItem()
        {
            return _currentUser.IdRoleNavigation.NameRole != "Читатель";
        }

        private void AddItem()
        {
            if (_navigationFrame.Content is ReadersView)
            {
                _navigationFrame.Navigate(new ReaderView(null, _navigationFrame));
            }
            else if (_navigationFrame.Content is PriceListView)
            {
                _navigationFrame.Navigate(new ServiceView(null, _navigationFrame));
            }
            else if (_navigationFrame.Content is BooksView)
            {
                _navigationFrame.Navigate(new BookView(null, _navigationFrame));
            }
            else if (_navigationFrame.Content is PaymentsView)
            {
                _navigationFrame.Navigate(new PaymentView(null, _navigationFrame));
            }
            else if (_navigationFrame.Content is BookingsView)
            {
                _navigationFrame.Navigate(new BookingView(null, _navigationFrame));
            }
            else if (_navigationFrame.Content is ExhibitsView)
            {
                _navigationFrame.Navigate(new ExhibitView(null, _navigationFrame));
            }
            else if (_navigationFrame.Content is BookDistributionsView)
            {
                _navigationFrame.Navigate(new BookDistributionView(null, _navigationFrame));
            }
            else if (_navigationFrame.Content is EventsView)
            {
                _navigationFrame.Navigate(new EventView(null, _navigationFrame));
            }
        }

        private void DeleteItem()
        {
            if (_navigationFrame.Content is ReadersView readersView)
            {
                readersView.RemoveSelectedReaders();
            }
            else if (_navigationFrame.Content is PriceListView priceListView)
            {
                priceListView.RemoveSelectedServices();
            }
            else if (_navigationFrame.Content is BooksView booksView)
            {
                booksView.RemoveSelectedBooks();
            }
            else if (_navigationFrame.Content is PaymentsView paymentsView)
            {
                paymentsView.RemoveSelectedPayments();
            }
            else if (_navigationFrame.Content is BookingsView bookingsView)
            {
                bookingsView.RemoveSelectedBookings();
            }
            else if (_navigationFrame.Content is ExhibitsView exhibitsView)
            {
                exhibitsView.RemoveSelectedExhibits();
            }
            else if (_navigationFrame.Content is BookDistributionsView bookDistributionsView)
            {
                bookDistributionsView.RemoveSelectedBookDistributions();
            }
            else if (_navigationFrame.Content is EventsView eventsView)
            {
                eventsView.RemoveSelectedEvents();
            }
        }
        private void ExportItem()
        {
            if (_navigationFrame.Content is ReadersView readersView)
            {
                readersView.ExportReaders();
            }
            else if (_navigationFrame.Content is PriceListView priceListView)
            {
                priceListView.ExportPriceList();
            }
            else if (_navigationFrame.Content is BooksView booksView)
            {
                booksView.ExportBooks();
            }
            else if (_navigationFrame.Content is PaymentsView paymentsView)
            {
                paymentsView.ExportPayments();
            }
            else if (_navigationFrame.Content is BookingsView bookingsView)
            {
                bookingsView.ExportBookings();
            }
            else if (_navigationFrame.Content is ExhibitsView exhibitsView)
            {
                exhibitsView.ExportExhibits();
            }
            else if (_navigationFrame.Content is BookDistributionsView bookDistributionsView)
            {
                bookDistributionsView.ExportBookDistributions();
            }
            else if (_navigationFrame.Content is EventsView eventsView)
            {
                eventsView.ExportEvents();
            }
        }
    }
}
