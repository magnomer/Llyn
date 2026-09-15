# LEngineDraftTranslation.cs

## `public sealed partial class LEngine`

The translation side of a draft commit, called from `LEngineDraftCommit` in `LEngineDraftHold.cs`.
A card's translation list holds entry ids, and some of them are draft ids of targets not yet stored.
The settle pass maps each such id through the round's identity map and strikes what still names nothing.
The court pass runs once the round is through and writes the links the walk had to hold back.
Both read and write the database only, so nothing here touches a draft file.

## `private void LEngineCourtApply(IReadOnlyDictionary<long, LDraft> loaded, IReadOnlyDictionary<long, LOutcome> settled, IReadOnlyList<LCourt> deferred)`

Writes every link the walk held back, now that every draft of the round has an entry id.
The owner's content as it was loaded says which cards carried the target's draft id.
The owner's map says which stored card each of those became.
A link whose owner or target did not settle is passed over, because there is nothing to write it against.

## `private void LEngineCourtApply(IReadOnlyList<LCardDraft> cards, LOutcome made, long draftId, long entryId, bool collocation)`

One card list of the owner walked for the target's draft id, children included.
A card that carried it has the target's entry id appended to its stored translations.

## `private static bool LEngineTranslationCheck(IReadOnlyList<long> translations, long id)`

Whether one card's translation list carries `id`.

## `private void LEngineTranslationAppend(long ownerId, long entryId, bool collocation)`

Adds `entryId` to the end of one stored card's translations, unless the card already links it.

## `private LEntryDraft LEngineTranslationSettle(LEntryDraft content, IReadOnlyDictionary<long, long> identity)`

The same content with every translation that names no stored entry removed.
Both card lists are settled, because a chip can sit on a meaning or on a collocation.

## `private IReadOnlyList<LCardDraft> LEngineTranslationSettle(IReadOnlyList<LCardDraft> cards, IReadOnlyDictionary<long, long> identity)`

Keeps only the translations that still read as a stored entry.
An id that reads nothing points at a record that was discarded or never arrived.
It may also be one still being committed in this round.
There is nothing to store it against yet, and the round's last pass writes the ones that become real.
The check is a single row read, not a load of the whole entry tree.
