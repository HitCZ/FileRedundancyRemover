using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileRedundancyRemover.Logic
{
    public class RelayCommand : ICommand
    {
        #region Properties

        /// <summary>
        /// Execution logic.
        /// </summary>
        public Action ExecuteAction { get; set; }

        /// <summary>
        /// Determines whether the execution can proceed.
        /// </summary>
        public Func<bool> CanExecuteAction { get; set; }

        #endregion Properties

        #region Constructor

        public RelayCommand(Action execute) : this(execute, null)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            ExecuteAction = execute ?? throw new ArgumentNullException($"{nameof(execute)} is null.");
            CanExecuteAction = canExecute;
        }

        #endregion Constructor

        public bool CanExecute(object parameter) => CanExecuteAction is null || CanExecuteAction();


        public void Execute(object parameter) => Execute();

        public void Execute() => ExecuteAction();

        public async Task ExecuteAsync() => await Task.Run(() => ExecuteAction());

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    public class RelayCommand<T> : ICommand
    {
        #region Properties

        public Action<T> ExecuteAction { get; set; }

        public Predicate<T> CanExecuteAction { get; set; }

        #endregion Properties

        #region Constructors

        public RelayCommand(Action<T> execute) : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            ExecuteAction = execute ?? throw new ArgumentNullException(nameof(execute));
            CanExecuteAction = canExecute;
        }

        #endregion Constructors

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            if (CanExecuteAction is null)
                return true;

            return ConvertToType(parameter, out var convertedValue) && CanExecuteAction(convertedValue);
        }

        public void Execute(object parameter)
        {
            if (ExecuteAction is null)
                return;

           if (ConvertToType(parameter, out var convertedValue))
               ExecuteAction?.Invoke(convertedValue);
           
        }

        public void Execute()
        {
            Execute(null);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (!(CanExecuteAction is null))
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (!(CanExecuteAction is null))
                    CommandManager.RequerySuggested -= value;
            }
        }

        #endregion ICommand Members

        #region Private Methods

        private static bool ConvertToType(object parameter, out T convertedValue)
        {
            if (parameter is null && typeof(T).IsValueType)
            {
                convertedValue = default(T);
                return false;
            }

            if (parameter is null || parameter is T)
            {
                convertedValue = (T) parameter;
                return true;
            }

            try
            {
                convertedValue = (T) Convert.ChangeType(parameter, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (InvalidCastException)
            {
                convertedValue = default(T);
                return false;
            }

            return true;
        }

        #endregion Private Methods
    }
}
