using IPScan.GUI.Support;
using IPScan.SUP;
using System;
using System.Windows;
using System.Windows.Input;

namespace IPScan.GUI
{
    /// <summary>
    /// Basic viewmodel of dialog or control
    /// </summary>
    /// <typeparam name="VIEW">System.Windows.Controls.Control</typeparam>
    public class DialogViewModel<VIEW> : NPCBase, IDialogModel<VIEW> where VIEW: System.Windows.Controls.Control, new()
    {
        #region Properties
        /// <summary>
        /// Разрешение для AcceptCommand
        /// </summary>
        public Func<object, bool> AcceptCanExecute { get; set; } = (_) => true;

        /// <summary>
        /// Разрешение для RejectCommand
        /// </summary>
        public Func<object, bool> RejectCanExecute { get; set; } = (_) => true;

        /// <summary>
        /// Событие «принятого» диалога
        /// </summary>
        public event Action<object> DialogAccepted;       

        /// <summary>
        /// Событие «отклоненного» диалога
        /// </summary>
        public event Action<object> DialogRejected;       
        #endregion


        #region Public methods
        /// <summary>
        /// Accepted response
        /// </summary>
        /// <param name="result">Callback result</param>
        public void OnDialogAccepted(object result) => DialogAccepted?.Invoke(result);

        /// <summary>
        /// Rejected response
        /// </summary>
        /// <param name="result">Callback result</param>
        public void OnDialogRejected(object result) => DialogRejected?.Invoke(result);
        #endregion


        #region Commands
        /// <summary>
        /// Accepted command
        /// </summary>
        public ICommand AcceptCommand
        {
            get => new RelayCommand(result => {
                // Callback on accept
                OnDialogAccepted(result);

                // Close dialog and return 'true'
                if (GetView() is Window window)
                    window.DialogResult = true;
            }, AcceptCanExecute);
        }

        /// <summary>
        /// Rejected command
        /// </summary>
        public ICommand RejectCommand
        {
            get => new RelayCommand(result => {
                // Callback on reject
                OnDialogRejected(result);

                // Close dialog?
                if (GetView() is Window window)
                    window.Close();
            }, RejectCanExecute);
        }
        #endregion


        // Implementation of IDialogModel<VIEW>
        VIEW _view;
        public VIEW GetView() => _view;
        public void SetView(VIEW view) => _view = view;
    }
}
