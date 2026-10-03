# TSchemaMigrationVersion.cs
Hash: `55fbd9393e4297e0`

## `public sealed class TSchemaMigrationVersion`

Covers the rebuild steps that belong to one older version of the schema.
Each fact writes a workspace as that version left it, then rebuilds it.
A build without a table gets the table added empty, and every other row carries across.
A Collocation numbered apart from the Meanings is renumbered with its links.
A register named twice per language keeps one row and every mark.
An entry frequency moves into the frequency table.
It builds each workspace through `TWorkspace.TWorkspacePrepare`.

## `public void DatabaseCreate_BuildWithoutEtymology_RaisesTheTablesEmpty()`

A version-73 workspace has no etymology tables.
The rebuild adds all three empty and carries every entry across.

## `public void DatabaseCreate_CollocationSharingMeaningId_RenumbersItAndItsLinks()`

An older workspace numbered Collocations apart from Meanings, so the first of each was number 1.
The migration moves the Collocation above both sequences and its links and history follow it.
The Meaning and a Collocation with a number of its own keep theirs.

## `public void DatabaseCreate_OlderBuildHoldingOneNamePerLanguage_KeepsOneRowAndEveryMark()`

A version-32 workspace may hold the same register name twice.
The rebuild keeps one row for the name and every sense still carries its mark, in order, without a repeat.

## `public void DatabaseCreate_OlderBuildWithEntryFrequency_MovesItIntoTheFrequencyTable()`

A version-66 workspace kept frequency as text on the entry.
The rebuild drops that column.
A value naming a source moves into the frequency table, split into source and raw reading.
A bare value with no source gains no row, and neither does an empty one.

## `public void DatabaseCreate_SituationMediaAbsent_RebuildsWithSituationsIntact()`

A version-47 workspace has no Situation media tables.
The rebuild adds them empty and carries every Situation and Image across untouched.
