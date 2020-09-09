using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace IPScan.GUI
{
    /// <summary>
    /// Базовый класс для модели представления диалогового окна.
    /// Реализует интерфейс INotifyPropertyChanged и команды для
    /// завершения диалога. 
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
        /// "Одобренный" диалог
        /// </summary>
        /// <param name="result">Результат диалога</param>
        public void OnDialogAccepted(object result) => DialogAccepted?.Invoke(result);
        /// <summary>
        /// "Не одобренный" диалог
        /// </summary>
        /// <param name="result">Результат диалога</param>
        public void OnDialogRejected(object result) => DialogRejected?.Invoke(result);
        #endregion


        #region Commands
        /// <summary>
        /// Команда «Принять» и закрыть диалог(положительно)
        /// </summary>
        public ICommand AcceptCommand
        {
            get => new RelayCommand(result => {
                // Callback on accept
                OnDialogAccepted(result);

                // Close dialog and return 'true'
                Window window = GetView() as Window;
                if (window != null)
                    window.DialogResult = true;
            }, AcceptCanExecute);
        }

        /// <summary>
        /// Команда «Отклонить» и закрыть диалог
        /// </summary>
        public ICommand RejectCommand
        {
            get => new RelayCommand(result => {
                // Callback on reject
                OnDialogRejected(result);

                // Close dialog?
                Window window = GetView() as Window;
                if (window != null)
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
