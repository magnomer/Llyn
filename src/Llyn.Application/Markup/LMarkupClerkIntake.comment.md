# LMarkupClerkIntake.cs
Hash: `a64a1951c509c025`

## `public sealed class LMarkupClerkIntake`

The import of a parsed markup cargo under the reader's intake choices.
Every entry is created or rewritten in one session and one revision, the held mentions settled at the end.

## `public LMarkupClerkIntake(LRig rig, LClaimClerk claims, LEntryClerk entries, LRevisionClerk revisions, LEntryQueryClerk query, LLacunaClerk lacunae, LFrequencyClerk frequencies, LMarkupClerkLink link, LMarkupClerkDraft draft)`

Reads the vault and the entry, meaning and mention ports out of `rig`.
Keeps the clerks the import writes through.
`revisions` stamps the one revision an import records, and `query` finds the stored entries a parsed one may join.

## `public static LMarkupIntake LMarkupIntakeCreate(int index, LMarkupMode mode, long target)`

The intake one declared row stands for, built by the Core record's factory.
The shell reaches that rule through it, so the shell calls no Core member for it.

## `public IReadOnlyList<IReadOnlyList<LMarkupTarget>> LMarkupTargetFind(IReadOnlyList<LMarkupEntry> entries)`

The stored entries each parsed entry may join, in file order, one list per entry.
A stored entry shares the parsed headword and language, as the query clerk's headword find matches them.
Each target counts its cards, so the import window shows what a replacement drops without a second read.
A stored entry is loaded through the entry clerk, so its reflex rows come in the pack's declared order.

## `public LMarkupOutcome LMarkupClerkImport(LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes)`

One intake per entry, each index once, else the import is refused.
A replace first detaches the senses that point into the entry.
A merge folds the parsed draft into the stored one.
Every entry is saved through the entry clerk with its pending inflection fetch cancelled.
The frequency fetches start once the session committed.

## `private static void LMarkupLineSet(List<LMarkupOmission> omissions, int noted, int line)`

Omissions added since `noted` that carry no line get the entry's line.

## `private void LMarkupIntakeValidate(IReadOnlyList<LMarkupIntake> intakes)`

A target that is not stored, or named twice, refuses the import.
A target held in an open entry draft refuses it as stale.

## `private IReadOnlyDictionary<int, long> LMarkupIntakePrepare(IReadOnlyList<LMarkupEntry> entries, IReadOnlyList<LMarkupIntake> intakes)`

The entry id of every index, fresh entries created bare so later entries can link to them.
A fresh entry with a blank headword refuses the import.

## `private void LMarkupSenseDetach(long entryId, List<LMarkupOmission> omissions)`

Clears the sense of every mention that points at a meaning of the entry, noting each example touched.

## `private void LMarkupMentionSettle(IReadOnlyList<(LMarkupMention, int)> held, IReadOnlyDictionary<long, long> identity, IReadOnlyDictionary<(string, string), long> prepared, List<LMarkupOmission> omissions)`

The sense of every held mention resolved now that the entries stand, a sense not found omitted.
