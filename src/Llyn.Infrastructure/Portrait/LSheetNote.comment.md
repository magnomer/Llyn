# LSheetNote.cs

## `public static class LSheetNote`

Renders the note's Markdown blocks as HTML inside the sheet's note box.

## `public static void LSheetNoteAppend(StringBuilder page, string? markdown)`

Walks the parsed blocks and writes one element per block.
Consecutive items of one kind share a list, and a deeper item opens a nested list inside the last.
A heading is written two levels down, so the note never outranks the sheet's own section headings.
A line break inside a block becomes `<br>`.
Code text is escaped and nothing else, so a fence shows exactly what was typed.

## `private static void LSheetNoteClose(StringBuilder page, LMarkdownKind kind)`

Closes the list element `kind` opened.

## `private static string LSheetNoteFormat(IReadOnlyList<LMarkdownSpan> spans)`

Writes the spans of one block as inline HTML, nesting link, strong and em around each.
