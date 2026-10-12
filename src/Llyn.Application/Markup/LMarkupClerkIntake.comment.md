# LMarkupClerkIntake.cs
Hash: `a7b87b94b3d4cf6d`

## `public sealed class LMarkupClerkIntake`

The import of a parsed markup cargo under the reader's intake choices.
All entries are saved in one session and one revision, held mentions and cited references settled last.

## `public LMarkupClerkIntake(LRig rig, LClaimClerk claims, LEntryClerk entries, LRevisionClerk revisions, LEntryQueryClerk query, LLacunaClerk lacunae, LFrequencyClerk frequencies, LMarkupClerkLink link, LMarkupClerkDraft draft)`

Reads the vault and the entry, meaning, mention and example ports out of `rig`.
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
A merge or replace with a blank language keeps the stored language.
A replace with a blank headword keeps the stored headword, since the save refuses a blank one.
A merge folds the parsed draft into the stored one.
Every entry is saved through the entry clerk with its pending inflection fetch cancelled.
The frequency fetches start once the session committed.
The entry's omissions, the detach rows included, take the entry's line when they carry none.
The outcome is clamped to the omission ceiling once, so import rows never follow the `...` row.

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
Each example is named by its text, since its id is never shown to the user.
The text is cut to forty text elements, so a long sentence keeps the row on one glance.
An example can stand in several entries or none, so a headword would not name it.

## `private void LMarkupMentionSettle(IReadOnlyList<(LMarkupMention, int, long)> held, IReadOnlyDictionary<long, long> identity, IReadOnlyDictionary<(string, string), long> prepared, List<LMarkupOmission> omissions)`

The sense of every held mention resolved now that the entries stand, a sense not found omitted.
A held mention absent from `identity` was never stored, so it is skipped.

## `private void LMarkupReferenceSettle(IReadOnlyList<(LMarkupReference, long)> cited, IReadOnlyDictionary<long, long> identity)`

The source of every cited example found or created, and written onto the stored example.
An example absent from `identity` was dropped, as text-less or already present in a merge, so its reference stays untouched.
