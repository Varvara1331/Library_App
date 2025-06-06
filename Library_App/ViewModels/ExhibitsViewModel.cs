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
    public class ExhibitsViewModel : BaseViewModel
    {
        private readonly Frame _navigationFrame;
        private readonly LibraryContext _context;
        private ObservableCollection<Exhibit> _exhibits;
        private Exhibit _selectedExhibit;

        public ObservableCollection<Exhibit> Exhibits
        {
            get => _exhibits;
            set { _exhibits = value; OnPropertyChanged(); }
        }

        public Exhibit SelectedExhibit
        {
            get => _selectedExhibit;
            set { _selectedExhibit = value; OnPropertyChanged(); }
        }

        public ICommand LoadExhibitsCommand { get; }
        public ICommand EditExhibitCommand { get; }

        public ExhibitsViewModel(Frame frame)
        {
            _navigationFrame = frame;
            _context = new LibraryContext();

            LoadExhibitsCommand = new RelayCommand(LoadExhibits);
            EditExhibitCommand = new RelayCommand(EditExhibit);

            LoadExhibits();
        }

        public async void LoadExhibits()
        {
            try
            {
                var exhibits = await _context.Exhibits
                    .AsNoTracking()
                    .ToListAsync();

                Exhibits = new ObservableCollection<Exhibit>(exhibits);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditExhibit()
        {
            _navigationFrame.Navigate(new ExhibitView(SelectedExhibit, _navigationFrame));
        }

        public void RemoveExhibit(List<Exhibit> exhibits)
        {
            if (exhibits.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранные записи?",
            "Подтверждение удаления",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new LibraryContext())
                    {
                        foreach (var exhibit in exhibits)
                        {
                            context.Exhibits.Remove(exhibit);
                            context.SaveChanges();
                        }
                        LoadExhibits();
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите читателей для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void ExportExhibits()
        {
            if (Exhibits == null || !Exhibits.Any())
            {
                MessageBox.Show("Нет данных для экспорта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Exhibits");

                worksheet.Cell(1, 1).Value = "ДАТА СОЗДАНИЯ";
                worksheet.Cell(1, 2).Value = "НАЗВАНИЕ";
                worksheet.Cell(1, 3).Value = "АВТОР";
                worksheet.Cell(1, 4).Value = "ТЕМАТИКА";

                for (int i = 0; i < Exhibits.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = Exhibits[i].CreationDate.ToString();
                    worksheet.Cell(i + 2, 2).Value = Exhibits[i].Title;
                    worksheet.Cell(i + 2, 3).Value = Exhibits[i].Author;
                    worksheet.Cell(i + 2, 4).Value = Exhibits[i].Subject;
                }

                FormatWorksheet(worksheet);


                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = "Экспонаты",
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