using System;
using System.Globalization;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PWindow
{
    internal void PWindowFailureShow(string key)
    {
        MessageBox.Show(
            this,
            PLocalizationCatalog.PLocalizationTextRead(key),
            PLocalizationCatalog.PLocalizationTextRead("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }

    internal bool PWindowUnreadableConfirm()
    {
        return MessageBox.Show(
            this,
            PLocalizationCatalog.PLocalizationTextRead("Notice.UnreadableDrop"),
            PLocalizationCatalog.PLocalizationTextRead("Terms.Product"),
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning) == MessageBoxResult.Yes;
    }

    internal void PWindowFailureShow(string key, Exception exception)
    {
        MessageBox.Show(
            this,
            $"{PLocalizationCatalog.PLocalizationTextRead(key)}\n\n{PWindowDetailRead(exception)}",
            PLocalizationCatalog.PLocalizationTextRead("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }

    private string PWindowDetailRead(Exception exception)
    {
        if (PWindowRefusalRead(exception) is string refused)
        {
            return refused;
        }

        string unexpected = PLocalizationCatalog.PLocalizationTextRead("Notice.Unexpected");
        string? recorded = _lWindow.LWindowAuditRecord(exception);

        return recorded is null
            ? unexpected
            : $"{unexpected}\n\n{PLocalizationCatalog.PLocalizationTextRead("Notice.Recorded")}\n{recorded}";
    }

    private string? PWindowRefusalRead(Exception exception)
    {
        if (exception is LRefusal refusal)
        {
            return PLocalizationCatalog.PLocalizationTextRead(refusal.LRefusalReason);
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
            (PYunjing.PYunjingChangeCheck, PYunjing.PYunjingDraftFinish),
            (PFavorite.PFavoriteChangeCheck, PFavorite.PFavoriteDraftFinish),
            (PCorpus.PCorpusChangeCheck, PCorpus.PCorpusDraftFinish),
            (PRepertoire.PRepertoireChangeCheck, PRepertoire.PRepertoireDraftFinish),
            (PReference.PReferenceChangeCheck, PReference.PReferenceDraftFinish),
            (PGuild.PGuildChangeCheck, PGuild.PGuildDraftFinish)
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
            PSLeaveAnswer answer = PSLeave.PSLeaveShow(this);
            if (answer == PSLeaveAnswer.PSLeaveAnswerStay)
            {
                return false;
            }

            store = answer == PSLeaveAnswer.PSLeaveAnswerStore;
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
        string count = string.Concat(
            PLocalizationCatalog.PLocalizationTextRead($"{scope}.DetachCount"),
            " ",
            usage.ToString(CultureInfo.CurrentCulture));

        string question = usage > 0
            ? $"{PLocalizationCatalog.PLocalizationTextRead($"{scope}.DetachConfirm")}\n\n{count}"
            : PLocalizationCatalog.PLocalizationTextRead($"{scope}.DeleteConfirm");

        return MessageBox.Show(
            this,
            question,
            PLocalizationCatalog.PLocalizationTextRead("Terms.Product"),
            MessageBoxButton.YesNo,
            MessageBoxImage.Question) == MessageBoxResult.Yes;
    }

    internal bool PWindowUnionConfirm(string dropped, string kept)
    {
        string confirm = PLocalizationCatalog.PLocalizationTextRead("Guild.MergeConfirm");
        string question = $"{confirm}\n\n{dropped.Trim()} \u2192 {kept.Trim()}";

        return MessageBox.Show(
            this,
            question,
            PLocalizationCatalog.PLocalizationTextRead("Terms.Product"),
            MessageBoxButton.YesNo,
            MessageBoxImage.Question) == MessageBoxResult.Yes;
    }

    internal bool PWindowDeleteConfirm()
    {
        return MessageBox.Show(
            this,
            PLocalizationCatalog.PLocalizationTextRead("Scribe.DeleteConfirm"),
            PLocalizationCatalog.PLocalizationTextRead("Terms.Product"),
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning) == MessageBoxResult.Yes;
    }

    internal bool PWindowDiscardConfirm(bool unsaved, Func<bool, bool> finish)
    {
        ArgumentNullException.ThrowIfNull(finish);

        if (!unsaved)
        {
            return true;
        }

        return PSLeave.PSLeaveShow(this) switch
        {
            PSLeaveAnswer.PSLeaveAnswerStore => finish(true),
            PSLeaveAnswer.PSLeaveAnswerDiscard => true,
            _ => false,
        };
    }
}
