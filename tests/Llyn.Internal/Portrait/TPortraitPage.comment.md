# TPortraitPage.cs

## `public sealed class TPortraitPage`

Covers the page likeness: an example, a source or a situation read into the shape the sheet prints.
It covers the engine read for each realm, the HTML rendering, and the print hand-off to the press.

## `public void PortraitRead_ExampleWithGlossAndSource_CarriesWhatTheExcerptShows()`

Sentence, language, tally, glosses and the cited source, in the excerpt's own order.

## `public void PortraitRead_UnwrittenExample_FallsBackToTheUnwrittenWord()`

A sentence never written heads the page with the legend's word, and empty sections are left out.

## `public void PortraitRead_SourceWithCredits_CarriesWhatTheColophonShows()`

Title, kind chip, tally, credited authors and the written fields, with the unwritten note left out.

## `public void PortraitRead_SituationWithDescription_CarriesItAsANote()`

The description arrives as Markdown, so the sheet draws it as blocks rather than one line.

## `public void PortraitRead_MissingRow_Throws()`

A page for a row that no longer stands is an error, not an empty page.

## `public void SheetPageFormat_FullPage_CarriesTitleChipsAndSections()`

Title, chip row, labelled lines and the note, with reader text escaped.

## `public async Task PortraitPrint_ExampleOnFakePress_HandsThePageAndTheTicketToThePress()`

The press receives the rendered page and the reader's printer choice, and nothing else decides them.

