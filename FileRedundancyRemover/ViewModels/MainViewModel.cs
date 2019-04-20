using DirectoryEqualizer.Annotations;
using DirectoryEqualizer.Logic;
using DirectoryEqualizer.Logic.Extensions;
using FileRedundancyRemover.Logic;
using FileRedundancyRemover.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace FileRedundancyRemover.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Fields

        private string sourcePath;
        private string targetPath;

        #endregion Fields


        #region Properties

        /// <summary>
        /// Represents the path to the desired state, i.e. the destination with the files I want included in the target.
        /// </summary>
        public string SourcePath
        {
            get => sourcePath;
            set
            {
                sourcePath = value;
                OnPropertyChanged(nameof(SourcePath));
            }
        }

        /// <summary>
        /// Path to the folder where files will be added / removed.
        /// </summary>
        public string TargetPath
        {
            get => targetPath;
            set
            {
                targetPath = value;
                OnPropertyChanged(nameof(TargetPath));
            }
        }

        #endregion Properties

        #region Commands

        public ICommand BrowseCommand { get; set; }
        public ICommand ConfirmCommand { get; set; }

        #endregion Commands

        #region Constructor

        public MainViewModel()
        {
            InitializeCommands();
        }

        #endregion Constructor

        #region Private Methods

        private void InitializeCommands()
        {
            BrowseCommand = new RelayCommand<bool>(BrowseCommandExecute);
            ConfirmCommand = new RelayCommand(ConfirmCommandExecute, ConfirmCommandCanExecute);
        }

        private void BrowseCommandExecute(bool isSource)
        {
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            if (isSource)
                SourcePath = dialog.SelectedPath;
            else
                TargetPath = dialog.SelectedPath;
        }

        private async void ConfirmCommandExecute()
        {
            var manager = new FileChangesManager();

            var progressView = new ProgressView();
            manager.ProgressChangedAction = progressView.OnProgressChanged;
            progressView.Show();

            await manager.EqualizeFoldersAsync(SourcePath, TargetPath);
        }

        private bool ConfirmCommandCanExecute() => !sourcePath.IsNullOrEmpty() && !targetPath.IsNullOrEmpty();

        #endregion Private Methods

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Members
    }
}
