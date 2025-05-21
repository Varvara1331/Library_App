using Library_App.Models;
using Library_App.Views;
using System.Windows.Controls;

namespace Library_App.Services
{
    public class NavigationService : INavigationService
    {
        private readonly Frame _mainFrame;

        public NavigationService(Frame mainFrame)
        {
            _mainFrame = mainFrame;
        }

        public void NavigateToReaderView(Reader reader)
        {
            _mainFrame.Navigate(new ReaderView(reader, _mainFrame));
        }
    }
}
