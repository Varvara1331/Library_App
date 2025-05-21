using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Library_App.Models;
using Library_App.Data;
using Microsoft.EntityFrameworkCore;

namespace Library_App.ViewModels
{
    public class AddBookDistributionViewModel : BaseViewModel
    {
        private readonly LibraryContext _context;
        private BookDistribution _selectedBookDistribution;

        public ObservableCollection<BookDistribution> BookDistributions { get; set; }
        public ObservableCollection<string> ReaderTickets { get; set; }
        public ObservableCollection<BookDistributionBook> BookDistributionBooks { get; set; }
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

        private DateOnly? _returnDate;
        private DateOnly? _issuanceDate;
        private string _readerTicket;
        private string _idBookDistribution;

        public DateOnly? IssuanceDate
        {
            get => _issuanceDate;
            set
            {
                _issuanceDate = value;
                OnPropertyChanged(nameof(IssuanceDate));
            }
        }

        public DateOnly? ReturnDate
        {
            get => _returnDate;
            set
            {
                _returnDate = value;
                OnPropertyChanged(nameof(ReturnDate));
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

        public string IdBookDistribution
        {
            get => _idBookDistribution;
            set
            {
                _idBookDistribution = value;
                OnPropertyChanged(nameof(IdBookDistribution));
            }
        }

        public BookDistribution SelectedBookDistribution
        {
            get => _selectedBookDistribution;
            set
            {
                _selectedBookDistribution = value;
                OnPropertyChanged(nameof(SelectedBookDistribution));

                if (_selectedBookDistribution != null)
                {
                    ReaderTicket = _selectedBookDistribution.ReaderTicket.ToString();
                    ReturnDate = _selectedBookDistribution.ReturnDate;
                    IssuanceDate = _selectedBookDistribution.IssuanceDate;
                    IdBookDistribution = "ВЫДАЧА № " + _selectedBookDistribution.IdBookDistribution;

                    LoadBookDistributionBooks();
                }
                else
                {
                    ClearFields();
                }
            }
        }

        public ICommand AddBookDistributionCommand { get; }
        public ICommand AddBookDistributionBookCommand { get; }
        public ICommand RemoveBookDistributionBookCommand { get; }

        public AddBookDistributionViewModel(BookDistribution bookDistribution)
        {
            _context = new LibraryContext();

            BookDistributions = new ObservableCollection<BookDistribution>();
            BookDistributionBooks = new ObservableCollection<BookDistributionBook>();
            LoadInitialData();

            AddBookDistributionCommand = new RelayCommand(AddBookDistribution);
            AddBookDistributionBookCommand = new RelayCommand(AddBookDistributionBook);
            RemoveBookDistributionBookCommand = new RelayCommandT<BookDistributionBook>(RemoveBookDistributionBook);
            SelectedBookDistribution = bookDistribution;
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

        private void LoadBookDistributionBooks()
        {
            if (_selectedBookDistribution == null) return;

            BookDistributionBooks.Clear();

            var books = _context.BookDistributionBooks
                .Include(bb => bb.IdBookNavigation)
                .Where(bb => bb.IdBookDistribution == _selectedBookDistribution.IdBookDistribution)
                .ToList();

            foreach (var book in books)
            {
                BookDistributionBooks.Add(book);
            }
        }

        private void AddBookDistributionBook()
        {
            if (SelectedNewBook == null)
            {
                MessageBox.Show("Выберите книгу для добавления", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newBookDistributionBook = new BookDistributionBook
            {
                IdBook = SelectedNewBook.IdBook,
                IdBookNavigation = SelectedNewBook
            };

            if (_selectedBookDistribution == null)
            {
                BookDistributionBooks.Add(newBookDistributionBook);
            }
            else
            {
                newBookDistributionBook.IdBookDistribution = _selectedBookDistribution.IdBookDistribution;
                BookDistributionBooks.Add(newBookDistributionBook);
            }

            SelectedBookDisplay = null;
            SelectedNewBook = null;
        }

        private void RemoveBookDistributionBook(BookDistributionBook bookDistributionBook)
        {
            if (bookDistributionBook == null) return;

            if (MessageBox.Show("Удалить книгу из выдачи?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                if (bookDistributionBook.IdBookDistributionBook > 0)
                {
                    var entity = _context.BookDistributionBooks.Find(bookDistributionBook.IdBookDistributionBook);
                    if (entity != null)
                    {
                        _context.BookDistributionBooks.Remove(entity);
                        _context.SaveChanges();
                    }
                }

                BookDistributionBooks.Remove(bookDistributionBook);

                if (_selectedBookDistribution != null)
                {
                    var book = _context.Books.Find(bookDistributionBook.IdBook);
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

        private void AddBookDistribution()
        {
            if (!DateOnly.TryParse(IssuanceDate?.ToString(), out var date))
            {
                MessageBox.Show("Укажите корректную дату выдачи", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!DateOnly.TryParse(ReturnDate?.ToString(), out var date2))
            {
                MessageBox.Show("Укажите корректную дату возврата", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (date > date2)
            {
                MessageBox.Show("Дата возврата должна быть после даты выдачи", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(ReaderTicket, out var ticket))
            {
                MessageBox.Show("Укажите корректный номер читательского билета", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (BookDistributionBooks.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы одну книгу", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (_selectedBookDistribution == null)
                {
                    var bookDistribution = new BookDistribution
                    {
                        ReaderTicket = ticket,
                        IssuanceDate = date,
                        ReturnDate = date2
                    };

                    _context.BookDistributions.Add(bookDistribution);
                    _context.SaveChanges();

                    foreach (var bookDistributionBook in BookDistributionBooks)
                    {
                        bookDistributionBook.IdBookDistribution = bookDistribution.IdBookDistribution;
                        _context.BookDistributionBooks.Add(bookDistributionBook);
                    }
                    MessageBox.Show($"Выдача успешно добавлена! ID: {bookDistribution.IdBookDistribution}", "Добавление",
                                MessageBoxButton.OK, MessageBoxImage.Information);

                    _context.SaveChanges();

                    ClearFields();
                }
                else
                {
                    _selectedBookDistribution.ReaderTicket = ticket;
                    _selectedBookDistribution.IssuanceDate = date;
                    _selectedBookDistribution.ReturnDate = date2;

                    var existingBookIds = _context.BookDistributionBooks
                        .Where(bb => bb.IdBookDistribution == _selectedBookDistribution.IdBookDistribution)
                        .Select(bb => bb.IdBook)
                        .ToList();

                    foreach (var bookDistributionBook in BookDistributionBooks)
                    {
                        if (!existingBookIds.Contains(bookDistributionBook.IdBook))
                        {
                            var newBookDistributionBook = new BookDistributionBook
                            {
                                IdBookDistribution = _selectedBookDistribution.IdBookDistribution,
                                IdBook = bookDistributionBook.IdBook
                            };

                            try
                            {
                                _context.BookDistributionBooks.Add(newBookDistributionBook);
                            }
                            catch (DbUpdateException ex)
                            {
                                MessageBox.Show($"Ошибка при добавлении книги: {ex.InnerException?.Message}", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                                _context.Entry(newBookDistributionBook).State = EntityState.Detached;
                                continue;
                            }
                        }
                    }

                    var currentBookIds = BookDistributionBooks.Select(bb => bb.IdBook).ToList();
                    var booksToRemove = _context.BookDistributionBooks
                        .Where(bb => bb.IdBookDistribution == _selectedBookDistribution.IdBookDistribution &&
                                    !currentBookIds.Contains(bb.IdBook))
                        .ToList();

                    foreach (var bookToRemove in booksToRemove)
                    {
                        _context.BookDistributionBooks.Remove(bookToRemove);
                    }

                    _context.SaveChanges();

                    MessageBox.Show("Выдача успешно обновлена", "Редактирование",
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
            IssuanceDate = null;
            ReturnDate = null;
            BookDistributionBooks.Clear();
            SelectedBookDisplay = null;
            SelectedNewBook = null;
        }
    }
}