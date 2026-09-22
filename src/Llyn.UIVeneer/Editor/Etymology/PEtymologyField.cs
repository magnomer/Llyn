using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    private IReadOnlyList<LMentionDraft> _pEtymologyMention = [];

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

    private void PEtymologyShow(LEntryDraft draft, IReadOnlyDictionary<long, LTranslationTarget> targets)
    {
        LEtymologyDraft etymology = draft.LEntryDraftEtymology;
        _pEtymologyMention = etymology.LEtymologyDraftMentions;

        List<PEtymologyChip> chips = [];
        foreach (long id in etymology.LEtymologyDraftEtymons)
        {
            if (targets.TryGetValue(id, out LTranslationTarget? target))
            {
                chips.Add(new PEtymologyChip(
                    id, target.LTranslationTargetHeadword, target.LTranslationTargetLanguage, true));
            }
        }

        PEtymologyField.PEtymologyLanguage = draft.LEntryDraftLanguage;
        PEtymologyField.PEtymologyText = etymology.LEtymologyDraftText;
        PEtymologyField.PEtymologySourceShow(chips);
        PEtymologyField.PEtymologyMentionShow(
            _pEtymologyMention, PLocalizationCatalog.PLocalizationTextRead("Mention.Silent"));
    }

    private void PEtymologyWriteHandle(object sender, TextChangedEventArgs e)
    {
        PEditorRequestDefer(new LRequestEtymologyText(PEditorDraft, PEtymologyField.PEtymologyBox.Text));
    }

    private void PEtymologyAddHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is not PEtymologyCaret caret || e.OriginalSource is not TextBox box)
        {
            return;
        }

        string word = caret.PEtymologyCaretText.Trim();
        if (word.Length == 0)
        {
            return;
        }

        _pEditorHost.PWindowProspectShow(
            box,
            PMentionSelection.PMentionSelectionPlace(box),
            word,
            _lEditor.LEditorLanguage,
            entryId => PEtymonSend(caret, entryId));
    }

    private void PEtymonSend(PEtymologyCaret caret, long entryId)
    {
        if (entryId <= 0)
        {
            return;
        }

        caret.PEtymologyCaretText = string.Empty;
        PEditorRequestSend(new LRequestEtymonAddition(PEditorDraft, entryId, int.MaxValue));
    }

    private void PEtymologyRemoveHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is PEtymologyChip chip)
        {
            PEditorRequestSend(new LRequestEtymonRemoval(PEditorDraft, chip.PEtymologyChipId));
        }
    }

    private void PEtymologyEntryHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is PEtymologyChip chip)
        {
            _pEditorHost.PWindowEntryShow(chip.PEtymologyChipId);
        }
    }

    private void PEtymologyLinkHandle(object sender, ExecutedRoutedEventArgs e)
    {
        TextBox box = PEtymologyField.PEtymologyBox;
        (int offset, int length) = PMentionSelection.PMentionSelectionRead(box, _pEditorHost.PWindowDeportment);
        if (length == 0)
        {
            return;
        }

        _pEditorHost.PWindowProspectShow(
            box,
            PMentionSelection.PMentionSelectionPlace(box),
            box.SelectedText.Trim(),
            _lEditor.LEditorLanguage,
            entryId => PEtymologyMentionSend(offset, length, entryId));
    }

    private void PEtymologyMentionSend(int offset, int length, long entryId)
    {
        if (entryId > 0)
        {
            PEditorRequestSend(new LRequestEtymologyMention(PEditorDraft, offset, length, entryId));
        }
    }

    private void PEtymologyUnlinkHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (PEtymologySpanFind() is not LMentionDraft mention)
        {
            return;
        }

        PEditorRequestSend(new LRequestEtymologyMention(
            PEditorDraft, mention.LMentionDraftOffset, mention.LMentionDraftLength, 0));
    }

    private void PEtymologyLinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = PMentionSelection.PMentionSelectionRead(
            PEtymologyField.PEtymologyBox, _pEditorHost.PWindowDeportment).PMentionSelectionLength > 0;
    }

    private void PEtymologyUnlinkCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = PEtymologySpanFind() is not null;
    }

    private LMentionDraft? PEtymologySpanFind()
    {
        (int offset, int length) = PMentionSelection.PMentionSelectionRead(
            PEtymologyField.PEtymologyBox, _pEditorHost.PWindowDeportment);
        return PMentionSelection.PMentionSelectionFind(_pEtymologyMention, offset, length);
    }
}
