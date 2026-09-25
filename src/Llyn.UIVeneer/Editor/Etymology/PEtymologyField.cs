using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PEditor
{
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

    private void PEtymologyShow(LEntryDraft draft)
    {
        PEtymologyField.PEtymologyLanguage = draft.LEntryDraftLanguage;
        PEtymologyField.PEtymologyText = draft.LEntryDraftEtymology.LEtymologyDraftText;
        PEtymologyField.PEtymologySourceShow(_lEditor.LEditorLectern.LLecternEtymonRead(draft));
        PEtymologyField.PEtymologyMentionShow(
            _pEditorHost.PWindowDeportment,
            draft.LEntryDraftEtymology.LEtymologyDraftText,
            draft.LEntryDraftEtymology.LEtymologyDraftMentions,
            PLocalizationCatalog.PLocalizationTextRead("Mention.Silent"));
    }

    private void PEtymologyWriteHandle(object sender, TextChangedEventArgs e)
    {
        _lEditor.LEditorCard.LCardEtymologySet(PEtymologyField.PEtymologyBox.Text);
    }

    private void PEtymologyAddHandle(object sender, ExecutedRoutedEventArgs e)
    {
        PEtymon caret = (PEtymon)e.Parameter;
        TextBox box = (TextBox)e.OriginalSource;
        _pEditorHost.PWindowProspectShow(
            box,
            PMentionSelection.PMentionSelectionPlace(box),
            caret.PEtymonText.Trim(),
            _lEditor.LEditorLanguage,
            entryId => PEtymonSend(caret, entryId));
    }

    private void PEtymonSend(PEtymon caret, long entryId)
    {
        caret.PEtymonText = string.Empty;
        _lEditor.LEditorCard.LCardEtymonAdd(entryId);
    }

    private void PEtymologyRemoveHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _lEditor.LEditorCard.LCardEtymonRemove(((PEtymon)e.Parameter).PEtymonId);
    }

    private void PEtymologyEntryHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _pEditorHost.PWindowEntryShow((long)e.Parameter);
    }

    private void PEtymologyLinkHandle(object sender, ExecutedRoutedEventArgs e)
    {
        TextBox box = PEtymologyField.PEtymologyBox;
        string text = box.Text;
        int start = box.SelectionStart;
        int length = box.SelectionLength;
        _pEditorHost.PWindowProspectShow(
            box,
            PMentionSelection.PMentionSelectionPlace(box),
            box.SelectedText.Trim(),
            _lEditor.LEditorLanguage,
            entryId => _lEditor.LEditorCard.LCardMentionSave(text, start, length, entryId));
    }

    private void PEtymologyUnlinkHandle(object sender, ExecutedRoutedEventArgs e)
    {
        TextBox box = PEtymologyField.PEtymologyBox;
        _lEditor.LEditorCard.LCardMentionDelete(box.Text, box.SelectionStart, box.SelectionLength);
    }

    private void PEtymologyLinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = !string.IsNullOrWhiteSpace(PEtymologyField.PEtymologyBox.SelectedText);
    }

    private void PEtymologyUnlinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        TextBox box = PEtymologyField.PEtymologyBox;
        e.CanExecute = _lEditor.LEditorCard.LCardMentionCheck(box.Text, box.SelectionStart, box.SelectionLength);
    }
}
