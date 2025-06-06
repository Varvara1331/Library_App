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
    public class EventsViewModel : BaseViewModel
    {
        private readonly Frame _navigationFrame;
        private readonly LibraryContext _context;
        private ObservableCollection<Event> __events;
        private Event _selectedEvent;

        public ObservableCollection<Event> Events
        {
            get => __events;
            set { __events = value; OnPropertyChanged(); }
        }

        public Event SelectedEvent
        {
            get => _selectedEvent;
            set { _selectedEvent = value; OnPropertyChanged(); }
        }

        public ICommand LoadEventsCommand { get; }
        public ICommand EditEventCommand { get; }

        public EventsViewModel(Frame frame)
        {
            _navigationFrame = frame;
            _context = new LibraryContext();

            LoadEventsCommand = new RelayCommand(LoadEvents);
            EditEventCommand = new RelayCommand(EditEvent);

            LoadEvents();
        }

        public async void LoadEvents()
        {
            try
            {
                var _events = await _context.Events
            .Include(b => b.EventExhibits)
            .ThenInclude(bb => bb.IdExhibitNavigation)
            .AsNoTracking()
            .ToListAsync();

                Events = new ObservableCollection<Event>(_events);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditEvent()
        {
            _navigationFrame.Navigate(new EventView(SelectedEvent, _navigationFrame));
        }

        public void RemoveEvent(List<Event> _events)
        {
            if (_events.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранные записи?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new LibraryContext())
                    {
                        foreach (var _event in _events)
                        {
                            foreach (var _eventExhibit in _event.EventExhibits)
                            {
                                context.EventExhibits.Remove(_eventExhibit);
                            }
                            context.Events.Remove(_event);
                            context.SaveChanges();
                        }
                        LoadEvents();
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите записи о бронировании для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void ExportEvents()
        {
            if (Events == null || !Events.Any())
            {
                MessageBox.Show("Нет данных для экспорта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Events");

                worksheet.Cell(1, 1).Value = "ДАТА ПРОВЕДЕНИЯ";
                worksheet.Cell(1, 2).Value = "НАЗВАНИЕ";
                worksheet.Cell(1, 3).Value = "МЕСТО ПРОВЕДЕНИЯ";
                worksheet.Cell(1, 4).Value = "ТИП";
                worksheet.Cell(1, 5).Value = "ОПИСАНИЕ";
                worksheet.Cell(1, 6).Value = "ЭКСПОНАТЫ";


                for (int i = 0; i < Events.Count; i++)
                {
                    var Event = Events[i];
                    worksheet.Cell(i + 2, 1).Value = Events[i].EventDate.ToString();
                    worksheet.Cell(i + 2, 2).Value = Events[i].EventName;
                    worksheet.Cell(i + 2, 3).Value = Events[i].EventLocation;
                    worksheet.Cell(i + 2, 4).Value = Events[i].EventType;
                    worksheet.Cell(i + 2, 5).Value = Events[i].Description;
                    string events = string.Empty;
                    if (Event.EventExhibits != null)
                    {
                        foreach (var EventExhibit in Event.EventExhibits)
                        {
                            if (EventExhibit?.IdExhibitNavigation != null)
                            {
                                events += $"{EventExhibit.IdExhibitNavigation.Title} - ({EventExhibit.IdExhibitNavigation.Author}), ";
                            }
                        }
                        if (events.Length > 2) events = events.Remove(events.Length - 2);
                    }
                    worksheet.Cell(i + 2, 6).Value = events;
                }

                FormatWorksheet(worksheet);

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = "Мероприятия",
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