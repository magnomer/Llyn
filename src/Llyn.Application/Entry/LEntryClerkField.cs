using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public static class LEntryClerkField
{
    public static IReadOnlyList<LMarkdownBlock> LMarkdownParse(string? text)
    {
        return LMarkdown.LMarkdownParse(text);
    }

    public static IReadOnlyList<LPronunciationDraft> LPronunciationReset(IReadOnlyList<LPronunciationDraft> drafts)
    {
        ArgumentNullException.ThrowIfNull(drafts);

        List<LPronunciationDraft> renewed = new(drafts.Count);
        foreach (LPronunciationDraft draft in drafts)
        {
            renewed.Add(draft.LPronunciationDraftId > 0 ? draft with { LPronunciationDraftId = 0 } : draft);
        }

        return renewed;
    }

    public static void LFormUpdate(LEntryVault entries, long entryId, LEntryDraft draft, List<LRevisionDelta> changes)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(changes);

        IReadOnlyList<LForm> stored = entries.LEntryFormRead(entryId);
        IReadOnlyList<LForm> current = draft.LEntryDraftForms;

        if (LFormMatch(stored, current))
        {
            return;
        }

        entries.LEntryFormSet(entryId, current);
        changes.Add(new LRevisionDelta(
            entryId,
            "form",
            current.Count == 0 ? "delete" : stored.Count == 0 ? "create" : "update",
            null));
    }

    public static bool LFormMatch(IReadOnlyList<LForm> stored, IReadOnlyList<LForm> current)
    {
        ArgumentNullException.ThrowIfNull(stored);
        ArgumentNullException.ThrowIfNull(current);

        if (stored.Count != current.Count)
        {
            return false;
        }

        for (int index = 0; index < stored.Count; index++)
        {
            if (!string.Equals(stored[index].LFormText, current[index].LFormText, StringComparison.Ordinal)
                || !string.Equals(stored[index].LFormRole, current[index].LFormRole, StringComparison.Ordinal)
                || !string.Equals(stored[index].LFormLocal, current[index].LFormLocal, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    public static void LNoteUpdate(LNoteVault notes, long entryId, LEntryDraft draft, List<LRevisionDelta> changes)
    {
        ArgumentNullException.ThrowIfNull(notes);
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(changes);

        LNote? stored = notes.LNoteRead(entryId);
        string text = LMarkdown.LMarkdownNormalize(draft.LEntryDraftNote);

        if (text.Length == 0)
        {
            if (stored is not null)
            {
                notes.LNoteDelete(entryId);
                changes.Add(new LRevisionDelta(entryId, "note", "delete", null));
            }

            return;
        }

        if (string.Equals(stored?.LNoteText, text, StringComparison.Ordinal))
        {
            return;
        }

        notes.LNoteSave(new LNote(entryId, text));
        changes.Add(new LRevisionDelta(entryId, "note", stored is null ? "create" : "update", null));
    }
}
