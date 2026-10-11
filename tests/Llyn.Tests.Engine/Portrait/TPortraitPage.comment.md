# TPortraitPage.cs
Hash: `548fe500a7c8f50f`

## `public sealed class TPortraitPage`

Covers the page likeness.
An example, a source or a situation is read into the shape the sheet prints.
It covers the engine read for each realm, the HTML rendering, and the press opened for print.

## `public void PortraitRead_ExampleWithGlossAndSource_CarriesWhatTheExcerptShows()`

Sentence, language, tally, glosses and the cited source, in the excerpt's own order.

## `public void PortraitRead_UnwrittenExample_FallsBackToTheUnwrittenWord()`

A sentence never written heads the page with the legend's word, and empty sections are left out.

## `public void PortraitRead_SourceWithCredits_CarriesWhatTheColophonShows()`

Title, kind chip, tally, credited authors and the written fields, with the unwritten note left out.

## `public void PortraitRead_SituationWithDescription_CarriesItAsANote()`

The description arrives as Markdown, so the sheet draws it as blocks rather than one line.

## `public void PortraitRead_MissingRow_Throws()`

A page for a row that does not exist throws, rather than reading as an empty page.

## `public void SheetPageFormat_FullPage_CarriesTitleChipsAndSections()`

Title, chip row, labelled lines and the note, with reader text escaped.

## `public async Task PortraitPrint_ExampleOnFakePress_HandsThePageToThePressToPrint()`

The rendered page opens a board on the press, and nothing prints yet.
The print window then decides every setting, so the engine hands the press none.
The fact disposes the relay, so the board never outlives it.

## `private static LReference TPortraitSourceCreate(LEngine engine, string title)`

Saves a source with only a title, so an example can cite it.

