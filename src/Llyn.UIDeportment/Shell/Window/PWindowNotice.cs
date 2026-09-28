using System;
using System.Windows;

namespace Llyn.UIDeportment;

public partial class PWindow
{
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
        if (PWindowAtelier.CAtelierLedger.CLedgerNoticeRead(exception) is string reason)
        {
            return QLocalizationCatalog.QLocalizationTextRead(reason);
        }

        string unexpected = QLocalizationCatalog.QLocalizationTextRead("Notice.Unexpected");
        string? recorded = PWindowAtelier.CAtelierLedger.CLedgerAuditRecord(exception);

        return recorded is null
            ? unexpected
            : $"{unexpected}\n\n{QLocalizationCatalog.QLocalizationTextRead("Notice.Recorded")}\n{recorded}";
    }

    internal bool PWindowDiscardConfirm()
    {
        return PWindowAtelier.CAtelierQuitConfirm(
            [
                PInput.PInputChangeCheck,
                _qLibrary.QLibraryChangeCheck,
                _qPhonology.QPhonologyChangeCheck,
                _qTaxonomy.QTaxonomyChangeCheck,
                _qTenor.QTenorChangeCheck,
                _qXiesheng.QXieshengChangeCheck,
                PYunjing.PYunjingChangeCheck,
                _qFavorite.QFavoriteChangeCheck,
                _qCorpus.QCorpusChangeCheck,
                _qRepertoire.QRepertoireChangeCheck,
                _qReference.QReferenceChangeCheck,
                _qGuild.QGuildChangeCheck,
            ],
            [
                PInput.PInputDraftFinish,
                _qLibrary.QLibraryDraftFinish,
                _qPhonology.QPhonologyDraftFinish,
                _qTaxonomy.QTaxonomyDraftFinish,
                _qTenor.QTenorDraftFinish,
                _qXiesheng.QXieshengDraftFinish,
                PYunjing.PYunjingDraftFinish,
                _qFavorite.QFavoriteDraftFinish,
                _qCorpus.QCorpusDraftFinish,
                _qRepertoire.QRepertoireDraftFinish,
                _qReference.QReferenceDraftFinish,
                _qGuild.QGuildDraftFinish,
            ],
            PWindowEnvoy);
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
