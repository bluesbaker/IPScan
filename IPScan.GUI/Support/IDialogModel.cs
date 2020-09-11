﻿namespace IPScan.GUI
{
    /// <summary>
    /// Special interface for the ViewControlLinker
    /// </summary>
    /// <typeparam name="VIEW">View/Control</typeparam>
    public interface IDialogModel<VIEW>
    {
        VIEW GetView();
        void SetView(VIEW view);
    }
}