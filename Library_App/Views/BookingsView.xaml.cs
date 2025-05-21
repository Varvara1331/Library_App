using Library_App.Models;
using Library_App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Library_App.Views
{
    /// <summary>
    /// Логика взаимодействия для BookingsView.xaml
    /// </summary>
    public partial class BookingsView : UserControl
    {
        private BookingsViewModel _viewModel;
        public BookingsView(Frame frame)
        {
            InitializeComponent();
            _viewModel = new BookingsViewModel(frame);
            this.DataContext = _viewModel;
        }

        public void RemoveSelectedBookings()
        {
            var selectedBookings = BookingsDataGrid.SelectedItems.Cast<Booking>().ToList();
            _viewModel.RemoveBooking(selectedBookings);
        }

        public void ExportBookings()
        {
            _viewModel.ExportBookings();
        }
    }
}
