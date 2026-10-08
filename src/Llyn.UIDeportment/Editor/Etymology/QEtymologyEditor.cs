using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QEtymologyEditor
{
    private readonly FrameworkElement _qEtymologySurface;

    private CEditor _cEditor = null!;

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

    internal void QEtymologyIntroduce(CEditor editor, CAtelier atelier)
    {
        _cEditor = editor;
        _cAtelier = atelier;
        editor.CEditorEntry.CEntryDraftChanged += QEtymologyRefine;
        editor.CEditorEntry.CEntryDraftChanged += QEtymologyMentionRefine;
    }

    internal static void QEtymologyCaretRefine(PEtymon caret)
    {
        caret.PEtymonText = string.Empty;
    }

    private void QEtymologyRefine(CEntryDraft draft)
    {
        QEtymologyField.QEtymologyText = draft.CEntryDraftEtymology.CEtymologyDraftText;
        QEtymologyField.QEtymologySourceShow(_cEditor.CEditorEntry.CEntryEtymonRead()
            .Select(static target => new PEtymon(
                target.CTranslationTargetId, target.CTranslationTargetHeadword, target.CTranslationTargetLanguage))
            .ToList(),
            true);
    }

    private void QEtymologyMentionRefine(CEntryDraft _)
    {
        QEtymologyField.QEtymologyLine.PMentionLineRefine(
            QMentionChip.QMentionChipCreate(_cEditor.CEditorCard.CCardEtymologyRead()));
    }

    private void QEtymologyWriteObserve(object sender, TextChangedEventArgs e)
    {
        _cEditor.CEditorCard.CCardEtymologySet(QEtymologyField.QEtymologyBox.Text);
    }

    private void QEtymologyAddRefine(object sender, ExecutedRoutedEventArgs e)
    {
        TextBox box = (TextBox)e.OriginalSource;
        _qEtymologyProspect.QProspectPlaceRefine(box, PMentionSelection.PMentionSelectionPlace(box));
        _qEtymologyProspect.QProspectOpenRefine(
            _cEditor.CEditorCard.CCardMentionRead(((PEtymon)e.Parameter).PEtymonText));
    }

    private void QEtymologyRemoveObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _cEditor.CEditorCard.CCardEtymonRemove(((PEtymon)e.Parameter).PEtymonId);
    }

    private void QEtymologyEntryObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _cAtelier.CAtelierNavigation.CNavigationEntryOpen((long)e.Parameter);
    }

    private void QEtymologyLinkRefine(object sender, ExecutedRoutedEventArgs e)
    {
        TextBox box = QEtymologyField.QEtymologyBox;
        _qEtymologyProspect.QProspectPlaceRefine(box, PMentionSelection.PMentionSelectionPlace(box));
        _qEtymologyProspect.QProspectOpenRefine(_cEditor.CEditorCard.CCardMentionRead(box.SelectedText));
    }

    private void QEtymologyPickObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is TextBox box)
        {
            _cEditor.CEditorCard.CCardMentionSave(
                box.Text, box.SelectionStart, box.SelectionLength, e.Parameter as long?);
        }
    }

    private void QEtymologyUnlinkObserve(object sender, ExecutedRoutedEventArgs e)
    {
        TextBox box = QEtymologyField.QEtymologyBox;
        _cEditor.CEditorCard.CCardMentionDelete(box.Text, box.SelectionStart, box.SelectionLength);
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
        e.CanExecute = _cEditor.CEditorCard.CCardMentionCheck(
            box.Text, box.SelectionStart, box.SelectionLength);
    }
}
