# TMarkupLink.cs

## `public sealed class TMarkupLink`

Covers the links that cross from one card to another entry or sense.

A relation, a card translation and a collocation synonym are the one part of the model
where an entry is not self-contained.
A relation in the first entry of a file may point at the last, and at a sense rather than an entry.
So these tests are about order and identity rather than about fields.

The reader's own refusals are here too, because each one names the element at fault
and the message is what the author reads.

## Inline notes

### `private const string TMarkupPair`

Two entries pointing at each other, in every way the format allows.
The first names the second by entry key and the second names a sense of the first.
A collocation of the first names a sense of the second, which is declared after it.
Every link therefore points forward or backward across an entry boundary.

### `MarkupImport_RelationNamingALaterEntry_StoresTheLink`

The entry a relation names may not exist while the relation is being read.
This is the case the second pass exists for.

### `MarkupImport_RelationNamingASenseOfAnotherEntry_StoresTheLink`

A sense key names a card rather than an entry, and the two go into one namespace.
The stored row is the meaning, which is what `relation_sense` holds.

### `MarkupImport_RelationWithNoTarget_Refuses`

Section 9 of the format spec refuses the file, and the message names the relation and its card.

### `MarkupImport_RelationWithNoType_Refuses`

The same refusal for the other half of the rule.

### `MarkupImport_TargetNamingBothKinds_Refuses`

A target names an entry or a sense and never both.
The document is otherwise sound, so nothing but the target can be what refuses it.

### `MarkupImport_SynonymNamingNeitherKind_Refuses`

The same rule read through the other element, which is how the shared check is proved shared.

### `MarkupImport_TranslationAndCollocationSynonym_ComeBackOnASecondTrip`

Exporting the pair together loses nothing, so the export reports no loss.
Reimporting into a clean workspace rebuilds every link against ids that workspace handed out.

### `MarkupExport_RelationOutsideTheSet_IsReportedAndNotWritten`

Exporting one of the two entries leaves every link pointing outside the file.
None of them is written, because a key names nothing beyond the file that declares it.
All three are reported instead, named by the entry that held them.

### `MarkupExport_TwoEntriesPointingAtEachOther_LeaveNoDanglingRow`

The whole round trip, ending in `PRAGMA foreign_key_check`.
The schema cascades nothing from a link's target side, so a link to a missing row would stand.
An empty check is what says every link found the row it named.

### `private static void TMarkupLinkCheck(string text, string named)`

One refused document, its message, and the proof that the workspace kept nothing.
A failed import leaves the workspace as it was, so the entry count is still zero.

### `private static string TMarkupLinkRead(LEngine engine, string entryId, int place)`

The stored id of one meaning of an entry, which is what a link is asserted against.
