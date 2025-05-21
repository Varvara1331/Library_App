using Library_App.Models;
using Library_App.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Library_App.Views
{
    /// <summary>
    /// Логика взаимодействия для EventView.xaml
    /// </summary>
    public partial class EventView : Page
    {
        private Event _selectedEvent;
        private Frame _frame;
        public EventView(Event _event, Frame frame)
        {
            InitializeComponent();
            _frame = frame;
            _selectedEvent = _event;
            this.DataContext = new AddEventViewModel(_selectedEvent);
        }
        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EventsView(_frame));
        }
    }
}
