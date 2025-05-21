using System.Windows;
using System.Windows.Controls;
using Library_App.ViewModels;
using System.Windows.Navigation;
using Library_App.Models;

namespace Library_App.Views
{
    /// <summary>
    /// Логика взаимодействия для ExhibitView.xaml
    /// </summary>
    public partial class ExhibitView : Page
    {
        private Exhibit _selectedExhibit;
        private Frame _frame;
        public ExhibitView(Exhibit exhibit, Frame frame)
        {
            InitializeComponent();
            _frame = frame;
            _selectedExhibit = exhibit;
            this.DataContext = new AddExhibitViewModel(_selectedExhibit);
        }
        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ExhibitsView(_frame));
        }
    }
}
