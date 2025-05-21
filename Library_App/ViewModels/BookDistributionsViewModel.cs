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
    public class BookDistributionsViewModel : BaseViewModel
    {
        private readonly Frame _navigationFrame;
        private readonly LibraryContext _context;
        private ObservableCollection<BookDistribution> _bookDistributions;
        private BookDistribution _selectedBookDistribution;

        public ObservableCollection<BookDistribution> BookDistributions
        {
            get => _bookDistributions;
            set { _bookDistributions = value; OnPropertyChanged(); }
        }

        public BookDistribution SelectedBookDistribution
        {
            get => _selectedBookDistribution;
            set { _selectedBookDistribution = value; OnPropertyChanged(); }
        }

        public ICommand LoadBookDistributionsCommand { get; }
        public ICommand EditBookDistributionCommand { get; }

        public BookDistributionsViewModel(Frame frame)
        {
            _navigationFrame = frame;
            _context = new LibraryContext();

            LoadBookDistributionsCommand = new RelayCommand(LoadBookDistributions);
            EditBookDistributionCommand = new RelayCommand(EditBookDistribution);

            LoadBookDistributions();
        }

        public async void LoadBookDistributions()
        {
            try
            {
                var bookDistributions = await _context.BookDistributions
            .Include(b => b.BookDistributionBooks)
            .ThenInclude(bb => bb.IdBookNavigation)
            .AsNoTracking()
            .ToListAsync();

                BookDistributions = new ObservableCollection<BookDistribution>(bookDistributions);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditBookDistribution()
        {
            _navigationFrame.Navigate(new BookDistributionView(SelectedBookDistribution, _navigationFrame));
        }

        public void RemoveBookDistribution(List<BookDistribution> bookDistributions)
        {
            if (bookDistributions.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранные записи?",
            "Подтверждение удаления",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new LibraryContext())
                    {
                        foreach (var bookDistribution in bookDistributions)
                        {
                            foreach (var bookDistributionBook in bookDistribution.BookDistributionBooks)
                            {
                                context.BookDistributionBooks.Remove(bookDistributionBook);
                            }
                            context.BookDistributions.Remove(bookDistribution);
                            context.SaveChanges();
                        }
                        LoadBookDistributions();
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите записи о выдаче для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void ExportBookDistributions()
        {
            if (BookDistributions == null || !BookDistributions.Any())
            {
                MessageBox.Show("Нет данных для экспорта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var workbook = new XLWorkbook())
            {
                var distributionsWorksheet = workbook.Worksheets.Add("BookDistributions");

                distributionsWorksheet.Cell(1, 1).Value = "ID выдачи";
                distributionsWorksheet.Cell(1, 2).Value = "Дата выдачи";
                distributionsWorksheet.Cell(1, 3).Value = "Дата возврата";
                distributionsWorksheet.Cell(1, 4).Value = "№ читательского билета";

                for (int i = 0; i < BookDistributions.Count; i++)
                {
                    var distribution = BookDistributions[i];
                    distributionsWorksheet.Cell(i + 2, 1).Value = distribution.IdBookDistribution;
                    distributionsWorksheet.Cell(i + 2, 2).Value = distribution.IssuanceDate.ToString("dd.MM.yyyy");
                    distributionsWorksheet.Cell(i + 2, 3).Value = distribution.ReturnDate.ToString("dd.MM.yyyy");
                    distributionsWorksheet.Cell(i + 2, 4).Value = distribution.ReaderTicket;
                }

                var distributionBooksWorksheet = workbook.Worksheets.Add("BookDistributionBooks");

                distributionBooksWorksheet.Cell(1, 1).Value = "ID выдачи";
                distributionBooksWorksheet.Cell(1, 2).Value = "ID книги";
                distributionBooksWorksheet.Cell(1, 3).Value = "Название книги";
                distributionBooksWorksheet.Cell(1, 4).Value = "Автор";
                distributionBooksWorksheet.Cell(1, 5).Value = "Год издания";
                distributionBooksWorksheet.Cell(1, 6).Value = "Возвращена";

                int row = 2;
                foreach (var distribution in BookDistributions)
                {
                    foreach (var distributionBook in distribution.BookDistributionBooks)
                    {
                        distributionBooksWorksheet.Cell(row, 1).Value = distributionBook.IdBookDistribution;
                        distributionBooksWorksheet.Cell(row, 2).Value = distributionBook.IdBook;

                        if (distributionBook.IdBookNavigation != null)
                        {
                            distributionBooksWorksheet.Cell(row, 3).Value = distributionBook.IdBookNavigation.TitleBook;
                            distributionBooksWorksheet.Cell(row, 4).Value = distributionBook.IdBookNavigation.AuthorBook;
                            distributionBooksWorksheet.Cell(row, 5).Value = distributionBook.IdBookNavigation.YearOfPublication;
                        }

                        distributionBooksWorksheet.Cell(row, 6).Value = distributionBook.Refund.HasValue && distributionBook.Refund.Value ? "Да" : "Нет";
                        row++;
                    }
                }

                FormatWorksheet(distributionsWorksheet);
                FormatWorksheet(distributionBooksWorksheet);

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = "Выдача",
                    DefaultExt = ".xlsx",
                    Filter = "Excel Documents (.xlsx)|*.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    workbook.SaveAs(saveFileDialog.FileName);

                    Process.Start(new ProcessStartInfo
                    {
                        FileName = saveFileDialog.FileName,
                        UseShellExecute = true
                    });

                    MessageBox.Show("Данные о выдачах успешно экспортированы в Excel.", "Экспорт",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
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