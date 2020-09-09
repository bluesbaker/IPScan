using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IPScan.GUI
{
    /// <summary>
    /// Create a composition by <typeparamref name="VIEW"/> and <typeparamref name="VIEWMODEL"/> with the "link" from <typeparamref name="VIEW"/>
    /// </summary>
    /// <typeparam name="VIEW">View or UserControl</typeparam>
    /// <typeparam name="VIEWMODEL">ViewModel or DataContext</typeparam>
    public static class ViewControlLinker<VIEW, VIEWMODEL> where VIEW: System.Windows.Controls.Control, new() where VIEWMODEL: IDialogModel<VIEW>, new()
    {
        public static VIEW GetComposite(Action<VIEW, VIEWMODEL> action = null)
        {
            return ViewLinker<VIEW, VIEWMODEL>.GetView((view, vm) =>
            {
                vm.SetView(view);
                action?.Invoke(view, vm);
            });
        }
    }

    /// <summary>
    /// Create a composition by <typeparamref name="VIEW"/> and <typeparamref name="VIEWMODEL"/>
    /// </summary>
    /// <typeparam name="VIEW">View or UserControl</typeparam>
    /// <typeparam name="VIEWMODEL">Data context</typeparam>
    public static class ViewLinker<VIEW, VIEWMODEL> where VIEWMODEL : new() where VIEW : System.Windows.Controls.Control, new()
    {
        /// <summary>
        /// Get the composition
        /// </summary>
        /// <param name="viewControl">View or UserControl</param>
        /// <param name="viewModel">ViewModel or DataContext</param>
        /// <param name="action">Callback action</param>
        /// <returns><typeparamref name="VIEW"/></returns>
        public static VIEW GetView(VIEW viewControl, VIEWMODEL viewModel, Action<VIEWMODEL> action)
        {
            action(viewModel);
            viewControl.DataContext = viewModel;
            return viewControl;
        }

        /// <summary>
        /// Get the composition
        /// </summary>
        /// <param name="action">Callback action with <typeparamref name="VIEWMODEL"/></param>
        /// <returns><typeparamref name="VIEW"/></returns>
        public static VIEW GetView(Action<VIEWMODEL> action)
        {
            VIEWMODEL viewModel = new VIEWMODEL();
            action(viewModel);
            return new VIEW
            {   
                DataContext = viewModel
            };
        }

        /// <summary>
        /// Get the composition
        /// </summary>
        /// <param name="action">Callback action with <typeparamref name="VIEW"/> and <typeparamref name="VIEWMODEL"/></param>
        /// <returns><typeparamref name="VIEW"/></returns>
        public static VIEW GetView(Action<VIEW, VIEWMODEL> action)
        {
            VIEWMODEL viewModel = new VIEWMODEL();
            VIEW view = new VIEW()
            {
                DataContext = viewModel
            };
            action(view, viewModel);
            return view;
        }        
    }
}
