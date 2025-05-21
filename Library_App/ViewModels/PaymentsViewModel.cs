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
    public class PaymentsViewModel : BaseViewModel
    {
        private readonly Frame _navigationFrame;
        private readonly LibraryContext _context;
        private ObservableCollection<Payment> _payments;
        private Payment _selectedPayment;

        public ObservableCollection<Payment> Payments
        {
            get => _payments;
            set { _payments = value; OnPropertyChanged(); }
        }

        public Payment SelectedPayment
        {
            get => _selectedPayment;
            set { _selectedPayment = value; OnPropertyChanged(); }
        }

        public ICommand LoadPaymentsCommand { get; }
        public ICommand EditPaymentCommand { get; }

        public PaymentsViewModel(Frame frame)
        {
            _navigationFrame = frame;
            _context = new LibraryContext();

            LoadPaymentsCommand = new RelayCommand(LoadPayments);
            EditPaymentCommand = new RelayCommand(EditPayment);

            LoadPayments();
        }

        public async void LoadPayments()
        {
            try
            {
                var payments = await _context.Payments
                    .AsNoTracking()
                    .ToListAsync();

                Payments = new ObservableCollection<Payment>(payments);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditPayment()
        {
            _navigationFrame.Navigate(new PaymentView(SelectedPayment, _navigationFrame));
        }

        public void RemovePayment(List<Payment> payments)
        {
            if (payments.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранные записи?",
            "Подтверждение удаления",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new LibraryContext())
                    {
                        foreach (var payment in payments)
                        {
                            context.Payments.Remove(payment);
                            context.SaveChanges();
                        }
                        LoadPayments();
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите платежи для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void ExportPayments()
        {
            if (Payments == null || !Payments.Any())
            {
                MessageBox.Show("Нет данных для экспорта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Payments");

                worksheet.Cell(1, 1).Value = "ДАТА ОПЛАТЫ";
                worksheet.Cell(1, 2).Value = "№ ЧИТАТЕЛЬСКОГО БИЛЕТА ПЛАТЕЛЬЩИКА";
                worksheet.Cell(1, 3).Value = "НАЗВАНИЕ УСЛУГИ";
                worksheet.Cell(1, 4).Value = "СУММА ПЛАТЕЖА";

                for (int i = 0; i < Payments.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = Payments[i].PaymentDate.ToString();
                    worksheet.Cell(i + 2, 2).Value = Payments[i].ReaderTicket;
                    worksheet.Cell(i + 2, 3).Value = Payments[i].NameService;
                    worksheet.Cell(i + 2, 4).Value = Payments[i].Cost;
                }

                var headerRange = worksheet.Range("A1:D1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
                worksheet.Columns().AdjustToContents();

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = "Платежи",
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