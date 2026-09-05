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
        bool unsaved = PInput.PInputChangeCheck() || PLibrary.PLibraryChangeCheck() || PPhonology.PPhonologyChangeCheck()
            || PTaxonomy.PTaxonomyChangeCheck() || PRepertoire.PRepertoireChangeCheck()
            || PCorpus.PCorpusChangeCheck();

        bool store;
        if (unsaved)
        {
            MessageBoxResult answer = MessageBox.Show(
                this,
                PLocalizationTextRead("Input.ExitConfirm"),
                PLocalizationTextRead("Terms.Product"),
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (answer == MessageBoxResult.Cancel)
            {
                return false;
            }

            store = answer == MessageBoxResult.Yes;
        }
        else
        {
            store = false;
        }

        PWindowDraftFinish(store);
        return true;
    }

    private void PWindowDraftFinish(bool store)
    {
        PInput.PInputDraftFinish(store);
        PLibrary.PLibraryDraftFinish(store);
        PPhonology.PPhonologyDraftFinish(store);
        PTaxonomy.PTaxonomyDraftFinish(store);
    }

    internal bool PWindowRemovalConfirm(int usage)
    {
        return PWindowRemovalConfirm(usage, "Situation");
    }

    internal bool PWindowRemovalConfirm(int usage, string scope)
    {
        string count = $"{PLocalizationTextRead($"{scope}.DetachCount")} "
            + usage.ToString(CultureInfo.CurrentCulture);

        string question = usage > 0
            ? $"{PLocalizationTextRead($"{scope}.DetachConfirm")}\n\n{count}"
            : PLocalizationTextRead($"{scope}.DeleteConfirm");

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
