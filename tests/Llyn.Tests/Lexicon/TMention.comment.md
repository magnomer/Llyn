# TMention.cs

## `public sealed class TMention`

Mentions persist ordered text spans linked to Entries and optional meanings.
Overlaps, out-of-range spans, and meanings without Entries are rejected.
Deleting linked records or shortening text preserves only valid data.
Copies receive new IDs.
Sentence reads restore mention data for each embedded Example.

## `ExampleCreate_TwoMentions_ReadsBackSortedByStart()`

Creating an Example sorts mentions by text offset and assigns IDs.
Archive reads preserve spans and links, including optional sense identity.

## `MentionSave_OverlappingPair_Refuses()`

Overlapping spans are rejected without leaving partial mention rows behind.

## `MentionSave_SpanPastText_Refuses()`

A span extending beyond Example text is refused, while the same start with an in-bounds length can be saved.

## `MentionSave_SenseWithoutEntry_RefusesAtCheck()`

A sense reference without its required Entry violates the database check.
An Entry-less mention without a sense remains valid.

## `EntryDelete_MentionedEntry_DropsTheMention()`

Deleting an Entry removes mentions that point to it but preserves the containing Example.

## `MeaningDelete_MentionedSense_KeepsTheEntry()`

Deleting a referenced meaning clears only the sense link, retaining the mention ID and its Entry link.

## `ExampleTextUpdate_ShorterText_DropsOnlyTheSpanPastTheEnd()`

When Example text shrinks, mentions extending beyond its end are removed.
Earlier spans remain.

## `MentionCopy_ToAnotherExample_GivesTheSameSpansUnderNewIds()`

Copying preserves span and link data while assigning fresh IDs.
The copied IDs do not overlap the originals.

## `SentenceMeaningRead_MentionedExamples_FillsEveryRow()`

Meaning-based sentence reads include mention data in each Example row, preserve non-Example rows, and retain sense links.

## `private static (long TMentionEntry, long TMentionMeaning) TMentionEntryCreate(TWorkspace workspace)`

Creates a minimal Entry and Meaning pair for persistence and referential-integrity cases.

## `MentionDraftResolve_NegativeId_KeepsTheSpan()`

Resolving a draft preserves its temporary negative ID, offsets, and Entry/meaning links for unsaved mentions.
