using FileRedundancyRemover.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using FileRedundancyRemover.Logic;

namespace FileRedundancyRemover.Views
{
    /// <summary>
    /// Interaction logic for ProgressView.xaml
    /// </summary>
    public partial class ProgressView : INotifyPropertyChanged
    {
        public double Progress
        {
            get => (double)GetValue(ProgressProperty);
            set
            {
                SetValue(ProgressProperty, value);
                OnPropertyChanged(nameof(Progress));
            }
        }

        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(double), typeof(ProgressView));

        public string ProgressText
        {
            get => (string)GetValue(ProgressTextProperty);
            set
            {
                SetValue(ProgressTextProperty, value);
                OnPropertyChanged(nameof(ProgressText));
            }
        }

        public static readonly DependencyProperty ProgressTextProperty =
            DependencyProperty.Register("ProgressText", typeof(string), typeof(ProgressView));

        public ProgressView()
        {
            InitializeComponent();
        }

        #region Public Methods

        public void OnProgressChanged(ProgressState state)
        {
            Progress = state.Progress;
            ProgressText = state.ProgressText;
        }

        #endregion Public Methods

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
