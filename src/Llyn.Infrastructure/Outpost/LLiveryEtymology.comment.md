# LLiveryEtymology.cs
Hash: `a176dc108be2be28`

## `internal static class LLiveryEtymology`

Writes the etymology, the note and the stamps of a Joplin entry body for `LLiverySheet.LLiveryFormat`.
It reads only `LLiveryPage`, the `note` map and the `lookup` it is handed.

## `public static void LLiveryEtymologyAppend(StringBuilder sheet, LLiveryPage page, Func<long, string> note, Func<string, string> lookup)`

`LLiverySheet.LLiveryFormat` calls it last, right before the closing div.
An etymology with text or etymons writes a `Display.Etymology` heading first.
The text and its mentions follow through `LLiveryMentionFormat`, then one list line per `LLiveryPageEtymon`.
Each etymon line links its headword through `LLiveryLinkFormat` and adds its language as a `llyn-language` chip.
A stored note then writes a `Display.Note` heading and the note as its stored Markdown.
`LLiveryStampAppend` writes the stamps last.
An empty etymology or note writes nothing, not even its heading.

## `internal static string LLiveryLinkFormat(string text, long id, Func<long, string> note)`

The escaped `text` as a `:/` link to the note id `note` gives for `id`.
An id of zero or less, or an empty note id, keeps the text plain.
So a target outside the workspace stays plain text.

## `internal static string LLiveryMentionFormat(string text, IReadOnlyList<LMentionDraft> mentions, Func<long, string> note)`

Writes `text` with each linked mention swapped for a `LLiveryLinkFormat` link.
Leading and trailing white space of `text` is left out, so no break opens or closes the result.
A mention that is unlinked, empty, overlapping or outside the trimmed text stays plain.
The plain parts go through `LLiveryBreakFormat`.
`LLiveryEtymologyAppend` hands it the etymology text and its mentions.
`LLiveryCard` hands it each card meaning with no mentions, and each example text with its mentions.

## `private static string LLiveryBreakFormat(string text)`

Escapes each line through `LLiveryHeader.LLiveryTextFormat` and joins the lines with Markdown backslash breaks.
So a stored line break survives inside one Markdown paragraph without inline HTML.

## `private static void LLiveryStampAppend(StringBuilder sheet, LLiveryPage page, Func<string, string> lookup)`

Writes a rule, then `LLiveryPageCreated` and `LLiveryPageUpdated` with their `llyn-stamp` labels.
The stamps share one paragraph, split by a Markdown backslash break.
The labels come through `Display.Created` and `Display.Updated`.
An empty stamp is left out, and two empty stamps write no rule.
