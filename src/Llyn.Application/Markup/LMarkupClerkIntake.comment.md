# LMarkupClerkIntake.cs

## `public sealed class LMarkupClerkIntake`

The import of a parsed markup cargo under the reader's intake choices.
Every entry is created or rewritten in one session and one revision, the held mentions settled at the end.

## `public LMarkupClerkIntake(LRig rig, LClaimClerk claims, LEntryClerk entries, LLacunaClerk lacunae, LFrequencyClerk frequencies, LMarkupClerkLink link, LMarkupClerkDraft draft)`

Reads the vault and the entry, meaning and mention ports out of `rig`.
Keeps the clerks the import writes through.

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
