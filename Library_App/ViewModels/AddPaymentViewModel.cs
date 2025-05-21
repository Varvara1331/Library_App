using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Library_App.Models;
using Library_App.Data;
using Microsoft.EntityFrameworkCore;

namespace Library_App.ViewModels
{
    public class AddPaymentViewModel : BaseViewModel
    {
        private LibraryContext _context;

        private Payment _selectedPayment;
        public ObservableCollection<Payment> Payments { get; set; }
        public ObservableCollection<string> ReaderTickets { get; set; }
        public ObservableCollection<string> Services { get; set; }


        private DateOnly? _paymentDate;
        private string _cost;
        private string _nameService;
        private string _ticket;
        private string _id;

        public string Cost
        {
            get => _cost;
            set
            {
                _cost = value;
                OnPropertyChanged(nameof(Cost));
            }
        }

        public string NameService
        {
            get => _nameService;
            set
            {
                _nameService = value;
                OnPropertyChanged(nameof(NameService));
            }
        }

        public DateOnly? PaymentDate
        {
            get => _paymentDate;
            set
            {
                _paymentDate = value;
                OnPropertyChanged(nameof(PaymentDate));
            }
        }

        public string ReaderTicket
        {
            get => _ticket;
            set
            {
                _ticket = value;
                OnPropertyChanged(nameof(ReaderTicket));
            }
        }

        public string IdPayment
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(IdPayment));
            }
        }

        public Payment SelectedPayment
        {
            get => _selectedPayment;
            set
            {
                _selectedPayment = value;
                OnPropertyChanged(nameof(SelectedPayment));
                if (_selectedPayment != null)
                {
                    ReaderTicket = ReaderTickets.FirstOrDefault(rt => rt == _selectedPayment.ReaderTicket.ToString());
                    PaymentDate = _selectedPayment.PaymentDate;
                    Cost = _selectedPayment.Cost.ToString();
                    NameService = Services.FirstOrDefault(s => s == _selectedPayment.NameService);
                    IdPayment = "ПЛАТЕЖ № " + _selectedPayment.IdPayment.ToString();
                }
            }
        }

        public bool IsPaymentValid(decimal cost, int ticket)
        {
            var reader = _context.Readers
                .FirstOrDefault(r => r.ReaderTicket == ticket);

            if (reader == null)
            {
                return false;
            }

            return cost <= reader.Fine;
        }

        public ICommand AddPaymentCommand { get; set; }

        public ICommand EditPaymentCommand { get; set; }

        public AddPaymentViewModel(Payment payment)
        {
            _context = new LibraryContext();
            LoadPayments();
            ReaderTickets = new ObservableCollection<string>(_context.Readers.Select(r => r.ReaderTicket.ToString()).ToList());
            Services = new ObservableCollection<string>(_context.PriceLists.Select(r => r.NameService).ToList());
            SelectedPayment = payment;
            AddPaymentCommand = new RelayCommand(AddPayment);
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
        private void AddPayment()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NameService))
                {
                    MessageBox.Show("Наименование услуги обязательно для заполнения", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(ReaderTicket))
                {
                    MessageBox.Show("№ ЧБ обязателен для заполнения", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!DateOnly.TryParse(PaymentDate.ToString(), out DateOnly date))
                {
                    MessageBox.Show("Укажите корректную дату платежа", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(Cost, out decimal cost))
                {
                    cost = 0;
                }

                if (!int.TryParse(ReaderTicket, out int ticket))
                {
                    MessageBox.Show("Укажите корректный № ЧБ", "Ошибка",
                                                     MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (NameService == "Штрафная ставка")
                {
                    var reader = _context.Readers.FirstOrDefault(r => r.ReaderTicket == ticket);
                    if (reader == null || reader.Fine <= 0)
                    {
                        MessageBox.Show("У читателя нет штрафов для оплаты", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    reader.Fine -= cost;
                }


                if (SelectedPayment != null)
                {
                    var entity = _context.Payments.FirstOrDefault(p => p.IdPayment == _selectedPayment.IdPayment);
                    entity.NameService = NameService;
                    entity.ReaderTicket = ticket;
                    entity.PaymentDate = date;
                    entity.Cost = cost;
                    _context.SaveChanges();
                    LoadPayments();

                    MessageBox.Show("Книга успешно отредактирована!", "Редактирование", MessageBoxButton.OK, MessageBoxImage.Information);


                    ClearFields();
                }
                else
                {
                    var newPayment = new Payment
                    {
                        NameService = NameService,
                        ReaderTicket = ticket,
                        PaymentDate = date,
                        Cost = cost
                    };

                    _context.Payments.Add(newPayment);
                    _context.SaveChanges();

                    LoadPayments();

                    MessageBox.Show($"Платеж успешно добавлен! ID: {newPayment.IdPayment}", "Добавление",
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
            ReaderTicket = string.Empty;
            NameService = string.Empty;
            PaymentDate = null;
            Cost = string.Empty;
            IdPayment = string.Empty;
            SelectedPayment = null;
        }
    }
}
