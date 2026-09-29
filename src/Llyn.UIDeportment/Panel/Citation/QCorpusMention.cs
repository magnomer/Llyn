using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QCorpus
{
    private readonly PMentionLine _qTranscriptChip = new();

    private void QTranscriptLinkObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox box)
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

    private void QTranscriptMeaningRefine(object sender, ExecutedRoutedEventArgs e)
    {
        if (_cCorpus.CCorpusSenseRead(
                QTranscriptText.Text, QTranscriptText.SelectionStart, QTranscriptText.SelectionLength)
            is IReadOnlyList<CMeaning> meanings)
        {
            _qCorpusHost.PMentionMenuShow(
                QTranscriptText,
                PMentionSelection.PMentionSelectionPlace(QTranscriptText),
                meanings,
                QTranscriptSenseObserve);
        }
    }

    private void QTranscriptSenseObserve(FrameworkElement anchor, long sense)
    {
        if (anchor is not TextBox box)
        {
            return;
        }

        _cCorpus.CCorpusSenseSet(box.Text, box.SelectionStart, box.SelectionLength, sense);
    }

    private void QTranscriptSilenceObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox box)
        {
            return;
        }

        _cCorpus.CCorpusMentionAdd(box.Text, box.SelectionStart, box.SelectionLength, 0);
    }

    private void QTranscriptUnlinkObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is PMentionChip chip)
        {
            _cCorpus.CCorpusMentionRemove(chip.PMentionChipId);
        }
        else if (e.Source is TextBox)
        {
            _cCorpus.CCorpusMentionRemove(
                QTranscriptText.Text, QTranscriptText.SelectionStart, QTranscriptText.SelectionLength);
        }
    }

    private void QTranscriptSpanRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Source is TextBox box
            && _qCorpusHost.PWindowAtelier.CAtelierMention.CMentionSpanCheck(
                box.Text, box.SelectionStart, box.SelectionLength);
    }

    private void QTranscriptSenseRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Source is TextBox
            && _cCorpus.CCorpusSenseCheck(
                QTranscriptText.Text, QTranscriptText.SelectionStart, QTranscriptText.SelectionLength);
    }

    private void QTranscriptUnlinkRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Parameter is PMentionChip
            || (e.Source is TextBox
                && _cCorpus.CCorpusMentionCheck(
                    QTranscriptText.Text, QTranscriptText.SelectionStart, QTranscriptText.SelectionLength));
    }

    private void QTranscriptMentionRefine()
    {
        _qTranscriptChip.PMentionLineRefine(_cCorpus.CCorpusMentionRead());
    }
}
