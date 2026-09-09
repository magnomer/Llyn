# TPortraitMarkup.cs

## `public sealed class TPortraitMarkup`

Covers the markup export: the tags the writer emits for one entry draft.
The writer still writes the retired format, which the reader no longer accepts.
So these tests pin the text it produces rather than what reading it back gives.
The round trip returns when the writer is rewritten against the new format.

## `public void MarkupDraftFormat_WrittenEntry_ReturnsEveryTag()`

Every field of an entry and of both card kinds reaches the file.
A field the writer drops is a field an export loses, whatever the reader can do with it.

## `public void MarkupDraftFormat_UnreadableField_WritesAnEmptyTag()`

An unset field and an unreadable one are different, and the file must keep them apart.
An unreadable title is written as an empty tag and an unset expression is not written at all.

## `public void MarkupDraftFormat_ClosingTagInsideText_DropsThatTag()`

The reader unescapes nothing, so a closing tag inside text would end an element early.
Everything after it would be lost, which is worse than losing the tag.

## `private static LEntryDraft TPortraitMarkupCreate()`

One entry holding every element the writer emits.
