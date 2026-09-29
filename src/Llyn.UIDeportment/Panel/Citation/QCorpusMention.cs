using System;
using System.Windows;
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

        QTranscriptLinkShow(box, PMentionSelection.PMentionSelectionRead(box, _qCorpusHost.PWindowAtelier));
    }

    private void QTranscriptLinkShow(
        TextBox box, (int PMentionSelectionOffset, int PMentionSelectionLength) selection)
    {
        if (selection.PMentionSelectionLength == 0)
        {
            return;
        }

        QCorpusEditor.PProspectPlaceRefine(box, PMentionSelection.PMentionSelectionPlace(box));
        _cCorpus.CCorpusMentionOpen(box.SelectedText);
    }

    private void QTranscriptPickObserve(TextBox box, long entryId)
    {
        _cCorpus.CCorpusMentionAdd(box.Text, box.SelectionStart, box.SelectionLength, entryId);
    }

    private void QTranscriptSenseHandle(object sender, ExecutedRoutedEventArgs e)
    {
        QTranscriptSenseShow(QTranscriptMentionFind(true));
    }

    private void QTranscriptSenseShow(CMentionDraft? mention)
    {
        if (mention is not { CMentionDraftLinked: true })
        {
            return;
        }

        _qCorpusHost.PWindowSenseRefine(
            QTranscriptText,
            PMentionSelection.PMentionSelectionPlace(QTranscriptText),
            mention.CMentionDraftEntry,
            QTranscriptSenseRead(mention.CMentionDraftId));
    }

    private Action<FrameworkElement, long> QTranscriptSenseRead(long mention)
    {
        return (_, sense) => QTranscriptQuill?.LQuillMentionSet(0, 0, mention, sense);
    }

    private void QTranscriptSilenceHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox box)
        {
            return;
        }

        QTranscriptSilenceRun(PMentionSelection.PMentionSelectionRead(box, _qCorpusHost.PWindowAtelier));
    }

    private void QTranscriptSilenceRun((int PMentionSelectionOffset, int PMentionSelectionLength) selection)
    {
        if (selection.PMentionSelectionLength == 0)
        {
            return;
        }

        QTranscriptQuill?.LQuillMentionAdd(
            0, 0, selection.PMentionSelectionOffset, selection.PMentionSelectionLength, 0);
    }

    private void QTranscriptUnlinkHandle(object sender, ExecutedRoutedEventArgs e)
    {
        QTranscriptUnlinkRun(
            QSender.QSenderParameterRead<PMentionChip>(e)?.PMentionChipId
            ?? QTranscriptMentionFind(true)?.CMentionDraftId);
    }

    private void QTranscriptUnlinkRun(long? mention)
    {
        if (mention is not long id)
        {
            return;
        }

        QTranscriptQuill?.LQuillMentionRemove(0, 0, id);
    }

    private void QTranscriptLinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Source is TextBox box
            && _qCorpusHost.PWindowAtelier.CAtelierMention.CMentionSpanCheck(
                box.Text, box.SelectionStart, box.SelectionLength);
    }

    private void QTranscriptSenseCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Source is TextBox && QTranscriptMentionFind(false) is { CMentionDraftEntry: not 0 };
    }

    private void QTranscriptUnlinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Parameter is PMentionChip
            || (e.Source is TextBox && QTranscriptMentionFind(false) is not null);
    }

    private CMentionDraft? QTranscriptMentionFind(bool settled)
    {
        return _qCorpusHost.PWindowAtelier.CAtelierMention.CMentionFind(
            QTranscriptDesk, 0, 0,
            QTranscriptText.Text, QTranscriptText.SelectionStart, QTranscriptText.SelectionLength, settled);
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
                _qCorpusHost.PWindowAtelier,
                example.CExampleText.CStateValueText,
                example.CExampleMention,
                QLocalizationCatalog.QLocalizationTextRead("Mention.Silent"));
        }
        catch (Exception exception)
        {
            _qCorpusHost.PWindowFailureRefine("Mention.FindFailed", exception);
        }
    }
}
