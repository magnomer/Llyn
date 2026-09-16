# TReflexFixture.cs

## `internal static class TReflexFixture`

The reflex pack, the pages behind it and the readers the reflex engine tests share.
The pack declares three rules, one each for Korean, Japanese and Mandarin, over the stubbed pages.
The Wu pack declares one rule with a recast and superscript tones.

## `internal static async Task TReflexSettle(LEngine engine, long entryId)`

Waits until the engine reports no fetch running for the entry, or fails after the patience.

## `internal static (string, string, string, string, bool) TReflexRowRead(LReflexDraft row)`

Reads a found or stored row down to its language, kind, text, note and mark, so a test compares tuples.

## `internal static LEntryDraft TReflexDraftCreate(string headword, string language)`

A draft of the headword in the language with one bare card, enough for the entry to be saved.
