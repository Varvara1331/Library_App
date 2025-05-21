using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using Library_App.Models;
using Library_App.Data;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;
using Library_App.Views;
using ClosedXML.Excel;
using System.Diagnostics;

namespace Library_App.ViewModels
{
    public class BookingsViewModel : BaseViewModel
    {
        private readonly Frame _navigationFrame;
        private readonly LibraryContext _context;
        private ObservableCollection<Booking> _bookings;
        private Booking _selectedBooking;

        public ObservableCollection<Booking> Bookings
        {
            get => _bookings;
            set { _bookings = value; OnPropertyChanged(); }
        }

        public Booking SelectedBooking
        {
            get => _selectedBooking;
            set { _selectedBooking = value; OnPropertyChanged(); }
        }

        public ICommand LoadBookingsCommand { get; }
        public ICommand EditBookingCommand { get; }

        public BookingsViewModel(Frame frame)
        {
            _navigationFrame = frame;
            _context = new LibraryContext();

            LoadBookingsCommand = new RelayCommand(LoadBookings);
            EditBookingCommand = new RelayCommand(EditBooking);

            LoadBookings();
        }

        public async void LoadBookings()
        {
            try
            {
                var bookings = await _context.Bookings
            .Include(b => b.BookingBooks)
            .ThenInclude(bb => bb.IdBookNavigation)
            .AsNoTracking()
            .ToListAsync();

                Bookings = new ObservableCollection<Booking>(bookings);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditBooking()
        {
            _navigationFrame.Navigate(new BookingView(SelectedBooking, _navigationFrame));
        }

        public void RemoveBooking(List<Booking> bookings)
        {
            if (bookings.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранные записи?",
            "Подтверждение удаления",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new LibraryContext())
                    {
                        foreach (var booking in bookings)
                        {
                            foreach (var bookingBook in booking.BookingBooks)
                            {
                                context.BookingBooks.Remove(bookingBook);
                            }
                            context.Bookings.Remove(booking);
                            context.SaveChanges();
                        }
                        LoadBookings();
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите записи о бронировании для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void ExportBookings()
        {
            if (Bookings == null || !Bookings.Any())
            {
                MessageBox.Show("Нет данных для экспорта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var workbook = new XLWorkbook())
            {
                var bookingsWorksheet = workbook.Worksheets.Add("Booking");

                bookingsWorksheet.Cell(1, 1).Value = "ID бронирования";
                bookingsWorksheet.Cell(1, 2).Value = "Дата бронирования";
                bookingsWorksheet.Cell(1, 3).Value = "№ читательского билета";

                for (int i = 0; i < Bookings.Count; i++)
                {
                    var booking = Bookings[i];
                    bookingsWorksheet.Cell(i + 2, 1).Value = booking.IdBooking;
                    bookingsWorksheet.Cell(i + 2, 2).Value = booking.BookingDate.ToString("dd.MM.yyyy");
                    bookingsWorksheet.Cell(i + 2, 3).Value = booking.ReaderTicket;
                }

                var bookingBooksWorksheet = workbook.Worksheets.Add("BookingBook");

                bookingBooksWorksheet.Cell(1, 1).Value = "ID бронирования";
                bookingBooksWorksheet.Cell(1, 2).Value = "ID книги";
                bookingBooksWorksheet.Cell(1, 3).Value = "Название книги";
                bookingBooksWorksheet.Cell(1, 4).Value = "Автор";
                bookingBooksWorksheet.Cell(1, 5).Value = "Год издания";

                int row = 2;
                foreach (var booking in Bookings)
                {
                    foreach (var bookingBook in booking.BookingBooks)
                    {
                        bookingBooksWorksheet.Cell(row, 1).Value = bookingBook.IdBooking;
                        bookingBooksWorksheet.Cell(row, 2).Value = bookingBook.IdBook;

                        if (bookingBook.IdBookNavigation != null)
                        {
                            bookingBooksWorksheet.Cell(row, 3).Value = bookingBook.IdBookNavigation.TitleBook;
                            bookingBooksWorksheet.Cell(row, 4).Value = bookingBook.IdBookNavigation.AuthorBook;
                            bookingBooksWorksheet.Cell(row, 5).Value = bookingBook.IdBookNavigation.YearOfPublication;
                        }
                        row++;
                    }
                }

                FormatWorksheet(bookingsWorksheet);
                FormatWorksheet(bookingBooksWorksheet);

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = "Бронирование",
                    DefaultExt = ".xlsx",
                    Filter = "Excel Documents (.xlsx)|*.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;
                    workbook.SaveAs(filePath);

                    Process.Start(new ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true
                    });

                    MessageBox.Show("Данные успешно экспортированы в Excel.", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void FormatWorksheet(IXLWorksheet worksheet)
        {
            var headerRange = worksheet.Range(1, 1, 1, worksheet.Columns().Count());
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var dataRange = worksheet.RangeUsed();
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            worksheet.Columns().AdjustToContents();
        }
    }
}