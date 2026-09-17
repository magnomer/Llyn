# LEnginePortraitCard.cs

## `public sealed partial class LEngine`

The banded sections of an entry likeness: the card bands, the links here, and the note.
Each builder adds one section to the list, or nothing when the entry has nothing to show.

## `private static void LEngineBandAdd(List<LPortraitSection> sections, string heading, IReadOnlyList<LPortraitSection> cards)`

A headed band over the given card sections, skipped when there are none.

## `private void LEngineIncomingAdd(List<LPortraitSection> sections, long entryId, LPortraitLabel label)`

One usage section per entry that links here, under one band.
The link carries the headword and language.
The line carries the card kind as its label and the card title as its text.
A writer draws the four parts in its own row shape, as the display's incoming rows do.

## `private static void LEngineNoteAdd(List<LPortraitSection> sections, LEntryDraft draft, LPortraitLabel label)`

The note as a Markdown section, skipped when none is written.
