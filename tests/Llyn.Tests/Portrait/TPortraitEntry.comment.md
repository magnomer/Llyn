# TPortraitEntry.cs

## `public sealed class TPortraitEntry`

Covers the entry likeness: one saved entry read into the section tree every export writes.

## `public void PortraitRead_EntryWithCardAndSentence_CarriesTheTreeTheDisplayShows()`

Title, language, reading line, speech chips, the numbered card, its example row and the note, in the panel's order.

## `public void PortraitRead_MissingEntry_Throws()`

An entry that no longer stands is an error, not an empty page.
