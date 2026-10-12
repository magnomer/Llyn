# TMarkupMerge.cs
Hash: `1334450da1c9ffff`

## `public sealed class TMarkupMerge`

Covers the markup import under the Merge intake, which lands on a stored entry.
Merge joins what the file adds onto the stored cards and keeps every id it can.
A file row whose key matches a stored row is skipped, so a repeated merge adds no keyed row twice.

## Inline notes

### `public void MarkupImport_AppendKnownMeaning_JoinsCardAndKeepsId()`

The file's first meaning names a stored head, so its tag joins that card under its old id.
The file's second meaning is new and lands third, after the two stored ones.

### `public void MarkupImport_AppendKnownIpa_SkipsRowAndJoinsNote()`

The first file pronunciation equals the stored ipa and is skipped, and the second is new and kept.

### `public void MarkupImport_AppendKnownScheme_KeepsStoredRow()`

The file's transcription names a scheme the entry holds, with other text.
The import succeeds and the stored row stands, since the save allows one row per scheme.

### `public void MarkupImport_AppendUnknownRegister_KeepsOneRegister()`

A meaning with an unknown register is merged into the same stored meaning, which has one already.
The two unknown registers are one, so the card still holds a single register.

### `public void MarkupImport_AppendKnownNote_KeepsNoteOnce()`

The same file is merged twice onto an entry whose note has another paragraph.
The second merge finds the file note as a whole block of the stored note and skips it.

### `public void MarkupImport_AppendSpacedNote_KeepsStoredNote()`

The file note differs from the stored one only by trailing spaces and a carriage return.
Both normalize to the same text, so nothing is appended.

### `public void MarkupImport_AppendAudioOnly_KeepsOnePronunciation()`

A pronunciation with audio and no ipa is matched on its audio location.
The audio sits inside the workspace, so the stored path loads back as the file wrote it.

### `public void MarkupImport_AppendUnknownImage_KeepsOneImage()`

An image whose location is unknown is matched on that state, so a second merge adds no image.
