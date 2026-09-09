# TPortraitSheet.cs

## `public sealed class TPortraitSheet`

Covers the HTML rendering, which the PDF is printed from.
What is proved here holds for both.

## `public void SheetFormat_FullPortrait_CarriesEveryFieldTheDisplayShows()`

A field silently dropped is the failure this export exists to avoid.

## `public void SheetFormat_ThemedPortrait_CarriesTheDisplayPalette()`

The palette is asserted against the theme rather than against literals.
A changed theme must move the page with it.

## `public void SheetFormat_MarkupInFieldText_LeavesNoUnescapedTag()`

Entry text is written by the reader and may hold anything.

## `public void SheetFormat_YouTubeVideo_ShowsThePosterFrameAndTheLink()`

A document plays nothing, so the plate must still say what the video was.
