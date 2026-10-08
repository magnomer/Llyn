# LMarkupClerkExample.cs
Hash: `2ee2c4e6735563fd`

## `public sealed class LMarkupClerkExample`

The example side of an export: example texts, mentions, sense paths and cited references.
Cards and etymologies both name entries through it, so one instance serves both.

## `public LMarkupClerkExample(LRig rig)`

Reads the entry, meaning, reference and author ports out of `rig`.
It opens no database of its own, so a test can hand it in-memory ports.

## `public LMarkupExample LMarkupExampleCreate(LExampleDraft example)`

Copies the example with its glosses, translated mentions and resolved reference.

## `public LMarkupMention LMarkupMentionCreate(LMentionDraft mention)`

Names the mentioned entry by headword and language and its sense by position path.
A mention with no entry, or whose entry is gone, keeps its span and names nothing.

## `private string LMarkupMeaningResolve(long entryId, long meaningId)`

Walks from `meaningId` up to the root of the entry's meaning tree.
The path is one-based positions joined by dots, so the first sub-sense of the first meaning is `1.1`.
No sense yields the empty string.

## `private LMarkupReference? LMarkupReferenceCreate(LStateAnchor anchor)`

Reads the cited reference and its credited authors in their stored order.
An unspecified or unknown anchor, or a reference that is gone, yields null.
