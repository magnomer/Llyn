using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QTranscriptMention
{
    private readonly UserControl _qTranscriptMentionScope;

    private readonly QMentionLine _qTranscriptChip = new();

    private CTranscript _cTranscript = null!;

    private CAtelier _cAtelier = null!;

    private QProspect _qTranscriptMentionProspect = null!;

    private QMentionMenu _qTranscriptMentionMenu = null!;

    internal QTranscriptMention(UserControl scope)
    {
        ArgumentNullException.ThrowIfNull(scope);

        _qTranscriptMentionScope = scope;

        QTranscriptView.CommandBindings.Add(new CommandBinding(
            QMentionCommand.QMentionCommandLink, QTranscriptLinkObserve, QTranscriptSpanRefine));
        QTranscriptView.CommandBindings.Add(new CommandBinding(
            QMentionCommand.QMentionCommandChoose, QTranscriptMeaningRefine, QTranscriptSenseRefine));
        QTranscriptView.CommandBindings.Add(new CommandBinding(
            QMentionCommand.QMentionCommandSilence, QTranscriptSilenceObserve, QTranscriptSpanRefine));
        QTranscriptView.CommandBindings.Add(new CommandBinding(
            QMentionCommand.QMentionCommandUnlink, QTranscriptUnlinkObserve, QTranscriptUnlinkRefine));
        QTranscriptView.CommandBindings.Add(new CommandBinding(
            QMentionCommand.QMentionCommandPick, QTranscriptPickObserve));
    }

    private Grid QTranscriptView => QContract.QContractFind<Grid>(_qTranscriptMentionScope, "PTranscript");

    private TextBox QTranscriptText => QContract.QContractFind<TextBox>(_qTranscriptMentionScope, "PTranscriptText");

    private ItemsControl QTranscriptMentionLine =>
        QContract.QContractFind<ItemsControl>(_qTranscriptMentionScope, "PTranscriptMentionLine");

    internal void QTranscriptMentionIntroduce(
        CCorpus corpus, CAtelier atelier, QProspect prospect, QMentionMenu mentionMenu)
    {
        ArgumentNullException.ThrowIfNull(corpus);
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(prospect);
        ArgumentNullException.ThrowIfNull(mentionMenu);

        _cTranscript = corpus.CCorpusTranscript;
        _cAtelier = atelier;
        _qTranscriptMentionProspect = prospect;
        _qTranscriptMentionMenu = mentionMenu;

        QTranscriptMentionLine.ItemsSource = _qTranscriptChip.QMentionLineChip;
        QLookItem.QLookItemAttach(QTranscriptMentionLine, QMentionChip.QMentionChipRefine);
        corpus.CCorpusTranscriptChanged += QTranscriptMentionRefine;
        _cTranscript.CTranscriptDraftChanged += QTranscriptMentionRefine;
        _cTranscript.CTranscriptMentionOffered += prospect.QProspectOpenRefine;
    }

    private void QTranscriptLinkObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox box)
        {
            return;
        }

        _qTranscriptMentionProspect.QProspectPlaceRefine(box, QMentionSelection.QMentionSelectionPlace(box));
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
            QMentionAsk ask = _qTranscriptMentionMenu.QMentionMeaningRefine(
                QTranscriptText, QMentionSelection.QMentionSelectionPlace(QTranscriptText), sense);
            ask.QMentionAskChosen += QTranscriptSenseObserve;
        }
    }

    private void QTranscriptSenseObserve(FrameworkElement _, long sense)
    {
        _cTranscript.CTranscriptSenseSet(
            QTranscriptText.Text, QTranscriptText.SelectionStart, QTranscriptText.SelectionLength, sense);
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
        if (e.Parameter is QMentionChip chip)
        {
            _cTranscript.CTranscriptMentionRemove(chip.QMentionChipId);
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
            && _cAtelier.CAtelierMention.CMentionSpanCheck(box.Text, box.SelectionStart, box.SelectionLength);
    }

    private void QTranscriptSenseRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Source is TextBox
            && _cTranscript.CTranscriptSenseCheck(
                QTranscriptText.Text, QTranscriptText.SelectionStart, QTranscriptText.SelectionLength);
    }

    private void QTranscriptUnlinkRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Parameter is QMentionChip
            || (e.Source is TextBox
                && _cTranscript.CTranscriptMentionCheck(
                    QTranscriptText.Text, QTranscriptText.SelectionStart, QTranscriptText.SelectionLength));
    }

    private void QTranscriptMentionRefine(CExample _)
    {
        _qTranscriptChip.QMentionLineRefine(QMentionChip.QMentionChipCreate(_cTranscript.CTranscriptMentionRead()));
    }
}
