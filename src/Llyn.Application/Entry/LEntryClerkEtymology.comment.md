# LEntryClerkEtymology.cs

## `public static class LEntryClerkEtymology`

The seam that writes one entry's etymology when the entry is saved.
It is where the two shapes are decided between, since only one of them may be stored.

## `public static void LEtymologyUpdate(LEtymologyVault etymologies, long entryId, LEntryDraft draft, List<LRevisionChange>? changes)`

Writes the draft's etymology and clears whichever shape the draft did not end in.
A narrative wins when the prose is written, otherwise the links win when there are any.
A draft that says nothing clears both, and a draft matching the store writes nothing at all.
The change is recorded on `changes`, which a creating save passes as null.

## `public static LEtymologyDraft LEtymologyRead(LEtymologyVault etymologies, long entryId)`

The stored etymology of one entry as a draft, both shapes read together.

## `public static bool LEtymologyMatch(LEtymologyDraft one, LEtymologyDraft other)`

Whether two etymology drafts say the same thing.
The prose is compared trimmed, and the spans and links in their order.
Span ids are not compared, since a save mints them fresh each time.
