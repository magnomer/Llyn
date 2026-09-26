# LEntryClerkField.cs

## `public static class LEntryClerkField`

The pure field helpers of the entry save.
They sit beside the entry clerk so the clerk stays under its line ceiling.

## `public static IReadOnlyList<LMarkdownBlock> LMarkdownParse(string? text)`

The note text parsed into blocks.

## `public static IReadOnlyList<LPronunciationDraft> LPronunciationReset(IReadOnlyList<LPronunciationDraft> drafts)`

The drafts with every positive id cleared, for a fresh entry that has no stored rows to match.

## `public static void LFormUpdate(LEntryVault entries, long entryId, LEntryDraft draft, List<LRevisionDelta> changes)`

The forms of an entry rewritten from the draft when they differ, with one change recorded.

## `public static bool LFormMatch(IReadOnlyList<LForm> stored, IReadOnlyList<LForm> current)`

Whether two form lists say the same thing, in order.

## `public static void LNoteUpdate(LNoteVault notes, long entryId, LEntryDraft draft, List<LRevisionDelta> changes)`

The note of an entry saved, rewritten or deleted to match the draft, with one change recorded.
An unchanged note writes nothing.
