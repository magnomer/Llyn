# LFolioSection.cs

## `public static class LFolioSection`

Renders one section node of a page likeness as Word paragraphs, and recurses into its children.
The role of the node decides its style, so a situation, a register and a tag each keep their own.

## `public static void LFolioSectionAppend(StringBuilder body, LPortraitSection section, List<LPortraitAsset> plates, LTheme theme)`

Dispatches on the role: card, phrase, scene, tone, label, bridge, quote, else band.
A bridge draws nothing of its own, since the shared body already draws links.

## `private static void LFolioBandAppend(StringBuilder body, LPortraitSection section, List<LPortraitAsset> plates, LTheme theme)`

A band heading, the chips as one sound line, one row per line, then the shared body.
The chips take the sound style because the only band chips are parts of speech and characters.

## `private static void LFolioCardAppend(StringBuilder body, LPortraitSection section, List<LPortraitAsset> plates, LTheme theme)`

A one-column table: a shaded header row with the number and heading, then one cell for everything else.
The header is muted when the heading is only the card's kind.
A phrase standing first among the children is drawn before the sense lines, as the panel shows the expression.
Each line is a sense paragraph led by its bold label when it has one.
The rest of the shared body follows.
A cell must end in a paragraph, so an empty one closes it before the table closes.

## `private static void LFolioQuoteAppend(StringBuilder body, LPortraitSection section, List<LPortraitAsset> plates, LTheme theme)`

A bulleted quote paragraph whose bold lead is the first line's label and whose text is its text.
The further lines follow as indented detail rows, the chips as one label line, then the shared body.

## `private static void LFolioUsageAppend(StringBuilder body, LPortraitSection section)`

One entry that links here as two rows: the arrow and headword, then the card title, kind and language.

## `private static void LFolioBodyAppend(StringBuilder body, LPortraitSection section, List<LPortraitAsset> plates, LTheme theme, int start = 0)`

The bridge line, the note as paragraphs, the children from `start`, then the plates.
The children come before the plates because the panel draws its pictures under the tags.
A readable picture is embedded and scaled to the text width, and its caption follows as a label.
An unreadable one falls back to its address, and a video is a play mark before its address.

## `private static void LFolioLineAppend(StringBuilder body, string style, IReadOnlyList<LPortraitLine> lines)`

One paragraph per line in the given style, its label bold before the text when it has one.

## `private static void LFolioChipAppend(StringBuilder body, IReadOnlyList<string> chips, string style)`

The chips joined on one paragraph in the given style, skipped when there are none.

## `private static (long LFolioAcross, long LFolioDown) LFolioPlateClamp(int width, int height)`

Word measures in EMUs, so pixel sizes are scaled and clamped to the column width.
An unreadable size takes a widescreen box, so the picture still has a place.
