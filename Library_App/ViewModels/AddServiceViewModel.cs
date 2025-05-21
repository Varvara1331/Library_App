using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Library_App.Models;
using Library_App.Data;

namespace Library_App.ViewModels
{
    public class AddServiceViewModel : BaseViewModel
    {
        private LibraryContext _context;

        private PriceList _selectedService;
        public ObservableCollection<PriceList> PriceList { get; set; }


        private string _nameService;
        private string _price;

        public string NameService
        {
            get => _nameService;
            set
            {
                _nameService = value;
                OnPropertyChanged(nameof(NameService));
            }
        }

        public string Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }

        public PriceList SelectedService
        {
            get => _selectedService;
            set
            {
                _selectedService = value;
                OnPropertyChanged(nameof(SelectedService));
                if (_selectedService != null)
                {
                    NameService = _selectedService.NameService;
                    Price = _selectedService.Price.ToString();
                }
            }
        }

        public ICommand AddServiceCommand { get; set; }

        public ICommand EditServiceCommand { get; set; }

        public AddServiceViewModel(PriceList service)
        {
            _context = new LibraryContext();
            PriceList = new ObservableCollection<PriceList>(_context.PriceLists.ToList());
            SelectedService = service;
            AddServiceCommand = new RelayCommand(AddService);
        }
        private void AddService()
        {
            decimal price;
            if (!string.IsNullOrWhiteSpace(NameService)
                )
            {
                if (!decimal.TryParse(Price, out price))
                {
                    price = 0;
                }
                if (SelectedService != null)
                {
                    var entity = _context.PriceLists.FirstOrDefault(p => p.NameService == SelectedService.NameService);
                    if(_context.PriceLists.FirstOrDefault(p => p.NameService == NameService) != null && NameService != SelectedService.NameService)
                    {
                        MessageBox.Show("Пожалуйста, выберите другое название услуги, это занято.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        entity.NameService = NameService;
                        entity.Price = price;
                        _context.SaveChanges();
                        MessageBox.Show("Услуга успешно отредактирована!", "Редактирование", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    if (_context.PriceLists.FirstOrDefault(p => p.NameService == NameService) == null)
                    {
                        _context.PriceLists.Add(new PriceList { NameService = NameService, Price = price });
                        _context.SaveChanges();
                        NameService = string.Empty;
                        Price = string.Empty;
                        MessageBox.Show("Услуга успешно добавлена!", "Добавление", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Пожалуйста, выберите другое название услуги, это занято.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните обязательное поле названия услуги.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
