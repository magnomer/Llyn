# LLiveryInflection.cs
Hash: `41638ef2745a54c8`

## `internal static class LLiveryInflection`

Writes the inflection sheet of a Joplin entry body for `LLiveryRime.LLiveryRimeAppend`.
Its rows come from `LSheetInflection`, which the HTML export's paradigm box shares.
It reads only the `LParadigmView` and the `lookup` it is handed.

## `public static void LLiveryInflectionAppend(StringBuilder sheet, LParadigmView? view, Func<string, string> lookup)`

The note mirrors the app's Short/Full switch, without script, since Joplin runs none.
The switch is a `details` element, which Joplin renders and folds on a click.
A `llyn-inflection-box` div wraps it, the box the app draws around its sheet too.
It has its own class, since `llyn-paradigm` is the rime box and its fill would leak in.
Each sheet's rows are `LSheetInflection.LSheetInflectionFormat`, compared as text.
The `llyn-inflection-full` details holds the expanded table after its summary.
The summary holds the `Paradigm.Short` and `Paradigm.Full` captions, the labels of the app's radio pair.
They sit in `llyn-inflection-switch-short` and `llyn-inflection-switch-long` spans.
No class shares the rime prefix, so no rime rule reaches the sheet.
The collapsed table follows the details, with the extra class `llyn-inflection-short`.
The style hides it while the details is open, so one sheet shows at a time.
A closed details shows Short, the state the app's switch starts in.
`LSheetInflection.LSheetInflectionResolve` decides between one table and the switch.
The paradigm box of the HTML export asks it too.
That single table sits in the same `llyn-inflection-box` div, so the box stays.
A null view, or two sheets without lines, writes nothing.
The block is raw HTML, so Markdown never parses inside it.
Text is therefore HTML encoded only, as the `llyn-more` summary is.
No blank line may sit inside the block, since a blank line ends the HTML block.
A blank line follows it, so the next writer starts a fresh Markdown block.
