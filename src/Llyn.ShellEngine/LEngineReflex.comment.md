# LEngineReflex.cs

## `public sealed partial class LEngine`

The reflex side of the engine.
It covers the ordered rows an Entry keeps, the readings of its characters in the languages that borrowed them.
The rows are edited like transcriptions and stored with the entry, so a fetched reading can be corrected.
The fetch that fills an empty entry sits in `LEngineReflexFetch.cs`.

## `public IReadOnlyList<long> LEngineAnchorToggle(IReadOnlyList<long> anchors, long fanqieId, bool anchored)`

The anchor list with one fanqie added or removed, kept sorted and without repeats.

## `public bool LEngineAnchorMatch(IReadOnlyList<long> one, IReadOnlyList<long> other)`

Whether two anchor lists name the same fanqie rows.

## `public IReadOnlyList<LReflexRule> LEngineReflexRead(string language)`

The fetch rules the pack of `language` declares, in written order.
An empty list means the language shows no reflex lines and nothing is fetched.

## `public IReadOnlyList<LReflex> LEngineReflexRead(long entryId)`

Reads the reflexes the Entry identified by `entryId` keeps, in order.
Empty when it keeps none.

## `internal IReadOnlyList<LReflex> LEngineReflexSet(long entryId, IReadOnlyList<LReflex> reflexes)`

Makes `reflexes` the whole list of the Entry, and returns the rows with their ids filled in.

## `private void LEngineReflexSync(`

The entry's reflexes reconciled to the draft, on a create and on an update alike.
A row blank in every text is left out, and a row with any text is stored.
Every stored row has its anatomy cut under the rules of the pack `language` names, the entry's own.
An unchanged list writes nothing and raises no change.
A list differing only in anatomy, as after a rule edit, is stored but raises no change.
A positive id naming no stored row of this entry refuses the commit rather than rebinding to a fresh row.
Each minted id is recorded against the draft id it replaces.

## `private static IReadOnlyList<LReflexDraft> LEngineReflexScan(IReadOnlyList<LReflexDraft> drafts)`

The draft rows worth storing, every row that carries any text.

## `private static IReadOnlyList<LReflex> LEngineReflexRead(long entryId, IReadOnlyList<LReflexDraft> drafts)`

The draft rows as the rows the archive stores, positions from list order, anchors carried across.

## `private static bool LEngineReflexMatch(IReadOnlyList<LReflex> stored, IReadOnlyList<LReflex> current)`

Whether two stored lists are the same rows in the same order.
The anchors are compared by position apart, since a record compares its list by reference.

## `private static bool LEngineReflexMatch(IReadOnlyList<LReflexDraft> one, IReadOnlyList<LReflexDraft> other)`

Whether two draft lists hold the same rows once blank rows are dropped, for the dirty check of a draft.
The anchors are compared by position apart, so a tick alone dirties the draft.

## `private static IReadOnlyList<LReflexDraft> LEngineReflexReset(IReadOnlyList<LReflexDraft> drafts)`

The rows with every stored id dropped, for a draft saved as a new entry.

## `private IReadOnlyList<LReflexDraft> LEngineReflexNormalize(IReadOnlyList<LReflexDraft> drafts)`

Mints an id for every unnamed row and drops an unnamed row that is blank.

## `private static string LEngineReflexFormat(IReadOnlyList<LReflex> reflexes)`

The list as words, for the revision change text.

## `private static IEnumerable<string> LReflexPartScan(LReflex reflex)`

The language, kind, text and note of one row, those that are filled.
