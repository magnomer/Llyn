# LCardFacade.cs
Hash: `a1a9b4e2d8b0a8f3`

## `public sealed class LCardFacade : LCardPort`

Card storage access uses the hearth's shared gate, keeping the shell behind the engine's clerks.
Only Entry owners may read Meanings through the owner overload.
The facade implements `LCardPort` directly, so its consumer needs no intervening outlet.

## `internal LCardFacade(LEngineHearth hearth, LPronunciationFacade pronunciation, LSettingsFacade settings, LVistaRowFacade row)`

The hearth supplies the shared gate and current staff.
Sibling facades are explicit dependencies rather than an unrestricted engine reference.

## `public IReadOnlyList<LMeaning> LEngineMeaningRead(long ownerId, LOwner owner)`

Meanings belong only to Entries, so another owner is rejected before storage is read.

## `public IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)> LEngineMeaningRead(long entryId, string key)`

`LMeaningClerkSort` owns reading order and fallback naming.
The caller supplies the fallback localization key, while the settings facade supplies its wording.

## `public IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)>? LEngineSenseRead(LTenure held, long card, long sentence, string text, int start, int length, string key)`

A sense menu requires a linked Mention under the selection.
Without one, null prevents a menu from opening.

## `public LTranslationOffer LEngineTranslationFind(LTenure held, string text, string word, bool chosen)`

A card cannot translate its own stored Entry, so that row is excluded from the offer.
Exclusion follows vista construction, preserving the numbering assigned before filtering.
Workspace languages supply creation choices, with the held draft's language last.

## `public IReadOnlyList<LVistaRow> LEngineProspectFind(LTenure held, string word)`

The held draft supplies mention-language context, so callers need not pass a separate language.
The tenure protects the draft read before the shared storage gate is taken.

## `public LEntry? LEngineTranslationResolve(string word, long? entryId)`

Only one normalized whole-headword match can resolve a translation.
No match or ambiguity answers null, and the supplied Entry is excluded.

## `internal LEntry LEngineTranslationCreate(string headword, string language)`

A committed translation stub receives frequency work and an entry bulletin.
The bulletin leaves the shared gate before notifying readers.

## `public IReadOnlyList<LTranslationTarget> LEngineTargetRead(IReadOnlyList<long> ids)`

This overload reads stored targets without any draft's court links.
Tentative targets therefore require the owner overload.

## `public IReadOnlyList<LTranslationTarget> LEngineEtymonRead(LEntryDraft draft)`

Etymons retain the draft's link order.
Missing stored targets are omitted, preventing blank chips.

## `public LEtymologyResult LEngineEtymologyRead(LEntryDraft draft)`

Source links and the draft's narrative verdict form one answer.
Link-read failures propagate to the caller's failure boundary.

## `public IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> LEngineTranslationRead(LEntryDraft draft)`

Every top-level meaning and collocation receives a key, even without translation targets.
Distinct target ids share one stored read, omitted when no ids exist.
Storage failures propagate to the caller's failure boundary.

## `internal static IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> LEngineTranslationResolve(IReadOnlyList<LCardDraft> cards, IReadOnlyList<LTranslationTarget> read)`

Each card retains its link order, omitting targets absent from the supplied read.
The supplied cards receive dictionary keys even when no links resolve.

## `public IReadOnlyList<LTranslationTarget> LEngineTargetRead(long ownerId, IReadOnlyList<long> ids)`

The owner's court links supplement stored targets, allowing tentative targets to be named.

## `public IReadOnlyList<LUsage> LEngineIncomingRead(long entryId)`

Both incoming card kinds use the hearth's current epithet setting.

## `internal static ArgumentOutOfRangeException LEngineOwnerRaise(LOwner owner)`

Unsupported owners share one exception shape.
Returning the exception lets switch arms throw it directly.

## `public IReadOnlySet<long> LEngineFoldRead(long entryId)`

An entry-scoped read covers folded cards of both kinds under the shared gate.

## `public void LEngineFoldSave(long entryId, long cardId)`

Folding bypasses the held draft, so it cannot dirty lexical data or enter undo.
Normal clerk completion raises a fold bulletin for `entryId` after releasing the gate.
Even an ignored nonpositive card id raises that bulletin.
Other views of the same entry can re-read their folds.

## `public void LEngineFoldDelete(long entryId, long cardId)`

Unfolding uses the same entry-scoped bulletin and leaves the held draft untouched.
Normal clerk completion announces it after releasing the gate, even when a nonpositive card id was ignored.
