using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private PEtymology PEtymologyField => (PEtymology)FindName(nameof(PEtymologyField));

    internal void PEtymologyAttach()
    {
        CommandBindings.Add(new CommandBinding(
            PEtymologyCommand.PEtymologyCommandAddition, PEtymologyAddRefine));
        CommandBindings.Add(new CommandBinding(
            PEtymologyCommand.PEtymologyCommandRemoval, PEtymologyRemoveObserve));
        CommandBindings.Add(new CommandBinding(
            PEtymologyCommand.PEtymologyCommandEntry, PEtymologyEntryObserve));
        CommandBindings.Add(new CommandBinding(
            PEtymologyCommand.PEtymologyCommandLink, PEtymologyLinkRefine, PEtymologyLinkCheck));
        CommandBindings.Add(new CommandBinding(
            PEtymologyCommand.PEtymologyCommandUnlink, PEtymologyUnlinkObserve, PEtymologyUnlinkCheck));
        PEtymologyField.PEtymologyBox.TextChanged += PEtymologyWriteObserve;
    }

    internal void PEtymologyRefine(CEntryDraft draft)
    {
        PEtymologyField.PEtymologyLanguage = draft.CEntryDraftLanguage;
        PEtymologyField.PEtymologyText = draft.CEntryDraftEtymology.CEtymologyDraftText;
        PEtymologyField.PEtymologySourceShow(_qEditor.QEditorArea.CEditorEtymonRead()
            .Select(static target => new PEtymon(
                target.CTranslationTargetId, target.CTranslationTargetHeadword, target.CTranslationTargetLanguage))
            .ToList());
        PEtymologyField.PEtymologyMentionShow(
            _pEditorHost,
            draft.CEntryDraftEtymology.CEtymologyDraftText,
            draft.CEntryDraftEtymology.CEtymologyDraftMentions,
            QLocalizationCatalog.QLocalizationTextRead("Mention.Silent"));
    }

    private void PEtymologyWriteObserve(object sender, TextChangedEventArgs e)
    {
        _qEditor.QEditorArea.CEditorCard.CCardEtymologySet(PEtymologyField.PEtymologyBox.Text);
    }

    private void PEtymologyAddRefine(object sender, ExecutedRoutedEventArgs e)
    {
        TextBox box = (TextBox)e.OriginalSource;
        PProspectPlaceRefine(box, PMentionSelection.PMentionSelectionPlace(box));
        PProspectOpenRefine(
            _qEditor.QEditorArea.CEditorCard.CCardMentionRead(((PEtymon)e.Parameter).PEtymonText));
    }

    private static void PEtymonCaretRefine(PEtymon caret)
    {
        caret.PEtymonText = string.Empty;
    }

    private void PEtymologyRemoveObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _qEditor.QEditorArea.CEditorCard.CCardEtymonRemove(((PEtymon)e.Parameter).PEtymonId);
    }

    private void PEtymologyEntryObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _pEditorHost.PWindowAtelier.CAtelierNavigation.CNavigationEntryOpen((long)e.Parameter);
    }

    private void PEtymologyLinkRefine(object sender, ExecutedRoutedEventArgs e)
    {
        TextBox box = PEtymologyField.PEtymologyBox;
        PProspectPlaceRefine(box, PMentionSelection.PMentionSelectionPlace(box));
        PProspectOpenRefine(_qEditor.QEditorArea.CEditorCard.CCardMentionRead(box.SelectedText));
    }

    private void PEtymologyUnlinkObserve(object sender, ExecutedRoutedEventArgs e)
    {
        TextBox box = PEtymologyField.PEtymologyBox;
        _qEditor.QEditorArea.CEditorCard.CCardMentionDelete(box.Text, box.SelectionStart, box.SelectionLength);
    }

    private void PEtymologyLinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        TextBox box = PEtymologyField.PEtymologyBox;
        e.CanExecute = _pEditorHost.PWindowAtelier.CAtelierMention.CMentionSpanCheck(
            box.Text, box.SelectionStart, box.SelectionLength);
    }

    private void PEtymologyUnlinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        TextBox box = PEtymologyField.PEtymologyBox;
        e.CanExecute = _qEditor.QEditorArea.CEditorCard.CCardMentionCheck(
            box.Text, box.SelectionStart, box.SelectionLength);
    }
}
