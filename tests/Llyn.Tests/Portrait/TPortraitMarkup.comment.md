# TPortraitMarkup.cs

## `public sealed class TPortraitMarkup`

Covers the markup export, which is the only format that must read back.

## `public void MarkupDraftFormat_WrittenEntry_ReadsBackAsTheSameDraft()`

The writer and the reader are separate code, and only a round trip proves they agree.

## `public void MarkupDraftFormat_UnreadableField_ReadsBackAsUnreadable()`

An unset field and an unreadable one are different, and the file must keep them apart.

## `public void MarkupDraftFormat_ClosingTagInsideText_LeavesTheDocumentReadable()`

The reader unescapes nothing, so a closing tag inside text would end an element early.
Everything after it would be lost, which is worse than losing the tag.

## `private static LEntryDraft TPortraitMarkupCreate()`

One entry holding every element the writer emits.
