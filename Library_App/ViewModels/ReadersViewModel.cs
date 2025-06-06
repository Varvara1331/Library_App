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
    public class ReadersViewModel : BaseViewModel
    {
        private readonly Frame _navigationFrame;
        private readonly LibraryContext _context;
        private ObservableCollection<Reader> _readers;
        private Reader _selectedReader;

        public ObservableCollection<Reader> Readers
        {
            get => _readers;
            set { _readers = value; OnPropertyChanged(); }
        }

        public Reader SelectedReader
        {
            get => _selectedReader;
            set { _selectedReader = value; OnPropertyChanged(); }
        }

        public ICommand LoadReadersCommand { get; }
        public ICommand EditReaderCommand { get; }

        public ReadersViewModel(Frame frame)
        {
            _navigationFrame = frame;
            _context = new LibraryContext();

            LoadReadersCommand = new RelayCommand(LoadReaders);
            EditReaderCommand = new RelayCommand(EditReader);

            LoadReaders();
        }

        public async void LoadReaders()
        {
            try
            {
                var readers = await _context.Readers
                    .AsNoTracking()
                    .OrderBy(r => r.LastName)
                    .ThenBy(r => r.FirstName)
                    .ToListAsync();

                Readers = new ObservableCollection<Reader>(readers);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditReader()
        {
            _navigationFrame.Navigate(new ReaderView(SelectedReader, _navigationFrame));
        }

        public void RemoveReader(List<Reader> readers)
        {
            if (readers.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранные записи?",
            "Подтверждение удаления",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new LibraryContext())
                    {
                        foreach (var reader in readers)
                        {
                            context.Readers.Remove(reader);
                            context.SaveChanges();
                        }
                        LoadReaders();
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите читателей для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void ExportReaders()
        {
            if (Readers == null || !Readers.Any())
            {
                MessageBox.Show("Нет данных для экспорта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Readers");

                worksheet.Cell(1, 1).Value = "№ ЧИТАТЕЛЬСКОГО БИЛЕТА";
                worksheet.Cell(1, 2).Value = "ЧИТАТЕЛЬ";
                worksheet.Cell(1, 3).Value = "ДАТА РОЖДЕНИЯ";
                worksheet.Cell(1, 4).Value = "ВОЗРАСТ";
                worksheet.Cell(1, 5).Value = "ТЕЛЕФОН";
                worksheet.Cell(1, 6).Value = "ПОЧТА";
                worksheet.Cell(1, 7).Value = "ШТРАФ";

                for (int i = 0; i < Readers.Count; i++)
                {
                    var age = DateTime.Now.Year - Readers[i].BirthDate.Year;
                    worksheet.Cell(i + 2, 1).Value = Readers[i].ReaderTicket;
                    worksheet.Cell(i + 2, 2).Value = Readers[i].LastName + " " + Readers[i].FirstName;
                    worksheet.Cell(i + 2, 3).Value = Readers[i].BirthDate.ToString();
                    worksheet.Cell(i + 2, 4).Value = age;
                    worksheet.Cell(i + 2, 5).Value = Readers[i].Telephone;
                    worksheet.Cell(i + 2, 6).Value = Readers[i].Email;
                    worksheet.Cell(i + 2, 7).Value = Readers[i].Fine;
                }

                FormatWorksheet(worksheet);

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = "Читатели",
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