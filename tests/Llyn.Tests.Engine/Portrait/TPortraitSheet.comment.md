# TPortraitSheet.cs
Hash: `6f7592d5e2cb9603`

## `public sealed class TPortraitSheet`

Covers the HTML rendering, which the PDF is printed from.
What is proved here holds for both.
Only the collocation rows differ, since the PDF prints a page opened by `LPortraitPage.LPortraitPageOpen`.

## `public void SheetFormat_OpenedPage_PrintsTheFoldedRowOpen()`

A folded row on a page opened for print shows open under its list heading.
Its summary holds the phrase and the meaning, and its fields follow.

## `public void SheetFormat_FoldedRow_WritesClosedDetails()`

A folded row on an unopened page keeps its fold, as an HTML export shows it.
Its fields stay inside the closed disclosure.

## `public void SheetFormat_FullPortrait_CarriesEveryFieldTheDisplayShows()`

A field silently dropped is the failure this export exists to avoid.

## `public void SheetFormat_ThemedPortrait_CarriesTheDisplayPalette()`

The palette is asserted against the theme rather than against literals.
A changed theme must move the page with it.

## `public void SheetFormat_MarkupInFieldText_LeavesNoUnescapedTag()`

Entry text is written by the reader and may hold anything.

## `public void SheetFormat_YouTubeVideo_ShowsThePosterFrameAndTheLink()`

A document plays nothing, so the plate must still say what the video was.
