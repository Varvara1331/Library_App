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
    public class BooksViewModel : BaseViewModel
    {
        private readonly Frame _navigationFrame;
        private readonly LibraryContext _context;
        private ObservableCollection<Book> _books;
        private Book _selectedBook;

        public ObservableCollection<Book> Books
        {
            get => _books;
            set { _books = value; OnPropertyChanged(); }
        }

        public Book SelectedBook
        {
            get => _selectedBook;
            set { _selectedBook = value; OnPropertyChanged(); }
        }

        public ICommand LoadBooksCommand { get; }
        public ICommand EditBookCommand { get; }

        public BooksViewModel(Frame frame)
        {
            _navigationFrame = frame;
            _context = new LibraryContext();

            LoadBooksCommand = new RelayCommand(LoadBooks);
            EditBookCommand = new RelayCommand(EditBook);

            LoadBooks();
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

        private void EditBook()
        {
            _navigationFrame.Navigate(new BookView(SelectedBook, _navigationFrame));
        }

        public void RemoveBook(List<Book> books)
        {
            if (books.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранные записи?",
            "Подтверждение удаления",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new LibraryContext())
                    {
                        foreach (var book in books)
                        {
                            context.Books.Remove(book);
                            context.SaveChanges();
                        }
                        LoadBooks();
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите книги для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void ExportBooks()
        {
            if (Books == null || !Books.Any())
            {
                MessageBox.Show("Нет данных для экспорта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Books");

                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "НЗВАНИЕ";
                worksheet.Cell(1, 3).Value = "АВТОР";
                worksheet.Cell(1, 4).Value = "ИЗДАТЕЛЬСТВО";
                worksheet.Cell(1, 5).Value = "ГОД ПУБЛИКАЦИИ";
                worksheet.Cell(1, 6).Value = "ЖАНР";
                worksheet.Cell(1, 7).Value = "ВОЗРАСТНЫЕ ОГРАНИЧЕНИЯ";
                worksheet.Cell(1, 8).Value = "РАЗРЕШЕНИЕ НА ВЫДАЧУ";
                worksheet.Cell(1, 9).Value = "КОЛИЧЕСТВО ЭКЗЕМПЛЯРОВ";

                for (int i = 0; i < Books.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = Books[i].IdBook;
                    worksheet.Cell(i + 2, 2).Value = Books[i].TitleBook;
                    worksheet.Cell(i + 2, 3).Value = Books[i].AuthorBook;
                    worksheet.Cell(i + 2, 4).Value = Books[i].Publishing;
                    worksheet.Cell(i + 2, 5).Value = Books[i].YearOfPublication;
                    worksheet.Cell(i + 2, 6).Value = Books[i].Genre;
                    worksheet.Cell(i + 2, 7).Value = Books[i].AgeRestrictions;
                    worksheet.Cell(i + 2, 8).Value = Books[i].PermissionToIssuance.ToString();
                    worksheet.Cell(i + 2, 9).Value = Books[i].CopiesNumber;
                }

                var headerRange = worksheet.Range("A1:I1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
                worksheet.Columns().AdjustToContents();

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = "Книги",
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
    }
}