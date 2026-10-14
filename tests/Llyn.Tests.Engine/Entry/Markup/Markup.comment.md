# Markup

This folder holds the tests of markup import and export of entries.
They cover cargo, the intake clerk, the etymology map, links, merge, references, replace and targets.

## `TMarkupCargo.cs`

It covers the cargo one read of a `.llx` file hands to the import.
It also covers what the import refuses of the intakes it is given.
A file past the size ceiling is refused before a byte is read.
The oversize file is grown by length alone and holds no content, so the refusal comes from the length check.
A language that could not name a pack folder is blanked at the read and reported at its entry's line.
Both entries carrying the same bad name are reported and blanked.
Two intakes on one target are refused, since the second write would erase the first.
A Replace and a Merge on one entry make the refusal about the target and not the mode.
What the read skipped rides in the cargo and comes out of the import unchanged.
The whole import report stops at the omission ceiling and ends with one `...` row.
Each unknown `origin` is a read row and each inflection with an unknown morphology an import row.
The first case passes the read cap too, so the two caps must not leave two `...` rows.
The second case stays under the read cap, so only the outcome clamp can stop the list.

## `TMarkupClerkImporting.cs`

It covers the markup intake clerk composed over a real rig.
Two linked entries parsed from markup, imported and loaded back format to the same markup.
`TMarkupIntakeCreate` makes one fresh intake per parsed entry.

## `TMarkupAncestor.cs`

It covers the etymology map crossing a markup file.
That is what the writer emits, what the reader resolves, and the round trip.
Each Ancestor box is written with its key, its child's key, its position and its relation by name.
A box linked to an entry carries that entry's headword and language.
A text box carries its text, language and gloss.
An exported entry read back as new stores the same tree shape.
The shape compare ignores the row keys, since the import mints new ones.
The old `<etymon>` and `<etymology>` elements still read.
The import loads the entry, stores no box, and reports one omission for each element.
A link the workspace cannot resolve keeps its headword as the box text and reports one omission.
`TMarkupAncestorSave` stores one map of three boxes, and `TMarkupAncestorExport` hands back the parsed record beside the text.
`TMarkupAncestorRead` reads the map as the store gives it back.

## `TMarkupExporting.cs`

It covers the markup export from stored rows to a `.llx` file.
Every id a row carries must leave the file as a name, a path or nothing at all.
The portrait export in markup format is the same file by another door.
Its label is passed but never read, since markup carries states rather than display words.
A linked entry has two other entries, a two-author reference and a sub-sense of the mentioned entry.
The file is parsed back through the reader, so the assertions are on records and not on text.
The one exception asserts that no `id` appears anywhere in the text.
A title stated as unknown leaves as a `state` attribute and reads back as unknown.
An id naming no stored entry is refused before any file is written.
Every string the exemplar draft carries must reach the file, since a person shares what they typed.
A dropped field names itself by its own distinct string.
One person exports, another imports into an empty workspace and exports again, and the two files must match.
So the loader, writer, reader and importer carry the same fields.
The translation and mention targets travel in the same file, so every name resolves on arrival.
The same trip is judged on the stored drafts, since two files can match while both lack a field.
Ids are zeroed first, and a field the writer never writes or the importer never reads breaks the shape.

## `TMarkupImporting.cs`

It covers the markup import from a `.llx` file to stored rows under the New intake.
The Merge and Replace intakes are covered by `TMarkupReplace` and `TMarkupMerge`.
Every link the file names by headword must land on an id or be reported as an omission.
A refusal anywhere in the file must leave the workspace as it was.
Entries that translate to each other resolve both links to rows the same import creates.
A translation target neither stored nor in the file drops only the link.
Two files with an unknown register, imported in turn, share the one unknown register the first created.
Twin targets differing only in case are dropped with an omission, since normalization makes them twins.
A file that is not well-formed is refused whole, and no entry is stored.
A blank second headword stores neither entry, and only the rollback removes the sound first one.
An inflection's speech and morphology sit on their own lines, so the reader keeps the newlines around them.
The workspace trim strips only spaces, so the names must be trimmed before the lookup.
An exported entry deleted before its file comes back is created rather than found.
The mentioned entry and the reference stay, so the file's names resolve to the rows that were there.
The example keeps the original reference id, which shows the file's reference was matched.

## `TMarkupLink.cs`

