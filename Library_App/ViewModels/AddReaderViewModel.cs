using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Library_App.Models;
using Library_App.Data;

namespace Library_App.ViewModels
{
    public class AddReaderViewModel : BaseViewModel
    {
        private LibraryContext _context;

        private Reader _selectedReader;
        public ObservableCollection<Reader> Readers { get; set; }


        private string _firstName;
        private string _lastName;
        private DateOnly? _birthDate;
        private string _email;
        private string _telephone;
        private string _fine;
        private string _ticket;

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        public string Telephone
        {
            get => _telephone;
            set
            {
                _telephone = value;
                OnPropertyChanged(nameof(Telephone));
            }
        }

        public DateOnly? BirthDate
        {
            get => _birthDate;
            set
            {
                _birthDate = value;
                OnPropertyChanged(nameof(BirthDate));
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string Fine
        {
            get => _fine;
            set
            {
                _fine = value;
                OnPropertyChanged(nameof(Fine));
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

        public Reader SelectedReader
        {
            get => _selectedReader;
            set
            {
                _selectedReader = value;
                OnPropertyChanged(nameof(SelectedReader));
                if (_selectedReader != null)
                {
                    FirstName = _selectedReader.FirstName;
                    LastName = _selectedReader.LastName;
                    Email = _selectedReader.Email;
                    Telephone = _selectedReader.Telephone;
                    Fine = _selectedReader.Fine.ToString();
                    BirthDate =_selectedReader.BirthDate;
                    ReaderTicket = "ЧИТАТЕЛЬСКИЙ БИЛЕТ № " + _selectedReader.ReaderTicket.ToString();
                }
            }
        }

        public ICommand AddReaderCommand { get; set; }

        public ICommand EditReaderCommand { get; set; }

        public AddReaderViewModel(Reader reader)
        {
            _context = new LibraryContext();
            Readers = new ObservableCollection<Reader>(_context.Readers.ToList());
            SelectedReader = reader;
            AddReaderCommand = new RelayCommand(AddReader);
        }
        private void AddReader()
        {
            decimal fine;
            if (!string.IsNullOrWhiteSpace(FirstName) &&
                !string.IsNullOrWhiteSpace(LastName)
                )
            {
                if (DateOnly.TryParse(BirthDate.ToString(), out DateOnly birthDate) &&
                DateTime.Now.Year - birthDate.Year > 14)
                {
                    if(!decimal.TryParse(Fine, out fine))
                    {
                        fine = 0;
                    }
                    if (SelectedReader != null)
                    {
                        var entity = _context.Readers.FirstOrDefault(p => p.ReaderTicket == _selectedReader.ReaderTicket);
                        entity.FirstName = FirstName;
                        entity.LastName = LastName;
                        entity.Email = Email;
                        entity.Telephone = Telephone;
                        entity.Fine = fine;
                        entity.BirthDate = birthDate;
                        _context.SaveChanges();
                        MessageBox.Show("Читатель успешно отредактирован!", "Редактирование", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        Reader reader = new Reader { FirstName = FirstName, LastName = LastName, Email = Email, Telephone = Telephone, Fine = fine, BirthDate = birthDate };
                        _context.Readers.Add(reader);
                        _context.SaveChanges();
                        FirstName = string.Empty;
                        LastName = string.Empty;
                        Email = string.Empty;
                        Telephone = string.Empty;
                        Fine = string.Empty;
                        MessageBox.Show($"Читатель успешно добавлен! № читательского билета - {reader.ReaderTicket}", "Добавление", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, заполните поле даты верно (возраст читателя должен быть выше 14).", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните обязательные поля фамилии и имени.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
