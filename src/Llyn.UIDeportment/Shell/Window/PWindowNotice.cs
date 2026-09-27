using System;
using System.Globalization;
using System.Windows;

namespace Llyn.UIDeportment;

public partial class PWindow
{
    internal bool PWindowUnreadableConfirm()
    {
        return PWindowEnvoy.CEnvoyConfirm("Notice.UnreadableDrop");
    }

    internal void PWindowFailureShow(string key, Exception exception)
    {
        MessageBox.Show(
            _pWindowSurface,
            $"{QLocalizationCatalog.QLocalizationTextRead(key)}\n\n{PWindowDetailRead(exception)}",
            QLocalizationCatalog.QLocalizationTextRead("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }

    private string PWindowDetailRead(Exception exception)
    {
        if (_lWindow.LWindowWorkspace.QWorkspaceNoticeRead(exception) is string reason)
        {
            return QLocalizationCatalog.QLocalizationTextRead(reason);
        }

        string unexpected = QLocalizationCatalog.QLocalizationTextRead("Notice.Unexpected");
        string? recorded = _lWindow.LWindowWorkspace.QWorkspaceAuditRecord(exception);

        return recorded is null
            ? unexpected
            : $"{unexpected}\n\n{QLocalizationCatalog.QLocalizationTextRead("Notice.Recorded")}\n{recorded}";
    }

    private (Func<bool> PWindowEditorPending, Func<bool, bool> PWindowEditorClosure)[] PWindowEditorRead()
    {
        return
        [
            (PInput.PInputChangeCheck, PInput.PInputDraftFinish),
            (_qLibrary.QLibraryChangeCheck, _qLibrary.QLibraryDraftFinish),
            (_qPhonology.QPhonologyChangeCheck, _qPhonology.QPhonologyDraftFinish),
            (_qTaxonomy.QTaxonomyChangeCheck, _qTaxonomy.QTaxonomyDraftFinish),
            (_qTenor.QTenorChangeCheck, _qTenor.QTenorDraftFinish),
            (_qXiesheng.QXieshengChangeCheck, _qXiesheng.QXieshengDraftFinish),
            (PYunjing.PYunjingChangeCheck, PYunjing.PYunjingDraftFinish),
            (_qFavorite.QFavoriteChangeCheck, _qFavorite.QFavoriteDraftFinish),
            (_qCorpus.QCorpusChangeCheck, _qCorpus.QCorpusDraftFinish),
            (_qRepertoire.QRepertoireChangeCheck, _qRepertoire.QRepertoireDraftFinish),
            (_qReference.QReferenceChangeCheck, _qReference.QReferenceDraftFinish),
            (_qGuild.QGuildChangeCheck, _qGuild.QGuildDraftFinish)
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
            QSLeaveAnswer answer = QSLeave.QSLeaveShow(_pWindowSurface);
            if (answer == QSLeaveAnswer.QSLeaveAnswerStay)
            {
                return false;
            }

            store = answer == QSLeaveAnswer.QSLeaveAnswerStore;
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
            QLocalizationCatalog.QLocalizationTextRead($"{scope}.DetachCount"),
            " ",
            usage.ToString(CultureInfo.CurrentCulture));

        string question = usage > 0
            ? $"{QLocalizationCatalog.QLocalizationTextRead($"{scope}.DetachConfirm")}\n\n{count}"
            : QLocalizationCatalog.QLocalizationTextRead($"{scope}.DeleteConfirm");

        return MessageBox.Show(
            _pWindowSurface,
            question,
            QLocalizationCatalog.QLocalizationTextRead("Terms.Product"),
            MessageBoxButton.YesNo,
            MessageBoxImage.Question) == MessageBoxResult.Yes;
    }

    internal bool PWindowUnionConfirm(string dropped, string kept)
    {
        string confirm = QLocalizationCatalog.QLocalizationTextRead("Guild.MergeConfirm");
        string question = $"{confirm}\n\n{dropped.Trim()} \u2192 {kept.Trim()}";

        return MessageBox.Show(
            _pWindowSurface,
            question,
            QLocalizationCatalog.QLocalizationTextRead("Terms.Product"),
            MessageBoxButton.YesNo,
            MessageBoxImage.Question) == MessageBoxResult.Yes;
    }

    internal bool PWindowDeleteConfirm()
    {
        return PWindowEnvoy.CEnvoyConfirm("Scribe.DeleteConfirm");
    }

    internal bool PWindowDiscardConfirm(bool unsaved, Func<bool, bool> finish)
    {
        ArgumentNullException.ThrowIfNull(finish);

        if (!unsaved)
        {
            return true;
        }

        return QSLeave.QSLeaveShow(_pWindowSurface) switch
        {
            QSLeaveAnswer.QSLeaveAnswerStore => finish(true),
            QSLeaveAnswer.QSLeaveAnswerDiscard => true,
            _ => false,
        };
    }
}