It covers the links a markup import makes by name, the sense pass above all.
A sense cited by position path must land on the sense that exists once the file is stored.
A span the store cannot take must be dropped with an omission, never fail the file.
A file citing a sense of the entry it replaces sees the old sense id gone.
This happens before the mention is stored.
Before the sense pass this failed the whole import on a foreign key.
Two new entries citing each other's sense both link, though neither sense exists when the citing entry is written.
A path naming a fourth meaning of an entry with one keeps only the entry.
The omission carries the entry's line, since the record has none of its own.
A span running past the example text is dropped, which the archive would refuse with an exception.
A span standing for no entry is stored, and the export writes it back with no headword.
A Replace file with no language element must not blank the stored language.
A blank headword under Replace takes the target's stored one.
The save refuses a blank headword, so without the fallback the whole import would fail.
An offset of `int.MaxValue` plus its length wraps negative in `int` and would pass the end check.
A UNC path and a `file` address with a host both name a machine Windows would send credentials to.
The web image between them passes, which shows only the refused rows are dropped.
`TMarkupOwn` is an entry whose example cites its own first sense, and `TMarkupReference` builds its files on it too.
`TExampleRead` is the one example a card of these files carries, and `TMarkupReference` reads through it too.
`TMarkupSave` lands the file inside the test workspace, so it is cleaned up with it.

## `TMarkupMerge.cs`

It covers the markup import under the Merge intake, which lands on a stored entry.
Merge joins what the file adds onto the stored cards and keeps every id it can.
A file row whose key matches a stored row is skipped, so a repeated merge adds no keyed row twice.
A file meaning naming a stored head joins that card under its old id.
A new meaning lands after the stored ones.
A file pronunciation equal to the stored ipa is skipped, and a new one is kept.
A transcription naming a scheme the entry holds keeps the stored row, since the save allows one row per scheme.
The import still succeeds then.
An unknown register merged into a meaning that has one already leaves a single register.
The same file merged twice onto an entry with another note paragraph adds its note once.
The second merge finds the file note as a whole block of the stored note.
A note differing only by trailing spaces and a carriage return normalizes to the same text, so nothing is appended.
A pronunciation with audio and no ipa is matched on its audio location.
The audio sits inside the workspace, so the stored path loads back as the file wrote it.
An image whose location is unknown is matched on that state, so a second merge adds no image.

## `TMarkupReference.cs`

It covers the references a markup import creates or reuses for its examples.
A reference is matched against the stored ones before a new row is created.
A reference whose example is never stored must create nothing.
A stored reference with no title and no year must not catch every file reference that has none either.
A textless sentence keeps its particle and dependence, but its example is never stored.
Its reference carries a new title, so creating it early would leave an orphan behind.
A reference with no title and no address never matches, so each creation adds a row.
The Merge after the first import drops the sentence as already present, so it must create nothing.

## `TMarkupReplace.cs`

It covers the markup import under the Replace intake, which lands on a stored entry.
Replace keeps the entry id and what hangs off it, and rebuilds the rest from the file.
A refusal on one target rolls back every entry in the file.
Grasp and favorite hang off the entry id, so a Replace that keeps the id keeps them.
Another entry's example citing a sense the Replace deletes keeps only the entry in the mention.
The detach runs before any other row of the entry, and its row must still take the entry's line.
The exact text proves the example is named by its sentence and not by its id.
The situation and the example lose their card but stay stored, since detaching never deletes.
The New intake comes first in the file, yet a stale target refuses before any entry is created.

## `TMarkupTarget.cs`

It covers the targets the engine finds for parsed entries before the customs question is asked.
Each target is a stored entry sharing the headword and language, with its cards counted.
The counts are what a replacement would drop, nested cards included.

## Stamp

Commit: `c6e60141a172`
- `TMarkupAncestor.cs` `64b1d4fafbcd9164`
- `TMarkupCargo.cs` `4d32e994f2b00697`
- `TMarkupClerkImporting.cs` `9bb3ac18987a3b9b`
- `TMarkupExporting.cs` `21b4ed675ba53237`
- `TMarkupImporting.cs` `8d86de7909f3b53d`
- `TMarkupLink.cs` `3c865b1d6a8a9897`
- `TMarkupMerge.cs` `1334450da1c9ffff`
- `TMarkupReference.cs` `858c8526b703ab1f`
- `TMarkupReplace.cs` `92ea41fb2567f516`
- `TMarkupTarget.cs` `3f3db3b29f70d2ad`
