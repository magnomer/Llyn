# LCardFacade.cs
Hash: `f5c6221f3767c9f1`

## `public sealed class LCardFacade : LCardPort`

The engine's facade for cards, over Meanings, Collocations and Translations.
A call that reaches a clerk takes the gate and hands the work to the clerk owning the rows.
The facade stays because the shell calls the engine, and the engine alone holds the gate and the observers.
Which side an id names arrives as an `LOwner` rather than in the method's name.
Only an Entry holds Meanings, so any other side is refused here.
Tags and Registers live in `LCatalogFacade`, beside favorites.
It implements the card port itself, so Host hands it to Conduct with no outlet between.

## `internal LCardFacade(LEngineHearth hearth, LPronunciationFacade pronunciation, LSettingsFacade settings, LVistaRowFacade row)`

Stores the hearth, its gate and the sibling facades it calls, all built by `LEngine` before this one.
The gate, the staff and the shared state are read through the hearth.
It takes its siblings rather than the engine, so it names only the facades it uses.

## `public IReadOnlyList<LMeaning> LEngineMeaningRead(long ownerId, LOwner owner)`

Reads the Meanings of the Entry identified by `ownerId`, in stored order.
Only an Entry holds Meanings, so any other owner is refused.

## `public IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)> LEngineMeaningRead(long entryId, string key)`

Reads the Meanings of the Entry identified by `entryId`, ready in reading order.
The clerk's `LMeaningClerkSort` owns the order and the name fallback.
`key` is the localization key of that fallback, which the caller chooses and the engine words.

## `public IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)>? LEngineSenseRead(LTenure held, long card, long sentence, string text, int start, int length, string key)`

The Meanings a sense menu offers for the linked Mention under a sentence field's selection.
The held draft finds the Mention first, then its Entry's Meanings are read in reading order.
No linked Mention under the selection answers null, so no menu opens.

## `public LTranslationOffer LEngineTranslationFind(LTenure held, string text, string word, bool chosen)`

The offer a typed translation opens, ready for the dropdown under a card.
The search is built into vista rows, and the held draft's own stored Entry is dropped afterwards.
So it joins the numbering first, and its twin keeps the number the catalog shows.
A card may not translate its own Entry, so that row is never offered.
The fresh Entry languages come from the languages the workspace holds, with the draft's own last.
Those are read from the language clerk under the gate, so the card facade needs no language facade.

## `public IReadOnlyList<LVistaRow> LEngineProspectFind(LTenure held, string word)`

The Entries a mention of `word` in the held draft may name, built into vista rows.
The held draft names the language, so no caller passes one.
The draft is read before the gate is taken, as the tenure guards itself.

## `public LEntry? LEngineTranslationResolve(string word, long? entryId)`

The one Entry whose whole headword reads as `word`, or null when none or several do, under the gate.
The Entry `entryId` names is left out, because a card may not translate its own Entry.

## `internal LEntry LEngineTranslationCreate(string headword, string language)`

The translation clerk's stub create under the gate.
A frequency fill starts for it after the commit, exactly as it does for a saved draft.
The making is announced as an entry bulletin once the lock is released.
The library list then shows the stub at once.

## `public IReadOnlyList<LTranslationTarget> LEngineTargetRead(IReadOnlyList<long> ids)`

Stored Entries only, read without any draft's court links.
So a tentative id reads as nothing here, where the owner overload would name it.

## `public IReadOnlyList<LTranslationTarget> LEngineEtymonRead(LEntryDraft draft)`

The source links of the draft's etymology, named and in the draft's own order.
A link whose entry is gone is left out, so a chip is never drawn blank.

## `public LEtymologyResult LEngineEtymologyRead(LEntryDraft draft)`

The draft's named source links and whether its narrative holds words, as one record.
The narrated verdict is read once here, from the etymology draft's own rule.
A failed link read is not caught here.
It travels up to the reading view's gate.

## `public IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> LEngineTranslationRead(LEntryDraft draft)`

The link targets of every card of a stored entry, keyed by card id, in one read.
A failed read is not caught here.
It travels up to the reading view's gate.

## `internal static IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> LEngineTranslationResolve(IReadOnlyList<LCardDraft> cards, IReadOnlyList<LTranslationTarget> read)`

Pairs each card with the targets of its links, in the order the card lists them.
A link the read did not find is left out, so a chip row never shows a blank.
The editor's tenure and the reading view share it, so both keep one rule.

## `public IReadOnlyList<LTranslationTarget> LEngineTargetRead(long ownerId, IReadOnlyList<long> ids)`

The targets of `ids` as the draft `ownerId` sees them.
The engine reads the draft's court links and hands them to the clerk, which answers a tentative target from them.

## `public IReadOnlyList<LUsage> LEngineIncomingRead(long entryId)`

Every card of either kind that links to the Entry, with the epithet the settings ask for.

## `internal static ArgumentOutOfRangeException LEngineOwnerRaise(LOwner owner)`

The one failure for a side an entity has no association table for.
Returned rather than thrown so a switch arm can throw it.
The card and portrait facades both throw it.
