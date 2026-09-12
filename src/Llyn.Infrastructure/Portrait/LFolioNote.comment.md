# LFolioNote.cs

## `public static class LFolioNote`

Renders the note's Markdown blocks as word-processor paragraphs.

## `public static void LFolioNoteAppend(StringBuilder body, string? markdown)`

Writes one paragraph per block under the note styles `LFolioStyle` declares.
A list item is a hanging-indent paragraph led by a bullet or its number, one indent step per level.
Numbers count up while consecutive items stay on one level and restart otherwise.
A rule is an empty paragraph with a bottom border.
Code keeps its line breaks through `LFolioLine`.

## `private static string LFolioNoteFormat(IReadOnlyList<LMarkdownSpan> spans)`

Writes the spans of one block as runs carrying bold, italic, monospace and underline.
A link whose target differs from its text is followed by the target in parentheses.
