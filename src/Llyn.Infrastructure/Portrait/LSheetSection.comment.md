# LSheetSection.cs

## `public static class LSheetSection`

Renders one section node of a page likeness as HTML, and recurses into its children.
The role of the node decides its dress, so a situation, a register and a tag keep their own styles.

## `public static void LSheetSectionAppend(StringBuilder sheet, LPortraitSection section)`

Dispatches on the role: card, phrase, scene, tone, label, bridge, quote, else band.
A bridge draws nothing of its own, since the shared body already draws links.

## `private static void LSheetBandAppend(StringBuilder sheet, LPortraitSection section)`

One headed band: its chips as a speech row, then its lines, then the shared body.
A band of usage rows wraps them in one rows block, so the row style stacks them.

## `private static void LSheetUsageAppend(StringBuilder sheet, LPortraitSection section)`

One entry that links here, drawn as the display does.
An arrow, the headword over the card title, the kind as a pill, the language muted.

## `private static void LSheetCardAppend(StringBuilder sheet, LPortraitSection section)`

The badge number and heading form the card header, the heading muted when it is only the card's kind.
A phrase standing first among the children is drawn before the sense lines, as the panel shows the expression.
Each line is a sense paragraph, with its label as a small tag when it has one.
The rest of the shared body follows.

## `private static void LSheetPhraseAppend(StringBuilder sheet, LPortraitSection section)`

Each line as a phrase paragraph, then the shared body.

## `private static void LSheetQuoteAppend(StringBuilder sheet, LPortraitSection section)`

A bulleted row whose first line is the quote: its label as the frame, its text as what was said.
The further lines follow as tagged lines, the chips as labels, then the shared body.
The enclosing quotes block is opened by the parent, so neighbouring quotes share one.

## `private static void LSheetBodyAppend(StringBuilder sheet, LPortraitSection section, int start = 0)`

The links as a bridge row, the note as Markdown blocks, the children from `start`, then the plates.
A run of quote children is wrapped in one quotes block, as the panel groups a card's examples.
The children come before the plates because the panel draws its pictures under the tags.

## `private static void LSheetLineAppend(StringBuilder sheet, IReadOnlyList<LPortraitLine> lines, int start)`

The lines from the given index as tagged lines, which is how a gloss shows its language.

## `private static void LSheetTagAppend(StringBuilder sheet, string label)`

A small tag before a text, written only when the label is not empty.

## `private static void LSheetChipAppend(StringBuilder sheet, IReadOnlyList<string> chips, string style)`

One chip row under the given class, skipped when there are no chips.
