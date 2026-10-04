# LCardFacade.cs
Hash: `6f3d0b6c7b94ffc9`

## `internal sealed class LCardFacade`

The engine's facade for cards, over Meanings, Collocations, Tags, Registers and Translations.
A call that reaches a clerk takes the gate and hands the work to the clerk owning the rows.
The facade stays because the shell calls the engine, and the engine alone holds the gate and the observers.
Which side an id names arrives as an `LOwner` rather than in the method's name.
Only an Entry holds Meanings, so any other side is refused here.

## `public LCardFacade(LEngine engine)`

Stores the engine and its shared gate for this facade.

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

## `internal IReadOnlyList<LTag> LEngineTagRead()`

Reads every Tag the workspace holds, once each, in alphabetical order.

## `public IReadOnlyList<LTag> LEngineTagFind(string query, LCatalogOrder order)`

The stored Tags answering `query`, in `order`, as the taxonomy vista lists them.

## `public LTagOffer LEngineTagFind(LTenure held, long card, string text)`

The stored Tags a card's tag field offers for the text it keeps.
Those the held draft's card already shows are left out.
The draft is read before the gate is taken, as the tenure guards itself.

## `public IReadOnlyList<LCatalogTag> LEngineTagFind(LVista vista)`

The tags the taxonomy panel's vista lists, with the query and order read off the vista.
The vista's filter hides languages from the entries of the chosen tag, not tags, so it is not applied here.
A vista whose chosen tag no longer answers is deselected.

## `public LTag LEngineTagCreate(string text)`

The tag clerk's create, then the tag bulletin raised outside the gate.
The catalog is announced so every panel listing Tags shows the new row.

## `public LRegisterOffer LEngineRegisterFind(LTenure held, long card, string text)`

The stored Registers a card's register field offers for the text it keeps.
Those the held draft's card already links are left out.
The language whose pack seeds the shelf is the held draft's own, so no caller passes one.
The draft is read before the gate is taken, as the tenure guards itself.

## `public IReadOnlyList<LCatalogRegister> LEngineRegisterFind(LVista vista)`

The shelf the tenor panel's vista lists, with the query and order read off the vista.
The vista's filter hides languages from the entries of the chosen Register, not Registers, so it is not applied here.
A vista whose chosen Register no longer answers is deselected.

## `public LRegister LEngineRegisterCreate(string name)`

The register clerk's create, then the register bulletin raised outside the gate.
The announcement is raised after the lock is released, so the panel lists and selects the row.

## `public LTranslationOffer LEngineTranslationFind(LTenure held, string text, string word, bool chosen)`

The offer a typed translation opens, ready for the dropdown under a card.
The search is built into vista rows, and the held draft's own stored Entry is dropped afterwards.
So it joins the numbering first, and its twin keeps the number the catalog shows.
A card may not translate its own Entry, so that row is never offered.
The fresh Entry languages come from the languages the workspace holds, with the draft's own last.

## `public IReadOnlyList<LVistaRow> LEngineProspectFind(LTenure held, string word)`

The Entries a mention of `word` in the held draft may name, built into vista rows.
The held draft names the language, so no caller passes one.
The draft is read before the gate is taken, as the tenure guards itself.

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
