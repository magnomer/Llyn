# LOutlineNote.cs

## `public static class LOutlineNote`

Carries the note into the outline, where it is already the right language.

## `public static void LOutlineNoteAppend(StringBuilder page, string? markdown)`

Writes the normalized note text as it is, with no escaping.
A heading is demoted two levels, so the note stays under the outline's own `##` band.
Lines inside a code fence are left alone.
