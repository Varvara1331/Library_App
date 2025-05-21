using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Library_App.Models;
using Library_App.Data;
using Microsoft.EntityFrameworkCore;

namespace Library_App.ViewModels
{
    public class AddExhibitViewModel : BaseViewModel
    {
        private LibraryContext _context;

        private Exhibit _selectedExhibit;
        public ObservableCollection<Exhibit> Exhibits { get; set; }


        private string _title;
        private string _author;
        private DateOnly? _creationDate;
        private string _subject;
        private string _id;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Author
        {
            get => _author;
            set
            {
                _author = value;
                OnPropertyChanged(nameof(Author));
            }
        }

        public string Subject
        {
            get => _subject;
            set
            {
                _subject = value;
                OnPropertyChanged(nameof(Subject));
            }
        }

        public DateOnly? CreationDate
        {
            get => _creationDate;
            set
            {
                _creationDate = value;
                OnPropertyChanged(nameof(CreationDate));
            }
        }

        public string IdExhibit
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(IdExhibit));
            }
        }

        public Exhibit SelectedExhibit
        {
            get => _selectedExhibit;
            set
            {
                _selectedExhibit = value;
                OnPropertyChanged(nameof(SelectedExhibit));
                if (_selectedExhibit != null)
                {
                    Title = _selectedExhibit.Title;
                    Author = _selectedExhibit.Author;
                    Subject = _selectedExhibit.Subject;
                    CreationDate = _selectedExhibit.CreationDate;
                    IdExhibit = "ЭКСПОНАТ № " + _selectedExhibit.IdExhibit.ToString();
                }
            }
        }

        public ICommand AddExhibitCommand { get; set; }

        public ICommand EditExhibitCommand { get; set; }

        public AddExhibitViewModel(Exhibit exhibit)
        {
            _context = new LibraryContext();
            LoadExhibits();
            SelectedExhibit = exhibit;
            AddExhibitCommand = new RelayCommand(AddExhibit);
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
        private void AddExhibit()
        {
            decimal fine;
            if (!string.IsNullOrWhiteSpace(Title) &&
                !string.IsNullOrWhiteSpace(Author)
                )
            {
                if (DateOnly.TryParse(CreationDate.ToString(), out DateOnly date))
                {
                    if (SelectedExhibit != null)
                    {
                        var entity = _context.Exhibits.FirstOrDefault(p => p.IdExhibit == _selectedExhibit.IdExhibit);
                        entity.Title = Title;
                        entity.Author = Author;
                        entity.Subject = !string.IsNullOrWhiteSpace(Subject) ? Subject.Trim() : null;
                        entity.CreationDate = date;
                        _context.SaveChanges();
                        MessageBox.Show("Экспонат успешно отредактирован!", "Редактирование", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadExhibits();
                    }
                    else
                    {
                        var newExhibit = new Exhibit
                        {
                            Title = Title,
                            Author = Author,
                            Subject = !string.IsNullOrWhiteSpace(Subject) ? Subject.Trim() : null,
                            CreationDate = date
                        };
                        _context.Exhibits.Add(newExhibit);
                        _context.SaveChanges();

                        LoadExhibits();

                        MessageBox.Show($"Экспонат успешно добавлен! ID: {newExhibit.IdExhibit}", "Добавление",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                        Title = string.Empty;
                        Author = string.Empty;
                        Subject = string.Empty;
                        CreationDate = null;
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, заполните поле даты верно.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните обязательные поляназвания и автора.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
