# TInterfaceEtymology.cs

## `internal static partial class TInterface`

The etymology half of the test interface: the record and the requests a test needs built.
Tests never construct a production type, so every shape crosses here.

## `internal static LEtymology TEtymologyCreate(long entryId, string text, IReadOnlyList<LMention> mentions)`

One narrative with its spans and no id, as a store is handed it.

## `internal static LRequest TEtymologyTextCreate(long draftId, string text)`

Writes the narrative of the open draft.

## `internal static LRequest TEtymologyMentionCreate(long draftId, int offset, int length, long entryId)`

Names an entry over a stretch of the narrative, or clears the span when it names none.

## `internal static LRequest TEtymonAdditionCreate(long draftId, long entryId, int position)`

Adds a source link at one place among the links.

## `internal static LRequest TEtymonRemovalCreate(long draftId, long entryId)`

Drops the source link naming that entry.

## `internal static LRequest TEtymonShiftCreate(long draftId, long entryId, int position)`

Moves a source link to another place among the links.
