# LCardFacade.cs

## `internal sealed class LCardFacade`

The engine's facade for cards, over Meanings, Collocations, Tags, Registers and Translations.
Every call takes the gate and hands the work to the clerk that owns the rows.
The facade stays because the shell calls the engine, and the engine alone holds the gate and the observers.
Which side an id names arrives as an `LOwner` rather than in the method's name.
A name carries three components after its prefix, and a method per side would need four.
A Tag, a Register or a link hangs from a Meaning or a Collocation and from nothing else.
So the facade turns the owner into the flag the clerks take, and any other side is refused here.

## `public LCardFacade(LEngine engine)`

Stores the engine and its shared gate for this facade.

## `public IReadOnlyList<LMeaning> LEngineMeaningRead(long ownerId, LOwner owner)`

Reads the Meanings of the Entry identified by `ownerId`, in stored order.
Only an Entry holds Meanings, so any other owner is refused.

## `public IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)> LEngineMeaningRead(`

Reads the Meanings of the Entry identified by `entryId`, ready in reading order.
The clerk's `LMeaningClerkSort` owns the order and the name fallback.
`key` is the localization key of that fallback, which the caller chooses and the engine words.

## `internal IReadOnlyList<LTag> LEngineTagRead()`

Reads every Tag the workspace holds, once each, in alphabetical order.

## `public IReadOnlyList<LTag> LEngineTagFind(string query, LCatalogOrder order)`

The stored Tags answering `query`, in `order`, as the taxonomy vista lists them.

## `public IReadOnlyList<LTag> LEngineTagFind(LTenure? held, long card, string query, LCatalogOrder order)`

The stored Tags a card's field offers, leaving out those the held draft's card already shows.
The draft is read before the gate is taken, as the tenure guards itself.

## `public IReadOnlyList<LTag> LEngineTagFind(LVista vista)`

The tags the taxonomy panel's vista lists, with the query and order read off the vista.
The vista's filter hides languages from the entries of the chosen tag, not tags, so it is not applied here.
A vista whose chosen tag no longer answers is deselected.

## `public LTag LEngineTagCreate(string text)`

The tag clerk's create, then the tag bulletin raised outside the gate.
The catalog is announced so every panel listing Tags shows the new row.
The rename and the delete raise the same bulletin the same way.

## `public IReadOnlyList<LRegister> LEngineRegisterFind(LTenure? held, long card, string query, string language)`

The stored Registers a card's field offers in `language`, leaving out those the held draft's card links.

## `public IReadOnlyList<LCatalogRegister> LEngineRegisterFind(LVista vista)`

The shelf the tenor panel's vista lists, with the query and order read off the vista.
The vista's filter hides languages from the entries of the chosen Register, not Registers, so it is not applied here.

## `public LRegister LEngineRegisterCreate(string name)`

The register clerk's create, then the register bulletin raised outside the gate.
The announcement is raised either way, so the panel lists and selects the row.
The rename and the delete raise the same bulletin whether or not the row moved.
So a shown panel always re-reads.

## `public IReadOnlyList<LVistaRow> LEngineProspectFind(string query)`

The translation search built into vista rows, for the prospect list under a card.

## `internal LEntry LEngineTranslationCreate(string headword, string language)`

The translation clerk's stub create under the gate.
A frequency fill starts for it after the commit, exactly as it does for a saved draft.
The making is announced as an entry bulletin once the lock is released.
The library list then shows the stub at once.

## `public IReadOnlyList<LTranslationTarget> LEngineEtymonRead(LEntryDraft draft)`

The source links of the draft's etymology, named and in the draft's own order.
A link whose entry is gone is left out, so a chip is never drawn blank.

## `public (IReadOnlyList<LTranslationTarget>, bool) LEngineEtymologyRead(LEntryDraft draft)`

The draft's named source links, and whether its etymology has anything to show.
It shows with a narrative that holds words or with a link whose entry still stands.

## `public IReadOnlyList<LTranslationTarget> LEngineTargetRead(LEntryDraft draft)`

The targets of the draft's link ids, so a card resolves its chips in one read.

## `public IReadOnlyList<LTranslationTarget> LEngineTargetRead(long ownerId, IReadOnlyList<long> ids)`

The targets of `ids` as the draft `ownerId` sees them.
The engine reads the draft's court links and hands them to the clerk, which answers a tentative target from them.

## `public IReadOnlyList<LUsage> LEngineIncomingRead(long entryId)`

Every card of either kind that links to the Entry, with the epithet the settings ask for.
