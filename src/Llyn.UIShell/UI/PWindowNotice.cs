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

    internal string? PLocalizationTextFind(string key)
    {
        return TryFindResource(key) as string;
    }

    internal void PWindowFailureShow(string key)
    {
        MessageBox.Show(
            this,
            PLocalizationTextRead(key),
            PLocalizationTextRead("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }

    internal PWindowStored PWindowCommitRun<PWindowStored>(long held, Func<long, PWindowStored> commit)
    {
        ArgumentNullException.ThrowIfNull(commit);

        try
        {
            return commit(held);
        }
        catch (LRefusal refusal) when (refusal.LRefusalReason == LRefusal.LRefusalUnreadable)
        {
            if (!PWindowUnreadableConfirm())
            {
                throw;
            }

            _lEngine.LEngineDraftSweep(held);
            return commit(held);
        }
    }

    private bool PWindowUnreadableConfirm()
    {
        return MessageBox.Show(
            this,
            PLocalizationTextRead("Notice.UnreadableDrop"),
            PLocalizationTextRead("Terms.Product"),
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning) == MessageBoxResult.Yes;
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
        if (PWindowRefusalRead(exception) is string refused)
        {
            return refused;
        }

        string unexpected = PLocalizationTextRead("Notice.Unexpected");
        string? recorded = _lEngine.LEngineAuditRecord(exception);

        return recorded is null
            ? unexpected
            : $"{unexpected}\n\n{PLocalizationTextRead("Notice.Recorded")}\n{recorded}";
    }

    private string? PWindowRefusalRead(Exception exception)
    {
        if (exception is LRefusal refusal)
        {
            return PLocalizationTextRead(refusal.LRefusalReason);
        }

        return exception.InnerException is null ? null : PWindowRefusalRead(exception.InnerException);
    }

    private (Func<bool> PWindowEditorPending, Func<bool, bool> PWindowEditorClosure)[] PWindowEditorRead()
    {
        return
        [
            (PInput.PInputChangeCheck, PInput.PInputDraftFinish),
            (PLibrary.PLibraryChangeCheck, PLibrary.PLibraryDraftFinish),
            (PPhonology.PPhonologyChangeCheck, PPhonology.PPhonologyDraftFinish),
            (PTaxonomy.PTaxonomyChangeCheck, PTaxonomy.PTaxonomyDraftFinish),
            (PTenor.PTenorChangeCheck, PTenor.PTenorDraftFinish),
            (PFavorite.PFavoriteChangeCheck, PFavorite.PFavoriteDraftFinish),
            (PCorpus.PCorpusChangeCheck, PCorpus.PCorpusDraftFinish),
            (PRepertoire.PRepertoireChangeCheck, PRepertoire.PRepertoireDraftFinish),
            (PReference.PReferenceChangeCheck, PReference.PReferenceDraftFinish)
        ];
    }

    internal bool PWindowDiscardConfirm()
    {
        (Func<bool> PWindowEditorPending, Func<bool, bool> PWindowEditorClosure)[] editors = PWindowEditorRead();

        bool unsaved = false;
        foreach ((Func<bool> check, Func<bool, bool> _) in editors)
        {
            unsaved |= check();
        }

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

        return PWindowDraftFinish(editors, store);
    }

    private bool PWindowDraftFinish(
        (Func<bool> PWindowEditorPending, Func<bool, bool> PWindowEditorClosure)[] editors, bool store)
    {
        bool finished = true;
        foreach ((Func<bool> _, Func<bool, bool> finish) in editors)
        {
            finished &= finish(store);
        }

        return finished;
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

    internal bool PWindowDeleteConfirm()
    {
        return MessageBox.Show(
            this,
            PLocalizationTextRead("Scribe.DeleteConfirm"),
            PLocalizationTextRead("Terms.Product"),
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning) == MessageBoxResult.Yes;
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
