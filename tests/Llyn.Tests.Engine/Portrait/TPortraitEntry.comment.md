# TPortraitEntry.cs
Hash: `6dcee7664ac19656`

## `public sealed class TPortraitEntry`

Covers the entry likeness.
One saved entry is read into the section tree every export writes.

## `public void PortraitRead_EntryWithCardAndSentence_CarriesTheTreeTheDisplayShows()`

Title, language, reading line, speech chips, the numbered card, its example row and the note, in the panel's order.

## `public void PortraitRead_ChosenUnitAndFreedom_LeadTheSpeechChips()`

A chosen unit's word opens the speech chips, and the chosen freedom's word follows it.
Both stand ahead of the parts of speech.

## `public void PortraitRead_LinkedCollocation_ShowsUnderTheLinkedMeaning()`

A collocation's first link decides the meaning it prints under, and no other-collocations band appears.
It prints as an open row, headed by its phrase, in a list headed by the row count.
Each further link adds a see-line under its meaning.

## `public void PortraitRead_UnlinkedCollocation_ShowsAsOtherRow()`

A collocation linked to no meaning shows under the Other heading as a row, not a numbered card.
The row carries the phrase as heading, the meaning as its line, and its stored fold.

## `public void PortraitRead_FoldedCollocation_CarriesTheFoldOnItsRow()`

A stored fold reaches the collocation's row, which still carries its meaning.

## `public void PortraitRead_RespelledLanguage_PrintsOneReadingPerRow()`

With respelling on, the reading line prints each respelling in place of its IPA.
The page follows the setting at read time, so one entry shows either form without a new save.

## `public void PortraitRead_LinkedEtymology_CarriesOneBandOfSourceLinks()`

The linked shape prints as links under one heading and no line of its own.

## `public void PortraitRead_NarratedEtymology_CarriesTheProseAndItsSpans()`

The narrative prints as a line, with the entries its spans name beside it.

## `public void PortraitRead_NoEtymology_AddsNoBand()`

An entry that says nothing about its origin gets no heading.

## `private static long TPortraitEntryCreate(LEngine engine, string headword)`

Saves a bare English entry under `headword` and answers its id.

## `public void PortraitRead_MissingEntry_Throws()`

An entry that no longer stands is an error, not an empty page.
