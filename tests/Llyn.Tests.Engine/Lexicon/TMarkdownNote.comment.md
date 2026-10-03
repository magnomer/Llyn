# TMarkdownNote.cs
Hash: `14407d8ac4c9c600`

## `public sealed class TMarkdownNote`

Covers the note dialect.
That is what the reader makes of note text and what the engine stores.

## `public void MarkdownNormalize_WindowsLineEndsAndEdgeBlanks_YieldsCanonical()`

Two notes that differ only in line ends or trailing blanks must compare equal, else every pause would write.

## `public void MarkdownParse_EveryBlockShape_YieldsOneBlockEach()`

Each block shape is read once, in order, with its level and its verbatim code kept.

## `public void MarkdownParse_BoldItalicCodeLink_YieldsStyledSpans()`

The styles nest and an escaped marker is text.

## `public void MarkdownParse_StrayMarkerAndSnakeCase_KeepsThemLiteral()`

A lone asterisk or an underscore inside a word must not vanish from a note.

## `public void EntrySave_NoteWithWindowsLineEnds_StoresNormalizedMarkdown()`

The create path normalizes the note like the update path does.

## `public void NoteUpdate_WindowsLineEnds_StoresNormalizedMarkdown()`

The update path normalizes as well, so no path stores a raw note.

## `private static LEntryDraft TMarkdownNoteCreate(string note)`

A minimal saveable draft carrying only `note`.

## `public void MarkdownParse_NumberedRuns_NumbersEachRunFromOne()`

Each run of numbered lines is counted from one, and a block outside a run counts zero.
A later list after plain text must restart, or the reader would show a continued count.

## `public void MarkdownSpanAddress_WebAndLocalLinks_KeepsOnlyWebAddresses()`

A link span carries an address only when it is a web address, and local file links carry none.
The reader opens only what carries an address, so a note cannot launch a local path.
