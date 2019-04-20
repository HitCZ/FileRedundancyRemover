using System.Windows;
using FileRedundancyRemover.ViewModels;

namespace FileRedundancyRemover.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainViewModel ViewModel
        {
            get => DataContext as MainViewModel;
            set => DataContext = value;
        }

        public MainView()
        {
            ViewModel = new MainViewModel();
            InitializeComponent();
        }
    }
}
