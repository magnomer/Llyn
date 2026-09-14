# LEngineInflectionFetch.cs

## `public sealed partial class LEngine`

The inflection fetch side of the engine.
It fills the unspecified slots of an Entry's paradigm from the morphology sources its language pack names.
A form found is stored once as an inflection, so the next read finds it specified.
A form a reached source could not name is remembered for the session, so its slot reads unknown.
An Entry no source could be reached for is remembered as lost, so the session does not ask again.
Both memories go when the Entry's inflections are rewritten, when the switch turns off, or when the workspace changes.
The fetch runs in the background and never blocks the caller that asked for it.
Unlike the frequency fill, a paradigm read never starts a fetch by itself, because the caller decides when to ask.

## `private IReadOnlyList<LSource> LEngineInflectionLoad(string language)`

The built morphology sources of one language, made once and kept for the engine's life.
A pack without a `morphology` list yields no sources, so every fetch for that language ends at once.
An Entry with no language named has no pack, so it yields no sources either.

## `private async Task<(IReadOnlyDictionary<string, string> LInflectionFound, bool LInflectionReached)> LEngineInflectionScan(string word, string language, CancellationToken cancellation)`

Asks every morphology source of `language` for `word` and merges the readings by variety.
The variety of a morphology reading is the morphology value code, so the map runs from code to form.
Each source is called directly rather than through a lookup, since one answer carries several readings.
The first source naming a code wins, and a later source only adds codes still missing.
The second value says whether any source was reached at all.

## `public bool LEngineInflectionCheck(long entryId)`

Reports whether a fetch is running for the Entry identified by `entryId`.
The reading view asks this after starting.
A slot still empty can then say it is being looked up rather than lost.

## `public void LEngineInflectionStart(long entryId)`

Begins a background fetch for the Entry identified by `entryId` and returns at once.
Nothing starts when the setting is off, a fetch is already running, or the Entry was lost this session.
Nothing starts while a held draft edits the Entry, since a save would overwrite what the fetch stored.
Nothing starts when the Entry is missing, the pack has no sources, or no slot is unspecified.
An Entry whose parts declare no paradigm has no slots, so it starts nothing either.
A slot already marked unknown is not asked again, since the source has already answered it.
The gate holds the pending map, so the same Entry never fetches twice at once.

## `private void LEngineInflectionReset(long entryId)`

Cancels a pending fetch of one Entry and forgets its misses and its loss.
Called under the gate whenever the Entry's inflections are rewritten by hand or by a save.
What the user wrote replaces what the source said, so the source may be asked again later.

## `private void LEngineInflectionClear()`

Cancels every pending fetch and forgets every miss and loss.
Called under the gate when the workspace changes, the switch turns off, or the engine is disposed.
A fetch begun against one workspace must never write into the next.

## `private LDraft? LEngineDraftFind(long entryId)`

Finds the held draft editing the Entry identified by `entryId`, or `null` when none does.
The held drafts are few, so each is read from the archive in turn.
Called under the gate.

## `private void LEngineInflectionApply(LEntry entry, IReadOnlyDictionary<string, string> found)`

Writes what a reached fetch answered into the Entry's slots, under the gate.
The slots are read again, so a form the user typed meanwhile is never doubled.
Each unspecified slot whose code was found becomes one inflection carrying the form, the slot's part, and the slot's morphology.
The inflections are appended in slot order behind whatever the Entry already holds.
Every unspecified slot not found is remembered as a miss by its morphology id, replacing the earlier set.

## `private async Task LEngineInflectionRun(LEntry entry, CancellationTokenSource fetch)`

The fetch itself, run outside the gate for the whole scan.
The answer is dropped when the fetch was cancelled or the setting turned off meanwhile.
It is dropped too when the Entry is gone, renamed, moved to another language, or now held by a draft.
A reached answer is applied through `LEngineInflectionApply`.
An answer no source was reached for marks the Entry lost instead.
Every exception is swallowed, since a missing form is not an error the user can act on.
The pending mark is dropped only when it is still this fetch's own.
So a newer fetch is never unmarked by an older one.
The bulletin is raised outside the gate whenever the fetch finished, found or not, so the view redraws its slots.
