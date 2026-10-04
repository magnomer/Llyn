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

        _qCorpusEditor.QEditorProspect.QProspectPlaceRefine(box, PMentionSelection.PMentionSelectionPlace(box));
        _cTranscript.CTranscriptMentionOpen(box.SelectedText);
    }

    private void QTranscriptPickObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is TextBox box)
        {
            _cTranscript.CTranscriptMentionAdd(
                box.Text, box.SelectionStart, box.SelectionLength, e.Parameter as long?);
        }
    }

    private void QTranscriptMeaningRefine(object sender, ExecutedRoutedEventArgs e)
    {
        if (_cTranscript.CTranscriptSenseRead(
                QTranscriptText.Text, QTranscriptText.SelectionStart, QTranscriptText.SelectionLength)
            is CMentionSense sense)
        {
            _qCorpusHost.QMentionMeaningRefine(
                QTranscriptText,
                PMentionSelection.PMentionSelectionPlace(QTranscriptText),
                sense,
                QTranscriptSenseObserve);
        }
    }

    private void QTranscriptSenseObserve(FrameworkElement anchor, long sense)
    {
        if (anchor is not TextBox box)
        {
            return;
        }

        _cTranscript.CTranscriptSenseSet(box.Text, box.SelectionStart, box.SelectionLength, sense);
    }

    private void QTranscriptSilenceObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox box)
        {
            return;
        }

        _cTranscript.CTranscriptSilenceSet(box.Text, box.SelectionStart, box.SelectionLength);
    }

    private void QTranscriptUnlinkObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is PMentionChip chip)
        {
            _cTranscript.CTranscriptMentionRemove(chip.PMentionChipId);
        }
        else if (e.Source is TextBox)
        {
            _cTranscript.CTranscriptMentionRemove(
                QTranscriptText.Text, QTranscriptText.SelectionStart, QTranscriptText.SelectionLength);
        }
    }

    private void QTranscriptSpanRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Source is TextBox box
            && _qCorpusHost.QWindowAtelier.CAtelierMention.CMentionSpanCheck(
                box.Text, box.SelectionStart, box.SelectionLength);
    }

    private void QTranscriptSenseRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Source is TextBox
            && _cTranscript.CTranscriptSenseCheck(
                QTranscriptText.Text, QTranscriptText.SelectionStart, QTranscriptText.SelectionLength);
    }

    private void QTranscriptUnlinkRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Parameter is PMentionChip
            || (e.Source is TextBox
                && _cTranscript.CTranscriptMentionCheck(
                    QTranscriptText.Text, QTranscriptText.SelectionStart, QTranscriptText.SelectionLength));
    }

    private void QTranscriptMentionRefine()
    {
        _qTranscriptChip.PMentionLineRefine(QMentionChip.QMentionChipCreate(_cTranscript.CTranscriptMentionRead()));
    }
}
