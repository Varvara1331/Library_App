using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Library_App.Models;
using Library_App.Data;
using Microsoft.EntityFrameworkCore;

namespace Library_App.ViewModels
{
    public class AddEventViewModel : BaseViewModel
    {
        private readonly LibraryContext _context;
        private Event _selectedEvent;

        public ObservableCollection<Event> Events { get; set; }
        public ObservableCollection<EventExhibit> EventExhibits { get; set; }
        public ObservableCollection<string> AvailableExhibitsDisplay { get; set; }

        private readonly Dictionary<string, Exhibit> _exhibitsDictionary = new Dictionary<string, Exhibit>();

        private string _selectedExhibitDisplay;
        public string SelectedExhibitDisplay
        {
            get => _selectedExhibitDisplay;
            set
            {
                _selectedExhibitDisplay = value;
                OnPropertyChanged(nameof(SelectedExhibitDisplay));
                if (!string.IsNullOrEmpty(value) && _exhibitsDictionary.TryGetValue(value, out var exhibit))
                {
                    SelectedNewExhibit = exhibit;
                }
                else
                {
                    SelectedNewExhibit = null;
                }
            }
        }

        private Exhibit _selectedNewExhibit;
        public Exhibit SelectedNewExhibit
        {
            get => _selectedNewExhibit;
            private set
            {
                _selectedNewExhibit = value;
                OnPropertyChanged(nameof(SelectedNewExhibit));
            }
        }

        private DateTime? _eventDate;
        private string _eventName;
        private string _location;
        private string _type;
        private string _description;
        private string _idEvent;

        public DateTime? EventDate
        {
            get => _eventDate;
            set
            {
                _eventDate = value;
                OnPropertyChanged(nameof(EventDate));
            }
        }

        public string EventName
        {
            get => _eventName;
            set
            {
                _eventName = value;
                OnPropertyChanged(nameof(EventName));
            }
        }

