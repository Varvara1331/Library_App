using Library_App.Models;
using Library_App.ViewModels;
using System.Windows.Controls;

namespace Library_App.Views
{
    /// <summary>
    /// Логика взаимодействия для ExhibitsView.xaml
    /// </summary>
    public partial class ExhibitsView : UserControl
    {
        private ExhibitsViewModel _viewModel;
        public ExhibitsView(Frame frame)
        {
            InitializeComponent();
            _viewModel = new ExhibitsViewModel(frame);
            this.DataContext = _viewModel;
        }

        public void RemoveSelectedExhibits()
        {
            var selectedExhibits = ExhibitsDataGrid.SelectedItems.Cast<Exhibit>().ToList();
            _viewModel.RemoveExhibit(selectedExhibits);
        }

        public void ExportExhibits()
        {
            _viewModel.ExportExhibits();
        }
    }
}