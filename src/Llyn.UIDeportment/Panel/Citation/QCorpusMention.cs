using System;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QCorpus
{
    private readonly PMentionLine _qTranscriptChip = new();

    private void QTranscriptLinkHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox box)
        {
            return;
        }

        QTranscriptLinkShow(box, PMentionSelection.PMentionSelectionRead(box, _qCorpusHost.PWindowDeportment));
    }

    private void QTranscriptLinkShow(
        TextBox box, (int PMentionSelectionOffset, int PMentionSelectionLength) selection)
    {
        if (selection.PMentionSelectionLength == 0)
        {
            return;
        }

        _qCorpusHost.PWindowProspectShow(
            box,
            PMentionSelection.PMentionSelectionPlace(box),
            box.SelectedText.Trim(),
            _qTranscriptLanguage,
            QTranscriptLinkRead(selection.PMentionSelectionOffset, selection.PMentionSelectionLength));
    }

    private Action<long> QTranscriptLinkRead(int offset, int length)
    {
        return entry => QTranscriptQuill.QQuillMentionAdd(0, 0, offset, length, entry);
    }

    private void QTranscriptSenseHandle(object sender, ExecutedRoutedEventArgs e)
    {
        QTranscriptDesk.LDeskPersist();
        QTranscriptSenseShow(QTranscriptMentionFind());
    }

    private void QTranscriptSenseShow(CMentionDraft? mention)
    {
        if (mention is not { CMentionDraftLinked: true })
        {
            return;
        }

        _qCorpusHost.PWindowSenseShow(
            QTranscriptText,
            PMentionSelection.PMentionSelectionPlace(QTranscriptText),
            mention.CMentionDraftEntry,
            QTranscriptSenseRead(mention.CMentionDraftId));
    }

    private Action<long> QTranscriptSenseRead(long mention)
    {
        return sense => QTranscriptQuill.QQuillMentionChange(0, 0, mention, sense);
    }

    private void QTranscriptSilenceHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox box)
        {
            return;
        }

        QTranscriptSilenceRun(PMentionSelection.PMentionSelectionRead(box, _qCorpusHost.PWindowDeportment));
    }

    private void QTranscriptSilenceRun((int PMentionSelectionOffset, int PMentionSelectionLength) selection)
    {
        if (selection.PMentionSelectionLength == 0)
        {
            return;
        }

        QTranscriptQuill.QQuillMentionAdd(
            0, 0, selection.PMentionSelectionOffset, selection.PMentionSelectionLength, 0);
    }

    private void QTranscriptUnlinkHandle(object sender, ExecutedRoutedEventArgs e)
    {
        QTranscriptDesk.LDeskPersist();
        QTranscriptUnlinkRun(
            QSender.QSenderParameterRead<PMentionChip>(e)?.PMentionChipId ?? QTranscriptMentionFind()?.CMentionDraftId);
    }

    private void QTranscriptUnlinkRun(long? mention)
    {
        if (mention is not long id)
        {
            return;
        }

        QTranscriptQuill.QQuillMentionRemove(0, 0, id);
    }

    private void QTranscriptLinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Source is TextBox box
            && _qCorpusHost.PWindowDeportment.LWindowSpanCheck(box.Text, box.SelectionStart, box.SelectionLength);
    }

    private void QTranscriptSenseCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Source is TextBox && QTranscriptMentionFind() is { CMentionDraftEntry: not 0 };
    }

    private void QTranscriptUnlinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Parameter is PMentionChip || (e.Source is TextBox && QTranscriptMentionFind() is not null);
    }

    private CMentionDraft? QTranscriptMentionFind()
    {
        return QTranscriptDesk.LDeskMentionFind(
            0, 0, QTranscriptText.Text, QTranscriptText.SelectionStart, QTranscriptText.SelectionLength);
    }

    private void QTranscriptMentionShow(CExample? example)
    {
        if (example is null)
        {
            _qTranscriptChip.PMentionLineClear();
            return;
        }

        try
        {
            _qTranscriptChip.PMentionLineShow(
                _qCorpusHost.PWindowDeportment,
                example.CExampleText.CStateValueText,
                example.CExampleMention,
                QLocalizationCatalog.QLocalizationTextRead("Mention.Silent"));
        }
        catch (Exception exception)
        {
            _qCorpusHost.PWindowFailureShow("Mention.FindFailed", exception);
        }
    }
}
