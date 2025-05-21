using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Library_App.Models;
using Library_App.Data;
using Microsoft.EntityFrameworkCore;

namespace Library_App.ViewModels
{
    public class AddBookingViewModel : BaseViewModel
    {
        private readonly LibraryContext _context;
        private Booking _selectedBooking;

        public ObservableCollection<Booking> Bookings { get; set; }
        public ObservableCollection<string> ReaderTickets { get; set; }
        public ObservableCollection<BookingBook> BookingBooks { get; set; }
        public ObservableCollection<string> AvailableBooksDisplay { get; set; }

        private readonly Dictionary<string, Book> _booksDictionary = new Dictionary<string, Book>();

        private string _selectedBookDisplay;
        public string SelectedBookDisplay
        {
            get => _selectedBookDisplay;
            set
            {
                _selectedBookDisplay = value;
                OnPropertyChanged(nameof(SelectedBookDisplay));
                if (!string.IsNullOrEmpty(value) && _booksDictionary.TryGetValue(value, out var book))
                {
                    SelectedNewBook = book;
                }
                else
                {
                    SelectedNewBook = null;
                }
            }
        }

        private Book _selectedNewBook;
        public Book SelectedNewBook
        {
            get => _selectedNewBook;
            private set
            {
                _selectedNewBook = value;
                OnPropertyChanged(nameof(SelectedNewBook));
            }
        }

        private DateOnly? _bookingDate;
        private string _readerTicket;
        private string _idBooking;

        public DateOnly? BookingDate
        {
            get => _bookingDate;
            set
            {
                _bookingDate = value;
                OnPropertyChanged(nameof(BookingDate));
            }
        }

        public string ReaderTicket
        {
            get => _readerTicket;
            set
            {
                _readerTicket = value;
                OnPropertyChanged(nameof(ReaderTicket));
            }
        }

        public string IdBooking
        {
            get => _idBooking;
            set
            {
                _idBooking = value;
                OnPropertyChanged(nameof(IdBooking));
            }
        }

        public Booking SelectedBooking
        {
            get => _selectedBooking;
            set
            {
                _selectedBooking = value;
                OnPropertyChanged(nameof(SelectedBooking));

                if (_selectedBooking != null)
                {
                    ReaderTicket = _selectedBooking.ReaderTicket.ToString();
                    BookingDate = _selectedBooking.BookingDate;
                    IdBooking = "БРОНИРОВАНИЕ № " + _selectedBooking.IdBooking;

                    LoadBookingBooks();
                }
                else
                {
                    ClearFields();
                }
            }
        }

        public ICommand AddBookingCommand { get; }
        public ICommand AddBookingBookCommand { get; }
        public ICommand RemoveBookingBookCommand { get; }

        public AddBookingViewModel(Booking booking)
        {
            _context = new LibraryContext();

            Bookings = new ObservableCollection<Booking>();
            BookingBooks = new ObservableCollection<BookingBook>();
            LoadInitialData();

            AddBookingCommand = new RelayCommand(AddBooking);
            AddBookingBookCommand = new RelayCommand(AddBookingBook);
            RemoveBookingBookCommand = new RelayCommandT<BookingBook>(RemoveBookingBook);
            SelectedBooking = booking;
        }

        private void LoadInitialData()
        {
            var readers = _context.Readers.ToList();
            ReaderTickets = new ObservableCollection<string>(
                readers.Select(r => r.ReaderTicket.ToString()));

            var books = _context.Books.ToList();
            _booksDictionary.Clear();
            foreach (var book in books)
            {
                var displayText = $"{book.TitleBook} - ({book.AuthorBook})";
                _booksDictionary[displayText] = book;
            }
            AvailableBooksDisplay = new ObservableCollection<string>(_booksDictionary.Keys);
        }

        private void LoadBookingBooks()
        {
            if (_selectedBooking == null) return;

            BookingBooks.Clear();

            var books = _context.BookingBooks
                .Include(bb => bb.IdBookNavigation)
                .Where(bb => bb.IdBooking == _selectedBooking.IdBooking)
                .ToList();

            foreach (var book in books)
            {
                BookingBooks.Add(book);
            }
        }

