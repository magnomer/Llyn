# TPortraitEntry.cs
Hash: `37c8bcf75bb8e324`

## `public sealed class TPortraitEntry`

Covers the entry likeness.
One saved entry is read into the section tree every export writes.

## `public void PortraitRead_EntryWithCardAndSentence_CarriesTheTreeTheDisplayShows()`

Title, language, reading line, speech chips, the numbered card, its example row and the note, in the panel's order.

## `public void PortraitRead_RespelledLanguage_PrintsOneReadingPerRow()`

With respelling on, the reading line prints each respelling in place of its IPA.
The page follows the setting at read time, so one entry shows either form without a new save.

## `public void PortraitRead_LinkedEtymology_CarriesOneBandOfSourceLinks()`

The linked shape prints as links under one heading and no line of its own.

## `public void PortraitRead_NarratedEtymology_CarriesTheProseAndItsSpans()`

The narrative prints as a line, with the entries its spans name beside it.

## `public void PortraitRead_NoEtymology_AddsNoBand()`

An entry that says nothing about its origin gets no heading.

## `public void PortraitRead_MissingEntry_Throws()`

An entry that no longer stands is an error, not an empty page.
