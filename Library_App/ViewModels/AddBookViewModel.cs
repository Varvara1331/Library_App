using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Library_App.Models;
using Library_App.Data;
using Microsoft.EntityFrameworkCore;

namespace Library_App.ViewModels
{
    public class AddBookViewModel : BaseViewModel
    {
        private LibraryContext _context;

        private Book _selectedBook;
        public ObservableCollection<Book> Books { get; set; }


        private string _titleBook;
        private string _authorBook;
        private string _publishing;
        private string _year;
        private string? _genre;
        private string _age;
        private string _permission;
        private string _copies;
        private string _idBook;

        public string TitleBook
        {
            get => _titleBook;
            set
            {
                _titleBook = value;
                OnPropertyChanged(nameof(TitleBook));
            }
        }

        public string AuthorBook
        {
            get => _authorBook;
            set
            {
                _authorBook = value;
                OnPropertyChanged(nameof(AuthorBook));
            }
        }

        public string Publishing
        {
            get => _publishing;
            set
            {
                _publishing = value;
                OnPropertyChanged(nameof(Publishing));
            }
        }

        public string YearOfPublication
        {
            get => _year;
            set
            {
                _year = value;
                OnPropertyChanged(nameof(YearOfPublication));
            }
        }

        public string? Genre
        {
            get => _genre;
            set
            {
                _genre = value;
                OnPropertyChanged(nameof(Genre));
            }
        }

        public string AgeRestrictions
        {
            get => _age;
            set
            {
                _age = value;
                OnPropertyChanged(nameof(AgeRestrictions));
            }
        }

        public string PermissionToIssuance
        {
            get => _permission;
            set
            {
                _permission = value;
                OnPropertyChanged(nameof(PermissionToIssuance));
            }
        }

        public string CopiesNumber
        {
            get => _copies;
            set
            {
                _copies = value;
                OnPropertyChanged(nameof(CopiesNumber));
            }
        }

        public string IdBook
        {
            get => _idBook;
            set
            {
                _idBook = value;
                OnPropertyChanged(nameof(IdBook));
            }
        }

        public Book SelectedBook
        {
            get => _selectedBook;
            set
            {
                _selectedBook = value;
                OnPropertyChanged(nameof(SelectedBook));
                if (_selectedBook != null)
                {
                    TitleBook = _selectedBook.TitleBook;
                    AuthorBook = _selectedBook.AuthorBook;
                    Publishing = _selectedBook.Publishing;
                    YearOfPublication = _selectedBook.YearOfPublication.ToString();
                    Genre = _selectedBook.Genre;
                    AgeRestrictions = _selectedBook.AgeRestrictions.ToString();
                    PermissionToIssuance = _selectedBook.PermissionToIssuance.ToString();
                    CopiesNumber = _selectedBook.CopiesNumber.ToString();
                    IdBook = "КНИГА № " + _selectedBook.IdBook.ToString();
                }
            }
        }

        public ICommand AddBookCommand { get; set; }

        public ICommand EditBookCommand { get; set; }

        public AddBookViewModel(Book book)
        {
            _context = new LibraryContext();
            LoadBooks();
            SelectedBook = book;
            AddBookCommand = new RelayCommand(AddBook);
        }

        public async void LoadBooks()
        {
            try
            {
                var books = await _context.Books
                    .AsNoTracking()
                    .ToListAsync();

                Books = new ObservableCollection<Book>(books);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddBook()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TitleBook))
                {
                    MessageBox.Show("Название книги обязательно для заполнения", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(Publishing))
                {
                    MessageBox.Show("Издательство обязательно для заполнения", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(YearOfPublication, out int year) || year <= 0)
                {
                    MessageBox.Show("Укажите корректный год издания", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(AgeRestrictions, out int age) || age < 0 || age > 21)
                {
                    MessageBox.Show("Возрастные ограничения должны быть от 0 до 21", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(CopiesNumber, out int copies) || copies <= 0)
                {
                    MessageBox.Show("Количество экземпляров должно быть положительным числом", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Обработка nullable поля
                bool? permission = null;
                if (!string.IsNullOrWhiteSpace(PermissionToIssuance))
                {
                    if (bool.TryParse(PermissionToIssuance, out bool parsedPermission))
                    {
                        permission = parsedPermission;
                    }
                }

                if (SelectedBook != null)
                {
                    var entity = _context.Books.FirstOrDefault(p => p.IdBook == _selectedBook.IdBook);
                    entity.TitleBook = TitleBook.Trim();
                    entity.AuthorBook = !string.IsNullOrWhiteSpace(AuthorBook) ? AuthorBook.Trim() : null;
                    entity.Publishing = Publishing.Trim();
                    entity.YearOfPublication = year;
                    entity.Genre = !string.IsNullOrWhiteSpace(Genre) ? Genre.Trim() : null;
                    entity.AgeRestrictions = age;
                    entity.PermissionToIssuance = permission;
                    entity.CopiesNumber = copies;
                    _context.SaveChanges();
                    LoadBooks();

                    MessageBox.Show("Книга успешно отредактирована!", "Редактирование", MessageBoxButton.OK, MessageBoxImage.Information);


                    ClearFields();
                }
                else
                {
                    var newBook = new Book
                    {
                        TitleBook = TitleBook.Trim(),
                        AuthorBook = !string.IsNullOrWhiteSpace(AuthorBook) ? AuthorBook.Trim() : null,
                        Publishing = Publishing.Trim(),
                        YearOfPublication = year,
                        Genre = !string.IsNullOrWhiteSpace(Genre) ? Genre.Trim() : null,
                        AgeRestrictions = age,
                        PermissionToIssuance = permission,
                        CopiesNumber = copies
                    };

                    _context.Books.Add(newBook);
                    _context.SaveChanges();

                    LoadBooks();

                    MessageBox.Show($"Книга успешно добавлена! ID: {newBook.IdBook}", "Добавление",
                                  MessageBoxButton.OK, MessageBoxImage.Information);

                    ClearFields();
                }
            }
            catch (DbUpdateException dbEx)
            {
                MessageBox.Show($"Ошибка базы данных: {GetInnermostException(dbEx).Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Exception GetInnermostException(Exception ex)
        {
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            return ex;
        }

        private void ClearFields()
        {
            TitleBook = string.Empty;
            AuthorBook = string.Empty;
            Publishing = string.Empty;
            YearOfPublication = string.Empty;
            Genre = string.Empty;
            AgeRestrictions = string.Empty;
            PermissionToIssuance = string.Empty;
            CopiesNumber = string.Empty;
            SelectedBook = null;
        }
    }
}
