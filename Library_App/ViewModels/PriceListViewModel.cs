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
    public class PriceListViewModel : BaseViewModel
    {
        private readonly Frame _navigationFrame;
        private readonly LibraryContext _context;
        private ObservableCollection<PriceList> _priceList;
        private PriceList _selectedService;

        public ObservableCollection<PriceList> PriceList
        {
            get => _priceList;
            set { _priceList = value; OnPropertyChanged(); }
        }

        public PriceList SelectedService
        {
            get => _selectedService;
            set { _selectedService = value; OnPropertyChanged(); }
        }

        public ICommand LoadPriceListCommand { get; }
        public ICommand EditServiceCommand { get; }

        public PriceListViewModel(Frame frame)
        {
            _navigationFrame = frame;
            _context = new LibraryContext();

            LoadPriceListCommand = new RelayCommand(LoadPriceList);
            EditServiceCommand = new RelayCommand(EditServicer);

            LoadPriceList();
        }

        public async void LoadPriceList()
        {
            try
            {
                var priceList = await _context.PriceLists
                    .AsNoTracking()
                    .OrderBy(r => r.NameService)
                    .ToListAsync();

                PriceList = new ObservableCollection<PriceList>(priceList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditServicer()
        {
            _navigationFrame.Navigate(new ServiceView(SelectedService, _navigationFrame));
        }

        public void RemoveService(List<PriceList> priceList)
        {
            if (priceList.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранные записи?",
            "Подтверждение удаления",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new LibraryContext())
                    {
                        foreach (var service in priceList)
                        {
                            context.PriceLists.Remove(service);
                            context.SaveChanges();
                        }
                        LoadPriceList();
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите услуги для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void ExportPriceList()
        {
            if (PriceList == null || !PriceList.Any())
            {
                MessageBox.Show("Нет данных для экспорта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("PriceList");

                worksheet.Cell(1, 1).Value = "НАЗВАНИЕ УСЛУГИ";
                worksheet.Cell(1, 2).Value = "СТОИМОСТЬ";

                for (int i = 0; i < PriceList.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = PriceList[i].NameService;
                    worksheet.Cell(i + 2, 2).Value = PriceList[i].Price;
                }

                var headerRange = worksheet.Range("A1:B1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
                worksheet.Columns().AdjustToContents();

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = "Прайс-лист",
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