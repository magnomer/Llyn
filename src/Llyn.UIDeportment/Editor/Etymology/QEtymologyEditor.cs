using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QEtymologyEditor
{
    private readonly FrameworkElement _qEtymologySurface;

    private CCard _cCard = null!;

    private CEntry _cEntry = null!;

    private CAtelier _cAtelier = null!;

    private readonly QProspect _qEtymologyProspect;

    internal QEtymologyEditor(FrameworkElement surface, QProspect prospect)
    {
        _qEtymologySurface = surface;
        _qEtymologyProspect = prospect;
        _qEtymologySurface.CommandBindings.Add(new CommandBinding(
            PEtymologyCommand.PEtymologyCommandAddition, QEtymologyAddRefine));
        _qEtymologySurface.CommandBindings.Add(new CommandBinding(
            PEtymologyCommand.PEtymologyCommandRemoval, QEtymologyRemoveObserve));
        _qEtymologySurface.CommandBindings.Add(new CommandBinding(
            PEtymologyCommand.PEtymologyCommandEntry, QEtymologyEntryObserve));
        _qEtymologySurface.CommandBindings.Add(new CommandBinding(
            PEtymologyCommand.PEtymologyCommandLink, QEtymologyLinkRefine, QEtymologyLinkCheck));
        _qEtymologySurface.CommandBindings.Add(new CommandBinding(
            PEtymologyCommand.PEtymologyCommandUnlink, QEtymologyUnlinkObserve, QEtymologyUnlinkCheck));
        QEtymologyField.QEtymologyEditable = true;
        QEtymologyField.QEtymologyCard = true;
        QEtymologyField.QEtymologyBox.TextChanged += QEtymologyWriteObserve;
        QEtymologyField.CommandBindings.Add(new CommandBinding(
            PMentionCommand.PMentionCommandPick, QEtymologyPickObserve));
    }

    internal QEtymology QEtymologyField => QContract.QContractFind<QEtymology>(_qEtymologySurface, "PEtymologyField");

    internal void QEtymologyIntroduce(CCard card, CEntry entry, CAtelier atelier)
    {
        _cCard = card;
        _cEntry = entry;
        _cAtelier = atelier;
        entry.CEntryDraftChanged += QEtymologyRefine;
        entry.CEntryDraftChanged += QEtymologyMentionRefine;
    }

    internal static void QEtymologyCaretRefine(PEtymon caret)
    {
        caret.PEtymonText = string.Empty;
    }

    private void QEtymologyRefine(CEntryDraft draft)
    {
        QEtymologyField.QEtymologyText = draft.CEntryDraftEtymology.CEtymologyDraftText;
        QEtymologyField.QEtymologySourceShow(_cEntry.CEntryEtymonRead()
            .Select(static target => new PEtymon(
                target.CTranslationTargetId, target.CTranslationTargetHeadword, target.CTranslationTargetLanguage))
            .ToList(),
            true);
    }

    private void QEtymologyMentionRefine(CEntryDraft _)
    {
        QEtymologyField.QEtymologyLine.PMentionLineRefine(
            QMentionChip.QMentionChipCreate(_cCard.CCardEtymologyRead()));
    }

    private void QEtymologyWriteObserve(object sender, TextChangedEventArgs e)
    {
        _cCard.CCardEtymologySet(QEtymologyField.QEtymologyBox.Text);
    }

    private void QEtymologyAddRefine(object sender, ExecutedRoutedEventArgs e)
    {
        TextBox box = (TextBox)e.OriginalSource;
        _qEtymologyProspect.QProspectPlaceRefine(box, PMentionSelection.PMentionSelectionPlace(box));
        _qEtymologyProspect.QProspectOpenRefine(
            _cCard.CCardMentionRead(((PEtymon)e.Parameter).PEtymonText));
    }

    private void QEtymologyRemoveObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _cCard.CCardEtymonRemove(((PEtymon)e.Parameter).PEtymonId);
    }

    private void QEtymologyEntryObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _cAtelier.CAtelierNavigation.CNavigationEntryOpen((long)e.Parameter);
    }

    private void QEtymologyLinkRefine(object sender, ExecutedRoutedEventArgs e)
    {
        TextBox box = QEtymologyField.QEtymologyBox;
        _qEtymologyProspect.QProspectPlaceRefine(box, PMentionSelection.PMentionSelectionPlace(box));
        _qEtymologyProspect.QProspectOpenRefine(_cCard.CCardMentionRead(box.SelectedText));
    }

    private void QEtymologyPickObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is TextBox box)
        {
            _cCard.CCardMentionSave(
                box.Text, box.SelectionStart, box.SelectionLength, e.Parameter as long?);
        }
    }

    private void QEtymologyUnlinkObserve(object sender, ExecutedRoutedEventArgs e)
    {
        TextBox box = QEtymologyField.QEtymologyBox;
        _cCard.CCardMentionDelete(box.Text, box.SelectionStart, box.SelectionLength);
    }

    private void QEtymologyLinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        TextBox box = QEtymologyField.QEtymologyBox;
        e.CanExecute = _cAtelier.CAtelierMention.CMentionSpanCheck(
            box.Text, box.SelectionStart, box.SelectionLength);
    }

    private void QEtymologyUnlinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        TextBox box = QEtymologyField.QEtymologyBox;
        e.CanExecute = _cCard.CCardMentionCheck(
            box.Text, box.SelectionStart, box.SelectionLength);
    }
}