        private void AddBookingBook()
        {
            if (SelectedNewBook == null)
            {
                MessageBox.Show("Выберите книгу для добавления", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newBookingBook = new BookingBook
            {
                IdBook = SelectedNewBook.IdBook,
                IdBookNavigation = SelectedNewBook
            };

            if (_selectedBooking == null)
            {
                BookingBooks.Add(newBookingBook);
            }
            else
            {
                newBookingBook.IdBooking = _selectedBooking.IdBooking;
                BookingBooks.Add(newBookingBook);
            }

            SelectedBookDisplay = null;
            SelectedNewBook = null;
        }

        private void RemoveBookingBook(BookingBook bookingBook)
        {
            if (bookingBook == null) return;

            if (MessageBox.Show("Удалить книгу из бронирования?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                if (bookingBook.IdBookingBook > 0)
                {
                    var entity = _context.BookingBooks.Find(bookingBook.IdBookingBook);
                    if (entity != null)
                    {
                        _context.BookingBooks.Remove(entity);
                        _context.SaveChanges();
                    }
                }

                BookingBooks.Remove(bookingBook);

                if (_selectedBooking != null)
                {
                    var book = _context.Books.Find(bookingBook.IdBook);
                    if (book != null)
                    {
                        book.CopiesNumber++;
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddBooking()
        {
            if (!DateOnly.TryParse(BookingDate?.ToString(), out var date))
            {
                MessageBox.Show("Укажите корректную дату бронирования", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(ReaderTicket, out var ticket))
            {
                MessageBox.Show("Укажите корректный номер читательского билета", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (BookingBooks.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы одну книгу", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (_selectedBooking == null)
                {
                    var booking = new Booking
                    {
                        ReaderTicket = ticket,
                        BookingDate = date
                    };

                    _context.Bookings.Add(booking);
                    _context.SaveChanges();

                    foreach (var bookingBook in BookingBooks)
                    {
                        bookingBook.IdBooking = booking.IdBooking;
                        _context.BookingBooks.Add(bookingBook);
                    }
                    MessageBox.Show($"Бронирование успешно добавлено! ID: {booking.IdBooking}", "Добавление",
                                MessageBoxButton.OK, MessageBoxImage.Information);

                    _context.SaveChanges();

                    ClearFields();
                }
                else
                {
                    _selectedBooking.ReaderTicket = ticket;
                    _selectedBooking.BookingDate = date;

                    var existingBookIds = _context.BookingBooks
                        .Where(bb => bb.IdBooking == _selectedBooking.IdBooking)
                        .Select(bb => bb.IdBook)
                        .ToList();

                    foreach (var bookingBook in BookingBooks)
                    {
                        if (!existingBookIds.Contains(bookingBook.IdBook))
                        {
                            var newBookingBook = new BookingBook
                            {
                                IdBooking = _selectedBooking.IdBooking,
                                IdBook = bookingBook.IdBook
                            };

                            try
                            {
                                _context.BookingBooks.Add(newBookingBook);
                            }
                            catch (DbUpdateException ex)
                            {
                                MessageBox.Show($"Ошибка при добавлении книги: {ex.InnerException?.Message}", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                                _context.Entry(newBookingBook).State = EntityState.Detached;
                                continue;
                            }
                        }
                    }

                    var currentBookIds = BookingBooks.Select(bb => bb.IdBook).ToList();
                    var booksToRemove = _context.BookingBooks
                        .Where(bb => bb.IdBooking == _selectedBooking.IdBooking &&
                                    !currentBookIds.Contains(bb.IdBook))
                        .ToList();

                    foreach (var bookToRemove in booksToRemove)
                    {
                        _context.BookingBooks.Remove(bookToRemove);
                    }

                    _context.SaveChanges();

                    MessageBox.Show("Бронирование успешно обновлено", "Редактирование",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearFields()
        {
            ReaderTicket = string.Empty;
            BookingDate = null;
            BookingBooks.Clear();
            SelectedBookDisplay = null;
            SelectedNewBook = null;
        }
    }
}