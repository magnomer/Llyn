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
        string detail = exception is LRefusal refusal
            ? PLocalizationTextRead(refusal.LRefusalReason)
            : exception.Message;

        MessageBox.Show(
            this,
            $"{PLocalizationTextRead(key)}\n\n{detail}",
            PLocalizationTextRead("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
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
