using Library_App.Models;
using Library_App.ViewModels;
using System.Windows.Controls;

namespace Library_App.Views
{
    /// <summary>
    /// Логика взаимодействия для EventsView.xaml
    /// </summary>
    public partial class EventsView : UserControl
    {
        private EventsViewModel _viewModel;
        public EventsView(Frame frame)
        {
            InitializeComponent();
            _viewModel = new EventsViewModel(frame);
            this.DataContext = _viewModel;
        }

        public void RemoveSelectedEvents()
        {
            var selectedEvents = EventsDataGrid.SelectedItems.Cast<Event>().ToList();
            _viewModel.RemoveEvent(selectedEvents);
        }

        public void ExportEvents()
        {
            _viewModel.ExportEvents();
        }
    }
}
