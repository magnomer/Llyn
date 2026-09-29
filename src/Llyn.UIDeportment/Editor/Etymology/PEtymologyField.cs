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
            PEtymologyCommand.PEtymologyCommandAddition, PEtymologyAddHandle));
        CommandBindings.Add(new CommandBinding(
            PEtymologyCommand.PEtymologyCommandRemoval, PEtymologyRemoveHandle));
        CommandBindings.Add(new CommandBinding(
            PEtymologyCommand.PEtymologyCommandEntry, PEtymologyEntryHandle));
        CommandBindings.Add(new CommandBinding(
            PEtymologyCommand.PEtymologyCommandLink, PEtymologyLinkHandle, PEtymologyLinkCheck));
        CommandBindings.Add(new CommandBinding(
            PEtymologyCommand.PEtymologyCommandUnlink, PEtymologyUnlinkHandle, PEtymologyUnlinkCheck));
        PEtymologyField.PEtymologyBox.TextChanged += PEtymologyWriteHandle;
    }

    internal void PEtymologyShow(CEntryDraft draft)
    {
        PEtymologyField.PEtymologyLanguage = draft.CEntryDraftLanguage;
        PEtymologyField.PEtymologyText = draft.CEntryDraftEtymology.CEtymologyDraftText;
        PEtymologyField.PEtymologySourceShow(_qEditor.QEditorArea.CEditorEtymonRead());
        PEtymologyField.PEtymologyMentionShow(
            _pEditorHost,
            draft.CEntryDraftEtymology.CEtymologyDraftText,
            draft.CEntryDraftEtymology.CEtymologyDraftMentions,
            QLocalizationCatalog.QLocalizationTextRead("Mention.Silent"));
    }

    private void PEtymologyWriteHandle(object sender, TextChangedEventArgs e)
    {
        _qEditor.QEditorArea.CEditorCard.CCardEtymologySet(PEtymologyField.PEtymologyBox.Text);
    }

    private void PEtymologyAddHandle(object sender, ExecutedRoutedEventArgs e)
    {
        PEtymon caret = (PEtymon)e.Parameter;
        TextBox box = (TextBox)e.OriginalSource;
        PProspectShow(
            box,
            PMentionSelection.PMentionSelectionPlace(box),
            caret.PEtymonText.Trim(),
            _qEditor.QEditorArea.CEditorLanguage,
            entryId => PEtymonSend(caret, entryId));
    }

    private void PEtymonSend(PEtymon caret, long entryId)
    {
        caret.PEtymonText = string.Empty;
        _qEditor.QEditorArea.CEditorCard.CCardEtymonAdd(entryId);
    }

    private void PEtymologyRemoveHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _qEditor.QEditorArea.CEditorCard.CCardEtymonRemove(((PEtymon)e.Parameter).PEtymonId);
    }

    private void PEtymologyEntryHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _pEditorHost.PWindowAtelier.CAtelierNavigation.CNavigationEntryOpen((long)e.Parameter);
    }

    private void PEtymologyLinkHandle(object sender, ExecutedRoutedEventArgs e)
    {
        TextBox box = PEtymologyField.PEtymologyBox;
        string text = box.Text;
        int start = box.SelectionStart;
        int length = box.SelectionLength;
        PProspectShow(
            box,
            PMentionSelection.PMentionSelectionPlace(box),
            box.SelectedText.Trim(),
            _qEditor.QEditorArea.CEditorLanguage,
            entryId => _qEditor.QEditorArea.CEditorCard.CCardMentionSave(text, start, length, entryId));
    }

    private void PEtymologyUnlinkHandle(object sender, ExecutedRoutedEventArgs e)
    {
        TextBox box = PEtymologyField.PEtymologyBox;
        _qEditor.QEditorArea.CEditorCard.CCardMentionDelete(box.Text, box.SelectionStart, box.SelectionLength);
    }

    private void PEtymologyLinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = !string.IsNullOrWhiteSpace(PEtymologyField.PEtymologyBox.SelectedText);
    }

    private void PEtymologyUnlinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        TextBox box = PEtymologyField.PEtymologyBox;
        e.CanExecute = _qEditor.QEditorArea.CEditorCard.CCardMentionCheck(
            box.Text, box.SelectionStart, box.SelectionLength);
    }
}
