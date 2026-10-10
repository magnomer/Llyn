# LLiveryInflection.cs
Hash: `5acf50c51a74425d`

## `internal static class LLiveryInflection`

Writes the inflection sheet of a Joplin entry body for `LLiveryRime.LLiveryRimeAppend`.
It reads only the `LParadigmView` and the `lookup` it is handed.

## `public static void LLiveryInflectionAppend(StringBuilder sheet, LParadigmView? view, Func<string, string> lookup)`

The note mirrors the app's Short/Full switch, without script, since Joplin runs none.
The switch is a `details` element, which Joplin renders and folds on a click.
A `llyn-inflection-box` div wraps it, the box the app draws around its sheet too.
It has its own class, since `llyn-paradigm` is the rime box and its fill would leak in.
The `llyn-inflection-full` details holds the expanded table after its summary.
The summary holds the `Paradigm.Short` and `Paradigm.Full` captions, the labels of the app's radio pair.
They sit in `llyn-inflection-switch-short` and `llyn-inflection-switch-long` spans.
No class shares the rime prefix, so no rime rule reaches the sheet.
The collapsed table follows the details, with the extra class `llyn-inflection-short`.
The style hides it while the details is open, so one sheet shows at a time.
A closed details shows Short, the state the app's switch starts in.
A sheet without lines writes only the other sheet, as one table with no switch.
Two sheets that render the same write one table, since a switch would change nothing.
That single table sits in the same `llyn-inflection-box` div, so the box stays.
A null view, or two sheets without lines, writes nothing.
The block is raw HTML, so Markdown never parses inside it.
Text is therefore HTML encoded only, as the `llyn-more` summary is.
No blank line may sit inside the block, since a blank line ends the HTML block.
A blank line follows it, so the next writer starts a fresh Markdown block.

## `private static string LLiveryTableFormat(LParadigmTable? table, Func<string, string> lookup)`

Writes the rows of one sheet, without the table tags, for both sheets.
The caller wraps them, so the two sheets share one loop and can be compared as text.
A null table or a table without lines yields an empty string.
Headers, groups and labels are localization keys, so each passes through `lookup`.
An empty key writes an empty cell, as the app leaves its grid cell empty.
The header row exists only when the sheet has headers, after two empty cells for group and label.
The first line has no rule above it.
A later line with a group gets `llyn-close`, the full rule the app draws before a group.
Any other later line gets `llyn-rule`, the dotted rule that skips the group column.

## `private static string LLiveryInflectionFormat(LParadigmForm form, Func<string, string> lookup)`

Reads the cell's ready text and tip, which the Core view build resolved for the app box too.
The facade asked with `held` off, since a note never holds a fill.
A cell with a tip is a `llyn-muted` span of its stand-in mark, titled with the tip key's text.
That is how the app shows a muted cell with a tooltip.
A text form writes its runs as `QParadigm` does.
A marked run becomes a `llyn-marked` span.
A split inside the text adds a `llyn-cut` hyphen there.
A text form with empty text writes an empty cell, the gap where the layout has no slot.
