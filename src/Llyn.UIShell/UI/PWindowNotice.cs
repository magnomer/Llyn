using System;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PWindow
{
    internal string PLocalizationTextRead(string key)
    {
        return TryFindResource(key) as string ?? key;
    }

    internal void PWindowFailureShow(string key, Exception exception)
    {
        MessageBox.Show(
            this,
            $"{PLocalizationTextRead(key)}\n\n{PWindowDetailRead(exception)}",
            PLocalizationTextRead("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }

    private string PWindowDetailRead(Exception exception)
    {
        if (exception is LRefusal refusal)
        {
            return PLocalizationTextRead(refusal.LRefusalReason);
        }

        return exception.InnerException is null
            ? exception.Message
            : $"{exception.Message}\n{PWindowDetailRead(exception.InnerException)}";
    }

    internal bool PWindowDiscardConfirm()
    {
        return PWindowDiscardConfirm(PInput.PInputChangeCheck() || PList.PListChangeCheck());
    }

    internal bool PWindowDiscardConfirm(bool unsaved)
    {
        if (!unsaved)
        {
            return true;
        }

        return MessageBox.Show(
            this,
            PLocalizationTextRead("Input.DiscardConfirm"),
            PLocalizationTextRead("Terms.Product"),
            MessageBoxButton.YesNo,
            MessageBoxImage.Question) == MessageBoxResult.Yes;
    }
}
