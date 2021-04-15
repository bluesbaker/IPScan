using System;
using System.Windows;
using System.Windows.Input;

namespace IPScan.GUI.Support
{
    #region Delegates for WPF command methods
    public delegate void ExecuteHandler(object parameter);
    public delegate bool CanExecuteHandler(object parameter);
    #endregion

    #region RelayCommand
    /// <summary>Implementation ICommand for WPF</summary>
    public class RelayCommand : ICommand
    {
        private readonly CanExecuteHandler _canExecute;
        private readonly ExecuteHandler _onExecute;
        private readonly EventHandler _requerySuggested;

        /// <summary>Command state change notification event</summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>Constructor of Command</summary>
        /// <param name="execute">Executable method</param>
        /// <param name="canExecute">Permission method for execution</param>
        public RelayCommand(ExecuteHandler execute, CanExecuteHandler canExecute = null)
        {
            _onExecute = execute;
            _canExecute = canExecute;
            _requerySuggested = (o, e) => Invalidate();
            CommandManager.RequerySuggested += _requerySuggested;
        }
        
        public void Invalidate()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }), null);
        }
        
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute.Invoke(parameter);
        public void Execute(object parameter) => _onExecute?.Invoke(parameter);
    }
    #endregion
}
