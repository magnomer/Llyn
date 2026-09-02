using System;
using System.Globalization;
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
        return PWindowDiscardConfirm(PInput.PInputChangeCheck() || PList.PListChangeCheck() || PSound.PSoundChangeCheck()
            || PTag.PTagChangeCheck() || PSituation.PSituationChangeCheck());
    }

    internal bool PWindowRemovalConfirm(int usage)
    {
        string count = $"{PLocalizationTextRead("Situation.DetachCount")} "
            + usage.ToString(CultureInfo.CurrentCulture);

        string question = usage > 0
            ? $"{PLocalizationTextRead("Situation.DetachConfirm")}\n\n{count}"
            : PLocalizationTextRead("Situation.DeleteConfirm");

        return MessageBox.Show(
            this,
            question,
            PLocalizationTextRead("Terms.Product"),
            MessageBoxButton.YesNo,
            MessageBoxImage.Question) == MessageBoxResult.Yes;
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
