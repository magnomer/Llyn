# LOutlineSection.cs

## `public static class LOutlineSection`

Renders one section node of a page likeness as Markdown, and recurses into its children.
The role of the node decides its form, so a situation, a register and a tag keep their own marks.

## `public static void LOutlineSectionAppend(StringBuilder outline, LPortraitSection section, int depth)`

Dispatches on the role: card, phrase, scene, tone, label, bridge, quote, else band.
A scene is an italic run, and a tone and a label are code runs.
A bridge is the arrow run of the shared body.

## `private static void LOutlineBandAppend(StringBuilder outline, LPortraitSection section, int depth)`

A second-level heading, the chips as code runs, one bullet per line, then the shared body.

## `private static void LOutlineCardAppend(StringBuilder outline, LPortraitSection section, int depth)`

The position and heading form the card heading, so a reader can cite a card by number.
The heading level deepens with nesting and stops at six, which is all Markdown has.
A phrase standing first among the children is written before the lines, as the panel shows the expression.
Each line is a paragraph, and the rest of the shared body follows.

## `private static void LOutlineUsageAppend(StringBuilder outline, LPortraitSection section)`

One list item: the headword in bold, the card title after a dash, then the kind and language in brackets.

## `private static void LOutlineQuoteAppend(StringBuilder outline, LPortraitSection section, int depth)`

One list item whose text is the first line.
The further lines, the chips and every link nest one level deeper as sub-items.
A child of another role, the note and the plates follow as any section's would, so nothing is dropped.
A quote with no line writes nothing, since there is nothing to quote.

## `private static void LOutlineMediaAppend(StringBuilder outline, LPortraitSection section)`

Each image as a picture link and each video as a play link with its span.

## `private static void LOutlineBodyAppend(StringBuilder outline, LPortraitSection section, int depth, int start = 0)`

The links as an arrow run, the note carried over unescaped, the children from `start`, then the plates.
A blank line closes a run of quote or usage items, so the next paragraph does not join the list.

## `private static void LOutlineLineAppend(StringBuilder outline, IReadOnlyList<LPortraitLine> lines, string open, string close)`

Each line as its own paragraph, wrapped in the given marks.

## `private static void LOutlineChipAppend(StringBuilder outline, IReadOnlyList<string> chips, string open, string close, string join)`

The chips as one paragraph, skipped when there are none.

## `private static string LOutlineLineFormat(LPortraitLine line)`

The label in bold before the text, or the text alone when the line has no label.

## `private static string LOutlineLinkFormat(IReadOnlyList<LPortraitLink> links)`

An arrow, then each headword with its language in brackets, separated by dots.

## `private static string LOutlineChipFormat(IReadOnlyList<string> chips, string open, string close, string join)`

Each chip wrapped in the given marks and joined on one line.
A code run needs no escaping, so a chip in backticks is written as the reader wrote it.
Any other run is escaped like every field.