        public string EventLocation
        {
            get => _location;
            set
            {
                _location = value;
                OnPropertyChanged(nameof(EventLocation));
            }
        }
        public string EventType
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged(nameof(EventType));
            }
        }
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        public string IdEvent
        {
            get => _idEvent;
            set
            {
                _idEvent = value;
                OnPropertyChanged(nameof(IdEvent));
            }
        }

        public Event SelectedEvent
        {
            get => _selectedEvent;
            set
            {
                _selectedEvent = value;
                OnPropertyChanged(nameof(SelectedEvent));

                if (_selectedEvent != null)
                {
                    EventName = _selectedEvent.EventName;
                    EventLocation = _selectedEvent.EventLocation;
                    EventType = _selectedEvent.EventType;
                    Description = _selectedEvent.Description;
                    EventDate = _selectedEvent.EventDate;
                    IdEvent = "МЕРОПРИЯТИЕ № " + _selectedEvent.IdEvent;

                    LoadEventExhibits();
                }
                else
                {
                    ClearFields();
                }
            }
        }

        public ICommand AddEventCommand { get; }
        public ICommand AddEventExhibitCommand { get; }
        public ICommand RemoveEventExhibitCommand { get; }

        public AddEventViewModel(Event _event)
        {
            _context = new LibraryContext();

            Events = new ObservableCollection<Event>();
            EventExhibits = new ObservableCollection<EventExhibit>();
            LoadInitialData();

            AddEventCommand = new RelayCommand(AddEvent);
            AddEventExhibitCommand = new RelayCommand(AddEventExhibit);
            RemoveEventExhibitCommand = new RelayCommandT<EventExhibit>(RemoveEventExhibit);
            SelectedEvent = _event;
        }

        private void LoadInitialData()
        {
            var exhibits = _context.Exhibits.ToList();
            _exhibitsDictionary.Clear();
            foreach (var exhibit in exhibits)
            {
                var displayText = $"{exhibit.Title} - ({exhibit.Author})";
                _exhibitsDictionary[displayText] = exhibit;
            }
            AvailableExhibitsDisplay = new ObservableCollection<string>(_exhibitsDictionary.Keys);
        }

        private void LoadEventExhibits()
        {
            if (_selectedEvent == null) return;

            EventExhibits.Clear();

            var exhibits = _context.EventExhibits
                .Include(bb => bb.IdExhibitNavigation)
                .Where(bb => bb.IdEvent == _selectedEvent.IdEvent)
                .ToList();

            foreach (var exhibit in exhibits)
            {
                EventExhibits.Add(exhibit);
            }
        }

        private void AddEventExhibit()
        {
            if (SelectedNewExhibit == null)
            {
                MessageBox.Show("Выберите экспонат для добавления", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newEventExhibit = new EventExhibit
            {
                IdExhibit = SelectedNewExhibit.IdExhibit,
                IdExhibitNavigation = SelectedNewExhibit
            };

            if (_selectedEvent == null)
            {
                EventExhibits.Add(newEventExhibit);
            }
            else
            {
                newEventExhibit.IdEvent = _selectedEvent.IdEvent;
                EventExhibits.Add(newEventExhibit);
            }

            SelectedExhibitDisplay = null;
            SelectedNewExhibit = null;
        }

        private void RemoveEventExhibit(EventExhibit _eventExhibit)
        {
            if (_eventExhibit == null) return;

            if (MessageBox.Show("Удалить экспонат с мероприятия?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                if (_eventExhibit.IdEventExhibit > 0)
                {
                    var entity = _context.EventExhibits.Find(_eventExhibit.IdEventExhibit);
                    if (entity != null)
                    {
                        _context.EventExhibits.Remove(entity);
                        _context.SaveChanges();
                    }
                }

                EventExhibits.Remove(_eventExhibit);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddEvent()
        {
            if (!DateTime.TryParse(EventDate?.ToString(), out var date))
            {
                MessageBox.Show("Укажите корректную дату проведения", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(EventName))
            {
                MessageBox.Show("Название мероприятия обязательно для заполнения", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(EventLocation))
            {
                MessageBox.Show("Место проведения обязательно для заполнения", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (_selectedEvent == null)
                {
                    var _event = new Event
                    {
                        EventName = EventName,
                        EventLocation = EventLocation,
                        EventType = !string.IsNullOrWhiteSpace(EventType) ? EventType.Trim() : null,
                        Description = !string.IsNullOrWhiteSpace(Description) ? Description.Trim() : null,
                        EventDate = date
                    };

                    _context.Events.Add(_event);
                    _context.SaveChanges();

                    foreach (var _eventExhibit in EventExhibits)
                    {
                        _eventExhibit.IdEvent = _event.IdEvent;
                        _context.EventExhibits.Add(_eventExhibit);
                    }
                    MessageBox.Show($"Мероприятие успешно добавлено! ID: {_event.IdEvent}", "Добавление",
                                MessageBoxButton.OK, MessageBoxImage.Information);

                    _context.SaveChanges();

                    ClearFields();
                }
                else
                {
                    _selectedEvent.EventName = EventName;
                    _selectedEvent.EventLocation = EventLocation;
                    _selectedEvent.EventType = !string.IsNullOrWhiteSpace(EventType) ? EventType.Trim() : null;
                    _selectedEvent.Description = !string.IsNullOrWhiteSpace(Description) ? Description.Trim() : null;
                    _selectedEvent.EventDate = date;

                    var existingExhibitIds = _context.EventExhibits
                        .Where(bb => bb.IdEvent == _selectedEvent.IdEvent)
                        .Select(bb => bb.IdExhibit)
                        .ToList();

                    foreach (var _eventExhibit in EventExhibits)
                    {
                        if (!existingExhibitIds.Contains(_eventExhibit.IdExhibit))
                        {
                            var newEventExhibit = new EventExhibit
                            {
                                IdEvent = _selectedEvent.IdEvent,
                                IdExhibit = _eventExhibit.IdExhibit
                            };

                            try
                            {
                                _context.EventExhibits.Add(newEventExhibit);
                            }
                            catch (DbUpdateException ex)
                            {
                                MessageBox.Show($"Ошибка при добавлении экспоната: {ex.InnerException?.Message}", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                                _context.Entry(newEventExhibit).State = EntityState.Detached;
                                continue;
                            }
                        }
                    }

                    var currentExhibitIds = EventExhibits.Select(bb => bb.IdExhibit).ToList();
                    var exhibitsToRemove = _context.EventExhibits
                        .Where(bb => bb.IdEvent == _selectedEvent.IdEvent &&
                                    !currentExhibitIds.Contains(bb.IdExhibit))
                        .ToList();

                    foreach (var exhibitToRemove in exhibitsToRemove)
                    {
                        _context.EventExhibits.Remove(exhibitToRemove);
                    }

                    _context.SaveChanges();

                    MessageBox.Show("Мероприятие успешно обновлено", "Редактирование",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearFields()
        {
            EventName = string.Empty;
            EventLocation = string.Empty;
            EventType = string.Empty;
            EventName = string.Empty;
            Description = string.Empty;
            EventDate = null;
            EventExhibits.Clear();
            SelectedExhibitDisplay = null;
            SelectedNewExhibit = null;
        }
    }
}