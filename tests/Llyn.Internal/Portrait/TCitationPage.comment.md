# TCitationPage.cs

## `public sealed class TCitationPage`

The pages the citation clerks compose for an Example, a Source and a Situation, read without a workspace.
Each carries the rows the read areas show, so the engine's page facade has nothing left to decide.

## `public void ExamplePageRead_GlossAndSource_CarriesWhatTheExcerptShows()`

The sentence heads the page, the language and tally are its chips, and the gloss and source are its sections.

## `public void ExamplePageRead_UnwrittenAndUncited_FallsBackToTheUnwrittenWord()`

An unwritten sentence heads its page with the legend's word and carries no section.

## `public void ReferencePageRead_TwoCreditsAndAYear_CarriesWhatTheColophonShows()`

The credits are joined on one line, and the year and address each take a section.

## `public void ReferencePageRead_AuthorsUnknown_ShowsTheMarkAlone()`

A Source whose authors were ruled unknown shows the mark under the authors heading and nothing else.

## `public void SituationPageRead_KindAndDescription_CarriesTheDescriptionAsANote()`

The kind is a chip beside the tally, and the description rides as a note so it draws as blocks.
